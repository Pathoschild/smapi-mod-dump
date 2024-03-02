/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Linq;
using System.Collections.Generic;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants.Modded;

namespace StardewArchipelago.Stardew.NameMapping
{
    public class CompoundNameMapper : INameMapper, IRecipeNameMapper
    {
        private List<INameMapper> _mappers;
        private List<IRecipeNameMapper> _recipeMappers;

        public CompoundNameMapper(SlotData slotData)
        {
            _mappers = new List<INameMapper>();
            _recipeMappers = new List<IRecipeNameMapper>();

            // This one is not the same type of mapping
            var craftingRecipeMapper = new CraftingRecipeNameMapper();
            _recipeMappers.Add(craftingRecipeMapper);

            if (slotData.Mods.HasMod(ModNames.ARCHAEOLOGY))
            {
                var archaeologyMapper = new ArchaeologyNameMapper();
                _mappers.Add(archaeologyMapper);
                _recipeMappers.Add(archaeologyMapper);
            }
        }

        public string GetEnglishName(string internalName)
        {
            return _mappers.Aggregate(internalName, (current, nameMapper) => nameMapper.GetEnglishName(current));
        }

        public string GetInternalName(string englishName)
        {
            return _mappers.Aggregate(englishName, (current, nameMapper) => nameMapper.GetInternalName(current));
        }

        public string GetItemName(string recipeName)
        {
            return _recipeMappers.Aggregate(recipeName, (current, nameMapper) => nameMapper.GetItemName(current));
        }

        public string GetRecipeName(string itemName)
        {
            return _recipeMappers.Aggregate(itemName, (current, nameMapper) => nameMapper.GetRecipeName(current));
        }

        public bool RecipeNeedsMapping(string itemName)
        {
            return _recipeMappers.Any(x => x.RecipeNeedsMapping(itemName));
        }
    }
}