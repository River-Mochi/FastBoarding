// File: Localization/LocaleZH_HANT.cs
// Purpose: Traditional Chinese zh-HANT locale entries for Fast Boarding.

namespace FastBoarding
{
    using Colossal;
    using System.Collections.Generic;

    /// <summary>
    /// English localization source.
    /// </summary>
    public sealed class LocaleZH_HANT : IDictionarySource
    {
        private readonly Setting m_Setting;

        /// <summary>
        /// Constructs the English locale.
        /// </summary>
        /// <param name="setting">Settings object used for locale IDs.</param>
        public LocaleZH_HANT(Setting setting)
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

            const string ToggleName = "跳過遲到乘客";

            // One helper keeps all seven status tooltips in sync for future translations.
            string StatusDescription(string transitName)
            {
                return
                    $"<目前{transitName}狀態>\n" +
                    "**等待中** = 目前正在等待的乘客總數。\n" +
                    "**平均** = 這些乘客的平均等待時間。\n" +
                    "**最差**站點 = 單一站點最高平均等待時間。\n" +
                    "最差站點很適合優先檢查：可能有交通事故、被堵住/出錯的站點，或附近車輛被卡住。\n" +
                    $"**跳過** = 今天透過 <{ToggleName}> 跳過的遲到單獨乘客。\n" +
                    "使用 <統計到日誌> 查看詳細報告：站點名稱、實體 ID 等。";
            }

