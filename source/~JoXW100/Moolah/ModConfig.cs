/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/


using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace Moolah
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool Debug { get; set; } = false;
        public string Separator { get; set; } = ",";
        public int SeparatorInterval { get; set; } = 3;
        public int SeparatorX { get; set; } = 12;
        public int SeparatorY { get; set; } = -4;
    }
}
