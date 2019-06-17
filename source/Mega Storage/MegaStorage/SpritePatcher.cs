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
            var assetImage = asset.AsImage();
            ExpandSpriteSheet(assetImage);
            foreach (var niceChest in NiceChestFactory.NiceChests)
            {
                PatchSprite(assetImage, niceChest);
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
            assetImage.ReplaceWith(expandedSpriteSheet);
        }

        private void PatchSprite(IAssetDataForImage assetImage, NiceChest niceChest)
        {
            var sprite = _modHelper.Content.Load<Texture2D>(niceChest.SpritePath);
            var destinationRect = Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, niceChest.ParentSheetIndex, 16, 32);
            destinationRect.Width = sprite.Width;
            _monitor.VerboseLog($"Destination rect: ({destinationRect.X}, {destinationRect.Y}) - ({destinationRect.Width}, {destinationRect.Height})");
            assetImage.PatchImage(sprite, targetArea: destinationRect);
        }

    }
}
