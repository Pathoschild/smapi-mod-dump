/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-ModCollection
**
*************************************************/

namespace WhoIsStillAwakeMod
{
    public class UserConfig
    {
        /// <summary>
        /// Whether or not the mod is enabled
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The number of pixels to ident the drawing
        /// </summary>
        public float Indentation { get; set; } = 25f;

        /// <summary>
        /// The color to draw the numbers in
        /// </summary>
        public string HexColor { get; set; } = "#808080";

        /// <summary>
        /// Time to start displaying the numbers at
        /// </summary>
        public int TimeToStartDisplaying { get; set; } = 2300;

        /// <summary>
        /// Whether or not to show the numbers of farmers currently in bed (ex: 2/3)
        /// </summary>
        public bool ShowNumberOfFarmersInBed { get; set; } = true;

        /// <summary>
        /// Whether or not to draw while not playing an arcade game
        /// </summary>
        public bool ShowOutsideMinigames { get; set; } = true;

        /// <summary>
        /// Whether or not to draw while playing the Junimo Carts minigame
        /// </summary>
        public bool ShowWhilePlayingJunimoCarts { get; set; } = true;

        /// <summary>
        /// Whether or not to draw while playing the Journey of the Paririe King minigame
        /// </summary>
        public bool ShowWhilePlayingJourneyOfThePrairieKing { get; set; } = true;
    }
}
