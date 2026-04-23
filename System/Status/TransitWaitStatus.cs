// File: System/Status/TransitWaitStatus.cs
// Purpose: Cached Options UI status text for current transit wait snapshots.

namespace FastBoarding
{
    using Game;
    using Game.Common;
    using Game.Creatures;
    using Game.Pathfind;
    using Game.Prefabs;
    using Game.SceneFlow;
    using Game.Simulation;
    using System;
    using System.Text;
    using Unity.Entities;
    using UnityEngine;

    public static class TransitWaitStatus
    {
        public const string KeyStatusNotLoaded = "FB_STATUS_NOT_LOADED";
        public const string KeyNoCityLoaded = "FB_STATUS_NO_CITY_LOADED";
        public const string KeyNoStopsFound = "FB_STATUS_NO_STOPS";
        public const string KeyStatusLine = "FB_STATUS_LINE";
        public const string KeyStatusOverviewLine = "FB_STATUS_OVERVIEW_LINE";
        public const string KeyReportNoCityLoaded = "FB_REPORT_NO_CITY_LOADED";
        public const string KeyReportTitle = "FB_REPORT_TITLE";
        public const string KeyReportSettings = "FB_REPORT_SETTINGS";
        public const string KeyReportNote = "FB_REPORT_NOTE";
        public const string KeyReportTesterHintsHeader = "FB_REPORT_TESTER_HINTS_HEADER";
        public const string KeyReportHintWorstStops = "FB_REPORT_HINT_WORST_STOPS";
        public const string KeyReportHintSkippedCims = "FB_REPORT_HINT_SKIPPED_CIMS";
        public const string KeyReportHintLateGroups = "FB_REPORT_HINT_LATE_GROUPS";
        public const string KeyReportFamilyHeader = "FB_REPORT_FAMILY_HEADER";
        public const string KeyReportServedStops = "FB_REPORT_SERVED_STOPS";
        public const string KeyReportStopsWithWaiting = "FB_REPORT_STOPS_WITH_WAITING";
        public const string KeyReportWaitingPassengers = "FB_REPORT_WAITING_PASSENGERS";
        public const string KeyReportAverageWait = "FB_REPORT_AVERAGE_WAIT";
        public const string KeyReportLateBoardersSkipped = "FB_REPORT_LATE_BOARDERS_SKIPPED";
        public const string KeyReportWorstStopNone = "FB_REPORT_WORST_STOP_NONE";
        public const string KeyReportWorstStopAverageWait = "FB_REPORT_WORST_STOP_AVERAGE_WAIT";
        public const string KeyReportWorstStopName = "FB_REPORT_WORST_STOP_NAME";
        public const string KeyReportWorstStopEntity = "FB_REPORT_WORST_STOP_ENTITY";
        public const string KeyReportWorstWaypointEntity = "FB_REPORT_WORST_WAYPOINT_ENTITY";
        public const string KeyReportWorstLineHint = "FB_REPORT_WORST_LINE_HINT";
        public const string KeyReportWorstLineEntity = "FB_REPORT_WORST_LINE_ENTITY";
        public const string KeyReportWorstLineWaypointAverage = "FB_REPORT_WORST_LINE_WAYPOINT_AVERAGE";
        public const string KeyReportTopWorstStopsHeader = "FB_REPORT_TOP_WORST_STOPS_HEADER";
        public const string KeyReportTopWorstStopLine = "FB_REPORT_TOP_WORST_STOP_LINE";
        public const string KeyReportLateGroups = "FB_REPORT_LATE_GROUPS";
        public const string KeyReportLastSkippedSamplesHeader = "FB_REPORT_LAST_SKIPPED_SAMPLES_HEADER";
        public const string KeyReportLastSkippedSampleLine = "FB_REPORT_LAST_SKIPPED_SAMPLE_LINE";
        public const string KeyReportNone = "FB_REPORT_NONE";
        public const string KeyReportUnknown = "FB_REPORT_UNKNOWN";

        private const int ReportHeaderWidth = 60;
        private const int SkippedSampleCapacityPerMode = 3;

        public static int RefreshIntervalSeconds { get; set; } = 15;

