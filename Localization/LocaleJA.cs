// File: Localization/LocaleJA.cs
// Purpose: Japanese ja-JP locale entries for Fast Boarding.

namespace FastBoarding
{
    using Colossal;
    using System.Collections.Generic;

    /// <summary>
    /// English localization source.
    /// </summary>
    public sealed class LocaleJA : IDictionarySource
    {
        private readonly Setting m_Setting;

        /// <summary>
        /// Constructs the English locale.
        /// </summary>
        /// <param name="setting">Settings object used for locale IDs.</param>
        public LocaleJA(Setting setting)
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

            const string ToggleName = "遅れた乗客をスキップ";

            // One helper keeps all seven status tooltips in sync for future translations.
            string StatusDescription(string transitName)
            {
                return
                    $"<現在の{transitName}状態>\n" +
                    "**待機** = 今この瞬間に待っている乗客の合計です。\n" +
                    "**平均** = その乗客たちの平均待ち時間です。\n" +
                    "**最悪** 停留所 = 1つの停留所で平均待ち時間が最も高い場所です。\n" +
                    "最悪の停留所は、事故、詰まった/バグった停留所、近くで足止めされた車両を調べるのに向いています。\n" +
                    $"**スキップ** = 今日 <{ToggleName}> でスキップされた遅れた単独乗客です。\n" +
                    "<Stats to Log> を使うと、停留所名、Entity ID などを含む詳細レポートを出せます。";
            }

