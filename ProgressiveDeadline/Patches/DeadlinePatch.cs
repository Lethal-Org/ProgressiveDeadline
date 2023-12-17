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
			__instance.quotaVariables.startingCredits = 1000;
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
                ProgressiveDeadlineMod.Instance.mls.LogInfo("This person is not the host. Will not change deadline or send rpc.");
                return;
            }

			// Read configuration values
			float minimumDays = ProgressiveDeadlineMod.setMinimumDays.Value;
            float maximumDays = ProgressiveDeadlineMod.setMaximumDays.Value;
			float minScrapValuePerDay = ProgressiveDeadlineMod.minScrapValuePerDay.Value;
			float incrementalDailyValue = ProgressiveDeadlineMod.incrementalDailyValue.Value;

			// Get some values from instance
			float totalTime = (float)__instance.totalTime;
            float runCount = __instance.timesFulfilledQuota;
			int profitQuota = __instance.profitQuota;

			// Calculate new Deadline
			float dailyScrapIncrease = (runCount * incrementalDailyValue);
			float averageDays = Mathf.Ceil(profitQuota / (minScrapValuePerDay + dailyScrapIncrease));
			float newDeadline = Mathf.Clamp(averageDays, minimumDays, maximumDays);

			__instance.timeUntilDeadline = totalTime * newDeadline;

			ProgressiveDeadlineMod.Instance.mls.LogInfo($"Player is host, new deadline: {newDeadline}");
			TimeOfDay.Instance.SyncTimeClientRpc(__instance.globalTime, (int)__instance.timeUntilDeadline);
        }
    }
}

