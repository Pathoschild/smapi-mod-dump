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
using DaLion.Shared.Classes;
using Microsoft.Xna.Framework;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class GameLocation_Quadtree
{
    internal static ConditionalWeakTable<GameLocation, Quadtree<Character>> Values { get; } = [];

    // returns nullable to avoid unnecessary iteration
    internal static Quadtree<Character> Get_Quadtree(this GameLocation location)
    {
        return Values.GetValue(location, Create);
    }

    private static Quadtree<Character> Create(GameLocation location)
    {
        var regionBounds = new Rectangle(0, 0, location.Map.Layers[0].LayerWidth, location.Map.Layers[0].LayerHeight);
        var tree = new Quadtree<Character>(regionBounds, c => c.GetBoundingBox());
        foreach (var c in location.characters)
        {
            tree.Insert(c);
        }

        return tree;
    }
}
