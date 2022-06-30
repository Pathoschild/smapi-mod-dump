/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using StardewModdingAPI;


namespace TraktoriShared.Utils
{
	internal class GenericHelper
	{
		/// <summary>
		/// Read data from a JSON file in the mod's folder.
		/// </summary>
		/// <typeparam name="T">The model type. This should be a plain class that has public properties for the data you want. The properties can be complex types.</typeparam>
		/// <param name="assetPath">The file path relative to the mod folder.</param>
		/// <param name="dataHelper">The api for reading the mod data.</param>
		/// <param name="monitor">The monitor to log into.</param>
		/// <returns>Returns the deserialized model, or null if the file doesn't exist or is empty.</returns>
		internal static T? LoadAsset<T>(string assetPath, IDataHelper dataHelper, IMonitor monitor) where T : class, new()
		{
			T? asset = null;

			try
			{
				asset = dataHelper.ReadJsonFile<T>(assetPath);
			}
			catch (Exception ex)
			{
				monitor.Log(ex.ToString(), LogLevel.Error);
			}

			return asset;
		}


		/// <summary>
		/// Read data from a JSON file in the mod's folder.
		/// </summary>
		/// <typeparam name="T">The model type. This should be a plain class that has public properties for the data you want. The properties can be complex types.</typeparam>
		/// <param name="assetPath">The file path relative to the mod folder.</param>
		/// <param name="dataHelper">The api for reading the mod data.</param>
		/// <param name="monitor">The monitor to log into.</param>
		/// <returns>Returns the deserialized model, or the default instance of the object.</returns>
		internal static T LoadAssetOrDefault<T>(string assetPath, IDataHelper dataHelper, IMonitor monitor) where T : class, new()
		{
			T? asset =  LoadAsset<T>(assetPath, dataHelper, monitor);

			if (asset is null)
			{
				monitor.Log($"Loading {assetPath} failed. The mod may not work correctly.", LogLevel.Error);
				asset = new T();
			}

			return asset;
		}


		/// <summary>
		/// Read data from a JSON file that is in a list form in the mod's folder.
		/// </summary>
		/// <typeparam name="TData">The model type. This should be a plain class that has public properties for the data you want. The properties can be complex types.</typeparam>
		/// <param name="assetPath">The file path relative to the mod folder.</param>
		/// <param name="dataHelper">The api for reading the mod data.</param>
		/// <param name="monitor">The monitor to log into.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <returns>Returns a dictionary of deserialized models, or empty dictionary.</returns>
		internal static Dictionary<string, TData> ReadListAssetToDict<TData>(string assetPath, IDataHelper dataHelper, IMonitor monitor, Func<TData, string> keySelector) where TData : class
		{
			try
			{
				List<TData> data = LoadAssetOrDefault<List<TData>>(assetPath, dataHelper, monitor);

				return data.ToDictionary(keySelector);
			}
			catch (Exception ex)
			{
				monitor.Log(ex.ToString(), LogLevel.Error);
			}

			monitor.Log($"Returning an empty dictionary. The mod may not work correctly.", LogLevel.Error);
			return new Dictionary<string, TData>();
		}


		/// <summary>
		/// Tries to find the index key from the provided dictionary for the item data that matches the given name ignoring case.
		/// Works only for dictionaries where the value is data delimited by / and the name is the first entry.
		/// </summary>
		/// <param name="dictionary">The dictionary containing the item data</param>
		/// <param name="itemName">The name of the item to look for</param>
		/// <param name="index">The index key for the item's data, if it was found</param>
		/// <returns>If the mathing item data was found in the dictionary</returns>
		internal static bool TryGetIndexByName(IDictionary<int, string>? dictionary, string itemName, out int index)
		{
			index = 0;

			if (dictionary is null)
			{
				return false;
			}

			ReadOnlySpan<char> objectNameSpan = itemName.AsSpan();

			foreach (KeyValuePair<int, string> kvp in dictionary)
			{
				ReadOnlySpan<char> splitName = kvp.Value.AsSpan(0, kvp.Value.IndexOf('/'));

				if (objectNameSpan.Equals(splitName, StringComparison.OrdinalIgnoreCase))
				{
					index = kvp.Key;
					return true;
				}
			}

			return false;
		}
	}
}
