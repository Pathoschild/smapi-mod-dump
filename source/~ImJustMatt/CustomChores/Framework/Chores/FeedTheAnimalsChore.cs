using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using SObject = StardewValley.Object;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class FeedTheAnimalsChore : BaseChore
    {
        private readonly IDictionary<AnimalHouse, IList<Vector2>> _animalHouses = new Dictionary<AnimalHouse, IList<Vector2>>();
        private readonly bool _enableBarns;
        private readonly bool _enableCoops;
        private int _animalsFed;

        public FeedTheAnimalsChore(ChoreData choreData) : base(choreData)
        {
            ChoreData.Config.TryGetValue("EnableBarns", out var enableBarns);
            ChoreData.Config.TryGetValue("EnableCoops", out var enableCoops);

            _enableBarns = !(enableBarns is bool b1) || b1;
            _enableCoops = !(enableCoops is bool b2) || b2;
        }

        public override bool CanDoIt(bool today = true)
        {
            _animalsFed = 0;
            _animalHouses.Clear();

            var animalHouses = (
                    from building in Game1.getFarm().buildings
                    where building.daysOfConstructionLeft <= 0 &&
                          ((_enableBarns && building is Barn barn && !barn.buildingType.Contains("Deluxe")) ||
                           (_enableCoops && building is Coop coop && !coop.buildingType.Contains("Deluxe")))
                    select building.indoors.Value)
                .OfType<AnimalHouse>()
                .ToList();

            foreach (var animalHouse in animalHouses)
            {
                _animalHouses.Add(animalHouse, GetAnimalTroughs(animalHouse));
            }

            return _animalHouses.Sum(animalHouse => animalHouse.Value.Count) > 0;
        }

        public override bool DoIt()
        {
            foreach (var animalHouse in _animalHouses)
            {
                foreach (var key in animalHouse.Value)
                {
                    if (Game1.getFarm().piecesOfHay <= 0)
                        continue;
                    if (animalHouse.Key.objects.ContainsKey(key))
                        continue;
                    animalHouse.Key.objects.Add(key, new SObject(178, 1));
                    --Game1.getFarm().piecesOfHay.Value;
                    ++_animalsFed;
                }
            }

            return _animalsFed > 0;
        }

        public override IDictionary<string, Func<string>> GetTokens()
        {
            var tokens = base.GetTokens();
            tokens.Add("AnimalName", GetFarmAnimalName);
            tokens.Add("AnimalsFed", GetAnimalsFed);
            tokens.Add("WorkDone", GetAnimalsFed);
            tokens.Add("WorkNeeded", GetWorkNeeded);
            return tokens;
        }

        public string GetFarmAnimalName()
        {
            var farmAnimals = (
                from farmAnimal in Game1.getFarm().getAllFarmAnimals()
                where (_enableBarns && farmAnimal.buildingTypeILiveIn.Value.Equals("Barn", StringComparison.CurrentCultureIgnoreCase)) ||
                      (_enableCoops && farmAnimal.buildingTypeILiveIn.Value.Equals("Coop", StringComparison.CurrentCultureIgnoreCase))
                select farmAnimal).ToList();
            return farmAnimals.Any() ? farmAnimals.Shuffle().First().Name : null;
        }

        private string GetAnimalsFed() =>
            _animalsFed.ToString(CultureInfo.InvariantCulture);
        private string GetWorkNeeded() =>
            _animalHouses.Values.Sum(animalHouse => animalHouse.Count)
                .ToString(CultureInfo.InvariantCulture);

        private static IList<Vector2> GetAnimalTroughs(AnimalHouse animalHouse)
        {
            var animalTroughs = new List<Vector2>();
            for (var xTile = 0; xTile < animalHouse.map.Layers[0].LayerWidth; ++xTile)
            {
                for (var yTile = 0; yTile < animalHouse.map.Layers[0].LayerHeight; ++yTile)
                {
                    if (animalHouse.doesTileHaveProperty(xTile, yTile, "Trough", "Back") == null)
                        continue;
                    var key = new Vector2(xTile, yTile);
                    animalTroughs.Add(key);
                    if (animalTroughs.Count >= animalHouse.animalLimit)
                        return animalTroughs;
                }
            }
            return animalTroughs;
        }
    }
}
