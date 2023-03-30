/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat;

#region using directives

using Newtonsoft.Json;

#endregion using directives

/// <summary>Configs related to general combat.</summary>
public sealed class Config : Shared.Configs.Config
{
    /// <summary>Gets a value indicating whether to overhaul the knockback stat adding collision damage.</summary>
    [JsonProperty]
    public bool KnockbackDamage { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to overhaul the defense stat with better scaling and other features.</summary>
    [JsonProperty]
    public bool OverhauledDefense { get; internal set; } = true;

    /// <summary>Gets a multiplier which allows scaling the health of all monsters.</summary>
    [JsonProperty]
    public float MonsterHealthMultiplier { get; internal set; } = 1f;

    /// <summary>Gets a multiplier which allows scaling the damage dealt by all monsters.</summary>
    [JsonProperty]
    public float MonsterDamageMultiplier { get; internal set; } = 1f;

    /// <summary>Gets a multiplier which allows scaling the resistance of all monsters.</summary>
    [JsonProperty]
    public float MonsterDefenseMultiplier { get; internal set; } = 1f;

    /// <summary>Gets a value indicating whether randomizes monster stats to add variability to monster encounters.</summary>
    [JsonProperty]
    public bool VariedEncounters { get; internal set; } = true;
}
