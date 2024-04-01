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
using SolidFoundations.Framework.Models.ContentPack.Actions;
using StardewModdingAPI;
using StardewValley.GameData.Buildings;
using System.Collections.Generic;
using System.Linq;

namespace SolidFoundations.Framework.Models.ContentPack
{
    public class ExtendedBuildingModel : BuildingData
    {
        public string ID;
        internal string NameTranslationKey { get; set; }
        internal string DescriptionTranslationKey { get; set; }
        internal string Owner { get; set; }
        internal string PackName { get; set; }
        internal string PaintMaskTexture { get; set; }
        internal ITranslationHelper Translations { get; set; }

        public List<Light> Lights;

        public List<PaintMaskData> PaintMasks;

        public new List<ExtendedBuildingChest> Chests
        {
            set
            {
                _chests = value;
                base.Chests = _chests.ToList<BuildingChest>();
            }
            get
            {
                return _chests;
            }
        }
        private List<ExtendedBuildingChest> _chests = new List<ExtendedBuildingChest>();

        public new List<ExtendedBuildingSkin> Skins
        {
            set
            {
                _skins = value;
                base.Skins = _skins.ToList<BuildingSkin>();
            }
            get
            {
                return _skins;
            }
        }
        private List<ExtendedBuildingSkin> _skins;

        public new List<ExtendedBuildingDrawLayer> DrawLayers
        {
            set
            {
                _drawLayers = value;
                base.DrawLayers = _drawLayers.ToList<BuildingDrawLayer>();
            }
            get
            {
                return _drawLayers;
            }
        }
        private List<ExtendedBuildingDrawLayer> _drawLayers;

        public new List<ExtendedBuildingItemConversion> ItemConversions
        {
            set
            {
                _itemConversions = value;
                base.ItemConversions = _itemConversions.ToList<BuildingItemConversion>();
            }
            get
            {
                return _itemConversions;
            }
        }
        private List<ExtendedBuildingItemConversion> _itemConversions;

        public int MaxConcurrentConversions { get; set; } = -1;

        public bool DisableAutomate { get; set; }

        public new List<ExtendedBuildingActionTiles> ActionTiles
        {
            set
            {
                _extendedActionTiles = value;
                base.ActionTiles = _extendedActionTiles.ToList<BuildingActionTile>();
            }
            get
            {
                return _extendedActionTiles;
            }
        }
        private List<ExtendedBuildingActionTiles> _extendedActionTiles = new List<ExtendedBuildingActionTiles>();

        protected Dictionary<Point, SpecialAction> _specialActionTiles;

        public List<ChestActionTile> LoadChestTiles = new List<ChestActionTile>();
        protected Dictionary<Point, string> _loadChestTiles;

        public List<ChestActionTile> CollectChestTiles = new List<ChestActionTile>();
        protected Dictionary<Point, string> _collectChestTiles;

        public List<ExtendedBuildingActionTiles> EventTiles = new List<ExtendedBuildingActionTiles>();
        protected Dictionary<Point, string> _eventTiles;
        protected Dictionary<Point, SpecialAction> _specialEventTiles;

        public List<Point> TunnelDoors = new List<Point>();

        public List<Point> AuxiliaryHumanDoors = new List<Point>();

        public SpecialAction DefaultSpecialAction { get; set; }

        public SpecialAction DefaultSpecialEventAction { get; set; }

        public List<InputFilter> InputFilters = new List<InputFilter>();

        public new string IndoorMapType { get { return _indoorMapType; } set { _indoorMapType = value; base.IndoorMapType = _indoorMapType; } }
        private string _indoorMapType;
        public string IndoorMapTypeAssembly { get { return _indoorMapTypeAssembly; } set { _indoorMapTypeAssembly = value; base.IndoorMapType = $"{this.IndoorMapType},{this.IndoorMapTypeAssembly}"; } }
        private string _indoorMapTypeAssembly = "Stardew Valley";

        // Required for alert to check for missing MagicalConstruction value 
        public new bool? MagicalConstruction
        {
            set
            {
                _magicalConstruction = value.Value;
                base.MagicalConstruction = value.Value;
            }
            get
            {
                return _magicalConstruction;
            }
        }
        private bool _magicalConstruction;

