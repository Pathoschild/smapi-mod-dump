/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Harmony;

#region using directives

using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;
using StardewModdingAPI;

#endregion using directives

/// <summary>Instantiates and applies <see cref="IHarmonyPatcher"/> classes in the assembly.</summary>
internal sealed class Harmonizer
{
    /// <inheritdoc cref="IModRegistry"/>
    private readonly IModRegistry _modRegistry;

    /// <inheritdoc cref="Stopwatch"/>
    private readonly Stopwatch _sw = new();

    /// <summary>Initializes a new instance of the <see cref="Harmonizer"/> class.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="harmonyId">The unique ID of the declaring mod.</param>
    internal Harmonizer(IModRegistry modRegistry, string harmonyId)
    {
        this._modRegistry = modRegistry;
        this.HarmonyId = harmonyId;
        this.Harmony = new Harmony(harmonyId);
    }

    /// <inheritdoc cref="HarmonyLib.Harmony"/>
    internal Harmony Harmony { get; }

    /// <summary>Gets the unique ID of the <see cref="HarmonyLib.Harmony"/> instance.</summary>
    internal string HarmonyId { get; }

    /// <summary>Implicitly applies<see cref="IHarmonyPatcher"/> types in the assembly using reflection.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="harmonyId">The unique ID of the declaring mod.</param>
    /// <returns>The <see cref="Harmonizer"/> instance.</returns>
    internal static Harmonizer ApplyAll(IModRegistry modRegistry, string harmonyId)
    {
        Log.D("[Harmonizer]: Gathering all patches...");
        return new Harmonizer(modRegistry, harmonyId).ApplyImplicitly();
    }

    /// <summary>Implicitly applies <see cref="IHarmonyPatcher"/> types in the specified namespace.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="namespace">The desired namespace.</param>
    /// <param name="harmonyId">The unique ID of the declaring mod. Defaults to <paramref name="namespace"/> if null.</param>
    /// <returns>The <see cref="Harmonizer"/> instance.</returns>
    internal static Harmonizer ApplyFromNamespace(IModRegistry modRegistry, string @namespace, string? harmonyId = null)
    {
        Log.D($"[Harmonizer]: Gathering patches in {@namespace}...");
        return new Harmonizer(modRegistry, harmonyId ?? @namespace)
            .ApplyImplicitly(t => t.Namespace?.Contains(@namespace) == true);
    }

    /// <summary>Implicitly applies <see cref="IHarmonyPatcher"/> types with the specified attribute.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="harmonyId">The unique ID of the declaring mod.</param>
    /// <typeparam name="TAttribute">An <see cref="Attribute"/> type.</typeparam>
    /// <returns>The <see cref="Harmonizer"/> instance.</returns>
    internal static Harmonizer ApplyWithAttribute<TAttribute>(IModRegistry modRegistry, string harmonyId)
        where TAttribute : Attribute
    {
        Log.D($"[Harmonizer]: Gathering patches with {nameof(TAttribute)}...");
        return new Harmonizer(modRegistry, harmonyId)
            .ApplyImplicitly(t => t.GetCustomAttribute<TAttribute>() is not null);
    }

    /// <summary>Unapplies all <see cref="IHarmonyPatcher"/>s applied by this instance.</summary>
    /// <returns>Always <see langword="null"/>.</returns>
    internal Harmonizer? Unapply()
    {
        this.Harmony.UnpatchAll(this.HarmonyId);
        Log.D($"[Harmonizer]: Unapplied all patches for {this.HarmonyId}.");
        return null;
    }

