/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

#if DEBUG
using System.Diagnostics;
#endif

using CommunityToolkit.Diagnostics;

namespace AtraShared.Integrations;

/// <summary>
/// Base class for integration management.
/// </summary>
public class IntegrationHelper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationHelper"/> class.
    /// </summary>
    /// <param name="monitor">Logger instance.</param>
    /// <param name="translation">Translation helper.</param>
    /// <param name="modRegistry">Mod registry.</param>
    /// <param name="loglevel">Level to log issues to.</param>
    public IntegrationHelper(IMonitor monitor, ITranslationHelper translation, IModRegistry modRegistry, LogLevel loglevel = LogLevel.Info)
    {
        Guard.IsNotNull(monitor);
        Guard.IsNotNull(translation);
        Guard.IsNotNull(modRegistry);

        this.Monitor = monitor;
        this.Translation = translation;
        this.ModRegistry = modRegistry;
        this.LogLevel = loglevel;
    }

    /// <summary>
    /// Gets the logger instance.
    /// </summary>
    protected IMonitor Monitor { get; init; }

    /// <summary>
    /// Gets the translation helper instance.
    /// </summary>
    protected ITranslationHelper Translation { get; init; }

    /// <summary>
    /// Gets the mod registry instance.
    /// </summary>
    protected IModRegistry ModRegistry { get; init; }

    /// <summary>
    /// Gets the level to log at.
    /// </summary>
    protected LogLevel LogLevel { get; init; }

    /// <summary>
    /// Attempts to get the API from a different mod.
    /// </summary>
    /// <typeparam name="T">Interface to map to.</typeparam>
    /// <param name="apiid">UniqueID of the other mod.</param>
    /// <param name="minversion">Minimum semantic version, or null for no minimum.</param>
    /// <param name="api">An instance of the api.</param>
    /// <returns>True if successful, false otherwise.</returns>
    public bool TryGetAPI<T>(
        string apiid,
        string? minversion,
        [NotNullWhen(returnValue: true)] out T? api)
        where T : class
    {
        Guard.IsNotNull(apiid);

        if (this.ModRegistry.Get(apiid) is not IModInfo modInfo)
        {
            this.Monitor.Log(
                this.Translation.Get("api-not-found")
                .Default("{{APIID}} not found, integration disabled.")
                .Tokens(new { apiid }), this.LogLevel);
            api = default;
            return false;
        }
        if (minversion is not null && modInfo.Manifest.Version.IsOlderThan(minversion))
        {
            this.Monitor.Log(
                this.Translation.Get("api-too-old")
                .Default("Please update {{apiName}}({{APIID}}) to at least version {{minversion}}. Current version {{currentversion}}. Integration disabled.")
                .Tokens(new { apiName = modInfo.Manifest.Name, APIID = apiid, minversion, currentversion = modInfo.Manifest.Version }), this.LogLevel);
            api = default;
            return false;
        }
#if DEBUG
        Stopwatch sw = Stopwatch.StartNew();
#endif
        try
        {
            api = this.ModRegistry.GetApi<T>(apiid);
        }
        catch (Exception ex)
        {
            this.Monitor.Log($"Failed while attempting to map {apiid}\n\n{ex}", LogLevel.Error);
            api = null;
        }
#if DEBUG
        sw.Stop();
        this.Monitor.Log($"Mapping {apiid} took {sw.ElapsedMilliseconds} milliseconds", LogLevel.Info);
#endif
        return api is not null;
    }
}
