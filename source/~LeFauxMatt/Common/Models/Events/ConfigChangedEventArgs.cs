/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Models.Events;
#else
namespace StardewMods.Common.Models.Events;
#endif

/// <summary>Represents the event arguments for a configuration changes.</summary>
/// <typeparam name="TConfig">The config type.</typeparam>
internal sealed class ConfigChangedEventArgs<TConfig> : EventArgs
{
    /// <summary>Initializes a new instance of the <see cref="ConfigChangedEventArgs{TConfig}" /> class.</summary>
    /// <param name="config">The config.</param>
    public ConfigChangedEventArgs(TConfig config) => this.Config = config;

    /// <summary>Gets the current config options.</summary>
    public TConfig Config { get; }
}