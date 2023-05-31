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
using Microsoft.Xna.Framework.Input;
using Omegasis.Revitalize.Framework.Constants.PathConstants;
using Omegasis.Revitalize.Framework.Content.JsonContent.Objects;
using Omegasis.Revitalize.Framework.ContentPacks;

namespace Omegasis.Revitalize.Framework.Utilities.JsonContentLoading
{
    /// <summary>
    /// Loads in json content from content packs.
    /// </summary>
    public static class JsonContentPackUtilities
    {
        /// <summary>
        /// Loads a description string for the item.
        /// </summary>
        /// <param name="Id">The id for </param>
        /// <returns></returns>
        public static string LoadItemDescription(string Id, bool ThrowExceptionIfNotFound = true)
        {
            /*
            if (!RevitalizeModCore.ModContentManager.objectManager.displayStrings.ContainsKey(Id) && ThrowExceptionIfNotFound)
            {
                throw new JsonContentLoadingException("The given item id {0} does not have a registered value for display strings! A file can be created under the ModAssets/Strings/Objects/DisplayStrings directory with the given info.");
            }
            return RevitalizeModCore.ModContentManager.objectManager.displayStrings[Id].description;
            */

            List<RevitalizeContentPack> contentPacks = RevitalizeModCore.ModContentManager.revitalizeContentPackManager.getContentPacksForCurrentLanguageCode();

            foreach(RevitalizeContentPack contentPack in contentPacks)
            {
                if (contentPack.objectDisplayStrings.ContainsKey(Id) && !string.IsNullOrEmpty(contentPack.objectDisplayStrings[Id].description))
                {
                    return contentPack.objectDisplayStrings[Id].description;
                }
            }
            if (ThrowExceptionIfNotFound)
            {
                throw new JsonContentLoadingException("The given item id {0} does not have a registered value for display strings in any Revitalize content pack! A file can be created under the ModAssets/Strings/Objects/DisplayStrings directory with the given info.");
            }
            return null;

        }

        /// <summary>
        /// Loads a display name string for the item.
        /// </summary>
        /// <param name="Id">The id for </param>
        /// <returns></returns>
        public static string LoadItemDisplayName(string Id, bool ThrowExceptionIfNotFound = true)
        {
            /*
            if (!RevitalizeModCore.ModContentManager.objectManager.displayStrings.ContainsKey(Id) && ThrowExceptionIfNotFound)
            {
                throw new JsonContentLoadingException("The given item id {0} does not have a registered value for display strings! A file can be created under the ModAssets/Strings/Objects/DisplayStrings directory with the given info.");
            }
            return RevitalizeModCore.ModContentManager.objectManager.displayStrings[Id].displayName;
            */

            List<RevitalizeContentPack> contentPacks = RevitalizeModCore.ModContentManager.revitalizeContentPackManager.getContentPacksForCurrentLanguageCode();

            foreach (RevitalizeContentPack contentPack in contentPacks)
            {
                if (contentPack.objectDisplayStrings.ContainsKey(Id) && !string.IsNullOrEmpty(contentPack.objectDisplayStrings[Id].displayName))
                {
                    return contentPack.objectDisplayStrings[Id].displayName;
                }
            }
            if (ThrowExceptionIfNotFound)
            {
                throw new JsonContentLoadingException(string.Format("The given item id {0} does not have a registered value for display strings in any Revitalize content pack! A file can be created under the ModAssets/Strings/Objects/DisplayStrings directory with the given info.", Id));
            }
            return null;
        }

        /// <summary>
        /// Loads a category string for the item.
        /// </summary>
        /// <param name="Id">The id for </param>
        /// <returns></returns>
        public static string LoadItemCategory (string Id, bool ThrowExceptionIfNotFound = true)
        {
            /*
            if (!RevitalizeModCore.ModContentManager.objectManager.displayStrings.ContainsKey(Id) && ThrowExceptionIfNotFound)
            {
                throw new JsonContentLoadingException("The given item id {0} does not have a registered value for display strings! A file can be created under the ModAssets/Strings/Objects/DisplayStrings directory with the given info.");
            }
            return RevitalizeModCore.ModContentManager.objectManager.displayStrings[Id].displayName;
            */

            List<RevitalizeContentPack> contentPacks = RevitalizeModCore.ModContentManager.revitalizeContentPackManager.getContentPacksForCurrentLanguageCode();

            foreach (RevitalizeContentPack contentPack in contentPacks)
            {
                if (contentPack.objectDisplayStrings.ContainsKey(Id) && !string.IsNullOrEmpty(contentPack.objectDisplayStrings[Id].displayName))
                {
                    return contentPack.objectDisplayStrings[Id].category;
                }
            }
            if (ThrowExceptionIfNotFound)
            {
                throw new JsonContentLoadingException(string.Format("The given item id {0} does not have a registered value for display strings in any Revitalize content pack! A file can be created under the ModAssets/Strings/Objects/DisplayStrings directory with the given info."));
            }
            return null;
        }

