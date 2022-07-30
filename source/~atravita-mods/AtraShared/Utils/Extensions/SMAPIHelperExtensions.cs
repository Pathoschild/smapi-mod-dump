/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace AtraShared.Utils.Extensions;

/// <summary>
/// Extension methods on SMAPI's helper class.
/// </summary>
public static class SMAPIHelperExtensions
{
    /// <summary>
    /// Writes the config async.
    /// </summary>
    /// <typeparam name="TConfig">Type of the config model.</typeparam>
    /// <param name="helper">SMAPI helper.</param>
    /// <param name="monitor">SMAPI logger.</param>
    /// <param name="config">Config class.</param>
    public static void AsyncWriteConfig<TConfig>(this IModHelper helper, IMonitor monitor, TConfig config)
        where TConfig : class, new()
    {
        Task.Run(() => helper.WriteConfig(config))
            .ContinueWith(t =>
            {
                switch(t.Status)
                {
                    case TaskStatus.RanToCompletion:
                        monitor.Log("Configuration written successfully!");
                        break;
                    case TaskStatus.Faulted:
                        monitor.Log($"Configuration failed to write {t.Exception}", LogLevel.Error);
                        break;
                }
            });
    }
}
