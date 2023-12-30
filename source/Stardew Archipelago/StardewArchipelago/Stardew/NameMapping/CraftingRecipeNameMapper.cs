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

namespace StardewArchipelago.Stardew.NameMapping
{
    public class CraftingRecipeNameMapper : INameMapper
    {
        private static readonly Dictionary<string, string> _internalToEnglishNames = new(){
            {"Wild Seeds (Sp)", "Spring Seeds"},
            {"Wild Seeds (Su)", "Summer Seeds"},
            {"Wild Seeds (Fa)", "Fall Seeds"},
            {"Wild Seeds (Wi)", "Winter Seeds"},
        };

        private static readonly Dictionary<string, string> _englishToInternalNames =
            _internalToEnglishNames.ToDictionary(x => x.Value, x => x.Key);

        public CraftingRecipeNameMapper()
        {
        }

        public string GetEnglishName(string internalName)
        {
            return _internalToEnglishNames.ContainsKey(internalName) ? _internalToEnglishNames[internalName] : internalName;
        }

        public string GetInternalName(string englishName)
        {
            return _englishToInternalNames.ContainsKey(englishName) ? _englishToInternalNames[englishName] : englishName;
        }

        public bool RecipeNeedsMapping(string itemOfRecipe)
        {
            return _englishToInternalNames.ContainsKey(itemOfRecipe);
        }
    }
}