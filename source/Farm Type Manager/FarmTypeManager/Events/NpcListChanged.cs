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
                IEnumerable<SavedObject> lootList = Utility.MonsterTracker.GetLoot(monster); //get this monster's custom loot list, if any

                if (lootList != null) //if this monster has a loot list
                {
                    Utility.Monitor.VerboseLog($"Dropping loot for defeated monster. Monster: {monster.displayName}. ID: {monster.id}. Location: {monster.getTileX()},{monster.getTileY()} ({monster.currentLocation.Name}).");

                    //get the position where the monster's loot should drop
                    Point center = monster.GetBoundingBox().Center;
                    Vector2 lootPosition = new Vector2(center.X, center.Y);

                    foreach (SavedObject loot in lootList) //for each loot object
                    {
                        double? spawnChance = loot.ConfigItem?.PercentChanceToSpawn; //get this item's spawn chance, if provided
                        if (spawnChance.HasValue && spawnChance.Value < Utility.RNG.Next(100)) //if this item "fails" its chance to spawn
                        {
                            continue; //skip to the next item
                        }

                        //if this loot has contents with spawn chances, process them
                        if (loot.ConfigItem?.Contents != null) //if this loot has contents
                        {
                            for (int content = loot.ConfigItem.Contents.Count - 1; content >= 0; content--) //for each of the contents
                            {
                                List<SavedObject> contentSave = Utility.ParseSavedObjectsFromItemList(new object[] { loot.ConfigItem.Contents[content] }, $"[unknown: monster loot dropped at {monster.currentLocation.Name}]"); //parse this into a saved object

                                double? contentSpawnChance = contentSave[0].ConfigItem?.PercentChanceToSpawn; //get this item's spawn chance, if provided
                                if (contentSpawnChance.HasValue && contentSpawnChance.Value < Utility.RNG.Next(100)) //if this item "fails" its chance to spawn
                                {
                                    loot.ConfigItem.Contents.RemoveAt(content); //remove this content from the loot
                                }
                            }
                        }

                        Game1.createItemDebris(Utility.CreateItem(loot), lootPosition, Utility.RNG.Next(4), monster.currentLocation); //create and "drop" the loot at the monster's location
                    }
                }
            }
        }
    }
}
