using Harmony;
using StardewModdingAPI;
using StardewValley;
using System;

namespace AngryGrandpa
{
	/// <summary>The class for patching methods on the StardewValley.Utility class.</summary>
	class UtilityPatches
	{
		/*********
        ** Accessors
        *********/
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ModConfig Config => ModConfig.Instance;
		private static HarmonyInstance Harmony => ModEntry.Instance.Harmony;


		/*********
        ** Public methods
        *********/
		/// <summary>
		/// Applies the harmony patches defined in this class.
		/// </summary>
		public static void Apply()
		{
			Harmony.Patch(
				original: AccessTools.Method(typeof(Utility),
					nameof(Utility.getGrandpaScore)),
				postfix: new HarmonyMethod(typeof(UtilityPatches),
					nameof(UtilityPatches.getGrandpaScore_Postfix))
			); 
			Harmony.Patch(
				original: AccessTools.Method(typeof(Utility),
					nameof(Utility.getGrandpaCandlesFromScore)),
				postfix: new HarmonyMethod(typeof(UtilityPatches),
					nameof(UtilityPatches.getGrandpaCandlesFromScore_Postfix))
			);
		}

		/// <summary>
		/// Alters the points scoring for grandpa's evaluation if config ScoringSystem: "Original"
		/// </summary>
		/// <param name="__result">The original result returned by Utility.getGrandpaScore</param>
		/// <returns>Integer result of raw points score</returns>
		public static int getGrandpaScore_Postfix(int __result)
		{
			try
			{
				// Only modify scoring if they wanted the original 13 points
				if (Config.ScoringSystem == "Original") 
				{
					int num = 0;
					if (Game1.player.totalMoneyEarned >= 100000U) // Earned 100K+
						++num;
					if (Game1.player.totalMoneyEarned >= 200000U) // Earned 200K+
						++num;
					if (Game1.player.totalMoneyEarned >= 300000U) // Earned 300K+
						++num;
					if (Game1.player.totalMoneyEarned >= 500000U) // Earned 500K+
						++num;
					if (Game1.player.totalMoneyEarned >= 1000000U) // Earned 1 million +
						++num;
					if (Utility.foundAllStardrops()) // Found all stardrops(!!!) Very difficult.
						++num;
					if (Game1.isLocationAccessible("CommunityCenter")) // All CC bundles are complete
						++num;
					if (Game1.player.isMarried() && Utility.getHomeOfFarmer(Game1.player).upgradeLevel >= 2) // Married with 2nd house upgrade
						++num;
					if (Game1.player.achievements.Contains(5)) // A Complete Collection (museum)
						++num;
					if (Game1.player.achievements.Contains(26)) // Master Angler (catch every fish)
						++num;
					if (Game1.player.achievements.Contains(34)) // Full Shipment (ship every item)
						++num;
					if (Utility.getNumberOfFriendsWithinThisRange(Game1.player, 1975, 999999, false) >= 10) // 8 hearts with 10+ villagers
						++num;
					if (Game1.player.Level >= 25) // Total 50 levels in skills (max all)
						++num;
					return num; // return revised score
				}
				// Vanilla game scoring below for comparison. No need to re-implement this.
				/**else 
				{
					int num1 = 0; // Initialize score
					if (Game1.player.totalMoneyEarned >= 50000U) // Earned 50K+
						++num1;
					if (Game1.player.totalMoneyEarned >= 100000U) // Earned 100K+
						++num1;
					if (Game1.player.totalMoneyEarned >= 200000U) // Earned 200K+
						++num1;
					if (Game1.player.totalMoneyEarned >= 300000U) // Earned 300K+
						++num1;
					if (Game1.player.totalMoneyEarned >= 500000U) // Earned 500K+
						++num1;
					if (Game1.player.totalMoneyEarned >= 1000000U) // Earned 1 million +
						num1 += 2;
					if (Game1.player.achievements.Contains(5)) // A Complete Collection (museum)
						++num1;
					if (Game1.player.hasSkullKey) // Skull key
						++num1;
					int num2 = Game1.isLocationAccessible("CommunityCenter") ? 1 : 0; // All CC bundles are complete
					if (num2 != 0 || Game1.player.hasCompletedCommunityCenter())
						++num1;
					if (num2 != 0) // CC ceremony has taken place
						num1 += 2;
					if (Game1.player.isMarried() && Utility.getHomeOfFarmer(Game1.player).upgradeLevel >= 2) // Married with 2nd house upgrade
						++num1;
					if (Game1.player.hasRustyKey) // Rusty key (sewers)
						++num1;
					if (Game1.player.achievements.Contains(26)) // Master Angler (catch every fish)
						++num1;
					if (Game1.player.achievements.Contains(34)) // Full Shipment (ship every item)
						++num1;
					int friendsWithinThisRange = Utility.getNumberOfFriendsWithinThisRange(Game1.player, 1975, 999999, false);
					if (friendsWithinThisRange >= 5) // 8 hearts with 5+ villagers
						++num1;
					if (friendsWithinThisRange >= 10) // 8 hearts with 10+ villagers
						++num1;
					int level = Game1.player.Level;
					if (level >= 15) // Total 30+ levels in skills
						++num1;
					if (level >= 25) // Total 50 levels in skills (max all)
						++num1;
					string petName = Game1.player.getPetName();
					if (petName != null)
					{
						Pet characterFromName = Game1.getCharacterFromName<Pet>(petName, false);
						if (characterFromName != null && (int)(NetFieldBase<int, NetInt>)characterFromName.friendshipTowardFarmer >= 999)
							++num1; // 999 friendship points with your pet
					}
					return num1;
				}**/
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(getGrandpaScore_Postfix)}:\n{ex}",
					LogLevel.Error);
			}
			return __result; // Return original calculated score
		}

		/// <summary>
		/// Alters the conversion of raw points score to candle number depending on a user's config.
		/// </summary>
		/// <param name="__result">The original result returned by Utility.getGrandpaCandlesFromScore</param>
		/// <param name="score">The raw points score out of 21 (or 13 if ScoringSystem: "Original")</param>
		/// <returns>Number of grandpa candles from 1-4</returns>
		public static int getGrandpaCandlesFromScore_Postfix(int __result, int score)
		{
			try
			{
				if (score >= Config.GetScoreForCandles(4))
					return 4;
				if (score >= Config.GetScoreForCandles(3))
					return 3;
				return score >= Config.GetScoreForCandles(2) ? 2 : 1;
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(getGrandpaCandlesFromScore_Postfix)}:\n{ex}",
					LogLevel.Error);
			}
			return __result;
		}
	}
}
