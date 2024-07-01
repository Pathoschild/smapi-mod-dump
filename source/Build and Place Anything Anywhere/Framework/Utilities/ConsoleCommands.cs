/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using AnythingAnywhere.Framework.Helpers;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Linq;

namespace AnythingAnywhere.Framework.Utilities;
internal static class ConsoleCommands
{
    public static void RegisterCommands()
    {
        ModEntry.ModHelper.ConsoleCommands.Add("aa_remove_objects", "Removes all objects of a specified ID at a specified location.\n\nUsage: aa_remove_objects [LOCATION] [OBJECT_ID]", DebugRemoveObjects);
        ModEntry.ModHelper.ConsoleCommands.Add("aa_remove_furniture", "Removes all furniture of a specified ID at a specified location.\n\nUsage: aa_remove_furniture [LOCATION] [FURNITURE_ID]", DebugRemoveFurniture);
        ModEntry.ModHelper.ConsoleCommands.Add("aa_upgrade_cabin", "Instantly upgrades the cabin the player is inside of.\n\nUsage: aa_upgrade_cabin", DebugUpgradeCabin);
        ModEntry.ModHelper.ConsoleCommands.Add("aa_upgrade_all_cabins", "Instantly upgrades all cabins to max level.\n\nUsage: aa_upgrade_all_cabins", DebugUpgradeAllCabins);
        ModEntry.ModHelper.ConsoleCommands.Add("aa_renovate_cabin", "Opens the renovation menu for the cabin the player is inside of.\n\nUsage: aa_renovate_cabin", DebugRenovateCabin);
        ModEntry.ModHelper.ConsoleCommands.Add("aa_greenhouse_fix", "Finishes construction on all greenhouses.\n\nUsage: aa_greenhouse_fix", DebugGreenhouseFix);
    }

    private static void DebugRemoveFurniture(string command, string[] args)
    {
        if (args.Length <= 1)
        {
            ModEntry.ModMonitor.Log("Missing required arguments: [LOCATION] [FURNITURE_ID]", LogLevel.Warn);
            return;
        }

        if (!Context.IsWorldReady)
        {
            ModEntry.ModMonitor.Log("You need to load a save to use this command.", LogLevel.Error);
            return;
        }

        // get target location
        var location = Game1.locations.FirstOrDefault(p => p.Name?.Equals(args[0], StringComparison.OrdinalIgnoreCase) == true);
        if (location == null && args[0] == "current")
            location = Game1.currentLocation;
        if (location == null)
        {
            string[] locationNames = (from loc in Game1.locations where !string.IsNullOrWhiteSpace(loc.Name) orderby loc.Name select loc.Name).ToArray();
            ModEntry.ModMonitor.Log($"Could not find a location with that name. Must be one of [{string.Join(", ", locationNames)}].", LogLevel.Error);
            return;
        }

        // remove objects
        int removed = 0;
        foreach (var pair in location.furniture.ToArray())
        {
            if (pair.QualifiedItemId != args[1])
                continue;
            location.furniture.Remove(pair);
            removed++;
        }

        ModEntry.ModMonitor.Log($"Command removed {removed} furniture objects at {location.NameOrUniqueName}", LogLevel.Info);
    }

    private static void DebugRemoveObjects(string command, string[] args)
    {
        if (args.Length <= 1)
        {
            ModEntry.ModMonitor.Log("Missing required arguments: [LOCATION] [OBJECT_ID]", LogLevel.Warn);
            return;
        }

        if (!Context.IsWorldReady)
        {
            ModEntry.ModMonitor.Log("You need to load a save to use this command.", LogLevel.Error);
            return;
        }

        // get target location
        var location = Game1.locations.FirstOrDefault(p => p.Name?.Equals(args[0], StringComparison.OrdinalIgnoreCase) == true);
        if (location == null && args[0] == "current")
            location = Game1.currentLocation;
        if (location == null)
        {
            string[] locationNames = (from loc in Game1.locations where !string.IsNullOrWhiteSpace(loc.Name) orderby loc.Name select loc.Name).ToArray();
            ModEntry.ModMonitor.Log($"Could not find a location with that name. Must be one of [{string.Join(", ", locationNames)}].", LogLevel.Error);
            return;
        }

        // remove objects
        int removed = 0;
        foreach (var (tile, obj) in location.Objects.Pairs.ToArray())
        {
            if (obj.QualifiedItemId != args[1])
                continue;
            location.Objects.Remove(tile);
            removed++;
        }

        ModEntry.ModMonitor.Log($"Command removed {removed} objects at {location.NameOrUniqueName}", LogLevel.Info);
    }

