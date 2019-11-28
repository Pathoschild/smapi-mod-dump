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
    }
}
