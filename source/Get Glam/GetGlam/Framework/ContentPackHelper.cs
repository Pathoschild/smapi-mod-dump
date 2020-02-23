using GetGlam.Framework.DataModels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;

namespace GetGlam.Framework
{
    /// <summary>Class that handles loading Content Packs and changing base textures.</summary>
    public class ContentPackHelper
    {
        //Instance of ModEntry
        private ModEntry Entry;

        //List of Hair Models
        public List<HairModel> HairList = new List<HairModel>();

        //List of Accessory Models
        public List<AccessoryModel> AccessoryList = new List<AccessoryModel>();

        //List of Dresser Models
        public List<DresserModel> DresserList = new List<DresserModel>();

        //List of Skin Color Models
        public List<SkinColorModel> SkinColorList = new List<SkinColorModel>();

        //List of Female base textures
        public List<Texture2D> FemaleBaseTextureList = new List<Texture2D>();

        //List of Female base bald textures
        public List<Texture2D> FemaleBaseBaldTextureList = new List<Texture2D>();

        //List of Male base textures
        public List<Texture2D> MaleBaseTextureList = new List<Texture2D>();

        //List of Male base bald textures
        public List<Texture2D> MaleBaseBaldTextureList = new List<Texture2D>();

        //List of Female shoe textures
        public List<Texture2D> FemaleShoeTextureList = new List<Texture2D>();

        //List of Male shoe textures
        public List<Texture2D> MaleShoeTextureList = new List<Texture2D>();

        //Dictionary of male base Textures that has a Dictionary of filenames and the noseFace textures, yo dawg heard ya like dictionaries
        public Dictionary<Texture2D, Dictionary<string, Texture2D>> MaleFaceAndNoseTextureDict = new Dictionary<Texture2D, Dictionary<string, Texture2D>>();

        //Dictionary that houses the quantities of the noses and faces for a male base texture
        public Dictionary<Texture2D, int[]> MaleBaseFaceNoseCount = new Dictionary<Texture2D, int[]>();

        //Dictionary of female base textures that has a Dictionary of filenames and the noseFace textures
        public Dictionary<Texture2D, Dictionary<string, Texture2D>> FemaleFaceAndNoseTextureDict = new Dictionary<Texture2D, Dictionary<string, Texture2D>>();

        //Dictionary that house the qunatities of the noses and faces for a femal base texture
        public Dictionary<Texture2D, int[]> FemaleBaseFaceNoseCount = new Dictionary<Texture2D, int[]>();

        //Wether the player is male
        private bool IsMale = false;

        //The index of the face
        private int FaceIndex = 0;

        //The index of the nose
        private int NoseIndex = 0;

        //The index of the base
        private int BaseIndex = 0;

        //The index of the shoe
        private int ShoeIndex = 0;

        //An array of tweaks for the male shoe sprite heights
        private int[] MaleShoeSpriteHeights = new int[21] { 11, 16, 15, 14, 13, 16, 16, 14, 16, 12, 14, 14, 15, 15, 16, 13, 15, 16, 16, 16, 15 };

        //An array of tweaks for the female shoe sprite heights
        private int[] FemaleShoeSpriteHeights = new int[21] { 15, 16, 14, 13, 12, 16, 16, 15, 16, 10, 13, 13, 13, 14, 14, 11, 14, 14, 14, 16, 13 };

        //Whether the player is bald
        public bool IsBald = false;

        //Number of hairstyles added by content packs including default hairs
        public int NumberOfHairstlyesAdded = 56;

        //Number of accessories added by content packs including default accessories
        public static int NumberOfAccessoriesAdded = 19;

        public Dictionary<string, int> HairStyleSearch = new Dictionary<string, int>();

        /// <summary>ContentPackHelpers Contructor</summary>
        /// <param name="entry">An instance of <see cref="ModEntry"/></param>
        public ContentPackHelper(ModEntry entry)
        {
            //Set the var to the instance
            Entry = entry;
            HairStyleSearch.Add("Default", 0);
        }

