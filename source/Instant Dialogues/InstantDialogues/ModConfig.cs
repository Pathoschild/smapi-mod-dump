/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/idermailer/InstantDialogues
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace InstantDialogues
{
    internal class ModConfig
    {
        public bool EntireModEnabled { get; set; } = true;
        public bool UseToggleKey { get; set; } = true;
        public KeybindList ToggleKey { get; set; } = KeybindList.Parse("OemPeriod");
        public bool EnableOnLaunchWhenUseToggleKey { get; set; } = true;
        public int SkipPreventTimeWhileModOn { get; set; } = 750;
    }
}
