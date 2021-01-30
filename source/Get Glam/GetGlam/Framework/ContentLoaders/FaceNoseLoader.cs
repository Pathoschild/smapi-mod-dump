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
using System.Collections.Generic;
using System.IO;

namespace GetGlam.Framework.ContentLoaders
{
    public class FaceNoseLoader
    {
        // Instance of ModEntry
        private ModEntry Entry;

        // Directory where the face and nose files are stored
        private DirectoryInfo FaceNoseDirectory;

        // The model of the face and nose
        private FaceNoseModel FaceNose;

        // Current content pack being looked at
        private IContentPack CurrentContentPack;

        // Instance of ContentPackHelper
        private ContentPackHelper PackHelper;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entry">Instance of ModEntry</param>
        /// <param name="contentPack">Current Content Pack</param>
        /// <param name="packHelper">Instance of ContentPackHelper</param>
        public FaceNoseLoader(ModEntry entry, IContentPack contentPack, ContentPackHelper packHelper)
        {
            FaceNoseDirectory = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "FaceAndNose"));
            Entry = entry;
            CurrentContentPack = contentPack;
            PackHelper = packHelper;
        }

        /// <summary>
        /// Loads face and nose from a Content Pack.
        /// </summary>
        public void LoadFaceAndNose()
        {
            if (DoesFaceNoseDirectoryExists()) 
            {
                try
                {
                    CreateNewFaceNoseModel();
                    AddFaceNoseCountToBaseList();
                    CreateNewDictionaries();
                }
                catch 
                {
                    Entry.Monitor.Log($"{CurrentContentPack.Manifest.Name} faces and noses is empty. This pack was not added.", LogLevel.Warn);
                }
            }
        }

        /// <summary>
        /// Whether the face and nose directory exists.
        /// </summary>
        /// <returns></returns>
        private bool DoesFaceNoseDirectoryExists()
        {
            return FaceNoseDirectory.Exists;
        }

        /// <summary>
        /// Creates a new face and nose model.
        /// </summary>
        private void CreateNewFaceNoseModel()
        {
            FaceNose = CurrentContentPack.ReadJsonFile<FaceNoseModel>(Path.Combine("FaceAndNose", "count.json"));
        }

        /// <summary>
        /// Adds face and nose count for each base.
        /// </summary>
        private void AddFaceNoseCountToBaseList()
        {
            PackHelper.MaleBaseFaceNoseCount.Add(PackHelper.MaleBaseTextureList[PackHelper.MaleBaseTextureList.Count - 1],
                new int[] 
                {
                    FaceNose.NumberOfMaleFaces,
                    FaceNose.NumberOfMaleNoses 
                });
            PackHelper.FemaleBaseFaceNoseCount.Add(PackHelper.FemaleBaseTextureList[PackHelper.FemaleBaseTextureList.Count - 1], 
                new int[] {
                    FaceNose.NumberOfFemaleFaces, 
                    FaceNose.NumberOfFemaleNoses
                });
        }

        /// <summary>
        /// Creates new dictionaries and finds files in the facenose directory.
        /// </summary>
        private void CreateNewDictionaries()
        {
            Dictionary<string, Texture2D> currentPackMaleFaceNoseDict = new Dictionary<string, Texture2D>();
            Dictionary<string, Texture2D> currentPackFemaleFaceNoseDict = new Dictionary<string, Texture2D>();

            foreach (FileInfo file in FaceNoseDirectory.EnumerateFiles())
            {
                FindFemaleFaceNose(file, currentPackFemaleFaceNoseDict);
                FindMaleFaceNose(file, currentPackMaleFaceNoseDict);
            }

            AddToFaceNoseDictionary(currentPackFemaleFaceNoseDict, currentPackMaleFaceNoseDict);
        }

        /// <summary>
        /// Finds female faces and nose and adds them to the dictionary.
        /// </summary>
        /// <param name="file">The current file</param>
        /// <param name="currentPackFemaleFaceNoseDict">Dictionary for current pack</param>
        private void FindFemaleFaceNose(FileInfo file, Dictionary<string, Texture2D> currentPackFemaleFaceNoseDict)
        {
            if (file.Name.Contains("female_face"))
                currentPackFemaleFaceNoseDict.Add(file.Name, CurrentContentPack.LoadAsset<Texture2D>(Path.Combine("FaceAndNose", file.Name)));

        }

        /// <summary>
        /// Finds male faces and noses and adds them to the dictionary.
        /// </summary>
        /// <param name="file">The current pack</param>
        /// <param name="currentPackMaleFaceNoseDict">Dictionary for current pack</param>
        private void FindMaleFaceNose(FileInfo file, Dictionary<string, Texture2D> currentPackMaleFaceNoseDict)
        {
            if (file.Name.Contains("male_face"))
                currentPackMaleFaceNoseDict.Add(file.Name, CurrentContentPack.LoadAsset<Texture2D>(Path.Combine("FaceAndNose", file.Name)));
        }

        /// <summary>
        /// Adds faces and noses to the texture dictionary.
        /// </summary>
        /// <param name="currentPackFemaleFaceNoseDict">Dictionary of added female faces and nose of the current pack</param>
        /// <param name="currentPackMaleFaceNoseDict">Dictionary of added male faces and nose of the current pack</param>
        private void AddToFaceNoseDictionary(Dictionary<string, Texture2D> currentPackFemaleFaceNoseDict, Dictionary<string, Texture2D> currentPackMaleFaceNoseDict)
        {
            PackHelper.MaleFaceAndNoseTextureDict.Add(
                PackHelper.MaleBaseTextureList[PackHelper.MaleBaseTextureList.Count - 1],
                currentPackMaleFaceNoseDict);
            PackHelper.FemaleFaceAndNoseTextureDict.Add(
                PackHelper.FemaleBaseTextureList[PackHelper.FemaleBaseTextureList.Count - 1],
                currentPackFemaleFaceNoseDict);
        }
    }
}
