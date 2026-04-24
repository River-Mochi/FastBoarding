// File: Localization/LocaleFR.cs
// Purpose: French fr-FR locale entries for Fast Boarding.

namespace FastBoarding
{
    using Colossal;
    using System.Collections.Generic;

    /// <summary>
    /// English localization source.
    /// </summary>
    public sealed class LocaleFR : IDictionarySource
    {
        private readonly Setting m_Setting;

        /// <summary>
        /// Constructs the English locale.
        /// </summary>
        /// <param name="setting">Settings object used for locale IDs.</param>
        public LocaleFR(Setting setting)
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

            const string ToggleLabel = "Ignorer passagers en retard";

            // One helper keeps all seven status tooltips in sync for future translations.
            string StatusDescription(string transitName)
            {
                return
                    $"<État actuel {transitName}>\n" +
                    "**Attente** = total des passagers qui attendent maintenant.\n" +
                    "**Moy.** = temps d'attente moyen de ces passagers.\n" +
                    "**Pire** arrêt = attente moyenne la plus élevée sur un arrêt.\n" +
                    "Les pires arrêts sont de bons endroits à vérifier pour les accidents, arrêts bloqués/buggés ou véhicules coincés à proximité.\n" +
                    $"**Sautés** = passagers solo en retard sautés aujourd'hui par <{ToggleLabel}>.\n" +
                    "Utilise <Stats vers log> pour le rapport détaillé : noms d'arrêts, ID d'entités et plus.";
            }

