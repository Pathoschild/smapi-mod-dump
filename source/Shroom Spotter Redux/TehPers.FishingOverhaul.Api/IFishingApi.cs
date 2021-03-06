/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;
using TehPers.Core.Api.Enums;
using TehPers.Core.Api.Weighted;

namespace TehPers.FishingOverhaul.Api {
    public interface IFishingApi {
        /// <summary>Invoked when a fish is hit, before the bobber bar appears.</summary>
        event EventHandler<FishingEventArgs> BeforeFishCatching;

        /// <summary>Invoked when a fish is caught, before the bobber bar is closed.</summary>
        event EventHandler<FishingEventArgs> FishCaught;

        /// <summary>Invoked when trash is caught, before the animation occurs.</summary>
        event EventHandler<FishingEventArgs> TrashCaught;

        /// <summary>Sets the chance of catching fish. This overrides any fish chance calculations done by the mod itself.</summary>
        /// <param name="chance">The new chance of catching fish, or null to remove any overrides.</param>
        void SetFishChance(float? chance);

        /// <summary>Gets the chance for a <see cref="Farmer"/> to catch a fish.</summary>
        /// <param name="who">The farmer.</param>
        /// <returns>The chance for the given <see cref="Farmer"/> to catch a fish.</returns>
        float GetFishChance(Farmer who);

        /// <summary>Sets the chance of finding treasure. This overrides any treasure chance calculations done by the mod itself.</summary>
        /// <param name="chance">The new chance of finding treasure, or null to remove any overrides.</param>
        void SetTreasureChance(float? chance);

        /// <summary>Gets the chance for a <see cref="Farmer"/> to find treasure while fishing.</summary>
        /// <param name="who">The farmer.</param>
        /// <param name="rod">The equipped fishing rod.</param>
        /// <returns>The chance for the given <see cref="Farmer"/> to find treasure with the given <see cref="FishingRod"/>.</returns>
        float GetTreasureChance(Farmer who, FishingRod rod);

        /// <summary>Sets the chance that a <see cref="Farmer"/> finds an unaware fish. Legendary fish cannot be unaware. This disables any unaware fish chance calculations done by the mod itself.</summary>
        /// <param name="chance">The new chance of finding unaware fish, or null to remove any overrides.</param>
        void SetUnawareChance(float? chance);

        /// <summary>Gets the chance for a <see cref="Farmer"/> to find an unaware fish.</summary>
        /// <param name="who">The farmer.</param>
        /// <param name="fish">The fish. Legendary fish cannot be unaware.</param>
        /// <returns>The chance for the given <see cref="Farmer"/> to find an unaware fish.</returns>
        float GetUnawareChance(Farmer who, int fish);

        /// <summary>Sets whether players can catch fish from the "Farm" location on their farm, regardless of what type of farm it is. To disable fishing on farms that normally allow fishing as well, see <see cref="SetFishableFarmFishing"/>.</summary>
        /// <param name="allowFish">True to allow fishing, false to disallow fishing, or null to remove any overrides.</param>
        void SetFarmFishing(bool? allowFish);

        /// <summary>Gets whether players can catch fish from the "Farm" location on their farm. For normally fishable farms, if <see cref="GetFishableFarmFishing"/> returns true, players can still fish on those farms.</summary>
        /// <returns>True if players can fish on their farm, false if not.</returns>
        bool GetFarmFishing();

        /// <summary>Sets whether players can catch fish on normally fishable farms. If <see cref="GetFarmFishing"/> returns true, fish can also be pulled from the "Farm" location regardless of this value.</summary>
        /// <param name="allowFish">True to allow fishing, false to disallow fishing, or null to remove any overrides.</param>
        void SetFishableFarmFishing(bool? allowFish);

        /// <summary>Gets whether players can catch fish on normally fishable farms. If <see cref="GetFarmFishing"/> returns true, fish will still be pulled from the "Farm" location.</summary>
        /// <returns></returns>
        bool GetFishableFarmFishing();

