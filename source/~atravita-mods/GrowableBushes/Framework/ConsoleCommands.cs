/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace GrowableBushes.Framework;

/// <summary>
/// Manages console commands for this mod.
/// </summary>
internal static class ConsoleCommands
{
    /// <summary>
    /// Registers these console commands with SMAPI.
    /// </summary>
    /// <param name="commandHelper">Command helper.</param>
    internal static void RegisterCommands(ICommandHelper commandHelper)
    {
        commandHelper.Add("av.gb.add_bush", "Adds a placeable bush to your inventory", AddBushToInventory);
    }

    private static void AddBushToInventory(string command, string[] args)
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
            foreach (BushSizes possibleBush in BushSizesExtensions.GetValues())
            {
                if (possibleBush == BushSizes.Invalid)
                {
                    continue;
                }

                InventoryBush item = new(possibleBush, count);
                if (!Game1.player.addItemToInventoryBool(item))
                {
                    Game1.currentLocation.debris.Add(new Debris(item, Game1.player.Position));
                }
            }
            return;
        }

        BushSizes bushIndex;
        if (int.TryParse(name, out int id) && BushSizesExtensions.IsDefined((BushSizes)id))
        {
            bushIndex = (BushSizes)id;
        }
        else if (!BushSizesExtensions.TryParse(name, out bushIndex, ignoreCase: true))
        {
            ModEntry.ModMonitor.Log($"{name.ToString()} is not a valid bush. Valid bushes are: {string.Join(" ,", BushSizesExtensions.GetNames())}", LogLevel.Error);
            return;
        }

        {
            InventoryBush item = new(bushIndex, count);
            if (!Game1.player.addItemToInventoryBool(item))
            {
                Game1.currentLocation.debris.Add(new Debris(item, Game1.player.Position));
            }
        }
    }
}
