// File: Localization/LocaleDE.cs
// Purpose: German de-DE locale entries for Fast Boarding.

namespace FastBoarding
{
    using Colossal;
    using System.Collections.Generic;

    /// <summary>
    /// English localization source.
    /// </summary>
    public sealed class LocaleDE : IDictionarySource
    {
        private readonly Setting m_Setting;

        /// <summary>
        /// Constructs the English locale.
        /// </summary>
        /// <param name="setting">Settings object used for locale IDs.</param>
        public LocaleDE(Setting setting)
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

            const string ToggleLabel = "Verspätete Fahrgäste überspringen";

            // One helper keeps all seven status tooltips in sync for future translations.
            string StatusDescription(string transitName)
            {
                return
                    $"<Aktueller {transitName}-Status>\n" +
                    "**Wartend** = alle Fahrgäste, die gerade warten.\n" +
                    "**Schnitt** = durchschnittliche Wartezeit dieser Fahrgäste.\n" +
                    "**Schlimmster** Halt = höchste durchschnittliche Wartezeit an einem Halt.\n" +
                    "Schlimmste Halte sind gute Stellen, um Unfälle, blockierte/verbuggte Halte oder festhängende Fahrzeuge in der Nähe zu prüfen.\n" +
                    $"**Übersprungen** = heute durch <{ToggleLabel}> übersprungene späte Solo-Fahrgäste.\n" +
                    "Nutze <Stats ins Log> für Details: Haltestellennamen, Entity-IDs und mehr.";
            }

            return new Dictionary<string, string>
            {
                // Options mod name
                { m_Setting.GetSettingsLocaleID(), title },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Aktionen" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "Info" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.SpeedGroup), "Einstiegsgeschwindigkeit" },
                { m_Setting.GetOptionGroupLocaleID(Setting.BehaviorGroup), "Verhalten" },
                { m_Setting.GetOptionGroupLocaleID(Setting.StatusGroup), "Status" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGroup), "Mod-Info" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGroup), "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.DebugGroup), "Debug" },

