/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MissCoriel/Event-Repeater
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace EventRepeater
{
    internal sealed class ConfigModel
    {
        //public SButton EventWindow { get; set; } = SButton.Pause;
        public KeybindList EmergencySkip { get; set; } = KeybindList.Parse("LeftControl + S");
        public KeybindList ShowInfo { get; set; } = KeybindList.Parse("LeftControl + I");
        public KeybindList NormalSkip { get; set; } = KeybindList.Parse("LeftAlt + S");

    }
}
