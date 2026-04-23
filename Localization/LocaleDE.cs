// File: Localization/LocaleDE.cs
// Purpose: German de-DE locale entries for Fast Boarding.

namespace FastBoarding
{
    using Colossal;
    using System.Collections.Generic;

    /// <summary>
    /// German localization source.
    /// </summary>
    public sealed class LocaleDE : IDictionarySource
    {
        private readonly Setting m_Setting;

        /// <summary>
        /// Constructs the German locale.
        /// </summary>
        /// <param name="setting">Settings object used for locale IDs.</param>
        public LocaleDE(Setting setting)
        {
            m_Setting = setting;
        }

        /// <summary>
        /// Creates all German localization entries for this mod.
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
                    $"<Aktueller {transitName}-Status>\n" +
                    "**Wartend** = alle Passagiere, die gerade warten.\n" +
                    "**Durchschn.** = durchschnittliche Wartezeit dieser Passagiere.\n" +
                    "**Schlimmster** Halt = höchste durchschnittliche Wartezeit an einem Halt.\n" +
                    "Schlimmste Halte sind gute Stellen zum Prüfen auf Unfälle, blockierte/verbuggte Halte oder Fahrzeuge, die in der Nähe festhängen.\n" +
                    "**Übersprungen** = heute durch die Option abgebrochene späte Einstiege.\n" +
                    "Nutze <Stats ins Log> für einen Detailbericht: Haltestellennamen, Entity-IDs und mehr.";
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
                    "Höhere Werte verkürzen Einstiegs-/Ladezeit an Bushaltestellen.\n" +
                    "Normale Warteschlangen werden schneller abgearbeitet, aber ein verspäteter Fahrgast kann die Abfahrt durch das Vanilla-Design weiterhin verzögern.\n" +
                    "Nutze [✓] <Fahrzeuge ohne späte Cims abfahren lassen>, wenn einzelne verspätete Cims das Fahrzeug verpassen sollen.\n" +
                    "2x bedeutet etwa doppelte Einstiegsgeschwindigkeit.\n" +
                    "Technische Notiz: Ein höherer Ladefaktor bedeutet kürzere geplante Haltezeit, und Boarding Time ist eher die passagierseitige Warte-/Einstiegsschätzung.\n" +
                    "Das ist nicht dasselbe wie das Fahrzeug zum Abfahren zu zwingen."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RailBoardingSpeedFactor)), "Bahn-Einstieg" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RailBoardingSpeedFactor)),
                    "<1x = Vanilla>\n" +
                    "Gilt für Zug-, Tram- und U-Bahn-Halte.\n" +
                    "Höhere Werte verkürzen Einstiegs-/Ladezeit an Schienenhalten.\n" +
                    "Normale Warteschlangen werden schneller abgearbeitet, aber ein verspäteter Fahrgast kann die Abfahrt durch das Vanilla-Design weiterhin verzögern.\n" +
                    "Nutze [✓] <Fahrzeuge ohne späte Cims abfahren lassen>, wenn einzelne verspätete Cims das Fahrzeug verpassen sollen.\n" +
                    "2x bedeutet etwa doppelte Einstiegsgeschwindigkeit."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WaterBoardingSpeedFactor)), "Schiff + Fähre" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WaterBoardingSpeedFactor)),
                    "<1x = Vanilla>\n" +
                    "Gilt für Schiffs- und Fährhaltestellen.\n" +
                    "Höhere Werte verkürzen Einstiegs-/Ladezeit an Schiffs- und Fährhalten.\n" +
                    "Normale Warteschlangen werden schneller abgearbeitet, aber ein verspäteter Fahrgast kann die Abfahrt durch das Vanilla-Design weiterhin verzögern.\n" +
                    "Nutze [✓] <Fahrzeuge ohne späte Cims abfahren lassen>, wenn einzelne verspätete Cims das Fahrzeug verpassen sollen.\n" +
                    "2x bedeutet etwa doppelte Einstiegsgeschwindigkeit."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AirBoardingSpeedFactor)), "Flugzeug-Einstieg" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AirBoardingSpeedFactor)),
                    "<1x = Vanilla>\n" +
                    "Gilt für Passagierflugzeug-Terminals.\n" +
                    "Höhere Werte verkürzen Einstiegs-/Ladezeit an Flugzeug-Terminals.\n" +
                    "Normale Warteschlangen werden schneller abgearbeitet, aber ein verspäteter Fahrgast kann die Abfahrt durch das Vanilla-Design weiterhin verzögern.\n" +
                    "Nutze [✓] <Fahrzeuge ohne späte Cims abfahren lassen>, wenn einzelne verspätete Cims das Fahrzeug verpassen sollen.\n" +
                    "2x bedeutet etwa doppelte Einstiegsgeschwindigkeit."
                },

                // Late passenger behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CancelLateBoarders)), "Ohne späte Cims abfahren" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CancelLateBoarders)),
                    "**BETA**\n" +
                    "Verspätete Fahrgäste, die nach der Vanilla-Abfahrtszeit noch <nicht bereit> sind, dürfen das Fahrzeug verpassen.\n" +
                    "Hinweis: Wir überspringen vorerst nur einzelne verspätete Bürger; Gruppen/Familien, die zusammen reisen, werden <nicht übersprungen> und können weiterhin Verzögerungen wie in Vanilla verursachen.\n" +
                    "Gruppenreisende sind im Vergleich zu vielen Solo-Fahrgästen nur eine kleine Anzahl.\n" +
                    "Übersprungene späte Bürger werden nicht gelöscht; Vanilla-Systeme weisen sie danach weiter zu."
                },

                // Status overview
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusOverview)), "Gesamtnutzung" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusOverview)),
                    "Monatliche ÖPNV-Nutzung aus der Transport-Infoansicht des Spiels.\n" +
                    "Die Uhrzeit zeigt, wann dieser Schnappschuss erstellt wurde."
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
                    "Enthält Wartesummen, Top 3 schlimmste Halte pro Modus, Beispiele übersprungener Cims und Entity-IDs."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Log öffnen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Öffnet **FastBoarding.log**, falls vorhanden.\n" +
                    "Sonst wird der Logs-Ordner geöffnet."
                },

                // Runtime status strings
                { TransitWaitStatus.KeyStatusNotLoaded, "Status nicht geladen." },
                { TransitWaitStatus.KeyNoCityLoaded, "Keine Stadt geladen." },
                { TransitWaitStatus.KeyNoStopsFound, "Keine Halte gefunden." },
                { TransitWaitStatus.KeyStatusLine, "{0} warten | durchschn. {1} | schlimmster {2} | {3} übersprungen" },
                { TransitWaitStatus.KeyStatusOverviewLine, "{0} Touristen/Monat | {1} Bürger/Monat | aktualisiert {2}" },

                // Stats-to-log report strings
                { TransitWaitStatus.KeyReportNoCityLoaded, "[FB] Bericht angefordert, aber keine Stadt ist geladen." },
                { TransitWaitStatus.KeyReportTitle, "Stats-ins-Log-Schnappschuss - Fast Boarding" },
                { TransitWaitStatus.KeyReportSettings, "Einstellungen: {0}" },
                { TransitWaitStatus.KeyReportNote, "Der Linienhinweis kommt vom Wegpunkt mit der höchsten Wartezeit an diesem Halt." },
                { TransitWaitStatus.KeyReportTesterHintsHeader, "Tester-Hinweise" },
                { TransitWaitStatus.KeyReportHintWorstStops, "Schlimmste Halte: zuerst im Spiel oder mit dem Scene Explorer Mod prüfen. Achte auf Unfälle, blockierten Verkehr, schlechte Halteposition oder einen verbuggten Halt." },
                { TransitWaitStatus.KeyReportHintSkippedCims, "Übersprungene Solo-Cims: verspätete Passagiere, die übersprungen werden, damit der Verkehr abfahren kann. Der spätere Zustand sollte meist 'has path' oder 'assigned' werden. Wenn er bei 'no path yet' bleibt, prüfe diese Cim-Entity nach mehr Zeit." },
                { TransitWaitStatus.KeyReportHintLateGroups, "Späte Gruppen: Familien/Gruppen, die Vanilla überlassen werden. Hohe Werte sind Hinweise für zukünftige sichere Unterstützung von Gruppenreisen." },
                { TransitWaitStatus.KeyReportFamilyHeader, "{0}" },
                { TransitWaitStatus.KeyReportServedStops, "Bediente Halte: {0}" },
                { TransitWaitStatus.KeyReportStopsWithWaiting, "Halte mit Wartenden: {0}" },
                { TransitWaitStatus.KeyReportWaitingPassengers, "Wartende Passagiere: {0}" },
                { TransitWaitStatus.KeyReportAverageWait, "Durchschnittliche Wartezeit: {0}" },
                { TransitWaitStatus.KeyReportLateBoardersSkipped, "Heute übersprungene späte Passagiere: {0}" },
                { TransitWaitStatus.KeyReportWorstStopNone, "Schlimmster Halt: keiner, aktuell warten keine Passagiere." },
                { TransitWaitStatus.KeyReportWorstStopAverageWait, "Durchschn. Wartezeit schlimmster Halt: {0}" },
                { TransitWaitStatus.KeyReportWorstStopName, "Name schlimmster Halt: {0}" },
                { TransitWaitStatus.KeyReportWorstStopEntity, "Entity schlimmster Halt: {0}" },
                { TransitWaitStatus.KeyReportWorstWaypointEntity, "Waypoint-Entity: {0}" },
                { TransitWaitStatus.KeyReportWorstLineHint, "Linienhinweis: {0}" },
                { TransitWaitStatus.KeyReportWorstLineEntity, "Linien-Entity: {0}" },
                { TransitWaitStatus.KeyReportWorstLineWaypointAverage, "Linien-Waypoint-Durchschnitt: {0} mit {1} wartend" },
                { TransitWaitStatus.KeyReportTopWorstStopsHeader, "Top {0} schlimmste Halte nach durchschnittlicher Wartezeit:" },
                { TransitWaitStatus.KeyReportTopWorstStopLine, "{0}. {1} | durchschn. {2} | wartend {3} | Halt {4} | Waypoint {5} | Linie {6} | Hinweis {7}" },
                { TransitWaitStatus.KeyReportLateGroups, "Späte Gruppen nicht übersprungen: {0} Passagiere in {1} Gruppen auf {2} Fahrzeugen" },
                { TransitWaitStatus.KeyReportLastSkippedSamplesHeader, "Beispiele übersprungener Solo-Cims" },
                { TransitWaitStatus.KeyReportLastSkippedSampleLine, "{0}. {1} | Passagier {2} | Fahrzeug {3} | Frame {4} | Zeit {5} | jetzt {6}" },
                { TransitWaitStatus.KeyReportNone, "keine" },
                { TransitWaitStatus.KeyReportUnknown, "(unbekannt)" },

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
                    "Fügt Live-Diagnosen zu <FastBoarding.log> hinzu, während die Stadt läuft.\n" +
                    "**Nicht für normales Spielen aktivieren.**\n" +
                    "Aktiviert lassen kann Leistung verringern und sehr große Logdateien erzeugen.\n" +
                    "Alte Logdateien können später gelöscht werden."
                },
            };
        }

        public void Unload()
        {
        }
    }
}
