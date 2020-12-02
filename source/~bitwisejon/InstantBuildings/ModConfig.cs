/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bitwisejon/StardewValleyMods
**
*************************************************/

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
        /// Button to initiate the house/cabin upgrade
        /// </summary>
        public SButton PerformInstantHouseUpgradeButton { get; set; } = SButton.U;

        /// <summary>
        /// If false, will only allow the Magician buildings to be built once the player has the magic ink. Set to true for "sandbox" mode.
        /// </summary>
        public bool AllowMagicalBuildingsWithoutMagicInk { get; set; } = false;
    }
}
