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
using StardewModdingAPI.Utilities;

namespace Quick_Sell
{
    public class ModConfig
    {
        public KeybindList SellKey { get; set; } = KeybindList.Parse("MouseMiddle, LeftStick");

        //public SButton ModifierKey { get; set; } = SButton.LeftShift;

        public bool CheckIfItemsCanBeShipped { get; set; } = true;

        public bool EnableHUDMessages { get; set; } = false;
    }
}