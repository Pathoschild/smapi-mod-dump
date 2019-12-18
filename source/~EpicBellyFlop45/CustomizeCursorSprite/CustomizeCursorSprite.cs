using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.IO;

namespace CustomizeCursorSprite
{
    /// <summary>The mod entry point.</summary>
    class CustomizeCursorSprite : Mod, IAssetEditor
    {
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides method for interacting with the modding API and mod directory.</param>
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

        /// <summary>The method gets called for each asset being loaded. Used for editing the cursor sprite.</summary>
        /// <typeparam name="T">The asset type.</typeparam>
        /// <param name="asset">The asset info.</param>
        /// <returns>True if the passed asset is one that needs to be changed.</returns>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            string cursorPath = Path.Combine(Helper.DirectoryPath, "Assets", "cursor.png");

            if (asset.AssetNameEquals("LooseSprites/Cursors"))
            {
                return File.Exists(cursorPath);
            }

            return false;
        }

        /// <summary>The method that gets called foreach asset we want to change. Used to apply the new cursor sprite.</summary>
        /// <typeparam name="T">The asset type.</typeparam>
        /// <param name="asset">The asset data.</param>
        public void Edit<T>(IAssetData asset)
        {
            Texture2D cursorTexture = this.Helper.Content.Load<Texture2D>("Assets/Cursor.png", ContentSource.ModFolder);
            asset.AsImage().PatchImage(cursorTexture, sourceArea: new Rectangle(0, 0, 126, 27), targetArea: new Rectangle(0, 0, 126, 27), patchMode: PatchMode.Replace);
        }
    }
}
