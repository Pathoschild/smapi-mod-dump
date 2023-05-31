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
        public SButton OpenMenuKey { get; set; } = SButton.OemOpenBrackets;
        public SButton SecondaryControllerKey { get; set; } = SButton.None;

        /****
        ** Menu settings
        ****/
        /// <summary>The tab shown by default when you open the menu.</summary>
        public MenuTab DefaultTab { get; set; } = MenuTab.Daily;
        public bool ShowOverlay { get; set; } = true;
        public bool ShowPlannerTasks { get; set; } = true;
        public bool ShowChecklistTasks { get; set; } = false;
        public float OverlayBackgroundOpacity { get; set; } = 0.7F;
        public float OverlayTextOpacity { get; set; } = 1.0F;
        public int OverlayMaxLength { get; set; } = 25;
        public int OverlayMaxLines { get; set; } = 10;
        public int OverlayXBufferPercent { get; set; } = 0;
        public int OverlayYBufferPercent { get;set; } = 0;
    }
}
