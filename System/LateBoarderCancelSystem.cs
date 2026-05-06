// File: System/LateBoarderCancelSystem.cs
// Purpose: Main update loop and cancellation pass for late solo passengers.

namespace FastBoarding
{
    using Colossal.Serialization.Entities;
    using Game;
    using Game.Common;
    using Game.Creatures;
    using Game.SceneFlow;
    using Game.Simulation;
    using Game.Tools;
    using Game.Vehicles;
    using System;
    using System.Collections.Generic;
    using Unity.Collections;
    using Unity.Entities;
    using TransportType = Game.Prefabs.TransportType;

    public partial class LateBoarderCancelSystem : GameSystemBase
    {
        // High-level flow:
        // 1. Scan boarding vehicles for solo passengers who are now late.
        // 2. Detach only the passengers whose remaining path still contains that exact vehicle.
        // 3. Optionally record a delayed "what happened next?" sample for verbose diagnostics.
        // 2048/day means every 128 simulation frames (~42 game seconds); cancellation work is also capped below.
        public const int UpdatesPerDay = 2048;

        // 128 keeps the old daily throughput ceiling after halving UpdatesPerDay from 4096 to 2048.
        // Higher caps process large crowds faster but can create larger one-frame edits.
        private const int MaxCancellationsPerUpdate = 128;

        private EntityQuery m_VehicleQuery;
        private SimulationSystem? m_SimulationSystem;
        private ToolSystem? m_ToolSystem;
        private DefaultToolSystem? m_DefaultToolSystem;

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
            int boardingHoldProbeLogs = 0;
            uint frame = m_SimulationSystem?.frameIndex ?? 0;

            try
            {
                // This pass reads vehicle/passenger data directly on the main thread.
                // Safety: CompleteDependency is only a sync point: it does not change the query,
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
                        publicTransport.m_DepartureFrame == 0 ||
                        frame <= publicTransport.m_DepartureFrame)
                    {
                        // Solo passengers still not ready after departure frame are treated as late.
                        continue;
                    }

                    TransportType transportType = GetTransportType(vehicleEntity);

                    var passengers = EntityManager.GetBuffer<Passenger>(vehicleEntity);
                    if (!passengers.IsCreated || passengers.Length == 0)
                    {
                        TryLogBoardingHoldProbe(
                            frame,
                            vehicleEntity,
                            transportType,
                            publicTransport,
                            passengerCount: 0,
                            readyCount: 0,
                            notReadyCount: 0,
                            groupNotReadyCount: 0,
                            unsafeNotReadyStats: default,
                            candidateCount: 0,
                            note: "empty-past-departure",
                            ref boardingHoldProbeLogs);

                        continue;
                    }

                    // Collect first, then mutate afterward, so do not edit the passenger buffer
                    // while still scanning it for late passengers.
                    // Managed HashSet is intentional here: this is short-lived main-thread scratch state,
                    // not Burst/job code, so NativeHashSet would add allocator/dispose noise with no payoff.
                    var pendingCancellation = new HashSet<Entity>();
                    int readyCount = 0;
                    int notReadyCount = 0;
                    int groupNotReadyCount = 0;
                    UnsafeNotReadyStats unsafeNotReadyStats = default;
                    int candidateCountForVehicle = 0;

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
                            readyCount++;
                            continue;
                        }

                        notReadyCount++;

                        if (IsGroupPassenger(passenger))
                        {
                            groupNotReadyCount++;
                            continue;
                        }

                        if (CanSafelyCancelPassenger(vehicleEntity, passenger, ref unsafeNotReadyStats))
                        {
                            pendingCancellation.Add(passenger);
                            candidates++;
                            candidateCountForVehicle++;
                        }
                    }

                    TryLogBoardingHoldProbe(
                        frame,
                        vehicleEntity,
                        transportType,
                        publicTransport,
                        passengers.Length,
                        readyCount,
                        notReadyCount,
                        groupNotReadyCount,
                        unsafeNotReadyStats,
                        candidateCountForVehicle,
                        candidateCountForVehicle == 0 ? "no-late-solo-candidates" : "late-solo-candidates",
                        ref boardingHoldProbeLogs);

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


        private static bool IsRealGameLoad(Purpose purpose, GameMode mode)
        {
            return mode == GameMode.Game &&
                (purpose == Purpose.NewGame || purpose == Purpose.LoadGame);
        }
    }
}