    private static void DebugUpgradeCabin(string command, string[] args)
    {
        if (args.Length > 0)
        {
            ModEntry.ModMonitor.Log("This command does not take any arguments", LogLevel.Warn);
            return;
        }

        if (!Context.IsWorldReady)
        {
            ModEntry.ModMonitor.Log("You need to load a save to use this command.", LogLevel.Error);
            return;
        }

        if (!Game1.IsMasterGame)
        {
            ModEntry.ModMonitor.Log("You need to be the main player to use this command.", LogLevel.Error);
            return;
        }

        Building? cabinBuilding = Game1.player.currentLocation.GetContainingBuilding();
        Cabin? cabinInstance = null;
        if (cabinBuilding is { isCabin: true })
            cabinInstance = (Cabin)cabinBuilding.indoors.Value;

        if (cabinInstance is null || cabinBuilding is null)
        {
            ModEntry.ModMonitor.Log("You need to be inside of a cabin to use this command.", LogLevel.Error);
            return;
        }

        if (cabinInstance.owner.isActive())
        {
            ModEntry.ModMonitor.Log("You can only upgrade player cabins when the player is offline.", LogLevel.Error);
            return;
        }

        UpgradeHelper.CompleteHouseUpgrade(cabinInstance.owner, true);
    }

    private static void DebugUpgradeAllCabins(string command, string[] args)
    {
        if (args.Length > 0)
        {
            ModEntry.ModMonitor.Log("This command does not take any arguments", LogLevel.Warn);
            return;
        }

        if (!Context.IsWorldReady)
        {
            ModEntry.ModMonitor.Log("You need to load a save to use this command.", LogLevel.Error);
            return;
        }

        if (!Game1.IsMasterGame)
        {
            ModEntry.ModMonitor.Log("You need to be the main player to use this command.", LogLevel.Error);
            return;
        }

        UpgradeHelper.UpgradeAllCabins();
    }

    private static void DebugRenovateCabin(string command, string[] args)
    {
        if (args.Length > 0)
        {
            ModEntry.ModMonitor.Log("This command does not take any arguments", LogLevel.Warn);
            return;
        }

        if (!Context.IsWorldReady)
        {
            ModEntry.ModMonitor.Log("You need to load a save to use this command.", LogLevel.Error);
            return;
        }

        if (!Game1.IsMasterGame)
        {
            ModEntry.ModMonitor.Log("You need to be the main player to use this command.", LogLevel.Error);
            return;
        }

        var cabinBuilding = Game1.player.currentLocation.GetContainingBuilding();
        Cabin? cabinInstance = null;
        if (cabinBuilding.isCabin)
            cabinInstance = (Cabin)cabinBuilding.indoors.Value;

        if (cabinInstance is null)
        {
            ModEntry.ModMonitor.Log("You need to be inside of a cabin to use this command.", LogLevel.Error);
            return;
        }

        Game1.activeClickableMenu = new ShopMenu("HouseRenovations", RenovationHelper.GetAvailableRenovationsForFarmer(cabinInstance.owner), 0, null, HouseRenovation.OnPurchaseRenovation)
        {
            purchaseSound = null
        };
    }

    private static void DebugGreenhouseFix(string command, string[] args)
    {
        if (args.Length > 0)
        {
            ModEntry.ModMonitor.Log("This command does not take any arguments", LogLevel.Warn);
            return;
        }

        if (!Context.IsWorldReady)
        {
            ModEntry.ModMonitor.Log("You need to load a save to use this command.", LogLevel.Error);
            return;
        }

        if (!Game1.IsMasterGame)
        {
            ModEntry.ModMonitor.Log("You need to be the main player to use this command.", LogLevel.Error);
            return;
        }

        int count = 0;
        foreach (var location in Game1.locations)
        {
            foreach (var building in location.buildings.Where(building => building.buildingType.Value == "Greenhouse" && building.isUnderConstruction()))
            {
                count++;
                building.FinishConstruction();
            }
        }

        ModEntry.ModMonitor.Log($"Command finished construction on {count} Greenhouses", LogLevel.Debug);
    }
}