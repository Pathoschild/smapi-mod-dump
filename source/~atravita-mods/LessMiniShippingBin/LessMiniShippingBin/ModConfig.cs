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

namespace LessMiniShippingBin;

/// <summary>
/// Configuration class for this mod.
/// </summary>
internal sealed class ModConfig
{
    private int minishippingcapacity = 36;
    private int juminocapcity = 9;

    /// <summary>
    /// Gets or sets capacity of the mini shipping bin.
    /// </summary>
    [GMCMInterval(9)]
    [GMCMRange(9, 48)]
    public int MiniShippingCapacity
    {
        get => this.minishippingcapacity;
        set => this.minishippingcapacity = Math.Clamp(value, 9, 48);
    }

    /// <summary>
    /// Gets or sets the capacity of the jumino chest.
    /// </summary>
    [GMCMInterval(9)]
    [GMCMRange(9, 48)]
    public int JuminoCapacity
    {
        get => this.juminocapcity;
        set => this.juminocapcity = Math.Clamp(value, 9, 48);
    }
}