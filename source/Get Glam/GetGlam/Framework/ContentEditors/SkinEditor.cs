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

namespace GetGlam.Framework.ContentEditors
{
    public class SkinEditor
    {
        // Instance of Mod Entry
        private ModEntry Entry;

        // Instance of ContentPackHelper
        private ContentPackHelper PackHelper;

        // The current asset being edited
        private IAssetData Asset;

        // The skin color starting Y, default: 24
        private static int SkinColorTextureHeight = 24;

        // Was the skin asset already edited
        private bool WasSkinEdited = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entry">Instance of ModEntry</param>
        /// <param name="packHelper">Instance of ContentPackHelper</param>
        /// <param name="asset">Current Asset being edited</param>
        public SkinEditor(ModEntry entry, ContentPackHelper packHelper, IAssetData asset) 
        {
            Entry = entry;
            PackHelper = packHelper;
            Asset = asset;
        }

        /// <summary>
        /// Edits the skin texture.
        /// </summary>
        public void EditSkinTexture()
        {
            if (Asset.AssetNameEquals("Characters\\Farmer\\skinColors"))
            {
                // Break if the skin colors were already edited
                if (WasSkinEdited)
                    return;

                WasSkinEdited = true;

                CreateNewTexture();

                // Loop through each skin color loaded and extend the image
                foreach (var skinColor in PackHelper.SkinColorList)
                {
                    if ((skinColor.TextureHeight + SkinColorTextureHeight) > 4096)
                    {
                        Entry.Monitor.Log($"{skinColor.ModName} skin colors cannot be added to the game, the texture is too big. Please show this Alert to MartyrPher and I guess they'll have to extend the skinColor image...uhh yea. Why do you need 4096 skin colors anyway?", LogLevel.Alert);
                        return;
                    }

                    PatchSkinTexture(skinColor);
                    SkinColorTextureHeight += skinColor.TextureHeight;
                }

                // Cut the blank image
                CutEmptyImage(Asset, SkinColorTextureHeight, 3);
            }
        }

        /// <summary>
        /// Creates a new texture from the data of the old texture.
        /// </summary>
        private void CreateNewTexture()
        {
            Texture2D oldTexture = Asset.AsImage().Data;
            Texture2D newTexture = new Texture2D(Game1.graphics.GraphicsDevice, oldTexture.Width, Math.Max(oldTexture.Height, 4096));
            Asset.ReplaceWith(newTexture);
            Asset.AsImage().PatchImage(oldTexture);
        }

        /// <summary>
        /// Patches the skin texture.
        /// </summary>
        /// <param name="skinColor">Current skin color patch</param>
        private void PatchSkinTexture(SkinColorModel skinColor)
        {
            Asset.AsImage().PatchImage(
                skinColor.Texture,
                null,
                new Rectangle(0, SkinColorTextureHeight, 3, skinColor.TextureHeight));
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

        /// <summary>
        /// Get the number of skin color, used in SkinColorPatch.
        /// </summary>
        /// <returns>The number of Skin Colors</returns>
        public static int GetNumberOfSkinColor()
        {
            return SkinColorTextureHeight;
        }

        /// <summary>
        /// Get the number of skin color minus one, used in SkinColorPatch.
        /// </summary>
        /// <returns>The number of Skin Colors minus one</returns>
        public static int GetNumberOfSkinColorMinusOne()
        {
            return SkinColorTextureHeight - 1;
        }
    }
}
