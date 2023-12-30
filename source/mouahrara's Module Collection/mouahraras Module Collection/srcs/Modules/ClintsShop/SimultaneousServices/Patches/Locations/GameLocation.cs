/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using xTile.Dimensions;
using StardewValley;
using StardewValley.Menus;

namespace mouahrarasModuleCollection.ClintsShop.SimultaneousServices.Patches
{
	internal class GameLocationPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.blacksmith), new Type[] { typeof(Location) }),
				prefix: new HarmonyMethod(typeof(GameLocationPatch), nameof(BlacksmithPrefix))
			);
		}

		private static bool BlacksmithPrefix(GameLocation __instance, Location tileLocation, ref bool __result)
		{
			if (!ModEntry.Config.ClintsShopSimultaneousServices)
				return true;

			foreach (NPC character in __instance.characters)
			{
				if (!character.Name.Equals("Clint"))
				{
					continue;
				}
				if (character.Tile != new Vector2(tileLocation.X, tileLocation.Y - 1))
				{
					_ = character.Tile != new Vector2(tileLocation.X - 1, tileLocation.Y - 1);
				}
				character.faceDirection(2);
				if (Game1.player.toolBeingUpgraded.Value != null && Game1.player.daysLeftForToolUpgrade.Value > 0)
				{
					bool flag = false;

					foreach (Item item in Game1.player.Items)
					{
						if (Utility.IsGeode(item))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						Utility.TryOpenShopMenu("Blacksmith", "Clint");
					}
					else
					{
						Response[] answerChoices = new Response[3]
						{
							new("Shop", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Shop")),
							new("Process", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Geodes")),
							new("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave"))
						};
						__instance.createQuestionDialogue("", answerChoices, "Blacksmith");
					};
					__result = true;
					return false;
				}
				__result = true;
				return true;
			}
			__result = false;
			return false;
		}
	}
}
