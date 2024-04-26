/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley;
using StardewValley.GameData.Locations;
using System.Collections.Generic;
using static StardewValley.GameData.QuantityModifier;
using SVLocationData = StardewValley.GameData.Locations.LocationData;

namespace Randomizer
{
    public class ArtifactSpotRandomizer
    {
        private static RNG Rng { get; set; }

        /// <summary>
        /// Adds an extra artifact spot drop item to each relevant location
        /// </summary>
        /// <param name="locationsReplacements">The list of replacements</param>
        public static void Randomize(
            Dictionary<string, SVLocationData> locationsReplacements)
        {
            if (!Globals.Config.AddRandomArtifactItem)
            {
                return;
            }

            Rng = RNG.GetFarmRNG(nameof(ArtifactSpotRandomizer));

            Globals.SpoilerWrite("==== Extra Artifact Spot Items ====");

            LocationData.ArtifactSpotLocations
                .ForEach(location => AddArtifactSpotDropData(location, locationsReplacements));

            Globals.SpoilerWrite("");
        }

        /// <summary>
        /// Adds an extra artifact spot drop item to the given location
        /// </summary>
        /// <param name="location">The location to add the item to</param>
        /// <param name="locationsReplacements">The list of replacements</param>
        private static void AddArtifactSpotDropData(
            Locations location, 
            Dictionary<string, SVLocationData> locationsReplacements)
        {
            ObtainingDifficulties difficulty = GetRandomItemDifficulty();
            double probability = GetDigProbabilityForDifficulty(difficulty);

            SVLocationData defaultLocData = DataLoader.Locations(Game1.content)[location.ToString()];
            SVLocationData locationData = LocationData.TrySetLocationData(
                location, defaultLocData, locationsReplacements);

            Item diggableItem = ItemList.GetRandomItemAtDifficulty(Rng, difficulty);
            locationData.ArtifactSpots.Add(
                GetArtifactSpotDropData(diggableItem, probability));

            Globals.SpoilerWrite($"{location}: ({diggableItem.Id}) {diggableItem.Name} | {probability}");
        }

        /// <summary>
        /// Gets a random item difficulty
        /// - 1/2 = no req
        /// - 1/4 = small time req
        /// - 1/8 = medium time req
        /// - 1/16 = large time req
        /// - 1/32 = uncommon
        /// - 1/64 = rare
        /// </summary>
        /// <returns></returns>
        private static ObtainingDifficulties GetRandomItemDifficulty()
        {
            if (Rng.NextBoolean())
            {
                return ObtainingDifficulties.NoRequirements;
            }

            else if (Rng.NextBoolean())
            {
                return ObtainingDifficulties.SmallTimeRequirements;
            }

            else if (Rng.NextBoolean())
            {
                return ObtainingDifficulties.MediumTimeRequirements;
            }

            else if (Rng.NextBoolean())
            {
                return ObtainingDifficulties.LargeTimeRequirements;
            }

            else if (Rng.NextBoolean())
            {
                return ObtainingDifficulties.UncommonItem;
            }

            else
            {
                return ObtainingDifficulties.RareItem;
            }
        }

        /// <summary>
        /// Gets the probability of getting the random item based on the given difficulty
        /// </summary>
        /// <param name="difficulty">The difficulty</param>
        /// <returns>A double representing the probability - 0 being never, 100 being always</returns>
        private static double GetDigProbabilityForDifficulty(ObtainingDifficulties difficulty)
        {
            switch (difficulty)
            {
                case ObtainingDifficulties.NoRequirements:
                    return (double)Rng.NextIntWithinRange(30, 60) / 100;
                case ObtainingDifficulties.SmallTimeRequirements:
                    return (double)Rng.NextIntWithinRange(30, 40) / 100;
                case ObtainingDifficulties.MediumTimeRequirements:
                    return (double)Rng.NextIntWithinRange(20, 30) / 100;
                case ObtainingDifficulties.LargeTimeRequirements:
                    return (double)Rng.NextIntWithinRange(10, 20) / 100;
                case ObtainingDifficulties.UncommonItem:
                    return (double)Rng.NextIntWithinRange(5, 15) / 100;
                case ObtainingDifficulties.RareItem:
                    return (double)Rng.NextIntWithinRange(1, 5) / 100;
                default:
                    Globals.ConsoleError($"Attempting to get a diggable item with invalid difficulty: {difficulty}");
                    return (double)Rng.NextIntWithinRange(30, 60) / 100;
            }
        }

        /// <summary>
        /// Creates an artifact spot drop object for the given item and probability
        /// Sets the precedence to 1 so it's chosen just before the clay
        /// - Precedence values are checked from low to high, so 1 is just above the default (0)
        /// </summary>
        /// <param name="item">The item to drop</param>
        /// <param name="probability">The chance the item drops</param>
        /// <returns>The artificat spot drop data to use</returns>
        private static ArtifactSpotDropData GetArtifactSpotDropData(
            Item item, 
            double probability)
        {
            string id = item.QualifiedId;
            return new ArtifactSpotDropData()
            {
                Chance = probability,
                ApplyGenerousEnchantment = false,
                OneDebrisPerDrop = true,
                Precedence = 1,
                ContinueOnDrop = false,
                Condition = null,
                Id = id,
                ItemId = id,
                RandomItemId = null,
                MaxItems = null,
                MinStack = -1,
                MaxStack = -1,
                Quality = -1,
                ObjectInternalName = null,
                ObjectDisplayName = null,
                ToolUpgradeLevel = -1,
                IsRecipe = false,
                StackModifiers = null,
                StackModifierMode = QuantityModifierMode.Stack,
                QualityModifiers = null,
                QualityModifierMode = QuantityModifierMode.Stack,
                PerItemCondition = null
            };
        }
    }
}
