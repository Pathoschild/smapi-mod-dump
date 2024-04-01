/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.FauxCore.Framework.Models;

using StardewMods.Common.Enums;
using StardewMods.FauxCore.Framework.Interfaces;

/// <summary>Mod config data for FauxCore.</summary>
internal sealed class DefaultConfig : IModConfig
{
    /// <inheritdoc />
    public SimpleLogLevel LogLevel { get; set; } = SimpleLogLevel.Less;
}