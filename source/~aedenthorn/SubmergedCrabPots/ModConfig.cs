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

namespace SubmergedCrabPots
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool SubmergeHarvestable { get; set; } = true;
        public bool ShowRipples { get; set; } = true;
        public Color BobberTint { get; set; } = Color.White;
        public float BobberScale { get; set; } = 4f;
        public int BobberOpacity { get; set; } = 100;
    }
}
