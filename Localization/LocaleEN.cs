// File: Localization/LocaleEN.cs
// Purpose: English en-US locale entries for Fast Boarding.

namespace FastBoarding
{
    using Colossal;
    using System.Collections.Generic;

    /// <summary>
    /// English localization source.
    /// </summary>
    public sealed class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        /// <summary>
        /// Constructs the English locale.
        /// </summary>
        /// <param name="setting">Settings object used for locale IDs.</param>
        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        /// <summary>
        /// Creates all English localization entries for this mod.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            string title = Mod.ModName;

            if (!string.IsNullOrEmpty(Mod.ModVersion))
            {
                title = title + " (" + Mod.ModVersion + ")";
            }

            // One helper keeps all seven status tooltips in sync for future translations.
            string StatusDescription(string transitName)
            {
                return
                    $"<Current {transitName} status>\n" +
                    "**Waiting** = total passengers waiting right now.\n" +
                    "**Avg** = average wait time for those passengers.\n" +
                    "**Worst** stop = highest average wait at one stop.\n" +
                    "Worst stops are good places to inspect for traffic accidents, blocked/bugged stops, or vehicles held up nearby.\n" +
                    "**Skipped** = late boardings canceled today by toggle.\n" +
                    "Use <Stats to Log> for detailed report: stop names, entity IDs, and more.";
            }

            return new Dictionary<string, string>
            {
                // Options mod name
                { m_Setting.GetSettingsLocaleID(), title },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Actions" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "About" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.SpeedGroup), "Boarding speed" },
                { m_Setting.GetOptionGroupLocaleID(Setting.BehaviorGroup), "Behavior" },
                { m_Setting.GetOptionGroupLocaleID(Setting.StatusGroup), "Status" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGroup), "Mod info" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGroup), "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.DebugGroup), "Debug" },

