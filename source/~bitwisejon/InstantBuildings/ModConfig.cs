using StardewModdingAPI;

namespace BitwiseJonMods
{
    public class ModConfig
    {
        /// <summary>
        /// Indicates if buildings should cost their usual resources. Set to false to build in "sandbox" mode.
        /// </summary>
        public bool BuildUsesResources { get; set; } = true;

        /// <summary>
        /// Button to open and close the Instant Build menu
        /// </summary>
        public SButton ToggleInstantBuildMenuButton { get; set; } = SButton.B;

        /// <summary>
        /// If false, will only allow the Magician buildings to be built once the player has the magic ink. Set to true for "sandbox" mode.
        /// </summary>
        public bool AllowMagicalBuildingsWithoutMagicInk { get; set; } = false;
    }
}
