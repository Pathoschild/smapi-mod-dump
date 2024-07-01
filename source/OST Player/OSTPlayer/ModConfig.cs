/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ProfeJavix/StardewValleyMods
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace OSTPlayer
{
    public sealed class ModConfig
    {
        public bool ModEnabled { get; set; }
        public KeybindList ToggleKey { get; set; }
        public bool ProgressiveMode{get;set;}
        public KeybindList SkipKey {get; set;}
        public bool RandomSkip {get; set;}
        public KeybindList StopKey {get; set;}

        public ModConfig() 
        { 
            ModEnabled = true;
            ToggleKey = KeybindList.Parse("Insert");
            ProgressiveMode = true;
            SkipKey = KeybindList.Parse("P");
            RandomSkip = false;
            StopKey = KeybindList.Parse("O");
        }

    }
}
