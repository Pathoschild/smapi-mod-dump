/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/TrashDoesNotConsumeBait
**
*************************************************/

namespace TrashDoesNotConsumeBait;

/// <summary>
/// The configuration class for this mod.
/// </summary>
public class ModConfig
{
    private float consumeChanceNormal = 1f;
    private float consumeChancePreserving = 0.5f;

    /// <summary>
    /// Gets or sets a value indicating whether or not tackles/bait should be automatically refilled.
    /// </summary>
    public bool AutomaticRefill { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether tackles should be only replaced with the same type of tackle.
    /// </summary>
    public bool SameTackleOnly { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether bait should only be replaced with the same type of bait.
    /// </summary>
    public bool SameBaitOnly { get; set; } = true;

    /// <summary>
    /// Gets or sets chance of consuming bait/tackle normally.
    /// </summary>
    public float ConsumeChanceNormal
    {
        get => this.consumeChanceNormal;
        set => this.consumeChanceNormal = Math.Clamp(value, 0f, 1f);
    }

    /// <summary>
    /// Gets or sets chance of consuming bait/tackle with the Preserving enchantment.
    /// </summary>
    public float ConsumeChancePreserving
    {
        get => this.consumeChancePreserving;
        set => this.consumeChancePreserving = Math.Clamp(value, 0f, 1f);
    }
}