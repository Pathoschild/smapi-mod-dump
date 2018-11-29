using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace GateOpener
{
    public class GateOpenerMainClass : Mod
    {
        /*********
        ** Properties
        *********/
        private readonly Dictionary<Vector2, Fence> OpenGates = new Dictionary<Vector2, Fence>();


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            GameEvents.FourthUpdateTick += this.GameEvents_FourthUpdateTick;
        }

        private void MyLog(String theString)
        {
#if DEBUG
            Monitor.Log(theString);
#endif
        }


        /*********
        ** Private methods
        *********/
        private void DebugThing(object data, string descriptor = "")
        {
            this.Helper.WriteJsonFile("debug.json", data);
            string result = File.ReadAllText(Path.Combine(this.Helper.DirectoryPath, "debug.json"));
            this.Monitor.Log($"{descriptor}\n{result}");
        }

        private Fence GetGate(BuildableGameLocation location, Vector2 pos)
        {
            if (!location.objects.TryGetValue(pos, out StardewValley.Object obj))
                return null;

            if (obj is Fence fence && fence.isGate.Value && !this.OpenGates.ContainsKey(pos))
            {
                this.OpenGates[pos] = fence;
                return fence;
            }
            return null;
        }

        private Fence LookAround(BuildableGameLocation location, List<Vector2> list)
        {
            foreach (Vector2 pos in list)
            {
                Fence gate = this.GetGate(location, pos);
                if (gate != null)
                    return gate;
            }
            return null;
        }

        private void GameEvents_FourthUpdateTick(object sender, EventArgs e)
        {
            if (Game1.currentLocation is BuildableGameLocation location)
            {
                List<Vector2> adj = Utility.getAdjacentTileLocations(Game1.player.getTileLocation());
                Fence gate = this.LookAround(location, adj);
                if (gate != null)
                {
                    //MyLog(gate.ToString());
                    gate.gatePosition.Set(88);
                    Game1.playSound("doorClose");
                }

                //need to close it now...
                foreach (KeyValuePair<Vector2, Fence> gateObj in OpenGates)
                {
                    if (Game1.player.getTileLocation() != gateObj.Key && !adj.Contains(gateObj.Key))
                    {
                        gateObj.Value.gatePosition.Set(0);
                        Game1.playSound("doorClose");
                        OpenGates.Remove(gateObj.Key);
                        break;
                    }
                }
            }
        }
    }
}
