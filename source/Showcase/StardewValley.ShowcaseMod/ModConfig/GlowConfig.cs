/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

using System.Collections.Generic;
using Igorious.StardewValley.ShowcaseMod.Data;

namespace Igorious.StardewValley.ShowcaseMod.ModConfig
{
    public sealed class GlowConfig
    {
        public bool ShowGlows { get; set; } = true;
        public bool ShowLights { get; set; } = true;

        public List<GlowEffect> Glows { get; set; } = new List<GlowEffect>();
        public GlowEffect GoldQualityGlow { get; set; }
        public GlowEffect IridiumQualityGlow { get; set; }
    }
}