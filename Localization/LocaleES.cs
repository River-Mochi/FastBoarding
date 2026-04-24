// File: Localization/LocaleES.cs
// Purpose: Spanish es-ES locale entries for Fast Boarding.

namespace FastBoarding
{
    using Colossal;
    using System.Collections.Generic;

    /// <summary>
    /// English localization source.
    /// </summary>
    public sealed class LocaleES : IDictionarySource
    {
        private readonly Setting m_Setting;

        /// <summary>
        /// Constructs the English locale.
        /// </summary>
        /// <param name="setting">Settings object used for locale IDs.</param>
        public LocaleES(Setting setting)
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

            const string ToggleName = "Saltar pasajeros tardíos";

            // One helper keeps all seven status tooltips in sync for future translations.
            string StatusDescription(string transitName)
            {
                return
                    $"<Estado actual de {transitName}>\n" +
                    "**Esperando** = pasajeros esperando ahora mismo.\n" +
                    "**Media** = tiempo medio de espera de esos pasajeros.\n" +
                    "**Peor** parada = mayor espera media en una sola parada.\n" +
                    "Las peores paradas son buenos lugares para revisar accidentes, paradas bloqueadas/con bug o vehículos atascados cerca.\n" +
                    $"**Saltados** = pasajeros tarde que hoy se saltaron con <{ToggleName}>.\n" +
                    "Usa <Stats al log> para más detalle: nombres de paradas, ID de entidades y más.";
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
                    "Valores más altos reducen el tiempo de embarque y carga en las paradas de bus.\n" +
                    "Esto ayuda a limpiar colas normales más rápido, pero un pasajero tarde aún puede retrasar la salida por diseño vanilla.\n" +
                    $"Usa [✓] <{ToggleName}> para que los buses no se queden esperando cims tarde.\n" +
                    "2x significa aprox. el doble de velocidad de embarque.\n" +
                    "Nota técnica: un valor de carga más alto = menos tiempo de parada planeado; boarding time se parece más a la estimación de espera/subida del pasajero.\n" +
                    $"No es lo mismo que <{ToggleName}>; esa casilla decide si los cims tarde pueden perder el vehículo después de la hora de salida.\n" +
                    "<==========================>\n" +
                    "Valor de carga para todo el transporte:\n" +
                    "1x  = 100% parada vanilla\n" +
                    "2x  = ~ 1/2 parada planeada\n" +
                    "4x  = ~ 1/4 parada planeada\n" +
                    "10x = ~ 1/10 parada planeada"

                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RailBoardingSpeedFactor)), "Velocidad rail" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RailBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Aplica a tren, tranvía y metro.\n" +
                    "Valores más altos reducen el tiempo de embarque/carga en paradas de rail.\n" +
                    "Esto ayuda a limpiar colas normales más rápido, pero un pasajero tarde aún puede retrasar la salida por diseño vanilla.\n" +
                    $"Usa [✓] <{ToggleName}> si quieres que los cims tarde pierdan el vehículo después de la hora de salida.\n" +
                    "Luego el juego suele reasignar al cim.\n" +
                    "2x significa aprox. el doble de velocidad de embarque."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WaterBoardingSpeedFactor)), "Barco + ferry" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WaterBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Aplica a paradas de barco y ferry.\n" +
                    "Valores más altos reducen el tiempo de embarque/carga en barco y ferry.\n" +
                    "Esto ayuda a limpiar colas normales más rápido, pero un pasajero tarde aún puede retrasar la salida por diseño vanilla.\n" +
                    $"Usa [✓] <{ToggleName}> si quieres que los cims tarde pierdan el vehículo después de la hora de salida.\n" +
                    "2x significa aprox. el doble de velocidad de embarque."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AirBoardingSpeedFactor)), "Velocidad avión" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AirBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Aplica a terminales de avión de pasajeros.\n" +
                    "Valores más altos reducen el tiempo de embarque/carga en terminales aéreas.\n" +
                    "Esto ayuda a limpiar colas normales más rápido, pero un pasajero tarde aún puede retrasar la salida por diseño vanilla.\n" +
                    $"Usa [✓] <{ToggleName}> si quieres que los cims tarde pierdan el vehículo después de la hora de salida.\n" +
                    "2x significa aprox. el doble de velocidad de embarque."
                },

                // Late passenger behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CancelLateBoarders)), ToggleName },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CancelLateBoarders)),
                    "Los pasajeros que sigan <no listos> después de la hora de salida pueden perder el vehículo.\n" +
                    "Nota: por ahora solo saltamos ciudadanos tarde que viajan solos.\n" +
                    "Los grupos/familias que viajan juntos y llegan tarde <no se saltan> y aún pueden causar retrasos como en vanilla.\n" +
                    "Los grupos son una parte pequeña de la multitud; la mayor mejora viene de saltar cims solos que llegan tarde.\n" +
                    "No se borran; el juego los reasigna de forma natural."
                },

                // Status overview
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusOverview)), "Uso total" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusOverview)),
                    "Uso mensual de transporte público desde la vista de transporte del juego.\n" +
                    "Actualizado muestra cuándo se tomó esta foto del estado (normalmente al abrir Opciones)."
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
                    "Escribe un informe detallado de una sola vez en **FastBoarding.log**.\n" +
                    "Incluye totales de espera, top 3 peores paradas por modo, ejemplos de cims saltados, ID de entidades y pistas de línea."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Abrir log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Abre **FastBoarding.log** si existe.\n" +
                    "Si el archivo aún no existe, abre la carpeta Logs."
                },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Nombre mostrado de este mod." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Versión" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Versión actual del mod." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Abre la página de Paradox Mods del autor." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableVerboseLogging)), "Activar log detallado" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableVerboseLogging)),
                    "**Solo debug / pruebas**\n" +
                    "Añade detalles <live> a <Logs/FastBoarding.log> mientras la ciudad corre.\n" +
                    "**No lo actives para jugar normal.**\n" +
                    "Dejarlo encendido puede bajar rendimiento y crear logs enormes.\n" +
                    "Luego puedes borrar logs viejos.\n" +
                    "Nota: <Stats al log> es solo una foto rápida del momento.\n" +
                    "Déjalo 15-30 min si quieres una línea de tiempo de lo que pasó.\n" +
                    "Solo no olvides volver a dejarlo en **APAGADO** antes de jugar normal."

                },

                // Runtime status strings
                { TransitWaitStatus.KeyStatusNotLoaded, "Estado no cargado." },
                { TransitWaitStatus.KeyNoCityLoaded, "No hay ciudad cargada." },
                { TransitWaitStatus.KeyNoStopsFound, "No se encontraron paradas." },
                { TransitWaitStatus.KeyStatusLine, "{0} esperando | media {1} | peor {2} | {3} saltados" },
                { TransitWaitStatus.KeyStatusOverviewLine, "{0} turistas/mes | {1} ciudadanos/mes | actualizado {2}" },

                // Stats-to-log report strings
                { TransitWaitStatus.KeyReportNoCityLoaded, "[FB] Se pidió el informe, pero no hay ciudad cargada." },
                { TransitWaitStatus.KeyReportTitle, "Foto de Stats al log - Fast Boarding" },
                { TransitWaitStatus.KeyReportSettings, "Ajustes: {0}" },
                { TransitWaitStatus.KeyReportNote, "La pista de línea sale del waypoint con mayor espera en esa parada." },
                { TransitWaitStatus.KeyReportTesterHintsHeader, "Pistas para testers" },
                { TransitWaitStatus.KeyReportHintWorstStops, "Peores paradas: revísalas primero en el juego o con el mod Scene Explorer. Mira accidentes, tráfico, mala ubicación o una parada bugueada." },
                { TransitWaitStatus.KeyReportHintSkippedCims, "Cims solos saltados: pasajeros tarde que saltamos para que el transporte pueda salir. Después su estado suele pasar a 'has path' o 'assigned'. Si sigue en 'no path yet', revisa esa entidad más tarde." },
                { TransitWaitStatus.KeyReportHintLateGroups, "Grupos tarde: familias/grupos que se dejan a vanilla. Números altos dan pistas para un soporte futuro y seguro para viajes en grupo." },
                { TransitWaitStatus.KeyReportFamilyHeader, "{0}" },
                { TransitWaitStatus.KeyReportServedStops, "Paradas servidas: {0}" },
                { TransitWaitStatus.KeyReportStopsWithWaiting, "Paradas con gente esperando: {0}" },
                { TransitWaitStatus.KeyReportWaitingPassengers, "Pasajeros esperando: {0}" },
                { TransitWaitStatus.KeyReportAverageWait, "Espera media: {0}" },
                { TransitWaitStatus.KeyReportLateBoardersSkipped, "Pasajeros tarde saltados hoy: {0}" },
                { TransitWaitStatus.KeyReportWorstStopNone, "Peor parada: ninguna, no hay pasajeros esperando ahora mismo." },
                { TransitWaitStatus.KeyReportWorstStopAverageWait, "Espera media peor parada: {0}" },
                { TransitWaitStatus.KeyReportWorstStopName, "Nombre peor parada: {0}" },
                { TransitWaitStatus.KeyReportWorstStopEntity, "Entidad peor parada: {0}" },
                { TransitWaitStatus.KeyReportWorstWaypointEntity, "Entidad waypoint: {0}" },
                { TransitWaitStatus.KeyReportWorstLineHint, "Pista de línea: {0}" },
                { TransitWaitStatus.KeyReportWorstLineEntity, "Entidad de línea: {0}" },
                { TransitWaitStatus.KeyReportWorstLineWaypointAverage, "Media del waypoint de línea: {0} con {1} esperando" },
                { TransitWaitStatus.KeyReportTopWorstStopsHeader, "Top {0} peores paradas por espera media:" },
                { TransitWaitStatus.KeyReportTopWorstStopLine, "{0}. {1} | media {2} | esperando {3} | parada {4} | waypoint {5} | línea {6} | pista {7}" },
                { TransitWaitStatus.KeyReportLateGroups, "Grupos tarde no saltados: {0} pasajeros en {1} grupos sobre {2} vehículos" },
                { TransitWaitStatus.KeyReportLastSkippedSamplesHeader, "Ejemplos de cims solos tarde saltados en este momento ACTUAL" },
                { TransitWaitStatus.KeyReportLastSkippedSampleLine, "{0}. {1} | pasajero {2} | vehículo perdido {3} | hora {4} | ahora {5}" },
                { TransitWaitStatus.KeyReportNone, "ninguno" },
                { TransitWaitStatus.KeyReportUnknown, "(desconocido)" },

            };
        }

        public void Unload()
        {
        }
    }
}
