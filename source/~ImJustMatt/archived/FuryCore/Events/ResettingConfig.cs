/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

#nullable disable

namespace StardewMods.FuryCore.Events;

using System;
using StardewMods.FuryCore.Interfaces;
using StardewMods.FuryCore.Interfaces.CustomEvents;
using StardewMods.FuryCore.Models.CustomEvents;
using StardewMods.FuryCore.Services;

/// <inheritdoc />
internal class ResettingConfig : SortedEventHandler<IResettingConfigEventArgs>
{
    private readonly Lazy<ConfigureGameObject> _configureGameObject;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ResettingConfig" /> class.
    /// </summary>
    /// <param name="services">Provides access to internal and external services.</param>
    public ResettingConfig(IModServices services)
    {
        this._configureGameObject = services.Lazy<ConfigureGameObject>();
    }

    private ConfigureGameObject ConfigureGameObject
    {
        get => this._configureGameObject.Value;
    }

    /// <summary>
    ///     Resets the current configuration back to default values.
    /// </summary>
    public void Reset()
    {
        this.InvokeAll(new ResettingConfigEventArgs(this.ConfigureGameObject.CurrentObject));
    }
}