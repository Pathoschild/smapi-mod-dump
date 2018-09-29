using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace MTN.Patches.GameLocationPatch
{
    //[HarmonyPatch(typeof(GameLocation))]
    //[HarmonyPatch("carpenters")]
    public class carpentersPatch
    {
        public static bool Prefix()
        {
            if (Memory.allowCustomizableFarmHouse)
            {
                return false;
            }
            return true;
        }

        public static void Postfix(GameLocation __instance, Location tileLocation)
        {
            //Adding "Change House Exterior Dessign)
            if (!Memory.allowCustomizableFarmHouse) return;
            if (Game1.player.currentUpgrade != null) return;

            foreach (NPC character in __instance.characters)
            {
                if (character.Name.Equals("Robin"))
                {
                    if ((double)Vector2.Distance(character.getTileLocation(), new Vector2((float)tileLocation.X, (float)tileLocation.Y)) > 3.0)
                        return;
                    character.faceDirection(2);
                    if (Game1.player.daysUntilHouseUpgrade < 0 && !Game1.getFarm().isThereABuildingUnderConstruction())
                    {
                        List<Response> responseList = new List<Response>();
                        responseList.Add(new Response("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop")));
                        if (Game1.IsMasterGame)
                        {
                            if ((int)((NetFieldBase<int, NetInt>)Game1.player.houseUpgradeLevel) < 3)
                                responseList.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeHouse")));
                            else if ((Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") || Game1.MasterPlayer.mailReceived.Contains("JojaMember") || Game1.MasterPlayer.hasCompletedCommunityCenter()) && ((int)((NetFieldBase<int, NetInt>)(Game1.getLocationFromName("Town") as Town).daysUntilCommunityUpgrade) <= 0 && !Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade")))
                                responseList.Add(new Response("CommunityUpgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
                        }
                        else if ((int)((NetFieldBase<int, NetInt>)Game1.player.houseUpgradeLevel) < 2)
                            responseList.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeCabin")));
                        //REEEEE
                        if (Game1.IsMasterGame)
                        {
                            responseList.Add(new Response("HouseDesign", "Change Farm House Exterior"));
                        }

                        responseList.Add(new Response("Construct", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Construct")));
                        responseList.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave")));
                        __instance.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu"), responseList.ToArray(), "carpenter");
                        return;
                    }
                    Game1.activeClickableMenu = (IClickableMenu)new ShopMenu(Utility.getCarpenterStock(), 0, "Robin");
                    return;
                }
            }
            if (!Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Tue"))
                return;
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_RobinAbsent").Replace('\n', '^'));
        }
    }
}
