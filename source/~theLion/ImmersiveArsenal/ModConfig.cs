/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal;

/// <summary>The mod user-defined settings.</summary>
public class ModConfig
{
    /// <summary>Make weapons more unique and useful.</summary>
    public bool RebalanceWeapons { get; set; } = true;

    /// <summary>Make footwear more unique and useful.</summary>
    public bool RebalanceFootwear { get; set; } = true;

    /// <summary>Improves certain underwhelming enchantments.</summary>
    public bool RebalanceEnchants { get; set; } = true;

    /// <summary>Weapons should cost energy to use.</summary>
    public bool WeaponsCostStamina { get; set; } = true;

    /// <summary>Projectiles should not be useless for the first 100ms.</summary>
    public bool RemoveSlingshotGracePeriod { get; set; } = true;

    /// <summary>Damage mitigation should not be soft-capped at 50%.</summary>
    public bool RemoveDefenseSoftCap { get; set; } = true;

    /// <summary>The Galaxy Sword should not be so easy to get.</summary>
    public bool ImmersiveGalaxyWeaponConditions { get; set; } = true;
}