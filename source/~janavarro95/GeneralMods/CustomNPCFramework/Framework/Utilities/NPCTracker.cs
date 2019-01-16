using System.Collections.Generic;
using CustomNPCFramework.Framework.NPCS;
using Microsoft.Xna.Framework;
using StardewValley;

namespace CustomNPCFramework.Framework.Utilities
{
    /// <summary>Used to keep track of all of the custom npcs.</summary>
    public class NpcTracker
    {
        /// <summary>A list used to keep track of the npcs.</summary>
        public List<ExtendedNpc> moddedNpcs;

        /// <summary>Construct an instance.</summary>
        public NpcTracker()
        {
            this.moddedNpcs = new List<ExtendedNpc>();
        }

        /// <summary>Use this to add a new npc into the game.</summary>
        /// <param name="loc">The game location to add the npc to.</param>
        /// <param name="npc">The extended npc to add to the location.</param>
        public void addNewNpcToLocation(GameLocation loc, ExtendedNpc npc)
        {
            this.moddedNpcs.Add(npc);
            npc.defaultLocation = loc;
            npc.currentLocation = loc;
            loc.addCharacter(npc);
        }

        /// <summary>Add a npc to a location.</summary>
        /// <param name="loc">The game location to add an npc to.</param>
        /// <param name="npc">The extended npc to add to the location.</param>
        /// <param name="tilePosition">The tile position at the game location to add the mpc to.</param>
        public void addNewNpcToLocation(GameLocation loc, ExtendedNpc npc, Vector2 tilePosition)
        {
            this.moddedNpcs.Add(npc);
            npc.defaultLocation = loc;
            npc.currentLocation = loc;
            npc.position.Value = tilePosition * Game1.tileSize;
            loc.addCharacter(npc);
        }

        /// <summary>Use this simply to remove a single npc from a location.</summary>
        public void removeCharacterFromLocation(GameLocation loc, ExtendedNpc npc)
        {
            loc.characters.Remove(npc);
        }

        /// <summary>Use this to completly remove and npc from the game as it is removed from the location and is no longer tracked.</summary>
        /// <param name="npc">The npc to remove from the location.</param>
        public void removeFromLocationAndTrackingList(ExtendedNpc npc)
        {
            npc.currentLocation?.characters.Remove(npc);
            this.moddedNpcs.Remove(npc);
        }

        /// <summary>Use this to clean up all of the npcs before the game is saved.</summary>
        public void cleanUpBeforeSave()
        {
            foreach (ExtendedNpc npc in this.moddedNpcs)
            {
                //npc.currentLocation.characters.Remove(npc);
                //Game1.removeThisCharacterFromAllLocations(npc);
                Game1.removeCharacterFromItsLocation(npc.Name);
                Class1.ModMonitor.Log("Removed an npc!");
                //Do some saving code here.
            }
        }

        /// <summary>Use this to load in all of the npcs again after saving.</summary>
        public void afterSave()
        {
            foreach (ExtendedNpc npc in this.moddedNpcs)
                npc.defaultLocation.addCharacter(npc);
        }
    }
}
