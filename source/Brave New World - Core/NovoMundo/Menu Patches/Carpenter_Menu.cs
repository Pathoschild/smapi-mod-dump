/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.ObjectModel;
using xTile.Tiles;
using Microsoft.Xna.Framework;
using HarmonyLib;
using StardewValley.Locations;
using xTile.Dimensions;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using System.Xml.Linq;

namespace NovoMundo
{
    public class Menu_Patches
    {
        public void Apply_Harmony(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.carpenters)),
                postfix: new HarmonyMethod(typeof(Menu_Patches), nameof(Menu_Patches.Carpenters_ChangeQuestionMenu))
            );

        }
        public static void Carpenters_ChangeQuestionMenu()
        {
            if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.getFarm().isThereABuildingUnderConstruction())
            {
                List<Response> list = new List<Response>();
                if (ModEntry.ModHelper.ModRegistry.IsLoaded("somaraezel.NovoMundoCPData") is false || (Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") || Game1.MasterPlayer.mailReceived.Contains("JojaMember") || Game1.MasterPlayer.hasCompletedCommunityCenter()) && (int)(Game1.getLocationFromName("Town") as Town).daysUntilCommunityUpgrade <= 0)
                {
                    if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
                    {
                        list.Add(new Response("CommunityUpgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
                    }
                    else if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
                    {
                        list.Add(new Response("CommunityUpgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
                    }
                }
                else if (ModEntry.ModHelper.ModRegistry.IsLoaded("PeacefulEnd.AMouseWithAHat.Core") && Game1.MasterPlayer.mailReceived.Contains("hatter") is true && Game1.MasterPlayer.mailReceived.Contains("HatShopRepaired") is false)
                {
                    list.Add(new Response("RepairHatShop", "Repair Hat Shop"));
                    Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu"), list.ToArray(), "carpenter");
                }
                else
                {
                    if (Game1.currentLocation.getCharacterFromName("Robin") != null)
                    {
                        Game1.drawDialogue(Game1.getCharacterFromName("Robin"),"Nenhum projeto.");
                    }
                    else
                    {
                        Game1.drawObjectDialogue("Nenhum projeto.");
                    }
                        
                }
            }
            else
            {
                if (Game1.currentLocation.getCharacterFromName("Robin") != null)
                {
                    Game1.drawDialogue(Game1.getCharacterFromName("Robin"), "Equipe Ocupada.");
                }
                else
                {
                    Game1.drawObjectDialogue("Equipe Ocupada.");
                }
            }

        }
    }
}

