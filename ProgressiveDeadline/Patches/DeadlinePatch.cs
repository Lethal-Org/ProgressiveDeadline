using HarmonyLib;
using UnityEngine;
using ProgressiveDeadlineMod;
using BepInEx.Logging;

namespace ProgressiveDeadlineMod.Patches {

	// Set starting deadline
	[HarmonyPatch(typeof(TimeOfDay))]
	[HarmonyPatch("Awake")]
	public static class QuotaSettingsPatch {
		[HarmonyPostfix]
		public static void SetStartingDeadline (TimeOfDay __instance) {
			if (__instance.quotaVariables == null || !RoundManager.Instance.NetworkManager.IsHost) 
				return;

			string currentSave = GameNetworkManager.Instance.currentSaveFileName;
			float minimumDays = ProgressiveDeadlineMod.minimumDays.Value;
			float deadlineAmount = ES3.Load("deadlineAmount", currentSave, minimumDays);

			__instance.timeUntilDeadline = Utils.getTimeUntilDeadline(__instance, deadlineAmount);
			__instance.SyncTimeClientRpc(__instance.globalTime, (int)__instance.timeUntilDeadline);

			// Hacks for testing the mod
			//__instance.quotaFulfilled = 1000;
			//__instance.quotaVariables.startingQuota = 0;
			//__instance.quotaVariables.deadlineDaysAmount = (int)deadlineAmount;
			//__instance.quotaVariables.startingCredits = 1000;
		}
	}

	[HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.ResetSavedGameValues))]
    public class ResetSavedValuesPatch {

        [HarmonyPostfix]
        public static void ResetSaves(GameNetworkManager __instance) {
			if (!RoundManager.Instance.NetworkManager.IsHost)
				return;

			TimeOfDay timeOfDay = Object.FindObjectOfType<TimeOfDay>();
            string currentSave = __instance.currentSaveFileName;

			float minimumDays = ProgressiveDeadlineMod.minimumDays.Value;
			float minDailyScrap = ProgressiveDeadlineMod.minDailyScrap.Value;

			timeOfDay.timeUntilDeadline = Utils.getTimeUntilDeadline(timeOfDay, minimumDays);
			timeOfDay.SyncTimeClientRpc(timeOfDay.globalTime, (int)timeOfDay.timeUntilDeadline);

			ES3.Save("deadlineAmount", minimumDays, currentSave);
			ES3.Save("previousDaily", minDailyScrap, currentSave);
        }
    }

	// Set new deadline
    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.SetNewProfitQuota))]
    public class ProfitQuotaPatch {

        [HarmonyPostfix]
        static void ProgressiveDeadline(TimeOfDay __instance) {

			// Return if not the host
            if (!RoundManager.Instance.NetworkManager.IsHost) {
                ProgressiveDeadlineMod.Instance.mls.LogInfo("You're not the host.");
                return;
            }

			// Read configuration values
			float minimumDays = ProgressiveDeadlineMod.minimumDays.Value;
            float maximumDays = ProgressiveDeadlineMod.maximumDays.Value;
			float minDailyScrap = ProgressiveDeadlineMod.minDailyScrap.Value;

			// Get some values from instance
            string currentSave = GameNetworkManager.Instance.currentSaveFileName;
			float totalTime = (float)__instance.totalTime;
            float runCount = __instance.timesFulfilledQuota;
			AnimationCurve randomizerCurve = __instance.quotaVariables.randomizerCurve;

			// Load and update daily scrap
			float dailyScrap = ES3.Load("previousDaily", currentSave, minDailyScrap);
			dailyScrap += Utils.dailyIncrease(runCount, randomizerCurve);

			// Calculate new Deadline
			float averageDays = Mathf.Ceil(__instance.profitQuota / dailyScrap);
			float newDeadline = Mathf.Clamp(averageDays, minimumDays, maximumDays);

			// Save values
			ES3.Save("previousDaily", dailyScrap, currentSave);
			ES3.Save("deadlineAmount", newDeadline, currentSave);

			// Update deadline and sync
			__instance.timeUntilDeadline = totalTime * newDeadline;
			TimeOfDay.Instance.SyncTimeClientRpc(__instance.globalTime, (int)__instance.timeUntilDeadline);
			ProgressiveDeadlineMod.Instance.mls.LogInfo($"You're host, new deadline: {newDeadline}");

			// HACK
			//__instance.quotaFulfilled = 1000;
        }
    }

	// Sync deadlineDaysAmount with client after 
    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.SyncTimeClientRpc))]
    public class DeadlineDaysAmountSyncPatch {

        [HarmonyPostfix]
		static void deadlineSync (TimeOfDay __instance) {

			int deadlineDaysAmount = Utils.getDeadlineDays(__instance);

			if (__instance.totalTime == 0){
				__instance.quotaVariables.deadlineDaysAmount = deadlineDaysAmount;
				return;
			}

			// Means that the quota is fullfilled
			if (deadlineDaysAmount > __instance.quotaVariables.deadlineDaysAmount){
				__instance.quotaVariables.deadlineDaysAmount = deadlineDaysAmount;
				StartOfRound.Instance.companyBuyingRate = Utils.buyingRate(__instance);
			}
		}
	}

	// Update buying rate
	[HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.SetBuyingRateForDay))]
	public class BuyingRatePatch {
		[HarmonyPostfix]
		public static void SetBuyingRate (TimeOfDay __instance) {
			StartOfRound.Instance.companyBuyingRate = Utils.buyingRate(__instance);
		}
	}
}
