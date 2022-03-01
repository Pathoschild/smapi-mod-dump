/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.TooManyAnimals.Models;

using StardewMods.TooManyAnimals.Interfaces;

/// <inheritdoc />
internal class ConfigData : IConfigData
{
    /// <inheritdoc />
    public int AnimalShopLimit { get; set; } = 30;

    /// <inheritdoc />
    public ControlScheme ControlScheme { get; set; } = new();
}