            return new Dictionary<string, string>
            {
                // Options mod name
                { m_Setting.GetSettingsLocaleID(), title },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Actions" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "À propos" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.SpeedGroup), "Vitesse d'embarquement" },
                { m_Setting.GetOptionGroupLocaleID(Setting.BehaviorGroup), "Comportement" },
                { m_Setting.GetOptionGroupLocaleID(Setting.StatusGroup), "Statut" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGroup), "Infos du mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGroup), "Liens" },
                { m_Setting.GetOptionGroupLocaleID(Setting.DebugGroup), "Debug" },

                // Boarding speed sliders
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BusBoardingSpeedFactor)), "Vitesse bus" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BusBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Des valeurs plus élevées réduisent le temps de montée et de chargement aux arrêts de bus.\n" +
                    "Les files normales se résorbent plus vite, mais un passager en retard peut encore retarder le départ à cause du design vanilla.\n" +
                    $"Utilise [✓] <{ToggleLabel}> pour que les bus n'attendent pas les cims en retard.\n" +
                    "2x = environ deux fois plus rapide pour embarquer.\n" +
                    "Note technique : un loading factor plus élevé = arrêt planifié plus court ; boarding time ressemble davantage à l'estimation côté passager pour l'attente/la montée.\n" +
                    $"Ce n'est pas la même chose que <{ToggleLabel}> ; cette case décide si les cims en retard peuvent rater le véhicule après l'heure de départ.\n" +
                    "<==========================>\n" +
                    "Loading factor pour tous les transports :\n" +
                    "1x  = 100% arrêt vanilla\n" +
                    "2x  = ~ 1/2 arrêt planifié\n" +
                    "4x  = ~ 1/4 arrêt planifié\n" +
                    "10x = ~ 1/10 arrêt planifié"

                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RailBoardingSpeedFactor)), "Vitesse rail" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RailBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "S'applique au train, tram et métro.\n" +
                    "Des valeurs plus élevées réduisent le temps de montée/chargement aux arrêts ferrés.\n" +
                    "Les files normales se résorbent plus vite, mais un passager en retard peut encore retarder le départ à cause du design vanilla.\n" +
                    $"Utilise [✓] <{ToggleLabel}> si tu veux que les cims en retard ratent le véhicule après l'heure de départ.\n" +
                    "Le jeu réaffecte ensuite généralement le cim.\n" +
                    "2x = environ deux fois plus rapide pour embarquer."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WaterBoardingSpeedFactor)), "Bateau + ferry" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WaterBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "S'applique aux arrêts de bateau et ferry.\n" +
                    "Des valeurs plus élevées réduisent le temps de montée/chargement aux arrêts d'eau.\n" +
                    "Les files normales se résorbent plus vite, mais un passager en retard peut encore retarder le départ à cause du design vanilla.\n" +
                    $"Utilise [✓] <{ToggleLabel}> si tu veux que les cims en retard ratent le véhicule après l'heure de départ.\n" +
                    "2x = environ deux fois plus rapide pour embarquer."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AirBoardingSpeedFactor)), "Vitesse avion" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AirBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "S'applique aux terminaux d'avion de passagers.\n" +
                    "Des valeurs plus élevées réduisent le temps de montée/chargement aux terminaux aériens.\n" +
                    "Les files normales se résorbent plus vite, mais un passager en retard peut encore retarder le départ à cause du design vanilla.\n" +
                    $"Utilise [✓] <{ToggleLabel}> si tu veux que les cims en retard ratent le véhicule après l'heure de départ.\n" +
                    "2x = environ deux fois plus rapide pour embarquer."
                },

                // Late passenger behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CancelLateBoarders)), ToggleLabel },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CancelLateBoarders)),
                    "Les passagers encore <pas prêts> après l'heure de départ peuvent rater le véhicule.\n" +
                    "Note : pour l'instant on ne saute que les citoyens en retard qui voyagent seuls.\n" +
                    "Les groupes/familles qui voyagent ensemble et arrivent en retard <ne sont pas sautés> et peuvent encore causer des retards comme en vanilla.\n" +
                    "Les groupes restent une petite partie de la foule ; le gros du gain vient des cims solo en retard.\n" +
                    "Les citoyens sautés ne sont pas supprimés ; le jeu les réaffecte ensuite naturellement."
                },

                // Status overview
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusOverview)), "Utilisation totale" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusOverview)),
                    "Utilisation mensuelle des transports publics depuis l'infoview Transport du jeu.\n" +
                    "Mis à jour montre quand ce snapshot a été pris (en général après l'ouverture des options)."
                },

                // Status rows
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusBus)), "Bus" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusBus)), StatusDescription("bus") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTram)), "Tram" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTram)), StatusDescription("tram") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTrain)), "Train" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTrain)), StatusDescription("train") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSubway)), "Métro" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSubway)), StatusDescription("métro") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusFerry)), "Ferry" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusFerry)), StatusDescription("ferry") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusShip)), "Bateau" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusShip)), StatusDescription("bateau") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusAir)), "Avion" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusAir)), StatusDescription("avion") },

                // Status buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatsToLog)), "Stats vers log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatsToLog)),
                    "Écrit un rapport détaillé ponctuel dans **FastBoarding.log**.\n" +
                    "Inclut totaux d'attente, top 3 des pires arrêts par mode, exemples de cims sautés, ID d'entités et indices de ligne."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Ouvrir log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Ouvre **FastBoarding.log** s'il existe.\n" +
                    "Sinon ouvre le dossier Logs."
                },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Nom affiché de ce mod." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Version actuelle du mod." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Ouvre la page Paradox Mods de l'auteur." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableVerboseLogging)), "Activer le log détaillé" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableVerboseLogging)),
                    "**Debug / test seulement**\n" +
                    "Ajoute des détails <live> dans <Logs/FastBoarding.log> pendant que la ville tourne.\n" +
                    "**Ne l'active pas pour une partie normale.**\n" +
                    "Le laisser activé peut baisser les performances et créer d'énormes logs.\n" +
                    "Tu peux supprimer les anciens logs plus tard.\n" +
                    "Note : <Stats vers log> n'est qu'un snapshot instantané.\n" +
                    "Laisse le log détaillé tourner 15-30 min si tu veux une timeline de ce qui s'est passé.\n" +
                    "Pense juste à le remettre sur **OFF** avant de jouer normalement."

                },


                // Runtime status strings
                { TransitWaitStatus.KeyStatusNotLoaded, "Statut non chargé." },
                { TransitWaitStatus.KeyNoCityLoaded, "Aucune ville chargée." },
                { TransitWaitStatus.KeyNoStopsFound, "Aucun arrêt trouvé." },
                { TransitWaitStatus.KeyStatusLine, "{0} attendent | moy. {1} | pire {2} | {3} sautés" },
                { TransitWaitStatus.KeyStatusOverviewLine, "{0} touristes/mois | {1} citoyens/mois | mis à jour {2}" },

                // Stats-to-log report strings
                { TransitWaitStatus.KeyReportNoCityLoaded, "[FB] Rapport demandé, mais aucune ville n'est chargée." },
                { TransitWaitStatus.KeyReportTitle, "Snapshot Stats vers log - Fast Boarding" },
                { TransitWaitStatus.KeyReportSettings, "Réglages : {0}" },
                { TransitWaitStatus.KeyReportNote, "L'indice de ligne vient du waypoint avec la plus forte attente sur cet arrêt." },
                { TransitWaitStatus.KeyReportTesterHintsHeader, "Astuces testeurs" },
                { TransitWaitStatus.KeyReportHintWorstStops, "Pires arrêts : vérifie-les d'abord en jeu ou avec le mod Scene Explorer. Cherche accidents, trafic, mauvais emplacement ou arrêt buggué." },
                { TransitWaitStatus.KeyReportHintSkippedCims, "Cims solo sautés : passagers en retard que l'on saute pour laisser partir le transport. L'état plus tard devrait souvent devenir 'has path' ou 'assigned'. S'il reste sur 'no path yet', recontrôle cette entité plus tard." },
                { TransitWaitStatus.KeyReportHintLateGroups, "Groupes en retard : familles/groupes laissés à vanilla. De grands nombres donnent des pistes pour un futur support sûr des voyages en groupe." },
                { TransitWaitStatus.KeyReportFamilyHeader, "{0}" },
                { TransitWaitStatus.KeyReportServedStops, "Arrêts desservis : {0}" },
                { TransitWaitStatus.KeyReportStopsWithWaiting, "Arrêts avec attente : {0}" },
                { TransitWaitStatus.KeyReportWaitingPassengers, "Passagers en attente : {0}" },
                { TransitWaitStatus.KeyReportAverageWait, "Attente moyenne : {0}" },
                { TransitWaitStatus.KeyReportLateBoardersSkipped, "Passagers en retard sautés aujourd'hui : {0}" },
                { TransitWaitStatus.KeyReportWorstStopNone, "Pire arrêt : aucun, personne n'attend pour le moment." },
                { TransitWaitStatus.KeyReportWorstStopAverageWait, "Attente moy. pire arrêt : {0}" },
                { TransitWaitStatus.KeyReportWorstStopName, "Nom du pire arrêt : {0}" },
                { TransitWaitStatus.KeyReportWorstStopEntity, "Entité du pire arrêt : {0}" },
                { TransitWaitStatus.KeyReportWorstWaypointEntity, "Entité waypoint : {0}" },
                { TransitWaitStatus.KeyReportWorstLineHint, "Indice de ligne : {0}" },
                { TransitWaitStatus.KeyReportWorstLineEntity, "Entité de ligne : {0}" },
                { TransitWaitStatus.KeyReportWorstLineWaypointAverage, "Moyenne waypoint de ligne : {0} avec {1} en attente" },
                { TransitWaitStatus.KeyReportTopWorstStopsHeader, "Top {0} des pires arrêts par attente moyenne :" },
                { TransitWaitStatus.KeyReportTopWorstStopLine, "{0}. {1} | moy. {2} | attente {3} | arrêt {4} | waypoint {5} | ligne {6} | indice {7}" },
                { TransitWaitStatus.KeyReportLateGroups, "Groupes en retard non sautés : {0} passagers dans {1} groupes sur {2} véhicules" },
                { TransitWaitStatus.KeyReportLastSkippedSamplesHeader, "Exemples de cims solo en retard sautés à cet instant ACTUEL" },
                { TransitWaitStatus.KeyReportLastSkippedSampleLine, "{0}. {1} | passager {2} | véhicule raté {3} | heure {4} | maintenant {5}" },
                { TransitWaitStatus.KeyReportNone, "aucun" },
                { TransitWaitStatus.KeyReportUnknown, "(inconnu)" },


            };
        }

        public void Unload()
        {
        }
    }
}
