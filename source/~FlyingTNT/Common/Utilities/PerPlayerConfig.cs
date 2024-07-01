/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using Newtonsoft.Json;
using StardewValley;

namespace Common.Utilities;

public static class PerPlayerConfig
{

    /// <summary>
    /// Stores a value in a given player's mod data.
    /// </summary>
    /// <typeparam name="T">The type of the value to be stored. Must be serializable by JsonConvert.</typeparam>
    /// <param name="farmer">The Farmer to store the value in.</param>
    /// <param name="key">The key to store the value under. Should be prefixed by the mod's unique id.</param>
    /// <param name="value">The value to store.</param>
    public static void SaveConfigOption<T>(Farmer farmer, string key, T value)
    {
        // This is in case this is run before a save is loaded. In the future, maybe give Common access to the mod's IMonitor so we can print an error message.
        if(farmer is null)
        {
            return;
        }

        // Serialize the data and store it in the mod data.
        string data = JsonConvert.SerializeObject(value);

        farmer.modData[key] = data;
    }

    /// <summary>
    /// Tries to load the value with the given key from the given Farmer's mod data.
    /// </summary>
    /// <typeparam name="T">The type of the value to be loaded.</typeparam>
    /// <param name="farmer">The farmer to load the value from.</param>
    /// <param name="key">The key the value is stored under. Should be prefixed by the mod's unique id.</param>
    /// <param name="value">The stored value, or T's default value if the given key is not in the farner's mod data.</param>
    /// <returns>True if the given key is in the farmer's mod data, and false otherwise.</returns>
    public static bool TryLoadConfigOption<T>(Farmer farmer, string key, out T value)
    {
        if(farmer is not null && farmer.modData.TryGetValue(key, out string data))
        {
            // If the mod data contains the given key, deserialize the data and return true.

            value = JsonConvert.DeserializeObject<T>(data);

            return true;
        }

        // If farmer is null or its mod data does not contain the key, use T's default value and return false.

        value = default(T);
        return false;
    }

    /// <summary>
    /// Loads the value with the given key from the given Farmer's mod data.
    /// </summary>
    /// <typeparam name="T">The type of the value to be loaded.</typeparam>
    /// <param name="farmer">The farmer to load the value from.</param>
    /// <param name="key">The key the value is stored under. Should be prefixed by the mod's unique id.</param>
    /// <param name="defaultValue">The value to use if there is no entry with the given key in the farmer's mod data.</param>
    /// <returns>The stored value, if the given key is in the farmer's mod data, and the given default value otherwise.</returns>
    public static T LoadConfigOption<T>(Farmer farmer, string key, T defaultValue)
    {
        // If the value can be loaded, use the stored value; otherwise, use the default.
        return TryLoadConfigOption<T>(farmer, key, out T value) ? value : defaultValue;
    }
}