using SpaceCore.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using xTile;
using Entoarox.Framework;

namespace StardewBrewery
{
    public class BreweryLocation : CustomDecoratableLocation, ICustomItem
    {
        public BreweryLocation()
        : base(Game1.content.Load<Map>(@"Maps\Brewery"), "Brewery")
        {
        }

        public override List<Rectangle> getFloors()
        {
            return new List<Rectangle> {};
        }

        public override List<Rectangle> getWalls()
        {
            return new List<Rectangle> {
                new Rectangle(17, 5, 4, 3),
                new Rectangle(29, 5, 11, 3)
            };
        }

        public override void setWallpaper(int which, int whichRoom = -1, bool persist = false)
        {
        }

        public override void resetForPlayerEntry()
        {
            base.resetForPlayerEntry();
            if (Game1.isDarkOut())
                Game1.ambientLight = new Color(180, 180, 0);
        }
    }
}
