// File: Localization/LocaleKO.cs
// Purpose: Korean ko-KR locale entries for Fast Boarding.

namespace FastBoarding
{
    using Colossal;
    using System.Collections.Generic;

    /// <summary>
    /// English localization source.
    /// </summary>
    public sealed class LocaleKO : IDictionarySource
    {
        private readonly Setting m_Setting;

        /// <summary>
        /// Constructs the English locale.
        /// </summary>
        /// <param name="setting">Settings object used for locale IDs.</param>
        public LocaleKO(Setting setting)
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

            const string ToggleName = "늦은 승객 건너뛰기";

            // One helper keeps all seven status tooltips in sync for future translations.
            string StatusDescription(string transitName)
            {
                return
                    $"<현재 {transitName} 상태>\n" +
                    "**대기** = 지금 기다리는 승객 총수입니다.\n" +
                    "**평균** = 그 승객들의 평균 대기 시간입니다.\n" +
                    "**최악** 정류장 = 한 정류장에서 가장 높은 평균 대기 시간입니다.\n" +
                    "최악의 정류장은 사고, 막힌/버그 난 정류장, 근처에서 묶인 차량을 확인하기 좋은 곳입니다.\n" +
                    $"**건너뜀** = 오늘 <{ToggleName}> 으로 건너뛴 늦은 솔로 승객입니다.\n" +
                    "<Stats to Log>를 쓰면 정류장 이름, Entity ID 같은 자세한 보고서를 볼 수 있습니다.";
            }