        /// <summary>Gets all the fish data at the given location.</summary>
        /// <param name="location">The location to get fish data for.</param>
        /// <returns>A <see cref="IReadOnlyDictionary{TKey,TValue}"/> containing.</returns>
        IReadOnlyDictionary<int, IFishData> GetFishData(string location);

        /// <summary>Gets the fish data associated with a particular location and fish.</summary>
        /// <param name="location">The location to get fish data for.</param>
        /// <param name="fish">The ID of the fish to get data for.</param>
        /// <returns>The data associated with the given fish in the given location.</returns>
        IFishData GetFishData(string location, int fish);

        /// <summary>Sets the data for a fish at a specific location.</summary>
        /// <param name="location">The name of the location.</param>
        /// <param name="fish">The fish which the data is associated with.</param>
        /// <param name="data">The data to assign to the fish at the given location, or null if the data should be removed.</param>
        void SetFishData(string location, int fish, IFishData data);

        /// <summary>Removes all fish data overrides.</summary>
        void ResetFishData();

        /// <summary>Removes all fish data overrides at the given location.</summary>
        /// <param name="location">The location to remove overrides for.</param>
        /// <returns>True if overrides were removed, false if not.</returns>
        bool ResetFishData(string location);

        /// <summary>Removes the fish data override for the given fish at the given location if one exists.</summary>
        /// <param name="location">The location to remove the override for.</param>
        /// <param name="fish">The fish to remove the override for.</param>
        /// <returns>True if an override was removed, false if not.</returns>
        bool ResetFishData(string location, int fish);

        /// <summary>Sets the traits of a fish.</summary>
        /// <param name="fish">The ID of the fish.</param>
        /// <param name="traits">The new traits of the fish, or null to remove any overrides.</param>
        void SetFishTraits(int fish, IFishTraits traits);

        /// <summary>Gets the traits of a fish.</summary>
        /// <param name="fish">The ID of the fish.</param>
        /// <returns>The traits of the fish, or null if unknown.</returns>
        IFishTraits GetFishTraits(int fish);

        /// <summary>Adds new treasure data to the list of obtainable treasure.</summary>
        /// <param name="data">The data to add as a new treasure.</param>
        /// <returns>True if added, false if it's a duplicate entry.</returns>
        bool AddTreasureData(ITreasureData data);

        /// <summary>Removes existing treasure data from the list of obtainable treasure.</summary>
        /// <param name="data">The data that should be removed.</param>
        /// <returns>True if removed, false if the data doesn't exist.</returns>
        bool RemoveTreasureData(ITreasureData data);

        /// <summary>Gets all the obtainable treasure data.</summary>
        /// <returns>An <see cref="IEnumerable{ITreasureData}"/> containing all the available treasure data.</returns>
        IEnumerable<ITreasureData> GetTreasureData();

        /// <summary>Adds new trash data to the list of obtainable trash.</summary>
        /// <param name="data">The data to add as new trash.</param>
        /// <returns>True if added, false if it's a duplicate entry.</returns>
        bool AddTrashData(ITrashData data);

        /// <summary>Removes existing trash data from the list of obtainable trash.</summary>
        /// <param name="data">The data that should be removed.</param>
        /// <returns>True if removed, false if the data doesn't exist.</returns>
        bool RemoveTrashData(ITrashData data);

        /// <summary>Gets all the obtainable trash data.</summary>
        /// <returns>An <see cref="IEnumerable{T}"/> containing all the available trash data.</returns>
        IEnumerable<ITrashData> GetTrashData();

        /// <summary>Gets all the trash a <see cref="Farmer"/> can catch.</summary>
        /// <param name="who">The farmer.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing all the trash the given <see cref="Farmer"/> can catch.</returns>
        IEnumerable<ITrashData> GetTrashData(Farmer who);

