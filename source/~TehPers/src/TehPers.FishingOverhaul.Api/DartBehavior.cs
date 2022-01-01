/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// The movement behavior of the fish in the minigame.
    /// </summary>
    public enum DartBehavior
    {
        /// <summary>
        /// Maps to "mixed" vanilla behavior.
        /// </summary>
        Mixed,
        /// <summary>
        /// Maps to "dart" vanilla behavior.
        /// </summary>
        Dart,
        /// <summary>
        /// Maps to "smooth" vanilla behavior.
        /// </summary>
        Smooth,
        /// <summary>
        /// Maps to "sink" vanilla behavior.
        /// </summary>
        Sink,
        /// <summary>
        /// Maps to "floater" vanilla behavior.
        /// </summary>
        Floater,
    }
}