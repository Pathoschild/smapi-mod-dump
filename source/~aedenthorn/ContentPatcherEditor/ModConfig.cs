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
using StardewModdingAPI.Utilities;
using System.Collections.Generic;

namespace ContentPatcherEditor
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool ShowButton { get; set; } = true;
        public bool Backup { get; set; } = true;
        public bool OpenModsFolderAfterZip { get; set; } = true;
        public KeybindList MenuButton { get; set; } = new KeybindList(new Keybind(SButton.LeftShift, SButton.OemCloseBrackets));
    }
}
