/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using AnythingAnywhere.Framework.Utilities;

namespace AnythingAnywhere.Framework.Helpers;
internal static class UpgradeHelper
{
    public static void UpgradeCabinsResponses()
    {
        var cabinPageNames = CabinUtility.GetCabinsToUpgrade();
        Game1.currentLocation.ShowPagedResponses("Upgrade Cabin?", cabinPageNames, OfferCabinUpgrade);
    }

    private static void OfferCabinUpgrade(string cabin)
    {
        var cabinBuilding = Game1.getLocationFromName(cabin).GetContainingBuilding();
        var cabinInstance = (Cabin)cabinBuilding.indoors.Value;

        string msg;
        switch (cabinInstance.owner.HouseUpgradeLevel)
        {
            case 0:
                msg = Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse1");
                msg = ModEntry.Config.EnableFreeHouseUpgrade ? msg.Replace("10,000", "0").Replace("10.000", "0").Replace("10 000", "0").Replace("450", "0") : msg;
                break;
            case 1:
                msg = Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse2", ModEntry.Config.EnableFreeHouseUpgrade ? "0" : "65,000", ModEntry.Config.EnableFreeHouseUpgrade ? "0" : "100");
                break;
            case 2:
                msg = Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse3");
                msg = ModEntry.Config.EnableFreeHouseUpgrade ? msg.Replace("10,000", "0").Replace("10.000", "0").Replace("100,000", "0").Replace("100.000", "0").Replace("100 000", "0") : msg;
                break;
            default:
                return; // or handle the default case
        }

        Game1.currentLocation.createQuestionDialogue(Game1.parseText(msg), Game1.currentLocation.createYesNoResponses(), (_, answer) =>
        {
            if (answer != "Yes") return;
            AcceptCabinUpgrade(cabin);
        });
    }


    private static void AcceptCabinUpgrade(string cabin)
    {
        var cabinBuilding = Game1.getLocationFromName(cabin).GetContainingBuilding();
        var cabinInstance = (Cabin)cabinBuilding.indoors.Value;

        if (cabinInstance == null)
        {
            ModEntry.ModMonitor.Log("Could not find cabin location", LogLevel.Error);
            return;
        }

        if (ModEntry.Config.EnableFreeHouseUpgrade)
        {
            cabinInstance.owner.daysUntilHouseUpgrade.Value = 3;
            Game1.RequireCharacter("Robin").setNewDialogue("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted");
            Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
            ModEntry.Multiplayer?.globalChatInfoMessage("HouseUpgrade", Game1.player.Name, Lexicon.getTokenizedPossessivePronoun(Game1.player.IsMale));
            CompleteHouseUpgrade(cabinInstance.owner);
            return;
        }

        switch (cabinInstance.upgradeLevel)
        {
            case 0:
                if (Game1.player.Money >= 10000 && Game1.player.Items.ContainsId("(O)388", 450))
                {
                    cabinInstance.owner.daysUntilHouseUpgrade.Value = 3;
                    Game1.player.Money -= 10000;
                    Game1.player.Items.ReduceId("(O)388", 450);
                    Game1.RequireCharacter("Robin").setNewDialogue("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted");
                    Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
                    ModEntry.Multiplayer?.globalChatInfoMessage("HouseUpgrade", Game1.player.Name, Lexicon.getTokenizedPossessivePronoun(Game1.player.IsMale));
                    CompleteHouseUpgrade(cabinInstance.owner);
                }
                else if (Game1.player.Money < 10000)
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
                }
                else
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_NotEnoughWood1"));
                }

                break;
            case 1:
                if (Game1.player.Money >= 65000 && Game1.player.Items.ContainsId("(O)709", 100))
                {
                    cabinBuilding.daysUntilUpgrade.Value = 3;
                    Game1.player.Money -= 65000;
                    Game1.player.Items.ReduceId("(O)709", 100);
                    Game1.RequireCharacter("Robin").setNewDialogue("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted");
                    Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
                    ModEntry.Multiplayer?.globalChatInfoMessage("HouseUpgrade", Game1.player.Name, Lexicon.getTokenizedPossessivePronoun(Game1.player.IsMale));
                    CompleteHouseUpgrade(cabinInstance.owner);
                }
                else if (Game1.player.Money < 65000)
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
                }
                else
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_NotEnoughWood2", "100"));
                }

                break;
            case 2:
                if (Game1.player.Money >= 100000)
                {
                    cabinBuilding.daysUntilUpgrade.Value = 3;
                    Game1.player.Money -= 100000;
                    Game1.RequireCharacter("Robin").setNewDialogue("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted");
                    Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
                    ModEntry.Multiplayer?.globalChatInfoMessage("HouseUpgrade", Game1.player.Name, Lexicon.getTokenizedPossessivePronoun(Game1.player.IsMale));
                    CompleteHouseUpgrade(cabinInstance.owner);
                }
                else if (Game1.player.Money < 100000)
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
                }

                break;
        }
    }

    public static void CompleteHouseUpgrade(Farmer owner, bool debug = false)
    {
        if (!ModEntry.Config.InstantHomeUpgrade && !debug)
            return;

        if (debug)
        {
            var msg = $"Upgrading cabin at {Utility.getHomeOfFarmer(owner).NameOrUniqueName} from level {owner.HouseUpgradeLevel} to {owner.HouseUpgradeLevel + 1}";
            ModEntry.ModMonitor.Log(msg, LogLevel.Debug);
        }

        var homeOfFarmer = Utility.getHomeOfFarmer(owner);
        homeOfFarmer.moveObjectsForHouseUpgrade(owner.HouseUpgradeLevel + 1);
        owner.HouseUpgradeLevel++;
        owner.daysUntilHouseUpgrade.Value = -1;
        homeOfFarmer.setMapForUpgradeLevel(owner.HouseUpgradeLevel);
        owner.stats.checkForBuildingUpgradeAchievements();
        owner.autoGenerateActiveDialogueEvent("houseUpgrade_" + owner.HouseUpgradeLevel);
    }

    public static void UpgradeAllCabins()
    {
        int count = 0;
        foreach (var cabin in CabinUtility.GetCabins())
        {
            if (cabin.owner.isActive() || cabin.owner.HouseUpgradeLevel >= 3)
                continue;

            var msg = $"Upgrading cabin at {cabin.NameOrUniqueName} from level {cabin.owner.HouseUpgradeLevel} to 3";
            ModEntry.ModMonitor.Log(msg, LogLevel.Debug);

            var homeOfFarmer = Utility.getHomeOfFarmer(cabin.owner);
            homeOfFarmer.moveObjectsForHouseUpgrade(3);
            cabin.owner.HouseUpgradeLevel = 3;
            cabin.owner.daysUntilHouseUpgrade.Value = -1;
            homeOfFarmer.setMapForUpgradeLevel(cabin.owner.HouseUpgradeLevel);
            cabin.owner.stats.checkForBuildingUpgradeAchievements();
            cabin.owner.autoGenerateActiveDialogueEvent("houseUpgrade_" + cabin.owner.HouseUpgradeLevel);

            count++;
        }

        ModEntry.ModMonitor.Log($"Upgraded {count} cabins", LogLevel.Debug);

    }
}