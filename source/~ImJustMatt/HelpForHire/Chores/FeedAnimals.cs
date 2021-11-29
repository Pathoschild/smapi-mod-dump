/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace HelpForHire.Chores
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Services;
    using Microsoft.Xna.Framework;
    using StardewValley;
    using StardewValley.Buildings;
    using SObject = StardewValley.Object;

    internal class FeedAnimals : GenericChore
    {
        public FeedAnimals(ServiceManager serviceManager)
            : base("feed-animals", serviceManager)
        {
        }

        protected override bool DoChore()
        {
            var animalsFed = false;
            var piecesOfHay = Game1.getFarm().piecesOfHay;

            foreach (var (animalHouse, pos) in FeedAnimals.GetFeedingSpots())
            {
                if (piecesOfHay.Value <= 0)
                {
                    continue;
                }

                animalHouse.Objects.Add(pos, new(178, 1));
                piecesOfHay.Value--;
                animalsFed = true;
            }

            return animalsFed;
        }

        protected override bool TestChore()
        {
            return FeedAnimals.GetFeedingSpots().Any();
        }

        private static IEnumerable<Tuple<AnimalHouse, Vector2>> GetFeedingSpots()
        {
            var animalHouses = (
               from building in Game1.getFarm().buildings
               where building.daysOfConstructionLeft.Value <= 0 &&
                     ((building is Barn barn && !barn.buildingType.Contains("Deluxe")) ||
                      (building is Coop coop && !coop.buildingType.Contains("Deluxe")))
               select building.indoors.Value).OfType<AnimalHouse>();

            foreach (var animalHouse in animalHouses)
            {
                for (var xTile = 0; xTile < animalHouse.map.Layers[0].LayerWidth; ++xTile)
                {
                    for (var yTile = 0; yTile < animalHouse.map.Layers[0].LayerHeight; ++yTile)
                    {
                        if (animalHouse.doesTileHaveProperty(xTile, yTile, "Trough", "Back") is null)
                        {
                            continue;
                        }

                        var pos = new Vector2(xTile, yTile);
                        if (animalHouse.Objects.ContainsKey(pos))
                        {
                            continue;
                        }

                        yield return new(animalHouse, pos);
                    }
                }
            }
        }
    }
}