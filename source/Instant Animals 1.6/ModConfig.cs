/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/siweipancc/InstantAnimals
**
*************************************************/

using StardewModdingAPI;

namespace InstantAnimals
{
    public class ModConfig
    {
        /// <summary>
        /// Indicates if animals should cost their usual resources.
        /// </summary>
        public bool BuyUsesResources { get; set; } = true;

        /// <summary>
        /// Button to open and close the Instant Animals menu
        /// </summary>
        public SButton ToggleInstantBuyMenuButton { get; set; } = SButton.J;

    }
}