/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dunc4nNT/StardewMods
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using StardewValley.TokenizableStrings;
using StardewValley.Tools;
using System.Collections.Generic;

namespace NeverToxic.StardewMods.SelfServe.Framework
{
    internal class ShopHelper
    {
        public static void OpenBlacksmithShop(GameLocation location)
        {
            if (Game1.player.toolBeingUpgraded.Value != null && Game1.player.daysLeftForToolUpgrade.Value <= 0)
            {
                if (Game1.player.freeSpotsInInventory() > 0 || Game1.player.toolBeingUpgraded.Value is GenericTool)
                {
                    Tool tool = Game1.player.toolBeingUpgraded.Value;
                    Game1.player.toolBeingUpgraded.Value = null;
                    Game1.player.hasReceivedToolUpgradeMessageYet = false;
                    Game1.player.holdUpItemThenMessage(tool);

                    if (tool is GenericTool)
                        tool.actionWhenClaimed();
                    else
                        Game1.player.addItemToInventoryBool(tool);

                    if (Game1.player.team.useSeparateWallets.Value && tool.UpgradeLevel == 4)
                        Game1.Multiplayer.globalChatInfoMessage("IridiumToolUpgrade", Game1.player.Name, TokenStringBuilder.ToolName(tool.QualifiedItemId, tool.UpgradeLevel));
                }
                else
                    Game1.DrawDialogue(Game1.getCharacterFromName("Clint"), "Data\\ExtraDialogue:Clint_NoInventorySpace");
            }
            else
            {
                bool hasGeode = false;
                foreach (Item item in Game1.player.Items)
                {
                    if (Utility.IsGeode(item))
                    {
                        hasGeode = true;
                        break;
                    }
                }

                List<Response> responses = [new Response("Shop", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Shop"))];

                if (Game1.player.toolBeingUpgraded.Value == null)
                    responses.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Upgrade")));
                if (hasGeode)
                    responses.Add(new Response("Process", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Geodes")));
                responses.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave")));

                location.createQuestionDialogue("", [.. responses], "Blacksmith");
            }

        }

        public static void OpenCarpentersShop(GameLocation location)
        {
            if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.IsThereABuildingUnderConstruction())
            {
                List<Response> responses = [new Response("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop"))];

                if (Game1.IsMasterGame)
                {
                    if (Game1.player.HouseUpgradeLevel < 3)
                        responses.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeHouse")));
                    else if ((Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") || Game1.MasterPlayer.mailReceived.Contains("JojaMember") || Game1.MasterPlayer.hasCompletedCommunityCenter()) && Game1.RequireLocation<Town>("Town").daysUntilCommunityUpgrade.Value <= 0)
                    {
                        if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
                            responses.Add(new Response("CommunityUpgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
                        else if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
                            responses.Add(new Response("CommunityUpgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
                    }
                }
                else if (Game1.player.HouseUpgradeLevel < 3)
                    responses.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeCabin")));

                if (Game1.player.HouseUpgradeLevel >= 2)
                {
                    if (Game1.IsMasterGame)
                        responses.Add(new Response("Renovate", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateHouse")));
                    else
                        responses.Add(new Response("Renovate", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateCabin")));
                }

                responses.Add(new Response("Construct", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Construct")));
                responses.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave")));

                location.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu"), [.. responses], "carpenter");
            }
            else
                Utility.TryOpenShopMenu("Carpenter", location, forceOpen: true);
        }

        public static void OpenAnimalSuppliesShop(GameLocation location)
        {
            List<Response> responses =
            [
                new ("Supplies", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Supplies")),
                new ("Purchase", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Animals")),
                new ("Leave", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Leave"))
            ];

            if (Game1.player.mailReceived.Contains("MarniePetAdoption") || Game1.player.mailReceived.Contains("MarniePetRejectedAdoption"))
                responses.Insert(2, new Response("Adopt", Game1.content.LoadString("Strings\\1_6_Strings:AdoptPets")));

            location.createQuestionDialogue("", [.. responses], "Marnie");
        }

        public static void OpenBookSellerShop(GameLocation location)
        {

            if (Game1.player.mailReceived.Contains("read_a_book"))
                location.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:books_welcome"),
                [
                    new Response("Buy", Game1.content.LoadString("Strings\\1_6_Strings:buy_books")),
                    new Response("Trade", Game1.content.LoadString("Strings\\1_6_Strings:trade_books")),
                    new Response("Leave", Game1.content.LoadString("Strings\\1_6_Strings:Leave"))
                ], "Bookseller");
            else
                Utility.TryOpenShopMenu(Game1.shop_bookseller, location, forceOpen: true);
        }
    }
}
