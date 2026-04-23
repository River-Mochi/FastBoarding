// File: Localization/LocaleFR.cs
// Purpose: French fr-FR locale entries for Fast Boarding.

namespace FastBoarding
{
    using Colossal;
    using System.Collections.Generic;

    /// <summary>
    /// French localization source.
    /// </summary>
    public sealed class LocaleFR : IDictionarySource
    {
        private readonly Setting m_Setting;

        /// <summary>
        /// Constructs the French locale.
        /// </summary>
        /// <param name="setting">Settings object used for locale IDs.</param>
        public LocaleFR(Setting setting)
        {
            m_Setting = setting;
        }

        /// <summary>
        /// Creates all French localization entries for this mod.
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
                    $"<Statut actuel : {transitName}>\n" +
                    "**En attente** = nombre total de passagers qui attendent maintenant.\n" +
                    "**Moy.** = temps d'attente moyen de ces passagers.\n" +
                    "**Pire** arrêt = attente moyenne la plus haute à un arrêt.\n" +
                    "Les pires arrêts sont de bons endroits à inspecter pour accidents, arrêts bloqués/bugués ou véhicules coincés à proximité.\n" +
                    "**Ignorés** = embarquements tardifs annulés aujourd'hui par l'option.\n" +
                    "Utilisez <Stats vers le log> pour un rapport détaillé : noms d'arrêts, ID d'entités, etc.";
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
                { m_Setting.GetOptionGroupLocaleID(Setting.DebugGroup), "Débogage" },

