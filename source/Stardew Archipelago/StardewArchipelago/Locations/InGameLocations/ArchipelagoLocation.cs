/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.Textures;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Internal;
using StardewValley.Objects;
using Color = Microsoft.Xna.Framework.Color;

namespace StardewArchipelago.Locations.InGameLocations
{
    internal class ArchipelagoLocation : SpecialItem
    {
        private const string ARCHIPELAGO_PREFIX = "Archipelago: ";
        private const string ARCHIPELAGO_SHORT_PREFIX = "AP: ";
        private const string ARCHIPELAGO_NO_PREFIX = "";
        public const string EXTRA_MATERIALS_KEY = "Extra Materials";
        protected static Hint[] _activeHints;
        protected static uint _lastTimeUpdatedActiveHints;

        private Texture2D _archipelagoTexture;

        protected LocationChecker _locationChecker;
        protected string _locationDisplayName;

        public string LocationName { get; }

        private string _description;

        private Dictionary<string, int> _extraMaterialsRequired;

        private Dictionary<string, int> ExtraMaterialsRequired
        {
            get
            {
                if (_extraMaterialsRequired != null)
                {
                    return _extraMaterialsRequired;
                }

                _extraMaterialsRequired = new Dictionary<string, int>();
                if (modData == null || !modData.ContainsKey(EXTRA_MATERIALS_KEY))
                {
                    return _extraMaterialsRequired;
                }

                var extraMaterialsString = modData[EXTRA_MATERIALS_KEY];
                foreach (var extraMaterialString in extraMaterialsString.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    var extraMaterialFields = extraMaterialString.Split(":");
                    var materialId = extraMaterialFields[0];
                    var materialAmount = int.Parse(extraMaterialFields[1]);
                    _extraMaterialsRequired.Add(materialId, materialAmount);
                }

                return _extraMaterialsRequired;
            }
        }

        public ArchipelagoLocation(string locationName, IMonitor monitor, IModHelper modHelper, LocationChecker locationChecker, ArchipelagoClient archipelago, Hint[] myActiveHints) : this(locationName, locationName, monitor, modHelper, locationChecker, archipelago, myActiveHints)
        {
        }

        public ArchipelagoLocation(string locationDisplayName, string locationName, IMonitor monitor, IModHelper modHelper, LocationChecker locationChecker, ArchipelagoClient archipelago, Hint[] myActiveHints)
        {
            // Prefix removed for now, because the inconsistency makes it ugly
            // var prefix = locationDisplayName.Length < 18 ? ARCHIPELAGO_PREFIX : ARCHIPELAGO_NO_PREFIX;
            var prefix = ARCHIPELAGO_NO_PREFIX;
            _locationDisplayName = $"{prefix}{locationDisplayName}";
            Name = _locationDisplayName;
            LocationName = locationName;
            ItemId = $"{IDProvider.AP_LOCATION}_{LocationName/*.Replace(" ", "_")*/}";

            _locationChecker = locationChecker;

            var isHinted = myActiveHints.Any(hint => archipelago.GetLocationName(hint.LocationId).Equals(locationName, StringComparison.OrdinalIgnoreCase));
            var desiredTextureName = isHinted ? ArchipelagoTextures.PLEADING : ArchipelagoTextures.COLOR;
            _archipelagoTexture = ArchipelagoTextures.GetArchipelagoLogo(monitor, modHelper, 48, desiredTextureName);
            

            var scoutedLocation = archipelago.ScoutSingleLocation(LocationName);
            _description = scoutedLocation == null ? ScoutedLocation.GenericItemName() : scoutedLocation.ToString();
        }
        
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth,
            StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            var position = location + new Vector2(16f, 16f);
            var sourceRectangle = new Rectangle(0, 0, 48, 48);
            var transparentColor = color * transparency;
            var origin = new Vector2(8f, 8f);
            spriteBatch.Draw(_archipelagoTexture, position, sourceRectangle, transparentColor, 0.0f, origin, scaleSize, SpriteEffects.None, layerDepth);
        }

        public override bool isPlaceable()
        {
            return false;
        }

        public override int maximumStackSize()
        {
            return 1;
        }

        protected override Item GetOneNew()
        {
            return this;
        }

        public override bool canBeTrashed()
        {
            return false;
        }

        public override string TypeDefinitionId => QualifiedItemIds.ARCHIPELAGO_QUALIFER;

        public override string DisplayName => _locationDisplayName;

        public override bool CanBuyItem(Farmer who)
        {
            foreach (var (id, amount) in ExtraMaterialsRequired)
            {
                if (who.Items.CountId(id) < amount)
                {
                    return false;
                }
            }

            return true;
        }

        public override bool actionWhenPurchased(string shopId)
        {
            foreach (var (id, amount) in ExtraMaterialsRequired)
            {
                Game1.player.Items.ReduceId(id, amount);
            }
            SendCheck();
            return true;
        }

        public void SendCheck()
        {
            _locationChecker.AddCheckedLocation(LocationName);
        }

        public override string getDescription()
        {
            var descriptionWithExtraMaterials = $"{_description}{Environment.NewLine}";
            foreach (var (id, amount) in ExtraMaterialsRequired)
            {
                descriptionWithExtraMaterials += $"{Environment.NewLine}{amount} {DataLoader.Objects(Game1.content)[id].Name}";
            }

            return descriptionWithExtraMaterials;
        }

        public static IEnumerable<ItemQueryResult> Create(string locationName, IMonitor monitor, IModHelper modHelper, LocationChecker locationChecker, ArchipelagoClient archipelago)
        {
            if (string.IsNullOrWhiteSpace(locationName))
            {
                throw new ArgumentException($"Could not create {nameof(ArchipelagoLocation)} because there was no provided location name");
            }

            if (locationChecker.IsLocationMissing(locationName))
            {
                var currentTime = GetUniqueTimeIdentifier();
                if (currentTime != _lastTimeUpdatedActiveHints || _activeHints == null)
                {
                    _activeHints = archipelago.GetMyActiveHints();
                    _lastTimeUpdatedActiveHints = currentTime;
                }
                var item = new ArchipelagoLocation(locationName.Trim(), monitor, modHelper, locationChecker, archipelago, _activeHints);
                return new ItemQueryResult[] { new(item) };
            }

            return Enumerable.Empty<ItemQueryResult>();
        }

        protected static uint GetUniqueTimeIdentifier()
        {
            return Game1.stats.DaysPlayed * 3000 + (uint)Game1.timeOfDay;
        }
    }
}
