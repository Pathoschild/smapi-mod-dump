using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Tiles;
using xTile.Dimensions;
using xTile.ObjectModel;
using StardewValley;

namespace MapUtilities.Contact
{
    public class TileMorph
    {
        public Tile tileNormal;
        public Tile tileMorph;

        public Location tileLocation;

        public int timeRemaining;
        public int cooldown;

        public TileMorph(Tile morphTile, GameLocation location, Location tileLocation)
        {
            tileNormal = morphTile;
            timeRemaining = 1;
            cooldown = -1;
            this.tileLocation = tileLocation;
            if (!morphTile.Properties.ContainsKey("Morph"))
            {
                tileMorph = morphTile;
                return;
            }
            try
            {
                string[] morphInfo = morphTile.Properties["Morph"].ToString().Split(' ');
                int newIndex = Convert.ToInt32(morphInfo[0]);
                string newSheetName = "";
                if (morphInfo.Length > 1)
                    newSheetName = morphInfo[1];
                TileSheet newSheet = morphTile.TileSheet;
                foreach (TileSheet sheet in location.map.TileSheets)
                {
                    //Weak match - overridden by exact match if found
                    if (sheet.Id.ToLower() == newSheetName.ToLower())
                    {
                        newSheet = sheet;
                    }
                    //Exact match always favored
                    if (sheet.Id == newSheetName)
                    {
                        newSheet = sheet;
                        break;
                    }
                }
                tileMorph = new StaticTile(morphTile.Layer, newSheet, morphTile.BlendMode, newIndex);
                foreach(string property in tileNormal.Properties.Keys)
                {
                    tileMorph.Properties[property] = tileNormal.Properties[property];
                }
            }
            catch (FormatException) { tileMorph = morphTile; }
        }

        public override bool Equals(object value)
        {
            if (value is Location)
            {
                Location otherLoc = (Location)value;
                return tileLocation.X == otherLoc.X && tileLocation.Y == otherLoc.Y;
            }
            if(value is TileMorph)
            {
                TileMorph otherMorph = (TileMorph)value;
                return tileLocation.X == otherMorph.tileLocation.X && tileLocation.Y == otherMorph.tileLocation.Y;
            }
            return false;
        }
    }
}
