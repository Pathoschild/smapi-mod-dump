/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/noriteway/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace MoreSettings
{
    public sealed class ModConfig
    {
        public SButton MuteKey { get; set; } = StardewModdingAPI.SButton.K;
        public int MuteButtonIndex { get; set; } = 16;
        public SButton FullscreenKey { get; set; } = StardewModdingAPI.SButton.F11;
        public bool UseWindowedBorderless { get; set; } = true;
    }
}
