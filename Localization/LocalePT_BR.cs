// File: Localization/LocalePT_BR.cs
// Purpose: Brazilian Portuguese pt-BR locale entries for Fast Boarding.

namespace FastBoarding
{
    using Colossal;
    using System.Collections.Generic;

    /// <summary>
    /// English localization source.
    /// </summary>
    public sealed class LocalePT_BR : IDictionarySource
    {
        private readonly Setting m_Setting;

        /// <summary>
        /// Constructs the English locale.
        /// </summary>
        /// <param name="setting">Settings object used for locale IDs.</param>
        public LocalePT_BR(Setting setting)
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

            const string ToggleName = "Pular passageiros atrasados";

            // One helper keeps all seven status tooltips in sync for future translations.
            string StatusDescription(string transitName)
            {
                return
                    $"<Status atual de {transitName}>\n" +
                    "**Esperando** = total de passageiros esperando agora.\n" +
                    "**Média** = tempo médio de espera desses passageiros.\n" +
                    "**Pior** parada = maior média de espera em uma parada.\n" +
                    "As piores paradas são bons lugares para checar acidentes, parada travada/com bug ou veículos presos por perto.\n" +
                    $"**Pulados** = passageiros solo atrasados pulados hoje por <{ToggleName}>.\n" +
                    "Use <Stats para log> para relatório detalhado: nomes de paradas, IDs de entidade e mais.";
            }

            return new Dictionary<string, string>
            {
                // Options mod name
                { m_Setting.GetSettingsLocaleID(), title },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Ações" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "Sobre" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.SpeedGroup), "Velocidade de embarque" },
                { m_Setting.GetOptionGroupLocaleID(Setting.BehaviorGroup), "Comportamento" },
                { m_Setting.GetOptionGroupLocaleID(Setting.StatusGroup), "Status" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGroup), "Info do mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGroup), "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.DebugGroup), "Debug" },

