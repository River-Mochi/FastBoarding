// File: System/LateBoarderCancelSystem.cs
// Purpose: Experimental system that lets solo late passengers miss a departing transit vehicle.

namespace BoardingTime
{
    using Game;
    using Game.Common;
    using Game.Creatures;
    using Game.Pathfind;
    using Game.Simulation;
    using Game.Tools;
    using Game.Vehicles;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Unity.Collections;
    using Unity.Entities;
    using PrefabRef = Game.Prefabs.PrefabRef;
    using PublicTransportVehicleData = Game.Prefabs.PublicTransportVehicleData;
    using TransportType = Game.Prefabs.TransportType;

    public partial class LateBoarderCancelSystem : GameSystemBase
    {
        public const int UpdatesPerDay = 4096;
        private const int MaxCancellationsPerUpdate = 64;
        private const uint DiagnosticFrameInterval = 4096;

        private EntityQuery m_VehicleQuery;
        private SimulationSystem? m_SimulationSystem;
        private ToolSystem? m_ToolSystem;
        private DefaultToolSystem? m_DefaultToolSystem;
        private uint m_LastDiagnosticFrame;
        private long m_TotalCanceled;
        private int m_SkippedForTool;
        private bool m_LoggedActive;

        private readonly struct PassStats
        {
            public PassStats(int vehicles, int passengers, int candidates, int canceled)
            {
                Vehicles = vehicles;
                Passengers = passengers;
                Candidates = candidates;
                Canceled = canceled;
            }

            public int Vehicles { get; }

            public int Passengers { get; }

            public int Candidates { get; }

            public int Canceled { get; }
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            // CS2 uses 262144 simulation frames per in-game day.
            return 262144 / UpdatesPerDay;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            m_SimulationSystem = World.GetOrCreateSystemManaged<SimulationSystem>();
            m_ToolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            m_DefaultToolSystem = World.GetOrCreateSystemManaged<DefaultToolSystem>();
            m_VehicleQuery = SystemAPI.QueryBuilder()
                .WithAll<Game.Vehicles.PublicTransport, Passenger>()
                .WithNone<Deleted, Destroyed, Temp, Overridden>()
                .Build();
            RequireForUpdate(m_VehicleQuery);
        }

        protected override void OnUpdate()
        {
            try
            {
                if (!BoardingRuntimeSettings.CancelLateBoarders)
                {
                    // The Options UI setter wakes this system only when the toggle is enabled.
                    Enabled = false;
                    return;
                }

                LogActiveOnce();

                if (IsPlayerUsingTool())
                {
                    // Avoid racing user edit tools such as bulldoze/net/object placement.
                    m_SkippedForTool++;
                    LogPassSummary(
                        m_SimulationSystem?.frameIndex ?? 0,
                        new PassStats(0, 0, 0, 0),
                        "paused-tool");
                    return;
                }

                PassStats stats = RunCancellationPass();
                LogPassSummary(m_SimulationSystem?.frameIndex ?? 0, stats, "pass");
            }
            catch (Exception ex)
            {
                Enabled = false;
                BoardingRuntimeSettings.SetCancelLateBoarders(false);

                Mod.WarnOnce(
                    "BT_LATE_BOARDER_CANCEL_EXCEPTION",
                    () => $"[BT] Cancel late boarders disabled after {ex.GetType().Name}: {ex.Message}");
            }
        }

