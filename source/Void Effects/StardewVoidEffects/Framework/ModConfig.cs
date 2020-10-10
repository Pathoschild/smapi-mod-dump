/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/OfficialRenny/StardewVoidEffects
**
*************************************************/

namespace StardewVoidEffects.Framework
{
    internal class ModConfig
    {
        public bool modEnabledOnStartup { get; set; } = true;
        public float VoidItemPriceIncrease { get; set; } = 2.0f;
        public int VoidDecay { get; set; } = 10;
    }
}
