/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidFoundations.Framework.Models.Backport
{
    // Backport of SDV v1.6
    // TODO: When updated to SDV v1.6, this class should be deleted in favor of using StardewValley.GameData.BuildingData
    public class BuildingData
    {
        // This property is set by this framework
        public string ID;

        public string Name;

        public string Description;

        public string Texture;

        [ContentSerializer(Optional = true)]
        public List<BuildingSkin> Skins = new List<BuildingSkin>();

        [ContentSerializer(Optional = true)]
        public bool DrawShadow = true;

        [ContentSerializer(Optional = true)]
        public Vector2 UpgradeSignTile = new Vector2(-1f, -1f);

        [ContentSerializer(Optional = true)]
        public float UpgradeSignHeight;

        [ContentSerializer(Optional = true)]
        public Point Size = new Point(1, 1);

        [ContentSerializer(Optional = true)]
        public bool FadeWhenBehind = true;

        [ContentSerializer(Optional = true)]
        public Point ConvertBuildingOffset = new Point(0, 0);

        [ContentSerializer(Optional = true)]
        public string SourceRect;

        [ContentSerializer(Optional = true)]
        public Point UpgradeOffset = Point.Zero;

        [ContentSerializer(Optional = true)]
        public Point SeasonOffset = Point.Zero;

        [ContentSerializer(Optional = true)]
        public Vector2 DrawOffset = Vector2.Zero;

        [ContentSerializer(Optional = true)]
        public float SortTileOffset;

        [ContentSerializer(Optional = true)]
        public string CollisionMap;

        [ContentSerializer(Optional = true)]
        public List<BuildingPlacementTile> AdditionalPlacementTiles;

        [ContentSerializer(Optional = true)]
        public string BuildingType;

        [ContentSerializer(Optional = true)]
        public string Builder = "Robin";

        [ContentSerializer(Optional = true)]
        public string BuildCondition;

        [ContentSerializer(Optional = true)]
        public int BuildDays;

        [ContentSerializer(Optional = true)]
        public int BuildCost;

        [ContentSerializer(Optional = true)]
        public List<BuildingMaterial> BuildMaterials;

        [ContentSerializer(Optional = true)]
        public string BuildingToUpgrade;

        [ContentSerializer(Optional = true)]
        public bool MagicalConstruction;

        public List<string> ValidBuildLocations;

        [ContentSerializer(Optional = true)]
        public Point HumanDoor = new Point(-1, -1);

        [ContentSerializer(Optional = true)]
        public string AnimalDoor;

        [ContentSerializer(Optional = true)]
        public float AnimalDoorOpenDuration;

        [ContentSerializer(Optional = true)]
        public string AnimalDoorOpenSound;

        [ContentSerializer(Optional = true)]
        public float AnimalDoorCloseDuration;

        [ContentSerializer(Optional = true)]
        public string AnimalDoorCloseSound;

        [ContentSerializer(Optional = true)]
        public string NonInstancedIndoorLocation;

        [ContentSerializer(Optional = true)]
        public string IndoorMap;

        [ContentSerializer(Optional = true)]
        public string IndoorMapType;

        [ContentSerializer(Optional = true)]
        public int MaxOccupants = 20;

        [ContentSerializer(Optional = true)]
        public List<string> ValidOccupantTypes = new List<string>();

        [ContentSerializer(Optional = true)]
        public bool AllowAnimalPregnancy;

        [ContentSerializer(Optional = true)]
        public List<IndoorItemMove> IndoorItemMoves;

        [ContentSerializer(Optional = true)]
        public List<IndoorItemAdd> IndoorItems;

        [ContentSerializer(Optional = true)]
        public List<string> AddMailOnBuild;

        [ContentSerializer(Optional = true)]
        public Dictionary<string, string> Metadata = new Dictionary<string, string>();

        [ContentSerializer(Optional = true)]
        public Dictionary<string, string> ModData = new Dictionary<string, string>();

        [ContentSerializer(Optional = true)]
        public int HayCapacity;

        [ContentSerializer(Optional = true)]
        public List<BuildingChest> Chests;

        [ContentSerializer(Optional = true)]
        public string DefaultAction;

        [ContentSerializer(Optional = true)]
        public int AdditionalTilePropertyRadius;

        [ContentSerializer(Optional = true)]
        public List<BuildingActionTiles> ActionTiles = new List<BuildingActionTiles>();

        [ContentSerializer(Optional = true)]
        public List<BuildingTileProperties> TileProperties = new List<BuildingTileProperties>();

        [ContentSerializer(Optional = true)]
        public List<BuildingItemConversion> ItemConversions;

        [ContentSerializer(Optional = true)]
        public List<BuildingDrawLayer> DrawLayers;

        protected Rectangle? _sourceRect;

        protected Rectangle? _animalDoorRect;

        protected Dictionary<Point, string> _actionTiles;

        protected Dictionary<Point, bool> _collisionMap;

        protected Dictionary<string, Dictionary<Point, Dictionary<string, string>>> _tileProperties;

        public Rectangle GetSourceRect()
        {
            if (!_sourceRect.HasValue)
            {
                if (SourceRect == null)
                {
                    _sourceRect = Rectangle.Empty;
                }
                else
                {
                    try
                    {
                        string[] array = SourceRect.Split(' ');
                        _sourceRect = new Rectangle(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]), int.Parse(array[3]));
                    }
                    catch (Exception)
                    {
                        _sourceRect = Rectangle.Empty;
                    }
                }
            }
            return _sourceRect.Value;
        }

        public Rectangle GetAnimalDoorRect()
        {
            if (!_animalDoorRect.HasValue)
            {
                if (AnimalDoor == null)
                {
                    _animalDoorRect = new Rectangle(-1, -1, 0, 0);
                }
                else
                {
                    try
                    {
                        string[] array = AnimalDoor.Split(' ');
                        int width = 1;
                        int height = 1;
                        if (array.Length > 2)
                        {
                            width = int.Parse(array[2]);
                        }
                        if (array.Length > 3)
                        {
                            height = int.Parse(array[3]);
                        }
                        _animalDoorRect = new Rectangle(int.Parse(array[0]), int.Parse(array[1]), width, height);
                    }
                    catch (Exception)
                    {
                        _animalDoorRect = new Rectangle(-1, -1, 0, 0);
                    }
                }
            }
            return _animalDoorRect.Value;
        }

        public bool IsTilePassable(int relative_x, int relative_y)
        {
            if (CollisionMap == null)
            {
                if (relative_x >= 0 && relative_x < Size.X && relative_y >= 0 && relative_y < Size.Y)
                {
                    return false;
                }
                return true;
            }
            Point key = new Point(relative_x, relative_y);
            if (_collisionMap == null)
            {
                _collisionMap = new Dictionary<Point, bool>();
                if (CollisionMap != null)
                {
                    string[] array = CollisionMap.Trim().Split('\n');
                    for (int i = 0; i < array.Length; i++)
                    {
                        string text = array[i].Trim();
                        for (int j = 0; j < text.Length; j++)
                        {
                            _collisionMap[new Point(j, i)] = text[j] == 'X';
                        }
                    }
                }
            }
            bool value = false;
            if (!_collisionMap.TryGetValue(key, out value))
            {
                return true;
            }
            return !value;
        }

        public string GetActionAtTile(int relative_x, int relative_y)
        {
            Point key = new Point(relative_x, relative_y);
            if (_actionTiles == null)
            {
                _actionTiles = new Dictionary<Point, string>();
                foreach (BuildingActionTiles actionTile in ActionTiles)
                {
                    _actionTiles[actionTile.Tile] = actionTile.Action;
                }
            }
            string value = null;
            if (!_actionTiles.TryGetValue(key, out value))
            {
                if (relative_x < 0 || relative_x >= Size.X || relative_y < 0 || relative_y >= Size.Y)
                {
                    return null;
                }
                value = DefaultAction;
            }
            return value;
        }

        public bool HasPropertyAtTile(int relative_x, int relative_y, string property_name, string layer_name, ref string property_value)
        {
            new Point(relative_x, relative_y);
            if (_tileProperties == null)
            {
                _tileProperties = new Dictionary<string, Dictionary<Point, Dictionary<string, string>>>();
                foreach (BuildingTileProperties tileProperty in TileProperties)
                {
                    string layerName = tileProperty.LayerName;
                    Dictionary<Point, Dictionary<string, string>> dictionary = new Dictionary<Point, Dictionary<string, string>>();
                    _tileProperties[layerName] = dictionary;
                    foreach (BuildingTileProperty tile in tileProperty.Tiles)
                    {
                        if (!dictionary.ContainsKey(tile.Tile))
                        {
                            dictionary[tile.Tile] = new Dictionary<string, string>();
                        }
                        dictionary[tile.Tile][tile.Key] = tile.Value;
                    }
                }
            }
            if (!_tileProperties.TryGetValue(layer_name, out var value))
            {
                return false;
            }
            if (!value.TryGetValue(new Point(relative_x, relative_y), out var value2))
            {
                return false;
            }
            if (!value2.TryGetValue(property_name, out var value3))
            {
                return false;
            }
            property_value = value3;
            return true;
        }
    }
}
