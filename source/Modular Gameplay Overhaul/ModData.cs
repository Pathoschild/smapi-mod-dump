/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented

namespace DaLion.Overhaul;

#region using directives

using Newtonsoft.Json;

#endregion using directives

/// <summary>Holds global data, independent from save files.</summary>
public sealed class ModData
{
    [JsonProperty]
    public bool InitialSetupComplete { get; internal set; }
}

#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
