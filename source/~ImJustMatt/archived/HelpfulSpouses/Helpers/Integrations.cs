/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.HelpfulSpouses.Helpers;

using System;
using System.Collections.Generic;
using StardewMods.Common.Integrations.ContentPatcher;
using StardewMods.Common.Integrations.ProjectFluent;

/// <summary>
///     Handles integrations with other mods.
/// </summary>
internal class Integrations
{
    private static Integrations? Instance;

    private readonly ContentPatcherIntegration _contentPatcher;
    private readonly IManifest _manifest;

    private Integrations(IModHelper helper, IManifest manifest)
    {
        this._manifest = manifest;
        this._contentPatcher = new(helper.ModRegistry);

        RegisterToken(
            "TermOfEndearment",
            () => spouse);
    }

    /// <inheritdoc cref="IContentPatcherApi.RegisterToken(IManifest, string, Func{IEnumerable{string}})" />
    public static void RegisterToken(string name, Func<string?> getValue)
    {
        IEnumerable<string>? GetValue()
        {
            var value = getValue();
            return value is null ? null : new[] { value };
        }

        Integrations.Instance?._contentPatcher.API?.RegisterToken(Integrations.Instance._manifest, name, GetValue);
    }

    /// <summary>
    ///     Initializes <see cref="Integrations" />.
    /// </summary>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <param name="manifest">Manifest for the SMAPI mod.</param>
    /// <returns>Returns an instance of the <see cref="Integrations" /> class.</returns>
    public static Integrations Init(IModHelper helper, IManifest manifest)
    {
        return Integrations.Instance ??= new(helper, manifest);
    }
}