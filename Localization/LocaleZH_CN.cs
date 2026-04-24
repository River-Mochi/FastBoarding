// File: Localization/LocaleZH_CN.cs
// Purpose: Simplified Chinese zh-CN locale entries for Fast Boarding.

namespace FastBoarding
{
    using Colossal;
    using System.Collections.Generic;

    /// <summary>
    /// English localization source.
    /// </summary>
    public sealed class LocaleZH_CN : IDictionarySource
    {
        private readonly Setting m_Setting;

        /// <summary>
        /// Constructs the English locale.
        /// </summary>
        /// <param name="setting">Settings object used for locale IDs.</param>
        public LocaleZH_CN(Setting setting)
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

            const string ToggleLabel = "跳过迟到乘客";

            // One helper keeps all seven status tooltips in sync for future translations.
            string StatusDescription(string transitName)
            {
                return
                    $"<当前{transitName}状态>\n" +
                    "**等待中** = 现在正在等待的乘客总数。\n" +
                    "**平均** = 这些乘客的平均等待时间。\n" +
                    "**最差**站点 = 单个站点最高平均等待时间。\n" +
                    "最差站点很适合优先检查：可能有交通事故、被堵住/出错的站点，或附近车辆被卡住。\n" +
                    $"**跳过** = 今天通过 <{ToggleLabel}> 跳过的迟到单独乘客。\n" +
                    "使用 <统计到日志> 查看详细报告：站点名称、实体 ID 等。";
            }

