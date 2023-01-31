/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Configs;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The user-configurable settings for Arsenal.</summary>
public sealed class Config : Shared.Configs.Config
{
    /// <inheritdoc cref="SlingshotConfig"/>
    [JsonProperty]
    public SlingshotConfig Slingshots { get; internal set; } = new();

    /// <inheritdoc cref="WeaponConfig"/>
    [JsonProperty]
    public WeaponConfig Weapons { get; internal set; } = new();

    /// <summary>Gets the chosen mod key(s).</summary>
    [JsonProperty]
    public KeybindList ModKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets a value indicating whether to allow auto-selecting a weapon or slingshot.</summary>
    [JsonProperty]
    public bool EnableAutoSelection { get; internal set; } = true;

    /// <summary>Gets the <see cref="Color"/> used to indicate tools enabled or auto-selection.</summary>
    [JsonProperty]
    public Color SelectionBorderColor { get; internal set; } = Color.Magenta;

    /// <summary>Gets a value indicating whether face the current cursor position before swinging your arsenal.</summary>
    [JsonProperty]
    public bool FaceMouseCursor { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow drifting in the movement direction when swinging weapons.</summary>
    [JsonProperty]
    public bool SlickMoves { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to color-code weapon and slingshot names, <see href="https://tvtropes.org/pmwiki/pmwiki.php/Main/ColourCodedForYourConvenience">for your convenience</see>.</summary>
    [JsonProperty]
    public bool ColorCodedForYourConvenience { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to improve certain underwhelming gemstone enchantments.</summary>
    [JsonProperty]
    public bool RebalancedForges { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to overhaul the knockback stat adding collision damage.</summary>
    [JsonProperty]
    public bool KnockbackDamage { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to overhaul the defense stat with better scaling and other features.</summary>
    [JsonProperty]
    public bool OverhauledDefense { get; internal set; } = true;

    /// <summary>Gets increases the health of all monsters.</summary>
    [JsonProperty]
    public float MonsterHealthMultiplier { get; internal set; } = 1f;

    /// <summary>Gets increases the damage dealt by all monsters.</summary>
    [JsonProperty]
    public float MonsterDamageMultiplier { get; internal set; } = 1f;

    /// <summary>Gets increases the resistance of all monsters.</summary>
    [JsonProperty]
    public float MonsterDefenseMultiplier { get; internal set; } = 1f;

    /// <summary>Gets a value indicating whether randomizes monster stats to add variability to monster encounters.</summary>
    [JsonProperty]
    public bool VariedEncounters { get; internal set; } = true;

    /// <summary>Gets a value indicating whether replace the starting Rusty Sword with a Wooden Blade.</summary>
    [JsonProperty]
    public bool WoodyReplacesRusty { get; internal set; } = true;

    /// <summary>Gets a value indicating whether replace the starting Rusty Sword with a Wooden Blade.</summary>
    [JsonProperty]
    public bool DwarvishCrafting { get; internal set; } = true;

    /// <summary>Gets a value indicating whether replace lame Galaxy and Infinity weapons with something truly legendary.</summary>
    [JsonProperty]
    public bool InfinityPlusOne { get; internal set; } = true;

    /// <summary>Gets a value indicating the number of Iridium Bars required to obtain a Galaxy weapon.</summary>
    [JsonProperty]
    public int IridiumBarsRequiredForGalaxyArsenal { get; internal set; } = 10;

    /// <inheritdoc />
    internal override bool Validate()
    {
        var isValid = true;

        if (this.Weapons.GalaxySwordType == WeaponType.StabbingSword)
        {
            Collections.StabbingSwords.Add(Constants.GalaxySwordIndex);
        }
        else if (this.Weapons.GalaxySwordType != WeaponType.DefenseSword)
        {
            Log.W(
                $"Invalid type {this.Weapons.GalaxySwordType} for Galaxy Sword. Should be either 'StabbingSword' or 'DefenseSword'. The value will default to 'DefenseSword'.");
            this.Weapons.GalaxySwordType = WeaponType.DefenseSword;
        }

        if (this.Weapons.InfinityBladeType == WeaponType.StabbingSword)
        {
            Collections.StabbingSwords.Add(Constants.InfinityBladeIndex);
        }
        else if (this.Weapons.InfinityBladeType != WeaponType.DefenseSword)
        {
            Log.W(
                $"Invalid type {this.Weapons.InfinityBladeType} for Infinity Blade. Should be either 'StabbingSword' or 'DefenseSword'. The value will default to 'DefenseSword'.");
            this.Weapons.GalaxySwordType = WeaponType.DefenseSword;
        }

        return isValid;
    }
}
