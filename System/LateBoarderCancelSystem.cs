// File: System/LateBoarderCancelSystem.cs
// Purpose: Experimental system that lets solo late passengers miss a departing transit vehicle.

namespace FastBoarding
{
    using Colossal.Serialization.Entities;
    using Game;
    using Game.Common;
    using Game.Creatures;
    using Game.Pathfind;
    using Game.SceneFlow;
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
        // High-level flow:
        // 1. Scan boarding vehicles for solo passengers who are now late.
        // 2. Detach only the passengers whose remaining path still contains that exact vehicle.
        // 3. Optionally record a delayed "what happened next?" sample for verbose diagnostics.
        // 4096/day means every 64 simulation frames; cancellation work is also capped below.
        public const int UpdatesPerDay = 4096;
        private const int MaxCancellationsPerUpdate = 64;
        private const uint DiagnosticFrameInterval = 4096;
        private const int MaxSampledCimsPerModePerUpdate = 3;
        private const int MaxSampledCimsPerUpdate = MaxSampledCimsPerModePerUpdate * 7;
        private const uint FollowUpDelayFrames = 4096;
        private const int MaxFollowUpSamples = 256;
        private const int MaxFollowUpLogsPerUpdate = 6;

        private EntityQuery m_VehicleQuery;
        private SimulationSystem? m_SimulationSystem;
        private ToolSystem? m_ToolSystem;
        private DefaultToolSystem? m_DefaultToolSystem;
        private uint m_LastDiagnosticFrame;
        private long m_TotalCanceled;
        private static bool s_FollowUpLegendLogged;
        private int m_SkippedForTool;
        private bool m_LoggedActive;
        // Fixed-size storage for delayed follow-up checks. We reuse slots instead of allocating every update.
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
            public FollowUpSample(TransportType transportType, Entity vehicle, Entity passenger, uint frame, DateTime localTime)
            {
                Active = true;
                Logged = false;
                TransportType = transportType;
                Vehicle = vehicle;
                Passenger = passenger;
                Frame = frame;
                LocalTime = localTime;
            }

            public bool Active;

            public bool Logged;

            public TransportType TransportType;

            public Entity Vehicle;

            public Entity Passenger;

            public uint Frame;

            public DateTime LocalTime;
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

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            if (!IsRealGameLoad(purpose, mode))
            {
                return;
            }

