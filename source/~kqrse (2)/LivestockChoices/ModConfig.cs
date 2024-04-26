/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/


using StardewModdingAPI;

namespace LivestockChoices
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public int BlueChickenPrice { get; set; } = 500;
        public int VoidChickenPrice { get; set; } = 1000;
        public int GoldenChickenPrice { get; set; } = 5000;
    }
}
