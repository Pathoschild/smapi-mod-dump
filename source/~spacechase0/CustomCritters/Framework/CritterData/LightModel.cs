/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

namespace CustomCritters.Framework.CritterData
{
    internal class LightModel
    {
        public int VanillaLightId = 3;
        public float Radius { get; set; } = 0.5f;
        public LightColorModel Color { get; set; } = new();
    }
}
