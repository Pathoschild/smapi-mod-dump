/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Collections.Generic;

namespace WeatherTotems
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public string InvokeSound { get; set; } = "debuffSpell";
        public string SunnySound { get; set; } = "yoba";
        public string ThunderSound{ get; set; } = "thunder";
        public string RainSound { get; set; } = "rainsound";
        public string CloudySound { get; set; } = "ghost";
        public string SnowSound { get; set; } = "coldSpell";
    }
}
