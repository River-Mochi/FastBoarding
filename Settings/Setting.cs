// File: Settings/Setting.cs
// Purpose: Options UI settings for Boarding Time.

namespace BoardingTime
{
    using Colossal.IO.AssetDatabase;
    using Game;
    using Game.Modding;
    using Game.SceneFlow;
    using Game.Settings;
    using Game.UI;
    using System;
    using Unity.Entities;
    using UnityEngine;

    [FileLocation("ModsSettings/BoardingTime/BoardingTime")]
    [SettingsUITabOrder(ActionsTab, AboutTab)]
    [SettingsUIGroupOrder(SpeedGroup, BehaviorGroup, StatusGroup, AboutInfoGroup, AboutLinksGroup)]
    [SettingsUIShowGroupName(SpeedGroup, BehaviorGroup, StatusGroup, AboutLinksGroup)]
    public sealed class Setting : ModSetting
    {
        public const string ActionsTab = "Actions";
        public const string AboutTab = "About";

        public const string SpeedGroup = "BoardingSpeed";
        public const string BehaviorGroup = "Behavior";
        public const string StatusGroup = "Status";
        public const string StatusButtonsRow = "StatusButtonsRow";
        public const string AboutInfoGroup = "ModInfo";
        public const string AboutLinksGroup = "Links";

        private const string UrlParadox =
            "https://mods.paradoxplaza.com/authors/River-mochi/cities_skylines_2?games=cities_skylines_2&orderBy=desc&sortBy=best&time=alltime";

        public const int DefaultSpeedFactor = 1;
        public const int MinSpeedFactor = 1;
        public const int MaxSpeedFactor = 10;
        public const int SpeedStepFactor = 1;

        public Setting(IMod mod)
            : base(mod)
        {
            SetDefaults();
        }

        [SettingsUISlider(
            min = MinSpeedFactor,
            max = MaxSpeedFactor,
            step = SpeedStepFactor)]
        [SettingsUISection(ActionsTab, SpeedGroup)]
        [SettingsUISetter(typeof(Setting), nameof(SetBusBoardingSpeedFactorLive))]
        public int BusBoardingSpeedFactor { get; set; }

        [SettingsUISlider(
            min = MinSpeedFactor,
            max = MaxSpeedFactor,
            step = SpeedStepFactor)]
        [SettingsUISection(ActionsTab, SpeedGroup)]
        [SettingsUISetter(typeof(Setting), nameof(SetRailBoardingSpeedFactorLive))]
        public int RailBoardingSpeedFactor { get; set; }

        [SettingsUISlider(
            min = MinSpeedFactor,
            max = MaxSpeedFactor,
            step = SpeedStepFactor)]
        [SettingsUISection(ActionsTab, SpeedGroup)]
        [SettingsUISetter(typeof(Setting), nameof(SetWaterBoardingSpeedFactorLive))]
        public int WaterBoardingSpeedFactor { get; set; }

        [SettingsUISlider(
            min = MinSpeedFactor,
            max = MaxSpeedFactor,
            step = SpeedStepFactor)]
        [SettingsUISection(ActionsTab, SpeedGroup)]
        [SettingsUISetter(typeof(Setting), nameof(SetAirBoardingSpeedFactorLive))]
        public int AirBoardingSpeedFactor { get; set; }

        [SettingsUISection(ActionsTab, BehaviorGroup)]
        [SettingsUISetter(typeof(Setting), nameof(SetCancelLateBoardersLive))]
        public bool CancelLateBoarders { get; set; }

        [SettingsUISection(ActionsTab, StatusGroup)]
        public string StatusBus
        {
            get
            {
                // Options UI polls status rows separately; the cache prevents duplicate work.
                try { TransitWaitStatus.RefreshIfNeeded(); } catch { }
                return TransitWaitStatus.BusSummary ?? string.Empty;
            }
        }

        [SettingsUISection(ActionsTab, StatusGroup)]
        public string StatusTram
        {
            get
            {
                try { TransitWaitStatus.RefreshIfNeeded(); } catch { }
                return TransitWaitStatus.TramSummary ?? string.Empty;
            }
        }

