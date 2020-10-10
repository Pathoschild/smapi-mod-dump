/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.IO;

namespace CustomizeCursorSprite
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides method for interacting with the modding API and mod directory.</param>
        public override void Entry(IModHelper helper)
        {
            // check the cursor asset exists
            if (!File.Exists(Path.Combine(this.Helper.DirectoryPath, "assets", "Cursor.png")))
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
            if (asset.AssetNameEquals("LooseSprites/Cursors"))
            {
                return File.Exists(Path.Combine(Helper.DirectoryPath, "assets", "cursor.png"));
            }

            return false;
        }

        /// <summary>The method that gets called foreach asset we want to change. Used to apply the new cursor sprite.</summary>
        /// <typeparam name="T">The asset type.</typeparam>
        /// <param name="asset">The asset data.</param>
        public void Edit<T>(IAssetData asset)
        {
            Texture2D cursorTexture = this.Helper.Content.Load<Texture2D>("assets/Cursor.png", ContentSource.ModFolder);
            asset.AsImage().PatchImage(cursorTexture, sourceArea: new Rectangle(0, 0, 126, 27), targetArea: new Rectangle(0, 0, 126, 27), patchMode: PatchMode.Replace);
        }
    }
}
