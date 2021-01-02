/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;

namespace NpcAdventure.Loader
{
    public interface IContentLoader
    {
        /// <summary>
        /// Loads mod content data.
        /// </summary>
        /// <typeparam name="TKey">Type of asset data keys</typeparam>
        /// <typeparam name="TValue">Type of asset data values</typeparam>
        /// <param name="path">Name of asset, like `Strings/Strings` or `Dialogue/Abigail` and etc</param>
        /// <returns></returns>
        Dictionary<TKey, TValue> LoadData<TKey, TValue>(string path, bool isRequired = true);

        Dictionary<TKey, TValue> LoadMergedData<TKey, TValue>(params string[] paths);

        /// <summary>
        /// Loads mod content data as string map
        /// </summary>
        /// <param name="stringsAssetName"></param>
        /// <returns></returns>
        Dictionary<string, string> LoadStrings(string stringsAssetName);

        /// <summary>
        /// Load one string from strings dictionary content data asset
        /// </summary>
        /// <param name="path">Path to string in asset with whole asset name (like `Strings/Strings:companionRecruited.yes`</param>
        /// <returns>A loaded string from asset dictionary</returns>
        string LoadString(string path);

        /// <summary>
        /// Load one string from strings dictionary asset with substituions.
        /// Placeholders `{%number%}` in string wil be replaced with substitution.
        /// </summary>
        /// <param name="path">Path to string in asset with whole asset name (like `Strings/Strings:companionRecruited.yes`)</param>
        /// <param name="substitutions">A substitution for replace placeholder in string</param>
        /// <returns>A loaded string from asset dictionary</returns>
        string LoadString(string path, params object[] substitutions);

        /// <summary>
        /// Invalidate mod's cached assets
        /// </summary>
        void InvalidateCache();

        /// <summary>
        /// Get direct asset key
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        string GetAssetKey(string assetName);
    }
}
