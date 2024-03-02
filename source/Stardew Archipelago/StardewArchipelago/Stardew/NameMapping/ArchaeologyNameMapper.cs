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
    public class ArchaeologyNameMapper : INameMapper, IRecipeNameMapper
    {
        private static readonly Dictionary<string, string> ArchaeologyToEnglishNamesMap = new(){
            {"moonslime.excavation.ancient_battery", "Ancient Battery Production Station"},
            {"moonslime.excavation.glass_bazier", "Glass Bazier"},
            {"moonslime.excavation.grinder", "Grinder"},
            {"moonslime.excavation.preservation_chamber", "Preservation Chamber"},
            {"moonslime.excavation.h_preservation_chamber", "Hardwood Preservation Chamber"},
            {"moonslime.excavation.glass_fence", "Glass Fence"},
            {"moonslime.excavation.dummy_path_bone", "Bone Path"},
            {"moonslime.excavation.dummy_path_glass", "Glass Path"},
            {"moonslime.excavation.dummy_water_strainer", "Water Shifter"},
            {"moonslime.excavation.h_display", "Hardwood Display"},
            {"moonslime.excavation.w_display", "Wooden Display"},
            {"moonslime.excavation.totem_volcano_warp", "Dwarf Gadget: Infinite Volcano Simulation"}
        };

        private static readonly Dictionary<string, string> EnglishToArchaeologyNamesMap = ArchaeologyToEnglishNamesMap.ToDictionary(x => x.Value, x => x.Key);

        public ArchaeologyNameMapper()
        {
        }

        public string GetEnglishName(string internalName)
        {
            return ArchaeologyToEnglishNamesMap.ContainsKey(internalName) ? ArchaeologyToEnglishNamesMap[internalName] : internalName;
        }

        public string GetInternalName(string englishName)
        {
            return EnglishToArchaeologyNamesMap.ContainsKey(englishName) ? EnglishToArchaeologyNamesMap[englishName] : englishName;
        }

        public string GetItemName(string recipeName)
        {
            return GetEnglishName(recipeName);
        }

        public string GetRecipeName(string itemName)
        {
            return GetInternalName(itemName);
        }

        public bool RecipeNeedsMapping(string itemName)
        {
            return EnglishToArchaeologyNamesMap.ContainsKey(itemName);
        }
    }
}