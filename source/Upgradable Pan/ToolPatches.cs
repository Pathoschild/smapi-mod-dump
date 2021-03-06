/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/UpgradeablePan
**
*************************************************/

using StardewValley;
using StardewValley.Tools;

namespace UpgradablePan
{
	public class ToolPatches
	{

		public static bool set_IndexOfMenuItemView_Prefix(ref Tool __instance, int value)
		{
			if (__instance is Pan)
			{
				switch (__instance.UpgradeLevel)
				{
					case 2:
					case 3:
					case 4:
						__instance.indexOfMenuItemView.Set(15 + __instance.UpgradeLevel);
						break;
					default:
						__instance.indexOfMenuItemView.Set(value);
						break;
				}
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool get_IndexOfMenuItemView_Prefix(ref Tool __instance, ref int __result)
		{
			if (__instance is Pan)
			{
				switch (__instance.UpgradeLevel)
				{
					case 2:
					case 3:
					case 4:
						__result = 15 + __instance.UpgradeLevel;
						return false;
					default:
						return true;
				}
			}
			else
			{
				return true;
			}
		}

		public static bool actionWhenPurchased_Prefix(ref Tool __instance, ref bool __result)
		{
			if (Game1.player.toolBeingUpgraded.Value == null)
			{
				if (__instance is Axe || __instance is Pickaxe || __instance is Hoe || __instance is WateringCan || __instance is Pan)
				{
					// Hacky, but much easier than the alternative...
					if (__instance is Pan)
					{
						__instance.BaseName = "Pan";
					}
					Tool t = Game1.player.getToolFromName(__instance.BaseName);
					t.UpgradeLevel++;
					Game1.player.removeItemFromInventory(t);
					Game1.player.toolBeingUpgraded.Value = t;
					Game1.player.daysLeftForToolUpgrade.Value = 2;
					Game1.playSound("parry");
					Game1.exitActiveMenu();
					Game1.drawDialogue(Game1.getCharacterFromName("Clint"), Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14317"));
					__result = true;
					return false;
				}
				if (__instance is GenericTool)
				{
					int num = __instance.indexOfMenuItemView;
					if ((uint)(num - 13) <= 3u)
					{
						Game1.player.toolBeingUpgraded.Value = __instance;
						Game1.player.daysLeftForToolUpgrade.Value = 2;
						Game1.playSound("parry");
						Game1.exitActiveMenu();
						Game1.drawDialogue(Game1.getCharacterFromName("Clint"), Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14317"));
						__result = true;
						return false;
					}
				}
			}
			__result = __instance.actionWhenPurchased();
			return false;
		}
	}
}