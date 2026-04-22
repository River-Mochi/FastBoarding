// File: Localization/LocaleEN.cs
// Purpose: English en-US locale entries for Boarding Time.

namespace BoardingTime
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

            string StatusDescription(string transitName)
            {
                return
                    $"<Current {transitName} status>\n" +
                    "**Waiting** = total passengers waiting right now.\n" +
                    "**Avg** = average wait time for those passengers.\n" +
                    "**Worst** stop = highest average wait at one stop.\n" +
                    "**Skipped** = late boardings canceled today by toggle.\n" +
                    "Use <Stats to Log> for detailed report: stop names, entity IDs, and more.";
            }

            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), title },

                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Actions" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "About" },

                { m_Setting.GetOptionGroupLocaleID(Setting.SpeedGroup), "Boarding speed" },
                { m_Setting.GetOptionGroupLocaleID(Setting.BehaviorGroup), "Behavior" },
                { m_Setting.GetOptionGroupLocaleID(Setting.StatusGroup), "Status" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGroup), "Mod info" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGroup), "Links" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BusBoardingSpeedFactor)), "Bus boarding speed" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BusBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Higher values make buses load faster and leave sooner.\n" +
                    "2x means ~double boarding speed."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RailBoardingSpeedFactor)), "Rail boarding speed" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RailBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Applies to train, tram, and subway stops.\n" +
                    "Higher values means load faster and leave sooner.\n" +
                    "2x means ~double boarding speed"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WaterBoardingSpeedFactor)), "Ship + ferry speed" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WaterBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Applies to ship and ferry stops.\n" +
                    "Higher values means load faster and leave sooner.\n" +
                    "2x means ~double boarding speed."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AirBoardingSpeedFactor)), "Airplane speed" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AirBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Applies to passenger airplane terminals.\n" +
                    "Higher values means load faster and leave sooner.\n" +
                    "2x means ~double boarding speed."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CancelLateBoarders)), "Cancel late boarders" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CancelLateBoarders)),
                    "**BETA Experimental**\n" +
                    "Solo passengers who are still not ready after departure time can miss the vehicle.\n" +
                    "The mod clears the missed boarding attempt so vanilla simply reassigns them."
                },

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

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatsToLog)), "Stats to Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatsToLog)),
                    "Writes a detailed one-time report to **BoardingTime.log**.\n" +
                    "Includes waiting totals, top 5 worst stops per mode, entity IDs, and line hints."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Open Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Opens **BoardingTime.log** if it exists.\n" +
                    "If the file is not found yet, opens the Logs folder instead."
                },

                { TransitWaitStatus.KeyStatusNotLoaded, "Status not loaded." },
                { TransitWaitStatus.KeyNoCityLoaded, "No city loaded." },
                { TransitWaitStatus.KeyNoStopsFound, "No stops found." },
                { TransitWaitStatus.KeyStatusLine, "{0} wait | avg {1} | worst {2} | {3} skipped" },
                { TransitWaitStatus.KeyReportNoCityLoaded, "[BT] Stats report requested, but no city is loaded." },
                { TransitWaitStatus.KeyReportTitle, "========== [BT] TRANSIT BOARDING STATUS REPORT ==========" },
                { TransitWaitStatus.KeyReportSettings, "Settings: {0}" },
                { TransitWaitStatus.KeyReportNote, "Note: Worst line is a hint from the highest-wait waypoint at the worst stop." },
                { TransitWaitStatus.KeyReportFamilyHeader, "--- {0} ---" },
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
                { TransitWaitStatus.KeyReportTopWorstStopLine, "{0}. {1} | avg {2} | waiting {3} | stop entity {4} | line {5} | line entity {6} | waypoint {7}" },
                { TransitWaitStatus.KeyReportLateGroups, "Late group passengers left alone: {0} passengers in {1} groups on {2} vehicles" },
                { TransitWaitStatus.KeyReportNone, "none" },
                { TransitWaitStatus.KeyReportUnknown, "(unknown)" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Display name of this mod." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Current mod version." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Opens the author's Paradox Mods page." }
            };
        }

        public void Unload()
        {
        }
    }
}
