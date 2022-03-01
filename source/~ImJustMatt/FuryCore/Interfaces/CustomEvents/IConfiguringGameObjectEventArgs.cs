/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.FuryCore.Interfaces.CustomEvents;

using System;
using StardewModdingAPI;
using StardewMods.FuryCore.Events;
using StardewMods.FuryCore.Interfaces.GameObjects;

/// <summary>
///     <see cref="EventArgs" /> for the <see cref="ConfiguringGameObject" /> event.
/// </summary>
public interface IConfiguringGameObjectEventArgs
{
    /// <summary>
    ///     Gets the GameObject being configured.
    /// </summary>
    public IGameObject GameObject { get; }

    /// <summary>
    ///     Gets the mod manifest to add config options to.
    /// </summary>
    public IManifest ModManifest { get; }
}