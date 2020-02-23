using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.IO;

namespace GetGlam.Framework
{
    /// <summary>Class used to inject new textures into the games content.</summary>
    public class ImageInjector : IAssetEditor
    {
        //Instance of ModEntry
        private ModEntry Entry;

        //Instance of ContentPackHelper
        private ContentPackHelper PackHelper;

        //The HairTexture height, default: 672
        private int HairTextureHeight = 672;

        //The width of where to put the hair in the asset
        private int HairTextureWidth = 0;

        //The AccessoryTexture starting Y, default: 64
        private static int AccessoryTextureHeight = 64;

        //The AccessoryTextures starting X, default: 48
        private int AccessoryTextureWidth = 48;

        //The skin color starting Y, default: 24
        private static int SkinColorTextureHeight = 24;

        //The DresserTexture Height, default: 32
        private int DresserTextureHeight = 32;

        //Was the Dresser image already edited???
        private bool WasDresserImageEdited = false;

        //Why do I even need this?
        private int SkinEditedCounter = 0;

        /// <summary>Image Injectors Constructor</summary>
        /// <param name="entry"></param>
        /// <param name="packHelper"></param>
        public ImageInjector(ModEntry entry, ContentPackHelper packHelper)
        {
            //Set the vars to the instances
            Entry = entry;
            PackHelper = packHelper;
        }

        /// <summary>Wether SMAPI's Asset Editor can edit a specific asset.</summary>
        /// <typeparam name="T">The Type of asset</typeparam>
        /// <param name="asset">The asset in question</param>
        /// <returns>Whether it can load the specific asset</returns>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Characters\\Farmer\\hairstyles"))
                return true;
            else if (asset.AssetNameEquals("Characters\\Farmer\\accessories"))
                return true;
            else if (asset.AssetNameEquals("Characters\\Farmer\\skinColors"))
                return true;
            else if (asset.AssetNameEquals($"Mods/{Entry.ModManifest.UniqueID}/dresser.png"))
                return true;

            return false;
        }