    /// <summary>Instantiates and applies <see cref="IHarmonyPatcher"/> classes using reflection.</summary>
    /// <param name="predicate">An optional condition with which to limit the scope of applied <see cref="IHarmonyPatcher"/>es.</param>
    /// <returns>The <see cref="Harmonizer"/> instance.</returns>
    private Harmonizer ApplyImplicitly(Func<Type, bool>? predicate = null)
    {
        this.StartWatch();

        predicate ??= _ => true;
        var patchTypes = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IHarmonyPatcher)))
            .Where(t => t.IsAssignableTo(typeof(IHarmonyPatcher)) && !t.IsAbstract && predicate(t))
            .ToArray();

        Log.D($"[Harmonizer]: Found {patchTypes.Length} patch classes.");
        if (patchTypes.Length == 0)
        {
            return this;
        }

        Log.D("[Harmonizer]: Applying patches...");
        for (var i = 0; i < patchTypes.Length; i++)
        {
            var patchType = patchTypes[i];
#if RELEASE
            var debugAttribute = patchType.GetCustomAttribute<DebugAttribute>();
            if (debugAttribute is not null)
            {
                continue;
            }
#endif

            var implicitIgnoreAttribute = patchType.GetCustomAttribute<ImplicitIgnoreAttribute>();
            if (implicitIgnoreAttribute is not null)
            {
                continue;
            }

            var requiresModAttribute = patchType.GetCustomAttribute<RequiresModAttribute>();
            if (requiresModAttribute is not null)
            {
                if (!this._modRegistry.IsLoaded(requiresModAttribute.UniqueId))
                {
                    Log.D(
                        $"[Harmonizer]: The target mod {requiresModAttribute.UniqueId} is not loaded. {patchType.Name} will be ignored.");
                    continue;
                }

                var installedVersion = this._modRegistry.Get(requiresModAttribute.UniqueId)!.Manifest.Version;
                if (!string.IsNullOrEmpty(requiresModAttribute.Version) &&
                    installedVersion.IsOlderThan(requiresModAttribute.Version))
                {
                    Log.W(
                        $"[Harmonizer]: The integration patch {patchType.Name} will be ignored because the installed version of {requiresModAttribute.UniqueId} is older than minimum supported version." +
                        $" Please update {requiresModAttribute.UniqueId} in order to enable integrations with {this.HarmonyId}." +
                        $"\n\tInstalled version: {this._modRegistry.Get(requiresModAttribute.UniqueId)!.Manifest.Version}\n\tRequired version: {requiresModAttribute.Version}");
                    continue;
                }
            }

            var ignoreWithModAttribute = patchType.GetCustomAttribute<IgnoreWithModAttribute>();
            if (ignoreWithModAttribute is not null)
            {
                if (this._modRegistry.IsLoaded(ignoreWithModAttribute.UniqueId))
                {
                    Log.W(
                        $"[Harmonizer]: The conflicting mod {ignoreWithModAttribute.UniqueId} is loaded. {patchType.Name} will be ignored.");
                    continue;
                }
            }

            try
            {
                var patch = (IHarmonyPatcher?)patchType
                    .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null)
                    ?.Invoke(Array.Empty<object>());
                if (patch is null)
                {
                    ThrowHelper.ThrowMissingMethodException("Didn't find internal parameter-less constructor.");
                }

                if (patch.Apply(this.Harmony))
                {
                    Log.D($"[Harmonizer]: Applied {patchType.Name} to {patch.Target.GetFullName()}.");
                }
                else
                {
                    Log.W($"[Harmonizer]: {patchType.Name} was not applied.");
                }
            }
            catch (Exception ex)
            {
                Log.E($"[Harmonizer]: Failed to apply {patchType.Name}.\nHarmony returned {ex}");
            }
        }

        this.StopWatch();
        this.LogStats();
        return this;
    }

    [Conditional("DEBUG")]
    private void StartWatch()
    {
        this._sw.Start();
    }

    [Conditional("DEBUG")]
    private void StopWatch()
    {
        this._sw.Stop();
    }

    [Conditional("DEBUG")]
    private void LogStats()
    {
        var methodsPatched = this.Harmony.GetPatchedMethods().Count();
        var appliedPrefixes = this.Harmony.GetAllPrefixes(p => p.owner == this.HarmonyId).Count();
        var appliedPostfixes = this.Harmony.GetAllPostfixes(p => p.owner == this.HarmonyId).Count();
        var appliedTranspilers = this.Harmony.GetAllTranspilers(p => p.owner == this.HarmonyId).Count();
        var appliedFinalizers = this.Harmony.GetAllFinalizers(p => p.owner == this.HarmonyId).Count();
        var totalApplied = appliedPrefixes + appliedPostfixes + appliedTranspilers + appliedFinalizers;
        Log.A($"[Harmonizer]: {this.HarmonyId} patching completed in {this._sw.ElapsedMilliseconds}ms." +
              $"\nApplied {totalApplied} patches to {methodsPatched} methods, of which" +
              $"\n\t- {appliedPrefixes} prefixes" +
              $"\n\t- {appliedPostfixes} postfixes" +
              $"\n\t- {appliedTranspilers} transpilers" +
              $"\n\t- {appliedFinalizers} finalizers");
    }
}
