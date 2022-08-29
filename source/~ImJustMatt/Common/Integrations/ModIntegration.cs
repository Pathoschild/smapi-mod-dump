/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations;

using System;

/// <summary>Provides an integration point for using external mods' APIs.</summary>
/// <typeparam name="T">Interface for the external mod's API.</typeparam>
internal abstract class ModIntegration<T>
    where T : class
{
    private readonly Lazy<T?> _modAPI;

    /// <summary>Initializes a new instance of the <see cref="ModIntegration{T}" /> class.</summary>
    /// <param name="modRegistry">SMAPI's mod registry.</param>
    /// <param name="modUniqueId">The unique id of the external mod.</param>
    /// <param name="modVersion">The minimum supported version.</param>
    internal ModIntegration(IModRegistry modRegistry, string modUniqueId, string modVersion = "")
    {
        this.ModRegistry = modRegistry;
        this.UniqueId = modUniqueId;
        this.Version = string.IsNullOrWhiteSpace(modVersion) ? null : modVersion;
        this._modAPI = new(() => this.ModRegistry.GetApi<T>(this.UniqueId));
    }

    /// <summary>Gets the Mod's API through SMAPI's standard interface.</summary>
    protected internal T? API => this.IsLoaded ? this._modAPI.Value : default;

    /// <summary>Gets a value indicating whether the mod is loaded.</summary>
    [MemberNotNullWhen(true, nameof(ModIntegration<T>.API))]
    protected internal bool IsLoaded => this.ModRegistry.IsLoaded(this.UniqueId)
                                     && (this.Version is null
                                      || this.ModRegistry.Get(this.UniqueId)?.Manifest.Version.IsOlderThan(this.Version)
                                      == true);

    /// <summary>
    ///     Gets the Unique Id for this mod.
    /// </summary>
    protected internal string UniqueId { get; }

    /// <summary>
    ///     Gets the minimum supported version for this mod.
    /// </summary>
    protected internal string? Version { get; }

    private IModRegistry ModRegistry { get; }
}