            return new Dictionary<string, string>
            {
                // Options mod name
                { m_Setting.GetSettingsLocaleID(), title },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "操作" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "关于" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.SpeedGroup), "上车速度" },
                { m_Setting.GetOptionGroupLocaleID(Setting.BehaviorGroup), "行为" },
                { m_Setting.GetOptionGroupLocaleID(Setting.StatusGroup), "状态" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGroup), "模组信息" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGroup), "链接" },
                { m_Setting.GetOptionGroupLocaleID(Setting.DebugGroup), "调试" },

                // Boarding speed sliders
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BusBoardingSpeedFactor)), "公交上车速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BusBoardingSpeedFactor)),
                    "<1x = 原版>\n" +
                    "更高的数值会缩短公交站的上车/装载时间。\n" +
                    "普通排队会更快消化，但按照原版设计，迟到乘客仍可能拖慢发车。\n" +
                    $"使用 [✓] <{ToggleLabel}>，让公交别一直等迟到的 Cim。\n" +
                    "2x 大约是双倍上车速度。\n" +
                    "技术说明：loading factor 越高，计划停站时间越短；boarding time 更像乘客侧的等待/上车估算。\n" +
                    $"这和 <{ToggleLabel}> 不一样；这个勾选项决定了发车时间过后，迟到 Cim 是否可以错过这班车。\n" +
                    "<==========================>\n" +
                    "所有交通共用的 loading factor:\n" +
                    "1x  = 原版停站时间 100%\n" +
                    "2x  = 计划停站时间 ~ 1/2\n" +
                    "4x  = 计划停站时间 ~ 1/4\n" +
                    "10x = 计划停站时间 ~ 1/10"

                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RailBoardingSpeedFactor)), "轨道上车速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RailBoardingSpeedFactor)),
                    "<1x = 原版>\n" +
                    "适用于火车、电车和地铁。\n" +
                    "更高的数值会缩短轨道站的上车/装载时间。\n" +
                    "普通排队会更快消化，但按照原版设计，迟到乘客仍可能拖慢发车。\n" +
                    $"使用 [✓] <{ToggleLabel}>，可以让迟到的 Cim 在发车时间过后错过这班车。\n" +
                    "之后游戏通常会自然重新分配该 Cim。\n" +
                    "2x 大约是双倍上车速度。"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WaterBoardingSpeedFactor)), "轮船 + 渡轮速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WaterBoardingSpeedFactor)),
                    "<1x = 原版>\n" +
                    "适用于客船和渡轮。\n" +
                    "更高的数值会缩短水上交通站点的上车/装载时间。\n" +
                    "普通排队会更快消化，但按照原版设计，迟到乘客仍可能拖慢发车。\n" +
                    $"使用 [✓] <{ToggleLabel}>，可以让迟到的 Cim 在发车时间过后错过这班车。\n" +
                    "2x 大约是双倍上车速度。"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AirBoardingSpeedFactor)), "飞机上车速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AirBoardingSpeedFactor)),
                    "<1x = 原版>\n" +
                    "适用于客运机场航站楼。\n" +
                    "更高的数值会缩短机场的上车/装载时间。\n" +
                    "普通排队会更快消化，但按照原版设计，迟到乘客仍可能拖慢发车。\n" +
                    $"使用 [✓] <{ToggleLabel}>，可以让迟到的 Cim 在发车时间过后错过这班车。\n" +
                    "2x 大约是双倍上车速度。"
                },

                // Late passenger behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CancelLateBoarders)), ToggleLabel },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CancelLateBoarders)),
                    "在发车时间之后仍然 <未准备好> 的乘客，可以错过这班车。\n" +
                    "注意：目前我们只跳过迟到的单独市民。\n" +
                    "一起出行的群组/家庭如果迟到，<不会被跳过>，仍然可能像原版一样造成延误。\n" +
                    "群组只占人群中的一小部分，主要收益还是来自跳过迟到的单独 Cim。\n" +
                    "被跳过的市民不会被删除；之后会由游戏自然重新分配。"
                },

                // Status overview
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusOverview)), "总使用量" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusOverview)),
                    "来自游戏交通信息视图的每月公共交通使用量。\n" +
                    "更新时间显示这次状态快照是什么时候取的（通常是在打开选项时）。"
                },

                // Status rows
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusBus)), "公交" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusBus)), StatusDescription("公交") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTram)), "电车" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTram)), StatusDescription("电车") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTrain)), "火车" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTrain)), StatusDescription("火车") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSubway)), "地铁" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSubway)), StatusDescription("地铁") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusFerry)), "渡轮" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusFerry)), StatusDescription("渡轮") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusShip)), "客船" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusShip)), StatusDescription("客船") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusAir)), "飞机" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusAir)), StatusDescription("飞机") },

                // Status buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatsToLog)), "统计到日志" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatsToLog)),
                    "向 **FastBoarding.log** 写入一次详细报告。\n" +
                    "包括等待总数、每种交通的最差 3 个站点、被跳过的 Cim 示例、实体 ID 和线路提示。"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "打开日志" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "如果存在就打开 **FastBoarding.log**。\n" +
                    "如果还没有这个文件，就打开 Logs 文件夹。"
                },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "模组" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "这个模组的显示名称。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "版本" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "当前模组版本。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "打开作者的 Paradox Mods 页面。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableVerboseLogging)), "启用详细日志" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableVerboseLogging)),
                    "**仅调试/测试使用**\n" +
                    "城市运行时向 <Logs/FastBoarding.log> 添加 <live> 详细信息。\n" +
                    "**正常游玩时不要开启。**\n" +
                    "一直开着可能会降低性能，还会生成很大的日志文件。\n" +
                    "旧日志之后可以删掉。\n" +
                    "注意：<统计到日志> 只是当前瞬间的一次快照。\n" +
                    "如果你想看一段时间内发生了什么，就把详细日志开 15-30 分钟。\n" +
                    "正常游玩前记得再切回 **OFF**。"

                },


                // Runtime status strings
                { TransitWaitStatus.KeyStatusNotLoaded, "状态未加载。" },
                { TransitWaitStatus.KeyNoCityLoaded, "未加载城市。" },
                { TransitWaitStatus.KeyNoStopsFound, "未找到站点。" },
                { TransitWaitStatus.KeyStatusLine, "{0} 等待 | 平均 {1} | 最差 {2} | {3} 跳过" },
                { TransitWaitStatus.KeyStatusOverviewLine, "{0} 游客/月 | {1} 市民/月 | 更新 {2}" },

                // Stats-to-log report strings
                { TransitWaitStatus.KeyReportNoCityLoaded, "[FB] 请求统计报告，但未加载城市。" },
                { TransitWaitStatus.KeyReportTitle, "统计到日志快照 - Fast Boarding" },
                { TransitWaitStatus.KeyReportSettings, "设置：{0}" },
                { TransitWaitStatus.KeyReportNote, "线路提示来自该站点等待最高的路径点。" },
                { TransitWaitStatus.KeyReportTesterHintsHeader, "测试提示" },
                { TransitWaitStatus.KeyReportHintWorstStops, "最差站点：先在游戏内或用 Scene Explorer 模组检查。看看有没有事故、交通堵塞、站点位置不好，或者站点出错。" },
                { TransitWaitStatus.KeyReportHintSkippedCims, "被跳过的单独 Cim：这些是为了让交通工具先走而跳过的迟到乘客。之后状态通常会变成 'has path' 或 'assigned'。如果一直是 'no path yet'，就过一会儿再检查这个实体。" },
                { TransitWaitStatus.KeyReportHintLateGroups, "迟到群组：这些家庭/群组还是交给原版处理。数量高的话，可以作为以后安全支持群组出行的线索。" },
                { TransitWaitStatus.KeyReportFamilyHeader, "{0}" },
                { TransitWaitStatus.KeyReportServedStops, "服务站点：{0}" },
                { TransitWaitStatus.KeyReportStopsWithWaiting, "有等待乘客的站点：{0}" },
                { TransitWaitStatus.KeyReportWaitingPassengers, "等待乘客：{0}" },
                { TransitWaitStatus.KeyReportAverageWait, "平均等待：{0}" },
                { TransitWaitStatus.KeyReportLateBoardersSkipped, "今天跳过的迟到乘客：{0}" },
                { TransitWaitStatus.KeyReportWorstStopNone, "最差站点：当前没有等待乘客。" },
                { TransitWaitStatus.KeyReportWorstStopAverageWait, "最差站点平均等待：{0}" },
                { TransitWaitStatus.KeyReportWorstStopName, "最差站点名称：{0}" },
                { TransitWaitStatus.KeyReportWorstStopEntity, "最差站点实体：{0}" },
                { TransitWaitStatus.KeyReportWorstWaypointEntity, "最差路径点实体：{0}" },
                { TransitWaitStatus.KeyReportWorstLineHint, "线路提示：{0}" },
                { TransitWaitStatus.KeyReportWorstLineEntity, "线路实体：{0}" },
                { TransitWaitStatus.KeyReportWorstLineWaypointAverage, "线路路径点平均：{0}，等待 {1}" },
                { TransitWaitStatus.KeyReportTopWorstStopsHeader, "平均等待最差的前 {0} 个站点：" },
                { TransitWaitStatus.KeyReportTopWorstStopLine, "{0}. {1} | 平均 {2} | 等待 {3} | 站点 {4} | 路径点 {5} | 线路 {6} | 提示 {7}" },
                { TransitWaitStatus.KeyReportLateGroups, "未跳过的迟到群组：{0} 名乘客，{1} 个群组，{2} 辆车" },
                { TransitWaitStatus.KeyReportLastSkippedSamplesHeader, "这个当前时刻被跳过的迟到单独 Cim 示例" },
                { TransitWaitStatus.KeyReportLastSkippedSampleLine, "{0}. {1} | 乘客 {2} | 错过车辆 {3} | 时间 {4} | 当前 {5}" },
                { TransitWaitStatus.KeyReportNone, "无" },
                { TransitWaitStatus.KeyReportUnknown, "（未知）" },


            };
        }

        public void Unload()
        {
        }
    }
}
