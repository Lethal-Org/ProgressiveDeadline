using ProgressiveDeadlineMod.Patches;
using HarmonyLib;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace ProgressiveDeadlineMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class ProgressiveDeadlineMod : BaseUnityPlugin
    {
        private const string modGUID = "LethalOrg.ProgressiveDeadline";
        private const string modName = "Progressive Deadline";
        private const string modVersion = "1.0.1";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static ProgressiveDeadlineMod Instance;

        static internal ConfigEntry<float> minScrapValuePerDay;

        static internal ConfigEntry<float> incrementalDailyValue;

        static internal ConfigEntry<float> setMinimumDays;

        static internal ConfigEntry<float> setMaximumDays;

        internal ManualLogSource mls;

        internal void Awake() {

            if (Instance == null) {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modName);

            mls.LogInfo("Progressive deadline started");

            setMinimumDays = Config.Bind(
				"Customizable Values", "Minimum Deadline", 2f,
				"This is the minimum deadline you will have."
			);

            setMaximumDays = Config.Bind(
				"Customizable Values", "Maximum Deadline", float.MaxValue,
				"This is the maximum deadline you will have."
			);

            minScrapValuePerDay = Config.Bind(
				"Customizable Values", "Minimum Daily ScrapValue", 200f,
				"Minimum scrap value you can achieve per day. This will ignore the calculation for daily scrap if it's below this number."
			);

            incrementalDailyValue = Config.Bind(
				"Customizable Values", "Incremental Daily Value", 100f,
				"This is the amount the minimum scrap value will increase every time a quota is complete."
			);

            harmony.PatchAll(typeof(ProgressiveDeadlineMod));
            harmony.PatchAll(typeof(ProfitQuotaPatch));
            harmony.PatchAll(typeof(BuyingRatePatch));
            harmony.PatchAll(typeof(QuotaSettingsEndRoundPatch));
            harmony.PatchAll(typeof(QuotaSettingsPatch));
        }
    }
}
