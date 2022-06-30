/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewNametags
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace StardewNametags
{
    public class ModConfig
    {
        public bool MultiplayerOnly { get; set; } = false;
        public KeybindList ToggleKey { get; set; } = KeybindList.Parse("F1");

        public string TextColor { get; set; } = "#FFFFFF";

        public string BackgroundColor { get; set; } = "#000000";

        public float BackgroundOpacity { get; set; } = 0.6f;

        public bool AlsoApplyOpacityToText { get; set; } = false;
    }
}
