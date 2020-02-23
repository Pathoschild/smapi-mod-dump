using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class WaterTheCropsChore : BaseChore
    {
        private readonly IDictionary<GameLocation, IList<HoeDirt>> _hoeDirt = new Dictionary<GameLocation, IList<HoeDirt>>();
        private readonly IDictionary<GameLocation, IList<Vector2>> _sprinklerCoverage =
            new Dictionary<GameLocation, IList<Vector2>>();
        private readonly bool _enableFarm;
        private readonly bool _enableBuildings;
        private readonly bool _enableGreenhouse;
        private int _cropsWatered;

        public WaterTheCropsChore(ChoreData choreData) : base(choreData)
        {
            ChoreData.Config.TryGetValue("EnableFarm", out var enableFarm);
            ChoreData.Config.TryGetValue("EnableBuildings", out var enableBuildings);
            ChoreData.Config.TryGetValue("EnableGreenhouse", out var enableGreenhouse);

            _enableFarm = !(enableFarm is bool b1) || b1;
            _enableBuildings = !(enableBuildings is bool b2) || b2;
            _enableGreenhouse = !(enableGreenhouse is bool b3) || b3;
        }

        public override bool CanDoIt(bool today = true)
        {
            _cropsWatered = 0;
            _hoeDirt.Clear();
            _sprinklerCoverage.Clear();

            if (today && (Game1.isRaining ||
                           Game1.currentSeason.Equals("winter", StringComparison.CurrentCultureIgnoreCase)))
                return false;

            var locations = (
                from location in Game1.locations
                where (_enableFarm && location.IsFarm) ||
                      (_enableGreenhouse && location.IsGreenhouse)
                select location).ToList();
            
            if (_enableBuildings)
                locations.AddRange(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value);

            foreach (var location in locations)
            {
                var coverage = new List<Vector2>();
                var sprinklers = location.objects.Values
                    .Where(obj => obj.Name.Contains("Sprinkler"))
                    .ToList();

                foreach (var sprinkler in sprinklers)
                {
                    switch (sprinkler.ParentSheetIndex)
                    {
                        case 599:
                            coverage.AddRange(Utility.getAdjacentTileLocations(sprinkler.TileLocation));
                            break;
                        case 621:
                            coverage.AddRange(Utility.getSurroundingTileLocationsArray(sprinkler.TileLocation));
                            break;
                        case 645:
                            for (var tileX = sprinkler.tileLocation.X - 2;
                                tileX <= sprinkler.tileLocation.X + 2;
                                ++tileX)
                            {
                                for (var tileY = sprinkler.tileLocation.Y - 2;
                                    tileY <= sprinkler.tileLocation.Y + 2;
                                    ++tileY)
                                {
                                    coverage.Add(new Vector2(tileX, tileY));
                                }
                            }
                            break;
                    }
                }
                _sprinklerCoverage.Add(location, coverage);

                _hoeDirt.Add(location, location.terrainFeatures.Values
                    .OfType<HoeDirt>()
                    .Where(hoeDirt => !coverage.Contains(hoeDirt.currentTileLocation))
                    .ToList());
            }

            return _hoeDirt.Values.Sum(hoeDirt => hoeDirt.Count) > 0;
        }

        public override bool DoIt()
        {
            if (Game1.isRaining || Game1.currentSeason.Equals("winter", StringComparison.CurrentCultureIgnoreCase))
                return true;

            foreach (var location in _hoeDirt)
            {
                _sprinklerCoverage.TryGetValue(location.Key, out var coverage);

                foreach (var hoeDirt in location.Value)
                {
                    if (!hoeDirt.needsWatering() && hoeDirt.state.Value != HoeDirt.watered)
                        continue;
                    if (coverage != null && coverage.Contains(hoeDirt.currentTileLocation))
                        continue;
                    hoeDirt.state.Value = HoeDirt.watered;
                    _cropsWatered++;
                }
            }

            return _cropsWatered > 0;
        }

        public override IDictionary<string, Func<string>> GetTokens()
        {
            var tokens = base.GetTokens();
            tokens.Add("CropsWatered", GetCropsWatered);
            tokens.Add("WorkDone", GetCropsWatered);
            tokens.Add("WorkNeeded", GetWorkNeeded);
            return tokens;
        }

        private string GetCropsWatered() =>
            _cropsWatered.ToString(CultureInfo.InvariantCulture);

        private string GetWorkNeeded() =>
            _hoeDirt.Values.Sum(hoeDirt => hoeDirt.Count).ToString(CultureInfo.InvariantCulture);
    }
}
