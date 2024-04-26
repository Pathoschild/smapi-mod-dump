/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.CustomBush.Framework.Models;

using StardewMods.Common.Services.Integrations.CustomBush;
using StardewValley.GameData;

/// <inheritdoc cref="StardewMods.Common.Services.Integrations.CustomBush.ICustomBushDrop" />
public sealed class CustomBushDrop : GenericSpawnItemDataWithCondition, ICustomBushDrop
{
    /// <inheritdoc />
    public Season? Season { get; set; }

    /// <inheritdoc />
    public float Chance { get; set; } = 1f;
}