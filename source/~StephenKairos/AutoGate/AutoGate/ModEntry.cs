using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Network;
using SObject = StardewValley.Object;
using System;
using System.Collections.Generic;

namespace AutoGate
{
    public class ModEntry : Mod
    {
        public SerializableDictionary<Vector2, SObject> gateList;

        public override void Entry(IModHelper helper)
        {
            PlayerEvents.Warped += this.EnteredNewLocation;
            LocationEvents.ObjectsChanged += this.CreatedOrDestroyedGate;
            GameEvents.HalfSecondTick += this.ReceiveHalfSecondTick;
        }

        private void EnteredNewLocation(object sender, EventArgsPlayerWarped e)
        {
            this.gateList = new SerializableDictionary<Vector2, StardewValley.Object>();
            OverlaidDictionary<Vector2, SObject> objects = Game1.currentLocation.objects;
            foreach (Vector2 key in objects.Keys)
            {
                if (objects[key].name.Equals("Gate"))
                {
                    this.gateList.Add(key, objects[key]);
                    //this.Monitor.Log(string.Format("{0}", (object)key.ToString()), (LogLevel)1);
                }
            }
        }

        private void CreatedOrDestroyedGate(object sender, EventArgsLocationObjectsChanged e)
        {
            this.gateList = new SerializableDictionary<Vector2, StardewValley.Object>();
            OverlaidDictionary<Vector2, SObject> objects = Game1.currentLocation.objects;
            
            foreach (Vector2 key in objects.Keys)
            {
                if (objects[key].name.Equals("Gate"))
                {
                    this.gateList.Add(key, objects[key]);
                    //this.Monitor.Log(string.Format("{0}", (object)key.ToString()), (LogLevel)1);
                }
            }
        }

        private void ReceiveHalfSecondTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady || gateList == null)
                return;
            Vector2[] array = Utility.getAdjacentTileLocations(Game1.player.getTileLocation()).ToArray();
            foreach (Vector2 key in this.gateList.Keys)
            {
                bool flag = false;
                foreach (Vector2 other in array)
                {
                    if (key.Equals(other) && !(Game1.currentLocation.objects)[key].isPassable())
                        flag = true;
                }
                if (flag)
                    Game1.currentLocation.objects[key].checkForAction(Game1.player, false);
                else if ((Game1.currentLocation.objects)[key].isPassable())
                    Game1.currentLocation.objects[key].checkForAction(Game1.player, false);
            }
        }
    }
}