        public static string OverviewSummary { get; private set; } = string.Empty;
        public static string BusSummary { get; private set; } = string.Empty;
        public static string TrainSummary { get; private set; } = string.Empty;
        public static string TramSummary { get; private set; } = string.Empty;
        public static string SubwaySummary { get; private set; } = string.Empty;
        public static string ShipSummary { get; private set; } = string.Empty;
        public static string FerrySummary { get; private set; } = string.Empty;
        public static string AirSummary { get; private set; } = string.Empty;

        private static bool s_WasInGame;
        private static bool s_HasSnapshotThisCity;
        private static long s_LastRefreshTicksUtc;
        private static int s_LastUiFrame = -1;
        private static int s_CurrentDayKey = int.MinValue;
        private static uint s_LastSimulationFrame = uint.MaxValue;
        private static long s_BusLateBoardersToday;
        private static long s_TrainLateBoardersToday;
        private static long s_TramLateBoardersToday;
        private static long s_SubwayLateBoardersToday;
        private static long s_ShipLateBoardersToday;
        private static long s_FerryLateBoardersToday;
        private static long s_AirLateBoardersToday;
        private static DateTime s_LastSnapshotLocalTime;
        private static readonly SkippedPassengerSampleRing s_BusSkippedSamples = new SkippedPassengerSampleRing();
        private static readonly SkippedPassengerSampleRing s_TrainSkippedSamples = new SkippedPassengerSampleRing();
        private static readonly SkippedPassengerSampleRing s_TramSkippedSamples = new SkippedPassengerSampleRing();
        private static readonly SkippedPassengerSampleRing s_SubwaySkippedSamples = new SkippedPassengerSampleRing();
        private static readonly SkippedPassengerSampleRing s_ShipSkippedSamples = new SkippedPassengerSampleRing();
        private static readonly SkippedPassengerSampleRing s_FerrySkippedSamples = new SkippedPassengerSampleRing();
        private static readonly SkippedPassengerSampleRing s_AirSkippedSamples = new SkippedPassengerSampleRing();

        private readonly struct SkippedPassengerSample
        {
            public SkippedPassengerSample(TransportType transportType, Entity vehicle, Entity passenger, uint frame, DateTime localTime)
            {
                TransportType = transportType;
                Vehicle = vehicle;
                Passenger = passenger;
                Frame = frame;
                LocalTime = localTime;
            }

            public TransportType TransportType { get; }

            public Entity Vehicle { get; }

            public Entity Passenger { get; }

            public uint Frame { get; }

            public DateTime LocalTime { get; }
        }

        private sealed class SkippedPassengerSampleRing
        {
            private readonly SkippedPassengerSample[] m_Samples =
                new SkippedPassengerSample[SkippedSampleCapacityPerMode];
            private int m_Count;
            private int m_NextIndex;

            public int Count => m_Count;

            public void Add(SkippedPassengerSample sample)
            {
                m_Samples[m_NextIndex] = sample;
                m_NextIndex = (m_NextIndex + 1) % m_Samples.Length;

                if (m_Count < m_Samples.Length)
                {
                    m_Count++;
                }
            }

            public void Clear()
            {
                m_Count = 0;
                m_NextIndex = 0;
            }

            public SkippedPassengerSample GetNewest(int newestIndex)
            {
                int index = (m_NextIndex - 1 - newestIndex + m_Samples.Length) % m_Samples.Length;
                return m_Samples[index];
            }
        }

        public static void InvalidateCache()
        {
            s_HasSnapshotThisCity = false;
            s_LastRefreshTicksUtc = 0;
            s_LastUiFrame = -1;

            BusSummary = Localize(KeyStatusNotLoaded, "Status not loaded.");
            OverviewSummary = BusSummary;
            TrainSummary = string.Empty;
            TramSummary = string.Empty;
            SubwaySummary = string.Empty;
            ShipSummary = string.Empty;
            FerrySummary = string.Empty;
            AirSummary = string.Empty;
        }

        public static void RefreshIfNeeded()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return;
            }

            // The Options UI polls each property separately, so refresh once per UI frame at most.
            int frame = Time.frameCount;
            if (frame == s_LastUiFrame)
            {
                return;
            }

            s_LastUiFrame = frame;

            GameManager gm = GameManager.instance;
            bool isGame = gm.gameMode.IsGame();

