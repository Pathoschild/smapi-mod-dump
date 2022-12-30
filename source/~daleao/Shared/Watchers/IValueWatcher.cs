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

/// <summary>A watcher which tracks changes to a value.</summary>
/// <typeparam name="TValue">The watched value type.</typeparam>
/// <remarks>Pulled from <see href="https://github.com/Pathoschild/SMAPI/tree/develop/src/SMAPI/Modules/StateTracking">SMAPI</see>.</remarks>
internal interface IValueWatcher<out TValue> : IWatcher
{
    /// <summary>Gets the field value at the last reset.</summary>
    TValue PreviousValue { get; }

    /// <summary>Gets the latest value.</summary>
    TValue CurrentValue { get; }
}