                // Boarding speed sliders
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BusBoardingSpeedFactor)), "Bus-Einstieg" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BusBoardingSpeedFactor)),
                    "<1x = Vanilla>\n" +
                    "Höhere Werte verkürzen Ein- und Ladezeit an Bushaltestellen.\n" +
                    "Normale Warteschlangen bauen sich schneller ab, aber verspätete Fahrgäste können die Abfahrt im Vanilla-Design weiter verzögern.\n" +
                    $"Nutze [✓] <{ToggleLabel}>, damit Busse nicht auf späte Cims warten.\n" +
                    "2x bedeutet ungefähr doppelte Einstiegsgeschwindigkeit.\n" +
                    "Technik: höherer Loading-Factor = kürzere geplante Haltezeit; Boarding Time ist eher die Fahrgast-Schätzung für Warten/Einsteigen.\n" +
                    $"Das ist nicht dasselbe wie <{ToggleLabel}>; diese Checkbox entscheidet, ob späte Cims das Fahrzeug nach der Abfahrtszeit verpassen dürfen.\n" +
                    "<==========================>\n" +
                    "Loading-Factor für alle Transitarten:\n" +
                    "1x  = 100% Vanilla-Haltezeit\n" +
                    "2x  = ~ 1/2 geplante Haltezeit\n" +
                    "4x  = ~ 1/4 geplante Haltezeit\n" +
                    "10x = ~ 1/10 geplante Haltezeit"

                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RailBoardingSpeedFactor)), "Bahn-Einstieg" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RailBoardingSpeedFactor)),
                    "<1x = Vanilla>\n" +
                    "Gilt für Zug-, Tram- und U-Bahn-Halte.\n" +
                    "Höhere Werte verkürzen Ein- und Ladezeit an Schienenhalten.\n" +
                    "Normale Warteschlangen bauen sich schneller ab, aber verspätete Fahrgäste können die Abfahrt im Vanilla-Design weiter verzögern.\n" +
                    $"Nutze [✓] <{ToggleLabel}>, wenn späte Cims das Fahrzeug nach der Abfahrtszeit verpassen sollen.\n" +
                    "Das Spiel weist den Cim danach normalerweise neu zu.\n" +
                    "2x bedeutet ungefähr doppelte Einstiegsgeschwindigkeit."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WaterBoardingSpeedFactor)), "Schiff + Fähre" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WaterBoardingSpeedFactor)),
                    "<1x = Vanilla>\n" +
                    "Gilt für Schiffs- und Fährhaltestellen.\n" +
                    "Höhere Werte verkürzen Ein- und Ladezeit an Schiffs- und Fährhalten.\n" +
                    "Normale Warteschlangen bauen sich schneller ab, aber verspätete Fahrgäste können die Abfahrt im Vanilla-Design weiter verzögern.\n" +
                    $"Nutze [✓] <{ToggleLabel}>, wenn späte Cims das Fahrzeug nach der Abfahrtszeit verpassen sollen.\n" +
                    "2x bedeutet ungefähr doppelte Einstiegsgeschwindigkeit."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AirBoardingSpeedFactor)), "Flugzeug-Einstieg" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AirBoardingSpeedFactor)),
                    "<1x = Vanilla>\n" +
                    "Gilt für Passagierflugzeug-Terminals.\n" +
                    "Höhere Werte verkürzen Ein- und Ladezeit an Flugzeug-Terminals.\n" +
                    "Normale Warteschlangen bauen sich schneller ab, aber verspätete Fahrgäste können die Abfahrt im Vanilla-Design weiter verzögern.\n" +
                    $"Nutze [✓] <{ToggleLabel}>, wenn späte Cims das Fahrzeug nach der Abfahrtszeit verpassen sollen.\n" +
                    "2x bedeutet ungefähr doppelte Einstiegsgeschwindigkeit."
                },

                // Late passenger behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CancelLateBoarders)), ToggleLabel },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CancelLateBoarders)),
                    "Verspätete Fahrgäste, die nach der Abfahrtszeit noch <nicht bereit> sind, dürfen das Fahrzeug verpassen.\n" +
                    "Hinweis: Aktuell überspringen wir nur einzelne verspätete Bürger.\n" +
                    "Gruppen/Familien, die zusammen reisen und zu spät sind, werden <nicht übersprungen> und können wie in Vanilla weiter verzögern.\n" +
                    "Gruppen sind nur ein kleiner Teil der Menge; der Hauptnutzen kommt vom Überspringen verspäteter Solo-Cims.\n" +
                    "Übersprungene späte Bürger werden nicht gelöscht; das Spiel weist sie danach natürlich neu zu."
                },

                // Status overview
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusOverview)), "Gesamtnutzung" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusOverview)),
                    "Monatliche ÖPNV-Nutzung aus der Transport-Infoview des Spiels.\n" +
                    "Aktualisiert zeigt, wann dieser Status-Schnappschuss erstellt wurde (meist nach dem Öffnen der Optionen)."
                },

                // Status rows
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusBus)), "Bus" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusBus)), StatusDescription("Bus") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTram)), "Tram" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTram)), StatusDescription("Tram") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTrain)), "Zug" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTrain)), StatusDescription("Zug") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSubway)), "U-Bahn" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSubway)), StatusDescription("U-Bahn") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusFerry)), "Fähre" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusFerry)), StatusDescription("Fähre") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusShip)), "Schiff" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusShip)), StatusDescription("Schiff") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusAir)), "Flugzeug" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusAir)), StatusDescription("Flugzeug") },

                // Status buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatsToLog)), "Stats ins Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatsToLog)),
                    "Schreibt einen einmaligen Detailbericht in **FastBoarding.log**.\n" +
                    "Enthält Wartesummen, Top 3 schlimmste Halte pro Modus, Beispiele übersprungener Cims, Entity-IDs und Linienhinweise."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Log öffnen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Öffnet **FastBoarding.log**, falls vorhanden.\n" +
                    "Wenn die Datei noch nicht da ist, wird stattdessen der Logs-Ordner geöffnet."
                },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Anzeigename dieses Mods." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Aktuelle Mod-Version." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Öffnet die Paradox-Mods-Seite des Autors." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableVerboseLogging)), "Ausführliches Logging" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableVerboseLogging)),
                    "**Nur Debug / Tests**\n" +
                    "Fügt <live> Details zu <Logs/FastBoarding.log> hinzu, während die Stadt läuft.\n" +
                    "**Nicht für normales Spielen aktivieren.**\n" +
                    "Aktiv lassen kann Leistung kosten und riesige Logdateien erzeugen.\n" +
                    "Alte Logdateien kannst du später löschen.\n" +
                    "Hinweis: <Stats ins Log> ist nur ein aktueller Sofort-Schnappschuss.\n" +
                    "Lass ausführliches Logging 15-30 Min. laufen, wenn du einen Zeitverlauf sehen willst.\n" +
                    "Denk nur daran, es vor normalem Spielen wieder **AUS** zu schalten."

                },


                // Runtime status strings
                { TransitWaitStatus.KeyStatusNotLoaded, "Status nicht geladen." },
                { TransitWaitStatus.KeyNoCityLoaded, "Keine Stadt geladen." },
                { TransitWaitStatus.KeyNoStopsFound, "Keine Halte gefunden." },
                { TransitWaitStatus.KeyStatusLine, "{0} warten | Schnitt {1} | schlimmster {2} | {3} übersprungen" },
                { TransitWaitStatus.KeyStatusOverviewLine, "{0} Touristen/Monat | {1} Bürger/Monat | aktualisiert {2}" },

                // Stats-to-log report strings
                { TransitWaitStatus.KeyReportNoCityLoaded, "[FB] Stats-Bericht angefordert, aber keine Stadt ist geladen." },
                { TransitWaitStatus.KeyReportTitle, "Stats-ins-Log-Schnappschuss - Fast Boarding" },
                { TransitWaitStatus.KeyReportSettings, "Einstellungen: {0}" },
                { TransitWaitStatus.KeyReportNote, "Der Linienhinweis kommt vom Wegpunkt mit der höchsten Wartezeit an diesem Halt." },
                { TransitWaitStatus.KeyReportTesterHintsHeader, "Tester-Hinweise" },
                { TransitWaitStatus.KeyReportHintWorstStops, "Schlimmste Halte: zuerst im Spiel oder mit dem Scene Explorer Mod prüfen. Achte auf Unfälle, Verkehr, schlechte Halteposition oder einen verbuggten Halt." },
                { TransitWaitStatus.KeyReportHintSkippedCims, "Übersprungene Solo-Cims: verspätete Fahrgäste, die wir überspringen, damit der Transit abfahren kann. Der spätere Zustand sollte meist 'has path' oder 'assigned' werden. Wenn er bei 'no path yet' bleibt, prüfe diese Cim-Entity nach mehr Zeit." },
                { TransitWaitStatus.KeyReportHintLateGroups, "Späte Gruppen: Familien/Gruppen, die Vanilla überlassen werden. Hohe Werte sind Hinweise für spätere sichere Unterstützung von Gruppenreisen." },
                { TransitWaitStatus.KeyReportFamilyHeader, "{0}" },
                { TransitWaitStatus.KeyReportServedStops, "Bediente Halte: {0}" },
                { TransitWaitStatus.KeyReportStopsWithWaiting, "Halte mit Wartenden: {0}" },
                { TransitWaitStatus.KeyReportWaitingPassengers, "Wartende Fahrgäste: {0}" },
                { TransitWaitStatus.KeyReportAverageWait, "Durchschnittliche Wartezeit: {0}" },
                { TransitWaitStatus.KeyReportLateBoardersSkipped, "Heute übersprungene späte Fahrgäste: {0}" },
                { TransitWaitStatus.KeyReportWorstStopNone, "Schlimmster Halt: keiner, aktuell warten keine Fahrgäste." },
                { TransitWaitStatus.KeyReportWorstStopAverageWait, "Schnitt schlimmster Halt: {0}" },
                { TransitWaitStatus.KeyReportWorstStopName, "Name schlimmster Halt: {0}" },
                { TransitWaitStatus.KeyReportWorstStopEntity, "Entity schlimmster Halt: {0}" },
                { TransitWaitStatus.KeyReportWorstWaypointEntity, "Waypoint-Entity: {0}" },
                { TransitWaitStatus.KeyReportWorstLineHint, "Linienhinweis: {0}" },
                { TransitWaitStatus.KeyReportWorstLineEntity, "Linien-Entity: {0}" },
                { TransitWaitStatus.KeyReportWorstLineWaypointAverage, "Linien-Wegpunkt-Schnitt: {0} mit {1} wartend" },
                { TransitWaitStatus.KeyReportTopWorstStopsHeader, "Top {0} schlimmste Halte nach durchschnittlicher Wartezeit:" },
                { TransitWaitStatus.KeyReportTopWorstStopLine, "{0}. {1} | Schnitt {2} | wartend {3} | Halt {4} | Wegpunkt {5} | Linie {6} | Hinweis {7}" },
                { TransitWaitStatus.KeyReportLateGroups, "Späte Gruppen nicht übersprungen: {0} Fahrgäste in {1} Gruppen auf {2} Fahrzeugen" },
                { TransitWaitStatus.KeyReportLastSkippedSamplesHeader, "Beispiele übersprungener später Solo-Cims zu diesem AKTUELLEN Zeitpunkt" },
                { TransitWaitStatus.KeyReportLastSkippedSampleLine, "{0}. {1} | Fahrgast {2} | verpasstes Fahrzeug {3} | Zeit {4} | jetzt {5}" },
                { TransitWaitStatus.KeyReportNone, "keine" },
                { TransitWaitStatus.KeyReportUnknown, "(unbekannt)" },


            };
        }

        public void Unload()
        {
        }
    }
}
