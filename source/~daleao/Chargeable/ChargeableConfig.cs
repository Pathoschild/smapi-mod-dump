/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Chargeable;

#region using directives

using System.Linq;
using DaLion.Chargeable.Framework.Configs;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Config schema for the Chargeable mod.</summary>
public sealed class ChargeableConfig
{
    /// <inheritdoc cref="AxeConfig"/>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Tools/Axe", "tols.axe", true)]
    public AxeConfig Axe { get; internal set; } = new();

    /// <inheritdoc cref="PickaxeConfig"/>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Tools/Pick", "tols.pick", true)]
    public PickaxeConfig Pick { get; internal set; } = new();

    /// <summary>Gets a value indicating whether determines whether charging requires a mod key to activate.</summary>
    [JsonProperty]
    [GMCMPriority(0)]
    public bool RequireModKey { get; internal set; } = true;

    /// <summary>Gets the chosen mod key(s).</summary>
    [JsonProperty]
    [GMCMPriority(1)]
    public KeybindList ModKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets the number of update ticks between each peak of the shockwave. Affects the shockwave travel speed. Lower is faster. Set to 0 for instant.</summary>
    [JsonProperty]
    [GMCMPriority(2)]
    [GMCMRange(0, 10)]
    public uint TicksBetweenWaves { get; internal set; } = 4;

    /// <summary>Validate the config settings, replacing invalid values if necessary.</summary>
    internal void Validate(IModHelper helper)
    {
        var isValid = true;

        Log.T("[Tools]: Verifying tool configs...");

        if (this.Axe.RadiusAtEachPowerLevel.Length < 5)
        {
            Log.W("Missing values in Axe.RadiusAtEachPowerLevel. The default values will be restored.");
            this.Axe.RadiusAtEachPowerLevel = [1, 2, 3, 4, 5];
            isValid = false;
        }

        if (this.Pick.RadiusAtEachPowerLevel.Length < 5)
        {
            Log.W("Missing values Pickaxe.RadiusAtEachPowerLevel. The default values will be restored.");
            this.Pick.RadiusAtEachPowerLevel = [1, 2, 3, 4, 5];
            isValid = false;
        }

        if (this.RequireModKey && !this.ModKey.IsBound)
        {
            Log.W(
                "'RequireModkey' setting is set to true, but no Modkey is bound. Default keybind will be restored. To disable the Modkey, set this value to false.");
            this.ModKey = KeybindList.ForSingle(SButton.LeftShift);
            isValid = false;
        }

        if (this.Axe.StaminaCostMultiplier < 0)
        {
            Log.W("Axe 'StaminaCostMultiplier' is set to an illegal negative value. The value will default to 0");
            this.Axe.StaminaCostMultiplier = 0;
            isValid = false;
        }

        if (this.Pick.StaminaCostMultiplier < 0)
        {
            Log.W("Pick 'StaminaCostMultiplier' is set to an illegal negative value. The value will default to 0");
            this.Pick.StaminaCostMultiplier = 0;
            isValid = false;
        }

        if (this.TicksBetweenWaves > 100)
        {
            Log.W(
                "The value of 'TicksBetweenWaves' is excessively large. This is probably a mistake. The default value will be restored.");
            this.TicksBetweenWaves = 4;
            isValid = false;
        }

        if (this.Axe.RadiusAtEachPowerLevel.Length > 5)
        {
            Log.W("Too many values in Axe.RadiusAtEachPowerLevel. Additional values will be removed.");
            this.Axe.RadiusAtEachPowerLevel = this.Axe.RadiusAtEachPowerLevel.Take(5).ToArray();
            isValid = false;
        }

        if (this.Pick.RadiusAtEachPowerLevel.Length > 5)
        {
            Log.W("Too many values in Pickaxe.RadiusAtEachPowerLevel. Additional values will be removed.");
            this.Pick.RadiusAtEachPowerLevel =
                this.Pick.RadiusAtEachPowerLevel.Take(5).ToArray();
            isValid = false;
        }

        if (!isValid)
        {
            helper.WriteConfig(this);
        }
    }
}
