using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedSaveCore.Framework
{
    public class LocationHandler:IInformationHandler
    {
        /// <summary>
        /// The locations to store while saving is occuring.
        /// </summary>
        public List<GameLocation> locations;

        public GameLocation oldLocation;
        public Vector2 position;
        public int oldFacingDirection;

        public LocationHandler()
        {
            this.locations = new List<GameLocation>();
        }

        /// <summary>
        /// Restores all game locations once the game is finished loading.
        /// </summary>
        public void afterSave()
        {
            foreach (var loc in locations)
            {
                Game1.locations.Add(loc);
            }
            locations.Clear();
            Game1.warpFarmer(oldLocation.name, (int)position.X/Game1.tileSize, (int)position.Y/Game1.tileSize, oldFacingDirection);
        }

        //Removes all game locations for the game to save.
        public void beforeSave()
        {

            oldLocation = Game1.player.currentLocation;
            position = Game1.player.position;
            oldFacingDirection = Game1.player.facingDirection;

            Vector2 bed = Game1.player.mostRecentBed;
            Game1.warpFarmer("Farmhouse", (int)bed.X/Game1.tileSize, (int)bed.Y/Game1.tileSize, 2);
            foreach (var loc in Game1.locations)
            {
                //UnifiedSaveCore.monitor.Log(loc.GetType().ToString());
                //ModCore.monitor.Log();
                foreach (var type in UnifiedSaveCore.modTypes)
                {
                    if (loc.GetType().ToString() == type.ToString())
                    {
                        UnifiedSaveCore.monitor.Log("Temporarily removing unexpected location:"+ loc.name+" type: " + loc.GetType().ToString());
                        locations.Add(loc);
                    }
                }
            }
            foreach (var v in locations)
            {
                Game1.locations.Remove(v);
            }
        }

        public void afterLoad()
        {
            //do nothing
        }

    }
}
