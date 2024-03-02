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
using StardewValley;
using mouahrarasModuleCollection.TweaksAndFeatures.Machines.SafeReplacement.Utilities;

namespace mouahrarasModuleCollection.TweaksAndFeatures.Machines.SafeReplacement.Patches
{
	internal class ObjectPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(Object), nameof(Object.performObjectDropInAction), new System.Type[] { typeof(Item), typeof(bool), typeof(Farmer), typeof(bool) }),
				prefix: new HarmonyMethod(typeof(ObjectPatch), nameof(PerformObjectDropInActionPrefix))
			);
		}

		private static bool PerformObjectDropInActionPrefix(Object __instance, Item dropInItem, bool probe, Farmer who, ref bool __result)
		{
			if (!ModEntry.Config.MachinesSafeReplacement)
				return true;
			if (__instance.isTemporarilyInvisible)
				return true;
			if (dropInItem is not Object)
				return true;
			if (!__instance.name.Equals("Crystalarium"))
				return true;

			if ((dropInItem.Category == -2 || dropInItem.Category == -12) && dropInItem.ParentSheetIndex != 74 && (__instance.heldObject.Value == null || __instance.heldObject.Value.ParentSheetIndex != dropInItem.ParentSheetIndex) && (__instance.heldObject.Value == null || __instance.MinutesUntilReady > 0))
			{
				if (!probe)
				{
					if (who.freeSpotsInInventory() > 0 || (who.freeSpotsInInventory() == 0 && dropInItem.Stack == 1))
					{
						SafeReplacementUtility.ObjectToRecover = __instance.heldObject.Value;
						return true;
					}
					if (who.couldInventoryAcceptThisItem(__instance.heldObject.Value))
					{
						SafeReplacementUtility.ObjectToRecover = __instance.heldObject.Value;
						return true;
					}
					__result = false;
					return false;
				}
			}
			return true;
		}
	}
}
