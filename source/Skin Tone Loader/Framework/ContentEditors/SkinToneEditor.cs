/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/SkinToneLoader
**
*************************************************/


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkinToneLoader.Framework.DataModels;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.HomeRenovations;
using StardewValley.Objects;
using System;
using System.IO;

namespace SkinToneLoader.Framework.ContentEditors
{
    public class SkinToneEditor
    {
        // Instance of Mod Entry
        private ModEntry Entry;

        // Instance of ContentPackHelper
        private ContentPackHelper PackHelper;

        // The current asset being edited
        public IAssetData Asset;

        // The skin tone starting Y, default: 24
        private static int SkinToneTextureHeight = 24;

        // Was the skin asset already edited
        private bool WasSkinEdited = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entry">Instance of ModEntry</param>
        /// <param name="packHelper">Instance of ContentPackHelper</param>
        /// <param name="asset">Current Asset being edited</param>
        public SkinToneEditor(ModEntry entry, ContentPackHelper packHelper, IAssetData asset)
        {
            Entry = entry;
            PackHelper = packHelper;
            Asset = asset;
        }

        public SkinToneEditor(ModEntry entry, ContentPackHelper packHelper)
        {
            Entry = entry;
            PackHelper = packHelper;
        }


        /// <summary>
        /// Edits the skin texture.
        /// </summary>
        public void EditSkinTexture()
        {

            //Break if the skin colors were already edited

            if (WasSkinEdited)
                return;

            Texture2D originalTexture = Asset.AsImage().Data;

            CreateNewTexture();

            // Loop through each skin color loaded and extend the image
            foreach (var skinTone in PackHelper.SkinToneList)
            {
                if (skinTone.Texture.Height + SkinToneTextureHeight > 4096)
                {
                    Entry.Monitor.Log($"{skinTone.ModName} skin tones cannot be added to the game, the texture is too big. Limit is 4096 skin tones.", LogLevel.Alert);
                    return;
                }

                PatchSkinTexture(skinTone);
                SkinToneTextureHeight += skinTone.Texture.Height;
            }

            // Cut the blank image
            CutEmptyImage(Asset, SkinToneTextureHeight, 3);

            WasSkinEdited = true;

            ExportModdedTexture();

            Asset.AsImage().ReplaceWith(originalTexture);

            Entry.Monitor.Log("Skin texture replaced with a total of " + SkinToneTextureHeight + " skin tones.", LogLevel.Info);
        }

        private void ExportModdedTexture()
        {
            Texture2D myTexture = Asset.AsImage().Data;

            Stream stream = File.Create(Entry.moddedSkinColorsPathString);
            myTexture.SaveAsPng(stream, myTexture.Width, myTexture.Height);
            stream.Dispose();
        }

        /// <summary>
        /// Creates a new texture from the data of the old texture.
        /// </summary>
        private void CreateNewTexture()
        {
            Texture2D oldTexture = Asset.AsImage().Data;
            Texture2D newTexture = new Texture2D(Game1.graphics.GraphicsDevice, oldTexture.Width, 4096);

            Asset.ReplaceWith(newTexture);
            Asset.AsImage().PatchImage(oldTexture);
        }


        /// <summary>
        /// Patches the skin texture.
        /// </summary>
        /// <param name="skinTone">Current skin tone patch</param>
        private void PatchSkinTexture(SkinToneModel skinTone)
        {
            Asset.AsImage().PatchImage(
                skinTone.Texture,
                null, targetArea: new Rectangle(0, SkinToneTextureHeight, skinTone.Texture.Width, skinTone.Texture.Height));
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
            asset.AsImage().PatchImage(oldTexture, sourceArea: new Rectangle(0, 0, newWidth, newHeight), targetArea: new Rectangle(0, 0, newWidth, newHeight));
        }

        /// <summary>
        /// Get the number of skin color, used in SkinTonePatch.
        /// </summary>
        /// <returns>The number of Skin Tones</returns>
        public static int GetNumberOfSkinTones()
        {
            return SkinToneTextureHeight;
        }

        /// <summary>
        /// Get the number of skin tone minus one, used in SkinTonePatch.
        /// </summary>
        /// <returns>The number of Skin Colors minus one</returns>
        public static int GetNumberOfSkinToneMinusOne()
        {
            return SkinToneTextureHeight - 1;
        }
    }
}