            return new Dictionary<string, string>
            {
                // Options mod name
                { m_Setting.GetSettingsLocaleID(), title },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "操作" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "關於" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.SpeedGroup), "上車速度" },
                { m_Setting.GetOptionGroupLocaleID(Setting.BehaviorGroup), "行為" },
                { m_Setting.GetOptionGroupLocaleID(Setting.StatusGroup), "狀態" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGroup), "模組資訊" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGroup), "連結" },
                { m_Setting.GetOptionGroupLocaleID(Setting.DebugGroup), "除錯" },

                // Boarding speed sliders
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BusBoardingSpeedFactor)), "公車上車速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BusBoardingSpeedFactor)),
                    "<1x = 原版>\n" +
                    "較高的數值會縮短公車站的上車/裝載時間。\n" +
                    "一般排隊會更快消化，但依照原版設計，遲到乘客仍可能拖慢發車。\n" +
                    $"使用 [✓] <{ToggleName}>，讓公車不要一直等遲到的 Cim。\n" +
                    "2x 大約是雙倍上車速度。\n" +
                    "技術說明：裝載值越高，計畫停站時間越短；boarding time 更像乘客端的等待/上車估算。\n" +
                    $"這和 <{ToggleName}> 不一樣；這個勾選項決定了發車時間過後，遲到 Cim 是否可以錯過這班車。\n" +
                    "<==========================>\n" +
                    "所有交通共用的裝載值:\n" +
                    "1x  = 原版停站時間 100%\n" +
                    "2x  = 計畫停站時間 ~ 1/2\n" +
                    "4x  = 計畫停站時間 ~ 1/4\n" +
                    "10x = 計畫停站時間 ~ 1/10"

                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RailBoardingSpeedFactor)), "軌道上車速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RailBoardingSpeedFactor)),
                    "<1x = 原版>\n" +
                    "適用於火車、電車和地鐵。\n" +
                    "較高的數值會縮短軌道站的上車/裝載時間。\n" +
                    "一般排隊會更快消化，但依照原版設計，遲到乘客仍可能拖慢發車。\n" +
                    $"使用 [✓] <{ToggleName}>，可以讓遲到的 Cim 在發車時間過後錯過這班車。\n" +
                    "之後遊戲通常會自然重新分配該 Cim。\n" +
                    "2x 大約是雙倍上車速度。"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WaterBoardingSpeedFactor)), "船舶 + 渡輪速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WaterBoardingSpeedFactor)),
                    "<1x = 原版>\n" +
                    "適用於客船和渡輪。\n" +
                    "較高的數值會縮短水上交通站點的上車/裝載時間。\n" +
                    "一般排隊會更快消化，但依照原版設計，遲到乘客仍可能拖慢發車。\n" +
                    $"使用 [✓] <{ToggleName}>，可以讓遲到的 Cim 在發車時間過後錯過這班車。\n" +
                    "2x 大約是雙倍上車速度。"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AirBoardingSpeedFactor)), "飛機上車速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AirBoardingSpeedFactor)),
                    "<1x = 原版>\n" +
                    "適用於客運機場航廈。\n" +
                    "較高的數值會縮短機場的上車/裝載時間。\n" +
                    "一般排隊會更快消化，但依照原版設計，遲到乘客仍可能拖慢發車。\n" +
                    $"使用 [✓] <{ToggleName}>，可以讓遲到的 Cim 在發車時間過後錯過這班車。\n" +
                    "2x 大約是雙倍上車速度。"
                },

                // Late passenger behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CancelLateBoarders)), ToggleName },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CancelLateBoarders)),
                    "在發車時間之後仍然 <未準備好> 的乘客，可以錯過這班車。\n" +
                    "注意：目前我們只跳過遲到的單獨市民。\n" +
                    "一起出行的群組/家庭如果遲到，<不會被跳過>，仍然可能像原版一樣造成延誤。\n" +
                    "群組只佔人群中的一小部分，主要收益還是來自跳過遲到的單獨 Cim。\n" +
                    "被跳過的市民不會被刪除；之後會由遊戲自然重新分配。"
                },

                // Status overview
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusOverview)), "總使用量" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusOverview)),
                    "來自遊戲交通資訊視圖的每月公共交通使用量。\n" +
                    "更新時間顯示這次狀態快照是什麼時候取的（通常是在打開選項時）。"
                },

                // Status rows
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusBus)), "公車" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusBus)), StatusDescription("公車") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTram)), "電車" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTram)), StatusDescription("電車") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTrain)), "火車" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTrain)), StatusDescription("火車") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSubway)), "地鐵" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSubway)), StatusDescription("地鐵") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusFerry)), "渡輪" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusFerry)), StatusDescription("渡輪") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusShip)), "客船" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusShip)), StatusDescription("客船") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusAir)), "飛機" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusAir)), StatusDescription("飛機") },

                // Status buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatsToLog)), "統計到日誌" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatsToLog)),
                    "向 **FastBoarding.log** 寫入一次詳細報告。\n" +
                    "包括等待總數、每種交通的最差 3 個站點、被跳過的 Cim 範例、實體 ID 和路線提示。"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "開啟日誌" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "如果存在就開啟 **FastBoarding.log**。\n" +
                    "如果還沒有這個檔案，就開啟 Logs 資料夾。"
                },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "模組" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "這個模組的顯示名稱。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "版本" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "目前模組版本。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "開啟作者的 Paradox Mods 頁面。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableVerboseLogging)), "啟用詳細日誌" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableVerboseLogging)),
                    "**僅除錯/測試使用**\n" +
                    "城市運行時向 <Logs/FastBoarding.log> 加入 <live> 詳細資訊。\n" +
                    "**正常遊玩時不要開啟。**\n" +
                    "一直開著可能會降低效能，還會生成很大的日誌檔案。\n" +
                    "舊日誌之後可以刪掉。\n" +
                    "注意：<統計到日誌> 只是當前瞬間的一次快照。\n" +
                    "如果你想看一段時間內發生了什麼，就把詳細日誌開 15-30 分鐘。\n" +
                    "正常遊玩前記得再切回 **關閉**。"

                },


                // Runtime status strings
                { TransitWaitStatus.KeyStatusNotLoaded, "狀態未載入。" },
                { TransitWaitStatus.KeyNoCityLoaded, "未載入城市。" },
                { TransitWaitStatus.KeyNoStopsFound, "未找到站點。" },
                { TransitWaitStatus.KeyStatusLine, "{0} 等待 | 平均 {1} | 最差 {2} | {3} 跳過" },
                { TransitWaitStatus.KeyStatusOverviewLine, "{0} 遊客/月 | {1} 市民/月 | 更新 {2}" },

                // Stats-to-log report strings
                { TransitWaitStatus.KeyReportNoCityLoaded, "[FB] 請求統計報告，但未載入城市。" },
                { TransitWaitStatus.KeyReportTitle, "統計到日誌快照 - Fast Boarding" },
                { TransitWaitStatus.KeyReportSettings, "設定：{0}" },
                { TransitWaitStatus.KeyReportNote, "路線提示來自該站點等待最高的路徑點。" },
                { TransitWaitStatus.KeyReportTesterHintsHeader, "測試提示" },
                { TransitWaitStatus.KeyReportHintWorstStops, "最差站點：先在遊戲內或用 Scene Explorer 模組檢查。看看有沒有事故、交通堵塞、站點位置不好，或站點出錯。" },
                { TransitWaitStatus.KeyReportHintSkippedCims, "被跳過的單獨 Cim：這些是為了讓交通工具先走而跳過的遲到乘客。之後狀態通常會變成 'has path' 或 'assigned'。如果一直是 'no path yet'，就過一會兒再檢查這個實體。" },
                { TransitWaitStatus.KeyReportHintLateGroups, "遲到群組：這些家庭/群組還是交給原版處理。數量高的話，可以作為以後安全支援群組出行的線索。" },
                { TransitWaitStatus.KeyReportFamilyHeader, "{0}" },
                { TransitWaitStatus.KeyReportServedStops, "服務站點：{0}" },
                { TransitWaitStatus.KeyReportStopsWithWaiting, "有等待乘客的站點：{0}" },
                { TransitWaitStatus.KeyReportWaitingPassengers, "等待乘客：{0}" },
                { TransitWaitStatus.KeyReportAverageWait, "平均等待：{0}" },
                { TransitWaitStatus.KeyReportLateBoardersSkipped, "今天跳過的遲到乘客：{0}" },
                { TransitWaitStatus.KeyReportWorstStopNone, "最差站點：目前沒有等待乘客。" },
                { TransitWaitStatus.KeyReportWorstStopAverageWait, "最差站點平均等待：{0}" },
                { TransitWaitStatus.KeyReportWorstStopName, "最差站點名稱：{0}" },
                { TransitWaitStatus.KeyReportWorstStopEntity, "最差站點實體：{0}" },
                { TransitWaitStatus.KeyReportWorstWaypointEntity, "最差路徑點實體：{0}" },
                { TransitWaitStatus.KeyReportWorstLineHint, "路線提示：{0}" },
                { TransitWaitStatus.KeyReportWorstLineEntity, "路線實體：{0}" },
                { TransitWaitStatus.KeyReportWorstLineWaypointAverage, "路線路徑點平均：{0}，等待 {1}" },
                { TransitWaitStatus.KeyReportTopWorstStopsHeader, "平均等待最差的前 {0} 個站點：" },
                { TransitWaitStatus.KeyReportTopWorstStopLine, "{0}. {1} | 平均 {2} | 等待 {3} | 站點 {4} | 路徑點 {5} | 路線 {6} | 提示 {7}" },
                { TransitWaitStatus.KeyReportLateGroups, "未跳過的遲到群組：{0} 名乘客，{1} 個群組，{2} 輛車" },
                { TransitWaitStatus.KeyReportLastSkippedSamplesHeader, "這個目前時刻被跳過的遲到單獨 Cim 範例" },
                { TransitWaitStatus.KeyReportLastSkippedSampleLine, "{0}. {1} | 乘客 {2} | 錯過車輛 {3} | 時間 {4} | 目前 {5}" },
                { TransitWaitStatus.KeyReportNone, "無" },
                { TransitWaitStatus.KeyReportUnknown, "（未知）" },


            };
        }

        public void Unload()
        {
        }
    }
}
