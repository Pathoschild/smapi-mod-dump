/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

namespace TehPers.FishingOverhaul.Api.Weighted
{
    /// <summary>
    /// Defines a weighted chance for an object, allowing easy weighted choosing of a random
    /// element from a list of the object.
    /// </summary>
    public interface IWeighted
    {
        /// <summary>
        /// Gets the positive weighted chance of this object being selected (in comparison to other
        /// objects).
        /// </summary>
        double Weight { get; }
    }
}
