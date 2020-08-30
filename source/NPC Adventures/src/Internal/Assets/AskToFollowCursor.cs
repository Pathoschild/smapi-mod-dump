using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace NpcAdventure.Internal.Assets
{
    class AskToFollowCursor : IAssetEditor
    {
        public const int TILE_POSITION = 50;
        private readonly Texture2D cursor;

        public AskToFollowCursor(IContentHelper helper)
        {
            // This is hardcoded because I dont see any sense for patch this by content packs.
            // If you want change this cursor, use Content Patcher and edit tile 131 in `LooseSprites\\Cursors"`.
            // Don't forget define your content patcher content pack as depends on this mod.
            this.cursor = helper.Load<Texture2D>("assets/Sprites/ask_to_follow.png");
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.DataType == typeof(Texture2D) 
                && asset.AssetNameEquals("LooseSprites\\Cursors");
        }

        public void Edit<T>(IAssetData asset)
        {
            IAssetDataForImage imageAsset = asset.AsImage();

            imageAsset.PatchImage(this.cursor, targetArea: 
                Game1.getSourceRectForStandardTileSheet(
                    imageAsset.Data, TILE_POSITION, 16, 16));
        }
    }
}
