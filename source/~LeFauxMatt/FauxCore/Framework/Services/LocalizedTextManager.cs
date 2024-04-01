/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.FauxCore.Framework.Services;

using StardewMods.Common.Enums;

/// <summary>Formats the given <see cref="LogLevel" /> into a string.</summary>
internal static class LocalizedTextManager
{
    /// <summary>Formats the given <paramref name="value" /> into a string.</summary>
    /// <param name="value">The level of logging.</param>
    /// <returns>Returns the formatted log level.</returns>
    public static string TryFormat(string value)
    {
        if (!SimpleLogLevelExtensions.TryParse(value, out var logLevel))
        {
            logLevel = SimpleLogLevel.Less;
        }

        return logLevel switch
        {
            SimpleLogLevel.None => I18n.Config_LogLevel_Options_None(),
            SimpleLogLevel.Less => I18n.Config_LogLevel_Options_Less(),
            SimpleLogLevel.More => I18n.Config_LogLevel_Options_More(),
            _ => I18n.Config_LogLevel_Options_Less(),
        };
    }
}