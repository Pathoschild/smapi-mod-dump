/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AlejandroAkbal/Stardew-Valley-Quick-Sell-Mod
**
*************************************************/

using StardewModdingAPI;

namespace Quick_Sell
{
    internal class ModConfig
    {
        // public KeybindList SellKey { get; set; } = KeybindList.ForSingle(SButton.MouseMiddle);
        public SButton SellKey { get; set; } = SButton.MouseMiddle;

        //public SButton ModifierKey { get; set; } = SButton.LeftShift;

        public bool CheckIfItemsCanBeShipped { get; set; } = true;

        public bool EnableHUDMessages { get; set; } = false;
    }
}