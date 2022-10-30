/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zeldela/sdv-mods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Utilities;

namespace JustLuckMod
{
    internal class ModConfig
    {
        public bool Disable { get; set; } = false;
        public bool Fortune { get; set; } = true;
        public KeybindList Toggle { get; set; } = KeybindList.Parse("LeftShift + L");
        public bool Monochrome { get; set; } = false;
        public string Location { get; set; } = "HUD";
    }
}
