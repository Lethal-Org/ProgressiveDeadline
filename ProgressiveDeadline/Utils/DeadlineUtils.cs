using ProgressiveDeadlineMod;
using UnityEngine;
using BepInEx.Logging;

namespace ProgressiveDeadlineMod {

	public class Utils {

		// Returns the current daily increase
		public static float dailyIncrease(float runCount, AnimationCurve randomizerCurve) {
			float minScrapIncrease = ProgressiveDeadlineMod.minScrapIncrease.Value;
			float minScrapIncreaseSteepness = ProgressiveDeadlineMod.minScrapIncreaseSteepness.Value;
			bool linear = ProgressiveDeadlineMod.useLinearAlgorithm.Value;

			// If using linear increase, just return increase value
			if (linear == true)
				return minScrapIncrease;

			float random_point = randomizerCurve.Evaluate(Random.Range(0f, 1f)) + 1f;
			float increase = 1f + runCount * (runCount / minScrapIncreaseSteepness);
			increase = minScrapIncrease * increase * random_point;

			return increase;
		}

		// Returns the current company buying rate
		public static float buyingRate(TimeOfDay timeOfDay) {

			int total_deadline = timeOfDay.quotaVariables.deadlineDaysAmount;
			int remaining_deadline = getDeadlineDays(timeOfDay);
			
			ProgressiveDeadlineMod.Instance.mls.LogInfo($"AQ {total_deadline} {remaining_deadline}");

			if (remaining_deadline == 0)
				return 1f;

			float num = 1f / (float)total_deadline;
			float num2 = (1f - num) / (float)total_deadline;
			float buyingRate = num2 * (float)(total_deadline - remaining_deadline) + num;

			return buyingRate;
		}

		// Returns the length in hours of a in-game day
		// Useful bc it's only set after a while (even tho is a const)
		public static float getTotalTime(TimeOfDay timeOfDay) {
			float totalTime = timeOfDay.totalTime;

			if (totalTime == 0)
				totalTime = timeOfDay.lengthOfHours * (float)timeOfDay.numberOfHours;

			return totalTime;
		}

		// Returns TIME in in-game hours until deadline
		public static float getTimeUntilDeadline(TimeOfDay timeOfDay, float days) {
			return days * getTotalTime(timeOfDay);
		}

		// Returns amount of days until deadline
		public static int getDeadlineDays(TimeOfDay timeOfDay) {
			return (int)(timeOfDay.timeUntilDeadline / getTotalTime(timeOfDay));
		}
	}

}
