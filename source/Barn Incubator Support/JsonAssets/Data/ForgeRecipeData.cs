/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

namespace JsonAssets.Data
{
    public class ForgeRecipeData
    {
        public string EnableWithMod { get; set; }
        public string DisableWithMod { get; set; }

        public string BaseItemName { get; set; } // Checks by Item.Name, so supports anything
        public string IngredientContextTag { get; set; }
        public int CinderShardCost { get; set; }

        public string ResultItemName { get; set; } // Uses Utility.fuzzyItemSearch, so go nuts

        public string[] AbleToForgeConditions { get; set; }
    }
}