        // Used for backwards compatibility for packs that used StardewValley.Locations.BuildableGameLocation
        internal bool ForceLocationToBeBuildable { get; set; }

        public new string GetActionAtTile(int relative_x, int relative_y)
        {
            Point key = new Point(relative_x, relative_y);
            if (_actionTiles == null)
            {
                _actionTiles = new Dictionary<Point, string>();
                foreach (ExtendedBuildingActionTiles actionTile in ActionTiles)
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

        public SpecialAction GetSpecialActionAtTile(int relative_x, int relative_y)
        {
            Point key = new Point(relative_x, relative_y);
            if (_specialActionTiles == null)
            {
                _specialActionTiles = new Dictionary<Point, SpecialAction>();
                foreach (ExtendedBuildingActionTiles actionTile in ActionTiles)
                {
                    _specialActionTiles[actionTile.Tile] = actionTile.SpecialAction;
                }
            }

            SpecialAction value = null;
            if (!_specialActionTiles.TryGetValue(key, out value))
            {
                if (relative_x < 0 || relative_x >= Size.X || relative_y < 0 || relative_y >= Size.Y)
                {
                    return null;
                }
                value = DefaultSpecialAction;
            }
            return value;
        }

        public string GetLoadChestActionAtTile(int relative_x, int relative_y)
        {
            Point key = new Point(relative_x, relative_y);
            if (_loadChestTiles == null)
            {
                _loadChestTiles = new Dictionary<Point, string>();
                foreach (ChestActionTile chestTile in LoadChestTiles)
                {
                    _loadChestTiles[chestTile.Tile] = chestTile.Name;
                }
            }

            string chestName = null;
            if (!_loadChestTiles.TryGetValue(key, out chestName))
            {
                if (relative_x < 0 || relative_x >= Size.X || relative_y < 0 || relative_y >= Size.Y)
                {
                    return null;
                }
            }
            return chestName;
        }

        public string GetCollectChestActionAtTile(int relative_x, int relative_y)
        {
            Point key = new Point(relative_x, relative_y);
            if (_collectChestTiles == null)
            {
                _collectChestTiles = new Dictionary<Point, string>();
                foreach (ChestActionTile chestTile in CollectChestTiles)
                {
                    _collectChestTiles[chestTile.Tile] = chestTile.Name;
                }
            }

            string chestName = null;
            if (!_collectChestTiles.TryGetValue(key, out chestName))
            {
                if (relative_x < 0 || relative_x >= Size.X || relative_y < 0 || relative_y >= Size.Y)
                {
                    return null;
                }
            }
            return chestName;
        }

        public string GetEventAtTile(int relative_x, int relative_y)
        {
            Point key = new Point(relative_x, relative_y);
            if (_eventTiles == null)
            {
                _eventTiles = new Dictionary<Point, string>();
                foreach (ExtendedBuildingActionTiles eventTile in EventTiles)
                {
                    _eventTiles[eventTile.Tile] = eventTile.Action;
                }
            }

            string value = null;
            if (!_eventTiles.TryGetValue(key, out value))
            {
                if (relative_x < 0 || relative_x >= Size.X || relative_y < 0 || relative_y >= Size.Y)
                {
                    return null;
                }
            }
            return value;
        }

        public SpecialAction GetSpecialEventAtTile(int relative_x, int relative_y)
        {
            Point key = new Point(relative_x, relative_y);
            if (_specialEventTiles == null)
            {
                _specialEventTiles = new Dictionary<Point, SpecialAction>();
                foreach (ExtendedBuildingActionTiles eventTile in EventTiles)
                {
                    _specialEventTiles[eventTile.Tile] = eventTile.SpecialAction;
                }
            }

            SpecialAction value = null;
            if (!_specialEventTiles.TryGetValue(key, out value))
            {
                if (relative_x < 0 || relative_x >= Size.X || relative_y < 0 || relative_y >= Size.Y)
                {
                    return null;
                }
                value = DefaultSpecialEventAction;
            }
            return value;
        }

        public string GetTranslation(string text)
        {
            if (Translations is not null && Translations.GetTranslations().Any(t => t.Key == text))
            {
                return Translations.Get(text);
            }

            return text;
        }
    }
}
