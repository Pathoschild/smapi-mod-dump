using Harmony;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;
using Netcode;
using System;

namespace AngryGrandpa
{
	/// <summary>The class for patching methods on the StardewValley.Object class.</summary>
	class ObjectPatches
	{
		/*********
        ** Accessors
        *********/
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ModConfig Config => ModConfig.Instance;
		private static HarmonyInstance Harmony => ModEntry.Instance.Harmony;


		/*********
        ** Fields
        *********/
		protected static ITranslationHelper i18n = Helper.Translation;


		/*********
        ** Public methods
        *********/
		/// <summary>
		/// Applies the harmony patches defined in this class.
		/// </summary>
		public static void Apply()
		{
			Harmony.Patch(
				original: AccessTools.Method(typeof(Object),
					nameof(Object.checkForSpecialItemHoldUpMeessage)), // Yes, the game code spells it Meessage. -_-
				postfix: new HarmonyMethod(typeof(ObjectPatches),
					nameof(ObjectPatches.checkForSpecialItemHoldUpMeessage_Postfix))
			);
		}

		/// <summary>
		/// Provides a special message to display for the bonus reward items given by grandpa's shrine.
		/// In theory this *could* be triggered by non-shrine actions, but it's unlikely under normal circumstances.
		/// </summary>
		/// <param name="__result">The original string result returned by checkForSpecialItemHoldUpMeessage</param>
		/// <param name="__instance">The SDV Object that the method was called on.</param>
		public static void checkForSpecialItemHoldUpMeessage_Postfix(ref string __result, Object __instance)
		{
			try
			{
				if ( !(bool)(NetFieldBase<bool, NetBool>)__instance.bigCraftable &&
					(NetFieldBase<string, NetString>)__instance.type != (NetString)null &&
					Game1.getFarm().grandpaScore != 0 &&
					Game1.currentLocation is Farm)
				{
					if (__instance.type.Equals((object)"Arch") &&
						Game1.player.archaeologyFound.Count() > 0) // They've already found at least one artifact
					{
						switch ((int)(NetFieldBase<int, NetInt>)__instance.parentSheetIndex)
						{
							case 114: // Ancient seed
								__result = i18n.Get("Object.cs.1CandleReward");
								break;
							case 107: // Dinosaur egg
								__result = i18n.Get("Object.cs.2CandleReward");
								break;
						}
					} 
					else if (((int)(NetFieldBase<int, NetInt>)__instance.parentSheetIndex) == 74) // Prismatic shard
					{
						__result = i18n.Get("Object.cs.3CandleReward");
					}
				}
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(checkForSpecialItemHoldUpMeessage_Postfix)}:\n{ex}",
					LogLevel.Error);
			}
		}
	}
}
