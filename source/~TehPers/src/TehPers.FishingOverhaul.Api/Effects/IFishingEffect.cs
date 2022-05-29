/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace TehPers.FishingOverhaul.Api.Effects
{
    /// <summary>
    /// An effect that can be applied while fishing.
    /// </summary>
    public interface IFishingEffect
    {
        /// <summary>
        /// Applies this effect.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        void Apply(FishingInfo fishingInfo);

        /// <summary>
        /// Unapplies this effect.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        void Unapply(FishingInfo fishingInfo);

        /// <summary>
        /// Unapplies this effect from all players.
        /// </summary>
        void UnapplyAll();
    }
}
