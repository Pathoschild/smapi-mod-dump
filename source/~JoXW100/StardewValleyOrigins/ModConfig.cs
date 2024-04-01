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

namespace StardewValleyOrigins
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public KeybindList ToggleKey { get; set; } = new KeybindList(SButton.NumPad1);
        public string EnableSound { get; set; } = "yoba";
        public int MaxLength { get; set; } = 50;
        public int MoveSpeed { get; set; } = 10;
        public int StaminaUse { get; set; } = 0;
    }
}