            if (isGame != s_WasInGame)
            {
                bool leftGame = s_WasInGame && !isGame;
                s_WasInGame = isGame;

                // Loading/unloading a city makes cached stop text stale. Do not reset skipped
                // counters when entering a city here, because the skip system may have already
                // recorded passengers before the player opens Options UI for the first time.
                InvalidateCache();

                if (leftGame)
                {
                    ResetDailyCounters();
                }
            }

            if (!isGame)
            {
                BusSummary = Localize(KeyNoCityLoaded, "No city loaded.");
                OverviewSummary = BusSummary;
                TrainSummary = string.Empty;
                TramSummary = string.Empty;
                SubwaySummary = string.Empty;
                ShipSummary = string.Empty;
                FerrySummary = string.Empty;
                AirSummary = string.Empty;
                return;
            }

            EnsureCounterDay(world);

            long nowUtc = DateTime.UtcNow.Ticks;
            if (!s_HasSnapshotThisCity)
            {
                BuildSnapshotSafe(world);
                s_HasSnapshotThisCity = true;
                s_LastRefreshTicksUtc = nowUtc;
                return;
            }

            int intervalSeconds = Math.Max(1, RefreshIntervalSeconds);
            long nextAllowed = s_LastRefreshTicksUtc + TimeSpan.FromSeconds(intervalSeconds).Ticks;
            if (nowUtc < nextAllowed)
            {
                return;
            }