        private PassStats RunCancellationPass()
        {
            NativeArray<Entity> vehicles = default;
            EntityCommandBuffer ecb = default;
            bool hasCommandBuffer = false;
            int cancellationsThisUpdate = 0;
            int vehiclesScanned = 0;
            int passengersScanned = 0;
            int candidates = 0;
            int busCanceled = 0;
            int trainCanceled = 0;
            int tramCanceled = 0;
            int subwayCanceled = 0;
            int shipCanceled = 0;
            int ferryCanceled = 0;
            int airCanceled = 0;

            try
            {
                // Finish readers/writers for this query before we inspect buffers on the main thread.
                m_VehicleQuery.CompleteDependency();
                vehicles = m_VehicleQuery.ToEntityArray(Allocator.Temp);
                ecb = new EntityCommandBuffer(Allocator.Temp);
                hasCommandBuffer = true;

                foreach (var vehicleEntity in vehicles)
                {
                    vehiclesScanned++;

                    if (!EntityManager.Exists(vehicleEntity) ||
                        EntityManager.HasComponent<Destroyed>(vehicleEntity) ||
                        EntityManager.HasComponent<Overridden>(vehicleEntity))
                    {
                        continue;
                    }

                    var publicTransport = EntityManager.GetComponentData<Game.Vehicles.PublicTransport>(vehicleEntity);
                    if ((publicTransport.m_State & PublicTransportFlags.Boarding) == 0)
                    {
                        // Only intervene while vanilla says the vehicle is in a boarding state.
                        continue;
                    }

                    if ((publicTransport.m_State & (PublicTransportFlags.Evacuating | PublicTransportFlags.PrisonerTransport)) != 0)
                    {
                        continue;
                    }

                    if (m_SimulationSystem == null ||
                        m_SimulationSystem.frameIndex < publicTransport.m_DepartureFrame)
                    {
                        // Before the vanilla departure frame, passengers are not late yet.
                        continue;
                    }

                    var passengers = EntityManager.GetBuffer<Passenger>(vehicleEntity);
                    if (!passengers.IsCreated || passengers.Length == 0)
                    {
                        continue;
                    }

                    TransportType transportType = GetTransportType(vehicleEntity);

                    // Collect first, then mutate afterward, so we do not edit the passenger buffer
                    // while we are still scanning it for late boarders.
                    var pendingCancellation = new HashSet<Entity>();

                    for (var i = 0; i < passengers.Length; i++)
                    {
                        passengersScanned++;
                        var passenger = passengers[i].m_Passenger;
                        if (!EntityManager.Exists(passenger) ||
                            EntityManager.HasComponent<Deleted>(passenger) ||
                            EntityManager.HasComponent<Destroyed>(passenger) ||
                            EntityManager.HasComponent<Temp>(passenger) ||
                            EntityManager.HasComponent<Overridden>(passenger) ||
                            !EntityManager.HasComponent<CurrentVehicle>(passenger))
                        {
                            continue;
                        }

                        var currentVehicle = EntityManager.GetComponentData<CurrentVehicle>(passenger);
                        if (currentVehicle.m_Vehicle != vehicleEntity)
                        {
                            continue;
                        }

                        if ((currentVehicle.m_Flags & CreatureVehicleFlags.Ready) != 0)
                        {
                            continue;
                        }

                        if (CanSafelyCancelPassenger(vehicleEntity, passenger))
                        {
                            pendingCancellation.Add(passenger);
                            candidates++;
                        }
                    }

                    int canceledForVehicle = 0;
                    var canceledPassengers = new HashSet<Entity>();
                    foreach (var passenger in pendingCancellation)
                    {
                        if (cancellationsThisUpdate >= MaxCancellationsPerUpdate)
                        {
                            break;
                        }

                        if (QueuePassengerCancellation(ref ecb, vehicleEntity, passenger))
                        {
                            canceledPassengers.Add(passenger);
                            cancellationsThisUpdate++;
                            canceledForVehicle++;
                        }
                    }

                    if (canceledForVehicle > 0)
                    {
                        QueueVehiclePassengerBuffer(ref ecb, vehicleEntity, passengers, canceledPassengers);
                        AccumulateCanceledCount(
                            transportType,
                            canceledForVehicle,
                            ref busCanceled,
                            ref trainCanceled,
                            ref tramCanceled,
                            ref subwayCanceled,
                            ref shipCanceled,
                            ref ferryCanceled,
                            ref airCanceled);
                    }

                    if (cancellationsThisUpdate >= MaxCancellationsPerUpdate)
                    {
                        break;
                    }
                }

                ecb.Playback(EntityManager);
                // Update UI counters only after all queued ECS edits have succeeded.
                RecordCanceledCounts(
                    busCanceled,
                    trainCanceled,
                    tramCanceled,
                    subwayCanceled,
                    shipCanceled,
                    ferryCanceled,
                    airCanceled);

                return new PassStats(vehiclesScanned, passengersScanned, candidates, cancellationsThisUpdate);
            }
            finally
            {
                if (hasCommandBuffer)
                {
                    ecb.Dispose();
                }

                if (vehicles.IsCreated)
                {
                    vehicles.Dispose();
                }
            }
        }

