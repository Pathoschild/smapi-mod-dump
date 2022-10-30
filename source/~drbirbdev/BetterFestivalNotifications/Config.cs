/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using BirbShared.Config;

namespace BetterFestivalNotifications
{
    [ConfigClass]
    internal class Config
    {
        [ConfigOption]
        public bool PlayStartSound { get; set; } = true;
        [ConfigOption]
        public string StartSound { get; set; } = "crystal";

        [ConfigOption]
        public bool PlayWarnSound { get; set; } = true;

        [ConfigOption]
        public bool ShowWarnNotification { get; set; } = true;

        [ConfigOption]
        public string WarnSound { get; set; } = "phone";

        [ConfigOption]
        public bool PlayOverSound { get; set; } = false;

        [ConfigOption]
        public bool ShowOverNotification { get; set; } = false;

        [ConfigOption]
        public string OverSound { get; set; } = "ghost";
    }
}
