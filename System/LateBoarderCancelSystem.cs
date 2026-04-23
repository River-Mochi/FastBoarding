// File: System/LateBoarderCancelSystem.cs
// Purpose: Experimental system that lets solo late passengers miss a departing transit vehicle.

namespace FastBoarding
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
    using Unity.Collections;
    using Unity.Entities;
    using PrefabRef = Game.Prefabs.PrefabRef;
    using PublicTransportVehicleData = Game.Prefabs.PublicTransportVehicleData;
    using TransportType = Game.Prefabs.TransportType;

    public partial class LateBoarderCancelSystem : GameSystemBase
    {
        // 4096/day means every 64 simulation frames; cancellation work is also capped below.
        public const int UpdatesPerDay = 4096;
        private const int MaxCancellationsPerUpdate = 64;
        private const uint DiagnosticFrameInterval = 4096;
        private const int MaxSampledCimsPerModePerUpdate = 3;
        private const int MaxSampledCimsPerUpdate = MaxSampledCimsPerModePerUpdate * 7;
        private const uint FollowUpDelayFrames = 4096;
        private const int MaxFollowUpSamples = MaxSampledCimsPerUpdate;

        private EntityQuery m_VehicleQuery;
        private SimulationSystem? m_SimulationSystem;
        private ToolSystem? m_ToolSystem;
        private DefaultToolSystem? m_DefaultToolSystem;
        private uint m_LastDiagnosticFrame;
        private long m_TotalCanceled;
        private int m_SkippedForTool;
        private bool m_LoggedActive;
        private readonly FollowUpSample[] m_FollowUpSamples = new FollowUpSample[MaxFollowUpSamples];
        private int m_FollowUpCount;
        private int m_NextFollowUpSample;

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

        private readonly struct CanceledPassengerSample
        {
            public CanceledPassengerSample(TransportType transportType, Entity vehicle, Entity passenger)
            {
                TransportType = transportType;
                Vehicle = vehicle;
                Passenger = passenger;
            }

            public TransportType TransportType { get; }

            public Entity Vehicle { get; }

            public Entity Passenger { get; }
        }

        private struct FollowUpSample
        {
            public FollowUpSample(TransportType transportType, Entity vehicle, Entity passenger, uint frame)
            {
                Active = true;
                Logged = false;
                TransportType = transportType;
                Vehicle = vehicle;
                Passenger = passenger;
                Frame = frame;
            }

            public bool Active;

            public bool Logged;

            public TransportType TransportType;

            public Entity Vehicle;

            public Entity Passenger;

            public uint Frame;
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
                    // Avoid touching passenger buffers while edit tools may be deleting/rebuilding entities.
                    m_SkippedForTool++;
                    LogPassSummary(
                        m_SimulationSystem?.frameIndex ?? 0,
                        new PassStats(0, 0, 0, 0),
                        "paused-tool");
                    LogFollowUps(m_SimulationSystem?.frameIndex ?? 0);
                    return;
                }

                PassStats stats = RunCancellationPass();
                uint frame = m_SimulationSystem?.frameIndex ?? 0;
                LogPassSummary(frame, stats, "pass");
                LogFollowUps(frame);
            }
            catch (Exception ex)
            {
                Enabled = false;
                BoardingRuntimeSettings.SetCancelLateBoarders(false);

                Mod.WarnOnce(
                    "FB_LATE_BOARDER_CANCEL_EXCEPTION",
                    () => $"{Mod.ModTag} Late-cim skip disabled after {ex.GetType().Name}: {ex.Message}");
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
            int busSampleCount = 0;
            int trainSampleCount = 0;
            int tramSampleCount = 0;
            int subwaySampleCount = 0;
            int shipSampleCount = 0;
            int ferrySampleCount = 0;
            int airSampleCount = 0;
            CanceledPassengerSample[]? sampledCanceledPassengers = null;
            int sampledCanceledPassengerCount = 0;

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
                        // These flags are special gameplay flows, not normal public boarding.
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
                            // Spread very large crowds over multiple ticks instead of spiking one frame.
                            break;
                        }

                        if (QueuePassengerCancellation(ref ecb, vehicleEntity, passenger))
                        {
                            canceledPassengers.Add(passenger);
                            cancellationsThisUpdate++;
                            canceledForVehicle++;

                            if (sampledCanceledPassengerCount < MaxSampledCimsPerUpdate &&
                                TryCountCanceledSample(
                                    transportType,
                                    ref busSampleCount,
                                    ref trainSampleCount,
                                    ref tramSampleCount,
                                    ref subwaySampleCount,
                                    ref shipSampleCount,
                                    ref ferrySampleCount,
                                    ref airSampleCount))
                            {
                                // Keep a few entity IDs per mode for the on-demand troubleshooting report.
                                sampledCanceledPassengers ??= new CanceledPassengerSample[MaxSampledCimsPerUpdate];
                                sampledCanceledPassengers[sampledCanceledPassengerCount++] =
                                    new CanceledPassengerSample(transportType, vehicleEntity, passenger);
                            }
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

                // Mutate after scanning so we do not edit buffers while enumerating them.
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

                if (sampledCanceledPassengers != null)
                {
                    for (int i = 0; i < sampledCanceledPassengerCount; i++)
                    {
                        CanceledPassengerSample sample = sampledCanceledPassengers[i];
                        TransitWaitStatus.RecordLateBoarderSample(
                            World,
                            sample.TransportType,
                            sample.Vehicle,
                            sample.Passenger);

                        if (ShouldLogDiagnostics())
                        {
                            TrackFollowUpSample(sample);
                        }
                    }
                }

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

        private void LogActiveOnce()
        {
            if (!ShouldLogDiagnostics())
            {
                return;
            }

            if (m_LoggedActive)
            {
                return;
            }

            m_LoggedActive = true;
            LogUtils.Info(
                Mod.s_Log,
                () => $"Late-cim skip active. interval={GetUpdateInterval(SystemUpdatePhase.GameSimulation)} frames, cap={MaxCancellationsPerUpdate} per update. {BoardingRuntimeSettings.DescribeForLog()}");
        }

        private void LogPassSummary(uint frame, PassStats stats, string reason)
        {
            m_TotalCanceled += stats.Canceled;

            if (!ShouldLogDiagnostics())
            {
                return;
            }

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

        private void TrackFollowUpSample(CanceledPassengerSample sample)
        {
            uint frame = m_SimulationSystem?.frameIndex ?? 0;
            m_FollowUpSamples[m_NextFollowUpSample] =
                new FollowUpSample(sample.TransportType, sample.Vehicle, sample.Passenger, frame);
            m_NextFollowUpSample = (m_NextFollowUpSample + 1) % m_FollowUpSamples.Length;

            if (m_FollowUpCount < m_FollowUpSamples.Length)
            {
                m_FollowUpCount++;
            }
        }

        private void LogFollowUps(uint frame)
        {
            if (!ShouldLogDiagnostics())
            {
                return;
            }

            for (int i = 0; i < m_FollowUpCount; i++)
            {
                FollowUpSample sample = m_FollowUpSamples[i];
                if (!sample.Active || sample.Logged)
                {
                    continue;
                }

                uint elapsedFrames = frame >= sample.Frame
                    ? frame - sample.Frame
                    : uint.MaxValue;
                if (elapsedFrames < FollowUpDelayFrames)
                {
                    continue;
                }

                sample.Logged = true;
                m_FollowUpSamples[i] = sample;

                LogUtils.Info(
                    Mod.s_Log,
                    () => $"Late-cim follow-up: mode={sample.TransportType}, passenger={sample.Passenger}, missedVehicle={sample.Vehicle}, afterFrames={elapsedFrames}, {DescribePassengerState(sample.Passenger)}");
            }
        }

        private string DescribePassengerState(Entity passenger)
        {
            if (passenger == Entity.Null || !EntityManager.Exists(passenger))
            {
                return "state=entity missing";
            }

            if (EntityManager.HasComponent<Deleted>(passenger) ||
                EntityManager.HasComponent<Destroyed>(passenger))
            {
                return "state=deleted/destroyed";
            }

            string currentVehicleText = "none";
            if (EntityManager.HasComponent<CurrentVehicle>(passenger))
            {
                CurrentVehicle currentVehicle = EntityManager.GetComponentData<CurrentVehicle>(passenger);
                currentVehicleText = $"{currentVehicle.m_Vehicle} ({currentVehicle.m_Flags})";
            }

            int pathCount = EntityManager.HasBuffer<PathElement>(passenger)
                ? EntityManager.GetBuffer<PathElement>(passenger).Length
                : -1;
            int pathIndex = EntityManager.HasComponent<PathOwner>(passenger)
                ? EntityManager.GetComponentData<PathOwner>(passenger).m_ElementIndex
                : -1;

            return $"currentVehicle={currentVehicleText}, pathElements={pathCount}, pathIndex={pathIndex}";
        }

        private static bool ShouldLogDiagnostics()
        {
#if DEBUG
            return true;
#else
            return BoardingRuntimeSettings.EnableVerboseLogging;
#endif
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

        private static bool TryCountCanceledSample(
            TransportType transportType,
            ref int busSampleCount,
            ref int trainSampleCount,
            ref int tramSampleCount,
            ref int subwaySampleCount,
            ref int shipSampleCount,
            ref int ferrySampleCount,
            ref int airSampleCount)
        {
            switch (transportType)
            {
                case TransportType.Bus:
                    return TryIncrementSampleCount(ref busSampleCount);
                case TransportType.Train:
                    return TryIncrementSampleCount(ref trainSampleCount);
                case TransportType.Tram:
                    return TryIncrementSampleCount(ref tramSampleCount);
                case TransportType.Subway:
                    return TryIncrementSampleCount(ref subwaySampleCount);
                case TransportType.Ship:
                    return TryIncrementSampleCount(ref shipSampleCount);
                case TransportType.Ferry:
                    return TryIncrementSampleCount(ref ferrySampleCount);
                case TransportType.Airplane:
                    return TryIncrementSampleCount(ref airSampleCount);
                default:
                    return false;
            }
        }

        private static bool TryIncrementSampleCount(ref int sampleCount)
        {
            if (sampleCount >= MaxSampledCimsPerModePerUpdate)
            {
                return false;
            }

            sampleCount++;
            return true;
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
            // Clear urgency from the missed boarding attempt before vanilla resumes control.
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
