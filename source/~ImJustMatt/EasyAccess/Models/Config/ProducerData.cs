/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.EasyAccess.Models.Config;

using System.Collections.Generic;
using StardewMods.EasyAccess.Enums;
using StardewMods.EasyAccess.Interfaces.Config;

/// <inheritdoc />
internal class ProducerData : IProducerData
{
    /// <inheritdoc />
    public int CollectOutputDistance { get; set; } = 15;

    /// <inheritdoc />
    public HashSet<string> CollectOutputItems { get; set; } = new();

    /// <inheritdoc />
    public FeatureOptionRange CollectOutputs { get; set; } = FeatureOptionRange.Location;

    /// <inheritdoc />
    public int DispenseInputDistance { get; set; } = 15;

    /// <inheritdoc />
    public HashSet<string> DispenseInputItems { get; set; } = new();

    /// <inheritdoc />
    public int DispenseInputPriority { get; set; }

    /// <inheritdoc />
    public FeatureOptionRange DispenseInputs { get; set; } = FeatureOptionRange.Location;
}