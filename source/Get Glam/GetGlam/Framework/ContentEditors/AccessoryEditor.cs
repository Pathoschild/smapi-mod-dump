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
    public class AccessoryEditor
    {
        // Instance of ModEntry
        private ModEntry Entry;

        // Instance of ContentPackHelper
        private ContentPackHelper PackHelper;

        // The current asset being edited
        private IAssetData Asset;

        // The AccessoryTexture starting Y, default: 64
        private static int AccessoryTextureHeight = 64;

        // The AccessoryTexture starting X, default: 48
        private int AccessoryTextureWidth = 48;

        // Max height allowed for the texture
        private const int MaxHeightOfAccessoryTexture = 4096;

        // Height of a single accessory
        private const int SingleAccessoryHeight = 32;

        // Width of a single accessory
        private const int SingleAccessoryWidth = 16;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entry">Instance of ModEntry</param>
        /// <param name="packHelper">Instance of PackHelper</param>
        /// <param name="asset">Current Asset being edited</param>
        public AccessoryEditor(ModEntry entry, ContentPackHelper packHelper, IAssetData asset)
        {
            Entry = entry;
            PackHelper = packHelper;
            Asset = asset;
        }

        /// <summary>
        /// Edits the accessory texture.
        /// </summary>
        public void EditAccessoryTexture()
        {
            // If the asset is accessories
            if (Asset.AssetNameEquals("Characters\\Farmer\\accessories"))
            {
                CreateNewTexture();

                // Loop through each accessory loaded and extend the image
                foreach (var accessory in PackHelper.AccessoryList)
                {
                    // Ints to run through the current accessory.png to find each accessory within the png
                    int accessoryTextureX = 0;
                    int accessoryTextureY = 0;

                    for (int i = 0; i < accessory.NumberOfAccessories; i++)
                    {
                        if ((AccessoryTextureHeight + SingleAccessoryHeight) >= MaxHeightOfAccessoryTexture)
                        {
                            Entry.Monitor.Log($"{accessory.ModName} accessories cannot be added to the game, the texture is too big.", LogLevel.Error);
                            return;
                        }

                        try
                        {
                            PatchAccessoryTexture(accessory, accessoryTextureX, accessoryTextureY);
                            ChangeHairLocationOnSourceTexture(ref accessoryTextureX, ref accessoryTextureY);
                            ChangeHairLocationOnNewTexture();
                        }
                        catch 
                        {
                            Entry.Monitor.Log($"{accessory.ModName} NumberOfAccessories is wrong. Some accessories may have been added anyway causing blank ones to show in between packs.", LogLevel.Error);
                            break;
                        }
                    }
                }

                // Cut the blank image from the image
                CutEmptyImage(Asset, AccessoryTextureHeight, 128);
            }
        }

        /// <summary>
        /// Creates a new texture from the data of the old texture.
        /// </summary>
        private void CreateNewTexture()
        {
            Texture2D oldTexture = Asset.AsImage().Data;
            Texture2D newTexture = new Texture2D(Game1.graphics.GraphicsDevice, oldTexture.Width, Math.Max(oldTexture.Height, MaxHeightOfAccessoryTexture));
            Asset.ReplaceWith(newTexture);
            Asset.AsImage().PatchImage(oldTexture);
        }

        /// <summary>
        /// Patches the accessory texture.
        /// </summary>
        /// <param name="accessory">Current accessory patch</param>
        /// <param name="accessoryTextureX">X where texture is being patched</param>
        /// <param name="accessoryTextureY">Y where texture is being patched</param>
        private void PatchAccessoryTexture(AccessoryModel accessory, int accessoryTextureX, int accessoryTextureY)
        {
            Asset.AsImage().PatchImage(
                accessory.Texture,
                new Rectangle(accessoryTextureX, accessoryTextureY, SingleAccessoryWidth, SingleAccessoryHeight),
                new Rectangle(AccessoryTextureWidth, AccessoryTextureHeight, SingleAccessoryWidth, SingleAccessoryHeight));
        }

        /// <summary>
        /// Changes hair location for patching on source texture.
        /// </summary>
        /// <param name="accessoryTextureX">X where the current accessory is</param>
        /// <param name="accessoryTextureY">Y where the current accessory is</param>
        private static void ChangeHairLocationOnSourceTexture(ref int accessoryTextureX, ref int accessoryTextureY)
        {
            if (accessoryTextureX + SingleAccessoryWidth == 128)
            {
                accessoryTextureX = 0;
                accessoryTextureY += SingleAccessoryHeight;
            }
            else
                accessoryTextureX += SingleAccessoryWidth;
        }

        /// <summary>
        /// Changes the accessory location for patching on new texture.
        /// </summary>
        private void ChangeHairLocationOnNewTexture()
        {
            if (AccessoryTextureWidth + SingleAccessoryWidth == 128)
            {
                AccessoryTextureWidth = 0;
                AccessoryTextureHeight += SingleAccessoryHeight;
            }
            else
                AccessoryTextureWidth += SingleAccessoryWidth;
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