        private bool IsPlayerUsingTool()
        {
            if (m_ToolSystem == null || m_DefaultToolSystem == null)
            {
                return false;
            }

            ToolBaseSystem? activeTool = m_ToolSystem.activeTool;
            return activeTool != null && activeTool != m_DefaultToolSystem;
        }

        [Conditional("DEBUG")]
        private void LogActiveOnce()
        {
            if (m_LoggedActive)
            {
                return;
            }

            m_LoggedActive = true;
            LogUtils.Info(
                Mod.s_Log,
                () => $"Cancel late boarders active. interval={GetUpdateInterval(SystemUpdatePhase.GameSimulation)} frames, cap={MaxCancellationsPerUpdate} per update. {BoardingRuntimeSettings.DescribeForLog()}");
        }

        [Conditional("DEBUG")]
        private void LogPassSummary(uint frame, PassStats stats, string reason)
        {
            m_TotalCanceled += stats.Canceled;

            bool force = stats.Canceled >= MaxCancellationsPerUpdate;
            bool intervalElapsed = m_LastDiagnosticFrame == 0 ||
                frame < m_LastDiagnosticFrame ||
                frame - m_LastDiagnosticFrame >= DiagnosticFrameInterval;

            if (!force && !intervalElapsed && stats.Canceled == 0)
            {
                return;
            }

            if (!force && !intervalElapsed)
            {
                return;
            }

            m_LastDiagnosticFrame = frame;
            string activeTool = m_ToolSystem?.activeTool?.GetType().Name ?? "none";
            LogUtils.Info(
                Mod.s_Log,
                () => $"LateBoarder {reason}: frame={frame}, vehicles={stats.Vehicles}, passengers={stats.Passengers}, candidates={stats.Candidates}, canceled={stats.Canceled}, total={m_TotalCanceled}, skippedTool={m_SkippedForTool}, activeTool={activeTool}, {BoardingRuntimeSettings.DescribeForLog()}");
        }

        private bool CanSafelyCancelPassenger(Entity vehicleEntity, Entity passenger)
        {
            if (EntityManager.HasComponent<GroupMember>(passenger) ||
                EntityManager.HasBuffer<GroupCreature>(passenger))
            {
                // Group boarding has extra leader/member rules. Keep the first beta pass solo-only.
                return false;
            }

            if (!EntityManager.HasComponent<Game.Creatures.Resident>(passenger) ||
                !EntityManager.HasComponent<Human>(passenger) ||
                !EntityManager.HasComponent<PathOwner>(passenger) ||
                !EntityManager.HasBuffer<PathElement>(passenger))
            {
                return false;
            }

            var pathOwner = EntityManager.GetComponentData<PathOwner>(passenger);
            var pathElements = EntityManager.GetBuffer<PathElement>(passenger);
            int startIndex = Math.Max(0, pathOwner.m_ElementIndex);
            for (var i = startIndex; i < pathElements.Length; i++)
            {
                if (pathElements[i].m_Target == vehicleEntity)
                {
                    return true;
                }
            }

            return false;
        }

        private static void AccumulateCanceledCount(
            TransportType transportType,
            int count,
            ref int busCanceled,
            ref int trainCanceled,
            ref int tramCanceled,
            ref int subwayCanceled,
            ref int shipCanceled,
            ref int ferryCanceled,
            ref int airCanceled)
        {
            switch (transportType)
            {
                case TransportType.Bus:
                    busCanceled += count;
                    break;
                case TransportType.Train:
                    trainCanceled += count;
                    break;
                case TransportType.Tram:
                    tramCanceled += count;
                    break;
                case TransportType.Subway:
                    subwayCanceled += count;
                    break;
                case TransportType.Ship:
                    shipCanceled += count;
                    break;
                case TransportType.Ferry:
                    ferryCanceled += count;
                    break;
                case TransportType.Airplane:
                    airCanceled += count;
                    break;
            }
        }

