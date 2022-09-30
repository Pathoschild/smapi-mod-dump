/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.Revitalize.Framework.Crafting.JsonContent;

namespace Omegasis.Revitalize.Framework.Utilities
{
    /// <summary>
    /// Useful utilities for handling json files.
    /// </summary>
    public class JsonUtilities
    {
        /// <summary>
        /// Writes json files to disk.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="RelativePathToSaveTo"></param>
        public static void WriteJsonFile(object obj, params string[] RelativePathToSaveTo)
        {
            RevitalizeModCore.ModHelper.Data.WriteJsonFile(Path.Combine(RelativePathToSaveTo), obj);
        }

        /// <summary>
        /// Reads json files from disk.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="RelativePathToReadFrom"></param>
        public static T ReadJsonFile<T>(params string[] RelativePathToReadFrom) where T : class
        {
            return RevitalizeModCore.ModHelper.Data.ReadJsonFile<T>(Path.Combine(RelativePathToReadFrom));
        }

        private static T ReadJsonFilePathCombined<T>(string RelativePathToReadFrom) where T : class
        {
            return ReadJsonFile<T>(RelativePathToReadFrom);
        }

        /// <summary>
        /// Recursively searches all directories and sub directories under the relative path passed in, and performs the action when found.
        /// </summary>
        /// <param name="onFileFound"></param>
        /// <param name="relativePath"></param>
        public static void RecursiveSearchDirectoriesForJsonFiles(Action<string> onFileFound, params string[] relativePath)
        {
            string absPath = Path.Combine(RevitalizeModCore.ModHelper.DirectoryPath, Path.Combine(relativePath));
            foreach (string folder in Directory.GetDirectories(absPath))
            {
                RecursiveSearchDirectoriesForJsonFiles(onFileFound, Path.Combine(absPath, Path.GetFileName(folder)));
            }
            foreach (string file in Directory.GetFiles(absPath, "*.json"))
            {
                string fileRelativePath = Path.Combine(absPath, Path.GetFileName(file));
                onFileFound.Invoke(fileRelativePath);
            }
        }

        /// <summary>
        /// Recursively searches all directories and sub directories under the relative path passed in, and loads them from disk.
        /// </summary>
        /// <param name="onFileFound">The action that occurs when the json file is loaded. Note that this is not <see cref="ReadJsonFile{T}(string[])"/> incase there is post processing that should be done first.</param>
        /// <param name="relativePath"></param>
        /// <returns>A list of all json files that were loaded.</returns>
        public static List<T> LoadJsonFilesFromDirectories<T>(Func<string, T> onFileFound, params string[] relativePath) where T : class
        {
            string relativePathString = Path.Combine(relativePath);
            string absPath = Path.Combine(RevitalizeModCore.ModHelper.DirectoryPath, relativePathString);
            List<T> returnedJsonFiles = new List<T>();
            foreach (string folder in Directory.GetDirectories(absPath))
            {
                string folderRelativePath = Path.Combine(relativePathString, Path.GetFileName(folder));
                returnedJsonFiles.AddRange(LoadJsonFilesFromDirectories(onFileFound, folderRelativePath));
            }
            foreach (string file in Directory.GetFiles(absPath, "*.json"))
            {
                string fileRelativePath = Path.Combine(relativePathString, Path.GetFileName(file));
                returnedJsonFiles.Add(onFileFound.Invoke(fileRelativePath));
            }
            return returnedJsonFiles;
        }

        /// <summary>
        /// Recursively searches all directories and sub directories under the relative path passed in, and loads them from disk. <see cref="LoadJsonFilesFromDirectories{T}(Func{string, T}, string[])"/> if postprocessing is desired.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns>A list of all json files that were loaded.</returns>
        public static List<T> LoadJsonFilesFromDirectories<T>(params string[] relativePath) where T : class
        {
            return LoadJsonFilesFromDirectories<T>(new Func<string, T>(ReadJsonFilePathCombined<T>), relativePath);
        }


        /// <summary>
        /// Loads a string dictionary from a path relative to the Mod folder's location.
        /// </summary>
        /// <param name="RelativePathToFile"></param>
        /// <returns></returns>
        public static Dictionary<string, string> LoadStringDictionaryFile(string RelativePathToFile)
        {
            return ReadJsonFilePathCombined<Dictionary<string, string>>(RelativePathToFile);
        }

        /// <summary>
        /// Loads a string from a string dictionary.
        /// </summary>
        /// <param name="Key">The key in the json file to load from.</param>
        /// <param name="RelativePathToFile">The relative path to the dictionary file from the mods' content folder.</param>
        /// <returns></returns>
        public static string LoadStringFromDictionaryFile(string Key, string RelativePathToFile)
        {
            Dictionary<string, string> dictFile = LoadStringDictionaryFile(RelativePathToFile);
            if (dictFile.ContainsKey(Key))
            {
                return dictFile[Key];
            }
            return null;
        }

    }
}
