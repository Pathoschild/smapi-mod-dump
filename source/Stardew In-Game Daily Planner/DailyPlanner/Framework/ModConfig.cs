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