        private void RecordCanceledCounts(
            int busCanceled,
            int trainCanceled,
            int tramCanceled,
            int subwayCanceled,
            int shipCanceled,
            int ferryCanceled,
            int airCanceled)
        {
            TransitWaitStatus.RecordLateBoardersCanceled(World, TransportType.Bus, busCanceled);
            TransitWaitStatus.RecordLateBoardersCanceled(World, TransportType.Train, trainCanceled);
            TransitWaitStatus.RecordLateBoardersCanceled(World, TransportType.Tram, tramCanceled);
            TransitWaitStatus.RecordLateBoardersCanceled(World, TransportType.Subway, subwayCanceled);
            TransitWaitStatus.RecordLateBoardersCanceled(World, TransportType.Ship, shipCanceled);
            TransitWaitStatus.RecordLateBoardersCanceled(World, TransportType.Ferry, ferryCanceled);
            TransitWaitStatus.RecordLateBoardersCanceled(World, TransportType.Airplane, airCanceled);
        }

        private bool QueuePassengerCancellation(ref EntityCommandBuffer ecb, Entity vehicleEntity, Entity passenger)
        {
            if (!EntityManager.HasComponent<CurrentVehicle>(passenger) ||
                !EntityManager.HasComponent<Game.Creatures.Resident>(passenger) ||
                !EntityManager.HasComponent<Human>(passenger) ||
                !EntityManager.HasComponent<PathOwner>(passenger) ||
                !EntityManager.HasBuffer<PathElement>(passenger))
            {
                return false;
            }

            var pathOwner = EntityManager.GetComponentData<PathOwner>(passenger);
            var pathElements = EntityManager.GetBuffer<PathElement>(passenger);
            int startIndex = Math.Max(0, pathOwner.m_ElementIndex);
            int vehiclePathIndex = -1;
            for (var i = startIndex; i < pathElements.Length; i++)
            {
                if (pathElements[i].m_Target == vehicleEntity)
                {
                    vehiclePathIndex = i;
                    break;
                }
            }

            if (vehiclePathIndex < 0)
            {
                return false;
            }

            ecb.RemoveComponent<CurrentVehicle>(passenger);

            // Clear "in vehicle" state so the cim is treated as no longer attached to this vehicle.
            var resident = EntityManager.GetComponentData<Game.Creatures.Resident>(passenger);
            resident.m_Flags &= ~ResidentFlags.InVehicle;
            resident.m_Timer = 0;
            ecb.SetComponent(passenger, resident);

            var human = EntityManager.GetComponentData<Human>(passenger);
            human.m_Flags &= ~(HumanFlags.Run | HumanFlags.Emergency);
            ecb.SetComponent(passenger, human);

            // Replace the path buffer during ECB playback so vanilla can continue from the next leg.
            DynamicBuffer<PathElement> newPath = ecb.SetBuffer<PathElement>(passenger);
            for (var i = vehiclePathIndex + 1; i < pathElements.Length; i++)
            {
                newPath.Add(pathElements[i]);
            }

            pathOwner.m_ElementIndex = 0;
            ecb.SetComponent(passenger, pathOwner);
            return true;
        }

        private static void QueueVehiclePassengerBuffer(
            ref EntityCommandBuffer ecb,
            Entity vehicleEntity,
            DynamicBuffer<Passenger> passengers,
            HashSet<Entity> canceledPassengers)
        {
            DynamicBuffer<Passenger> newPassengers = ecb.SetBuffer<Passenger>(vehicleEntity);
            for (var i = 0; i < passengers.Length; i++)
            {
                if (!canceledPassengers.Contains(passengers[i].m_Passenger))
                {
                    newPassengers.Add(passengers[i]);
                }
            }
        }

        private TransportType GetTransportType(Entity vehicleEntity)
        {
            if (!EntityManager.HasComponent<PrefabRef>(vehicleEntity))
            {
                return TransportType.None;
            }

            Entity prefab = EntityManager.GetComponentData<PrefabRef>(vehicleEntity).m_Prefab;
            if (!EntityManager.HasComponent<PublicTransportVehicleData>(prefab))
            {
                return TransportType.None;
            }

            return EntityManager.GetComponentData<PublicTransportVehicleData>(prefab).m_TransportType;
        }
    }
}
