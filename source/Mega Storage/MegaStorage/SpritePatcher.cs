using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace MegaStorage
{
    public class SpritePatcher
    {
        private const int NewHeight = 4000;
        //private const int SourceItemId = 130;

        private readonly IModHelper _modHelper;
        private readonly IMonitor _monitor;

        public SpritePatcher(IModHelper modHelper, IMonitor monitor)
        {
            _modHelper = modHelper;
            _monitor = monitor;
        }

        public void Patch()
        {
            ExpandSpriteSheet();
            foreach (var niceChest in NiceChestFactory.NiceChests)
            {
                CopySprite(niceChest.SpritePath, niceChest.ParentSheetIndex);
            }
        }

        private void ExpandSpriteSheet()
        {
            _monitor.VerboseLog("Expanding spritesheet");
            var originalWidth = Game1.bigCraftableSpriteSheet.Width;
            var originalHeight = Game1.bigCraftableSpriteSheet.Height;
            if (originalHeight >= NewHeight) return;
            var originalSize = originalWidth * originalHeight;
            var data = new Color[originalSize];
            Game1.bigCraftableSpriteSheet.GetData(data);
            var originalRect = new Rectangle(0, 0, originalWidth, originalHeight);
            var biggerCraftableSpriteSheet = new Texture2D(Game1.graphics.GraphicsDevice, originalWidth, NewHeight);
            biggerCraftableSpriteSheet.SetData(0, originalRect, data, 0, originalSize);
            Game1.bigCraftableSpriteSheet = biggerCraftableSpriteSheet;
        }

        private void CopySprite(string spritePath, int destinationId)
        {
            var sprite = _modHelper.Content.Load<Texture2D>(spritePath);
            var sourceRect = new Rectangle(0, 0, sprite.Width, sprite.Height);
            _monitor.VerboseLog($"Source rect: ({sourceRect.Width}, {sourceRect.Height})");
            var count = sourceRect.Width * sourceRect.Height;
            var data = new Color[count];
            sprite.GetData(0, sourceRect, data, 0, count);
            var destinationRect = Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, destinationId, 16, 32);
            destinationRect.Width = sprite.Width;
            _monitor.VerboseLog($"Destination rect: ({destinationRect.X}, {destinationRect.Y}) - ({destinationRect.Width}, {destinationRect.Height})");
            Game1.bigCraftableSpriteSheet.SetData(0, destinationRect, data, 0, count);
        }

    }
}
