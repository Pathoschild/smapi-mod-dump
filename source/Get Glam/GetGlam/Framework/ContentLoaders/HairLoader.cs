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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.IO;

namespace GetGlam.Framework.ContentLoaders
{
    public class HairLoader
    {
        // Instance of ModEntry
        private ModEntry Entry;

        // Directory where the hair files are stored
        private DirectoryInfo HairDirectory;

        // The model of the hair
        private HairModel Hair;

        // Current content pack being looked at
        private IContentPack CurrentContentPack;

        // Instance of ContentPackHelper
        private ContentPackHelper PackHelper;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="modEntry">Instance of ModEntry</param>
        /// <param name="contentPack">Current content pack</param>
        public HairLoader(ModEntry modEntry, IContentPack contentPack, ContentPackHelper packHelper)
        {
            if (contentPack != null)
                HairDirectory = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Hairstyles"));
            Entry = modEntry;
            CurrentContentPack = contentPack;
            PackHelper = packHelper;
        }
        
        /// <summary>
        /// Loads Hair from a Content Pack.
        /// </summary>
        public void LoadHair()
        {
            if (DoesHairDirectoryExists())
            {
                try
                {
                    CreateNewHairModel();
                    SetHairModelVariables();

                    PackHelper.HairStyleSearch.Add(CurrentContentPack.Manifest.Name, PackHelper.NumberOfHairstlyesAdded);

                    AddNumberOfHairstyles();
                    AddHairToHairList();
                }
                catch
                {
                    Entry.Monitor.Log($"{CurrentContentPack.Manifest.Name} hairstyles is empty. This pack was not added.", LogLevel.Warn);
                }
            }
        }

        /// <summary>
        /// Creates a model for hairstyles2.png.
        /// </summary>
        public void LoadHairstyleTwo()
        {
            HairModel hairstyleTwo = new HairModel();
            hairstyleTwo.Texture = Entry.Helper.Content.Load<Texture2D>("Characters\\Farmer\\hairstyles2", ContentSource.GameContent);
            hairstyleTwo.ModName = "Hairstyles 2";
            hairstyleTwo.TextureHeight = hairstyleTwo.Texture.Height;
            hairstyleTwo.NumberOfHairstyles = 23;

            PackHelper.HairStyleSearch.Add("Default 1.5 Hair", PackHelper.NumberOfHairstlyesAdded);
            PackHelper.NumberOfHairstlyesAdded += hairstyleTwo.NumberOfHairstyles;
            PackHelper.HairList.Add(hairstyleTwo);
        }

        /// <summary>
        /// Checks if the hair directory exists.
        /// </summary>
        /// <returns>If the hair directory exists</returns>
        private bool DoesHairDirectoryExists()
        {
            return HairDirectory.Exists;
        }

        /// <summary>
        /// Creates a new Hair Model.
        /// </summary>
        private void CreateNewHairModel()
        {
            Hair = CurrentContentPack.ReadJsonFile<HairModel>("Hairstyles/hairstyles.json");
            Hair.Texture = CurrentContentPack.LoadAsset<Texture2D>("Hairstyles/hairstyles.png");
        }

        /// <summary>
        /// Sets texture and mod name in the model.
        /// </summary>
        private void SetHairModelVariables()
        {
            Hair.TextureHeight = Hair.Texture.Height;
            Hair.ModName = CurrentContentPack.Manifest.Name;
        }

        /// <summary>
        /// Adds number of hairstyles from the Content Pack.
        /// </summary>
        private void AddNumberOfHairstyles()
        {
            PackHelper.NumberOfHairstlyesAdded += Hair.NumberOfHairstyles;
        }

        /// <summary>
        /// Adds hair model to the hair list.
        /// </summary>
        private void AddHairToHairList() 
        {
            PackHelper.HairList.Add(Hair);
        }
    }
}
