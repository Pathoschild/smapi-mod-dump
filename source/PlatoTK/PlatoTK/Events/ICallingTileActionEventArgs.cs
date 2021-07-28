/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using xTile;
using xTile.Layers;
using xTile.Tiles;

namespace PlatoTK.Events
{
    public interface ICallingTileActionEventArgs
    {
        string FullString {get;}

        string Trigger {get;}

        string[] Parameter { get; }

        Farmer Caller { get; }

        GameLocation Location { get; }

        Point Position { get; }

        Tile Tile { get;}

        Layer Layer { get;}

        Map Map { get;}

        void TakeOver(bool preventDefault);
    }

}