                // Boarding speed sliders
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BusBoardingSpeedFactor)), "Vitesse bus" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BusBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Les valeurs plus hautes réduisent le temps d'embarquement/chargement aux arrêts de bus.\n" +
                    "Cela vide les files normales plus vite, mais un passager en retard peut encore retarder le départ à cause du design vanilla.\n" +
                    "Utilisez [✓] <Laisser partir les véhicules sans cims en retard> si vous voulez que les cims solo en retard ratent le véhicule.\n" +
                    "2x signifie environ deux fois plus rapide.\n" +
                    "Note technique : un facteur de chargement plus élevé signifie une durée d'arrêt planifiée plus courte, et le temps d'embarquement ressemble davantage à une estimation côté passager attente/embarquement.\n" +
                    "Ce n'est pas la même chose que forcer le véhicule à partir."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RailBoardingSpeedFactor)), "Vitesse rail" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RailBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "S'applique aux arrêts de train, tram et métro.\n" +
                    "Les valeurs plus hautes réduisent le temps d'embarquement/chargement aux arrêts ferroviaires.\n" +
                    "Cela vide les files normales plus vite, mais un passager en retard peut encore retarder le départ à cause du design vanilla.\n" +
                    "Utilisez [✓] <Laisser partir les véhicules sans cims en retard> si vous voulez que les cims solo en retard ratent le véhicule.\n" +
                    "2x signifie environ deux fois plus rapide."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WaterBoardingSpeedFactor)), "Vitesse bateau + ferry" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WaterBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "S'applique aux arrêts de bateaux et ferries.\n" +
                    "Les valeurs plus hautes réduisent le temps d'embarquement/chargement aux arrêts de bateaux et ferries.\n" +
                    "Cela vide les files normales plus vite, mais un passager en retard peut encore retarder le départ à cause du design vanilla.\n" +
                    "Utilisez [✓] <Laisser partir les véhicules sans cims en retard> si vous voulez que les cims solo en retard ratent le véhicule.\n" +
                    "2x signifie environ deux fois plus rapide."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AirBoardingSpeedFactor)), "Vitesse avion" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AirBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "S'applique aux terminaux d'avions passagers.\n" +
                    "Les valeurs plus hautes réduisent le temps d'embarquement/chargement aux terminaux d'avions.\n" +
                    "Cela vide les files normales plus vite, mais un passager en retard peut encore retarder le départ à cause du design vanilla.\n" +
                    "Utilisez [✓] <Laisser partir les véhicules sans cims en retard> si vous voulez que les cims solo en retard ratent le véhicule.\n" +
                    "2x signifie environ deux fois plus rapide."
                },

                // Late passenger behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CancelLateBoarders)), "Laisser partir sans cims en retard" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CancelLateBoarders)),
                    "**BÊTA expérimentale**\n" +
                    "Les citoyens solo en retard qui sont encore <pas prêts> après l'heure de départ vanilla peuvent rater le véhicule.\n" +
                    "Note : les groupes/familles voyageant ensemble ne sont <pas ignorés> pour l'instant ; ils peuvent encore causer des retards comme en vanilla.\n" +
                    "Même le mode solo uniquement aide, car les voyageurs solo sont généralement la majorité aux arrêts de transit.\n" +
                    "Les citoyens en retard ignorés ne sont pas supprimés ; les systèmes vanilla continuent ensuite à les assigner."
                },

                // Status overview
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusOverview)), "Utilisation totale" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusOverview)),
                    "Utilisation mensuelle du transport public depuis l'infovue Transport du jeu.\n" +
                    "L'heure indique quand cet instantané a été pris."
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
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatsToLog)), "Stats vers le log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatsToLog)),
                    "Écrit un rapport détaillé ponctuel dans **FastBoarding.log**.\n" +
                    "Inclut totaux d'attente, 3 pires arrêts par mode, exemples de cims ignorés et ID d'entités."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Ouvrir le log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Ouvre **FastBoarding.log** s'il existe.\n" +
                    "Sinon ouvre le dossier Logs."
                },

                // Runtime status strings
                { TransitWaitStatus.KeyStatusNotLoaded, "Statut non chargé." },
                { TransitWaitStatus.KeyNoCityLoaded, "Aucune ville chargée." },
                { TransitWaitStatus.KeyNoStopsFound, "Aucun arrêt trouvé." },
                { TransitWaitStatus.KeyStatusLine, "{0} attente | moy {1} | pire {2} | {3} ignorés" },
                { TransitWaitStatus.KeyStatusOverviewLine, "{0} touristes/mois | {1} citoyens/mois | maj {2}" },

                // Stats-to-log report strings
                { TransitWaitStatus.KeyReportNoCityLoaded, "[FB] Rapport demandé, mais aucune ville n'est chargée." },
                { TransitWaitStatus.KeyReportTitle, "Instantané Stats vers le log - Fast Boarding" },
                { TransitWaitStatus.KeyReportSettings, "Réglages : {0}" },
                { TransitWaitStatus.KeyReportNote, "L'indice de ligne vient du waypoint avec la plus forte attente à cet arrêt." },
                { TransitWaitStatus.KeyReportTesterHintsHeader, "Conseils de test" },
                { TransitWaitStatus.KeyReportHintWorstStops, "Pires arrêts : inspectez-les d'abord en jeu ou avec le mod Scene Explorer. Cherchez accidents, trafic bloqué, mauvais emplacement d'arrêt ou arrêt bugué." },
                { TransitWaitStatus.KeyReportHintSkippedCims, "Cims solo ignorés : passagers en retard que le mod ignore pour permettre au transit de partir. Leur état suivant devrait souvent devenir 'has path' ou 'assigned'. Si cela reste 'no path yet', inspectez l'entité cim après plus de temps." },
                { TransitWaitStatus.KeyReportHintLateGroups, "Groupes en retard : familles/groupes laissés à vanilla. Un nombre élevé donne des indices pour un futur support sûr des déplacements en groupe." },
                { TransitWaitStatus.KeyReportFamilyHeader, "{0}" },
                { TransitWaitStatus.KeyReportServedStops, "Arrêts desservis : {0}" },
                { TransitWaitStatus.KeyReportStopsWithWaiting, "Arrêts avec attente : {0}" },
                { TransitWaitStatus.KeyReportWaitingPassengers, "Passagers en attente : {0}" },
                { TransitWaitStatus.KeyReportAverageWait, "Attente moyenne : {0}" },
                { TransitWaitStatus.KeyReportLateBoardersSkipped, "Passagers en retard ignorés aujourd'hui : {0}" },
                { TransitWaitStatus.KeyReportWorstStopNone, "Pire arrêt : aucun passager en attente maintenant." },
                { TransitWaitStatus.KeyReportWorstStopAverageWait, "Attente moy. pire arrêt : {0}" },
                { TransitWaitStatus.KeyReportWorstStopName, "Nom du pire arrêt : {0}" },
                { TransitWaitStatus.KeyReportWorstStopEntity, "Entité du pire arrêt : {0}" },
                { TransitWaitStatus.KeyReportWorstWaypointEntity, "Entité waypoint : {0}" },
                { TransitWaitStatus.KeyReportWorstLineHint, "Indice de ligne : {0}" },
                { TransitWaitStatus.KeyReportWorstLineEntity, "Entité ligne : {0}" },
                { TransitWaitStatus.KeyReportWorstLineWaypointAverage, "Moy. waypoint ligne : {0} avec {1} en attente" },
                { TransitWaitStatus.KeyReportTopWorstStopsHeader, "Top {0} des pires arrêts par attente moyenne :" },
                { TransitWaitStatus.KeyReportTopWorstStopLine, "{0}. {1} | moy {2} | attente {3} | arrêt {4} | waypoint {5} | ligne {6} | indice {7}" },
                { TransitWaitStatus.KeyReportLateGroups, "Groupes en retard non ignorés : {0} passagers dans {1} groupes sur {2} véhicules" },
                { TransitWaitStatus.KeyReportLastSkippedSamplesHeader, "Exemples de cims solo ignorés" },
                { TransitWaitStatus.KeyReportLastSkippedSampleLine, "{0}. {1} | passager {2} | véhicule {3} | frame {4} | heure {5} | maintenant {6}" },
                { TransitWaitStatus.KeyReportNone, "aucun" },
                { TransitWaitStatus.KeyReportUnknown, "(inconnu)" },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Nom affiché du mod." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Version actuelle du mod." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Ouvre la page Paradox Mods de l'auteur." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableVerboseLogging)), "Activer le log détaillé" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableVerboseLogging)),
                    "**Débogage / tests uniquement**\n" +
                    "Ajoute des diagnostics en direct à <FastBoarding.log> pendant que la ville tourne.\n" +
                    "**Ne pas activer en jeu normal.**\n" +
                    "Laisser ceci activé peut réduire les performances et créer de gros fichiers log.\n" +
                    "Vous pouvez supprimer les anciens fichiers log plus tard."
                },
            };
        }

        public void Unload()
        {
        }
    }
}
