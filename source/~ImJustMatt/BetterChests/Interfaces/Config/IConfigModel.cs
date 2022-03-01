/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Interfaces.Config;

/// <inheritdoc />
internal interface IConfigModel : IConfigData
{
    /// <summary>
    ///     Resets all config options back to their default value.
    /// </summary>
    public void Reset();

    /// <summary>
    ///     Saves current config options to the config.json file.
    /// </summary>
    public void Save();
}