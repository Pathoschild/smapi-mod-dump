/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/


using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace AdvancedMenuPositioning
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public KeybindList MoveKeys { get; set; } = new KeybindList(new Keybind(SButton.LeftShift, SButton.MouseLeft));
        public KeybindList DetachKeys { get; set; } = new KeybindList(new Keybind(SButton.LeftShift, SButton.X));
        public KeybindList CloseKeys { get; set; } = new KeybindList(new Keybind(SButton.LeftShift, SButton.Z));
        public bool StrictKeybindings { get; set; } = true;
    }
}