        public static string LoadCategory(string CategoryID)
        {
          Dictionary<string,string> dict=  LoadStringDictionaryFile(Path.Combine(StringsPaths.Objects, "CategoryNames.json"));
            if (dict.ContainsKey(CategoryID))
            {
                return dict[CategoryID];
            }
            return null;
        }

        /// <summary>
        /// Loads in a specified error string from a given file.
        /// </summary>
        /// <param name="RelativePathToFile"></param>
        /// <param name="Key"></param>
        /// <param name="StringParamaters"></param>
        /// <returns></returns>
        public static string LoadErrorString(string RelativePathToFile, string Key, params object[] StringParamaters)
        {
            List<RevitalizeContentPack> contentPacks = RevitalizeModCore.ModContentManager.revitalizeContentPackManager.getContentPacksForCurrentLanguageCode();

            foreach (RevitalizeContentPack contentPack in contentPacks)
            {
                string baseString = JsonUtilities.LoadStringFromDictionaryFile(contentPack,Key, Path.Combine(Constants.PathConstants.StringsPaths.ErrorStrings, RelativePathToFile));
                if (!string.IsNullOrEmpty(baseString))
                {
                    return string.Format(baseString, StringParamaters);
                }
            }
            string fileToLoadFrom = Path.Combine(Constants.PathConstants.StringsPaths.ErrorStrings, RelativePathToFile);

            throw new JsonContentLoadingException(string.Format("The given file path {0} or key {1} do not exist for any Revitalize content pack! A file can be created under the ModAssets/Strings/ErrorStrings/Objects directory with the given info.",fileToLoadFrom,Key));
        }

        /// <summary>
        /// Loads in a specified error string from a given file.
        /// </summary>
        /// <param name="RelativePathToFile"></param>
        /// <param name="Key"></param>
        /// <param name="StringParamaters"></param>
        /// <returns></returns>
        public static string LoadShopDialogue(string RelativePathToFile, string Key, params object[] StringParamaters)
        {
            List<RevitalizeContentPack> contentPacks = RevitalizeModCore.ModContentManager.revitalizeContentPackManager.getContentPacksForCurrentLanguageCode();

            foreach (RevitalizeContentPack contentPack in contentPacks)
            {
                string baseString = JsonUtilities.LoadStringFromDictionaryFile(contentPack, Key, Path.Combine(Constants.PathConstants.StringsPaths.ShopDialogue, RelativePathToFile));
                if (!string.IsNullOrEmpty(baseString))
                {
                    return string.Format(baseString, StringParamaters);
                }
            }
            throw new JsonContentLoadingException("The given file path {0} or key {1} do not exist for any Revitalize content pack! A file can be created under the ModAssets/Strings/ShopDialogue directory with the given info.");
        }

        public static string LoadBuildingDisplayName(string Id, bool ThrowExceptionIfNotFound = true)
        {
            /*
            if (!RevitalizeModCore.ModContentManager.objectManager.displayStrings.ContainsKey(Id) && ThrowExceptionIfNotFound)
            {
                throw new JsonContentLoadingException("The given item id {0} does not have a registered value for display strings! A file can be created under the ModAssets/Strings/Objects/DisplayStrings directory with the given info.");
            }
            return RevitalizeModCore.ModContentManager.objectManager.displayStrings[Id].displayName;
            */

            List<RevitalizeContentPack> contentPacks = RevitalizeModCore.ModContentManager.revitalizeContentPackManager.getContentPacksForCurrentLanguageCode();

            foreach (RevitalizeContentPack contentPack in contentPacks)
            {
                if (contentPack.buildingDisplayStrings.ContainsKey(Id) && !string.IsNullOrEmpty(contentPack.buildingDisplayStrings[Id].displayName))
                {
                    return contentPack.buildingDisplayStrings[Id].displayName;
                }
            }
            if (ThrowExceptionIfNotFound)
            {
                throw new JsonContentLoadingException(string.Format("The given Building id {0} does not have a registered value for display strings in any Revitalize content pack! A file can be created under the ModAssets/Strings/Buildings/DisplayStrings directory with the given info.", Id));
            }
            return null;
        }