        /// <summary>SMAPI's Asset Editor tries to edit a specific asset.</summary>
        /// <typeparam name="T">The Type of asset</typeparam>
        /// <param name="asset">The asset in question</param>
        public void Edit<T>(IAssetData asset)
        {
            //If the asset is hairstyles
            if (asset.AssetNameEquals("Characters\\Farmer\\hairstyles"))
            {
                //Don't edit if they have no hair content packs
                if (PackHelper.NumberOfHairstlyesAdded == 56)
                    return;

                //Create a new texture and set it as the old one
                Texture2D oldTexture = asset.AsImage().Data;
                Texture2D newTexture = new Texture2D(Game1.graphics.GraphicsDevice, oldTexture.Width, Math.Max(oldTexture.Height, 4096));
                asset.ReplaceWith(newTexture);
                asset.AsImage().PatchImage(oldTexture);

                //Loop through each hair loaded and extend the image
                foreach (var hair in PackHelper.HairList)
                {
                    Entry.Monitor.Log($"Patching {hair.ModName}", LogLevel.Trace);

                    //Ints to run throught the current hairstyles.png to find each hairstyle within the png
                    int hairTextureX = 0;
                    int hairTextureY = 0;

                    for (int i = 0; i < hair.NumberOfHairstyles; i++)
                    {
                        if ((HairTextureHeight + 96) >= 4032 && !Entry.IsSpaceCoreInstalled)
                        {
                            Entry.Monitor.Log($"{hair.ModName} hairstyles cannot be added to the game, the texture is too big.", LogLevel.Error);
                            return;
                        }

                        //Patch the hair texture and change the hair texture height
                        if (Entry.IsSpaceCoreInstalled)
                            Entry.SpaceCorePatchExtendedTileSheet(asset.AsImage(), hair.Texture, new Rectangle(hairTextureX, hairTextureY, 16, 96), new Rectangle(HairTextureWidth, HairTextureHeight, 16, 96));
                        else
                            asset.AsImage().PatchImage(hair.Texture, new Rectangle(hairTextureX, hairTextureY, 16, 96), new Rectangle(HairTextureWidth, HairTextureHeight, 16, 96));

                        //Change which hair is being added from the source texture
                        if (hairTextureX + 16 == 128)
                        {
                            hairTextureX = 0;
                            hairTextureY += 96;
                        }
                        else
                            hairTextureX += 16;

                        //Change where to put the hair on the asset
                        if (HairTextureWidth + 16 >= 128)
                        {
                            HairTextureWidth = 0;
                            HairTextureHeight += 96;
                        }
                        else
                            HairTextureWidth += 16;
                    }
                }

                //Cut the blank image from the image
                if (!Entry.IsSpaceCoreInstalled || PackHelper.NumberOfHairstlyesAdded < 335)
                    CutEmptyImage(asset, HairTextureHeight, 128);
            }

            //If the asset is accessories
            if (asset.AssetNameEquals("Characters\\Farmer\\accessories"))
            {
                //Create a new texture and set it as the old one
                Texture2D oldTexture = asset.AsImage().Data;
                Texture2D newTexture = new Texture2D(Game1.graphics.GraphicsDevice, oldTexture.Width, Math.Max(oldTexture.Height, 4096));
                asset.ReplaceWith(newTexture);
                asset.AsImage().PatchImage(oldTexture);
    
                //Loop through each accessory loaded and extend the image
                foreach (var accessory in PackHelper.AccessoryList)
                {
                    //Ints to run throught the current hairstyles.png to find each hairstyle within the png
                    int accessoryTextureX = 0;
                    int accessoryTextureY = 0;

                    for (int i = 0; i < accessory.NumberOfAccessories; i++)
                    {
                        if ((AccessoryTextureHeight + 96) >= 4096)
                        {
                            Entry.Monitor.Log($"{accessory.ModName} accessories cannot be added to the game, the texture is too big.", LogLevel.Error);
                            return;
                        }

                        //Patch the hair texture and change the hair texture height
                        asset.AsImage().PatchImage(accessory.Texture, new Rectangle(accessoryTextureX, accessoryTextureY, 16, 32), new Rectangle(AccessoryTextureWidth, AccessoryTextureHeight, 16, 32));

                        //Change which accessory is being added from the source texture
                        if (accessoryTextureX + 16 == 128)
                        {
                            accessoryTextureX = 0;
                            accessoryTextureY += 32;
                        }
                        else
                            accessoryTextureX += 16;

                        //Change where to put the accessory on the asset
                        if (AccessoryTextureWidth + 16 == 128)
                        {
                            AccessoryTextureWidth = 0;
                            AccessoryTextureHeight += 32;
                        }
                        else
                            AccessoryTextureWidth += 16;
                    }
                }

                //Cut the blank image from the image
                //CutEmptyImage(asset, AccessoryTextureHeight, 128);
            }

            if (asset.AssetNameEquals("Characters\\Farmer\\skinColors"))
            {
                //Break if the skin colors were already edited
                if (SkinEditedCounter != 4)
                {
                    SkinEditedCounter++;
                    return;
                }

                SkinEditedCounter = 0;
                //Create a new texture and set it as the old one
                Texture2D oldTexture = asset.AsImage().Data;
                Texture2D newTexture = new Texture2D(Game1.graphics.GraphicsDevice, oldTexture.Width, Math.Max(oldTexture.Height, 4096));
                asset.ReplaceWith(newTexture);
                asset.AsImage().PatchImage(oldTexture);

                //Loop through each skin color loaded and extend the image
                foreach (var skinColor in PackHelper.SkinColorList)
                {
                    if ((skinColor.TextureHeight + SkinColorTextureHeight) > 4096)
                    {
                        Entry.Monitor.Log($"{skinColor.ModName} skin colors cannot be added to the game, the texture is too big. Please show this Alert to MartyrPher and I guess they'll have to extend the skinColor image...uhh yea. Why do you need 4096 skin colors anyway?", LogLevel.Alert);
                        return;
                    }

                    //Patch the skin color and change the skin color height
                    asset.AsImage().PatchImage(skinColor.Texture, null, new Rectangle(0, SkinColorTextureHeight, 3, skinColor.TextureHeight));
                    SkinColorTextureHeight += skinColor.TextureHeight;
                }

                //Cut the blank image
                CutEmptyImage(asset, SkinColorTextureHeight, 3);
            }

            //If the asset is the dresser
            if (asset.AssetNameEquals($"Mods/{Entry.ModManifest.UniqueID}/dresser.png"))
            {
                //Break if the image was already edited
                if (WasDresserImageEdited)
                    return;

                //Set dresser was edited and replace the old tecture with a new one
                WasDresserImageEdited = true;
                Texture2D oldTexture = asset.AsImage().Data;
                Texture2D newTexture = new Texture2D(Game1.graphics.GraphicsDevice, 16, Math.Max(oldTexture.Height, 4096));
                asset.ReplaceWith(newTexture);
                asset.AsImage().PatchImage(oldTexture);

                //Loop through each dresser and patch the image
                foreach (var dresser in PackHelper.DresserList)
                {
                    if ((dresser.TextureHeight + DresserTextureHeight) > 4096)
                    {
                        Entry.Monitor.Log($"{dresser.ModName} dressers cannot be added to the game, the texture is too big.", LogLevel.Warn);
                        return;
                    }

                    //Patch the dresser.png and adjust the height
                    asset.AsImage().PatchImage(dresser.Texture, null, new Rectangle(0, DresserTextureHeight, 16, dresser.TextureHeight));
                    DresserTextureHeight += dresser.TextureHeight;
                }

                Entry.Monitor.Log($"Dresser Image height is now: {DresserTextureHeight}", LogLevel.Trace);

                //Cut the empty image from the dresser texture
                CutEmptyImage(asset, DresserTextureHeight, 16);

                //Save the dresser to the mod folder so it can be used to create a TileSheet for the Farmhouse
                FileStream stream = new FileStream(Path.Combine(Entry.Helper.DirectoryPath, "assets", "dresser.png"), FileMode.Create);
                asset.AsImage().Data.SaveAsPng(stream, 16, DresserTextureHeight);
                stream.Close();
            }
        }

        /// <summary>Get the number of accessories, used in AccessoryPatch</summary>
        /// <returns>The number of Accessories</returns>
        public static int GetNumberOfAccessories()
        {
            return ContentPackHelper.NumberOfAccessoriesAdded;
        }

        /// <summary>Get the number of accessories minus one, used in AccessoryPatch</summary>
        /// <returns>The number of Accessories minus one</returns>
        public static int GetNumberOfAccessoriesMinusOne()
        {
            return ContentPackHelper.NumberOfAccessoriesAdded - 1;
        }

        /// <summary>Get the number of skin color, used in SkinColorPatch</summary>
        /// <returns>The number of Skin Colors</returns>
        public static int GetNumberOfSkinColor()
        {
            return SkinColorTextureHeight;
        }

        /// <summary>Get the number of skin color minus one, used in SkinColorPatch</summary>
        /// <returns>The number of Skin Colors minus one</returns>
        public static int GetNumberOfSkinColorMinusOne()
        {
            return SkinColorTextureHeight - 1;
        }

        /// <summary>Cuts the empty image from the texture.</summary>
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
