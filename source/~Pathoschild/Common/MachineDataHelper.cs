/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;

namespace Pathoschild.Stardew.Common
{
    /// <summary>Provides utility methods for working with machine data in <c>Data/Buildings</c> and <c>Data/Machines</c>.</summary>
    public static class MachineDataHelper
    {
        /*********
        ** Building data
        *********/
        /// <summary>Get the building chest names referenced by a building's item conversion rules.</summary>
        /// <param name="data">The building data.</param>
        /// <param name="inputChests">The input chest names found in the conversion rules.</param>
        /// <param name="outputChests">The output chest names found in the conversion rules.</param>
        public static void GetBuildingChestNames(BuildingData? data, ISet<string> inputChests, ISet<string> outputChests)
        {
            if (data?.ItemConversions?.Count is not > 0)
                return;

            foreach (BuildingItemConversion? rule in data.ItemConversions)
            {
                if (rule?.SourceChest is not null)
                    inputChests.Add(rule.SourceChest);

                if (rule?.DestinationChest is not null)
                    outputChests.Add(rule.DestinationChest);
            }
        }

        /// <summary>Get the building chest names referenced by a building's item conversion rules.</summary>
        /// <param name="data">The building data.</param>
        /// <param name="inputChests">The input chest names found in the conversion rules.</param>
        /// <param name="outputChests">The output chest names found in the conversion rules.</param>
        /// <returns>Returns whether any input or output chests were found.</returns>
        public static bool TryGetBuildingChestNames(BuildingData? data, out ISet<string> inputChests, out ISet<string> outputChests)
        {
            inputChests = new HashSet<string>();
            outputChests = new HashSet<string>();

            MachineDataHelper.GetBuildingChestNames(data, inputChests, outputChests);

            return inputChests.Count > 0 || outputChests.Count > 0;
        }

        /// <summary>Get the building chests which match a set of chest names.</summary>
        /// <param name="building">The building whose chests to get.</param>
        /// <param name="chestNames">The chest names to match.</param>
        public static IEnumerable<Chest> GetBuildingChests(Building building, ISet<string> chestNames)
        {
            foreach (Chest chest in building.buildingChests)
            {
                if (chestNames.Contains(chest.Name))
                    yield return chest;
            }
        }


        /*********
        ** Context tags
        *********/
        /// <summary>Get the item data matching a context tag, if the tag uniquely identifies one.</summary>
        /// <param name="contextTag">The context tag to match.</param>
        /// <param name="data">The item data, if found.</param>
        /// <returns>Returns whether the <paramref name="data"/> was set to a unique item match.</returns>
        public static bool TryGetUniqueItemFromContextTag(string contextTag, [NotNullWhen(true)] out ParsedItemData? data)
        {
            if (contextTag.StartsWith("id_"))
            {
                string rawIdentifier = contextTag[3..];
                string? qualifiedId = null;

                // extract qualified item ID
                if (rawIdentifier.StartsWith('('))
                    qualifiedId = rawIdentifier;
                else
                {
                    string[] parts = rawIdentifier.Split('_', 2);

                    foreach (IItemDataDefinition type in ItemRegistry.ItemTypes)
                    {
                        if (string.Equals(parts[0], type.StandardDescriptor, StringComparison.InvariantCultureIgnoreCase))
                        {
                            qualifiedId = type.Identifier + parts[1];
                            break;
                        }
                    }
                }

                // get data if valid
                data = ItemRegistry.GetData(qualifiedId);
                return data != null;
            }

            data = null;
            return false;
        }
    }
}
