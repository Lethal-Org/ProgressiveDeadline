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
        private const string modVersion = "2.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static ProgressiveDeadlineMod Instance;

        static internal ConfigEntry<float> minimumDays;

        static internal ConfigEntry<float> maximumDays;

        static internal ConfigEntry<float> minDailyScrap;

		static internal ConfigEntry<bool> useLinearAlgorithm;

        static internal ConfigEntry<float> minScrapIncrease;

		static internal ConfigEntry<float> minScrapIncreaseSteepness;

        internal ManualLogSource mls;

        internal void Awake() {

            if (Instance == null)
                Instance = this;

            mls = BepInEx.Logging.Logger.CreateLogSource(modName);

            mls.LogInfo("Progressive deadline started");

            minimumDays = Config.Bind(
				"Customizable Values", "Minimum Deadline", 2f,
				"This is the minimum deadline you will have."
			);

            maximumDays = Config.Bind(
				"Customizable Values", "Maximum Deadline", float.MaxValue,
				"This is the maximum deadline you will have."
			);

			minDailyScrap = Config.Bind(
				"Customizable Values", "Minimum Daily ScrapValue", 100f,
				"Minimum scrap value you can achieve per day. This will ignore the calculation for daily scrap if it's below this number. (Default: 100f)"
			);

            minScrapIncrease = Config.Bind(
				"Customizable Values", "Base Minimum Scrap Value Increase", 30f,
				"This is the minimum amount the minimum scrap value will increase every time a quota is complete. (Default: 30f)"
			);

            minScrapIncreaseSteepness = Config.Bind(
				"Customizable Values", "Incremental Daily Value Steepness", 200f,
				"Defines the Steepness of the minimum incremental daily value scalling with each completed quota. (Default: 200f)"
			);

            useLinearAlgorithm = Config.Bind(
				"Use linear calculations for minimum daily scrap", "Static Base Minimum Scrap", false,
				"If set to true, the 'Base Minimum Scrap Value Increase' will remain fixed and will not scale based on 'Incremental Daily Value Steepness' and quota completion. (Default: false)"
			);

			harmony.PatchAll();
        }
    }
}
