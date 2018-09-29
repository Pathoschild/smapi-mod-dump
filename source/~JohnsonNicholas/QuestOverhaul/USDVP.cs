using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using TwilightShards.Common;
using StardewValley.Quests;
using TwilightShards.Stardew.Common;
using SObject = StardewValley.Object;
using StardewValley.Menus;
using System.Collections.Specialized;
using StardewValley.Monsters;
using System.Collections.Generic;
using StardewValley.TerrainFeatures;
using System.Linq;

namespace TwilightShards.USDVP
{
    public class USDVP : Mod
    {
        public override void Entry(IModHelper Helper)
        {
            SaveEvents.AfterLoad += GameEvents_OneSecondTick;
           
        }

        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {            
            //resolve bug 3 (issue 46)
            foreach (GameLocation location in Game1.locations)
            {
                for (int index = location.terrainFeatures.Count - 1; index >= 0; --index)
                {
                    KeyValuePair<Vector2, TerrainFeature> keyValuePair = location.terrainFeatures.ElementAt<KeyValuePair<Vector2, TerrainFeature>>(index);
                    if (!location.isTileOnMap(keyValuePair.Key))
                    {
                        SerializableDictionary<Vector2, TerrainFeature> terrainFeatures = location.terrainFeatures;
                        keyValuePair = location.terrainFeatures.ElementAt<KeyValuePair<Vector2, TerrainFeature>>(index);
                        Vector2 key = keyValuePair.Key;
                        terrainFeatures.Remove(key);
                    }
                }
            }          
        }

        private void GameEvents_OneSecondTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            Monitor.Log("Running fixes....");
            for (int index = 0; index < Game1.player.questLog.Count; index++)
            {
                //resolve bug 1 (issue 44)
                if (Game1.player.questLog[index].id == 15 && Game1.player.questLog[index] is SlayMonsterQuest ourQuest)
                {
                    //check to see what hte monster actually is
                    if (ourQuest.monster.name != "Green Slime")
                    {
                        ourQuest.monsterName = "Green Slime";
                        ourQuest.numberToKill = 10;
                        ourQuest.monster = new Monster("Green Slime", Vector2.Zero);
                        ourQuest.questTitle = "Initation";
                        ourQuest.questDescription = "If you can slay 10 slimes, you'll have earned your place in the Adventurer's Guild.";
                        ourQuest.actualTarget = null;
                        ourQuest.targetMessage = null;

                    }
                }
            }
        }
    }
}
