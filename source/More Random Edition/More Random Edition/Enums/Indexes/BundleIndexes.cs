/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

namespace Randomizer
{
    /// <summary>
    /// The indexes in the Data/Bundles.xnb dictionary
    /// </summary>
    public enum BundleIndexes
    {
        /// <summary>
        /// The old name of the bundle; unsure if it's used for anything, so leave it alone
        /// </summary>
        BundleId = 0,

        /// <summary>
        /// Type / ID / Amount
        /// Ex: O 495 30
        /// => Object, spring seeds, 30 of them
        /// </summary>
        Reward = 1,

        /// <summary>
        /// ID / Amount needed / Min quality (0, 1, 2, 3 = any, silver, gold, iridium)
        /// Ex: 284 1 1
        /// => Beet, 1 required, at least silver quality
        /// </summary>
        RequiredItems = 2,

        /// <summary>
        /// Integer value (represented by BundleColor.cs)
        /// </summary>
        ColorIndex = 3,

        /// <summary>
        /// Number of items required to complete the bundle
        /// If left blank, all items are required
        /// </summary>
        MinimumRequiredItems = 4,

        /// <summary>
        /// The old display name that was used before - this is now unsued!
        /// </summary>
        OldDisplayName = 5,

        /// <summary>
        /// The display name for the bundle - works in any language now
        /// </summary>
        DisplayName = 6
    }
}
