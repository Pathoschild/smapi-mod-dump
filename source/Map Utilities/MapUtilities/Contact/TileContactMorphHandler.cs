using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;
using xTile.Tiles;
using xTile.Dimensions;

namespace MapUtilities.Contact
{
    public static class TileContactMorphHandler
    {
        public static List<TileMorph> currentMorphs;

        public static void init()
        {
            currentMorphs = new List<TileMorph>();
        }

        public static void performSustainedMorph(GameLocation location, int x, int y, Tile tile)
        {
            Location loc = new Location(x, y);
            bool morphPresent = false;
            TileMorph thisMorph = null;
            foreach(TileMorph morph in currentMorphs)
            {
                if (morph.Equals(loc))
                {
                    morphPresent = true;
                    thisMorph = morph;
                    morph.timeRemaining = Math.Max(1, morph.timeRemaining);
                    break;
                }
            }
            if (!morphPresent)
            {
                thisMorph = new TileMorph(tile, location, loc);
                currentMorphs.Add(thisMorph);
            }
            if (thisMorph == null)
                return;
            tile.Layer.Tiles[x, y] = thisMorph.tileMorph;
            
        }

        public static void update()
        {
            List<int> indicesToRemove = new List<int>();
            foreach(TileMorph morph in currentMorphs)
            {
                morph.timeRemaining--;
                if(morph.timeRemaining < 0)
                {
                    morph.tileMorph.Layer.Tiles[morph.tileLocation.X, morph.tileLocation.Y] = morph.tileNormal;
                    indicesToRemove.Add(currentMorphs.IndexOf(morph));
                }
            }
            indicesToRemove.Reverse();
            foreach(int index in indicesToRemove)
            {
                currentMorphs.RemoveAt(index);
            }
        }

        public static void cleanup()
        {
            foreach (TileMorph morph in currentMorphs)
            {
                morph.tileMorph.Layer.Tiles[morph.tileLocation.X, morph.tileLocation.Y] = morph.tileNormal;
            }
            currentMorphs.Clear();
        }
    }
}