        [SettingsUISection(ActionsTab, StatusGroup)]
        public string StatusTrain
        {
            get
            {
                try { TransitWaitStatus.RefreshIfNeeded(); } catch { }
                return TransitWaitStatus.TrainSummary ?? string.Empty;
            }
        }

        [SettingsUISection(ActionsTab, StatusGroup)]
        public string StatusSubway
        {
            get
            {
                try { TransitWaitStatus.RefreshIfNeeded(); } catch { }
                return TransitWaitStatus.SubwaySummary ?? string.Empty;
            }
        }

        [SettingsUISection(ActionsTab, StatusGroup)]
        public string StatusFerry
        {
            get
            {
                try { TransitWaitStatus.RefreshIfNeeded(); } catch { }
                return TransitWaitStatus.FerrySummary ?? string.Empty;
            }
        }

        [SettingsUISection(ActionsTab, StatusGroup)]
        public string StatusShip
        {
            get
            {
                try { TransitWaitStatus.RefreshIfNeeded(); } catch { }
                return TransitWaitStatus.ShipSummary ?? string.Empty;
            }
        }

        [SettingsUISection(ActionsTab, StatusGroup)]
        public string StatusAir
        {
            get
            {
                try { TransitWaitStatus.RefreshIfNeeded(); } catch { }
                return TransitWaitStatus.AirSummary ?? string.Empty;
            }
        }

        [SettingsUISection(ActionsTab, StatusGroup)]
        [SettingsUIButtonGroup(StatusButtonsRow)]
        [SettingsUIButton]
        public bool StatsToLog
        {
            set
            {
                if (!value)
                {
                    return;
                }

                // Detailed report belongs in the log so the UI rows can stay compact.
                TransitWaitStatus.LogDetailedReport();
            }
        }

        [SettingsUISection(ActionsTab, StatusGroup)]
        [SettingsUIButtonGroup(StatusButtonsRow)]
        [SettingsUIButton]
        public bool OpenLog
        {
            set
            {
                if (!value)
                {
                    return;
                }

                // Open the exact mod log when possible; otherwise open the Logs folder.
                ShellOpen.OpenModLogOrLogsFolder();
            }
        }

        [SettingsUISection(AboutTab, AboutInfoGroup)]
        public string AboutName => Mod.ModName;

        [SettingsUISection(AboutTab, AboutInfoGroup)]
        public string AboutVersion => Mod.ModVersion;

        [SettingsUISection(AboutTab, AboutLinksGroup)]
        [SettingsUIButtonGroup(AboutLinksGroup)]
        [SettingsUIButton]
        public bool OpenParadoxMods
        {
            set
            {
                if (!value)
                {
                    return;
                }

                try
                {
                    // External links use Unity's URL opener; no filesystem fallback needed here.
                    Application.OpenURL(UrlParadox);
                }
                catch (Exception ex)
                {
                    LogUtils.Info(Mod.s_Log, () => $"{Mod.ModTag} OpenParadoxMods failed: {ex.GetType().Name}: {ex.Message}");
                }
            }
        }

        public override void SetDefaults()
        {
            BusBoardingSpeedFactor = DefaultSpeedFactor;
            RailBoardingSpeedFactor = DefaultSpeedFactor;
            WaterBoardingSpeedFactor = DefaultSpeedFactor;
            AirBoardingSpeedFactor = DefaultSpeedFactor;
            CancelLateBoarders = false;
        }

        public override void Apply()
        {
            RepairAndClamp();

            base.Apply();

            // Apply() is still part of the normal CS2 auto-save path.
            // It keeps load/reset/clamp behavior aligned with the runtime snapshot.
            BoardingRuntimeChangeFlags changes = BoardingRuntimeSettings.Apply(this);

            if ((changes & BoardingRuntimeChangeFlags.StopTuning) != 0)
            {
                LogUtils.Info(Mod.s_Log, () => $"Settings applied. {BoardingRuntimeSettings.DescribeForLog()}");
                TryEnableStopTuningSystem();
            }

            if ((changes & BoardingRuntimeChangeFlags.LateBoarders) != 0)
            {
                LogUtils.Info(Mod.s_Log, () => $"Late-boarder setting applied. {BoardingRuntimeSettings.DescribeForLog()}");
                TrySetLateBoarderSystemEnabled(CancelLateBoarders);
            }
        }

