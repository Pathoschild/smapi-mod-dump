/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace Magic
{
    public class Configuration
    {
        public SButton Key_SwapSpells = SButton.Tab;
        public SButton Key_Cast = SButton.Q;
        public SButton Key_Spell1 = SButton.D1;
        public SButton Key_Spell2 = SButton.D2;
        public SButton Key_Spell3 = SButton.D3;
        public SButton Key_Spell4 = SButton.D4;
        public SButton Key_Spell5 = SButton.D5;

        public string AltarLocation = "SeedShop";
        public int AltarX = 36;
        public int AltarY = 16;
    }
}