                // Boarding speed sliders
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BusBoardingSpeedFactor)), "Velocidade do ônibus" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BusBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Valores maiores reduzem o tempo de embarque e carga nas paradas de ônibus.\n" +
                    "Filas normais andam mais rápido, mas um passageiro atrasado ainda pode segurar a saída por causa do design vanilla.\n" +
                    $"Use [✓] <{ToggleName}> para o ônibus não ficar esperando cim atrasado.\n" +
                    "2x significa mais ou menos o dobro da velocidade de embarque.\n" +
                    "Nota técnica: valor de carga maior = parada planejada mais curta; boarding time parece mais a estimativa de espera/embarque do lado do passageiro.\n" +
                    $"Isso não é a mesma coisa que <{ToggleName}>; essa caixa decide se cims atrasados podem perder o veículo depois do horário de saída.\n" +
                    "<==========================>\n" +
                    "Valor de carga para todo o transporte:\n" +
                    "1x  = 100% da parada vanilla\n" +
                    "2x  = ~ 1/2 da parada planejada\n" +
                    "4x  = ~ 1/4 da parada planejada\n" +
                    "10x = ~ 1/10 da parada planejada"

                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RailBoardingSpeedFactor)), "Velocidade no trilho" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RailBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Vale para trem, bonde e metrô.\n" +
                    "Valores maiores reduzem o tempo de embarque/carga nas paradas sobre trilhos.\n" +
                    "Filas normais andam mais rápido, mas um passageiro atrasado ainda pode segurar a saída por causa do design vanilla.\n" +
                    $"Use [✓] <{ToggleName}> se quiser que cims atrasados percam o veículo depois do horário de saída.\n" +
                    "Depois disso, o jogo normalmente reassocia o cim.\n" +
                    "2x significa mais ou menos o dobro da velocidade de embarque."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WaterBoardingSpeedFactor)), "Navio + balsa" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WaterBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Vale para navio e balsa.\n" +
                    "Valores maiores reduzem o tempo de embarque/carga nas paradas aquáticas.\n" +
                    "Filas normais andam mais rápido, mas um passageiro atrasado ainda pode segurar a saída por causa do design vanilla.\n" +
                    $"Use [✓] <{ToggleName}> se quiser que cims atrasados percam o veículo depois do horário de saída.\n" +
                    "2x significa mais ou menos o dobro da velocidade de embarque."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AirBoardingSpeedFactor)), "Velocidade do avião" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AirBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Vale para terminais de avião de passageiros.\n" +
                    "Valores maiores reduzem o tempo de embarque/carga nos terminais aéreos.\n" +
                    "Filas normais andam mais rápido, mas um passageiro atrasado ainda pode segurar a saída por causa do design vanilla.\n" +
                    $"Use [✓] <{ToggleName}> se quiser que cims atrasados percam o veículo depois do horário de saída.\n" +
                    "2x significa mais ou menos o dobro da velocidade de embarque."
                },

                // Late passenger behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CancelLateBoarders)), ToggleName },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CancelLateBoarders)),
                    "Passageiros que ainda estiverem <não prontos> depois do horário de saída podem perder o veículo.\n" +
                    "Nota: por enquanto, só pulamos cidadãos atrasados que estão viajando sozinhos.\n" +
                    "Grupos/famílias viajando juntos e atrasados <não são pulados> e ainda podem causar atraso como no vanilla.\n" +
                    "Grupos são parte pequena da multidão; o maior ganho vem de pular cims solo atrasados.\n" +
                    "Esses cidadãos não são apagados; o jogo os reatribui naturalmente depois."
                },

                // Status overview
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusOverview)), "Uso total" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusOverview)),
                    "Uso mensal do transporte público da tela de transporte do jogo.\n" +
                    "Atualizado mostra quando esse retrato de status foi tirado (normalmente ao abrir as opções)."
                },

                // Status rows
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusBus)), "Ônibus" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusBus)), StatusDescription("ônibus") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTram)), "Bonde" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTram)), StatusDescription("bonde") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTrain)), "Trem" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTrain)), StatusDescription("trem") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSubway)), "Metrô" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSubway)), StatusDescription("metrô") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusFerry)), "Balsa" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusFerry)), StatusDescription("balsa") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusShip)), "Navio" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusShip)), StatusDescription("navio") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusAir)), "Avião" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusAir)), StatusDescription("avião") },

                // Status buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatsToLog)), "Stats para log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatsToLog)),
                    "Escreve um relatório detalhado único em **FastBoarding.log**.\n" +
                    "Inclui totais de espera, top 3 piores paradas por modo, exemplos de cims pulados, IDs de entidade e dicas de linha."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Abrir log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Abre **FastBoarding.log** se ele existir.\n" +
                    "Se o arquivo ainda não existir, abre a pasta Logs."
                },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Nome exibido deste mod." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Versão" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Versão atual do mod." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Abre a página do autor no Paradox Mods." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableVerboseLogging)), "Ativar log detalhado" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableVerboseLogging)),
                    "**Só para debug / teste**\n" +
                    "Adiciona detalhes <live> em <Logs/FastBoarding.log> enquanto a cidade roda.\n" +
                    "**Não deixe isso ligado no jogo normal.**\n" +
                    "Deixar ligado pode baixar desempenho e criar logs enormes.\n" +
                    "Você pode apagar logs antigos depois.\n" +
                    "Nota: <Stats para log> é só um retrato do momento.\n" +
                    "Deixe o log detalhado rodando por 15-30 min se quiser uma linha do tempo do que aconteceu.\n" +
                    "Só não esqueça de voltar para **DESLIGADO** antes de jogar normal."

                },


                // Runtime status strings
                { TransitWaitStatus.KeyStatusNotLoaded, "Status não carregado." },
                { TransitWaitStatus.KeyNoCityLoaded, "Nenhuma cidade carregada." },
                { TransitWaitStatus.KeyNoStopsFound, "Nenhuma parada encontrada." },
                { TransitWaitStatus.KeyStatusLine, "{0} esperando | média {1} | pior {2} | {3} pulados" },
                { TransitWaitStatus.KeyStatusOverviewLine, "{0} turistas/mês | {1} cidadãos/mês | atualizado {2}" },

                // Stats-to-log report strings
                { TransitWaitStatus.KeyReportNoCityLoaded, "[FB] Relatório pedido, mas nenhuma cidade está carregada." },
                { TransitWaitStatus.KeyReportTitle, "Snapshot Stats para log - Fast Boarding" },
                { TransitWaitStatus.KeyReportSettings, "Configurações: {0}" },
                { TransitWaitStatus.KeyReportNote, "A dica da linha vem do waypoint com maior espera naquela parada." },
                { TransitWaitStatus.KeyReportTesterHintsHeader, "Dicas para tester" },
                { TransitWaitStatus.KeyReportHintWorstStops, "Piores paradas: olhe essas primeiro no jogo ou com o mod Scene Explorer. Veja acidentes, trânsito, posição ruim da parada ou uma parada bugada." },
                { TransitWaitStatus.KeyReportHintSkippedCims, "Cims solo pulados: passageiros atrasados que pulamos para deixar o transporte sair. Depois, o estado normalmente vira 'has path' ou 'assigned'. Se continuar em 'no path yet', confira essa entidade mais tarde." },
                { TransitWaitStatus.KeyReportHintLateGroups, "Grupos atrasados: famílias/grupos deixados para o vanilla. Números altos dão pistas para um suporte futuro e seguro a viagens em grupo." },
                { TransitWaitStatus.KeyReportFamilyHeader, "{0}" },
                { TransitWaitStatus.KeyReportServedStops, "Paradas atendidas: {0}" },
                { TransitWaitStatus.KeyReportStopsWithWaiting, "Paradas com gente esperando: {0}" },
                { TransitWaitStatus.KeyReportWaitingPassengers, "Passageiros esperando: {0}" },
                { TransitWaitStatus.KeyReportAverageWait, "Espera média: {0}" },
                { TransitWaitStatus.KeyReportLateBoardersSkipped, "Passageiros atrasados pulados hoje: {0}" },
                { TransitWaitStatus.KeyReportWorstStopNone, "Pior parada: nenhuma, ninguém esperando agora." },
                { TransitWaitStatus.KeyReportWorstStopAverageWait, "Média da pior parada: {0}" },
                { TransitWaitStatus.KeyReportWorstStopName, "Nome da pior parada: {0}" },
                { TransitWaitStatus.KeyReportWorstStopEntity, "Entidade da pior parada: {0}" },
                { TransitWaitStatus.KeyReportWorstWaypointEntity, "Entidade do waypoint: {0}" },
                { TransitWaitStatus.KeyReportWorstLineHint, "Dica de linha: {0}" },
                { TransitWaitStatus.KeyReportWorstLineEntity, "Entidade da linha: {0}" },
                { TransitWaitStatus.KeyReportWorstLineWaypointAverage, "Média do waypoint da linha: {0} com {1} esperando" },
                { TransitWaitStatus.KeyReportTopWorstStopsHeader, "Top {0} piores paradas por espera média:" },
                { TransitWaitStatus.KeyReportTopWorstStopLine, "{0}. {1} | média {2} | esperando {3} | parada {4} | waypoint {5} | linha {6} | dica {7}" },
                { TransitWaitStatus.KeyReportLateGroups, "Grupos atrasados não pulados: {0} passageiros em {1} grupos em {2} veículos" },
                { TransitWaitStatus.KeyReportLastSkippedSamplesHeader, "Exemplos de cims solo atrasados pulados neste momento ATUAL" },
                { TransitWaitStatus.KeyReportLastSkippedSampleLine, "{0}. {1} | passageiro {2} | veículo perdido {3} | hora {4} | agora {5}" },
                { TransitWaitStatus.KeyReportNone, "nenhum" },
                { TransitWaitStatus.KeyReportUnknown, "(desconhecido)" },


            };
        }

        public void Unload()
        {
        }
    }
}
