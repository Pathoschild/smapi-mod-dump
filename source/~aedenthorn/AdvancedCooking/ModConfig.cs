/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/


using StardewModdingAPI;

namespace AdvancedCooking
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public SButton CookAllModKey { get; set; } = SButton.LeftShift;
        public bool StoreOtherHeldItemOnCook { get; set; } = true;
        public bool ConsumeIngredientsOnFail { get; set; } = false;
        public bool GiveTrashOnFail { get; set; } = true;
        public bool ConsumeExtraIngredientsOnSucceed { get; set; } = false;
        public bool AllowUnknownRecipes { get; set; } = true;
        public bool LearnUnknownRecipes { get; set; } = true;
        public bool ShowCookTooltip { get; set; } = true;
        public bool ShowProductsInTooltip { get; set; } = true;
        public bool ShowProductInfo { get; set; } = true;
        public int MaxTypesInTooltip { get; set; } = 3;
        public int YOffset { get; set; } = 532;
    }
}
