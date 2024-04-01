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
using StardewModdingAPI.Utilities;

namespace CropStacking
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool CombineColored { get; set; } = true;
        public bool CombinePreserves { get; set; } = true;
        public bool CombineQualities { get; set; } = true;
        public KeybindList CombineKey { get; set; } = new KeybindList(SButton.MouseMiddle);
    }
}
