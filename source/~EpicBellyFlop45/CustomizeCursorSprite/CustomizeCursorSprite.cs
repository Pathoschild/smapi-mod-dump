using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeCursorSprite
{
    class CustomizeCursorSprite : Mod, IAssetEditor
    {
        public override void Entry(IModHelper helper)
        {
            // Get the path for the cursor asset
            string cursorPath = Path.Combine(Helper.DirectoryPath, "Assets", "Cursor.png");

            // Check the cursor asset exists
            if (!File.Exists(cursorPath))
            {
                this.Monitor.Log("Cursor asset missing from assets folder", LogLevel.Warn);
            }

            GameEvents.FourthUpdateTick += Events_FouthUpdateTick;
        }

        private void Events_FouthUpdateTick(object sender, EventArgs e)
        {
            // Get the current mouse position
            Vector2 mousePosition = new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY());


        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            string cursorPath = Path.Combine(Helper.DirectoryPath, "Assets", "cursor.png");

            if (asset.AssetNameEquals("LooseSprites/Cursors"))
            {
                return File.Exists(cursorPath);
            }

            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            Texture2D cursorTexture = this.Helper.Content.Load<Texture2D>("Assets/Cursor.png", ContentSource.ModFolder);
            asset.AsImage().PatchImage(cursorTexture, sourceArea: new Rectangle(0, 0, 126, 27), targetArea: new Rectangle(0, 0, 126, 27), patchMode: PatchMode.Replace);
        }

        public ITarget GetTargetFromScreenCoordinate(GameLocation location, Vector2 tile, Vector2 position, bool includeMapTile)
        {
            // Get target sprites which might overlap cursor position (first approximation)
            Rectangle tileArea = this.GameHelper.GetScreenCoordinatesFromTile(tile);
            var candidates = (
                from target in this.GetNearbyTargets(location, tile, includeMapTile)
                let spriteArea = target.GetSpriteArea()
                let isAtTile = target.IsAtTile(tile)
                where
                    target.Type != TargetType.Unknown
                    && (isAtTile || spriteArea.Intersects(tileArea))
                orderby
                    target.Type != TargetType.Tile ? 0 : 1, // Tiles are always under anything else.
                    spriteArea.Y descending,                // A higher Y value is closer to the foreground, and will occlude any sprites behind it.
                    spriteArea.X ascending                  // If two sprites at the same Y coordinate overlap, assume the left sprite occludes the right.

                select new { target, spriteArea, isAtTile }
            ).ToArray();

            // Choose best match
            return
                candidates.FirstOrDefault(p => p.target.SpriteIntersectsPixel(tile, position, p.spriteArea))?.target // Sprite pixel under cursor
                ?? candidates.FirstOrDefault(p => p.isAtTile)?.target; // Tile under cursor
        }
    }
}
