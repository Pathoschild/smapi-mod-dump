/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using TehPers.Core.Api.DI;
using TehPers.FishingOverhaul.Api.Effects;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// An entry for an effect that may be applied while fishing.
    /// </summary>
    public abstract record FishingEffectEntry
    {
        /// <summary>
        /// Conditions for when this effect should be applied.
        /// </summary>
        public AvailabilityConditions Conditions { get; init; } = new();

        /// <summary>
        /// Creates the fishing effect associated with this entry.
        /// </summary>
        /// <param name="kernel">The global kernel.</param>
        /// <returns>The fishing effect associated with this entry.</returns>
        public abstract IFishingEffect CreateEffect(IGlobalKernel kernel);
    }
}
