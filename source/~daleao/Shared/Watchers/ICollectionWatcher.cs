/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Watchers;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>A watcher which tracks changes to a collection.</summary>
/// <typeparam name="TValue">The collection value type.</typeparam>
/// <remarks>Pulled from <see href="https://github.com/Pathoschild/SMAPI/tree/develop/src/SMAPI/Modules/StateTracking">SMAPI</see>.</remarks>
internal interface ICollectionWatcher<out TValue> : IWatcher
{
    /// <summary>Gets the values added since the last reset.</summary>
    IEnumerable<TValue> Added { get; }

    /// <summary>Gets the values removed since the last reset.</summary>
    IEnumerable<TValue> Removed { get; }
}