            return new Dictionary<string, string>
            {
                // Options mod name
                { m_Setting.GetSettingsLocaleID(), title },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "アクション" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "情報" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.SpeedGroup), "乗車速度" },
                { m_Setting.GetOptionGroupLocaleID(Setting.BehaviorGroup), "動作" },
                { m_Setting.GetOptionGroupLocaleID(Setting.StatusGroup), "状態" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGroup), "Mod 情報" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGroup), "リンク" },
                { m_Setting.GetOptionGroupLocaleID(Setting.DebugGroup), "デバッグ" },

                // Boarding speed sliders
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BusBoardingSpeedFactor)), "バス乗車速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BusBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "値を上げると、バス停での乗車/積み込み時間が短くなります。\n" +
                    "通常の列は早くさばけますが、原版の仕組みでは遅れた乗客がまだ発車を遅らせることがあります。\n" +
                    $"[✓] <{ToggleName}> を使うと、バスが遅いCimを待ち続けにくくなります。\n" +
                    "2x はだいたい2倍の乗車速度です。\n" +
                    "技術メモ: 積み込み値が高いほど予定停車時間は短くなり、boarding time は乗客側の待機/乗車見積りに近い値です。\n" +
                    $"これは <{ToggleName}> とは別です。このチェックは、発車時刻後に遅いCimがその車両に乗り遅れてよいかを決めます。\n" +
                    "<==========================>\n" +
                    "全交通共通の積み込み値:\n" +
                    "1x  = 原版の停車時間 100%\n" +
                    "2x  = 予定停車時間 ~ 1/2\n" +
                    "4x  = 予定停車時間 ~ 1/4\n" +
                    "10x = 予定停車時間 ~ 1/10"

                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RailBoardingSpeedFactor)), "鉄道乗車速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RailBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "列車、路面電車、地下鉄に適用されます。\n" +
                    "値を上げると、鉄道系の停留所での乗車/積み込み時間が短くなります。\n" +
                    "通常の列は早くさばけますが、原版の仕組みでは遅れた乗客がまだ発車を遅らせることがあります。\n" +
                    $"[✓] <{ToggleName}> を使うと、発車時刻後に遅いCimがその車両を逃すようにできます。\n" +
                    "その後、ゲームがたいてい自然に再割り当てします。\n" +
                    "2x はだいたい2倍の乗車速度です。"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WaterBoardingSpeedFactor)), "船 + フェリー速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WaterBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "客船とフェリーに適用されます。\n" +
                    "値を上げると、水上交通の停留所での乗車/積み込み時間が短くなります。\n" +
                    "通常の列は早くさばけますが、原版の仕組みでは遅れた乗客がまだ発車を遅らせることがあります。\n" +
                    $"[✓] <{ToggleName}> を使うと、発車時刻後に遅いCimがその車両を逃すようにできます。\n" +
                    "2x はだいたい2倍の乗車速度です。"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AirBoardingSpeedFactor)), "飛行機乗車速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AirBoardingSpeedFactor)),
                    "<1x = vanilla>\n" +
                    "旅客航空ターミナルに適用されます。\n" +
                    "値を上げると、空港での乗車/積み込み時間が短くなります。\n" +
                    "通常の列は早くさばけますが、原版の仕組みでは遅れた乗客がまだ発車を遅らせることがあります。\n" +
                    $"[✓] <{ToggleName}> を使うと、発車時刻後に遅いCimがその車両を逃すようにできます。\n" +
                    "2x はだいたい2倍の乗車速度です。"
                },

                // Late passenger behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CancelLateBoarders)), ToggleName },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CancelLateBoarders)),
                    "発車時刻を過ぎてもまだ <準備完了ではない> 乗客は、その車両に乗り遅れてもよくなります。\n" +
                    "注意: 今のところ、遅れている単独の市民だけをスキップします。\n" +
                    "一緒に移動するグループ/家族で遅れている場合は <スキップされず>、原版と同じように遅延の原因になることがあります。\n" +
                    "グループは全体の中では少数で、主な効果は遅れた単独Cimをスキップすることから来ます。\n" +
                    "スキップされた市民は削除されません。ゲームがその後自然に再割り当てします。"
                },

                // Status overview
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusOverview)), "総利用量" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusOverview)),
                    "ゲームの交通インフォビューにある月間公共交通利用数です。\n" +
                    "更新時刻は、この状態スナップショットを取った時刻です（通常はオプションを開いた時）。"
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
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusShip)), "客船" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusShip)), StatusDescription("客船") },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusAir)), "飛行機" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusAir)), StatusDescription("飛行機") },

                // Status buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatsToLog)), "Stats to Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatsToLog)),
                    "**FastBoarding.log** に詳細レポートを1回だけ書き込みます。\n" +
                    "待機人数、各モードの最悪停留所トップ3、スキップされたCim例、Entity ID、路線ヒントを含みます。"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "ログを開く" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "**FastBoarding.log** があれば開きます。\n" +
                    "まだ無ければ Logs フォルダを開きます。"
                },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "この Mod の表示名です。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "バージョン" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "現在の Mod バージョンです。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "作者の Paradox Mods ページを開きます。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableVerboseLogging)), "詳細ログを有効化" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableVerboseLogging)),
                    "**デバッグ / テスト専用**\n" +
                    "都市の実行中、<Logs/FastBoarding.log> に <live> 詳細を追加します。\n" +
                    "**通常プレイでは有効にしないでください。**\n" +
                    "有効のままだとパフォーマンス低下や巨大なログの原因になります。\n" +
                    "古いログはあとで削除できます。\n" +
                    "注意: <Stats to Log> はその瞬間のスナップショットだけです。\n" +
                    "時間の流れを見たいなら、詳細ログを15〜30分ほど動かしてください。\n" +
                    "通常プレイ前に **オフ** へ戻すのを忘れないでください。"

                },

                // Runtime status strings
                { TransitWaitStatus.KeyStatusNotLoaded, "状態が読み込まれていません。" },
                { TransitWaitStatus.KeyNoCityLoaded, "都市が読み込まれていません。" },
                { TransitWaitStatus.KeyNoStopsFound, "停留所が見つかりません。" },
                { TransitWaitStatus.KeyStatusLine, "{0} 待機 | 平均 {1} | 最悪 {2} | {3} スキップ" },
                { TransitWaitStatus.KeyStatusOverviewLine, "{0} 観光客/月 | {1} 市民/月 | 更新 {2}" },

                // Stats-to-log report strings
                { TransitWaitStatus.KeyReportNoCityLoaded, "[FB] レポートを要求しましたが、都市が読み込まれていません。" },
                { TransitWaitStatus.KeyReportTitle, "Stats to Log スナップショット - Fast Boarding" },
                { TransitWaitStatus.KeyReportSettings, "設定: {0}" },
                { TransitWaitStatus.KeyReportNote, "路線ヒントは、その停留所で待機が最も多いウェイポイントから取られます。" },
                { TransitWaitStatus.KeyReportTesterHintsHeader, "テスター向けヒント" },
                { TransitWaitStatus.KeyReportHintWorstStops, "最悪の停留所: まずゲーム内か Scene Explorer Mod で確認してください。事故、渋滞、停留所の置き方、バグった停留所を見ます。" },
                { TransitWaitStatus.KeyReportHintSkippedCims, "スキップされた単独Cim: 交通を出発させるためにスキップした遅れた乗客です。あとで状態はたいてい 'has path' か 'assigned' になります。ずっと 'no path yet' のままなら、時間をおいてその Entity を見直してください。" },
                { TransitWaitStatus.KeyReportHintLateGroups, "遅れたグループ: 原版に任せている家族/グループです。数が多いなら、将来の安全なグループ対応のヒントになります。" },
                { TransitWaitStatus.KeyReportFamilyHeader, "{0}" },
                { TransitWaitStatus.KeyReportServedStops, "運行中の停留所: {0}" },
                { TransitWaitStatus.KeyReportStopsWithWaiting, "待機乗客がいる停留所: {0}" },
                { TransitWaitStatus.KeyReportWaitingPassengers, "待機乗客: {0}" },
                { TransitWaitStatus.KeyReportAverageWait, "平均待ち時間: {0}" },
                { TransitWaitStatus.KeyReportLateBoardersSkipped, "今日スキップした遅れた乗客: {0}" },
                { TransitWaitStatus.KeyReportWorstStopNone, "最悪の停留所: 現在待機乗客はいません。" },
                { TransitWaitStatus.KeyReportWorstStopAverageWait, "最悪停留所の平均待ち: {0}" },
                { TransitWaitStatus.KeyReportWorstStopName, "最悪停留所の名前: {0}" },
                { TransitWaitStatus.KeyReportWorstStopEntity, "最悪停留所 Entity: {0}" },
                { TransitWaitStatus.KeyReportWorstWaypointEntity, "ウェイポイント Entity: {0}" },
                { TransitWaitStatus.KeyReportWorstLineHint, "路線ヒント: {0}" },
                { TransitWaitStatus.KeyReportWorstLineEntity, "路線 Entity: {0}" },
                { TransitWaitStatus.KeyReportWorstLineWaypointAverage, "路線ウェイポイント平均: {0} / 待機 {1}" },
                { TransitWaitStatus.KeyReportTopWorstStopsHeader, "平均待ち時間が最悪の停留所トップ {0}:" },
                { TransitWaitStatus.KeyReportTopWorstStopLine, "{0}. {1} | 平均 {2} | 待機 {3} | 停留所 {4} | ウェイポイント {5} | 路線 {6} | ヒント {7}" },
                { TransitWaitStatus.KeyReportLateGroups, "スキップしていない遅れグループ: {0} 人 / {1} グループ / {2} 車両" },
                { TransitWaitStatus.KeyReportLastSkippedSamplesHeader, "この時点でスキップされた遅れた単独Cimの例" },
                { TransitWaitStatus.KeyReportLastSkippedSampleLine, "{0}. {1} | 乗客 {2} | 逃した車両 {3} | 時刻 {4} | 今 {5}" },
                { TransitWaitStatus.KeyReportNone, "なし" },
                { TransitWaitStatus.KeyReportUnknown, "(不明)" },

            };
        }
        public void Unload()
        {
        }
    }
}
