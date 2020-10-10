/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-TimeOfThePrairieKing
**
*************************************************/

using TimeOfThePrairieKingMod.Config.ScreenLocations;

namespace TimeOfThePrairieKingMod.Config
{
    /// <summary>
    /// User configuration options set in config.json
    /// </summary>
    public class UserConfig
    {
        /// <summary>
        /// Whether or not the mod is enabled
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The number of pixels to ident the time (for corners only)
        /// </summary>
        public float Indentation { get; set; } = 25f;

        /// <summary>
        /// Configuration for drawing on the minigame's HUD
        /// </summary>
        public MinigameHud MinigameHud { get; set; } = new MinigameHud();

        /// <summary>
        /// Configuration for drawing in the top left corner
        /// </summary>
        public TopLeft TopLeft { get; set; } = new TopLeft();

        /// <summary>
        /// Configuration for drawing in the top right corner
        /// </summary>
        public TopRight TopRight { get; set; } = new TopRight();

        /// <summary>
        /// Configuration for drawing in the bottom left corner
        /// </summary>
        public BottomLeft BottomLeft { get; set; } = new BottomLeft();

        /// <summary>
        /// Configuration for drawing in the bottom right corner
        /// </summary>
        public BottomRight BottomRight { get; set; } = new BottomRight();
    }
}
