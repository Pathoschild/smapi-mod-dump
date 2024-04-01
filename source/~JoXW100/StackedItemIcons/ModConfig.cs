/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace StackedItemIcons
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public int MinForDoubleStack { get; set; } = 10;
        public int MinForTripleStack { get; set; } = 100;
        public int Spacing { get; set; } = 10;
    }
}
