// File: Mod.cs
// Purpose: Entry point for Boarding Time.

namespace BoardingTime
{
    using Colossal;
    using Colossal.IO.AssetDatabase;
    using Colossal.Localization;
    using Colossal.Logging;
    using Game;
    using Game.Modding;
    using Game.SceneFlow;
    using Game.Simulation;
    using System;
    using System.Reflection;

    public sealed class Mod : IMod
    {
        public const string ModName = "Boarding Time";
        public const string ModId = "BoardingTime";
        public const string ModTag = "[BT]";

        public static readonly string ModVersion =
            Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0";

        public static readonly ILog s_Log =
            LogManager.GetLogger(ModId).SetShowsErrorsInUI(false);

        private static bool s_BannerLogged;

        public static Setting? Settings;

        public void OnLoad(UpdateSystem updateSystem)
        {
            if (!s_BannerLogged)
            {
                s_BannerLogged = true;
                LogUtils.Info(s_Log, () => $"{ModName} v{ModVersion} OnLoad");
            }

            GameManager gameManager = GameManager.instance;

            try
            {
                if (gameManager.modManager.TryGetExecutableAsset(this, out var asset))
                {
                    LogUtils.Info(s_Log, () => $"Current mod asset at {asset.path}");
                }
            }
            catch (Exception ex)
            {
                LogUtils.Warn(s_Log, () => $"TryGetExecutableAsset failed: {ex.GetType().Name}: {ex.Message}", ex);
            }

            Setting setting = new Setting(this);
            Settings = setting;
          

            // Register languages (uncomment as languages added)
            AddLocaleSource("en-US", new LocaleEN(setting));
            // AddLocaleSource("fr-FR", new LocaleFR(setting));
            // AddLocaleSource("es-ES", new LocaleES(setting));
            // AddLocaleSource("de-DE", new LocaleDE(setting));
            // AddLocaleSource("it-IT", new LocaleIT(setting));
            // AddLocaleSource("ja-JP", new LocaleJA(setting));
            // AddLocaleSource("ko-KR", new LocaleKO(setting));
            // AddLocaleSource("pl-PL", new LocalePL(setting));
            // AddLocaleSource("pt-BR", new LocalePT_BR(setting));
            // AddLocaleSource("zh-HANS", new LocaleZH_CN(setting));    // Simplified Chinese
           // AddLocaleSource("zh-HANT", new LocaleZH_HANT(setting));  // Traditional Chinese

            try
            {
                // CS2 persists ModSetting values in the mod .coc file.
                AssetDatabase.global.LoadSettings(ModId, setting, new Setting(this));
                setting.RegisterInOptionsUI();
                BoardingRuntimeSettings.Apply(setting);
            }
            catch (Exception ex)
            {
                LogUtils.Warn(s_Log, () => $"Settings/UI init failed: {ex.GetType().Name}: {ex.Message}", ex);
            }

            try
            {
                // Stop tuning is one-shot; late-boarder cancel runs only while the option is enabled.
                updateSystem.UpdateBefore<TransportStopTuningSystem, TransportStopSystem>(
                    SystemUpdatePhase.GameSimulation);
                updateSystem.UpdateAfter<LateBoarderCancelSystem, ResidentAISystem.Actions>(
                    SystemUpdatePhase.GameSimulation);
                updateSystem.UpdateBefore<LateBoarderCancelSystem, HumanMoveSystem>(
                    SystemUpdatePhase.GameSimulation);

                updateSystem.World.GetOrCreateSystemManaged<TransportStopTuningSystem>().Enabled = true;
                updateSystem.World.GetOrCreateSystemManaged<LateBoarderCancelSystem>().Enabled =
                    BoardingRuntimeSettings.CancelLateBoarders;
            }
            catch (Exception ex)
            {
                LogUtils.Warn(s_Log, () => $"System scheduling failed: {ex.GetType().Name}: {ex.Message}", ex);
            }
        }

        public void OnDispose()
        {
            LogUtils.Info(s_Log, () => nameof(OnDispose));

            if (Settings != null)
            {
                try
                {
                    Settings.UnregisterInOptionsUI();
                }
                catch (Exception ex)
                {
                    LogUtils.Warn(s_Log, () => $"UnregisterInOptionsUI failed: {ex.GetType().Name}: {ex.Message}", ex);
                }

                Settings = null;
            }
        }

        internal static void WarnOnce(string key, Func<string> messageFactory)
        {
            LogUtils.WarnOnce(s_Log, key, messageFactory);
        }

        private static void AddLocaleSource(string localeId, IDictionarySource source)
        {
            if (string.IsNullOrEmpty(localeId))
            {
                return;
            }

            LocalizationManager? localizationManager = GameManager.instance.localizationManager;
            if (localizationManager == null)
            {
                LogUtils.Warn(s_Log, () => $"AddLocaleSource: No LocalizationManager; cannot add source for '{localeId}'.");
                return;
            }

            try
            {
                localizationManager.AddSource(localeId, source);
            }
            catch (Exception ex)
            {
                LogUtils.Warn(s_Log, () => $"AddLocaleSource: AddSource for '{localeId}' failed: {ex.GetType().Name}: {ex.Message}", ex);
            }
        }
    }
}
