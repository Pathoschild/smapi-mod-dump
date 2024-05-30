/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Shared.Pathfinding;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Character_Pathfinder
{
    internal static ConditionalWeakTable<Character, MTDStarLite> Values { get; } = [];

    internal static MTDStarLite Get_IncrementalPathfinder(this Character character)
    {
        return Values.GetValue(character, Create);
    }

    internal static void ResetIncrementalPathfinder(this Character character)
    {
        Values.Remove(character);
    }

    private static MTDStarLite Create(Character character)
    {
        const CollisionMask collisionMask = CollisionMask.Buildings | CollisionMask.Furniture | CollisionMask.Objects |
                                            CollisionMask.TerrainFeatures | CollisionMask.LocationSpecific;
        return new MTDStarLite(
            character.currentLocation,
            (l, t) => l.isTilePassable(t) && !l.IsTileOccupiedBy(t, collisionMask));
    }
}
