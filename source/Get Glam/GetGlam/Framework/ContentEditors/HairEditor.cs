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
    public class HairEditor
    {
        // Instance of Mod Entry
        private ModEntry Entry;

        // Instance of ContentPackHelper
        private ContentPackHelper PackHelper;

        // The current asset being edited
        private IAssetData Asset;

        // The HairTexture height, default: 672
        private int HairTextureHeight = 672;

        // The width of where to put the hair in the asset
        private int HairTextureWidth = 0;

        // The Max allowed texture height without SpaceCore
        private const int MaxHairTextureHeight = 4032;

        // The height of a single hairstyle
        private const int SingleHairstyleHeight = 96;

        // The width of a single hairstyle
        private const int SingleHairstyleWidth = 16;

        // Max amount of hairstyles added to the texture without SpaceCore
        private const int MaxHairStylesForSingleTexture = 335;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entry">Instance of ModEntry.</param>
        /// <param name="packHelper">Instance of ContentPackHelper</param>
        /// <param name="asset">Current asset being edited.</param>
        public HairEditor(ModEntry entry, ContentPackHelper packHelper, IAssetData asset)
        {
            Entry = entry;
            PackHelper = packHelper;
            Asset = asset;
        }

        /// <summary>
        /// Edits the hairstyles texture.
        /// </summary>
        public void EditHairTexture()
        {
            // If the asset is hairstyles
            if (Asset.AssetNameEquals("Characters\\Farmer\\hairstyles"))
            {
                // Don't edit if they have no hair content packs
                if (PackHelper.NumberOfHairstlyesAdded == 74)
                    return;

                CreateNewTexture();

                // Loop through each hair loaded and extend the image
                foreach (var hair in PackHelper.HairList)
                {
                    Entry.Monitor.Log($"Patching {hair.ModName}.", LogLevel.Trace);

                    // Ints to run through the current hairstyles.png to find each hairstyle within the png
                    int hairTextureX = 0;
                    int hairTextureY = 0;

                    for (int i = 0; i < hair.NumberOfHairstyles; i++)
                    {
                        if ((HairTextureHeight + SingleHairstyleHeight) >= MaxHairTextureHeight && !Entry.IsSpaceCoreInstalled)
                        {
                            Entry.Monitor.Log($"{hair.ModName} hairstyles cannot be added to the game, the texture is too big.", LogLevel.Error);
                            return;
                        }

                        try
                        {
                            PatchHairTexture(hair, hairTextureX, hairTextureY);
                            ChangeHairLocationOnSourceTexture(hair, ref hairTextureX, ref hairTextureY);
                            ChangeHairLocationOnNewTexture();
                        }
                        catch 
                        {
                            Entry.Monitor.Log($"{hair.ModName} NumberOfHairstyles is wrong. Some hairstyles may have been added anyway causing baldness in between hair packs.", LogLevel.Error);
                            break;
                        }
                    }
                }

                //CutBlankImage();
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
        /// Patches the hair texture.
        /// </summary>
        /// <param name="hair">Current hair patched</param>
        /// <param name="hairTextureX">X where texture is being patched</param>
        /// <param name="hairTextureY">Y where texture is being patched</param>
        private void PatchHairTexture(HairModel hair, int hairTextureX, int hairTextureY)
        {
                if (Entry.IsSpaceCoreInstalled)
                    Entry.HarmonyHelper.SpaceCorePatchExtendedTileSheet(Asset.AsImage(), hair.Texture, new Rectangle(hairTextureX, hairTextureY, SingleHairstyleWidth, SingleHairstyleHeight), new Rectangle(HairTextureWidth, HairTextureHeight, SingleHairstyleWidth, SingleHairstyleHeight));
                else
                    Asset.AsImage().PatchImage(hair.Texture, new Rectangle(hairTextureX, hairTextureY, SingleHairstyleWidth, SingleHairstyleHeight), new Rectangle(HairTextureWidth, HairTextureHeight, SingleHairstyleWidth, SingleHairstyleHeight));
        }

        /// <summary>
        /// Changes hair location for patching on source texture.
        /// </summary>
        /// <param name="hairTextureX">X where the current hair is</param>
        /// <param name="hairTextureY">Y where the current hair is</param>
        private void ChangeHairLocationOnSourceTexture(HairModel hairModel, ref int hairTextureX, ref int hairTextureY)
        {
            if (hairTextureX + SingleHairstyleWidth == 128)
            {
                hairTextureX = 0;
                hairTextureY += hairModel.ModName.Equals("Hairstyles 2") ? SingleHairstyleHeight + 32 : SingleHairstyleHeight;
            }
            else
                hairTextureX += SingleHairstyleWidth;
        }

        /// <summary>
        /// Changes hair location for patching on new texture.
        /// </summary>
        private void ChangeHairLocationOnNewTexture()
        {
            if (HairTextureWidth + SingleHairstyleWidth >= 128)
            {
                HairTextureWidth = 0;
                HairTextureHeight += SingleHairstyleHeight;
            }
            else
                HairTextureWidth += SingleHairstyleWidth;
        }

        /// <summary>
        /// Cuts the empty image based on installed optional mods.
        /// </summary>
        private void CutBlankImage()
        {
            if (!Entry.IsSpaceCoreInstalled || PackHelper.NumberOfHairstlyesAdded < MaxHairStylesForSingleTexture)
                CutEmptyImage(Asset, HairTextureHeight, 128);
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
