/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul;

#region using directives

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The collection of configs for each module.</summary>
public sealed class ModConfig
{
    #region module flags

    /// <summary>Gets a value indicating whether the Professions module is enabled.</summary>
    [JsonProperty]
    public bool EnableProfessions { get; internal set; } = true;

#if DEBUG

    /// <summary>Gets a value indicating whether the Combat module is enabled.</summary>
    [JsonProperty]
    public bool EnableCombat { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Weapons module is enabled.</summary>
    [JsonProperty]
    public bool EnableWeapons { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Slingshots module is enabled.</summary>
    [JsonProperty]
    public bool EnableSlingshots { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Tools module is enabled.</summary>
    [JsonProperty]
    public bool EnableTools { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Enchantments module is enabled.</summary>
    [JsonProperty]
    public bool EnableEnchantments { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Rings module is enabled.</summary>
    [JsonProperty]
    public bool EnableRings { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Ponds module is enabled.</summary>
    [JsonProperty]
    public bool EnablePonds { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Taxes module is enabled.</summary>
    [JsonProperty]
    public bool EnableTaxes { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Tweex module is enabled.</summary>
    [JsonProperty]
    public bool EnableTweex { get; internal set; } = true;

#elif RELEASE

    /// <summary>Gets a value indicating whether the Combat module is enabled.</summary>
    [JsonProperty]
    public bool EnableCombat { get; internal set; } = false;

     /// <summary>Gets a value indicating whether the Weapons module is enabled.</summary>
    [JsonProperty]
    public bool EnableWeapons { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the Slingshots module is enabled.</summary>
    [JsonProperty]
    public bool EnableSlingshots { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the Tools module is enabled.</summary>
    [JsonProperty]
    public bool EnableTools { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the Enchantments module is enabled.</summary>
    [JsonProperty]
    public bool EnableEnchantments { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the Rings module is enabled.</summary>
    [JsonProperty]
    public bool EnableRings { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the Ponds module is enabled.</summary>
    [JsonProperty]
    public bool EnablePonds { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the Taxes module is enabled.</summary>
    [JsonProperty]
    public bool EnableTaxes { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the Tweex module is enabled.</summary>
    [JsonProperty]
    public bool EnableTweex { get; internal set; } = true;

#endif

    #endregion module flags

    #region config sub-modules

    /// <summary>Gets the Professions module config settings.</summary>
    [JsonProperty]
    public Modules.Professions.Config Professions { get; internal set; } = new();

    /// <summary>Gets the Professions module config settings.</summary>
    [JsonProperty]
    public Modules.Combat.Config Combat { get; internal set; } = new();

    /// <summary>Gets the Weapons module config settings.</summary>
    [JsonProperty]
    public Modules.Weapons.Config Weapons { get; internal set; } = new();

    /// <summary>Gets the Slingshots module config settings.</summary>
    [JsonProperty]
    public Modules.Slingshots.Config Slingshots { get; internal set; } = new();

    /// <summary>Gets the Tools module config settings.</summary>
    [JsonProperty]
    public Modules.Tools.Config Tools { get; internal set; } = new();

    /// <summary>Gets the Enchantments module config settings.</summary>
    [JsonProperty]
    public Modules.Enchantments.Config Enchantments { get; internal set; } = new();

    /// <summary>Gets the Rings module config settings.</summary>
    [JsonProperty]
    public Modules.Rings.Config Rings { get; internal set; } = new();

    /// <summary>Gets the Ponds module config settings.</summary>
    [JsonProperty]
    public Modules.Ponds.Config Ponds { get; internal set; } = new();

    /// <summary>Gets the Taxes module config settings.</summary>
    [JsonProperty]
    public Modules.Taxes.Config Taxes { get; internal set; } = new();

    /// <summary>Gets the Tweex module config settings.</summary>
    [JsonProperty]
    public Modules.Tweex.Config Tweex { get; internal set; } = new();

    #endregion config sub-modules

    /// <summary>Gets the key used to open the Generic Mod Config Menu directly at this mod.</summary>
    [JsonProperty]
    public KeybindList OpenMenuKey { get; internal set; } = KeybindList.Parse("LeftShift + F12");

    /// <summary>Gets the key used to engage Debug Mode.</summary>
    [JsonProperty]
    public KeybindList DebugKey { get; internal set; } = KeybindList.Parse("OemQuotes, OemTilde");

    /// <summary>Validates all internal configs and overwrites the user's config file if any invalid settings were found.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    internal void Validate(IModHelper helper)
    {
        if (!this.Enumerate().Aggregate(true, (flag, config) => flag | config.Validate()))
        {
            helper.WriteConfig(this);
        }
    }

    /// <summary>Enumerates all individual module <see cref="Shared.Configs.Config"/>s.</summary>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Shared.Configs.Config"/>s.</returns>
    internal IEnumerable<Shared.Configs.Config> Enumerate()
    {
        yield return this.Professions;
        yield return this.Combat;
        yield return this.Weapons;
        yield return this.Slingshots;
        yield return this.Tools;
        yield return this.Enchantments;
        yield return this.Rings;
        yield return this.Ponds;
        yield return this.Taxes;
        yield return this.Tweex;
    }

    /// <summary>Logs all sub-config properties to the SMAPI console.</summary>
    internal void Log()
    {
        Shared.Log.T($"[Config]: Current settings:\n{this}");
        var message = this
            .Enumerate()
            .Aggregate("[Config]: Current settings:", (current, next) => current + "\n" + next);
    }
}
