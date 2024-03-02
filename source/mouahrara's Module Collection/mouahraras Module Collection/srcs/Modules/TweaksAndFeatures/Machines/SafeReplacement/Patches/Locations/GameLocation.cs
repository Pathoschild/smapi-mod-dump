/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using HarmonyLib;
using xTile.Dimensions;
using StardewValley;
using mouahrarasModuleCollection.TweaksAndFeatures.Machines.SafeReplacement.Utilities;

namespace mouahrarasModuleCollection.TweaksAndFeatures.Machines.SafeReplacement.Patches
{
	internal class GameLocationPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkAction), new System.Type[] { typeof(Location), typeof(Rectangle), typeof(Farmer) }),
				postfix: new HarmonyMethod(typeof(GameLocationPatch), nameof(CheckActionPostfix))
			);
		}

		private static void CheckActionPostfix()
		{
			if (!ModEntry.Config.MachinesSafeReplacement)
				return;
			if (SafeReplacementUtility.ObjectToRecover == null)
				return;

			Game1.player.addItemToInventory(SafeReplacementUtility.ObjectToRecover);
			SafeReplacementUtility.Reset();
		}
	}
}
