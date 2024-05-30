/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Ponds;

#region using directives

using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;

#endregion using directives

/// <summary>Config schema for the Ponds mod.</summary>
public sealed class PondsConfig
{
    private uint _daysUntilAlgaeSpawn = 3;
    private float _roeProductionChanceMultiplier = 1f;

    /// <summary>Gets the number of days until an empty pond will begin spawning algae.</summary>
    [JsonProperty]
    [GMCMPriority(0)]
    [GMCMRange(1, 14)]
    public uint DaysUntilAlgaeSpawn
    {
        get => this._daysUntilAlgaeSpawn;
        internal set => this._daysUntilAlgaeSpawn = Math.Max(value, 1);
    }

    /// <summary>Gets the multiplier to a fish's base chance to produce roe each day.</summary>
    [JsonProperty]
    [GMCMPriority(1)]
    [GMCMRange(0.1f, 10f)]
    public float RoeProductionChanceMultiplier
    {
        get => this._roeProductionChanceMultiplier;
        internal set => this._roeProductionChanceMultiplier = Math.Max(value, 0.1f);
    }
}
