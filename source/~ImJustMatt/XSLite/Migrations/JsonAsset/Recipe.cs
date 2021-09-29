/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace XSLite.Migrations.JsonAsset
{
    using System.Collections.Generic;

    internal record Recipe
    {
        public int ResultCount { get; set; }

        public List<Ingredient> Ingredients { get; set; }

        public bool CanPurchase { get; set; } = false;

        public bool IsDefault { get; set; } = true;

        public string PurchaseFrom { get; set; } = null;

        public int PurchasePrice { get; set; } = 0;

        public string SkillUnlockName { get; set; } = null;

        public int SkillUnlockLevel { get; set; } = 0;
    }
}