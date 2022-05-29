/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using StardewValley;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Api.Events;
using TehPers.FishingOverhaul.Api.Weighted;

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// API for working with fishing.
    /// </summary>
    public interface IFishingApi : ISimplifiedFishingApi
    {
        /// <summary>
        /// Invoked after an item is caught from the water.
        /// </summary>
        event EventHandler<CaughtItemEventArgs>? CaughtItem;

        /// <summary>
        /// Invoked before a treasure chest is opened.
        /// </summary>
        event EventHandler<OpeningChestEventArgs>? OpeningChest;

        /// <summary>
        /// Invoked whenever an item is caught and raises a custom event.
        /// </summary>
        event EventHandler<CustomEventArgs>? CustomEvent;

        /// <summary>
        /// Invoked after the default fishing info is created.
        /// </summary>
        event EventHandler<CreatedDefaultFishingInfoEventArgs>? CreatedDefaultFishingInfo;

        /// <summary>
        /// Invoked after fish chances are calculated. This is invoked at the end of
        /// <see cref="IFishingApi.GetFishChances"/>.
        /// </summary>
        event EventHandler<PreparedFishEventArgs>? PreparedFishChances;

        /// <summary>
        /// Invoked after trash chances are calculated. This is invoked at the end of
        /// <see cref="IFishingApi.GetTrashChances"/>.
        /// </summary>
        event EventHandler<PreparedTrashEventArgs>? PreparedTrashChances;

        /// <summary>
        /// Invoked after treasure chances are calculated. This is invoked at the end of
        /// <see cref="IFishingApi.GetTreasureChances"/>.
        /// </summary>
        event EventHandler<PreparedTreasureEventArgs>? PreparedTreasureChances;

        /// <summary>
        /// Invoked after the chance of catching a fish (instead of trash) is calculated. This is
        /// invoked at the end of <see cref="IFishingApi.GetChanceForFish(FishingInfo)"/>.
        /// </summary>
        event EventHandler<CalculatedFishChanceEventArgs>? CalculatedFishChance;

        /// <summary>
        /// Invoked after the minimum chance of catching a fish (instead of trash) is calculated.
        /// </summary>
        event EventHandler<CalculatedFishChanceEventArgs>? CalculatedMinFishChance;

        /// <summary>
        /// Invoked after the maximum chance of catching a fish (instead of trash) is calculated.
        /// </summary>
        event EventHandler<CalculatedFishChanceEventArgs>? CalculatedMaxFishChance;

        /// <summary>
        /// Invoked after the chance of finding a treasure chest is calculated. This is invoked at
        /// the end of <see cref="IFishingApi.GetChanceForTreasure(FishingInfo)"/>.
        /// </summary>
        event EventHandler<CalculatedTreasureChanceEventArgs>? CalculatedTreasureChance;

        /// <summary>
        /// Invoked after the minimum chance of finding a treasure chest is calculated.
        /// </summary>
        event EventHandler<CalculatedTreasureChanceEventArgs>? CalculatedMinTreasureChance;

        /// <summary>
        /// Invoked after the maximum chance of finding a treasure chest is calculated.
        /// </summary>
        event EventHandler<CalculatedTreasureChanceEventArgs>? CalculatedMaxTreasureChance;

        /// <summary>
        /// Creates a default <see cref="FishingInfo"/> for a farmer.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
        /// <returns>A default <see cref="FishingInfo"/> for that farmer.</returns>
        FishingInfo CreateDefaultFishingInfo(Farmer farmer);

        /// <summary>
        /// Gets the weighted chances of catching any fish. This does not take into account fish
        /// ponds.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <returns>The catchable fish and their chances of being caught.</returns>
        IEnumerable<IWeightedValue<FishEntry>> GetFishChances(FishingInfo fishingInfo);

        /// <summary>
        /// Gets the fish from a <see cref="FishPond"/> at the given tile if possible.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
        /// <param name="bobberTile">The tile the bobber is on.</param>
        /// <param name="takeFish">If <see langword="false"/>, simulates taking the fish. Otherwise, actually pulls the fish from the pond.</param>
        /// <returns>The fish to get from the pond, if any.</returns>
        NamespacedKey? GetFishPondFish(Farmer farmer, Vector2 bobberTile, bool takeFish = false);

        /// <summary>
        /// Gets the weighted chances of catching any trash.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <returns>The catchable trash and their chances of being caught.</returns>
        IEnumerable<IWeightedValue<TrashEntry>> GetTrashChances(FishingInfo fishingInfo);

        /// <summary>
        /// Gets the weighted chances of catching any treasure.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <returns>The catchable treasure and their chances of being caught.</returns>
        IEnumerable<IWeightedValue<TreasureEntry>> GetTreasureChances(FishingInfo fishingInfo);

        /// <summary>
        /// Gets the chance that a fish would be caught. This does not take into account whether
        /// there are actually fish to catch at the <see cref="Farmer"/>'s location. If no fish
        /// can be caught, then the <see cref="Farmer"/> will always catch trash.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <returns>The chance a fish would be caught instead of trash.</returns>
        double GetChanceForFish(FishingInfo fishingInfo);

        /// <summary>
        /// Gets the chance that treasure will be found during the fishing minigame.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <returns>The chance for treasure to appear during the fishing minigame.</returns>
        double GetChanceForTreasure(FishingInfo fishingInfo);

        /// <summary>
        /// Tries to get the traits for a fish.
        /// </summary>
        /// <param name="fishKey">The fish's <see cref="NamespacedKey"/>.</param>
        /// <param name="traits">The fish's traits.</param>
        /// <returns><see langword="true"/> if the fish has registered traits, otherwise <see langword="false"/>.</returns>
        bool TryGetFishTraits(NamespacedKey fishKey, [NotNullWhen(true)] out FishTraits? traits);

        /// <summary>
        /// Gets whether a fish is legendary.
        /// </summary>
        /// <param name="fishKey">The item key of the fish.</param>
        /// <returns>Whether that fish is legendary.</returns>
        bool IsLegendary(NamespacedKey fishKey);

        /// <summary>
        /// Selects a random catch. A player may catch either a fish or trash item.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <returns>A possible catch.</returns>
        PossibleCatch GetPossibleCatch(FishingInfo fishingInfo);

        /// <summary>
        /// Selects random treasure.
        /// </summary>
        /// <param name="catchInfo">Information about the caught fish.</param>
        /// <returns>Possible loot from a treasure chest.</returns>
        IEnumerable<TreasureEntry> GetPossibleTreasure(CatchInfo.FishCatch catchInfo);

        /// <summary>
        /// Selects random treasure.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <returns>Possible loot from a treasure chest.</returns>
        [Obsolete(
            "This overload will be removed soon. Pass a "
            + nameof(CatchInfo.FishCatch)
            + " instead."
        )]
        IEnumerable<TreasureEntry> GetPossibleTreasure(FishingInfo fishingInfo);

        /// <summary>
        /// Raises a custom event for other mods to handle.
        /// </summary>
        /// <param name="customEventArgs">The event to raise.</param>
        void RaiseCustomEvent(CustomEventArgs customEventArgs);

        /// <summary>
        /// Requests fishing data to be reloaded.
        /// </summary>
        void RequestReload();
    }
}
