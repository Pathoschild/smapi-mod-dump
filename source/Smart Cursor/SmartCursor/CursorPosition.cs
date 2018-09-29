using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCursor {
    class CursorPosition : ICursorPosition {
        public Vector2 AbsolutePixels { get; }
        public Vector2 ScreenPixels { get; }
        public Vector2 Tile { get; }
        public Vector2 GrabTile { get; }

        public CursorPosition(Vector2 absolutePixels, Vector2 screenPixels, Vector2 tile, Vector2 grabTile) {
            AbsolutePixels = absolutePixels;
            ScreenPixels = screenPixels;
            Tile = tile;
            GrabTile = grabTile;
        }

        public bool Equals(ICursorPosition other) {
            throw new NotImplementedException();
        }
    }
}