        public static string LoadBuildingDescription(string Id, bool ThrowExceptionIfNotFound = true)
        {
            /*
            if (!RevitalizeModCore.ModContentManager.objectManager.displayStrings.ContainsKey(Id) && ThrowExceptionIfNotFound)
            {
                throw new JsonContentLoadingException("The given item id {0} does not have a registered value for display strings! A file can be created under the ModAssets/Strings/Objects/DisplayStrings directory with the given info.");
            }
            return RevitalizeModCore.ModContentManager.objectManager.displayStrings[Id].displayName;
            */

            List<RevitalizeContentPack> contentPacks = RevitalizeModCore.ModContentManager.revitalizeContentPackManager.getContentPacksForCurrentLanguageCode();

            foreach (RevitalizeContentPack contentPack in contentPacks)
            {
                if (contentPack.buildingDisplayStrings.ContainsKey(Id) && !string.IsNullOrEmpty(contentPack.buildingDisplayStrings[Id].displayName))
                {
                    return contentPack.buildingDisplayStrings[Id].description;
                }
            }
            if (ThrowExceptionIfNotFound)
            {
                throw new JsonContentLoadingException(string.Format("The given Building id {0} does not have a registered value for display strings in any Revitalize content pack! A file can be created under the ModAssets/Strings/Buildings/DisplayStrings directory with the given info.", Id));
            }
            return null;
        }

        /// <summary>
        /// Loads in a string dictionary file from a given content pack.
        /// </summary>
        /// <param name="RelativePathToFile"></param>
        /// <returns></returns>
        public static Dictionary<string, string> LoadStringDictionaryFile(string RelativePathToFile)
        {
            List<RevitalizeContentPack> contentPacks = RevitalizeModCore.ModContentManager.revitalizeContentPackManager.getContentPacksForCurrentLanguageCode();

            foreach (RevitalizeContentPack contentPack in contentPacks)
            {
                Dictionary<string,string> dict= JsonUtilities.LoadStringDictionaryFile(contentPack,RelativePathToFile);
                if (dict!=null)
                {
                    return dict;
                }
            }
            throw new JsonContentLoadingException(string.Format("The given file at the path {0} does not exist for any Revitalize content pack!",RelativePathToFile));
        }

        /// <summary>
        /// Loads in a <typeparamref name="T"/> object dictionary file from a given content pack.
        /// </summary>
        /// <param name="RelativePathToFile"></param>
        /// <returns></returns>
        public static Dictionary<string, T> LoadDictionaryFile<T>(string RelativePathToFile)
        {
            List<RevitalizeContentPack> contentPacks = RevitalizeModCore.ModContentManager.revitalizeContentPackManager.getContentPacksForCurrentLanguageCode();

            foreach (RevitalizeContentPack contentPack in contentPacks)
            {
                Dictionary<string, T> dict = JsonUtilities.LoadDictionaryFile<T>(contentPack, RelativePathToFile);
                if (dict != null)
                {
                    return dict;
                }
            }
            throw new JsonContentLoadingException(string.Format("The given file at the path {0} does not exist for any Revitalize content pack!",RelativePathToFile));
        }

        /// <summary>
        /// Loads an object from a dictionary file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="RelativePathToFile"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static T LoadFromDictionaryFile<T>(string RelativePathToFile, string Key)
        {
            Dictionary<string, T> dict = LoadDictionaryFile<T>(RelativePathToFile);
            if (dict != null)
            {
                if (dict.ContainsKey(Key))
                {
                    return (T)dict[Key];
                }
            }
            return default(T);
        }

        /// <summary>
        /// Loads a string from a content pack.
        /// </summary>
        /// <param name="RelativePathToFile"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string LoadStringFromDictionaryFile(string RelativePathToFile, string Key)
        {
            return LoadFromDictionaryFile<string>(RelativePathToFile, Key);
        }
    }
}