        /// <summary>Gets all the obtainable trash data with the given criteria.</summary>
        /// <param name="who">The farmer.</param>
        /// <param name="locationName">The name of the current location being fished in.</param>
        /// <param name="waterType">The type of water the <see cref="Farmer"/> is fishing in.</param>
        /// <param name="date">The current date.</param>
        /// <param name="weather">The current weather.</param>
        /// <param name="time">The current time.</param>
        /// <param name="fishingLevel">The current <see cref="Farmer"/>'s fishing level.</param>
        /// <param name="mineLevel">The current level in the mine, or null if not in the mine.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing all the available trash data that meets the given criteria.</returns>
        IEnumerable<ITrashData> GetTrashData(Farmer who, string locationName, WaterType waterType, SDate date, Weather weather, int time, int fishingLevel, int? mineLevel);

        /// <summary>Gets all the fish a <see cref="Farmer"/> can catch.</summary>
        /// <param name="who">The farmer.</param>
        /// <returns>All the fish that can be caught with their associated weights.</returns>
        IEnumerable<IWeightedElement<int?>> GetPossibleFish(Farmer who);

        /// <summary>Gets all the fish a <see cref="Farmer"/> can catch under the given circumstances.</summary>
        /// <param name="who">The farmer.</param>
        /// <param name="locationName">The name of the <see cref="GameLocation"/>.</param>
        /// <param name="water">The current water type.</param>
        /// <param name="date">The current date.</param>
        /// <param name="weather">The current weather.</param>
        /// <param name="time">The current time. This should be between 0600 and 2600, where the first two digits represent the hour, and the second two digits represent the minute.</param>
        /// <param name="fishLevel">The allowed fishing level.</param>
        /// <param name="mineLevel">The current mine level, or null to ignore fish specific to certain levels in the mine.</param>
        /// <returns>All the fish that can be caught with their associated weights.</returns>
        IEnumerable<IWeightedElement<int?>> GetPossibleFish(Farmer who, string locationName, WaterType water, SDate date, Weather weather, int time, int fishLevel, int? mineLevel = null);

        /// <summary>Gets the display name of a fish.</summary>
        /// <param name="fish">The ID of the fish to get the name of.</param>
        /// <returns>The display name of the fish with the given ID.</returns>
        string GetFishName(int fish);

        /// <summary>Sets the display name of a fish within this mod.</summary>
        /// <param name="fish">The ID of the fish.</param>
        /// <param name="name">The name of the fish.</param>
        void SetFishName(int fish, string name);

        /// <summary>Gets the current perfect streak of a <see cref="Farmer"/>.</summary>
        /// <param name="who">The farmer.</param>
        /// <returns>The current perfect fishing streak of the <see cref="Farmer"/>.</returns>
        int GetStreak(Farmer who);

        /// <summary>Sets the current perfect fishing streak of a <see cref="Farmer"/>.</summary>
        /// <param name="who">The farmer.</param>
        /// <param name="streak">The streak the given <see cref="Farmer"/> should have.</param>
        void SetStreak(Farmer who, int streak);

        /// <summary>Hides a fish from the "possible fish" HUD element and the bobber bar.</summary>
        /// <param name="fish">The ID of the fish to hide.</param>
        void HideFish(int fish);

        /// <summary>Reveals a fish in the "possible fish" HUD element and the bobber bar. This undoes <see cref="HideFish"/>.</summary>
        /// <param name="fish">The ID of the fish to reveal.</param>
        /// <returns>True if the fish was revealed, false if it wasn't hidden to begin with.</returns>
        bool RevealFish(int fish);

        /// <summary>Gets whether a fish is hidden.</summary>
        /// <param name="fish">The ID of the fish.</param>
        /// <returns>True if the fish is hidden, false if otherwise.</returns>
        bool IsHidden(int fish);

        /// <summary>Gets whether a fish is legendary.</summary>
        /// <param name="fish">The ID of the fish.</param>
        /// <returns>True if the fish is legendary, false if otherwise.</returns>
        bool IsLegendary(int fish);

        /// <summary>Sets whether a fish is legendary.</summary>
        /// <param name="fish">The ID of the fish.</param>
        /// <param name="legendary">True if the fish should be legendary, false if it should not be, null to remove overrides.</param>
        void SetLegendary(int fish, bool? legendary);
    }
}