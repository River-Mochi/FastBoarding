// File: Localization/LocalePT_BR.cs
// Purpose: Portuguese Brazilian pt-BR locale entries for Fast Boarding.

namespace FastBoarding
{
    using Colossal;
    using System.Collections.Generic;

    /// <summary>
    /// Portuguese Brazilian localization source.
    /// </summary>
    public sealed class LocalePT_BR : IDictionarySource
    {
        private readonly Setting m_Setting;

        /// <summary>
        /// Constructs the Portuguese Brazilian locale.
        /// </summary>
        /// <param name="setting">Settings object used for locale IDs.</param>
        public LocalePT_BR(Setting setting)
        {
            m_Setting = setting;
        }

        /// <summary>
        /// Creates all Portuguese Brazilian localization entries for this mod.
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
                    $"<Status atual de {transitName}>\n" +
                    "**Esperando** = total de passageiros esperando agora.\n" +
                    "**Média** = tempo médio de espera desses passageiros.\n" +
                    "**Pior** parada = maior espera média em uma parada.\n" +
                    "As piores paradas são bons pontos para verificar acidentes, paradas bloqueadas/bugadas ou veículos presos por perto.\n" +
                    "**Pulados** = embarques atrasados cancelados hoje pela opção.\n" +
                    "Use <Stats para Log> para relatório detalhado: nomes de paradas, IDs de entidade e mais.";
            }

            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), title },
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Ações" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "Sobre" },
                { m_Setting.GetOptionGroupLocaleID(Setting.SpeedGroup), "Velocidade de embarque" },
                { m_Setting.GetOptionGroupLocaleID(Setting.BehaviorGroup), "Comportamento" },
                { m_Setting.GetOptionGroupLocaleID(Setting.StatusGroup), "Status" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGroup), "Info do mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGroup), "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.DebugGroup), "Debug" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BusBoardingSpeedFactor)), "Velocidade de ônibus" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BusBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "Valores maiores reduzem o tempo de embarque/carregamento nos pontos de ônibus.\n" +
                    "Isso ajuda filas normais a andar mais rápido, mas um passageiro atrasado ainda pode atrasar a saída por causa do design vanilla.\n" +
                    "Use [✓] <Deixar veículos saírem sem cims atrasados> se quiser que cims solo atrasados percam o veículo.\n" +
                    "2x significa aproximadamente o dobro da velocidade de embarque."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RailBoardingSpeedFactor)), "Velocidade ferroviária" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RailBoardingSpeedFactor)), "<1x = vanilla>\nAplica-se a trem, bonde e metrô.\nValores maiores reduzem o tempo de embarque/carregamento.\nUse [✓] <Deixar veículos saírem sem cims atrasados> se cims solo atrasados ainda estiverem travando." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WaterBoardingSpeedFactor)), "Velocidade navio + balsa" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WaterBoardingSpeedFactor)), "<1x = vanilla>\nAplica-se a navios e balsas.\nValores maiores reduzem o tempo de embarque/carregamento.\nUse [✓] <Deixar veículos saírem sem cims atrasados> se cims solo atrasados ainda estiverem travando." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AirBoardingSpeedFactor)), "Velocidade de avião" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AirBoardingSpeedFactor)), "<1x = vanilla>\nAplica-se a terminais de aviões de passageiros.\nValores maiores reduzem o tempo de embarque/carregamento.\nUse [✓] <Deixar veículos saírem sem cims atrasados> se cims solo atrasados ainda estiverem travando." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CancelLateBoarders)), "Deixar sair sem cims atrasados" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CancelLateBoarders)), "**BETA experimental**\nCidadãos solo atrasados que ainda estão <não prontos> depois do horário vanilla de saída podem perder o veículo.\nGrupos/famílias <não são pulados> ainda; eles ainda podem causar alguns atrasos como no vanilla.\nCidadãos pulados não são deletados; os sistemas vanilla continuam a partir dali." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusOverview)), "Uso total" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusOverview)), "Uso mensal do transporte público pela infovisão de Transporte do jogo.\nA hora mostra quando este snapshot de status foi feito." },
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

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatsToLog)), "Stats para Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatsToLog)), "Escreve um relatório detalhado único em **FastBoarding.log**.\nInclui totais de espera, 3 piores paradas por modo, exemplos de cims pulados, IDs de entidade e dicas de linha." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Abrir Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)), "Abre **FastBoarding.log** se existir.\nSe o arquivo ainda não existir, abre a pasta Logs." },

                { TransitWaitStatus.KeyStatusNotLoaded, "Status não carregado." },
                { TransitWaitStatus.KeyNoCityLoaded, "Nenhuma cidade carregada." },
                { TransitWaitStatus.KeyNoStopsFound, "Nenhuma parada encontrada." },
                { TransitWaitStatus.KeyStatusLine, "{0} esperando | média {1} | pior {2} | {3} pulados" },
                { TransitWaitStatus.KeyStatusOverviewLine, "{0} turistas/mês | {1} cidadãos/mês | atualizado {2}" },
                { TransitWaitStatus.KeyReportNoCityLoaded, "[FB] Relatório solicitado, mas nenhuma cidade está carregada." },
                { TransitWaitStatus.KeyReportTitle, "Snapshot Stats para Log - Fast Boarding" },
                { TransitWaitStatus.KeyReportSettings, "Configurações: {0}" },
                { TransitWaitStatus.KeyReportNote, "A dica de linha vem do waypoint com maior espera nessa parada." },
                { TransitWaitStatus.KeyReportTesterHintsHeader, "Dicas para testes" },
                { TransitWaitStatus.KeyReportHintWorstStops, "Piores paradas: inspecione primeiro no jogo ou com o mod Scene Explorer. Procure acidentes, tráfego bloqueado, localização ruim da parada ou uma parada bugada." },
                { TransitWaitStatus.KeyReportHintSkippedCims, "Cims solo pulados: passageiros atrasados que pulamos para permitir que o transporte saia. O estado posterior geralmente deve virar 'has path' ou 'assigned'. Se ficar em 'no path yet', inspecione essa entidade cim depois de mais tempo." },
                { TransitWaitStatus.KeyReportHintLateGroups, "Grupos atrasados: famílias/grupos deixados para o vanilla. Contagens altas dão pistas para suporte seguro a viagens em grupo no futuro." },
                { TransitWaitStatus.KeyReportFamilyHeader, "{0}" },
                { TransitWaitStatus.KeyReportServedStops, "Paradas atendidas: {0}" },
                { TransitWaitStatus.KeyReportStopsWithWaiting, "Paradas com passageiros esperando: {0}" },
                { TransitWaitStatus.KeyReportWaitingPassengers, "Passageiros esperando: {0}" },
                { TransitWaitStatus.KeyReportAverageWait, "Espera média: {0}" },
                { TransitWaitStatus.KeyReportLateBoardersSkipped, "Passageiros atrasados pulados hoje: {0}" },
                { TransitWaitStatus.KeyReportWorstStopNone, "Pior parada: nenhuma, sem passageiros esperando agora." },
                { TransitWaitStatus.KeyReportWorstStopAverageWait, "Espera média da pior parada: {0}" },
                { TransitWaitStatus.KeyReportWorstStopName, "Nome da pior parada: {0}" },
                { TransitWaitStatus.KeyReportWorstStopEntity, "Entidade da pior parada: {0}" },
                { TransitWaitStatus.KeyReportWorstWaypointEntity, "Entidade do pior waypoint: {0}" },
                { TransitWaitStatus.KeyReportWorstLineHint, "Dica da pior linha: {0}" },
                { TransitWaitStatus.KeyReportWorstLineEntity, "Entidade da pior linha: {0}" },
                { TransitWaitStatus.KeyReportWorstLineWaypointAverage, "Média do pior waypoint da linha: {0} com {1} esperando" },
                { TransitWaitStatus.KeyReportTopWorstStopsHeader, "Top {0} piores paradas por espera média:" },
                { TransitWaitStatus.KeyReportTopWorstStopLine, "{0}. {1} | média {2} | esperando {3} | parada {4} | waypoint {5} | linha {6} | dica {7}" },
                { TransitWaitStatus.KeyReportLateGroups, "Passageiros em grupos atrasados deixados em paz: {0} passageiros em {1} grupos em {2} veículos" },
                { TransitWaitStatus.KeyReportLastSkippedSamplesHeader, "Exemplos de cims solo pulados" },
                { TransitWaitStatus.KeyReportLastSkippedSampleLine, "{0}. {1} | passageiro {2} | veículo {3} | frame {4} | hora {5} | agora {6}" },
                { TransitWaitStatus.KeyReportNone, "nenhum" },
                { TransitWaitStatus.KeyReportUnknown, "(desconhecido)" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Nome exibido deste mod." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Versão" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Versão atual do mod." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Abre a página Paradox Mods da autora." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableVerboseLogging)), "Ativar log detalhado" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableVerboseLogging)), "**Somente debug / testes**\nAdiciona diagnósticos ao vivo em <FastBoarding.log> enquanto a cidade roda.\n**Não ative para jogo normal.**\nDeixar isso ligado pode reduzir desempenho e criar logs enormes." }
            };
        }

        public void Unload()
        {
        }
    }
}
