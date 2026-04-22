// File: System/Status/TransitWaitStatus.cs
// Purpose: Cached Options UI status text for current transit wait snapshots.

namespace FastBoarding
{
    using Game;
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
        public const string KeyReportNoCityLoaded = "FB_REPORT_NO_CITY_LOADED";
        public const string KeyReportTitle = "FB_REPORT_TITLE";
        public const string KeyReportSettings = "FB_REPORT_SETTINGS";
        public const string KeyReportNote = "FB_REPORT_NOTE";
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
        public const string KeyReportNone = "FB_REPORT_NONE";
        public const string KeyReportUnknown = "FB_REPORT_UNKNOWN";

        private const int ReportHeaderWidth = 60;

        public static int RefreshIntervalSeconds { get; set; } = 15;

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

        public static void InvalidateCache()
        {
            s_HasSnapshotThisCity = false;
            s_LastRefreshTicksUtc = 0;
            s_LastUiFrame = -1;
            ResetDailyCounters();

            BusSummary = Localize(KeyStatusNotLoaded, "Status not loaded.");
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
                // Loading/unloading a city means old counters and cached stop entities are stale.
                s_WasInGame = isGame;
                InvalidateCache();
            }

            if (!isGame)
            {
                BusSummary = Localize(KeyNoCityLoaded, "No city loaded.");
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
                AppendField(sb, "Generated local", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                AppendField(sb, "Settings", BoardingRuntimeSettings.DescribeForLog());
                AppendField(sb, "Note", Localize(KeyReportNote, "Worst line is a hint from the highest-wait waypoint at the worst stop."));

                AppendFamilyReport(sb, "Bus", snapshot.Bus, s_BusLateBoardersToday);
                AppendFamilyReport(sb, "Tram", snapshot.Tram, s_TramLateBoardersToday);
                AppendFamilyReport(sb, "Train", snapshot.Train, s_TrainLateBoardersToday);
                AppendFamilyReport(sb, "Subway", snapshot.Subway, s_SubwayLateBoardersToday);
                AppendFamilyReport(sb, "Ferry", snapshot.Ferry, s_FerryLateBoardersToday);
                AppendFamilyReport(sb, "Ship", snapshot.Ship, s_ShipLateBoardersToday);
                AppendFamilyReport(sb, "Airplane", snapshot.Air, s_AirLateBoardersToday);

                AppendDivider(sb);

                LogUtils.Info(Mod.s_Log, () => sb.ToString());
            }
            catch (Exception ex)
            {
                LogUtils.Warn(Mod.s_Log, () => $"{Mod.ModTag} Stats report failed: {ex.GetType().Name}: {ex.Message}", ex);
            }
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
            BusSummary = FormatFamily(snapshot.Bus, s_BusLateBoardersToday);
            TrainSummary = FormatFamily(snapshot.Train, s_TrainLateBoardersToday);
            TramSummary = FormatFamily(snapshot.Tram, s_TramLateBoardersToday);
            SubwaySummary = FormatFamily(snapshot.Subway, s_SubwayLateBoardersToday);
            ShipSummary = FormatFamily(snapshot.Ship, s_ShipLateBoardersToday);
            FerrySummary = FormatFamily(snapshot.Ferry, s_FerryLateBoardersToday);
            AirSummary = FormatFamily(snapshot.Air, s_AirLateBoardersToday);
        }

        private static void AppendFamilyReport(
            StringBuilder sb,
            string label,
            TransitWaitStatusSystem.FamilySnapshot family,
            long lateBoardersCanceledToday)
        {
            sb.AppendLine();
            AppendSectionHeader(sb, FormatReport(KeyReportFamilyHeader, "{0}", label));

            if (family.StopCount == 0)
            {
                AppendField(sb, "Served stops", "0");
                sb.AppendLine(Localize(KeyNoStopsFound, "No stops found."));
                return;
            }

            AppendField(sb, "Served stops", LocaleUtils.FormatN0(family.StopCount));
            AppendField(sb, "Stops with waiting", LocaleUtils.FormatN0(family.ActiveQueueStops));
            AppendField(sb, "Waiting passengers", LocaleUtils.FormatN0(family.WaitingPassengers));
            AppendField(sb, "Average wait", FormatDuration(family.AverageWaitSeconds));
            AppendField(sb, "Late boarders skipped today", LocaleUtils.FormatN0(lateBoardersCanceledToday));
            AppendField(
                sb,
                "Late group passengers left alone",
                $"{LocaleUtils.FormatN0(family.LateGroupPassengers)} passengers in {LocaleUtils.FormatN0(family.LateGroupGroups)} groups on {LocaleUtils.FormatN0(family.LateGroupVehicles)} vehicles");

            if (family.WaitingPassengers <= 0)
            {
                sb.AppendLine(Localize(KeyReportWorstStopNone, "Worst stop: none, no waiting passengers right now."));
                return;
            }

            sb.AppendLine();
            AppendSubHeader(sb, "Worst stop");
            AppendField(sb, "Average wait", FormatDuration(family.WorstStopWaitSeconds));
            AppendField(sb, "Name", TextOrUnknown(family.WorstStopName));
            AppendField(sb, "Stop entity", EntityText(family.WorstStopEntity));
            AppendField(sb, "Waypoint entity", EntityText(family.WorstWaypointEntity));
            AppendField(sb, "Line hint", TextOrUnknown(family.WorstLineName));
            AppendField(sb, "Line entity", EntityText(family.WorstLineEntity));
            AppendField(
                sb,
                "Worst line waypoint avg",
                $"{FormatDuration(family.WorstLineWaitSeconds)} with {LocaleUtils.FormatN0(family.WorstLineWaitingPassengers)} waiting");

            AppendTopWorstStops(sb, family);
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
                sb.AppendLine($"{LocaleUtils.FormatN0(i + 1)}. {TextOrUnknown(stop.StopName)}");
                AppendField(sb, "   Average wait", FormatDuration(stop.AverageWaitSeconds));
                AppendField(sb, "   Waiting", LocaleUtils.FormatN0(stop.WaitingPassengers));
                AppendField(sb, "   Stop entity", EntityText(stop.StopEntity));
                AppendField(sb, "   Waypoint entity", EntityText(stop.WaypointEntity));
                AppendField(sb, "   Line entity", EntityText(stop.LineEntity));
                AppendField(sb, "   Line hint", TextOrUnknown(stop.LineName));
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
        }

        private static string Localize(string entryId, string fallback) => LocaleUtils.Localize(entryId, fallback);
    }
}
