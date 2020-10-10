/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheThor59/StardewMods
**
*************************************************/

namespace Thor.Stardew.Mods.HealthBars
{
    /// <summary>
    /// Mod Configration class
    /// </summary>
    public class ModConfig
    {
        /// <summary>
        /// If true, the life counter needs XP + killcount of mobs to show the life level
        /// If false, always show the life level
        /// </summary>
        public bool EnableXPNeeded { get; set; }

        /// <summary>
        /// If true, the life text info is hidden
        /// If false, the text info appear
        /// </summary>
        public bool HideTextInfo { get; set; }

        /// <summary>
        /// If true, the life is hidden if mob has full life
        /// If false, the life is shown at all time
        /// </summary>
        public bool HideFullLifeBar { get; set; }

        /// <summary>
        /// Allow selecting the color scheme of the life bar
        /// </summary>
        public int ColorScheme { get; set; }

        /// <summary>
        /// Initialization of default values
        /// </summary>
        public ModConfig()
        {
            EnableXPNeeded = true;
            ColorScheme = 0;
            HideTextInfo = false;
            HideFullLifeBar = false;
        }
    }
}
