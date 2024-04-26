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
using System.Collections.Generic;

namespace BatForm
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool NightOnly { get; set; } = false;
        public bool OutdoorsOnly { get; set; } = true;
        public KeybindList TransformKey { get; set; } = new KeybindList(SButton.NumPad5);
        public string TransformSound { get; set; } = "cowboy_explosion";
        public int MoveSpeed { get; set; } = 10;
        public int StaminaUse { get; set; } = 0;
        public int MaxHeight { get; set; } = 50;
    }
}
