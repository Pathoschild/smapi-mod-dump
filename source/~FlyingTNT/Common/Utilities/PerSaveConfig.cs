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
using StardewModdingAPI;
using StardewValley;

namespace Common.Utilities;

public static class PerSaveConfig
{

    /// <summary>
    /// Stores a model in a save file.
    /// 
    /// Note: Can only be used by local players, and only once a save has been loaded.
    /// </summary>
    /// <typeparam name="TModel">The model to store the data as.</typeparam>
    /// <param name="helper">The IModHelper for the mod saving the data.</param>
    /// <param name="key">The key to store the value under. Does not need to be prefixed by the mod's unique id.</param>
    /// <param name="value">The value to store.</param>
    public static void SaveConfigOption<TModel>(IModHelper helper, string key, TModel value) where TModel : class
    {
        if(!Game1.IsMasterGame)
        {
            return;
        }

        // I'm using this to check if the save is loaded. Smapi uses Context.LoadStage to do this, but that variable is internal and I don't feel like doing reflection to get it.
        if(!Context.IsWorldReady)
        {
            return;
        }

        helper.Data.WriteSaveData(key, value);
    }

    /// <summary>
    /// Tries to load the value with the given key from a save file.
    /// 
    /// Note: Can only be used by local players, and only once a save has been loaded.
    /// </summary>
    /// <typeparam name="TModel">The model to store the data in.</typeparam>
    /// <param name="helper">The IModHelper for the mod saving the data.</param>
    /// <param name="key">The key the value is stored under. Does not need to be prefixed by the mod's unique id.</param>
    /// <param name="value">The stored value, or null if the given key is not in the save file, or the player is not local/there is no save loaded.</param>
    /// <returns>True if the given key is in the farmer's mod data, and false otherwise.</returns>
    public static bool TryLoadConfigOption<TModel>(IModHelper helper, string key, out TModel value) where TModel : class
    {
        if (!(Game1.IsMasterGame && Context.IsWorldReady))
        {
            value = null;
            return false;
        }

        value = helper.Data.ReadSaveData<TModel>(key);

        return value is not null;
    }

    /// <summary>
    /// Tries to load the value with the given key from a save file.
    /// 
    /// Note: Can only be used by local players, and only once a save has been loaded.
    /// </summary>
    /// <typeparam name="TModel">The model to store the data in.</typeparam>
    /// <param name="helper">The IModHelper for the mod saving the data.</param>
    /// <param name="key">The key the value is stored under. Does not need to be prefixed by the mod's unique id.</param>
    /// <param name="defaultValue">The value to use if there is no entry with the given key in the save.</param>
    /// <returns>The stored value, if the given key is in the save's mod data, and the given default value otherwise.</returns>
    public static TModel LoadConfigOption<TModel>(IModHelper helper, string key, TModel defaultValue) where TModel : class
    {
        // If the value can be loaded, use the stored value; otherwise, use the default.
        return TryLoadConfigOption(helper, key, out TModel value) ? value : defaultValue;
    }
}