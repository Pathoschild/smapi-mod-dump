/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Common.Extensions.SMAPI;

#region using directives

using Newtonsoft.Json.Linq;
using System.IO;

#endregion using directives

/// <summary>Extensions for the <see cref="IModHelper"/> interface.</summary>
public static class ModHelperExtensions
{
    /// <summary>Get the <see cref="IMod"/> interface for an external mod.</summary>
    /// <param name="uniqueID">The unique ID of the external mod.</param>
    /// <remarks>Will only for mods that implement <see cref="IMod"/>; i.e., will not work for content packs.</remarks>
    public static IMod? GetModEntryFor(this IModHelper helper, string uniqueID)
    {
        var modInfo = helper.ModRegistry.Get(uniqueID);
        if (modInfo is not null) return (IMod)modInfo.GetType().GetProperty("Mod")!.GetValue(modInfo)!;

        Log.V($"{uniqueID} mod not found.");
        return null;
    }

    /// <summary>Read an external mod's configuration file.</summary>
    /// <param name="uniqueID">The unique ID of the external mod.</param>
    /// <remarks>Will only for mods that implement <see cref="IMod"/>; i.e., will not work for content packs.</remarks>
    public static JObject? ReadConfigExt(this IModHelper helper, string uniqueID)
    {
        var modEntry = helper.GetModEntryFor(uniqueID);
        return modEntry?.Helper.ReadConfig<JObject>();
    }

    /// <summary>Read an external content pack's configuration file.</summary>
    /// <param name="uniqueID">The unique ID of the external mod.</param>
    /// <remarks>Will work for any mod, but is reserved for content packs.</remarks>
    public static JObject? ReadContentPackConfig(this IModHelper helper, string uniqueID)
    {
        var modInfo = helper.ModRegistry.Get(uniqueID);
        if (modInfo is null)
        {
            Log.V($"{uniqueID} mod not found. Integrations disabled.");
            return null;
        }

        var modPath = (string)modInfo.GetType().GetProperty("DirectoryPath")!.GetValue(modInfo)!;
        try
        {
            var config = JObject.Parse(File.ReadAllText(Path.Combine(modPath, "config.json")));
            Log.V("Success. Integrations will be enabled.");
            return config;
        }
        catch (FileNotFoundException)
        {
            Log.W(
                $"Detected {uniqueID}, but a corresponding config file was not found in the expected location '{modPath}'.\nIntegrations will not be enabled until the next restart.");
            return null;
        }
    }
}