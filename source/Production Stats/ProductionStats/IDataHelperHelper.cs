/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlameHorizon/ProductionStats
**
*************************************************/

using StardewModdingAPI;

namespace ProductionStats;

public static class IDataHelperHelper
{
    /// <summary>
    /// Checks if specified resource key exists in save data.
    /// </summary>
    /// <param name="dataHelper">Provides API for reading save data.</param>
    /// <param name="key">Resource key to look up.</param>
    /// <returns>True, if resource exists, otherwise false.</returns>
    public static bool SaveDataKeyExists(this IDataHelper dataHelper, string key)
    {
        var data = dataHelper.ReadSaveData<object>(key);
        return data != null;
    }

}
