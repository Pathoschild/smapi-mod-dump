/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Services.Integrations.FauxCore;

/// <summary>Api for shared functionality between mods.</summary>
public interface IFauxCoreApi
{
    /// <summary>Create an instance of the ILog service for the given IMonitor.</summary>
    /// <param name="monitor">Dependency used for monitoring and logging.</param>
    /// <returns>An instance of ILog that is associated with the provided IMonitor.</returns>
    public ILog CreateLogService(IMonitor monitor);

    /// <summary>Create an instance of the IPatchManager service.</summary>
    /// <returns>An instance of IPatchManager.</returns>
    /// <param name="log">Dependency used for logging debug information to the console.</param>
    public IPatchManager CreatePatchService(ILog log);

    /// <summary>Create an instance of the IThemeHelper service.</summary>
    /// <returns>An instance of IThemeHelper.</returns>
    public IThemeHelper CreateThemeService();
}