        /// <summary>Reads all the content packs for the mod</summary>
        public void ReadContentPacks()
        {
            //Loop through each content pack
            foreach (IContentPack contentPack in Entry.Helper.ContentPacks.GetOwned())
            {
                //Create new Directory infos for all the different folders
                Entry.Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version}", LogLevel.Trace);
                DirectoryInfo hairDirectory = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Hairstyles"));
                DirectoryInfo accessoriesDirectory = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Accessories"));
                DirectoryInfo baseDirectory = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Base"));
                DirectoryInfo dresserDirectory = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Dresser"));
                DirectoryInfo shoeDirectory = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Shoes"));
                DirectoryInfo faceAndNoseDirectory = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "FaceAndNose"));
                DirectoryInfo skinColorDirectory = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "SkinColor"));

                //If the hair directory exists
                if (hairDirectory.Exists)
                {
                    HairModel hair;
                    try
                    {
                        //Create a new hair model
                        hair = contentPack.ReadJsonFile<HairModel>("Hairstyles/hairstyles.json");
                        hair.Texture = contentPack.LoadAsset<Texture2D>("Hairstyles/hairstyles.png");

                        //Set the vars in the hair model
                        hair.TextureHeight = hair.Texture.Height;
                        hair.ModName = contentPack.Manifest.Name;

                        HairStyleSearch.Add(contentPack.Manifest.Name, NumberOfHairstlyesAdded);
                        NumberOfHairstlyesAdded += hair.NumberOfHairstyles;

                        //Add the hair model to the list
                        HairList.Add(hair);
                    }
                    catch
                    {
                        Entry.Monitor.Log($"{contentPack.Manifest.Name} hairstyles is emtpy. This pack was not added", LogLevel.Warn);
                    }
                }

                //If the accessories directory exists
                if (accessoriesDirectory.Exists)
                {
                    AccessoryModel accessory;
                    try
                    {
                        //Create the new accessory model
                        accessory = contentPack.ReadJsonFile<AccessoryModel>("Accessories/accessories.json");
                        accessory.Texture = contentPack.LoadAsset<Texture2D>("Accessories/accessories.png");

                        //Set the vars in the accessory model
                        accessory.TextureHeight = accessory.Texture.Height;
                        accessory.ModName = contentPack.Manifest.Name;
                        NumberOfAccessoriesAdded += accessory.NumberOfAccessories;

                        //Add the accessory to the accessory list
                        AccessoryList.Add(accessory);
                    }
                    catch
                    {
                        Entry.Monitor.Log($"{contentPack.Manifest.Name} accessories is emtpy. This pack was not added", LogLevel.Warn);
                    }
                }

                //If the base directory exists
                if (baseDirectory.Exists)
                {
                    //Loop through each file in the folder
                    foreach (FileInfo file in baseDirectory.EnumerateFiles())
                    {
                        //Check the name if the file and load the texture into the relevant list
                        if (file.Name.Contains("farmer_girl_base.png"))
                            FemaleBaseTextureList.Add(contentPack.LoadAsset<Texture2D>("Base/farmer_girl_base.png"));
                        else if (file.Name.Contains("farmer_base.png"))
                            MaleBaseTextureList.Add(contentPack.LoadAsset<Texture2D>("Base/farmer_base.png"));
                        else if (file.Name.Contains("bald"))
                        {
                            if (file.Name.Contains("girl_base_bald"))
                                FemaleBaseBaldTextureList.Add(contentPack.LoadAsset<Texture2D>("Base/farmer_girl_base_bald.png"));
                            else if (file.Name.Contains("farmer_base_bald"))
                                MaleBaseBaldTextureList.Add(contentPack.LoadAsset<Texture2D>("Base/farmer_base_bald.png"));
                        }         
                    }
                }
        
                //If the dresser directory exists
                if (dresserDirectory.Exists)
                {
                    //Create a new dresser model
                    DresserModel dresser;
                    try
                    {
                        dresser = new DresserModel();
                        dresser.Texture = contentPack.LoadAsset<Texture2D>("Dresser/dresser.png");

                        //Set the vars of the Dresser model
                        dresser.TextureHeight = dresser.Texture.Height;
                        dresser.ModName = contentPack.Manifest.Name;

                        //Add the dresser model to the dresser model list
                        DresserList.Add(dresser);
                    }
                    catch
                    {
                        Entry.Monitor.Log($"{contentPack.Manifest.Name} dressers is emtpy. This pack was not added", LogLevel.Warn);
                    }

                }

                //If the shoe directory exists
                if (shoeDirectory.Exists)
                {
                    //Loop through each file and add the relevant texture to the Shoe list
                    foreach (FileInfo file in shoeDirectory.EnumerateFiles())
                    {
                        //Always going to find the female shoes first
                        if (file.Name.Contains("female_shoes"))
                            FemaleShoeTextureList.Add(contentPack.LoadAsset<Texture2D>(Path.Combine("Shoes", file.Name)));
                        else if (file.Name.Contains("male_shoes"))
                            MaleShoeTextureList.Add(contentPack.LoadAsset<Texture2D>(Path.Combine("Shoes", file.Name)));
                    }
                }

                //If the Face and Nose directory exists
                if (faceAndNoseDirectory.Exists)
                {
                    try
                    {
                        //Create a facenose model and read the json
                        FaceNoseModel model = contentPack.ReadJsonFile<FaceNoseModel>(Path.Combine("FaceAndNose", "count.json"));

                        //Add the count of faces to the dictionary for the base texture
                        MaleBaseFaceNoseCount.Add(MaleBaseTextureList[MaleBaseTextureList.Count - 1], new int[] { model.NumberOfMaleFaces, model.NumberOfMaleNoses });
                        FemaleBaseFaceNoseCount.Add(FemaleBaseTextureList[FemaleBaseTextureList.Count - 1], new int[] { model.NumberOfFemaleFaces, model.NumberOfFemaleNoses });

                        //New dictionaries to be added to the dictionary
                        Dictionary<string, Texture2D> currentPackMaleFaceNoseDict = new Dictionary<string, Texture2D>();
                        Dictionary<string, Texture2D> currentPackFemaleFaceNoseDict = new Dictionary<string, Texture2D>();

                        //Load the face and nose textures somewhere
                        foreach (FileInfo file in faceAndNoseDirectory.EnumerateFiles())
                        {
                            //It's always going to find the female faces first
                            if (file.Name.Contains("female_face"))
                                currentPackFemaleFaceNoseDict.Add(file.Name, contentPack.LoadAsset<Texture2D>(Path.Combine("FaceAndNose", file.Name)));
                            else if (file.Name.Contains("male_face"))
                                currentPackMaleFaceNoseDict.Add(file.Name, contentPack.LoadAsset<Texture2D>(Path.Combine("FaceAndNose", file.Name)));
                        }

                        //Add it to the Dictionary
                        MaleFaceAndNoseTextureDict.Add(MaleBaseTextureList[MaleBaseTextureList.Count - 1], currentPackMaleFaceNoseDict);
                        FemaleFaceAndNoseTextureDict.Add(FemaleBaseTextureList[FemaleBaseTextureList.Count - 1], currentPackFemaleFaceNoseDict);
                    }
                    catch
                    {
                        Entry.Monitor.Log($"{contentPack.Manifest.Name} faces and noses is emtpy. This pack was not added", LogLevel.Warn);
                    }
                }

                //If the skin color directory exists
                if (skinColorDirectory.Exists)
                {
                    try
                    {
                        //Create a new skin color model
                        SkinColorModel model = new SkinColorModel();

                        //Set the model info
                        model.Texture = contentPack.LoadAsset<Texture2D>(Path.Combine("SkinColor", "skinColors.png"));
                        model.TextureHeight = model.Texture.Height;
                        model.ModName = contentPack.Manifest.Name;

                        //Add the model to the list
                        SkinColorList.Add(model);
                    }
                    catch
                    {
                        Entry.Monitor.Log($"{contentPack.Manifest.Name} skin colors is emtpy. This pack was not added", LogLevel.Warn);
                    }

                }
            }

            //Add ImageInjector to the Asset Editor to start patching the images
            Entry.Helper.Content.AssetEditors.Add(new ImageInjector(Entry, this));
        }

        /// <summary>Gets the number of faces and noses for a base texture</summary>
        /// <param name="isMale">Whether the player is male</param>
        /// <param name="baseIndex">The base texture index</param>
        /// <param name="isFace">Wether it's looking for the face count</param>
        /// <returns>The count of faces or noses</returns>
        public int GetNumberOfFacesAndNoses(bool isMale, int baseIndex, bool isFace)
        {
            //This is static so we return the default values
            if (baseIndex.Equals(0) && isFace)
                return 1;
            else if (baseIndex.Equals(0) && !isFace)
                return 2;

            //Try...catch incase the Key is not in the dictionary
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
                //If the key is not in the dictionary then give the default values
                return 0;
            }

            //Return 0 if it doens't find anything
            return 0;
        }

        /// <summary>Changes the player base by setting the texture name</summary>
        /// <param name="isMale">Whether the player is male</param>
        /// <param name="baseIndex">The base index</param>
        /// <param name="faceIndex">The face index</param>
        /// <param name="noseIndex">The nose index</param>
        /// <param name="shoeIndex">The shoe index</param>
        /// <param name="isBald">Whether the player is bald</param>
        public void ChangePlayerBase(bool isMale, int baseIndex, int faceIndex, int noseIndex, int shoeIndex, bool isBald)
        {
            //Set the fields
            IsMale = isMale;
            BaseIndex = baseIndex;
            FaceIndex = faceIndex;
            NoseIndex = noseIndex;
            ShoeIndex = shoeIndex;
            IsBald = isBald;

            //Check if the base is 0, if it's not we want to load the base version
            if (baseIndex != 0)
                Game1.player.FarmerRenderer.textureName.Set($"GetGlam_{isMale}_{baseIndex}_{faceIndex}_{noseIndex}_{shoeIndex}_isBald{isBald}");
            else
                Game1.player.FarmerRenderer.textureName.Set($"GetGlam_{isMale}_{faceIndex}_{noseIndex}_{shoeIndex}_isBald{isBald}");
        }

        /// <summary>Loads the Player base texture called by <see cref="ContentLoader"/></summary>
        /// <returns>The new player base texture</returns>
        public Texture2D LoadPlayerBase()
        {
            //Create a new texture
            Texture2D playerBase;

            //Wrap in a try...catch in case they removed the base from the folder
            try
            {
                //Bald checking
                if (BaseIndex != 0 && !IsBald)
                    playerBase = IsMale ? MaleBaseTextureList[BaseIndex - 1] : FemaleBaseTextureList[BaseIndex - 1];
                else if (BaseIndex != 0 && IsBald)
                    playerBase = IsMale ? MaleBaseBaldTextureList[BaseIndex - 1] : FemaleBaseBaldTextureList[BaseIndex - 1];
                else if (IsBald)
                    playerBase = Entry.Helper.Content.Load<Texture2D>(IsMale ? "Characters\\Farmer\\farmer_base_bald" : "Characters\\Farmer\\farmer_girl_base_bald", ContentSource.GameContent);
                else
                    playerBase = Entry.Helper.Content.Load<Texture2D>(IsMale ? "Characters\\Farmer\\farmer_base" : "Characters\\Farmer\\farmer_girl_base", ContentSource.GameContent);
            }
            catch (Exception ex)
            {
                //Set to default base if the above loading fails
                Entry.Monitor.Log("There was a problem loading the base file, setting the base to default.", LogLevel.Warn);
                if (IsBald)
                    playerBase = Entry.Helper.Content.Load<Texture2D>(IsMale ? "Characters\\Farmer\\farmer_base_bald" : "Characters\\Farmer\\farmer_girl_base_bald", ContentSource.GameContent);
                else
                    playerBase = Entry.Helper.Content.Load<Texture2D>(IsMale ? "Characters\\Farmer\\farmer_base" : "Characters\\Farmer\\farmer_girl_base", ContentSource.GameContent);

                BaseIndex = 0;
            }

            //Try...catch for the face and nose if the key was not found in the dictionary
            try
            {
                //The base is 0 then load the default textures
                if (BaseIndex.Equals(0))
                {
                    Texture2D faceAndNoseTexture;
                    if (!IsBald)
                        faceAndNoseTexture = Entry.Helper.Content.Load<Texture2D>(IsMale ? $"assets/male_face{FaceIndex}_nose{NoseIndex}.png" : $"assets/female_face{FaceIndex}_nose{NoseIndex}.png");
                    else
                        faceAndNoseTexture = Entry.Helper.Content.Load<Texture2D>(IsMale ? $"assets/male_face{FaceIndex}_nose{NoseIndex}_bald.png" : $"assets/female_face{FaceIndex}_nose{NoseIndex}_bald.png");

                    PatchBaseTexture(playerBase, faceAndNoseTexture, 0, 0);
                }
                //If the player is male and the count is not 0
                else if (IsMale && MaleBaseFaceNoseCount[playerBase][0] > 0 && MaleBaseFaceNoseCount[playerBase][1] > 0)
                {
                    Texture2D faceAndNoseTexture;
                    faceAndNoseTexture = MaleFaceAndNoseTextureDict[playerBase][$"male_face{FaceIndex}_nose{NoseIndex}.png"];

                    PatchBaseTexture(playerBase, faceAndNoseTexture, 0, 0);
                }
                //If the player is female and the count is not 0
                else if (!IsMale && FemaleBaseFaceNoseCount[playerBase][0] > 0 && FemaleBaseFaceNoseCount[playerBase][1] > 0)
                {
                    Texture2D faceAndNoseTexture;
                    faceAndNoseTexture = FemaleFaceAndNoseTextureDict[playerBase][$"female_face{FaceIndex}_nose{NoseIndex}.png"];

                    PatchBaseTexture(playerBase, faceAndNoseTexture, 0, 0);
                }
            }
            catch (KeyNotFoundException ex)
            {
                //Set to deafault face and nose
                NoseIndex = 0;
                FaceIndex = 0;
            }

            Texture2D shoeTexture;
            //Create new shoeTexture and check the shoe index to load the right shoe
            try
            {
                if (ShoeIndex == 0)
                    shoeTexture = Entry.Helper.Content.Load<Texture2D>(IsMale ? "assets/farmer_base_shoes.png" : "assets/farmer_girl_base_shoes.png");
                else
                    shoeTexture = IsMale ? MaleShoeTextureList[ShoeIndex - 1] : FemaleShoeTextureList[ShoeIndex - 1];
            }
            catch
            {
                //This error pops up when the user saves at a shoe index then removes the content pack
                Entry.Monitor.Log("Could not find a shoe at the index. Did you remove a pack that added shoes? Setting to deafault.", LogLevel.Warn);
                ShoeIndex = 0;
                
                shoeTexture = Entry.Helper.Content.Load<Texture2D>(IsMale ? "assets/farmer_base_shoes.png" : "assets/farmer_girl_base_shoes.png");
            }

            //Patch the player base with the shoes
            int[] shoeHeights = IsMale ? MaleShoeSpriteHeights : FemaleShoeSpriteHeights;
            for (int i = 0; i < shoeHeights.Length; i++)
                PatchBaseTexture(playerBase, shoeTexture, 1 * i, (1 * i) * 3, 96, 32, shoeHeights[i]);

            //Return the new player base
            return playerBase;
        }

        /// <summary>Patches a texture using Texture2D.GetData and Texture2D.SetData</summary>
        /// <param name="targetTexture">The target texture</param>
        /// <param name="sourceTexture">The source texture</param>
        /// <param name="sourceID">The source ID</param>
        /// <param name="targetID">The target ID</param>
        /// <param name="gridWidth">The grids width</param>
        /// <param name="gridHeight">The grids height</param>
        /// <param name="adjustedHeight">The adjusted height</param>
        private void PatchBaseTexture(Texture2D targetTexture, Texture2D sourceTexture, int sourceID, int targetID, int gridWidth = 96, int gridHeight = 672, int adjustedHeight = 0)
        {
            Color[] colorData = new Color[gridWidth * (adjustedHeight == 0 ? gridHeight : adjustedHeight)];
            sourceTexture.GetData(0, GetSourceRect(sourceID, sourceTexture, gridWidth, gridHeight, adjustedHeight), colorData, 0, colorData.Length);
            targetTexture.SetData(0, GetSourceRect(targetID, targetTexture, gridWidth, gridHeight, adjustedHeight), colorData, 0, colorData.Length);
        }

        /// <summary>Get the source rect for an image</summary>
        /// <param name="index">The index</param>
        /// <param name="texture">The target texture to get the source rect</param>
        /// <param name="gridWidth">The grids width</param>
        /// <param name="gridHeight">The grids height</param>
        /// <param name="adjustedHeight">The adjusted height</param>
        /// <returns>The textures source rect</returns>
        private Rectangle GetSourceRect(int index, Texture2D texture, int gridWidth, int gridHeight, int adjustedHeight)
        {
            return new Rectangle(
                index % (texture.Width / gridWidth) * gridWidth,
                index / (texture.Width / gridWidth) * gridHeight + (adjustedHeight == 0 ? 0 : 32 - adjustedHeight),
                gridWidth,
                adjustedHeight == 0 ? gridHeight : adjustedHeight
            );
        }
    }
}
