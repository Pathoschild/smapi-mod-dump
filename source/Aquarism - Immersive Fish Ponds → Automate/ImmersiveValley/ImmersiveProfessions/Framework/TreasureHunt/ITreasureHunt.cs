/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.TreasureHunt;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley;

#endregion using directives

/// <summary>Interface for treasure hunts.</summary>
public interface ITreasureHunt
{
    /// <summary>Whether the <see cref="TreasureTile"/> is set to a valid target.</summary>
    public bool IsActive { get; }

    /// <summary>The target tile containing treasure.</summary>
    public Vector2? TreasureTile { get; }

    /// <summary>Try to start a new hunt at the specified location.</summary>
    /// <param name="location">The game location.</param>
    public bool TryStart(GameLocation location);

    /// <summary>Forcefully start a new hunt at the specified location.</summary>
    /// <param name="location">The game location.</param>
    /// <param name="target">The target treasure tile.</param>
    public void ForceStart(GameLocation location, Vector2 target);

    /// <summary>End the active hunt unsuccessfully.</summary>
    public void Fail();
}