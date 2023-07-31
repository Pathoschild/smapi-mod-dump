/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Objects;
using xTile.Dimensions;

namespace NovoMundo
{
    public class Question_Dialogues
    {


        public void CarpenterMenu()
        {
            List<Response> list = new List<Response>();
            list.Add(new Response("Shop", "Shop"));
            if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.getFarm().isThereABuildingUnderConstruction())
            {

                if (Game1.IsMasterGame)
                {
                    if (Game1.player.HouseUpgradeLevel < 3)
                    {
                        list.Add(new Response("Upgrade", "UpgradeHouse"));
                    }
                }
                else if (Game1.player.HouseUpgradeLevel < 3)
                {
                    list.Add(new Response("Upgrade", "UpgradeCabin"));
                }

                if (Game1.player.HouseUpgradeLevel >= 2)
                {
                    if (Game1.IsMasterGame)
                    {
                        list.Add(new Response("Renovate", "RenovateHouse"));
                    }
                    else
                    {
                        list.Add(new Response("Renovate", "RenovateCabin"));
                    }
                }
                list.Add(new Response("Special", "Special"));
                list.Add(new Response("Leave", "Leave"));
                Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu"), list.ToArray(), new GameLocation.afterQuestionBehavior(Output_CarpenterMenu));
                
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

        public void Output_CarpenterMenu(Farmer farmer, string choice)
        {
            switch (choice)
            {
                case "Shop":
                    {
                        if (Game1.currentLocation.getCharacterFromName("Robin") != null)
                        {
                            Game1.activeClickableMenu = new ShopMenu(Utility.getCarpenterStock(), 0, "Robin");
                        }
                        else
                        {
                            Game1.activeClickableMenu = new ShopMenu(Utility.getCarpenterStock(), 0, null);
                        }
                        break;
                    }
                case "Upgrade":
                    {
                        if (Game1.currentLocation.getCharacterFromName("Robin") != null)
                        {
                            switch (Game1.player.HouseUpgradeLevel)
                            {
                                case 0:
                                    Game1.currentLocation.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse1")), Game1.currentLocation.createYesNoResponses(), "upgrade");
                                    break;
                                case 1:
                                    List<Response> list1 = new List<Response>();
                                    Game1.currentLocation.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse2")), Game1.currentLocation.createYesNoResponses(), "upgrade");
                                    break;
                                case 2:
                                    List<Response> list2 = new List<Response>();
                                    Game1.currentLocation.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse3")), Game1.currentLocation.createYesNoResponses(), "upgrade");
                                    break;
                            }
                            break;
                        }
                        else
                        {
                            Game1.drawObjectDialogue("Robin Ausente.");
                        }
                        break;      
                    }
                case "Special":
                    {
                        if (Game1.currentLocation.getCharacterFromName("Robin") != null)
                        {
                            Location tilelocation = new();
                            Game1.currentLocation.carpenters(tilelocation);
                            break;
                        }
                        else
                        {
                            Game1.drawObjectDialogue("Robin Ausente.");
                        }
                        break;
                    }
                case "Renovate":
                    {
                        HouseRenovation.ShowRenovationMenu();
                        break;
                    }
            }
        }
    }
}