            // If the player saved the checkbox ON, the Options UI setter will not fire on load.
            // Reactivate here so the live pass works without opening Options after city load/switch.
            ResetDiagnosticsForCityLoad();
            Enabled = BoardingRuntimeSettings.CancelLateBoarders;
        }

        protected override void OnUpdate()
        {
            try
            {
                if (!BoardingRuntimeSettings.CancelLateBoarders)
                {
                    // Options UI setter wakes this system only when the toggle is enabled.
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

                // Do one skip pass first, then separately ask "what did vanilla do next?" for older samples.
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
                // This pass reads vehicle/passenger data directly on the main thread.
                // CompleteDependency is only a sync point: it does not change the query,
                // it just waits for earlier ECS jobs touching the same data to finish first.
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
                    // while we are still scanning it for late passengers.
                    // Managed HashSet is intentional here: this is short-lived main-thread scratch state,
                    // not Burst/job code, so NativeHashSet would add allocator/dispose noise with no payoff.
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
                () => $"Skip pass active: every {GetUpdateInterval(SystemUpdatePhase.GameSimulation)} frames, cap={MaxCancellationsPerUpdate} late solo cims/update");
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
            if (reason == "paused-tool")
            {
                LogUtils.Info(
                    Mod.s_Log,
                    () => $"Skip pass paused: activeTool={activeTool}, pauses={m_SkippedForTool}, totalSkipped={m_TotalCanceled}");
                return;
            }

            LogUtils.Info(
                Mod.s_Log,
                () => $"Skip pass: vehicles={stats.Vehicles}, passengersScanned={stats.Passengers}, lateSolo={stats.Candidates}, skipped={stats.Canceled}, totalSkipped={m_TotalCanceled}");
        }

        private void TrackFollowUpSample(CanceledPassengerSample sample)
        {
            uint frame = m_SimulationSystem?.frameIndex ?? 0;
            int slot = FindFollowUpSampleSlot();
            if (slot < 0)
            {
                return;
            }

            m_FollowUpSamples[slot] =
                new FollowUpSample(sample.TransportType, sample.Vehicle, sample.Passenger, frame, DateTime.Now);

            if (m_FollowUpCount < m_FollowUpSamples.Length)
            {
                m_FollowUpCount++;
            }
        }

        private int FindFollowUpSampleSlot()
        {
            // Prefer finished/empty slots so high-volume skip passes do not overwrite samples
            // before they reach FollowUpDelayFrames and prove what vanilla did next.
            for (int attempt = 0; attempt < m_FollowUpSamples.Length; attempt++)
            {
                int index = (m_NextFollowUpSample + attempt) % m_FollowUpSamples.Length;
                FollowUpSample sample = m_FollowUpSamples[index];
                if (!sample.Active || sample.Logged)
                {
                    m_NextFollowUpSample = (index + 1) % m_FollowUpSamples.Length;
                    return index;
                }
            }

            return -1;
        }

        private void LogFollowUps(uint frame)
        {
            if (!ShouldLogDiagnostics())
            {
                return;
            }

            // Follow-up logs answer the "what did vanilla do next?" question after we detach a cim.
            TransitWaitStatusSystem followUpStatusSystem = World.GetOrCreateSystemManaged<TransitWaitStatusSystem>();
            int loggedThisUpdate = 0;
            for (int i = 0; i < m_FollowUpCount; i++)
            {
                if (loggedThisUpdate >= MaxFollowUpLogsPerUpdate)
                {
                    break;
                }

                FollowUpSample sample = m_FollowUpSamples[i];
                if (!sample.Active || sample.Logged)
                {
                    continue;
                }

                if ((frame >= sample.Frame
                        ? frame - sample.Frame
                        : uint.MaxValue) < FollowUpDelayFrames)
                {
                    continue;
                }

                sample.Logged = true;
                sample.Active = false;
                m_FollowUpSamples[i] = sample;
                loggedThisUpdate++;
                LogFollowUpLegendOnce();

                DateTime followUpLocalTime = DateTime.Now;
                TransitWaitStatusSystem.FollowUpSnapshot followUpSnapshot =
                    followUpStatusSystem.BuildLateBoarderFollowUpSnapshot(
                        sample.Passenger,
                        sample.TransportType,
                        sample.Vehicle);
                TransitWaitStatus.RecordLateBoarderFollowUp(
                    World,
                    sample.TransportType,
                    sample.Vehicle,
                    sample.Passenger,
                    sample.LocalTime,
                    followUpLocalTime,
                    followUpSnapshot);

                LogUtils.Info(
                    Mod.s_Log,
                    () => $"Skipped Late Passenger: {sample.TransportType} | cim={sample.Passenger} | missed={sample.Vehicle} | skipped={sample.LocalTime:HH:mm:ss} | followUp={followUpLocalTime:HH:mm:ss} | state={DescribeFollowUpState(followUpSnapshot)}");
            }
        }

        private static void LogFollowUpLegendOnce()
        {
            if (s_FollowUpLegendLogged)
            {
                return;
            }

            s_FollowUpLegendLogged = true;
            LogUtils.Info(
                Mod.s_Log,
                () => "Skip follow-up legend: state=same vehicle/different vehicle means assigned; has path means repathing or walking; no path yet means unresolved. next=stop/lane/waypoint/vehicle/target shows the cim's next path target.");
        }

        private static string EntityText(Entity entity)
        {
            return entity == Entity.Null ? "none" : entity.ToString();
        }

        private static string DescribeFollowUpState(TransitWaitStatusSystem.FollowUpSnapshot snapshot)
        {
            if (snapshot.CurrentVehicle != Entity.Null)
            {
                string assignment = snapshot.Outcome == TransitWaitStatusSystem.FollowUpOutcome.SameVehicle
                    ? $"same vehicle {snapshot.CurrentVehicleText}"
                    : $"different vehicle {snapshot.CurrentVehicleText}";
                return AppendFollowUpTargetDetails(assignment, snapshot);
            }

            if (snapshot.Outcome == TransitWaitStatusSystem.FollowUpOutcome.HasPathNotAssignedYet)
            {
                return AppendFollowUpTargetDetails("has path", snapshot);
            }

            return "no path yet";
        }

        private static string AppendFollowUpTargetDetails(
            string summary,
            TransitWaitStatusSystem.FollowUpSnapshot snapshot)
        {
            string nextTarget = DescribeFollowUpTarget(snapshot);
            if (!string.IsNullOrWhiteSpace(nextTarget))
            {
                summary += $" | next={nextTarget}";
            }

            if (snapshot.NextTargetKind == TransitWaitStatusSystem.FollowUpTargetKind.Stop &&
                !string.IsNullOrWhiteSpace(snapshot.NextLineName))
            {
                summary += $" | line={snapshot.NextLineName}";
            }

            return summary;
        }

        private static string DescribeFollowUpTarget(TransitWaitStatusSystem.FollowUpSnapshot snapshot)
        {
            switch (snapshot.NextTargetKind)
            {
                case TransitWaitStatusSystem.FollowUpTargetKind.Stop:
                    return DescribeNamedEntity("stop", snapshot.NextStopName, snapshot.NextStopEntity);
                case TransitWaitStatusSystem.FollowUpTargetKind.Lane:
                    return DescribeNamedEntity("lane", snapshot.NextTargetName, Entity.Null);
                case TransitWaitStatusSystem.FollowUpTargetKind.Waypoint:
                    return DescribeNamedEntity("waypoint", snapshot.NextTargetName, snapshot.NextTargetEntity);
                case TransitWaitStatusSystem.FollowUpTargetKind.Vehicle:
                    return DescribeNamedEntity("vehicle", snapshot.NextTargetName, snapshot.NextTargetEntity);
                case TransitWaitStatusSystem.FollowUpTargetKind.Target:
                    return DescribeNamedEntity("target", snapshot.NextTargetName, snapshot.NextTargetEntity);
                default:
                    return string.Empty;
            }
        }

        private static string DescribeNamedEntity(string kind, string name, Entity entity)
        {
            string label = string.IsNullOrWhiteSpace(name)
                ? kind
                : $"{kind} {name}";
            return entity == Entity.Null
                ? label
                : $"{label} {EntityText(entity)}";
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
            // Only inspect the remaining path from the current cursor forward.
            // Older elements before m_ElementIndex are already behind the cim.
            int startIndex = Math.Max(0, pathOwner.m_ElementIndex);
            // Only cancel when the cim still has this exact vehicle in the remaining path.
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
            // Match vanilla's "search from the current path cursor onward" behavior.
            int startIndex = Math.Max(0, pathOwner.m_ElementIndex);
            // -1 is a sentinel meaning "vehicle not found yet".
            // We never use it as an array index: the < 0 check below aborts first.
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
                // Safety stop: if the vehicle disappeared from the remaining path, do nothing
                // and leave the passenger fully to vanilla.
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
            // We keep only the elements AFTER the missed vehicle. That is why the copy starts at +1.
            // If the missed vehicle was already the final leg, the new path is intentionally empty.
            DynamicBuffer<PathElement> newPath = ecb.SetBuffer<PathElement>(passenger);
            for (var i = vehiclePathIndex + 1; i < pathElements.Length; i++)
            {
                newPath.Add(pathElements[i]);
            }

            // Restart from the beginning of the trimmed buffer. This matches the vanilla pattern:
            // once the consumed prefix is gone, the next remaining leg is now index 0.
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
            // Rebuild the passenger buffer once so we do not remove entries while iterating it.
            // SetBuffer replaces the whole buffer at playback, so there is no extra Clear() call here.
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

        private void ResetDiagnosticsForCityLoad()
        {
            m_LastDiagnosticFrame = 0;
            m_TotalCanceled = 0;
            m_SkippedForTool = 0;
            m_LoggedActive = false;
            m_FollowUpCount = 0;
            m_NextFollowUpSample = 0;
        }

        private static bool IsRealGameLoad(Purpose purpose, GameMode mode)
        {
            return mode == GameMode.Game &&
                (purpose == Purpose.NewGame || purpose == Purpose.LoadGame);
        }
    }
}
