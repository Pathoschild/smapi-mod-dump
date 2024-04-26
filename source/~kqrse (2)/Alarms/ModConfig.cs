/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System.Collections.Generic;

namespace Alarms
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public string DefaultSound { get; set; } = "rooster";
        public KeybindList MenuButton { get; set; } = new KeybindList(new Keybind(SButton.LeftShift, SButton.OemPipe));
    }
}
