/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraCore.Framework.ItemManagement;

using AtraShared.ConstantsAndEnums;

namespace GrowableGiantCrops.Framework;

/// <summary>
/// Manages console commands for this mod.
/// </summary>
internal static class ConsoleCommands
{
    internal static void RegisterCommands(ICommandHelper command)
    {
        command.Add("av.ggc.add_shovel", "Adds a shovel to your inventory", AddShovel);
        command.Add("av.ggc.add_giant", "Adds a giant crop to your inventory", AddGiant);
        command.Add("av.ggc.add_resource", "Adds a resource clump to your inventory", AddResource);
    }

    private static void AddShovel(string commands, string[] args)
    {
        ShovelTool shovel = new();
        Game1.player.addItemToInventoryBool(shovel, makeActiveObject: true);
    }

    private static void AddGiant(string commands, string[] args)
    {
        if (args.Length != 1 && args.Length != 2)
        {
            ModEntry.ModMonitor.Log("Expected one or two arguments", LogLevel.Error);
            return;
        }

        if (args.Length != 2 || !int.TryParse(args[1], out int count))
        {
            count = 1;
        }

        string name = args[0].Trim();

        if (!int.TryParse(name, out int productID))
        {
            productID = DataToItemMap.GetID(ItemTypeEnum.SObject, name);
        }

        if (productID < 0)
        {
            ModEntry.ModMonitor.Log($"Could not resolve product '{name}'.", LogLevel.Error);
            return;
        }
    }

    private static void AddResource(string command, string[] args)
    {
        if (args.Length != 1 && args.Length != 2)
        {
            ModEntry.ModMonitor.Log("Expected one or two arguments", LogLevel.Error);
            return;
        }

        if (args.Length != 2 || !int.TryParse(args[1], out int count))
        {
            count = 1;
        }

        ReadOnlySpan<char> name = args[0].AsSpan().Trim();

        if (name.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            foreach (ResourceClumpIndexes possibleBush in ResourceClumpIndexesExtensions.GetValues())
            {
                if (possibleBush == ResourceClumpIndexes.Invalid)
                {
                    continue;
                }

                InventoryResourceClump item = new(possibleBush, count);
                if (!Game1.player.addItemToInventoryBool(item))
                {
                    Game1.currentLocation.debris.Add(new Debris(item, Game1.player.Position));
                }
            }
            return;
        }

        ResourceClumpIndexes bushIndex;
        if (int.TryParse(name, out int id) && ResourceClumpIndexesExtensions.IsDefined((ResourceClumpIndexes)id))
        {
            bushIndex = (ResourceClumpIndexes)id;
        }
        else if (!ResourceClumpIndexesExtensions.TryParse(name, out bushIndex, ignoreCase: true))
        {
            ModEntry.ModMonitor.Log($"{name.ToString()} is not a valid bush. Valid bushes are: {string.Join(" ,", ResourceClumpIndexesExtensions.GetNames())}", LogLevel.Error);
            return;
        }

        {
            InventoryResourceClump item = new(bushIndex, count);
            if (!Game1.player.addItemToInventoryBool(item))
            {
                Game1.currentLocation.debris.Add(new Debris(item, Game1.player.Position));
            }
        }
    }
}
