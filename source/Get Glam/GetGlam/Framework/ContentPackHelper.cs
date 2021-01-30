/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MartyrPher/GetGlam
**
*************************************************/

using GetGlam.Framework.ContentLoaders;
using GetGlam.Framework.DataModels;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.Collections.Generic;

namespace GetGlam.Framework
{
    /// <summary>
    /// Class that handles loading Content Packs.
    /// </summary>
    public class ContentPackHelper
    {
        // Instance of ModEntry
        private ModEntry Entry;

        // List of Hair Models
        public List<HairModel> HairList = new List<HairModel>();

        // List of Accessory Models
        public List<AccessoryModel> AccessoryList = new List<AccessoryModel>();

        // List of Dresser Models
        public List<DresserModel> DresserList = new List<DresserModel>();

        // List of Skin Color Models
        public List<SkinColorModel> SkinColorList = new List<SkinColorModel>();

        // List of Female base textures
        public List<Texture2D> FemaleBaseTextureList = new List<Texture2D>();

        // List of Female base bald textures
        public List<Texture2D> FemaleBaseBaldTextureList = new List<Texture2D>();

        // List of Male base textures
        public List<Texture2D> MaleBaseTextureList = new List<Texture2D>();

        // List of Male base bald textures
        public List<Texture2D> MaleBaseBaldTextureList = new List<Texture2D>();

        // List of Female shoe textures
        public List<Texture2D> FemaleShoeTextureList = new List<Texture2D>();

        // List of Male shoe textures
        public List<Texture2D> MaleShoeTextureList = new List<Texture2D>();

        // Dictionary of male base Textures that has a Dictionary of filenames and the noseFace textures, yo dawg heard ya like dictionaries
        public Dictionary<Texture2D, Dictionary<string, Texture2D>> MaleFaceAndNoseTextureDict = new Dictionary<Texture2D, Dictionary<string, Texture2D>>();

        // Dictionary that houses the quantities of the noses and faces for a male base texture
        public Dictionary<Texture2D, int[]> MaleBaseFaceNoseCount = new Dictionary<Texture2D, int[]>();

        // Dictionary of female base textures that has a Dictionary of filenames and the noseFace textures
        public Dictionary<Texture2D, Dictionary<string, Texture2D>> FemaleFaceAndNoseTextureDict = new Dictionary<Texture2D, Dictionary<string, Texture2D>>();

        // Dictionary that house the qunatities of the noses and faces for a femal base texture
        public Dictionary<Texture2D, int[]> FemaleBaseFaceNoseCount = new Dictionary<Texture2D, int[]>();

        // Number of hairstyles added by content packs including default hairs
        public int NumberOfHairstlyesAdded = 56;

        // Number of accessories added by content packs including default accessories
        public static int NumberOfAccessoriesAdded = 19;

        // The Height Of The Dresser Texture
        public int DresserTextureHeight = 32;

        // Dresser Texure (It's here since it gets saved to a file)
        public Texture2D DresserTexture;

        // Dictionary Used for Jumping to specific Hair Pack.
        public Dictionary<string, int> HairStyleSearch = new Dictionary<string, int>();

        /// <summary>
        /// ContentPackHelpers Contructor.
        /// </summary>
        /// <param name="entry">An instance of <see cref="ModEntry"/></param>
        public ContentPackHelper(ModEntry entry)
        {
            // Set the var to the instance
            Entry = entry;
            HairStyleSearch.Add("Default", 0);
        }

        /// <summary>
        /// Reads all the content packs for the mod.
        /// </summary>
        public void ReadContentPacks()
        {
            AddHairstylesTwo();

            // Loop through each content pack
            foreach (IContentPack contentPack in Entry.Helper.ContentPacks.GetOwned())
            {
                Entry.Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version}", LogLevel.Trace);

                LoadHair(contentPack);
                LoadAccessories(contentPack);
                LoadBase(contentPack);
                LoadDresser(contentPack);
                LoadShoes(contentPack);
                LoadFaceAndNose(contentPack);
                LoadSkinColor(contentPack);
            }

            // Add ImageInjector to the Asset Editor to start patching the images
            Entry.Helper.Content.AssetEditors.Add(new ImageInjector(Entry, this));
        }

