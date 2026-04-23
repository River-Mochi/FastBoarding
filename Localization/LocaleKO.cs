// File: Localization/LocaleKO.cs
// Purpose: Korean ko-KR locale entries for Fast Boarding.

namespace FastBoarding
{
    using Colossal;
    using System.Collections.Generic;

    /// <summary>
    /// Korean localization source.
    /// </summary>
    public sealed class LocaleKO : IDictionarySource
    {
        private readonly Setting m_Setting;

        /// <summary>
        /// Constructs the Korean locale.
        /// </summary>
        /// <param name="setting">Settings object used for locale IDs.</param>
        public LocaleKO(Setting setting)
        {
            m_Setting = setting;
        }

        /// <summary>
        /// Creates all Korean localization entries for this mod.
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
                    $"<현재 {transitName} 상태>\n" +
                    "**대기 중** = 지금 기다리는 승객 총수.\n" +
                    "**평균** = 해당 승객들의 평균 대기 시간.\n" +
                    "**최악** 정류장 = 한 정류장에서 가장 높은 평균 대기 시간.\n" +
                    "최악 정류장은 사고, 막힌/버그난 정류장, 근처에서 붙잡힌 차량을 확인하기 좋은 곳입니다.\n" +
                    "**스킵** = 옵션으로 오늘 취소한 늦은 탑승.\n" +
                    "<Stats to Log>를 사용하면 정류장 이름, 엔티티 ID 등 자세한 보고서를 기록합니다.";
            }

            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), title },
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "동작" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "정보" },
                { m_Setting.GetOptionGroupLocaleID(Setting.SpeedGroup), "탑승 속도" },
                { m_Setting.GetOptionGroupLocaleID(Setting.BehaviorGroup), "동작 방식" },
                { m_Setting.GetOptionGroupLocaleID(Setting.StatusGroup), "상태" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGroup), "모드 정보" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGroup), "링크" },
                { m_Setting.GetOptionGroupLocaleID(Setting.DebugGroup), "디버그" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BusBoardingSpeedFactor)), "버스 탑승 속도" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BusBoardingSpeedFactor)), "<1x = 바닐라>\n값이 높을수록 버스 정류장의 탑승/적재 시간이 줄어듭니다.\n일반 대기열은 더 빨리 처리되지만, 바닐라 설계상 늦은 승객이 여전히 출발을 지연시킬 수 있습니다.\n혼자 이동하는 늦은 시민이 차량을 놓치게 하려면 [✓] <늦은 시민 없이 차량 출발 허용>을 사용하세요.\n2x는 대략 두 배 탑승 속도입니다." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RailBoardingSpeedFactor)), "철도 탑승 속도" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RailBoardingSpeedFactor)), "<1x = 바닐라>\n기차, 트램, 지하철 정류장에 적용됩니다.\n값이 높을수록 탑승/적재 시간이 줄어듭니다.\n혼자 이동하는 늦은 시민이 계속 막고 있다면 [✓] <늦은 시민 없이 차량 출발 허용>을 사용하세요." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WaterBoardingSpeedFactor)), "선박 + 페리 속도" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WaterBoardingSpeedFactor)), "<1x = 바닐라>\n선박과 페리 정류장에 적용됩니다.\n값이 높을수록 탑승/적재 시간이 줄어듭니다.\n혼자 이동하는 늦은 시민이 계속 막고 있다면 [✓] <늦은 시민 없이 차량 출발 허용>을 사용하세요." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AirBoardingSpeedFactor)), "비행기 탑승 속도" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AirBoardingSpeedFactor)), "<1x = 바닐라>\n여객기 터미널에 적용됩니다.\n값이 높을수록 탑승/적재 시간이 줄어듭니다.\n혼자 이동하는 늦은 시민이 계속 막고 있다면 [✓] <늦은 시민 없이 차량 출발 허용>을 사용하세요." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CancelLateBoarders)), "늦은 시민 없이 차량 출발 허용" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CancelLateBoarders)), "**실험적 베타**\n바닐라 출발 시간 이후에도 <준비 안 됨> 상태인 혼자 이동하는 시민은 그 차량을 놓칠 수 있습니다.\n참고: 함께 이동하는 그룹/가족은 아직 <스킵하지 않습니다>. 바닐라처럼 일부 지연을 일으킬 수 있습니다.\n스킵된 시민은 삭제되지 않으며, 이후 바닐라 시스템이 계속 처리합니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusOverview)), "전체 이용량" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusOverview)), "게임 교통 정보 보기의 월간 대중교통 이용량입니다.\n업데이트 시간은 이 상태 스냅샷을 만든 시간입니다." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusBus)), "버스" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusBus)), StatusDescription("버스") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTram)), "트램" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTram)), StatusDescription("트램") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTrain)), "기차" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTrain)), StatusDescription("기차") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSubway)), "지하철" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSubway)), StatusDescription("지하철") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusFerry)), "페리" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusFerry)), StatusDescription("페리") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusShip)), "선박" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusShip)), StatusDescription("선박") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusAir)), "비행기" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusAir)), StatusDescription("비행기") },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatsToLog)), "Stats to Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatsToLog)), "**FastBoarding.log**에 1회 상세 보고서를 씁니다.\n대기 총계, 모드별 최악 정류장 상위 3개, 스킵된 시민 예시, 엔티티 ID, 노선 힌트를 포함합니다." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "로그 열기" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)), "**FastBoarding.log**가 있으면 엽니다.\n파일이 아직 없으면 Logs 폴더를 엽니다." },

                { TransitWaitStatus.KeyStatusNotLoaded, "상태를 불러오지 못했습니다." },
                { TransitWaitStatus.KeyNoCityLoaded, "도시가 로드되지 않았습니다." },
                { TransitWaitStatus.KeyNoStopsFound, "정류장을 찾지 못했습니다." },
                { TransitWaitStatus.KeyStatusLine, "{0} 대기 | 평균 {1} | 최악 {2} | {3} 스킵" },
                { TransitWaitStatus.KeyStatusOverviewLine, "{0} 관광객/월 | {1} 시민/월 | 업데이트 {2}" },
                { TransitWaitStatus.KeyReportNoCityLoaded, "[FB] 통계 보고서를 요청했지만 도시가 로드되지 않았습니다." },
                { TransitWaitStatus.KeyReportTitle, "Stats to Log 스냅샷 - Fast Boarding" },
                { TransitWaitStatus.KeyReportSettings, "설정: {0}" },
                { TransitWaitStatus.KeyReportNote, "노선 힌트는 해당 정류장에서 대기 시간이 가장 높은 웨이포인트에서 옵니다." },
                { TransitWaitStatus.KeyReportTesterHintsHeader, "테스터 힌트" },
                { TransitWaitStatus.KeyReportHintWorstStops, "최악 정류장: 게임 안이나 Scene Explorer 모드로 먼저 확인하세요. 사고, 막힌 교통, 나쁜 정류장 위치, 버그난 정류장을 찾아보세요." },
                { TransitWaitStatus.KeyReportHintSkippedCims, "스킵된 혼자 이동 시민: 교통수단이 출발하도록 스킵한 늦은 승객입니다. 이후 상태는 보통 'has path' 또는 'assigned'가 되어야 합니다. 'no path yet'로 남으면 시간이 더 지난 뒤 해당 시민 엔티티를 확인하세요." },
                { TransitWaitStatus.KeyReportHintLateGroups, "늦은 그룹: 바닐라에 맡긴 가족/그룹입니다. 수가 많으면 향후 안전한 그룹 이동 지원을 위한 단서가 됩니다." },
                { TransitWaitStatus.KeyReportFamilyHeader, "{0}" },
                { TransitWaitStatus.KeyReportServedStops, "서비스 중인 정류장: {0}" },
                { TransitWaitStatus.KeyReportStopsWithWaiting, "대기 승객이 있는 정류장: {0}" },
                { TransitWaitStatus.KeyReportWaitingPassengers, "대기 승객: {0}" },
                { TransitWaitStatus.KeyReportAverageWait, "평균 대기: {0}" },
                { TransitWaitStatus.KeyReportLateBoardersSkipped, "오늘 스킵된 늦은 승객: {0}" },
                { TransitWaitStatus.KeyReportWorstStopNone, "최악 정류장: 현재 대기 승객 없음." },
                { TransitWaitStatus.KeyReportWorstStopAverageWait, "최악 정류장 평균 대기: {0}" },
                { TransitWaitStatus.KeyReportWorstStopName, "최악 정류장 이름: {0}" },
                { TransitWaitStatus.KeyReportWorstStopEntity, "최악 정류장 엔티티: {0}" },
                { TransitWaitStatus.KeyReportWorstWaypointEntity, "최악 웨이포인트 엔티티: {0}" },
                { TransitWaitStatus.KeyReportWorstLineHint, "최악 노선 힌트: {0}" },
                { TransitWaitStatus.KeyReportWorstLineEntity, "최악 노선 엔티티: {0}" },
                { TransitWaitStatus.KeyReportWorstLineWaypointAverage, "최악 노선 웨이포인트 평균: {0}, 대기 {1}" },
                { TransitWaitStatus.KeyReportTopWorstStopsHeader, "평균 대기 기준 최악 정류장 상위 {0}개:" },
                { TransitWaitStatus.KeyReportTopWorstStopLine, "{0}. {1} | 평균 {2} | 대기 {3} | 정류장 {4} | 웨이포인트 {5} | 노선 {6} | 힌트 {7}" },
                { TransitWaitStatus.KeyReportLateGroups, "그대로 둔 늦은 그룹 승객: {0}명, {1}그룹, 차량 {2}대" },
                { TransitWaitStatus.KeyReportLastSkippedSamplesHeader, "스킵된 혼자 이동 시민 예시" },
                { TransitWaitStatus.KeyReportLastSkippedSampleLine, "{0}. {1} | 승객 {2} | 차량 {3} | 프레임 {4} | 시간 {5} | 현재 {6}" },
                { TransitWaitStatus.KeyReportNone, "없음" },
                { TransitWaitStatus.KeyReportUnknown, "(알 수 없음)" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "모드" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "이 모드의 표시 이름입니다." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "버전" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "현재 모드 버전입니다." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "제작자의 Paradox Mods 페이지를 엽니다." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableVerboseLogging)), "자세한 로그 켜기" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableVerboseLogging)), "**디버그 / 테스트 전용**\n도시가 실행되는 동안 <FastBoarding.log>에 실시간 진단 정보를 추가합니다.\n**일반 플레이에서는 켜지 마세요.**\n켜 둔 상태는 성능을 낮추고 큰 로그 파일을 만들 수 있습니다." }
            };
        }

        public void Unload()
        {
        }
    }
}
