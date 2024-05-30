/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Interfaces.Data;
#else
namespace StardewMods.Common.Interfaces.Data;
#endif

/// <summary>Represents modeled data stored as a dictionary of string key value pairs.</summary>
internal interface IDictionaryModel
{
    /// <summary>Checks if the dictionary contains the specified key.</summary>
    /// <param name="key">The key to check for existence in the dictionary.</param>
    /// <returns>true if the dictionary contains the specified key; otherwise, false.</returns>
    bool ContainsKey(string key);

    /// <summary>Sets the value for a given key in the derived class.</summary>
    /// <param name="key">The key associated with the value.</param>
    /// <param name="value">The value to be set.</param>
    void SetValue(string key, string value);

    /// <summary>Tries to get the data associated with the specified key.</summary>
    /// <param name="key">The key to search for.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key; otherwise, null.</param>
    /// <returns><c>true</c> if the key was found; otherwise, <c>false</c>.</returns>
    bool TryGetValue(string key, [NotNullWhen(true)] out string? value);
}