using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AutoGrabberMod.Features;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
namespace AutoGrabberMod.Models
{
    using SVObject = StardewValley.Object;

    public class AutoGrabber
    {
        public static readonly int ParentIndex = 165;
        public static string MakeId(GameLocation location, Vector2 tile)
        {
            return $"{location.uniqueName.Value ?? location.Name} ({tile.X},{tile.Y})";
        }
        public readonly int FARMING = 0;
        public readonly int FORAGING = 2;
        private readonly List<Point> _effectiveArea = new List<Point>();

        private bool _mouseOver = false;
        private int _totalDirts = -1;
        private int _totalSeeds = -1;
        private int _totalFertilizer = -1;
        private int _totalWatering = -1;
        private Vector2[] _nearbyTiles1 = null;
        private int _range = 5;
        private Vector2[] _nearbyTilesRange = null;
        public Vector2[] NearbyTilesRange => _nearbyTilesRange ?? Utilities.GetNearbyTiles(Tile, Range).ToArray();

        //config attributes
        public int Range
        {
            get => _range;
            set
            {
                if (value != _range)
                {
                    _nearbyTilesRange = Utilities.GetNearbyTiles(Tile, value).ToArray();
                }
                _range = value;
            }
        } //Range in radius X tiles to top, right, bottom, left of its tile
        public bool RangeEntireMap { get; set; } = false; //Range coverage for entire map           
        public bool GainExperience { get; set; } = false;
        public bool ShowRange { get; set; } = false;
        public Feature[] FeaturesConfig { get; set; }
        public GameLocation Location { get; set; }
        public SVObject Grabber { get; set; }
        public Vector2 Tile { get; set; }

        public bool IsMouseOver
        {
            get => _mouseOver;
            set
            {
                if (value && !_mouseOver)
                {
                    _totalDirts = _totalWatering = _totalFertilizer = _totalSeeds = -1;
                }
                _mouseOver = value;
            }
        }

        private IEnumerable<string> NameConfigValues
        {
            get
            {
                if (Name != DefaultName)
                    foreach (Match match in Regex.Matches(Name, @"\|([^\|]+)\|")) yield return match.Groups[1].Value.ToLower();
            }
        }
        public Chest GrabberChest => (Grabber.heldObject.Value as Chest);
        public bool IsChestFull => GrabberChest.items.Count >= 36;
        public string DefaultName => (new SVObject(Tile, ParentIndex)).Name;
        public String Name
        {
            get => Grabber.Name;
            set => Grabber.name = value;
        }

        public string InstanceName { get => $"{Location.Name} ({Tile.X},{Tile.Y})"; }

        public string Id => MakeId(Location, Tile);

        public IEnumerable<NearbyChest> NearbyChests
        {
            get
            {
                _nearbyTiles1 = _nearbyTiles1 ?? Utilities.GetNearbyTiles(Tile, 1).ToArray();

                IEnumerable<NearbyChest> Chests()
                {
                    yield return new NearbyChest(Tile, GrabberChest);
                    foreach (Vector2 tile in _nearbyTiles1)
                    {
                        if (Location.isObjectAtTile((int)tile.X, (int)tile.Y) && Location.Objects[tile] is Chest)
                        {
                            yield return new NearbyChest(tile, (Location.Objects[tile] as Chest));
                        }
                    }
                }

                return (
                    from chest in Chests()
                    where chest.Seeds.Any() || chest.Fertilizers.Any() || chest.Sprinklers.Any()
                    select chest
                );
            }
        }        

        public IEnumerable<KeyValuePair<Vector2, HoeDirt>> Dirts => RangeEntireMap
            ? Utilities.GetDirts(Location, Location.Objects.Keys.Union(Location.terrainFeatures.Keys).ToArray())
            : Utilities.GetDirts(Location, NearbyTilesRange);

        public int TotalDirts
        {
            get
            {
                if (IsMouseOver)
                {
                    if (_totalDirts == -1)
                    {
                        _totalDirts = Dirts.Count();
                    }
                    return _totalDirts;
                }
                return Dirts.Count();
            }
        }

        public int TotalWateringCapacity
        {
            get
            {
                if (IsMouseOver)
                {
                    if (_totalWatering == -1) _totalWatering = NearbyChests.ToArray().Where((arg) => arg.SprinklerCapacity != 0).Sum((arg) => arg.SprinklerCapacity);
                    return _totalWatering;
                }
                return NearbyChests.ToArray().Where((arg) => arg.SprinklerCapacity != 0).Sum((arg) => arg.SprinklerCapacity);
            }
        }

        public int TotalSeeds
        {
            get
            {
                if (IsMouseOver)
                {
                    if (_totalSeeds == -1) _totalSeeds = NearbyChests.ToArray().Where((arg) => arg.Seeds.Any()).Sum((arg) => arg.TotalSeeds);
                }
                return NearbyChests.ToArray().Where((arg) => arg.Seeds.Any()).Sum((arg) => arg.TotalSeeds);
            }
        }

        public int TotalFertilizers
        {
            get
            {
                if (IsMouseOver)
                {
                    if (_totalFertilizer == -1) _totalFertilizer = NearbyChests.ToArray().Where((arg) => arg.Fertilizers.Any()).Sum((arg) => arg.TotalFertilizers);
                }
                return NearbyChests.ToArray().Where((arg) => arg.Fertilizers.Any()).Sum((arg) => arg.TotalFertilizers);
            }
        }

