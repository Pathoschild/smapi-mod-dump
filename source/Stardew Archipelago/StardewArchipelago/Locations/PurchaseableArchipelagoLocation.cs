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
using System.IO;
using System.Linq;
using Archipelago.MultiClient.Net.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Textures;
using StardewModdingAPI;
using StardewValley;
using Color = Microsoft.Xna.Framework.Color;

namespace StardewArchipelago.Locations
{
    internal class PurchaseableArchipelagoLocation : Item
    {
        private const string ARCHIPELAGO_PREFIX = "Archipelago: ";
        private const string ARCHIPELAGO_SHORT_PREFIX = "AP: ";
        private Texture2D _archipelagoTexture;

        private string _locationDisplayName;
        private string _description;
        private LocationChecker _locationChecker;
        private List<Item> _extraMaterialsRequired;
        private Action _purchaseCallBack;

        public string ApLocationName { get; }

        public PurchaseableArchipelagoLocation(string locationDisplayName, string apLocationName, IModHelper modHelper, LocationChecker locationChecker, ArchipelagoClient archipelago, Hint[] myActiveHints, Action purchaseCallback = null)
        {
            var prefix = locationDisplayName.Length < 18 ? ARCHIPELAGO_PREFIX : ARCHIPELAGO_SHORT_PREFIX;
            _locationDisplayName = $"{prefix}{locationDisplayName}";
            ApLocationName = apLocationName;
            var scoutedLocation = archipelago.ScoutSingleLocation(ApLocationName);
            _description = scoutedLocation == null ? ScoutedLocation.GenericItemName() : scoutedLocation.ToString();
            _locationChecker = locationChecker;
            _extraMaterialsRequired = new List<Item>();
            _purchaseCallBack = purchaseCallback;

            var isHinted = myActiveHints.Any(hint => archipelago.GetLocationName(hint.LocationId).Equals(apLocationName, StringComparison.OrdinalIgnoreCase));
            var desiredTextureName = isHinted ? ArchipelagoTextures.PLEADING : ArchipelagoTextures.COLOR;
            _archipelagoTexture = ArchipelagoTextures.GetColoredLogo(modHelper, 48, desiredTextureName);
        }

        public void AddMaterialRequirement(Item requiredItem)
        {
            _extraMaterialsRequired.Add(requiredItem);
        }

        public override bool CanBuyItem(Farmer who)
        {
            foreach (var item in _extraMaterialsRequired)
            {
                if (!who.hasItemInInventory(item.ParentSheetIndex, item.Stack))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool actionWhenPurchased()
        {
            foreach (var item in _extraMaterialsRequired)
            {
                Game1.player.removeItemsFromInventory(item.ParentSheetIndex, item.Stack);
            }
            _locationChecker.AddCheckedLocation(ApLocationName);
            _purchaseCallBack?.Invoke();
            return true;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth,
            StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            spriteBatch.Draw(_archipelagoTexture, location + new Vector2(16f, 16f),
                new Rectangle(0, 0, 48, 48),
                color * transparency, 0.0f, new Vector2(8f, 8f), scaleSize, SpriteEffects.None, layerDepth);
        }

        public override string getDescription()
        {
            var descriptionWithExtraMaterials = $"{_description}{Environment.NewLine}";
            foreach (var material in _extraMaterialsRequired)
            {
                descriptionWithExtraMaterials += $"{Environment.NewLine}{material.Stack} {material.DisplayName}";
            }

            return descriptionWithExtraMaterials;
        }

        public override bool isPlaceable()
        {
            return false;
        }

        public override Item getOne()
        {
            return this;
        }

        public override int maximumStackSize()
        {
            return 1;
        }

        public override int addToStack(Item stack)
        {
            return 1;
        }

        public override string DisplayName
        {
            get => _locationDisplayName;
            set => _locationDisplayName = value;
        }

        public override int Stack { get; set; }
    }
}
