/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;

namespace CommunityUpgradeFramework;

[HarmonyPatch]
class HarmonyPatches
{
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.carpenters))]
    [HarmonyPrefix]
    public static bool carpenters_Prefix(GameLocation __instance, bool __result, ref Location tileLocation)
    {
		try
		{
			if (Game1.player.currentUpgrade == null)
			{
				foreach (var i in __instance.characters.Where(i => i.Name.Equals("Robin")))
                {
                    if (Vector2.Distance(i.getTileLocation(), new Vector2(tileLocation.X, tileLocation.Y)) > 3f)
                    {
                        __result = false;
                        return false;
                    }
                    i.faceDirection(2);
                    if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.getFarm().isThereABuildingUnderConstruction())
                    {
                        List<Response> options = new()
                        {
                            new Response("Shop",
                                Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop")),
                            // offer Community Upgrades by default, to any player - menu may be empty
                            new Response("CommunityUpgrade",
                                Game1.content.LoadString(
                                    "Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade"))
                        };

                        if (Game1.IsMasterGame)
                        {
                            if (Game1.player.HouseUpgradeLevel < 3)
                            {
                                options.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeHouse")));
                            }
                        }
                        else if (Game1.player.HouseUpgradeLevel < 3)
                        {
                            options.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeCabin")));
                        }
                        if (Game1.player.HouseUpgradeLevel >= 2)
                        {
                            if (Game1.IsMasterGame)
                            {
                                options.Add(new Response("Renovate", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateHouse")));
                            }
                            else
                            {
                                options.Add(new Response("Renovate", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateCabin")));
                            }
                        }
                        options.Add(new Response("Construct", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Construct")));
                        options.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave")));
                        __instance.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu"), options.ToArray(), "carpenter");
                    }
                    else
                    {
                        Game1.activeClickableMenu = new ShopMenu(Utility.getCarpenterStock(), 0, "Robin");
                    }
                    __result = true;
                    return false;
                }
				if (__instance.getCharacterFromName("Robin") == null && Game1.IsVisitingIslandToday("Robin"))
				{
					Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_MoneyBox"));
					Game1.afterDialogues = delegate
					{
						Game1.activeClickableMenu = new ShopMenu(Utility.getCarpenterStock());
					};
					__result = true;
					return false;
				}
				if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Tue"))
				{
					Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_RobinAbsent").Replace('\n', '^'));
					__result = true;
					return false;
				}
				__result = false;
				return false;
			}
			__result = false;
			return false;
		}
		catch (Exception ex)
		{
			Log.Error($"Exception encountered while patching GameLocation::carpenters. Exception: {ex}");
			return true; // run original logic
		}
    }

    [HarmonyPatch(typeof(GameLocation), "communityUpgradeOffer")]
    [HarmonyPrefix]
    public static bool communityUpgradeOffer_Prefix()
    {
        Game1.activeClickableMenu = new CommunityUpgradeMenu("Robin");
        return false;
    }
}
