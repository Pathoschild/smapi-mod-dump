/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.FuryCore.Models.CustomEvents;

using StardewModdingAPI;
using StardewMods.FuryCore.Interfaces.CustomEvents;
using StardewMods.FuryCore.Interfaces.GameObjects;

/// <inheritdoc />
public class ConfiguringGameObjectEventArgs : IConfiguringGameObjectEventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ConfiguringGameObjectEventArgs" /> class.
    /// </summary>
    /// <param name="gameObject">The game object being configured.</param>
    /// <param name="manifest">The mod manifest to subscribe to GMCM with.</param>
    public ConfiguringGameObjectEventArgs(IGameObject gameObject, IManifest manifest)
    {
        this.GameObject = gameObject;
        this.ModManifest = manifest;
    }

    /// <inheritdoc />
    public IGameObject GameObject { get; }

    /// <inheritdoc />
    public IManifest ModManifest { get; }
}