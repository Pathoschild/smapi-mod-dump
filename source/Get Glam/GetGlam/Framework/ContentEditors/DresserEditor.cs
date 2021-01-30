/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MartyrPher/GetGlam
**
*************************************************/

using GetGlam.Framework.DataModels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.IO;

namespace GetGlam.Framework.ContentEditors
{
    public class DresserEditor
    {
        // Instance of Mod Entry
        private ModEntry Entry;

        // Instance of ContentPackHelper
        private ContentPackHelper PackHelper;

        // The current asset being edited
        private IAssetData Asset;

        // The DresserTexture Height, default: 32
        private int DresserTextureHeight = 32;

        // The max skin color texture height
        private const int MaxSkinColorTextureHeight = 4096;

        // Width of a single dresser image
        private const int SingleDresserWidth = 16;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entry">Instance of ModEntry</param>
        /// <param name="packHelper">Instance of ContentPackHelper</param>
        /// <param name="asset">Current asset being edited</param>
        public DresserEditor(ModEntry entry, ContentPackHelper packHelper, IAssetData asset) 
        {
            Entry = entry;
            PackHelper = packHelper;
            Asset = asset;
        }

        /// <summary>
        /// Edits the dresser texture.
        /// </summary>
        public void EditDresserTexture()
        {
            if (Asset.AssetNameEquals($"Mods/{Entry.ModManifest.UniqueID}/dresser.png"))
            {
                CreateNewTexture();

                // Loop through each dresser and patch the image
                foreach (var dresser in PackHelper.DresserList)
                {
                    if ((dresser.TextureHeight + DresserTextureHeight) > MaxSkinColorTextureHeight)
                    {
                        Entry.Monitor.Log($"{dresser.ModName} dressers cannot be added to the game, the texture is too big.", LogLevel.Warn);
                        return;
                    }

                    PatchDresserTexture(dresser);
                    DresserTextureHeight += dresser.TextureHeight;
                }

                Entry.Monitor.Log($"Dresser Image height is now: {DresserTextureHeight}", LogLevel.Trace);

                CutEmptyImage(Asset, DresserTextureHeight, SingleDresserWidth);
                SaveDresserImageToFile();
            }
        }

        /// <summary>
        /// Creates a new texture from the data of the old texture.
        /// </summary>
        private void CreateNewTexture()
        {
            Texture2D oldTexture = Asset.AsImage().Data;
            Texture2D newTexture = new Texture2D(Game1.graphics.GraphicsDevice, 16, Math.Max(oldTexture.Height, 4096));
            Asset.ReplaceWith(newTexture);
            Asset.AsImage().PatchImage(oldTexture);
        }

        /// <summary>
        /// Patches the dresser texture.
        /// </summary>
        /// <param name="dresser">Current dresser patched</param>
        private void PatchDresserTexture(DresserModel dresser)
        {
            Asset.AsImage().PatchImage(dresser.Texture,
                null,
                new Rectangle(0, DresserTextureHeight, SingleDresserWidth, dresser.TextureHeight));
        }

        /// <summary>
        /// Saves the dresser image to a file so it can be applied to a Tilesheet.
        /// </summary>
        private void SaveDresserImageToFile()
        {
            FileStream stream = new FileStream(Path.Combine(Entry.Helper.DirectoryPath, "assets", "dresser.png"), FileMode.Create);
            try
            {
                Asset.AsImage().Data.SaveAsPng(stream, SingleDresserWidth, DresserTextureHeight);
            }
            catch (Exception e)
            {
                Entry.Monitor.Log($"There was a problem saving the dresser image. {e.StackTrace}", LogLevel.Error);
            }
            finally 
            {
                stream.Close();
            }

            PackHelper.DresserTexture = Entry.Helper.Content.Load<Texture2D>(Path.Combine("assets", "dresser.png"), ContentSource.ModFolder);
        }

        /// <summary>
        /// Cuts the empty image from the texture.
        /// </summary>
        /// <param name="asset">The asset to cut</param>
        /// <param name="newHeight">The assets new height</param>
        /// <param name="newWidth">The assets new width</param>
        private void CutEmptyImage(IAssetData asset, int newHeight, int newWidth)
        {
            Texture2D oldTexture = asset.AsImage().Data;
            Texture2D cutTexture = new Texture2D(Game1.graphics.GraphicsDevice, oldTexture.Width, newHeight);

            asset.ReplaceWith(cutTexture);
            asset.AsImage().PatchImage(oldTexture, new Rectangle(0, 0, newWidth, newHeight), new Rectangle(0, 0, newWidth, newHeight));
        }
    }
}
