/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mikesnorth/StardewNewsFeed
**
*************************************************/


namespace StardewNewsFeed {
    public class ModConfig {
        public bool GreenhouseNotificationsEnabled { get; set; } = true;
        public bool CaveNotificationsEnabled { get; set; } = true;
        public bool CellarNotificationsEnabled { get; set; } = true;
        public bool ShedNotificationsEnabled { get; set; } = true;
        public bool BirthdayCheckEnabled { get; set; } = true;
        public bool CoopCheckEnabled { get; set; } = true;
        public bool BarnCheckEnabled { get; set; } = true;
        public bool SiloCheckEnabled { get; set; } = true;
    }
}
