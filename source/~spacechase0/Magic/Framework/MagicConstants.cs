/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

namespace Magic.Framework
{
    /// <summary>Defines constants for the magic system.</summary>
    internal class MagicConstants
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The number of spell bar which players are expected to have.</summary>
        public static int SpellBarCount { get; } = 2;

        /// <summary>The ID of the event in which the player learns magic from the Wizard.</summary>
        public static int LearnedMagicEventId { get; } = 90001;

        /// <summary>The number of mana points gained per magic level.</summary>
        public static int ManaPointsPerLevel { get; } = 100;
    }
}
