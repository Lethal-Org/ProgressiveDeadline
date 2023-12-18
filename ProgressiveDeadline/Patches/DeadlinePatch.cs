using HarmonyLib;
using UnityEngine;
using ProgressiveDeadlineMod;
using BepInEx.Logging;

namespace ProgressiveDeadlineMod.Patches
{
	// Set starting deadline
	[HarmonyPatch(typeof(TimeOfDay))]
	[HarmonyPatch("Awake")]
	public static class QuotaSettingsPatch 
	{
		[HarmonyPostfix]
		public static void SetStartingQuota (TimeOfDay __instance)
		{
			if (__instance.quotaVariables == null)
			{
				return;
			}

			float minimumDays = ProgressiveDeadlineMod.setMinimumDays.Value;

			__instance.quotaVariables.startingQuota = 0;
			//__instance.quotaVariables.startingCredits = 1000;
			__instance.quotaVariables.deadlineDaysAmount = (int)minimumDays;
		}
	}

	// Set new deadline
    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.SetNewProfitQuota))]
    public class ProfitQuotaPatch
    {

        [HarmonyPostfix]
        static void ProgressiveDeadline(TimeOfDay __instance) {

			// Return if not the host
            if (!RoundManager.Instance.NetworkManager.IsHost) {
                ProgressiveDeadlineMod.Instance.mls.LogInfo("You're not the host.");
                return;
            }

			// Read configuration values
			float minimumDays = ProgressiveDeadlineMod.setMinimumDays.Value;
            float maximumDays = ProgressiveDeadlineMod.setMaximumDays.Value;
			float minScrapValuePerDay = ProgressiveDeadlineMod.minScrapValuePerDay.Value;
			float incrementalDailyValue = ProgressiveDeadlineMod.incrementalDailyValue.Value;
			QuotaSettings quotaVariables = __instance.quotaVariables;

			// Get some values from instance
			float totalTime = (float)__instance.totalTime;
            float runCount = __instance.timesFulfilledQuota;
			int profitQuota = __instance.profitQuota;

			// Get some values from quota settings
			AnimationCurve randomizerCurve = quotaVariables.randomizerCurve;
			float increaseSteepness = quotaVariables.increaseSteepness;

			// Load previous daily value
			if (!ES3.KeyExists("previousDaily"))
				ES3.Save("previousDaily", minScrapValuePerDay);
			minScrapValuePerDay = ES3.Load("previousDaily", minScrapValuePerDay);

			// Calculate the daily increase
			float random_point = randomizerCurve.Evaluate(Random.Range(0f, 1f)) + 1f;
			float dailyScrapIncrease = 1f + (float)runCount * ((float)runCount / increaseSteepness);
			dailyScrapIncrease = incrementalDailyValue * dailyScrapIncrease * (random_point);

			// Update daily scrap
			minScrapValuePerDay += dailyScrapIncrease;
			ES3.Save("previousDaily", minScrapValuePerDay);

			// Calculate new Deadline
			float averageDays = Mathf.Ceil(profitQuota / (minScrapValuePerDay));
			float newDeadline = Mathf.Clamp(averageDays, minimumDays, maximumDays);

			__instance.timeUntilDeadline = totalTime * newDeadline;

			ProgressiveDeadlineMod.Instance.mls.LogInfo($"You're host, new deadline: {newDeadline}");
			TimeOfDay.Instance.SyncTimeClientRpc(__instance.globalTime, (int)__instance.timeUntilDeadline);
        }
    }
}
