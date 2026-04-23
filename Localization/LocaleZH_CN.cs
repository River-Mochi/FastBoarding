// File: Localization/LocaleZH_CN.cs
// Purpose: Simplified Chinese zh-HANS locale entries for Fast Boarding.

namespace FastBoarding
{
    using Colossal;
    using System.Collections.Generic;

    /// <summary>
    /// Simplified Chinese localization source.
    /// </summary>
    public sealed class LocaleZH_CN : IDictionarySource
    {
        private readonly Setting m_Setting;

        /// <summary>
        /// Constructs the Simplified Chinese locale.
        /// </summary>
        /// <param name="setting">Settings object used for locale IDs.</param>
        public LocaleZH_CN(Setting setting)
        {
            m_Setting = setting;
        }

        /// <summary>
        /// Creates all Simplified Chinese localization entries for this mod.
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
                    $"<当前{transitName}状态>\n" +
                    "**等待中** = 当前正在等待的乘客总数。\n" +
                    "**平均** = 这些乘客的平均等待时间。\n" +
                    "**最差**站点 = 单个站点最高平均等待时间。\n" +
                    "最差站点适合优先检查：可能有交通事故、被堵住/出错的站点，或附近车辆被卡住。\n" +
                    "**跳过** = 今天由选项取消的迟到上车。\n" +
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
                    "更高的值会缩短公交站的上车/装载时间。\n" +
                    "这会让正常队列更快清空，但由于原版设计，迟到乘客仍可能拖延发车。\n" +
                    "如果想让迟到的单独市民错过车辆，请启用 [✓] <让车辆不等迟到市民而离开>。\n" +
                    "2x 约等于两倍上车速度。\n" +
                    "技术说明：更高的 loading factor 表示计划停站时间更短，而 boarding time 更像是乘客侧等待/上车估算。\n" +
                    "这不等同于强制车辆离开。"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RailBoardingSpeedFactor)), "轨道上车速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RailBoardingSpeedFactor)),
                    "<1x = 原版>\n" +
                    "适用于火车、电车和地铁站。\n" +
                    "更高的值会缩短轨道站的上车/装载时间。\n" +
                    "这会让正常队列更快清空，但由于原版设计，迟到乘客仍可能拖延发车。\n" +
                    "如果想让迟到的单独市民错过车辆，请启用 [✓] <让车辆不等迟到市民而离开>。\n" +
                    "2x 约等于两倍上车速度。"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WaterBoardingSpeedFactor)), "船舶 + 渡轮速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WaterBoardingSpeedFactor)),
                    "<1x = 原版>\n" +
                    "适用于客船和渡轮站。\n" +
                    "更高的值会缩短客船和渡轮站的上车/装载时间。\n" +
                    "这会让正常队列更快清空，但由于原版设计，迟到乘客仍可能拖延发车。\n" +
                    "如果想让迟到的单独市民错过车辆，请启用 [✓] <让车辆不等迟到市民而离开>。\n" +
                    "2x 约等于两倍上车速度。"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AirBoardingSpeedFactor)), "飞机登机速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AirBoardingSpeedFactor)),
                    "<1x = 原版>\n" +
                    "适用于客运飞机航站楼。\n" +
                    "更高的值会缩短飞机航站楼的登机/装载时间。\n" +
                    "这会让正常队列更快清空，但由于原版设计，迟到乘客仍可能拖延发车。\n" +
                    "如果想让迟到的单独市民错过车辆，请启用 [✓] <让车辆不等迟到市民而离开>。\n" +
                    "2x 约等于两倍上车速度。"
                },

                // Late passenger behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CancelLateBoarders)), "让车辆不等迟到市民而离开" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CancelLateBoarders)),
                    "**BETA**\n" +
                    "在原版发车时间后仍然<未准备好>的迟到乘客，可以错过该车辆。\n" +
                    "注意：目前我们只跳过单独出行的迟到市民，因此同行群组/家庭<不会被跳过>，仍可能像原版一样造成延误。\n" +
                    "与大量单独乘客相比，群组出行者数量较少。\n" +
                    "被跳过的迟到市民不会被删除；之后由原版系统继续分配他们。"
                },

                // Status overview
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusOverview)), "总使用量" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusOverview)),
                    "来自游戏交通信息视图的每月公共交通使用量。\n" +
                    "更新时间显示此状态快照的生成时间。"
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
                    "包括等待总数、每种交通的最差 3 个站点、被跳过市民示例和实体 ID。"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "打开日志" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "如果存在则打开 **FastBoarding.log**。\n" +
                    "如果找不到文件，则打开 Logs 文件夹。"
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
                { TransitWaitStatus.KeyReportHintWorstStops, "最差站点：请先在游戏内或使用 Scene Explorer 模组检查。查看是否有事故、交通堵塞、站点位置不佳或站点出错。" },
                { TransitWaitStatus.KeyReportHintSkippedCims, "被跳过的单独市民：这些是为了让公交/列车离开而跳过的迟到乘客。之后状态通常应变成 'has path' 或 'assigned'。如果一直是 'no path yet'，请过一段时间再检查该市民实体。" },
                { TransitWaitStatus.KeyReportHintLateGroups, "迟到群组：这些家庭/群组仍交给原版处理。数量高时，可作为以后安全支持群组出行的线索。" },
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
                { TransitWaitStatus.KeyReportLastSkippedSamplesHeader, "被跳过的单独市民示例" },
                { TransitWaitStatus.KeyReportLastSkippedSampleLine, "{0}. {1} | 乘客 {2} | 车辆 {3} | 帧 {4} | 时间 {5} | 当前 {6}" },
                { TransitWaitStatus.KeyReportNone, "无" },
                { TransitWaitStatus.KeyReportUnknown, "（未知）" },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "模组" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "此模组的显示名称。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "版本" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "当前模组版本。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "打开作者的 Paradox Mods 页面。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableVerboseLogging)), "启用详细日志" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableVerboseLogging)),
                    "**仅调试/测试使用**\n" +
                    "城市运行时向 <FastBoarding.log> 添加实时诊断信息。\n" +
                    "**正常游戏时请勿启用。**\n" +
                    "保持开启可能降低性能并生成巨大的日志文件。\n" +
                    "之后可以删除旧日志文件。"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
