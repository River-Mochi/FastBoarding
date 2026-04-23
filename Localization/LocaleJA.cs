// File: Localization/LocaleJA.cs
// Purpose: Japanese ja-JP locale entries for Fast Boarding.

namespace FastBoarding
{
    using Colossal;
    using System.Collections.Generic;

    /// <summary>
    /// Japanese localization source.
    /// </summary>
    public sealed class LocaleJA : IDictionarySource
    {
        private readonly Setting m_Setting;

        /// <summary>
        /// Constructs the Japanese locale.
        /// </summary>
        /// <param name="setting">Settings object used for locale IDs.</param>
        public LocaleJA(Setting setting)
        {
            m_Setting = setting;
        }

        /// <summary>
        /// Creates all Japanese localization entries for this mod.
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
                    $"<現在の{transitName}ステータス>\n" +
                    "**待機中** = 現在待っている乗客の合計。\n" +
                    "**平均** = その乗客たちの平均待ち時間。\n" +
                    "**最悪**停留所 = 1つの停留所で最も高い平均待ち時間。\n" +
                    "最悪停留所は、事故、詰まった/不具合のある停留所、近くで足止めされた車両を調べる良い場所です。\n" +
                    "**スキップ** = このオプションで今日キャンセルされた遅れ乗車。\n" +
                    "<Stats to Log> で停留所名、エンティティIDなどを含む詳細レポートを書き出します。";
            }