            return new Dictionary<string, string>
            {
                // Options mod name
                { m_Setting.GetSettingsLocaleID(), title },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "동작" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "정보" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.SpeedGroup), "탑승 속도" },
                { m_Setting.GetOptionGroupLocaleID(Setting.BehaviorGroup), "동작" },
                { m_Setting.GetOptionGroupLocaleID(Setting.StatusGroup), "상태" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGroup), "모드 정보" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGroup), "링크" },
                { m_Setting.GetOptionGroupLocaleID(Setting.DebugGroup), "디버그" },

                // Boarding speed sliders
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BusBoardingSpeedFactor)), "버스 탑승 속도" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BusBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "값을 높이면 버스 정류장의 탑승/적재 시간이 줄어듭니다.\n" +
                    "일반 대기열은 더 빨리 풀리지만, 바닐라 설계상 늦은 승객이 여전히 출발을 지연시킬 수 있습니다.\n" +
                    $"[✓] <{ToggleName}> 를 쓰면 버스가 늦은 Cim을 계속 기다리지 않게 할 수 있습니다.\n" +
                    "2x 는 대략 2배 탑승 속도입니다.\n" +
                    "기술 메모: 적재 값이 높을수록 계획된 정차 시간이 짧아지고, boarding time 은 승객 쪽 대기/탑승 추정치에 더 가깝습니다.\n" +
                    $"이건 <{ToggleName}> 와는 다릅니다. 이 체크박스는 출발 시간 뒤에 늦은 Cim이 그 차량을 놓쳐도 되는지를 정합니다.\n" +
                    "<==========================>\n" +
                    "모든 대중교통 적재 값:\n" +
                    "1x  = 바닐라 정차 시간 100%\n" +
                    "2x  = 계획 정차 시간 ~ 1/2\n" +
                    "4x  = 계획 정차 시간 ~ 1/4\n" +
                    "10x = 계획 정차 시간 ~ 1/10"

                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RailBoardingSpeedFactor)), "철도 탑승 속도" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RailBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "기차, 트램, 지하철에 적용됩니다.\n" +
                    "값을 높이면 철도 정류장의 탑승/적재 시간이 줄어듭니다.\n" +
                    "일반 대기열은 더 빨리 풀리지만, 바닐라 설계상 늦은 승객이 여전히 출발을 지연시킬 수 있습니다.\n" +
                    $"[✓] <{ToggleName}> 를 쓰면 출발 시간 뒤에 늦은 Cim이 그 차량을 놓치게 할 수 있습니다.\n" +
                    "그 뒤에는 게임이 보통 자연스럽게 다시 배정합니다.\n" +
                    "2x 는 대략 2배 탑승 속도입니다."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WaterBoardingSpeedFactor)), "선박 + 페리 속도" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WaterBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "선박과 페리에 적용됩니다.\n" +
                    "값을 높이면 수상 교통 정류장의 탑승/적재 시간이 줄어듭니다.\n" +
                    "일반 대기열은 더 빨리 풀리지만, 바닐라 설계상 늦은 승객이 여전히 출발을 지연시킬 수 있습니다.\n" +
                    $"[✓] <{ToggleName}> 를 쓰면 출발 시간 뒤에 늦은 Cim이 그 차량을 놓치게 할 수 있습니다.\n" +
                    "2x 는 대략 2배 탑승 속도입니다."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AirBoardingSpeedFactor)), "비행기 탑승 속도" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AirBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "여객 공항 터미널에 적용됩니다.\n" +
                    "값을 높이면 공항의 탑승/적재 시간이 줄어듭니다.\n" +
                    "일반 대기열은 더 빨리 풀리지만, 바닐라 설계상 늦은 승객이 여전히 출발을 지연시킬 수 있습니다.\n" +
                    $"[✓] <{ToggleName}> 를 쓰면 출발 시간 뒤에 늦은 Cim이 그 차량을 놓치게 할 수 있습니다.\n" +
                    "2x 는 대략 2배 탑승 속도입니다."
                },

                // Late passenger behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CancelLateBoarders)), ToggleName },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CancelLateBoarders)),
                    "출발 시간 이후에도 아직 <준비 안 됨> 상태인 승객은 그 차량을 놓칠 수 있게 됩니다.\n" +
                    "참고: 지금은 늦은 솔로 시민만 건너뜁니다.\n" +
                    "함께 이동하는 그룹/가족이 늦은 경우는 <건너뛰지 않으며> 바닐라처럼 여전히 지연을 만들 수 있습니다.\n" +
                    "그룹은 전체 인원에서 작은 비중이고, 큰 효과는 늦은 솔로 Cim을 건너뛰는 데서 옵니다.\n" +
                    "건너뛴 시민은 삭제되지 않으며, 게임이 이후 자연스럽게 다시 배정합니다."
                },

                // Status overview
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusOverview)), "총 이용량" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusOverview)),
                    "게임 교통 인포뷰의 월간 대중교통 이용량입니다.\n" +
                    "업데이트 시각은 이 상태 스냅샷을 찍은 때를 보여줍니다(보통 옵션 창을 열었을 때)."
                },

                // Status rows
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

                // Status buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatsToLog)), "Stats to Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatsToLog)),
                    "**FastBoarding.log** 에 자세한 1회 보고서를 씁니다.\n" +
                    "대기 인원, 교통수단별 최악 정류장 TOP 3, 건너뛴 Cim 예시, Entity ID, 노선 힌트를 포함합니다."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "로그 열기" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "**FastBoarding.log** 가 있으면 엽니다.\n" +
                    "아직 없으면 Logs 폴더를 엽니다."
                },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "모드" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "이 모드의 표시 이름입니다." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "버전" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "현재 모드 버전입니다." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "제작자의 Paradox Mods 페이지를 엽니다." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableVerboseLogging)), "상세 로그 켜기" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableVerboseLogging)),
                    "**디버그 / 테스트 전용**\n" +
                    "도시가 돌아가는 동안 <Logs/FastBoarding.log> 에 <live> 상세를 추가합니다.\n" +
                    "**일반 플레이에서는 켜지 마세요.**\n" +
                    "켜 둔 채로 두면 성능이 떨어지고 로그 파일이 엄청 커질 수 있습니다.\n" +
                    "오래된 로그는 나중에 지워도 됩니다.\n" +
                    "참고: <Stats to Log> 는 그 순간의 스냅샷일 뿐입니다.\n" +
                    "시간 흐름을 보고 싶다면 상세 로그를 15~30분 정도 켜 두세요.\n" +
                    "일반 플레이 전에는 다시 **끔** 으로 돌려두는 걸 잊지 마세요."

                },


                // Runtime status strings
                { TransitWaitStatus.KeyStatusNotLoaded, "상태가 아직 로드되지 않았습니다." },
                { TransitWaitStatus.KeyNoCityLoaded, "도시가 로드되지 않았습니다." },
                { TransitWaitStatus.KeyNoStopsFound, "정류장을 찾지 못했습니다." },
                { TransitWaitStatus.KeyStatusLine, "{0} 대기 | 평균 {1} | 최악 {2} | {3} 건너뜀" },
                { TransitWaitStatus.KeyStatusOverviewLine, "{0} 관광객/월 | {1} 시민/월 | 업데이트 {2}" },

                // Stats-to-log report strings
                { TransitWaitStatus.KeyReportNoCityLoaded, "[FB] 보고서를 요청했지만 도시가 로드되지 않았습니다." },
                { TransitWaitStatus.KeyReportTitle, "Stats to Log 스냅샷 - Fast Boarding" },
                { TransitWaitStatus.KeyReportSettings, "설정: {0}" },
                { TransitWaitStatus.KeyReportNote, "노선 힌트는 그 정류장에서 가장 많이 기다리는 웨이포인트를 기준으로 합니다." },
                { TransitWaitStatus.KeyReportTesterHintsHeader, "테스터 힌트" },
                { TransitWaitStatus.KeyReportHintWorstStops, "최악의 정류장: 먼저 게임 안이나 Scene Explorer 모드로 확인하세요. 사고, 교통, 나쁜 위치, 버그 난 정류장을 살펴보면 됩니다." },
                { TransitWaitStatus.KeyReportHintSkippedCims, "건너뛴 솔로 Cim: 교통수단을 출발시키기 위해 건너뛴 늦은 승객입니다. 이후 상태는 보통 'has path' 나 'assigned' 가 됩니다. 계속 'no path yet' 이면 시간을 두고 그 Entity 를 다시 확인하세요." },
                { TransitWaitStatus.KeyReportHintLateGroups, "늦은 그룹: 바닐라에 맡겨둔 가족/그룹입니다. 숫자가 많으면 나중에 안전한 그룹 이동 지원을 추가할 단서가 됩니다." },
                { TransitWaitStatus.KeyReportFamilyHeader, "{0}" },
                { TransitWaitStatus.KeyReportServedStops, "운행 중 정류장: {0}" },
                { TransitWaitStatus.KeyReportStopsWithWaiting, "승객이 기다리는 정류장: {0}" },
                { TransitWaitStatus.KeyReportWaitingPassengers, "대기 승객: {0}" },
                { TransitWaitStatus.KeyReportAverageWait, "평균 대기: {0}" },
                { TransitWaitStatus.KeyReportLateBoardersSkipped, "오늘 건너뛴 늦은 승객: {0}" },
                { TransitWaitStatus.KeyReportWorstStopNone, "최악 정류장: 지금은 기다리는 승객이 없습니다." },
                { TransitWaitStatus.KeyReportWorstStopAverageWait, "최악 정류장 평균 대기: {0}" },
                { TransitWaitStatus.KeyReportWorstStopName, "최악 정류장 이름: {0}" },
                { TransitWaitStatus.KeyReportWorstStopEntity, "최악 정류장 Entity: {0}" },
                { TransitWaitStatus.KeyReportWorstWaypointEntity, "웨이포인트 Entity: {0}" },
                { TransitWaitStatus.KeyReportWorstLineHint, "노선 힌트: {0}" },
                { TransitWaitStatus.KeyReportWorstLineEntity, "노선 Entity: {0}" },
                { TransitWaitStatus.KeyReportWorstLineWaypointAverage, "노선 웨이포인트 평균: {0} / 대기 {1}" },
                { TransitWaitStatus.KeyReportTopWorstStopsHeader, "평균 대기가 가장 나쁜 정류장 TOP {0}:" },
                { TransitWaitStatus.KeyReportTopWorstStopLine, "{0}. {1} | 평균 {2} | 대기 {3} | 정류장 {4} | 웨이포인트 {5} | 노선 {6} | 힌트 {7}" },
                { TransitWaitStatus.KeyReportLateGroups, "건너뛰지 않은 늦은 그룹: {0}명 / {1}그룹 / {2}대 차량" },
                { TransitWaitStatus.KeyReportLastSkippedSamplesHeader, "이 현재 시점에 건너뛴 늦은 솔로 Cim 예시" },
                { TransitWaitStatus.KeyReportLastSkippedSampleLine, "{0}. {1} | 승객 {2} | 놓친 차량 {3} | 시간 {4} | 현재 {5}" },
                { TransitWaitStatus.KeyReportNone, "없음" },
                { TransitWaitStatus.KeyReportUnknown, "(알 수 없음)" },


            };
        }

        public void Unload()
        {
        }
    }
}