        /// <summary>
        /// Loads the hairstyles two image.
        /// </summary>
        private void AddHairstylesTwo()
        {
            HairLoader hairLoader = new HairLoader(Entry, null, this);
            hairLoader.LoadHairstyleTwo();
        }

        /// <summary>
        /// Loads the Hair for a Content Pack.
        /// </summary>
        /// <param name="contentPack">The current Content Pack.</param>
        private void LoadHair(IContentPack contentPack)
        {
            HairLoader hairLoader = new HairLoader(Entry, contentPack, this);
            hairLoader.LoadHair();      
        }

        /// <summary>
        /// Loads the accessories for a Content Pack.
        /// </summary>
        /// <param name="contentPack">The current Content Pack.</param>
        private void LoadAccessories(IContentPack contentPack)
        {
            AccessoryLoader accessoryLoader = new AccessoryLoader(Entry, contentPack, this);
            accessoryLoader.LoadAccessory();
        }

        /// <summary>
        /// Loads the bases for a Content Pack.
        /// </summary>
        /// <param name="contentPack">The current Content Pack.</param>
        private void LoadBase(IContentPack contentPack)
        {
            BaseLoader baseLoader = new BaseLoader(Entry, contentPack, this);
            baseLoader.LoadBase();
        }

        /// <summary>
        /// Loads the Dresser for a Content Pack.
        /// </summary>
        /// <param name="contentPack">The Current Content Pack.</param>
        private void LoadDresser(IContentPack contentPack)
        {
            DresserLoader dresserLoader = new DresserLoader(Entry, contentPack, this);
            dresserLoader.LoadDresser();
        }

        /// <summary>
        /// Loads shoes from a Content Pack.
        /// </summary>
        /// <param name="contentPack">The Current Content Pack.</param>
        private void LoadShoes(IContentPack contentPack)
        {
            ShoeLoader shoeLoader = new ShoeLoader(Entry, contentPack, this);
            shoeLoader.LoadShoes();
        }

        /// <summary>
        /// Loads Faces and Noses from a Content Pack.
        /// </summary>
        /// <param name="contentPack">The Current Content Pack.</param>
        private void LoadFaceAndNose(IContentPack contentPack)
        {
            FaceNoseLoader faceNoseLoader = new FaceNoseLoader(Entry, contentPack, this);
            faceNoseLoader.LoadFaceAndNose();
        }

        /// <summary>
        /// Load Skin Color from a Content Pack.
        /// </summary>
        /// <param name="contentPack">The Current Content Pack.</param>
        private void LoadSkinColor(IContentPack contentPack)
        {
            SkinColorLoader skinColorLoader = new SkinColorLoader(Entry, contentPack, this);
            skinColorLoader.LoadSkinColor();
        }

        /// <summary>
        /// Gets the number of faces and noses for a base texture.
        /// </summary>
        /// <param name="isMale">Whether the player is male</param>
        /// <param name="baseIndex">The base texture index</param>
        /// <param name="isFace">Wether it's looking for the face count</param>
        /// <returns>The count of faces or noses</returns>
        public int GetNumberOfFacesAndNoses(bool isMale, int baseIndex, bool isFace)
        {
            // This is static so we return the default values
            if (baseIndex.Equals(0) && isFace)
                return 1;
            else if (baseIndex.Equals(0) && !isFace)
                return 2;

            // Try...catch incase the Key is not in the dictionary
            try
            {
                if (isFace && isMale)
                    return MaleBaseFaceNoseCount[MaleBaseTextureList[baseIndex - 1]][0] - 1;
                else if (!isFace && isMale)
                    return MaleBaseFaceNoseCount[MaleBaseTextureList[baseIndex - 1]][1] - 1;
                else if (isFace && !isMale)
                    return FemaleBaseFaceNoseCount[FemaleBaseTextureList[baseIndex - 1]][0] - 1;
                else if (!isFace && !isMale)
                    return FemaleBaseFaceNoseCount[FemaleBaseTextureList[baseIndex - 1]][1] - 1;
            }
            catch
            {
                // If the key is not in the dictionary then give the default values
                return 0;
            }

            // Return 0 if it doesn't find anything
            return 0;
        }
    }
}