            return new Dictionary<string, string>
            {
                // Options mod name
                { m_Setting.GetSettingsLocaleID(), title },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "操作" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "情報" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.SpeedGroup), "乗車速度" },
                { m_Setting.GetOptionGroupLocaleID(Setting.BehaviorGroup), "動作" },
                { m_Setting.GetOptionGroupLocaleID(Setting.StatusGroup), "ステータス" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGroup), "Mod情報" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGroup), "リンク" },
                { m_Setting.GetOptionGroupLocaleID(Setting.DebugGroup), "デバッグ" },

                // Boarding speed sliders
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BusBoardingSpeedFactor)), "バス乗車速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BusBoardingSpeedFactor)),
                    "<1x = バニラ>\n" +
                    "値を上げるとバス停での乗車/積み込み時間が短くなります。\n" +
                    "通常の待ち行列は早く処理されますが、バニラ設計上、遅れた乗客がまだ発車を遅らせることがあります。\n" +
                    "単独の遅れたシムに車両を逃させたい場合は [✓] <遅れたシムを待たずに車両を発車> を使ってください。\n" +
                    "2x はおよそ2倍の乗車速度です。\n" +
                    "技術メモ: loading factor が高いほど予定停車時間は短くなり、boarding time は乗客側の待機/乗車見積もりに近い値です。\n" +
                    "これは車両を強制的に発車させることとは違います。"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RailBoardingSpeedFactor)), "鉄道乗車速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RailBoardingSpeedFactor)),
                    "<1x = バニラ>\n" +
                    "列車、路面電車、地下鉄の停留所に適用されます。\n" +
                    "値を上げると鉄道系停留所での乗車/積み込み時間が短くなります。\n" +
                    "通常の待ち行列は早く処理されますが、バニラ設計上、遅れた乗客がまだ発車を遅らせることがあります。\n" +
                    "単独の遅れたシムに車両を逃させたい場合は [✓] <遅れたシムを待たずに車両を発車> を使ってください。\n" +
                    "2x はおよそ2倍の乗車速度です。"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WaterBoardingSpeedFactor)), "船 + フェリー速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WaterBoardingSpeedFactor)),
                    "<1x = バニラ>\n" +
                    "船とフェリーの停留所に適用されます。\n" +
                    "値を上げると船とフェリー停留所での乗車/積み込み時間が短くなります。\n" +
                    "通常の待ち行列は早く処理されますが、バニラ設計上、遅れた乗客がまだ発車を遅らせることがあります。\n" +
                    "単独の遅れたシムに車両を逃させたい場合は [✓] <遅れたシムを待たずに車両を発車> を使ってください。\n" +
                    "2x はおよそ2倍の乗車速度です。"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AirBoardingSpeedFactor)), "飛行機搭乗速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AirBoardingSpeedFactor)),
                    "<1x = バニラ>\n" +
                    "旅客機ターミナルに適用されます。\n" +
                    "値を上げると飛行機ターミナルでの搭乗/積み込み時間が短くなります。\n" +
                    "通常の待ち行列は早く処理されますが、バニラ設計上、遅れた乗客がまだ発車を遅らせることがあります。\n" +
                    "単独の遅れたシムに車両を逃させたい場合は [✓] <遅れたシムを待たずに車両を発車> を使ってください。\n" +
                    "2x はおよそ2倍の乗車速度です。"
                },

                // Late passenger behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CancelLateBoarders)), "遅れたシムを待たずに車両を発車" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CancelLateBoarders)),
                    "**実験的BETA**\n" +
                    "バニラの発車時刻後も<準備未完了>の単独市民は、その車両を逃すことができます。\n" +
                    "注意: 一緒に移動するグループ/家族はまだ<スキップされません>。バニラと同じように一部の遅延を起こす場合があります。\n" +
                    "単独シムだけのスキップでも、停留所では単独旅行者が多数派であることが多いため効果があります。\n" +
                    "スキップされた遅れ市民は削除されません。その後はバニラのシステムが再び割り当てを続けます。"
                },

                // Status overview
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusOverview)), "総利用数" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusOverview)),
                    "ゲームの交通情報ビューから取得した月間公共交通利用数。\n" +
                    "更新時刻はこのステータスのスナップショット時刻です。"
                },

                // Status rows
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusBus)), "バス" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusBus)), StatusDescription("バス") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTram)), "路面電車" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTram)), StatusDescription("路面電車") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTrain)), "列車" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTrain)), StatusDescription("列車") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSubway)), "地下鉄" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSubway)), StatusDescription("地下鉄") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusFerry)), "フェリー" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusFerry)), StatusDescription("フェリー") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusShip)), "船" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusShip)), StatusDescription("船") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusAir)), "飛行機" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusAir)), StatusDescription("飛行機") },

                // Status buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatsToLog)), "Stats to Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatsToLog)),
                    "**FastBoarding.log** に1回だけ詳細レポートを書き込みます。\n" +
                    "待機数、各交通モードの最悪停留所トップ3、スキップされたシム例、エンティティID、路線ヒントを含みます。"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "ログを開く" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "**FastBoarding.log** があれば開きます。\n" +
                    "ファイルがまだない場合は Logs フォルダーを開きます。"
                },

                // Runtime status strings
                { TransitWaitStatus.KeyStatusNotLoaded, "ステータス未読み込み。" },
                { TransitWaitStatus.KeyNoCityLoaded, "都市が読み込まれていません。" },
                { TransitWaitStatus.KeyNoStopsFound, "停留所が見つかりません。" },
                { TransitWaitStatus.KeyStatusLine, "{0} 待機 | 平均 {1} | 最悪 {2} | {3} スキップ" },
                { TransitWaitStatus.KeyStatusOverviewLine, "{0} 観光客/月 | {1} 市民/月 | 更新 {2}" },

                // Stats-to-log report strings
                { TransitWaitStatus.KeyReportNoCityLoaded, "[FB] 統計レポートが要求されましたが、都市が読み込まれていません。" },
                { TransitWaitStatus.KeyReportTitle, "Stats to Log スナップショット - Fast Boarding" },
                { TransitWaitStatus.KeyReportSettings, "設定: {0}" },
                { TransitWaitStatus.KeyReportNote, "路線ヒントは、その停留所で最も待ち時間が高いウェイポイントから来ます。" },
                { TransitWaitStatus.KeyReportTesterHintsHeader, "テスター向けヒント" },
                { TransitWaitStatus.KeyReportHintWorstStops, "最悪停留所: まずゲーム内または Scene Explorer Mod で確認してください。事故、交通詰まり、停留所位置の悪さ、不具合のある停留所を探します。" },
                { TransitWaitStatus.KeyReportHintSkippedCims, "スキップされた単独シム: 交通機関を発車させるためにスキップした遅れ乗客です。後の状態は通常 'has path' または 'assigned' になるはずです。'no path yet' のままなら、少し時間を置いてそのシムEntityを確認してください。" },
                { TransitWaitStatus.KeyReportHintLateGroups, "遅れたグループ: バニラに任せている家族/グループです。数が多い場合、将来の安全なグループ移動対応の手がかりになります。" },
                { TransitWaitStatus.KeyReportFamilyHeader, "{0}" },
                { TransitWaitStatus.KeyReportServedStops, "対象停留所: {0}" },
                { TransitWaitStatus.KeyReportStopsWithWaiting, "待機乗客がいる停留所: {0}" },
                { TransitWaitStatus.KeyReportWaitingPassengers, "待機乗客: {0}" },
                { TransitWaitStatus.KeyReportAverageWait, "平均待ち時間: {0}" },
                { TransitWaitStatus.KeyReportLateBoardersSkipped, "今日スキップされた遅れ乗客: {0}" },
                { TransitWaitStatus.KeyReportWorstStopNone, "最悪停留所: 現在待機乗客はいません。" },
                { TransitWaitStatus.KeyReportWorstStopAverageWait, "最悪停留所の平均待ち時間: {0}" },
                { TransitWaitStatus.KeyReportWorstStopName, "最悪停留所名: {0}" },
                { TransitWaitStatus.KeyReportWorstStopEntity, "最悪停留所Entity: {0}" },
                { TransitWaitStatus.KeyReportWorstWaypointEntity, "最悪ウェイポイントEntity: {0}" },
                { TransitWaitStatus.KeyReportWorstLineHint, "最悪路線ヒント: {0}" },
                { TransitWaitStatus.KeyReportWorstLineEntity, "最悪路線Entity: {0}" },
                { TransitWaitStatus.KeyReportWorstLineWaypointAverage, "最悪路線ウェイポイント平均: {0}、待機 {1}" },
                { TransitWaitStatus.KeyReportTopWorstStopsHeader, "平均待ち時間が最悪の停留所トップ {0}:" },
                { TransitWaitStatus.KeyReportTopWorstStopLine, "{0}. {1} | 平均 {2} | 待機 {3} | 停留所 {4} | ウェイポイント {5} | 路線 {6} | ヒント {7}" },
                { TransitWaitStatus.KeyReportLateGroups, "残した遅れグループ乗客: {0}人、{1}グループ、{2}台の車両" },
                { TransitWaitStatus.KeyReportLastSkippedSamplesHeader, "スキップされた単独シム例" },
                { TransitWaitStatus.KeyReportLastSkippedSampleLine, "{0}. {1} | 乗客 {2} | 車両 {3} | フレーム {4} | 時刻 {5} | 現在 {6}" },
                { TransitWaitStatus.KeyReportNone, "なし" },
                { TransitWaitStatus.KeyReportUnknown, "(不明)" },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "このModの表示名。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "バージョン" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "現在のModバージョン。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "作者の Paradox Mods ページを開きます。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableVerboseLogging)), "詳細ログを有効化" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableVerboseLogging)),
                    "**デバッグ / テスト専用**\n" +
                    "都市実行中に <FastBoarding.log> へライブ診断を追加します。\n" +
                    "**通常プレイでは有効にしないでください。**\n" +
                    "オンのままだとパフォーマンスが低下し、巨大なログファイルが作られる可能性があります。\n" +
                    "古いログファイルは後で削除できます。"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
