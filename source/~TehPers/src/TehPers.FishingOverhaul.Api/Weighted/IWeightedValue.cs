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
    /// <summary>A wrapper for objects that assigns a weighted chance to them.</summary>
    /// <typeparam name="T">The type of object being wrapped.</typeparam>
    public interface IWeightedValue<out T> : IWeighted
    {
        /// <summary>Gets the value wrapped by this element.</summary>
        T Value { get; }
    }
}