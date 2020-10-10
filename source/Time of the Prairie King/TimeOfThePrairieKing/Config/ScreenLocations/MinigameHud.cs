/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-TimeOfThePrairieKing
**
*************************************************/

namespace TimeOfThePrairieKingMod.Config.ScreenLocations
{
    /// <summary>
    /// User configuration for drawing the time on the minigame's HUD
    /// </summary>
    public class MinigameHud
    {
        /// <summary>
        /// Whether or not to draw the time on the minigame's HUD (defaults true)
        /// </summary>
        public bool Show { get; set; } = true;

        /// <summary>
        /// The color to draw the time in
        /// </summary>
        public string HexColor { get; set; } = "#CD853F";
    }
}