        private void SetBusBoardingSpeedFactorLive(int value)
        {
            if (BoardingRuntimeSettings.SetBusBoardingSpeedFactor(ClampSpeedFactor(value)))
            {
                // Live setters let the relevant system react immediately instead of waking every system.
                LogSpeedChange();
                TryEnableStopTuningSystem();
            }
        }

        private void SetRailBoardingSpeedFactorLive(int value)
        {
            if (BoardingRuntimeSettings.SetRailBoardingSpeedFactor(ClampSpeedFactor(value)))
            {
                LogSpeedChange();
                TryEnableStopTuningSystem();
            }
        }

        private void SetWaterBoardingSpeedFactorLive(int value)
        {
            if (BoardingRuntimeSettings.SetWaterBoardingSpeedFactor(ClampSpeedFactor(value)))
            {
                LogSpeedChange();
                TryEnableStopTuningSystem();
            }
        }

        private void SetAirBoardingSpeedFactorLive(int value)
        {
            if (BoardingRuntimeSettings.SetAirBoardingSpeedFactor(ClampSpeedFactor(value)))
            {
                LogSpeedChange();
                TryEnableStopTuningSystem();
            }
        }

        private void SetCancelLateBoardersLive(bool value)
        {
            if (BoardingRuntimeSettings.SetCancelLateBoarders(value))
            {
                LogUtils.Info(
                    Mod.s_Log,
                    () => $"Cancel late boarders {(value ? "enabled" : "disabled")} from Options UI. {BoardingRuntimeSettings.DescribeForLog()}");
                TrySetLateBoarderSystemEnabled(value);
            }
        }

        private static void LogSpeedChange()
        {
            LogUtils.Info(Mod.s_Log, () => $"Boarding speed changed from Options UI. {BoardingRuntimeSettings.DescribeForLog()}");
        }

        private void RepairAndClamp()
        {
            BusBoardingSpeedFactor = ClampSpeedFactor(BusBoardingSpeedFactor);
            RailBoardingSpeedFactor = ClampSpeedFactor(RailBoardingSpeedFactor);
            WaterBoardingSpeedFactor = ClampSpeedFactor(WaterBoardingSpeedFactor);
            AirBoardingSpeedFactor = ClampSpeedFactor(AirBoardingSpeedFactor);
        }

        private static int ClampSpeedFactor(int value)
        {
            if (value < MinSpeedFactor)
            {
                return MinSpeedFactor;
            }

            if (value > MaxSpeedFactor)
            {
                return MaxSpeedFactor;
            }

            return value;
        }

        private static void TryEnableStopTuningSystem()
        {
            if (!TryGetLoadedWorld(out var world))
            {
                return;
            }

            try
            {
                // SettingsUISetter can fire while in-game, so wake only the relevant one-shot system.
                TransportStopTuningSystem system =
                    world.GetExistingSystemManaged<TransportStopTuningSystem>() ??
                    world.GetOrCreateSystemManaged<TransportStopTuningSystem>();

                // The stop tuning system does one pass, then disables itself again.
                system.Enabled = true;
            }
            catch (Exception ex)
            {
                LogUtils.Warn(Mod.s_Log, () => $"Failed enabling TransportStopTuningSystem: {ex.GetType().Name}: {ex.Message}", ex);
            }
        }

        private static void TrySetLateBoarderSystemEnabled(bool enabled)
        {
            if (!TryGetLoadedWorld(out var world))
            {
                return;
            }

            try
            {
                // The live system stays disabled unless the experimental toggle is on.
                LateBoarderCancelSystem system =
                    world.GetExistingSystemManaged<LateBoarderCancelSystem>() ??
                    world.GetOrCreateSystemManaged<LateBoarderCancelSystem>();

                system.Enabled = enabled;
            }
            catch (Exception ex)
            {
                LogUtils.Warn(Mod.s_Log, () => $"Failed updating LateBoarderCancelSystem state: {ex.GetType().Name}: {ex.Message}", ex);
            }
        }

        private static bool TryGetLoadedWorld(out World world)
        {
            world = World.DefaultGameObjectInjectionWorld;

            GameManager gameManager = GameManager.instance;
            if (!gameManager.gameMode.IsGame() || world == null)
            {
                return false;
            }

            return true;
        }
    }
}