            BuildSnapshotSafe(world);
            s_LastRefreshTicksUtc = nowUtc;
        }

        private static void BuildSnapshotSafe(World world)
        {
            try
            {
                // Snapshot work is on demand; this does not run every simulation frame.
                TransitWaitStatusSystem system = world.GetOrCreateSystemManaged<TransitWaitStatusSystem>();
                TransitWaitStatusSystem.Snapshot snapshot = system.BuildSnapshot();
                ApplySnapshot(snapshot);
            }
            catch (Exception ex)
            {
                BusSummary = Localize(KeyStatusNotLoaded, "Status not loaded.");
                TrainSummary = string.Empty;
                TramSummary = string.Empty;
                SubwaySummary = string.Empty;
                ShipSummary = string.Empty;
                FerrySummary = string.Empty;
                AirSummary = string.Empty;

                Mod.WarnOnce(
                    "FB_STATUS_SNAPSHOT_EXCEPTION",
                    () => $"{Mod.ModTag} Transit status snapshot failed: {ex.GetType().Name}: {ex.Message}");
            }
        }

        public static void LogDetailedReport()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            GameManager gm = GameManager.instance;
            if (world == null || !world.IsCreated || !gm.gameMode.IsGame())
            {
                LogUtils.Info(Mod.s_Log, () => Localize(KeyReportNoCityLoaded, "[FB] Stats report requested, but no city is loaded."));
                return;
            }

            try
            {
                EnsureCounterDay(world);
                TransitWaitStatusSystem system = world.GetOrCreateSystemManaged<TransitWaitStatusSystem>();
                TransitWaitStatusSystem.Snapshot snapshot = system.BuildSnapshot();
                ApplySnapshot(snapshot);

                // Keep this verbose output in the log, not the cramped Options UI row.
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                AppendSectionHeader(sb, Localize(KeyReportTitle, "Fast Boarding transit status report"));
                AppendField(sb, "Snapshot updated", s_LastSnapshotLocalTime == default ? "unknown" : s_LastSnapshotLocalTime.ToString("HH:mm:ss"));
                AppendField(sb, "Settings", BoardingRuntimeSettings.DescribeForLog());
                AppendField(sb, "Note", Localize(KeyReportNote, "Worst line is a hint from the highest-wait waypoint at the worst stop."));

                AppendTesterHints(sb);
                AppendSummaryReport(sb, snapshot);

                AppendFamilyReport(sb, world, "Bus", snapshot.Bus, s_BusLateBoardersToday, s_BusSkippedSamples);
                AppendFamilyReport(sb, world, "Tram", snapshot.Tram, s_TramLateBoardersToday, s_TramSkippedSamples);
                AppendFamilyReport(sb, world, "Train", snapshot.Train, s_TrainLateBoardersToday, s_TrainSkippedSamples);
                AppendFamilyReport(sb, world, "Subway", snapshot.Subway, s_SubwayLateBoardersToday, s_SubwaySkippedSamples);
                AppendFamilyReport(sb, world, "Ferry", snapshot.Ferry, s_FerryLateBoardersToday, s_FerrySkippedSamples);
                AppendFamilyReport(sb, world, "Ship", snapshot.Ship, s_ShipLateBoardersToday, s_ShipSkippedSamples);
                AppendFamilyReport(sb, world, "Airplane", snapshot.Air, s_AirLateBoardersToday, s_AirSkippedSamples);

                AppendDivider(sb);

                LogUtils.Info(Mod.s_Log, () => sb.ToString());
            }
            catch (Exception ex)
            {
                LogUtils.Warn(Mod.s_Log, () => $"{Mod.ModTag} Stats report failed: {ex.GetType().Name}: {ex.Message}", ex);
            }
        }

        private static void AppendTesterHints(StringBuilder sb)
        {
            sb.AppendLine();
            AppendSubHeader(sb, Localize(KeyReportTesterHintsHeader, "Tester hints"));
            sb.AppendLine("- " + Localize(
                KeyReportHintWorstStops,
                "Worst stops: inspect these first in-game or with Scene Explorer. Look for accidents, blocked traffic, bad stop placement, or a bugged stop."));
            sb.AppendLine("- " + Localize(
                KeyReportHintSkippedCims,
                "Skipped solo cims: later state should usually become 'has path' or 'assigned'. If it stays 'no path yet', inspect that cim entity after more time."));
            sb.AppendLine("- " + Localize(
                KeyReportHintLateGroups,
                "Late groups: these are families/groups left to vanilla. High counts are clues for future safe group-travel support."));
        }

        internal static void RecordLateBoardersCanceled(World world, TransportType transportType, int count)
        {
            if (count <= 0)
            {
                return;
            }

            EnsureCounterDay(world);

            switch (transportType)
            {
                case TransportType.Bus:
                    s_BusLateBoardersToday += count;
                    break;
                case TransportType.Train:
                    s_TrainLateBoardersToday += count;
                    break;
                case TransportType.Tram:
                    s_TramLateBoardersToday += count;
                    break;
                case TransportType.Subway:
                    s_SubwayLateBoardersToday += count;
                    break;
                case TransportType.Ship:
                    s_ShipLateBoardersToday += count;
                    break;
                case TransportType.Ferry:
                    s_FerryLateBoardersToday += count;
                    break;
                case TransportType.Airplane:
                    s_AirLateBoardersToday += count;
                    break;
            }
        }

        internal static void RecordLateBoarderSample(World world, TransportType transportType, Entity vehicle, Entity passenger)
        {
            EnsureCounterDay(world);

            uint frame = world.GetExistingSystemManaged<SimulationSystem>()?.frameIndex ?? 0;
            GetSkippedSampleRing(transportType).Add(
                new SkippedPassengerSample(transportType, vehicle, passenger, frame, DateTime.Now));
        }

        private static string FormatFamily(TransitWaitStatusSystem.FamilySnapshot family, long lateBoardersCanceledToday)
        {
            if (family.StopCount == 0)
            {
                return LocaleUtils.Localize(KeyNoStopsFound, "No stops found.");
            }

            return LocaleUtils.SafeFormat(
                KeyStatusLine,
                "{0} wait | avg {1} | worst {2} | {3} skipped",
                LocaleUtils.FormatN0(family.WaitingPassengers),
                FormatDuration(family.AverageWaitSeconds),
                FormatDuration(family.WorstStopWaitSeconds),
                LocaleUtils.FormatN0(lateBoardersCanceledToday));
        }

        private static void ApplySnapshot(TransitWaitStatusSystem.Snapshot snapshot)
        {
            s_LastSnapshotLocalTime = DateTime.Now;
            OverviewSummary = FormatOverview(snapshot);
            BusSummary = FormatFamily(snapshot.Bus, s_BusLateBoardersToday);
            TrainSummary = FormatFamily(snapshot.Train, s_TrainLateBoardersToday);
            TramSummary = FormatFamily(snapshot.Tram, s_TramLateBoardersToday);
            SubwaySummary = FormatFamily(snapshot.Subway, s_SubwayLateBoardersToday);
            ShipSummary = FormatFamily(snapshot.Ship, s_ShipLateBoardersToday);
            FerrySummary = FormatFamily(snapshot.Ferry, s_FerryLateBoardersToday);
            AirSummary = FormatFamily(snapshot.Air, s_AirLateBoardersToday);
        }

        private static string FormatOverview(TransitWaitStatusSystem.Snapshot snapshot)
        {
            return LocaleUtils.SafeFormat(
                KeyStatusOverviewLine,
                "{0} tourist/mo | {1} citizens/mo | updated {2}",
                LocaleUtils.FormatN0(snapshot.MonthlyTourists),
                LocaleUtils.FormatN0(snapshot.MonthlyCitizens),
                s_LastSnapshotLocalTime.ToString("HH:mm:ss"));
        }

        private static void AppendSummaryReport(StringBuilder sb, TransitWaitStatusSystem.Snapshot snapshot)
        {
            sb.AppendLine();
            AppendSectionHeader(sb, "Summary");
            AppendField(
                sb,
                "Total public transit",
                $"{LocaleUtils.FormatN0(snapshot.MonthlyTourists)} tourists/mo | {LocaleUtils.FormatN0(snapshot.MonthlyCitizens)} citizens/mo");
            AppendSummaryLine(sb, "Bus", snapshot.Bus, s_BusLateBoardersToday);
            AppendSummaryLine(sb, "Tram", snapshot.Tram, s_TramLateBoardersToday);
            AppendSummaryLine(sb, "Train", snapshot.Train, s_TrainLateBoardersToday);
            AppendSummaryLine(sb, "Subway", snapshot.Subway, s_SubwayLateBoardersToday);
            AppendSummaryLine(sb, "Ferry", snapshot.Ferry, s_FerryLateBoardersToday);
            AppendSummaryLine(sb, "Ship", snapshot.Ship, s_ShipLateBoardersToday);
            AppendSummaryLine(sb, "Airplane", snapshot.Air, s_AirLateBoardersToday);
        }

        private static void AppendSummaryLine(
            StringBuilder sb,
            string label,
            TransitWaitStatusSystem.FamilySnapshot family,
            long lateBoardersCanceledToday)
        {
            if (family.StopCount == 0)
            {
                AppendField(sb, label, Localize(KeyNoStopsFound, "No stops found."));
                return;
            }

            AppendField(
                sb,
                label,
                $"{LocaleUtils.FormatN0(family.WaitingPassengers)} waiting | avg wait {FormatDuration(family.AverageWaitSeconds)} | worst stop {FormatDuration(family.WorstStopWaitSeconds)} | queued/total stops {LocaleUtils.FormatN0(family.ActiveQueueStops)}/{LocaleUtils.FormatN0(family.StopCount)} | {LocaleUtils.FormatN0(lateBoardersCanceledToday)} skipped late passengers");
        }

        private static void AppendFamilyReport(
            StringBuilder sb,
            World world,
            string label,
            TransitWaitStatusSystem.FamilySnapshot family,
            long lateBoardersCanceledToday,
            SkippedPassengerSampleRing skippedSamples)
        {
            sb.AppendLine();
            AppendSectionHeader(sb, FormatReport(KeyReportFamilyHeader, "{0}", label));

            if (family.StopCount == 0)
            {
                sb.AppendLine(Localize(KeyNoStopsFound, "No stops found."));
                return;
            }

            AppendField(
                sb,
                "Status",
                $"{LocaleUtils.FormatN0(family.WaitingPassengers)} waiting | avg wait {FormatDuration(family.AverageWaitSeconds)} | worst stop {FormatDuration(family.WorstStopWaitSeconds)} | queued/total stops {LocaleUtils.FormatN0(family.ActiveQueueStops)}/{LocaleUtils.FormatN0(family.StopCount)}");
            AppendField(sb, "Late solo cims skipped", LocaleUtils.FormatN0(lateBoardersCanceledToday) + " today");
            AppendField(
                sb,
                "Late groups not skipped",
                $"{LocaleUtils.FormatN0(family.LateGroupPassengers)} passengers | {LocaleUtils.FormatN0(family.LateGroupGroups)} groups | {LocaleUtils.FormatN0(family.LateGroupVehicles)} vehicles");

            if (family.WaitingPassengers <= 0)
            {
                sb.AppendLine(Localize(KeyReportWorstStopNone, "Worst stop: none, no waiting passengers right now."));
            }
            else
            {
                AppendTopWorstStops(sb, family);
            }

            AppendSkippedPassengerSamples(sb, world, skippedSamples);
        }

        private static void AppendTopWorstStops(StringBuilder sb, TransitWaitStatusSystem.FamilySnapshot family)
        {
            if (family.TopWorstStops.Length == 0)
            {
                return;
            }

            sb.AppendLine();
            AppendSubHeader(sb, FormatReport(
                KeyReportTopWorstStopsHeader,
                "Top {0} worst stops by average wait",
                LocaleUtils.FormatN0(family.TopWorstStops.Length)).TrimEnd(':'));

            for (int i = 0; i < family.TopWorstStops.Length; i++)
            {
                TransitWaitStatusSystem.WorstStopSnapshot stop = family.TopWorstStops[i];
                sb.AppendLine(
                    $"{LocaleUtils.FormatN0(i + 1)}. {TextOrUnknown(stop.StopName)} | avg {FormatDuration(stop.AverageWaitSeconds)} | waiting {LocaleUtils.FormatN0(stop.WaitingPassengers)} | line hint {TextOrUnknown(stop.LineName)}");
                sb.AppendLine(
                    $"   stop {EntityText(stop.StopEntity)} | waypoint {EntityText(stop.WaypointEntity)} | line {EntityText(stop.LineEntity)}");
            }
        }

        private static void AppendSkippedPassengerSamples(
            StringBuilder sb,
            World world,
            SkippedPassengerSampleRing skippedSamples)
        {
            sb.AppendLine();
            AppendSubHeader(sb, Localize(KeyReportLastSkippedSamplesHeader, "Skipped solo cim examples"));

            if (skippedSamples.Count == 0)
            {
                sb.AppendLine(Localize(KeyReportNone, "none"));
                return;
            }

            for (int i = 0; i < skippedSamples.Count; i++)
            {
                SkippedPassengerSample sample = skippedSamples.GetNewest(i);
                sb.AppendLine(LocaleUtils.SafeFormat(
                    KeyReportLastSkippedSampleLine,
                    "{0}. {1} | passenger {2} | vehicle {3} | frame {4} | time {5} | now {6}",
                    LocaleUtils.FormatN0(i + 1),
                    sample.TransportType,
                    EntityText(sample.Passenger),
                    EntityText(sample.Vehicle),
                    sample.Frame,
                    sample.LocalTime.ToString("HH:mm:ss"),
                    FormatPassengerState(world, sample.Passenger)));
            }
        }

        private static string FormatPassengerState(World world, Entity passenger)
        {
            if (world == null || !world.IsCreated || passenger == Entity.Null)
            {
                return "unknown";
            }

            EntityManager entityManager = world.EntityManager;
            if (!entityManager.Exists(passenger))
            {
                return "entity missing";
            }

            if (entityManager.HasComponent<Deleted>(passenger) ||
                entityManager.HasComponent<Destroyed>(passenger))
            {
                return "deleted/destroyed";
            }

            bool hasCurrentVehicle = entityManager.HasComponent<CurrentVehicle>(passenger);
            Entity currentVehicle = Entity.Null;
            CreatureVehicleFlags vehicleFlags = default;
            if (hasCurrentVehicle)
            {
                CurrentVehicle currentVehicleData = entityManager.GetComponentData<CurrentVehicle>(passenger);
                currentVehicle = currentVehicleData.m_Vehicle;
                vehicleFlags = currentVehicleData.m_Flags;
            }

            int pathCount = entityManager.HasBuffer<PathElement>(passenger)
                ? entityManager.GetBuffer<PathElement>(passenger).Length
                : -1;
            int pathIndex = entityManager.HasComponent<PathOwner>(passenger)
                ? entityManager.GetComponentData<PathOwner>(passenger).m_ElementIndex
                : -1;

            string vehicleText = hasCurrentVehicle
                ? $"{EntityText(currentVehicle)} ({vehicleFlags})"
                : "none";

            string hint = hasCurrentVehicle
                ? "assigned"
                : pathCount > 0
                    ? "has path"
                    : "no path yet";

            return $"currentVehicle {vehicleText}; path {pathCount} elems idx {pathIndex}; hint {hint}";
        }

        private static SkippedPassengerSampleRing GetSkippedSampleRing(TransportType transportType)
        {
            switch (transportType)
            {
                case TransportType.Bus:
                    return s_BusSkippedSamples;
                case TransportType.Train:
                    return s_TrainSkippedSamples;
                case TransportType.Tram:
                    return s_TramSkippedSamples;
                case TransportType.Subway:
                    return s_SubwaySkippedSamples;
                case TransportType.Ship:
                    return s_ShipSkippedSamples;
                case TransportType.Ferry:
                    return s_FerrySkippedSamples;
                case TransportType.Airplane:
                    return s_AirSkippedSamples;
                default:
                    return s_BusSkippedSamples;
            }
        }

        private static void AppendSectionHeader(StringBuilder sb, string title)
        {
            AppendDivider(sb);
            sb.AppendLine(title);
            AppendDivider(sb);
        }

        private static void AppendSubHeader(StringBuilder sb, string title)
        {
            sb.AppendLine("-- " + title + " --");
        }

        private static void AppendDivider(StringBuilder sb)
        {
            sb.AppendLine(new string('=', ReportHeaderWidth));
        }

        private static void AppendField(StringBuilder sb, string label, string value)
        {
            sb.Append(label);
            if (label.Length < 32)
            {
                sb.Append('.', 32 - label.Length);
            }

            sb.Append(": ");
            sb.AppendLine(value);
        }

        private static string EntityText(Entity entity)
        {
            return entity == Entity.Null ? Localize(KeyReportNone, "none") : entity.ToString();
        }

        private static string TextOrUnknown(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? Localize(KeyReportUnknown, "(unknown)") : value;
        }

        private static string FormatReport(string entryId, string fallbackFormat, params object[] args)
        {
            return LocaleUtils.SafeFormat(entryId, fallbackFormat, args);
        }

        private static string FormatDuration(int seconds)
        {
            seconds = Math.Max(0, seconds);
            if (seconds < 60)
            {
                return seconds + "s";
            }

            int minutes = seconds / 60;
            int remainingSeconds = seconds % 60;
            return remainingSeconds == 0
                ? minutes + "m"
                : minutes + "m" + remainingSeconds + "s";
        }

        private static void EnsureCounterDay(World world)
        {
            SimulationSystem? simulationSystem = world.GetExistingSystemManaged<SimulationSystem>();
            if (simulationSystem == null)
            {
                return;
            }

            if (s_LastSimulationFrame != uint.MaxValue && simulationSystem.frameIndex < s_LastSimulationFrame)
            {
                // Lower frame index means a different load/city; reset stale daily counters.
                s_CurrentDayKey = int.MinValue;
                ResetDailyCounters();
            }

            s_LastSimulationFrame = simulationSystem.frameIndex;

            TimeSystem? timeSystem = world.GetExistingSystemManaged<TimeSystem>() ??
                world.GetOrCreateSystemManaged<TimeSystem>();
            if (timeSystem == null)
            {
                return;
            }

            DateTime currentDate = timeSystem.GetCurrentDateTime();
            int dayKey = (currentDate.Year * 1000) + currentDate.DayOfYear;
            if (dayKey == s_CurrentDayKey)
            {
                return;
            }

            // "Skipped today" follows the in-game calendar day.
            s_CurrentDayKey = dayKey;
            ResetDailyCounters();
        }

        private static void ResetDailyCounters()
        {
            s_BusLateBoardersToday = 0;
            s_TrainLateBoardersToday = 0;
            s_TramLateBoardersToday = 0;
            s_SubwayLateBoardersToday = 0;
            s_ShipLateBoardersToday = 0;
            s_FerryLateBoardersToday = 0;
            s_AirLateBoardersToday = 0;
            s_BusSkippedSamples.Clear();
            s_TrainSkippedSamples.Clear();
            s_TramSkippedSamples.Clear();
            s_SubwaySkippedSamples.Clear();
            s_ShipSkippedSamples.Clear();
            s_FerrySkippedSamples.Clear();
            s_AirSkippedSamples.Clear();
        }

        private static string Localize(string entryId, string fallback) => LocaleUtils.Localize(entryId, fallback);
    }
}
