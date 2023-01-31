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

namespace SingleParenthood;

/// <summary>
/// The config class for this mod.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Fields kept near accessors.")]
internal sealed class ModConfig
{
    private int gestation = 14;

    /// <summary>
    /// Gets or sets how long an adoption/birth takes.
    /// </summary>
    [GMCMRange(1, 28)]
    public int Gestation
    {
        get => this.gestation;
        set => this.gestation = Math.Clamp(value, 1, 28);
    }

    private int maxKids = 2;

    /// <summary>
    /// Gets or sets a value indicating the maximum number of kids to allow.
    /// </summary>
    [GMCMRange(0, 10)]
    public int MaxKids
    {
        get => this.maxKids;
        set => this.maxKids = Math.Clamp(value, 0, 10);
    }
}
