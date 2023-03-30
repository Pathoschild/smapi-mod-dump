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
using System;
using System.Linq;
using xTile;
using xTile.Layers;
using xTile.Tiles;

namespace PlatoTK.Events
{
    internal class CallingTileActionEventArgs : ICallingTileActionEventArgs
    {
        private readonly string[] Commands;

        private readonly Action<bool> Callback;

        public string FullString => string.Join(" ", Commands);

        public string[] Parameter => Commands.Skip(1).ToArray();

        public string Trigger => Commands[0];

        public Farmer Caller { get; }

        public GameLocation Location { get; }

        public Point Position { get; }

        public Tile Tile => Layer?.Tiles[Position.X, Position.Y];
        public Layer Layer { get; }

        public Map Map => Location?.Map;

        public CallingTileActionEventArgs(string[] commands, Farmer who, GameLocation location, string layer, Point position, Action<bool> callback)
        {
            Commands = commands;
            Caller = who ?? Game1.player;
            Location = location ?? Game1.currentLocation;
            Layer = Map?.GetLayer(layer);
            Position = position;
            Callback = callback;
        }

        public void TakeOver(bool preventDefault)
        {
            Callback?.Invoke(preventDefault);
        }
    }

}
