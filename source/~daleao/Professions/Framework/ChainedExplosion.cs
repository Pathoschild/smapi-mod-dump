/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework;

#region using directives

using DaLion.Shared.Classes;
using Microsoft.Xna.Framework;

#endregion using directives

internal sealed class ChainedExplosion
{
    private readonly Queue<HashSet<Vector2>> _tilesToExplode = [];
    private readonly HashSet<Vector2> _tilesExploded = [];
    private readonly GameLocation _location;
    private readonly int _damage;
    private readonly Farmer _pyro;

    internal ChainedExplosion(GameLocation location, Vector2 origin, int radius, int damage, Farmer pyro)
    {
        this._location = location;
        this._damage = damage;
        this._pyro = pyro;
        HashSet<Vector2> chained = [];
        foreach (var candidate in new CircleTileGrid(origin, (uint)radius + 2) - new CircleTileGrid(origin, (uint)radius))
        {
            if (this._location.Objects.ContainsKey(candidate) || this._location.characters.Any(c => c.Tile == candidate))
            {
                chained.Add(candidate);
            }
        }

        this._tilesToExplode.Enqueue(chained);
    }

    internal bool Update()
    {
        if (!this._tilesToExplode.TryDequeue(out var toExplode))
        {
            return true;
        }

        HashSet<Vector2> chained = [];
        foreach (var tile in toExplode)
        {
            if (!this._tilesExploded.Add(tile))
            {
                continue;
            }

            this._location.explode(tile, 0, this._pyro, true, this._damage);
            foreach (var candidate in new CircleTileGrid(tile, 2).Tiles)
            {
                if (this._location.Objects.ContainsKey(candidate) || this._location.characters.Any(c => c.Tile == candidate))
                {
                    chained.Add(candidate);
                }
            }
        }

        if (chained.Count == 0)
        {
            return true;
        }

        this._tilesToExplode.Enqueue(chained);
        return false;
    }
}
