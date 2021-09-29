/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

namespace JsonAssets.Framework
{
    /// <summary>A set of parsed in-game requirements.</summary>
    public interface IParsedConditions
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Whether there are any conditions.</summary>
        bool HasConditions { get; }

        /// <summary>Whether Expanded Preconditions Utility is needed to handle the conditions.</summary>
        bool NeedsExpandedPreconditionsUtility { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Get the current result of the conditions.</summary>
        bool CurrentlyMatch();
    }
}
