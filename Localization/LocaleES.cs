// File: Localization/LocaleES.cs
// Purpose: Spanish es-ES locale entries for Fast Boarding.

namespace FastBoarding
{
    using Colossal;
    using System.Collections.Generic;

    /// <summary>
    /// Spanish localization source.
    /// </summary>
    public sealed class LocaleES : IDictionarySource
    {
        private readonly Setting m_Setting;

        /// <summary>
        /// Constructs the Spanish locale.
        /// </summary>
        /// <param name="setting">Settings object used for locale IDs.</param>
        public LocaleES(Setting setting)
        {
            m_Setting = setting;
        }

        /// <summary>
        /// Creates all Spanish localization entries for this mod.
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
                    $"<Estado actual de {transitName}>\n" +
                    "**Esperando** = pasajeros esperando ahora mismo.\n" +
                    "**Media** = tiempo medio de espera de esos pasajeros.\n" +
                    "**Peor** parada = mayor espera media en una parada.\n" +
                    "Las peores paradas son buenos lugares para revisar accidentes, paradas bloqueadas/bugueadas o vehículos retenidos cerca.\n" +
                    "**Saltados** = embarques tardíos cancelados hoy por la opción.\n" +
                    "Usa <Stats al log> para un informe detallado: nombres de paradas, ID de entidades y más.";
            }