                // Boarding speed sliders
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BusBoardingSpeedFactor)), "Bus boarding speed" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BusBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Higher values reduce bus stop boarding/loading time.\n" +
                    "This helps normal queues clear faster, but a late passenger can still delay departure because of vanilla design.\n" +
                    "Use [✓] <Let vehicles leave> if you want the bus to not be forced to wait for late cims.\n" +
                    "2x means ~double boarding speed.\n" +
                    "Tech notes: higher loading factor means shorter planned stop duration, and boarding time is more like passenger-side wait/boarding estimate.\n" +
                    "This is not the same as forcing the vehicle to leave."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RailBoardingSpeedFactor)), "Rail boarding speed" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RailBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Applies to train, tram, and subway stops.\n" +
                    "Higher values reduce boarding/loading time at rail stops.\n" +
                    "This helps normal queues clear faster, but a late passenger can still delay departure because of vanilla design.\n" +
                    "Use [✓] <Let vehicles leave> to allow vehicle to leave if it's past departure time.\n" +
                    "vanilla will naturally reroute the cim.\n" +
                    "2x means ~double boarding speed."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WaterBoardingSpeedFactor)), "Ship + ferry speed" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WaterBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Applies to ship and ferry stops.\n" +
                    "Higher values reduce boarding/loading time at ship and ferry stops.\n" +
                    "This helps normal queues clear faster, but a late passenger can still delay departure because of vanilla design.\n" +
                    "Use [✓] <Let vehicles leave> to allow vehicle to leave if it's past departure time.\n" +
                    "2x means ~double boarding speed."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AirBoardingSpeedFactor)), "Airplane speed" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AirBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Applies to passenger airplane terminals.\n" +
                    "Higher values reduce boarding/loading time at airplane terminals.\n" +
                    "This helps normal queues clear faster, but a late passenger can still delay departure because of vanilla design.\n" +
                    "Use [✓] <Let vehicles leave> so that late cims don't hold up the vehicle.\n" +
                    "2x means ~double boarding speed."
                },

                // Late passenger behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CancelLateBoarders)), "Let vehicles leave without late cims" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CancelLateBoarders)),
                    "Late passengers who are still <not ready> after vanilla Departure time are allowed to miss the vehicle.\n" +
                    "Note: we only skip solo late citizens for now.\n" +
                    "Groups/families travelling together and LATE are <not skipped> and may still cause delays to transit like in vanilla.\n" +
                    "Groups are a very small number compared to many single passengers.\n" +
                    "Skipped Late citizens are not deleted; vanilla systems continue from there to assign them."
                },

                // Status overview
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusOverview)), "Total usage" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusOverview)),
                    "Monthly public transit usage from the game's Transportation infoview.\n" +
                    "Updated time shows when this status snapshot was taken (usually after entering Options menu)."
                },

                // Status rows
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusBus)), "Bus" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusBus)), StatusDescription("bus") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTram)), "Tram" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTram)), StatusDescription("tram") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTrain)), "Train" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTrain)), StatusDescription("train") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSubway)), "Subway" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSubway)), StatusDescription("subway") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusFerry)), "Ferry" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusFerry)), StatusDescription("ferry") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusShip)), "Ship" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusShip)), StatusDescription("ship") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusAir)), "Airplane" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusAir)), StatusDescription("airplane") },

                // Status buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatsToLog)), "Stats to Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatsToLog)),
                    "Writes a detailed one-time report to **FastBoarding.log**.\n" +
                    "Includes waiting totals, top 3 worst stops per mode, skipped cim examples, entity IDs, and line hints."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Open Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Opens **FastBoarding.log** if it exists.\n" +
                    "If the file is not found yet, opens the Logs folder instead."
                },

                // Runtime status strings
                { TransitWaitStatus.KeyStatusNotLoaded, "Status not loaded." },
                { TransitWaitStatus.KeyNoCityLoaded, "No city loaded." },
                { TransitWaitStatus.KeyNoStopsFound, "No stops found." },
                { TransitWaitStatus.KeyStatusLine, "{0} wait | avg {1} | worst {2} | {3} skipped" },
                { TransitWaitStatus.KeyStatusOverviewLine, "{0} tourist/mo | {1} citizens/mo | updated {2}" },

                // Stats-to-log report strings
                { TransitWaitStatus.KeyReportNoCityLoaded, "[FB] Stats report requested, but no city is loaded." },
                { TransitWaitStatus.KeyReportTitle, "Stats to Log snapshot - Fast Boarding" },
                { TransitWaitStatus.KeyReportSettings, "Settings: {0}" },
                { TransitWaitStatus.KeyReportNote, "Line hint comes from the highest-wait waypoint at that stop." },
                { TransitWaitStatus.KeyReportTesterHintsHeader, "Tester hints" },
                { TransitWaitStatus.KeyReportHintWorstStops, "Worst stops: inspect these first in-game or with Scene Explorer mod. Look for accidents, traffic, bad transit stop location, or a bugged stop." },
                { TransitWaitStatus.KeyReportHintSkippedCims, "Skipped solo cims: late passengers we skip to allow transit to leave. later state should usually become 'has path' or 'assigned'. If it stays 'no path yet', inspect that cim entity after more time." },
                { TransitWaitStatus.KeyReportHintLateGroups, "Late groups: these are families/groups left to vanilla. High counts are clues for future safe group-travel support." },
                { TransitWaitStatus.KeyReportFamilyHeader, "{0}" },
                { TransitWaitStatus.KeyReportServedStops, "Served stops: {0}" },
                { TransitWaitStatus.KeyReportStopsWithWaiting, "Stops with waiting passengers: {0}" },
                { TransitWaitStatus.KeyReportWaitingPassengers, "Waiting passengers: {0}" },
                { TransitWaitStatus.KeyReportAverageWait, "Average wait: {0}" },
                { TransitWaitStatus.KeyReportLateBoardersSkipped, "Late boarders skipped today: {0}" },
                { TransitWaitStatus.KeyReportWorstStopNone, "Worst stop: none, no waiting passengers right now." },
                { TransitWaitStatus.KeyReportWorstStopAverageWait, "Worst stop avg wait: {0}" },
                { TransitWaitStatus.KeyReportWorstStopName, "Worst stop name: {0}" },
                { TransitWaitStatus.KeyReportWorstStopEntity, "Worst stop entity: {0}" },
                { TransitWaitStatus.KeyReportWorstWaypointEntity, "Worst waypoint entity: {0}" },
                { TransitWaitStatus.KeyReportWorstLineHint, "Worst line hint: {0}" },
                { TransitWaitStatus.KeyReportWorstLineEntity, "Worst line entity: {0}" },
                { TransitWaitStatus.KeyReportWorstLineWaypointAverage, "Worst line waypoint avg: {0} with {1} waiting" },
                { TransitWaitStatus.KeyReportTopWorstStopsHeader, "Top {0} worst stops by average wait:" },
                { TransitWaitStatus.KeyReportTopWorstStopLine, "{0}. {1} | avg {2} | waiting {3} | stop entity {4} | waypoint entity {5} | line entity {6} | line hint {7}" },
                { TransitWaitStatus.KeyReportLateGroups, "Late group passengers left alone: {0} passengers in {1} groups on {2} vehicles" },
                { TransitWaitStatus.KeyReportLastSkippedSamplesHeader, "Skipped solo Late cim examples at this CURRENT time" },
                { TransitWaitStatus.KeyReportLastSkippedSampleLine, "{0}. {1} | passenger {2} | missed vehicle {3} | time {4} | now {5}" },
                { TransitWaitStatus.KeyReportNone, "none" },
                { TransitWaitStatus.KeyReportUnknown, "(unknown)" },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Display name of this mod." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Current mod version." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Opens the author's Paradox Mods page." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableVerboseLogging)), "Enable verbose logging" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableVerboseLogging)),
                    "**Debug / testing only**\n" +
                    "Adds live diagnostic details to <FastBoarding.log> while the city runs.\n" +
                    "**Do not enable for normal gameplay.**\n" +
                    "Leaving this on can decrease performance and create huge log files.\n" +
                    "You can delete old log files later.\n" +
                    "Note: since the [Stats to Logs] button is Current right now only snapshot of data, \n" +
                    "  running verbose logging for 15-30 min would give you more details of transactions over time."

                },
            };
        }

        public void Unload()
        {
        }
    }
}
