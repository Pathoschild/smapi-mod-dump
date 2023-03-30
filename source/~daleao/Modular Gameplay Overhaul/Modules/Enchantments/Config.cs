/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Enchantments;

#region using directives

using Newtonsoft.Json;
using StardewValley.Tools;

#endregion using directives

/// <summary>The user-configurable settings for Enchantments.</summary>
public sealed class Config : Shared.Configs.Config
{
    /// <summary>Gets a value indicating whether to use the overhauled enchantments for <see cref="MeleeWeapon"/>s.</summary>
    [JsonProperty]
    public bool MeleeEnchantments { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to use the overhauled enchantments for <see cref="Slingshot"/>s.</summary>
    [JsonProperty]
    public bool RangedEnchantments { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to improve certain underwhelming gemstone enchantments.</summary>
    [JsonProperty]
    public bool RebalancedForges { get; internal set; } = true;
}