        public NextItem NextSeed => NearbyChests.ToArray().Where(chest => chest.Seeds.Any()).Select(chest => new NextItem(chest.Seeds.First(), chest.Chest)).FirstOrDefault();

        public NextItem NextFertilizer => NearbyChests.ToArray().Where(chest => chest.Fertilizers.Any()).Select(chest => new NextItem(chest.Fertilizers.First(), chest.Chest)).FirstOrDefault();

        public AutoGrabber(GameLocation location, SVObject grabber, Vector2 tile)
        {
            //Utilities.Monitor.Log($"Creating an instance: {location.Name} {tile}");
            Location = location;
            Grabber = grabber;
            Tile = tile;
            var configVals = NameConfigValues.ToArray();
            FeaturesConfig = Utilities.FeatureTypes.Select(t =>
            {
                Feature instance = (Feature)Activator.CreateInstance(t);
                instance.Grabber = this;
                instance.ConfigParse(configVals);
                return instance;
            }).OrderBy(i => i.Order).ToArray();

            if (configVals.Contains("exp")) GainExperience = true;
            if (configVals.Contains("grid")) ShowRange = true;
            var rangeConfig = configVals.FirstOrDefault(c => c.StartsWith("r", StringComparison.CurrentCulture));
            if (rangeConfig != null && int.TryParse(rangeConfig.Substring(2).Trim(), out int range))
            {
                Range = range;
                RangeEntireMap = false;
            }
            else if (configVals.Contains("map")) RangeEntireMap = true;
        }

        public void Action()
        {
            foreach (var feature in FeaturesConfig) feature.Action();
        }

        public Feature FeatureType<T>() where T : Feature
        {
            return FeaturesConfig.First(f => f is T t);
        }

        public void Update()
        {
            string internalName = DefaultName;

            if (RangeEntireMap) internalName += " |map|";
            else internalName += $" |r:{Range}|";
            if (GainExperience) internalName += " |exp|";
            if (ShowRange) internalName += " |grid|";

            foreach (var feature in FeaturesConfig)
            {
                if (feature != null)
                {
                    internalName += feature.ConfigValue();
                }
            }

            //Utilities.Monitor.Log($"  {InstanceName} Setting name to {internalName} {!string.IsNullOrWhiteSpace(internalName)}", StardewModdingAPI.LogLevel.Trace);
            Grabber.name = !string.IsNullOrWhiteSpace(internalName) ? internalName : this.DefaultName;
        }

        public void DrawNameTooltip()
        {
            IClickableMenu.drawHoverText(Game1.spriteBatch, TooltipText(), Game1.smallFont);
        }

        public string TooltipText()
        {
            StringBuilder builder = new StringBuilder(InstanceName);
            if ((bool)FeatureType<PlantSeeds>().Value)
            {
                builder.Append($", {TotalSeeds} Seeds");
                if ((bool)FeatureType<Fertilize>().Value)
                {
                    builder.Append($", {TotalFertilizers} Fertilizers");
                }
            }
            if ((bool)FeatureType<WaterFields>().Value)
            {
                builder.Append($", {TotalWateringCapacity}/{TotalDirts} Watering capacity");
            }
            return builder.ToString();
        }

        public void DrawTileOutlines()
        {
            if (RangeEntireMap) return;
            CheckDrawTileOutlines();
            foreach (Point point in _effectiveArea)
                Game1.spriteBatch.Draw(
                    Game1.mouseCursors,
                    Game1.GlobalToLocal(new Vector2(point.X * Game1.tileSize, point.Y * Game1.tileSize)),
                    new Rectangle(194, 388, 16, 16),
                    Color.White * 0.7f,
                    0.0f,
                    Vector2.Zero,
                    Game1.pixelZoom,
                    SpriteEffects.None,
                    0.01f);
        }

        private void ParseConfigToHighlightedArea(int[][] highlightedLocation, int xPos, int yPos)
        {
            int xOffset = highlightedLocation.Length / 2;
            for (int i = 0; i < highlightedLocation.Length; ++i)
            {
                int yOffset = highlightedLocation[i].Length / 2;
                for (int j = 0; j < highlightedLocation[i].Length; ++j)
                {
                    if (highlightedLocation[i][j] == 1)
                        _effectiveArea.Add(new Point(xPos + i - xOffset, yPos + j - yOffset));
                }
            }
        }

        private void CheckDrawTileOutlines()
        {
            _effectiveArea.Clear();
            int[][] arrayToUse = null;
            if (Game1.activeClickableMenu == null && !Game1.eventUp)
            {
                var tiles = (Range * 2) + 1;
                arrayToUse = new int[tiles][];
                for (int i = 0; i < tiles; ++i)
                {
                    arrayToUse[i] = new int[tiles];
                    for (int j = 0; j < tiles; ++j)
                    {
                        arrayToUse[i][j] = i == Range && j == Range ? 0 : 1;
                    }
                }

                ParseConfigToHighlightedArea(arrayToUse, (int)Tile.X, (int)Tile.Y);
            }
        }
    }
}