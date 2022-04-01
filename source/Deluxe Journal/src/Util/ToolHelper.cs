/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewValley;
using StardewValley.Tools;

namespace DeluxeJournal.Util
{
    public class ToolHelper
    {
        /// <summary>Extract a ToolDescription from a Tool.</summary>
        public static ToolDescription GetToolDescription(Tool tool)
        {
            return tool.GetType().Name switch
            {
                nameof(Axe) => new ToolDescription(0, (byte)tool.UpgradeLevel),
                nameof(Hoe) => new ToolDescription(1, (byte)tool.UpgradeLevel),
                nameof(Pickaxe) => new ToolDescription(2, (byte)tool.UpgradeLevel),
                nameof(WateringCan) => new ToolDescription(3, (byte)tool.UpgradeLevel),
                nameof(FishingRod) => new ToolDescription(4, (byte)tool.UpgradeLevel),
                nameof(Pan) => new ToolDescription(5, (byte)tool.UpgradeLevel),
                nameof(Shears) => new ToolDescription(6, (byte)tool.UpgradeLevel),
                nameof(MilkPail) => new ToolDescription(7, (byte)tool.UpgradeLevel),
                nameof(Wand) => new ToolDescription(8, (byte)tool.UpgradeLevel),
                _ => new ToolDescription(0, 0),
            };
        }

        /// <summary>Create a Tool from a ToolDescription.</summary>
        public static Tool? GetToolFromDescription(byte index, byte upgradeLevel)
        {
            return index switch
            {
                0 => new Axe() { UpgradeLevel = upgradeLevel },
                1 => new Hoe() { UpgradeLevel = upgradeLevel },
                2 => new Pickaxe() { UpgradeLevel = upgradeLevel },
                3 => new WateringCan() { UpgradeLevel = upgradeLevel },
                4 => new FishingRod() { UpgradeLevel = upgradeLevel },
                5 => new Pan() { UpgradeLevel = upgradeLevel },
                6 => new Shears() { UpgradeLevel = upgradeLevel },
                7 => new MilkPail() { UpgradeLevel = upgradeLevel },
                8 => new Wand() { UpgradeLevel = upgradeLevel },
                _ => null
            };
        }

        /// <summary>Get the upgrade level for a Tool owned by the local player.</summary>
        /// <param name="toolType">The type value of the Tool.</param>
        /// <returns>The upgrade level for the Tool, or the base level if not found.</returns>
        public static int GetLocalToolUpgradeLevel(Type toolType)
        {
            int level = -1;
            int fallback = 0;

            if (Game1.player.toolBeingUpgraded.Value is Tool tool && tool.GetType() == toolType)
            {
                return tool.UpgradeLevel;
            }

            Utility.iterateAllItems(searchForTool);
            Utility.iterateChestsAndStorage(searchForTool);

            if (level == -1)
            {
                return fallback;
            }

            return level;

            void searchForTool(Item item)
            {
                if (level == -1 && item is Tool tool && tool.GetType() == toolType)
                {
                    if (tool.getLastFarmerToUse() == null)
                    {
                        fallback = tool.UpgradeLevel;
                    }
                    else if (tool.getLastFarmerToUse().IsLocalPlayer)
                    {
                        level = tool.UpgradeLevel;
                    }
                }
            }
        }

        /// <summary>Get the price for a Tool upgrade (at the blacksmith shop) for the given upgrade level.</summary>
        public static int PriceForToolUpgradeLevel(int level)
        {
            return level switch
            {
                1 => 2000,
                2 => 5000,
                3 => 10000,
                4 => 25000,
                _ => 2000,
            };
        }
    }
}
