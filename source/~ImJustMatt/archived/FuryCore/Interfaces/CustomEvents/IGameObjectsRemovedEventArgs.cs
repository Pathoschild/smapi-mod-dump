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

namespace StardewMods.FuryCore.Interfaces.CustomEvents;

using System;
using System.Collections.Generic;
using StardewMods.FuryCore.Events;
using StardewMods.FuryCore.Interfaces.GameObjects;

/// <summary>
///     <see cref="EventArgs" /> for the <see cref="GameObjectsRemoved" /> event.
/// </summary>
public interface IGameObjectsRemovedEventArgs
{
    /// <summary>
    ///     Gets <see cref="IGameObject" /> removed.
    /// </summary>
    public IEnumerable<IGameObject> Removed { get; }
}