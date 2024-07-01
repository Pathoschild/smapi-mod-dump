/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Textures;
using StardewModdingAPI;
using StardewValley.ItemTypeDefinitions;

namespace StardewArchipelago.Locations.InGameLocations
{
    /// <summary>Manages the data for archipelago location items.</summary>
    public class ParsedArchipelagoItemData : ParsedItemData
    {
        private const int _textureSize = 48;

        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public ParsedArchipelagoItemData(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker, IItemDataDefinition itemType, string itemId, int spriteIndex, string textureName, string internalName, string displayName, string description, int category, string objectType, object rawData, bool isErrorItem = false, bool excludeFromRandomSale = false) : base(itemType, itemId, spriteIndex, textureName, internalName, displayName, description, category, objectType, rawData, isErrorItem, excludeFromRandomSale)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        public override Texture2D GetTexture()
        {
            return ArchipelagoTextures.GetArchipelagoLogo(_monitor, _modHelper, _textureSize, ArchipelagoTextures.COLOR);
        }

        public override Rectangle GetSourceRect(int offset = 0, int? spriteIndex = null)
        {
            return new Rectangle(0, 0, _textureSize, _textureSize);
        }
    }
}
