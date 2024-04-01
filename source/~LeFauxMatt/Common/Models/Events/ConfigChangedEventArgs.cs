/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Models.Events;

/// <summary>Represents the event arguments for a configuration changes.</summary>
/// <typeparam name="TConfig">The config type.</typeparam>
internal sealed class ConfigChangedEventArgs<TConfig>(TConfig config) : EventArgs
{
    /// <summary>Gets the current config options.</summary>
    public TConfig Config { get; } = config;
}