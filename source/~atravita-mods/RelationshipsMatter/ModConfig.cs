/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Integrations.GMCMAttributes;

namespace RelationshipsMatter;

/// <summary>
/// The config class for this mod.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Fields kept near accessors.")]
internal sealed class ModConfig
{
    private float friendshipGainFactor = 0.5f;

    /// <summary>
    /// Gets or sets a value indicating how much friendship gain will be affected.
    /// </summary>
    [GMCMRange(0.01, 20)]
    public float FriendshipGainFactor
    {
        get => this.friendshipGainFactor;
        set => this.friendshipGainFactor = Math.Clamp(value, 0.01f, 20f);
    }

    private float friendshipLossFactor = 1.0f;

    [GMCMRange(0.01, 20)]
    public float FriendshipLossFactor
    {
        get => this.friendshipLossFactor;
        set => this.friendshipLossFactor = Math.Clamp(value, 0.01f, 20f);
    }

    private int minRelativeHeartLevel = 5;

    [GMCMRange(0, 8)]
    public int MinRelativeHeartLevel
    {
        get => this.minRelativeHeartLevel;
        set => this.minRelativeHeartLevel = Math.Clamp(value, 0, 8);
    }
}
