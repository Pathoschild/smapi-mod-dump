/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BuildABuddha/StardewDailyPlanner
**
*************************************************/

using StardewModdingAPI;
using DailyPlanner.Framework.Constants;

namespace DailyPlanner.Framework
{
    class ModConfig
    {
        /*********
        ** Accessors
        *********/

        /****
        ** Keyboard buttons
        ****/
        /// <summary>The keyboard button which opens the menu.</summary>
        public SButton OpenMenuKey { get; set; } = SButton.Tab;

        /****
        ** Menu settings
        ****/
        /// <summary>The tab shown by default when you open the menu.</summary>
        public MenuTab DefaultTab { get; set; } = MenuTab.Daily;
    }
}
