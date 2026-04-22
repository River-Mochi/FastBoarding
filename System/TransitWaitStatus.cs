// File: System/TransitWaitStatus.cs
// Purpose: Cached Options UI status text for current transit wait snapshots.

namespace BoardingTime
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
        public const string KeyStatusNotLoaded = "BT_STATUS_NOT_LOADED";
        public const string KeyNoCityLoaded = "BT_STATUS_NO_CITY_LOADED";
        public const string KeyNoStopsFound = "BT_STATUS_NO_STOPS";
        public const string KeyStatusLine = "BT_STATUS_LINE";
        public const string KeyReportNoCityLoaded = "BT_REPORT_NO_CITY_LOADED";
        public const string KeyReportTitle = "BT_REPORT_TITLE";
        public const string KeyReportSettings = "BT_REPORT_SETTINGS";
        public const string KeyReportNote = "BT_REPORT_NOTE";
        public const string KeyReportFamilyHeader = "BT_REPORT_FAMILY_HEADER";
        public const string KeyReportServedStops = "BT_REPORT_SERVED_STOPS";
        public const string KeyReportStopsWithWaiting = "BT_REPORT_STOPS_WITH_WAITING";
        public const string KeyReportWaitingPassengers = "BT_REPORT_WAITING_PASSENGERS";
        public const string KeyReportAverageWait = "BT_REPORT_AVERAGE_WAIT";
        public const string KeyReportLateBoardersSkipped = "BT_REPORT_LATE_BOARDERS_SKIPPED";
        public const string KeyReportWorstStopNone = "BT_REPORT_WORST_STOP_NONE";
        public const string KeyReportWorstStopAverageWait = "BT_REPORT_WORST_STOP_AVERAGE_WAIT";
        public const string KeyReportWorstStopName = "BT_REPORT_WORST_STOP_NAME";
        public const string KeyReportWorstStopEntity = "BT_REPORT_WORST_STOP_ENTITY";
        public const string KeyReportWorstWaypointEntity = "BT_REPORT_WORST_WAYPOINT_ENTITY";
        public const string KeyReportWorstLineHint = "BT_REPORT_WORST_LINE_HINT";
        public const string KeyReportWorstLineEntity = "BT_REPORT_WORST_LINE_ENTITY";
        public const string KeyReportWorstLineWaypointAverage = "BT_REPORT_WORST_LINE_WAYPOINT_AVERAGE";
        public const string KeyReportTopWorstStopsHeader = "BT_REPORT_TOP_WORST_STOPS_HEADER";
        public const string KeyReportTopWorstStopLine = "BT_REPORT_TOP_WORST_STOP_LINE";
        public const string KeyReportLateGroups = "BT_REPORT_LATE_GROUPS";
        public const string KeyReportNone = "BT_REPORT_NONE";
        public const string KeyReportUnknown = "BT_REPORT_UNKNOWN";

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
                    "BT_STATUS_SNAPSHOT_EXCEPTION",
                    () => $"[BT] Transit status snapshot failed: {ex.GetType().Name}: {ex.Message}");
            }
        }

        public static void LogDetailedReport()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            GameManager gm = GameManager.instance;
            if (world == null || !world.IsCreated || !gm.gameMode.IsGame())
            {
                LogUtils.Info(Mod.s_Log, () => Localize(KeyReportNoCityLoaded, "[BT] Stats report requested, but no city is loaded."));
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
                sb.AppendLine(Localize(KeyReportTitle, "========== [BT] TRANSIT BOARDING STATUS REPORT =========="));
                sb.AppendLine(FormatReport(KeyReportSettings, "Settings: {0}", BoardingRuntimeSettings.DescribeForLog()));
                sb.AppendLine(Localize(KeyReportNote, "Note: Worst line is a hint from the highest-wait waypoint at the worst stop."));

                AppendFamilyReport(sb, "Bus", snapshot.Bus, s_BusLateBoardersToday);
                AppendFamilyReport(sb, "Tram", snapshot.Tram, s_TramLateBoardersToday);
                AppendFamilyReport(sb, "Train", snapshot.Train, s_TrainLateBoardersToday);
                AppendFamilyReport(sb, "Subway", snapshot.Subway, s_SubwayLateBoardersToday);
                AppendFamilyReport(sb, "Ferry", snapshot.Ferry, s_FerryLateBoardersToday);
                AppendFamilyReport(sb, "Ship", snapshot.Ship, s_ShipLateBoardersToday);
                AppendFamilyReport(sb, "Airplane", snapshot.Air, s_AirLateBoardersToday);

                sb.AppendLine("==========================================================");

                LogUtils.Info(Mod.s_Log, () => sb.ToString());
            }
            catch (Exception ex)
            {
                LogUtils.Warn(Mod.s_Log, () => $"[BT] Stats report failed: {ex.GetType().Name}: {ex.Message}", ex);
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
            sb.AppendLine(FormatReport(KeyReportFamilyHeader, "--- {0} ---", label));

            if (family.StopCount == 0)
            {
                sb.AppendLine(FormatReport(KeyReportServedStops, "Served stops: {0}", "0"));
                sb.AppendLine(Localize(KeyNoStopsFound, "No stops found."));
                return;
            }

            sb.AppendLine(FormatReport(KeyReportServedStops, "Served stops: {0}", LocaleUtils.FormatN0(family.StopCount)));
            sb.AppendLine(FormatReport(KeyReportStopsWithWaiting, "Stops with waiting passengers: {0}", LocaleUtils.FormatN0(family.ActiveQueueStops)));
            sb.AppendLine(FormatReport(KeyReportWaitingPassengers, "Waiting passengers: {0}", LocaleUtils.FormatN0(family.WaitingPassengers)));
            sb.AppendLine(FormatReport(KeyReportAverageWait, "Average wait: {0}", FormatDuration(family.AverageWaitSeconds)));
            sb.AppendLine(FormatReport(KeyReportLateBoardersSkipped, "Late boarders skipped today: {0}", LocaleUtils.FormatN0(lateBoardersCanceledToday)));
            sb.AppendLine(FormatReport(
                KeyReportLateGroups,
                "Late group passengers left alone: {0} passengers in {1} groups on {2} vehicles",
                LocaleUtils.FormatN0(family.LateGroupPassengers),
                LocaleUtils.FormatN0(family.LateGroupGroups),
                LocaleUtils.FormatN0(family.LateGroupVehicles)));

            if (family.WaitingPassengers <= 0)
            {
                sb.AppendLine(Localize(KeyReportWorstStopNone, "Worst stop: none, no waiting passengers right now."));
                return;
            }

            sb.AppendLine(FormatReport(KeyReportWorstStopAverageWait, "Worst stop avg wait: {0}", FormatDuration(family.WorstStopWaitSeconds)));
            sb.AppendLine(FormatReport(KeyReportWorstStopName, "Worst stop name: {0}", TextOrUnknown(family.WorstStopName)));
            sb.AppendLine(FormatReport(KeyReportWorstStopEntity, "Worst stop entity: {0}", EntityText(family.WorstStopEntity)));
            sb.AppendLine(FormatReport(KeyReportWorstWaypointEntity, "Worst waypoint entity: {0}", EntityText(family.WorstWaypointEntity)));
            sb.AppendLine(FormatReport(KeyReportWorstLineHint, "Worst line hint: {0}", TextOrUnknown(family.WorstLineName)));
            sb.AppendLine(FormatReport(KeyReportWorstLineEntity, "Worst line entity: {0}", EntityText(family.WorstLineEntity)));
            sb.AppendLine(FormatReport(
                KeyReportWorstLineWaypointAverage,
                "Worst line waypoint avg: {0} with {1} waiting",
                FormatDuration(family.WorstLineWaitSeconds),
                LocaleUtils.FormatN0(family.WorstLineWaitingPassengers)));

            AppendTopWorstStops(sb, family);
        }

        private static void AppendTopWorstStops(StringBuilder sb, TransitWaitStatusSystem.FamilySnapshot family)
        {
            if (family.TopWorstStops.Length == 0)
            {
                return;
            }

            sb.AppendLine(FormatReport(
                KeyReportTopWorstStopsHeader,
                "Top {0} worst stops by average wait:",
                LocaleUtils.FormatN0(family.TopWorstStops.Length)));

            for (int i = 0; i < family.TopWorstStops.Length; i++)
            {
                TransitWaitStatusSystem.WorstStopSnapshot stop = family.TopWorstStops[i];
                sb.AppendLine(FormatReport(
                    KeyReportTopWorstStopLine,
                    "{0}. {1} | avg {2} | waiting {3} | stop entity {4} | line {5} | line entity {6} | waypoint {7}",
                    LocaleUtils.FormatN0(i + 1),
                    TextOrUnknown(stop.StopName),
                    FormatDuration(stop.AverageWaitSeconds),
                    LocaleUtils.FormatN0(stop.WaitingPassengers),
                    EntityText(stop.StopEntity),
                    TextOrUnknown(stop.LineName),
                    EntityText(stop.LineEntity),
                    EntityText(stop.WaypointEntity)));
            }
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
