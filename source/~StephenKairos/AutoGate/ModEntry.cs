using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SObject = StardewValley.Object;
using StardewValley.Network;

namespace AutoGate
{
    public class ModEntry : Mod
    {
        public SerializableDictionary<Vector2, SObject> gateList;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += this.OnWarped;
            helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }

        /// <summary>Raised after a player warps to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (!e.IsLocalPlayer)
                return;

            this.gateList = new SerializableDictionary<Vector2, SObject>();
            OverlaidDictionary objects = Game1.currentLocation.objects;
            foreach (Vector2 key in objects.Keys)
            {
                if (objects[key].name.Equals("Gate"))
                {
                    this.gateList.Add(key, objects[key]);
                    //this.Monitor.Log(string.Format("{0}", (object)key.ToString()), (LogLevel)1);
                }
            }
        }

        /// <summary>Raised after objects are added or removed in a location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            this.gateList = new SerializableDictionary<Vector2, SObject>();
            OverlaidDictionary objects = Game1.currentLocation.objects;
            
            foreach (Vector2 key in objects.Keys)
            {
                if (objects[key].name.Equals("Gate"))
                {
                    this.gateList.Add(key, objects[key]);
                    //this.Monitor.Log(string.Format("{0}", (object)key.ToString()), (LogLevel)1);
                }
            }
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!e.IsMultipleOf(30)) // half-second tick
                return;
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
