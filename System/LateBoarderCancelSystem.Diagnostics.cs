// File: System/LateBoarderCancelSystem.Diagnostics.cs
// Purpose: Verbose diagnostics, delayed follow-up samples, and boarding-hold probes.

namespace FastBoarding
{
    using Game; // GameSystemBase
    using Game.Routes; // CurrentRoute
    using Game.Vehicles; // PublicTransport
    using System; // DateTime
    using Unity.Entities; // Entity
    using TransportType = Game.Prefabs.TransportType; // bus/train/etc.

    public partial class LateBoarderCancelSystem : GameSystemBase
    {
        // Verbose summary throttles. 4096 frames is about 22.5 in-game minutes.
        private const uint DiagnosticFrameInterval = 4096;
        private const uint BoardingHoldProbeFrameInterval = 4096;

        private const int MaxSampledCimsPerModePerUpdate = 3;
        private const int MaxSampledCimsPerUpdate = MaxSampledCimsPerModePerUpdate * 7;
        private const uint FollowUpDelayFrames = 2048; // ~11.25 in-game minutes.
        private const int MaxFollowUpSamples = 256;
        private const int MaxFollowUpLogsPerUpdate = 6;
        private const int MaxBoardingHoldProbeLogsPerUpdate = 6;

        private uint m_LastDiagnosticFrame;
        private uint m_LastBoardingHoldProbeFrame;
        private long m_TotalCanceled;
        private static bool s_FollowUpLegendLogged;
        private int m_SkippedForTool;
        private bool m_LoggedActive;

        // Fixed-size storage for delayed follow-up checks. Reuse slots instead of allocating every update.
        private readonly FollowUpSample[] m_FollowUpSamples = new FollowUpSample[MaxFollowUpSamples];
        private int m_FollowUpCount;
        private int m_NextFollowUpSample;

        private void TryLogBoardingHoldProbe(
            uint frame,
            Entity vehicleEntity,
            TransportType transportType,
            Game.Vehicles.PublicTransport publicTransport,
            int passengerCount,
            int readyCount,
            int notReadyCount,
            int groupNotReadyCount,
            UnsafeNotReadyStats unsafeNotReadyStats,
            int candidateCount,
            string note,
            ref int logsThisUpdate)
        {
            if (!ShouldLogDiagnostics())
            {
                return;
            }

            if (logsThisUpdate >= MaxBoardingHoldProbeLogsPerUpdate)
            {
                return;
            }

            if (m_LastBoardingHoldProbeFrame != 0 &&
                frame >= m_LastBoardingHoldProbeFrame &&
                frame - m_LastBoardingHoldProbeFrame < BoardingHoldProbeFrameInterval)
            {
                return;
            }

            uint framesPastDeparture = frame >= publicTransport.m_DepartureFrame
                ? frame - publicTransport.m_DepartureFrame
                : 0u;

            // Only log cases that can explain "vehicle is still boarding after departure":
            // empty/low-use vehicles, no safe late solo candidates, or the new vanilla long-hold threshold.
            bool lowUse = passengerCount <= 2;
            bool noLateSoloCandidates = candidateCount == 0;
            bool pastVanillaLongHold = framesPastDeparture >= 1800u;

            if (!lowUse && !noLateSoloCandidates && !pastVanillaLongHold)
            {
                return;
            }

            logsThisUpdate++;
            m_LastBoardingHoldProbeFrame = frame;

            Entity route = Entity.Null;
            if (EntityManager.HasComponent<CurrentRoute>(vehicleEntity))
            {
                route = EntityManager.GetComponentData<CurrentRoute>(vehicleEntity).m_Route;
            }

            string routeText = route == Entity.Null ? "none" : route.ToString();
            string vehicleText = EntityText(vehicleEntity);
            double gameMinutesPastDeparture = FramesToGameMinutes(framesPastDeparture);

            LogUtils.Info(
                Mod.s_Log,
                () =>
                    $"BoardingHoldProbe: mode={transportType}, vehicle={vehicleText}, route={routeText}, " +
                    $"frame={frame}, departureFrame={publicTransport.m_DepartureFrame}, " +
                    $"pastDepartureFrames={framesPastDeparture}, pastDepartureGameMin={gameMinutesPastDeparture:F1}, " +
                    $"passengers={passengerCount}, ready={readyCount}, notReady={notReadyCount}, " +
                    $"lateSoloCandidates={candidateCount}, groupNotReady={groupNotReadyCount}, unsafeNotReady={unsafeNotReadyStats.Total}, unsafeMissingData={unsafeNotReadyStats.MissingData}, unsafeNoExactVehicleInPath={unsafeNotReadyStats.NoExactVehicleInPath}, unsafeOther={unsafeNotReadyStats.Other}, " +
                    $"maxBoardingDistance={publicTransport.m_MaxBoardingDistance}, minWaitingDistance={publicTransport.m_MinWaitingDistance}, " +
                    $"state={publicTransport.m_State}, note={note}");
        }

        private static double FramesToGameMinutes(uint frames)
        {
            return frames * 1440.0 / 262144.0;
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
            // Prefer finished/empty slots so high-volume skip passes do not overwrite samples before proof checks run.
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

            // Follow-up logs answer the "what did vanilla do next?" question after a cim is detached.
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

        private void ResetDiagnosticsForCityLoad()
        {
            m_LastDiagnosticFrame = 0;
            m_LastBoardingHoldProbeFrame = 0;
            m_TotalCanceled = 0;
            m_SkippedForTool = 0;
            m_LoggedActive = false;
            m_FollowUpCount = 0;
            m_NextFollowUpSample = 0;
        }
    }
}