            return new Dictionary<string, string>
            {
                // Options mod name
                { m_Setting.GetSettingsLocaleID(), title },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Acciones" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "Acerca de" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.SpeedGroup), "Velocidad de embarque" },
                { m_Setting.GetOptionGroupLocaleID(Setting.BehaviorGroup), "Comportamiento" },
                { m_Setting.GetOptionGroupLocaleID(Setting.StatusGroup), "Estado" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGroup), "Info del mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGroup), "Enlaces" },
                { m_Setting.GetOptionGroupLocaleID(Setting.DebugGroup), "Depuración" },

                // Boarding speed sliders
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BusBoardingSpeedFactor)), "Velocidad de bus" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BusBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Valores más altos reducen el tiempo de embarque/carga en paradas de bus.\n" +
                    "Esto ayuda a vaciar colas normales más rápido, pero un pasajero tarde aún puede retrasar la salida por el diseño vanilla.\n" +
                    "Usa [✓] <Dejar salir vehículos sin cims tarde> si quieres que cims solo tarde pierdan el vehículo.\n" +
                    "2x significa aprox. doble velocidad.\n" +
                    "Nota técnica: un factor de carga más alto significa una duración de parada planificada más corta, y el tiempo de embarque se parece más a la estimación de espera/embarque del lado del pasajero.\n" +
                    "No es lo mismo que forzar al vehículo a salir."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RailBoardingSpeedFactor)), "Velocidad rail" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RailBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Aplica a paradas de tren, tranvía y metro.\n" +
                    "Valores más altos reducen el tiempo de embarque/carga en paradas ferroviarias.\n" +
                    "Esto ayuda a vaciar colas normales más rápido, pero un pasajero tarde aún puede retrasar la salida por el diseño vanilla.\n" +
                    "Usa [✓] <Dejar salir vehículos sin cims tarde> si quieres que cims solo tarde pierdan el vehículo.\n" +
                    "2x significa aprox. doble velocidad."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WaterBoardingSpeedFactor)), "Velocidad barco + ferry" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WaterBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Aplica a paradas de barco y ferry.\n" +
                    "Valores más altos reducen el tiempo de embarque/carga en paradas de barco y ferry.\n" +
                    "Esto ayuda a vaciar colas normales más rápido, pero un pasajero tarde aún puede retrasar la salida por el diseño vanilla.\n" +
                    "Usa [✓] <Dejar salir vehículos sin cims tarde> si quieres que cims solo tarde pierdan el vehículo.\n" +
                    "2x significa aprox. doble velocidad."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AirBoardingSpeedFactor)), "Velocidad avión" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AirBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Aplica a terminales de aviones de pasajeros.\n" +
                    "Valores más altos reducen el tiempo de embarque/carga en terminales de avión.\n" +
                    "Esto ayuda a vaciar colas normales más rápido, pero un pasajero tarde aún puede retrasar la salida por el diseño vanilla.\n" +
                    "Usa [✓] <Dejar salir vehículos sin cims tarde> si quieres que cims solo tarde pierdan el vehículo.\n" +
                    "2x significa aprox. doble velocidad."
                },

                // Late passenger behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CancelLateBoarders)), "Dejar salir sin cims tarde" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CancelLateBoarders)),
                    "**BETA**\n" +
                    "Los pasajeros tarde que siguen <no listos> después de la hora de salida vanilla pueden perder el vehículo.\n" +
                    "Nota: por ahora solo saltamos ciudadanos tarde que viajan solos, así que los grupos/familias que viajan juntos <no se saltan> y aún pueden causar retrasos como en vanilla.\n" +
                    "Los viajeros en grupo son pocos comparados con muchos pasajeros solos.\n" +
                    "Los ciudadanos tarde saltados no se eliminan; los sistemas vanilla continúan desde ahí para asignarlos."
                },

                // Status overview
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusOverview)), "Uso total" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusOverview)),
                    "Uso mensual del transporte público desde la infovista de Transporte del juego.\n" +
                    "La hora muestra cuándo se tomó esta instantánea."
                },

                // Status rows
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusBus)), "Bus" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusBus)), StatusDescription("bus") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTram)), "Tranvía" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTram)), StatusDescription("tranvía") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTrain)), "Tren" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTrain)), StatusDescription("tren") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSubway)), "Metro" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSubway)), StatusDescription("metro") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusFerry)), "Ferry" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusFerry)), StatusDescription("ferry") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusShip)), "Barco" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusShip)), StatusDescription("barco") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusAir)), "Avión" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusAir)), StatusDescription("avión") },

                // Status buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatsToLog)), "Stats al log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatsToLog)),
                    "Escribe un informe detallado puntual en **FastBoarding.log**.\n" +
                    "Incluye totales de espera, 3 peores paradas por modo, ejemplos de cims saltados e ID de entidades."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Abrir log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Abre **FastBoarding.log** si existe.\n" +
                    "Si no, abre la carpeta Logs."
                },

                // Runtime status strings
                { TransitWaitStatus.KeyStatusNotLoaded, "Estado no cargado." },
                { TransitWaitStatus.KeyNoCityLoaded, "No hay ciudad cargada." },
                { TransitWaitStatus.KeyNoStopsFound, "No se encontraron paradas." },
                { TransitWaitStatus.KeyStatusLine, "{0} esperan | media {1} | peor {2} | {3} saltados" },
                { TransitWaitStatus.KeyStatusOverviewLine, "{0} turistas/mes | {1} ciudadanos/mes | act. {2}" },

                // Stats-to-log report strings
                { TransitWaitStatus.KeyReportNoCityLoaded, "[FB] Se pidió informe, pero no hay ciudad cargada." },
                { TransitWaitStatus.KeyReportTitle, "Instantánea Stats al log - Fast Boarding" },
                { TransitWaitStatus.KeyReportSettings, "Ajustes: {0}" },
                { TransitWaitStatus.KeyReportNote, "La pista de línea viene del waypoint con mayor espera en esa parada." },
                { TransitWaitStatus.KeyReportTesterHintsHeader, "Pistas para testers" },
                { TransitWaitStatus.KeyReportHintWorstStops, "Peores paradas: revísalas primero en el juego o con el mod Scene Explorer. Busca accidentes, tráfico bloqueado, mala ubicación de parada o una parada bugueada." },
                { TransitWaitStatus.KeyReportHintSkippedCims, "Cims solo saltados: pasajeros tarde que saltamos para permitir que el transporte salga. El estado posterior debería ser normalmente 'has path' o 'assigned'. Si sigue en 'no path yet', inspecciona esa entidad cim después de más tiempo." },
                { TransitWaitStatus.KeyReportHintLateGroups, "Grupos tarde: son familias/grupos que dejamos a vanilla. Recuentos altos dan pistas para soporte seguro de viajes en grupo en el futuro." },
                { TransitWaitStatus.KeyReportFamilyHeader, "{0}" },
                { TransitWaitStatus.KeyReportServedStops, "Paradas servidas: {0}" },
                { TransitWaitStatus.KeyReportStopsWithWaiting, "Paradas con espera: {0}" },
                { TransitWaitStatus.KeyReportWaitingPassengers, "Pasajeros esperando: {0}" },
                { TransitWaitStatus.KeyReportAverageWait, "Espera media: {0}" },
                { TransitWaitStatus.KeyReportLateBoardersSkipped, "Pasajeros tarde saltados hoy: {0}" },
                { TransitWaitStatus.KeyReportWorstStopNone, "Peor parada: ninguna, no hay pasajeros esperando ahora." },
                { TransitWaitStatus.KeyReportWorstStopAverageWait, "Espera media peor parada: {0}" },
                { TransitWaitStatus.KeyReportWorstStopName, "Nombre peor parada: {0}" },
                { TransitWaitStatus.KeyReportWorstStopEntity, "Entidad peor parada: {0}" },
                { TransitWaitStatus.KeyReportWorstWaypointEntity, "Entidad waypoint: {0}" },
                { TransitWaitStatus.KeyReportWorstLineHint, "Pista de línea: {0}" },
                { TransitWaitStatus.KeyReportWorstLineEntity, "Entidad línea: {0}" },
                { TransitWaitStatus.KeyReportWorstLineWaypointAverage, "Media waypoint línea: {0} con {1} esperando" },
                { TransitWaitStatus.KeyReportTopWorstStopsHeader, "Top {0} peores paradas por espera media:" },
                { TransitWaitStatus.KeyReportTopWorstStopLine, "{0}. {1} | media {2} | esperando {3} | parada {4} | waypoint {5} | línea {6} | pista {7}" },
                { TransitWaitStatus.KeyReportLateGroups, "Grupos tarde no saltados: {0} pasajeros en {1} grupos en {2} vehículos" },
                { TransitWaitStatus.KeyReportLastSkippedSamplesHeader, "Ejemplos de cims solo tardíos saltados en este momento ACTUAL" },
                { TransitWaitStatus.KeyReportLastSkippedSampleLine, "{0}. {1} | pasajero {2} | vehículo perdido {3} | hora {4} | ahora {5}" },
                { TransitWaitStatus.KeyReportNone, "ninguno" },
                { TransitWaitStatus.KeyReportUnknown, "(desconocido)" },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Nombre mostrado del mod." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Versión" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Versión actual del mod." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Abre la página de Paradox Mods del autor." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableVerboseLogging)), "Activar log detallado" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableVerboseLogging)),
                    "**Solo depuración / pruebas**\n" +
                    "Añade diagnósticos en vivo a <FastBoarding.log> mientras la ciudad corre.\n" +
                    "**No lo actives para juego normal.**\n" +
                    "Dejar esto activado puede reducir el rendimiento y crear logs enormes.\n" +
                    "Puedes borrar archivos log antiguos más tarde."
                },
            };
        }

        public void Unload()
        {
        }
    }
}
