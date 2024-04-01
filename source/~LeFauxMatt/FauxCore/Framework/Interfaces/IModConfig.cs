/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.FauxCore.Framework.Interfaces;

using StardewMods.Common.Enums;

/// <summary>Mod config data with log level.</summary>
public interface IModConfig
{
    /// <summary>Gets the amount of debugging information that will be logged to the console.</summary>
    public SimpleLogLevel LogLevel { get; }
}