using System;
using MegaStorage.Mapping;
using MegaStorage.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace MegaStorage
{
    public class SpritePatcher : IAssetEditor
    {
        private const int NewHeight = 4000;

        private readonly IModHelper _modHelper;
        private readonly IMonitor _monitor;

        public SpritePatcher(IModHelper modHelper, IMonitor monitor)
        {
            _modHelper = modHelper;
            _monitor = monitor;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("TileSheets/Craftables");
        }

        public void Edit<T>(IAssetData asset)
        {
            _monitor.VerboseLog("Type of asset: " + typeof(T));
            var assetImage = asset.AsImage();
            ExpandSpriteSheet(assetImage);
            foreach (var customChest in CustomChestFactory.CustomChests)
            {
                PatchSprite(assetImage, customChest);
            }
        }

        private void ExpandSpriteSheet(IAssetDataForImage assetImage)
        {
            _monitor.VerboseLog("Expanding sprite sheet.");
            var spriteSheet = assetImage.Data;
            _monitor.VerboseLog($"Width: {spriteSheet.Width}, Height: {spriteSheet.Height}");
            if (spriteSheet.Height >= NewHeight)
                return;
            var originalSize = spriteSheet.Width * spriteSheet.Height;
            var data = new Color[originalSize];
            spriteSheet.GetData(data);
            var originalRect = new Rectangle(0, 0, spriteSheet.Width, spriteSheet.Height);
            var expandedSpriteSheet = new Texture2D(Game1.graphics.GraphicsDevice, spriteSheet.Width, NewHeight);
            _monitor.VerboseLog($"New width: {expandedSpriteSheet.Width}, New height: {expandedSpriteSheet.Height}");
            expandedSpriteSheet.SetData(0, originalRect, data, 0, originalSize);
            try
            {
                assetImage.ReplaceWith(expandedSpriteSheet);
            }
            catch (Exception ex)
            {
                _monitor.Log("Error while expanding bigCraftableSpriteSheet: " + ex.Message);
            }
        }

        private void PatchSprite(IAssetDataForImage assetImage, CustomChest customChest)
        {
            var sprite = _modHelper.Content.Load<Texture2D>(customChest.Config.SpritePath);
            var destinationRect = Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, customChest.ParentSheetIndex, 16, 32);
            var sourceRect = new Rectangle(0, 0, 16, 32);
            _monitor.VerboseLog($"Destination rect: ({destinationRect.X}, {destinationRect.Y}) - ({destinationRect.Width}, {destinationRect.Height})");
            try
            {
                assetImage.PatchImage(sprite, targetArea: destinationRect, sourceArea: sourceRect);
            }
            catch (Exception ex)
            {
                _monitor.Log("Error while patching bigCraftableSpriteSheet: " + ex.Message);
            }
        }

    }
}
