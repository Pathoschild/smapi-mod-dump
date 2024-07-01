/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace StardewArchipelago.Items
{
    public class ToolUpgrader
    {
        public Tool UpgradeToolInEntireWorld(string toolGenericName)
        {
            var player = Game1.player;
            var toolName = toolGenericName.Replace(" ", "_");
            if (TryUpgradeToolInFarmer(player, toolName, out var upgradedTool))
            {
                return upgradedTool;
            }

            if (TryUpgradeToolInChests(toolName, out upgradedTool))
            {
                return upgradedTool;
            }

            if (TryUpgradeToolInLostAndFoundBox(player, toolName, out upgradedTool))
            {
                return upgradedTool;
            }

            return null;
        }

        private bool TryUpgradeToolInFarmer(Farmer player, string toolName, out Tool upgradedTool)
        {
            if (TryUpgradeToolInInventory(player.Items, toolName, out upgradedTool))
            {
                return true;
            }

            return TryUpgradeHat(player, toolName, out upgradedTool);
        }

        private bool TryUpgradeToolInInventory(IList<Item> inventory, string toolName, out Tool upgradedTool)
        {
            for (var i = 0; i < inventory.Count; i++)
            {
                if (TryUpgradeCorrectTool(toolName, inventory[i], out upgradedTool))
                {
                    inventory[i] = upgradedTool;
                    return true;
                }
            }

            upgradedTool = null;
            return false;
        }

        private bool TryUpgradeHat(Farmer player, string toolName, out Tool upgradedTool)
        {
            var currentHat = player.hat?.Value;
            upgradedTool = null;
            if (currentHat == null)
            {
                return false;
            }

            var hatInHand = Utility.PerformSpecialItemGrabReplacement(currentHat);
            if (TryUpgradeCorrectTool(toolName, hatInHand, out upgradedTool))
            {
                var upgradedHat = Utility.PerformSpecialItemPlaceReplacement(upgradedTool);
                player.Equip((Hat)upgradedHat, player.hat);
                return true;
            }

            return false;
        }

        private bool TryUpgradeToolInChests(string toolName, out Tool upgradedTool)
        {
            var locations = Game1.locations.ToList();

            foreach (var building in Game1.getFarm().buildings)
            {
                if (building?.indoors.Value == null)
                {
                    continue;
                }
                locations.Add(building.indoors.Value);
            }

            foreach (var gameLocation in locations)
            {
                foreach (var (tile, gameObject) in gameLocation.Objects.Pairs)
                {
                    if (gameObject is not Chest chest)
                    {
                        continue;
                    }

                    if (TryUpgradeToolInInventory(chest.Items, toolName, out upgradedTool))
                    {
                        return true;
                    }
                }
            }


            if (Game1.getLocationFromName("FarmHouse") is FarmHouse farmHouse)
            {
                if (TryUpgradeToolInInventory(farmHouse.GetFridge(false).Items, toolName, out upgradedTool))
                {
                    return true;
                }
            }

            if (Game1.getLocationFromName("IslandFarmHouse") is IslandFarmHouse islandHouse)
            {
                if (TryUpgradeToolInInventory(islandHouse.GetFridge(false).Items, toolName, out upgradedTool))
                {
                    return true;
                }
            }

            upgradedTool = null;
            return false;
        }

        private bool TryUpgradeToolInLostAndFoundBox(Farmer player, string toolName, out Tool upgradedTool)
        {
            if (TryUpgradeToolInInventory(player.team.returnedDonations, toolName, out upgradedTool))
            {
                return true;
            }

            upgradedTool = null;
            return false;
        }

        private bool TryUpgradeCorrectTool(string toolName, Item item, out Tool upgradedTool)
        {
            if (item is not Tool toolToUpgrade || !toolToUpgrade.Name.Replace(" ", "_").Contains(toolName))
            {
                upgradedTool = null;
                return false;
            }

            foreach (var (toolId, toolData) in Game1.toolData)
            {
                if (toolData.ConventionalUpgradeFrom == item.QualifiedItemId)
                {
                    upgradedTool = CreateTool(toolId);
                    return true;
                }
            }

            upgradedTool = null;
            return false;
        }

        public Tool CreateTool(string toolName)
        {
            return (Tool)ItemRegistry.Create("(T)" + toolName);
        }
    }
}
