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
using StardewValley.GameData.Tools;
using StardewValley.Tools;

namespace DeluxeJournal.Util
{
    public static class ToolHelper
    {
        /// <summary>Get the upgraded version of a tool or a tool with a specific upgrade level.</summary>
        /// <param name="toolData">The tool data.</param>
        /// <param name="upgradeLevel">
        /// A specific upgrade level or a value less than zero to upgrade to <c>toolData.UpgradeLevel+1</c>.
        /// </param>
        /// <returns>The upgraded tool or <c>null</c> if one could not be created.</returns>
        public static Tool? GetToolUpgrade(ToolData toolData, int upgradeLevel = -1)
        {
            upgradeLevel = upgradeLevel < 0 ? toolData.UpgradeLevel + 1 : upgradeLevel;

            foreach (KeyValuePair<string, ToolData> pair in Game1.toolData)
            {
                if (pair.Value.ClassName == toolData.ClassName && pair.Value.UpgradeLevel == upgradeLevel)
                {
                    return ItemRegistry.Create<Tool>(ItemRegistry.type_tool + pair.Key);
                }
            }

            return null;
        }

        /// <summary>
        /// Find a tool owned by a given player and return its upgrade.
        /// </summary>
        /// 
        /// <remarks>
        /// "Ownership" here is defined as the last player to use a tool or the tool in a
        /// player's inventory. When enabling both <paramref name="includeUsedTools"/> and
        /// <paramref name="includeUnusedTools"/>, unused tools are always prioritized over used
        /// ones and the tool of the highest upgrade level is chosen when breaking ties.
        /// </remarks>
        /// 
        /// <param name="toolData">The tool data.</param>
        /// <param name="player">The player that owns the tool.</param>
        /// <param name="includeUsedTools">Allow tools that were last used by another player.</param>
        /// <param name="includeUnusedTools">Allow tools that were not used by any player.</param>
        /// <returns>
        /// The upgraded tool. If no match is found, or the upgraded tool would exceed iridium
        /// quality, then the original tool with associated <paramref name="toolData"/> is returned.
        /// If the original tool could not be created, then <c>null</c>.
        /// </returns>
        public static Tool? GetToolUpgradeForPlayer(ToolData toolData, Farmer player, bool includeUsedTools = true, bool includeUnusedTools = true)
        {
            Tool? foundTool = null;

            if (IsTrashCan(toolData))
            {
                return GetToolUpgrade(toolData, Math.Max(toolData.UpgradeLevel, player.trashCanLevel + 1));
            }
            else if (player.toolBeingUpgraded.Value is Tool upgradedTool && upgradedTool.GetToolData()?.ClassName == toolData.ClassName)
            {
                return upgradedTool;
            }

            foreach (Item item in player.Items)
            {
                if (item is Tool tool && tool.GetToolData() is ToolData playerToolData && playerToolData.ClassName == toolData.ClassName)
                {
                    foundTool = tool;
                    break;
                }
            }

            if (foundTool == null)
            {
                Utility.ForEachItem(searchForTool);
            }

            return foundTool?.GetToolData() is ToolData foundToolData
                ? GetToolUpgrade(foundToolData)
                : GetToolUpgrade(toolData, toolData.UpgradeLevel);

            bool searchForTool(Item item)
            {
                if (item is Tool worldTool && worldTool.GetToolData() is ToolData worldToolData && worldToolData.ClassName == toolData.ClassName)
                {
                    Farmer? owner = worldTool.getLastFarmerToUse();

                    if (owner != null && owner.UniqueMultiplayerID == player.UniqueMultiplayerID)
                    {
                        foundTool = worldTool;
                        return false;
                    }
                    else if ((includeUnusedTools || includeUsedTools) && worldTool.UpgradeLevel < Tool.iridium)
                    {
                        if ((owner == null && !includeUnusedTools) || (owner != null && !includeUsedTools))
                        {
                            return true;
                        }

                        if (foundTool != null)
                        {
                            bool guessedToolUnowned = foundTool.getLastFarmerToUse() == null;
                            bool downgradeGuess = foundTool.UpgradeLevel > worldTool.UpgradeLevel;

                            if ((owner == null && guessedToolUnowned && downgradeGuess) ||
                                (owner != null && (guessedToolUnowned || downgradeGuess)))
                            {
                                return true;
                            }
                        }

                        foundTool = worldTool;
                    }
                }

                return true;
            }
        }

        /// <summary>Is the tool with the associated <see cref="ToolData"/> a trash can?</summary>
        /// <param name="toolData">The tool data.</param>
        public static bool IsTrashCan(ToolData toolData)
        {
            return toolData.ClassName == nameof(GenericTool) && (toolData.SpriteIndex >= 13 || toolData.SpriteIndex <= 16);
        }

        /// <summary>
        /// Is the tool type with the associated <see cref="ToolData"/> upgradable at the blacksmith?
        /// Does not check upgrade level.
        /// </summary>
        /// <param name="toolData">The tool data.</param>
        public static bool IsToolUpgradable(ToolData toolData)
        {
            return toolData.ClassName == nameof(Axe)
                || toolData.ClassName == nameof(Pickaxe)
                || toolData.ClassName == nameof(Hoe)
                || toolData.ClassName == nameof(WateringCan)
                || toolData.ClassName == nameof(Pan)
                || IsTrashCan(toolData);
        }

        /// <summary>Is the tool with the associated <see cref="ToolData"/> at its base upgrade level?</summary>
        /// <param name="toolData">The tool data.</param>
        public static bool IsToolBaseUpgradeLevel(ToolData toolData)
        {
            return toolData.UpgradeLevel < 1 || (toolData.UpgradeLevel < 2 && (toolData.ClassName == nameof(Pan) || IsTrashCan(toolData)));
        }

        /// <summary>Get the price for a tool upgrade (at the blacksmith shop) for the given upgrade level.</summary>
        /// <param name="level">The <see cref="Tool.UpgradeLevel"/> for a given tool.</param>
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
