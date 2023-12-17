using HarmonyLib;
using UnityEngine;
using ProgressiveDeadlineMod;
using BepInEx.Logging;

namespace ProgressiveDeadlineMod.Patches {

	// Update company buying rate when quota is complete
	// The game just sets it to 30% whenever you complete a quota (v45)
	[HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.SyncNewProfitQuotaClientRpc))]
	public static class QuotaSettingsEndRoundPatch {
		[HarmonyPostfix]
		public static void SetBuyingRateEndRound (TimeOfDay __instance)	{
			__instance.SetBuyingRateForDay();
		}
	}

	// Update company buying rate
	[HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.SetBuyingRateForDay))]
	public class BuyingRatePatch {
		[HarmonyPostfix]
		public static void SetBuyingRate (TimeOfDay __instance) {

			// Just return because game already calculated it being 100%
			if (__instance.daysUntilDeadline == 0)
				return;

			int deadlineDaysAmount = __instance.quotaVariables.deadlineDaysAmount;
			int daysUntilDeadline = __instance.daysUntilDeadline;

			float num = 1f / (float)deadlineDaysAmount;
			float num2 = (1f - num) / (float)deadlineDaysAmount;
			float buyingRate = num2 * (float)(deadlineDaysAmount - daysUntilDeadline) + num;

			StartOfRound.Instance.companyBuyingRate = buyingRate;
		}
	}
}
