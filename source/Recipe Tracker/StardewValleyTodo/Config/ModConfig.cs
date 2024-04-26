/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NoxChimaera/StardewValleyTODO
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace StardewValleyTodo.Config {
    public sealed class ModConfig {
        public int VerticalOffset { get; set; } = 0;

        public KeybindList ToggleVisibility { get; set; } = KeybindList.ForSingle(SButton.Z);
        public KeybindList ClearTracker { get; set; } = KeybindList.ForSingle(SButton.Z, SButton.LeftShift);
        public KeybindList TrackItem { get; set; } = KeybindList.ForSingle(SButton.Z);
    }
}
