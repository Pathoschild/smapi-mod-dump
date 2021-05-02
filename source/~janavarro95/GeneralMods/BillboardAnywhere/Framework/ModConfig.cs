/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace Omegasis.BillboardAnywhere.Framework
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /// <summary>The key which shows the billboard menu.</summary>
        public SButton CalendarKeyBinding { get; set; } = SButton.B;
        /// <summary>The key which shows the quest menu.</summary>
        public SButton QuestBoardKeyBinding { get; set; } = SButton.H;
        /// <summary>The offset for the calendar button from the active menu</summary>
        public Vector2 CalendarOffsetFromMenu { get; set; } = new Vector2(-100, 0);
        /// <summary>The offset for the quest button from the active menu</summary>
        public Vector2 QuestOffsetFromMenu { get; set; } = new Vector2(-200, 0);

        /// <summary>
        /// If true the calendar button is enabled for the in-game menu.
        /// </summary>
        public bool EnableInventoryCalendarButton { get; set; } = true;
        /// <summary>
        /// If true the quest button is enabled for the in-game menu.
        /// </summary>
        public bool EnableInventoryQuestButton { get; set; } = true;
    }
}
