using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Tasks performed after an NPC is added or removed in a location.</summary>
        private void NpcListChanged(object sender, NpcListChangedEventArgs e)
        {
            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything; most of this mod's functions should be limited to the host player

            foreach (Monster monster in e.Removed.OfType<Monster>()) //for each monster that was removed
            {
                if (monster.id <= Utility.MonsterTracker.HighestID) //if this monster's ID is within the range used by this mod
                {
                    IEnumerable<SavedObject> lootList = Utility.MonsterTracker.GetLoot(monster.id); //get this monster's custom loot set, if any

                    if (lootList != null) //if this monster has any loot to spawn
                    {
                        //get the position where the monster's loot should drop
                        Point center = monster.GetBoundingBox().Center;
                        Vector2 lootPosition = new Vector2(center.X, center.Y);

                        foreach (SavedObject loot in lootList) //for each loot object
                        {
                            monster.currentLocation.debris.Add(new Debris(Utility.CreateItem(loot), lootPosition, lootPosition)); //create and "drop" the loot at the monster's location
                        }
                    }
                }
            }
        }
    }
}
