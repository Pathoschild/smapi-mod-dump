/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using BirbCore.Attributes;
using StardewValley;

namespace RanchingToolUpgrades;

[SCommand("ranch_tool")]
public class Command
{
    [SCommand.Command("Add a pail to the player inventory")]
    public static void GivePail(int level = 0)
    {
        Game1.player.addItemToInventory(level switch
        {
            0 => ItemRegistry.Create("(T)MilkPail"),
            1 => ItemRegistry.Create("(T)drbirbdev.RanchingToolUpgrades_CopperMilkPail"),
            2 => ItemRegistry.Create("(T)drbirbdev.RanchingToolUpgrades_SteelMilkPail"),
            3 => ItemRegistry.Create("(T)drbirbdev.RanchingToolUpgrades_GoldMilkPail"),
            _ => ItemRegistry.Create("(T)drbirbdev.RanchingToolUpgrades_IridiumMilkPail")
        });
    }

    [SCommand.Command("Add shears to the player inventory")]
    public static void GiveShears(int level = 0)
    {
        Game1.player.addItemToInventory(level switch
        {
            0 => ItemRegistry.Create("(T)Shears"),
            1 => ItemRegistry.Create("(T)drbirbdev.RanchingToolUpgrades_CopperShears"),
            2 => ItemRegistry.Create("(T)drbirbdev.RanchingToolUpgrades_SteelShears"),
            3 => ItemRegistry.Create("(T)drbirbdev.RanchingToolUpgrades_GoldShears"),
            _ => ItemRegistry.Create("(T)drbirbdev.RanchingToolUpgrades_IridiumShears")
        });
    }

    [SCommand.Command("Remove a pail from the player inventory")]
    public static void RemovePail()
    {
        Item pail = Game1.player.getToolFromName("Pail");
        if (pail is not null)
        {
            Game1.player.removeItemFromInventory(pail);
        }
    }

    [SCommand.Command("Remove shears from the player inventory")]
    public static void RemoveShears()
    {
        Item shears = Game1.player.getToolFromName("Shears");
        if (shears is not null)
        {
            Game1.player.removeItemFromInventory(shears);
        }
    }
}
