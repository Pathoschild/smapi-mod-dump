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

namespace FurnitureRecolor
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;

        public SButton RedButton { get; set; } = SButton.NumPad1;
        public SButton GreenButton { get; set; } = SButton.NumPad7;
        public SButton BlueButton { get; set; } = SButton.NumPad9;
        public SButton ResetButton { get; set; } = SButton.NumPad3;

        public SButton ModKey { get; set; } = SButton.LeftAlt;
    }
}
