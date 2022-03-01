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

using StardewMods.EasyAccess.Enums;
using StardewMods.EasyAccess.Interfaces.Config;

/// <inheritdoc />
internal class ConfigData : IConfigData
{
    /// <inheritdoc />
    public bool Configurator { get; set; } = true;

    /// <inheritdoc />
    public ControlScheme ControlScheme { get; set; } = new();

    /// <inheritdoc />
    public ProducerData DefaultProducer { get; set; } = new()
    {
        CollectOutputs = FeatureOptionRange.Location,
        CollectOutputDistance = 15,
        DispenseInputs = FeatureOptionRange.Location,
        DispenseInputDistance = 15,
    };
}