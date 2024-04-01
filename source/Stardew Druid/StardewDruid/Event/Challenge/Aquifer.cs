/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewDruid.Cast;
using StardewDruid.Map;
using StardewDruid.Monster.Template;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace StardewDruid.Event.Challenge
{
    public class Aquifer : ChallengeHandle
    {

        public Queue<Vector2> trashQueue;

        public int trashCollected;

        public BigBat bossMonster;

        public Aquifer(Vector2 target,  Quest quest)
            : base(target, quest)
        {
            
        }

        public override void EventTrigger()
        {

            cues = DialogueData.DialogueScene(questData.name);

            challengeSpawn = new() { 5, };

            challengeFrequency = 2;

            challengeAmplitude = 1;

            challengeSeconds = 60;

            challengeWithin = new(17, 10);

            challengeRange = new(9, 9);

            challengeTorches = new() { new(20, 13), };

            if (questData.name.Contains("Two"))
            {
                
                challengeFrequency = 1;
                
                challengeAmplitude = 1;
            
            }

            SetupSpawn();

            Layer backLayer = targetLocation.Map.GetLayer("Back");

            trashQueue = new();

            trashCollected = 0;

            for (int i = 0; i < 3; i++)
            {

                List<Vector2> castSelection = ModUtility.GetTilesWithinRadius(targetLocation, targetVector, 3 + i);

                foreach (Vector2 castVector in castSelection)
                {

                    Tile backTile = backLayer.PickTile(new xTile.Dimensions.Location((int)castVector.X * 64, (int)castVector.Y * 64), Game1.viewport.Size);

                    if (backTile != null)
                    {

                        if (backTile.TileIndexProperties.TryGetValue("Water", out _))
                        {

                            if (randomIndex.Next(2) == 0) { continue; };

                            trashQueue.Enqueue(castVector);

                        }

                    }

                }

            }

            Mod.instance.CastMessage("Stand your ground until the rite completes", 2);

            SetTrack("tribal");

            Mod.instance.RegisterEvent(this, "active");

        }

        public override void MinutesLeft(int minutes)
        {

            Mod.instance.CastMessage($"{minutes} minutes left until cleanup complete", 2);

        }

        public override void RemoveMonsters()
        {

            if (bossMonster != null)
            {

                Mod.instance.rite.castLocation.characters.Remove(bossMonster);

                bossMonster = null;

            }

            base.RemoveMonsters();

        }

        public override bool EventExpire()
        {

            if (eventLinger == -1)
            {

                if (trashCollected < 12)
                {

                    Mod.instance.CastMessage("Try again tomorrow to collect more trash");

                    Mod.instance.ReassignQuest(questData.name);

                }
                else
                {
                    Mod.instance.CastMessage($"Collected {trashCollected} pieces of trash!", 2);

                    EventComplete();

                    if (!questData.name.Contains("Two"))
                    {

                        List<string> NPCIndex = VillagerData.VillagerIndex("mountain");

                        Mod.instance.CastMessage("You have gained favour with the mountain residents and their friends",1,true);

                        ModUtility.UpdateFriendship(Game1.player,NPCIndex);

                        Mod.instance.dialogue["Effigy"].AddSpecial("Effigy", "Aquifer");

                    }

                }

                eventLinger = 3;

                RemoveMonsters();

                return true;

            }

            return base.EventExpire();

        }

        public override void EventRemove()
        {
            if (ladders.Count > 0)
            {
                Layer layer = targetLocation.map.GetLayer("Buildings");

                int x = (int)ladders.First().X;

                int y = (int)ladders.First().Y;

                if (layer.Tiles[x, y] == null)
                {
                    
                    layer.Tiles[x, y] = new StaticTile(layer, targetLocation.map.TileSheets[0], 0, 173);
                    
                    Game1.player.TemporaryPassableTiles.Add(new Microsoft.Xna.Framework.Rectangle(x * 64, y * 64, 64, 64));
                    
                    Mod.instance.CastMessage("A way down has appeared");
                
                }
            
            }
            
            base.EventRemove();
        
        }

        public override void EventInterval()
        {

            activeCounter++;

            monsterHandle.SpawnCheck();

            if (eventLinger != -1)
            {

                return;

            }
            
            RemoveLadders();
            
            monsterHandle.SpawnInterval();

            if (randomIndex.Next(2) == 0)
            {

                ThrowTrash();

            }

            if (activeCounter == 20)
            {

                bossMonster = new(new Vector2(30, 13),Mod.instance.CombatModifier());

                bossMonster.posturing.Set(true);

                Mod.instance.rite.castLocation.characters.Add(bossMonster);

                bossMonster.update(Game1.currentGameTime, Mod.instance.rite.castLocation);

            }

            if (activeCounter <= 20)
            {
                return;
            }

            if (ModUtility.MonsterVitals(bossMonster,targetLocation))
            {

                DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = bossMonster, }, activeCounter);

                switch (activeCounter)
                {

                    case 39:

                        bossMonster.posturing.Set(false);

                        bossMonster.focusedOnFarmers = true; 
                        
                        break;

                    case 58:

                        ModUtility.AnimateRockfall(targetLocation, bossMonster.Tile, 0);

                        break;

                    case 59:

                        bossMonster.takeDamage(999, 0, 0, false, 999, targetPlayer);

                        break;

                    default: 
                        
                        break;

                }

            }


        }

        public void ThrowTrash()
        {

            if (trashQueue.Count == 0)
            {
                return;
            }

            Vector2 trashVector = trashQueue.Dequeue();

            Dictionary<int, int> artifactIndexes = new()
            {
                [0] = 105,
                [1] = 106,
                [2] = 110,
                [3] = 111,
                [4] = 112,
                [5] = 115,
                [6] = 117,
            };

            Dictionary<int, int> objectIndexes = new()
            {
                [0] = artifactIndexes[randomIndex.Next(7)],
                [1] = 167,
                [2] = 168,
                [3] = 169,
                [4] = 170,
                [5] = 171,
                [6] = 172,
            };

            int objectIndex = objectIndexes[randomIndex.Next(7)];

            Throw throwObject;

            if (trashCollected == 8)
            {

                objectIndex = 517;

                throwObject = new(targetPlayer, trashVector * 64, objectIndex, 0);

                throwObject.objectInstance = new Ring(objectIndex.ToString());

            }
            else if (trashCollected == 16)
            {

                objectIndex = 519;

                throwObject = new(targetPlayer, trashVector * 64, objectIndex, 0);

                throwObject.objectInstance = new Ring( objectIndex.ToString());

            }
            else
            {

                throwObject = new(targetPlayer, trashVector * 64, objectIndex, 0);

            }

            throwObject.ThrowObject();

            targetPlayer.currentLocation.playSound("pullItemFromWater");

            bool targetDirection = targetPlayer.Tile.X >= trashVector.X;

            Utility.addSprinklesToLocation(targetLocation, (int)trashVector.X, (int)trashVector.Y, 2, 2, 1000, 200, new Color(0.8f, 1f, 0.8f, 0.75f));

            ModUtility.AnimateSplash(targetLocation, trashVector, targetDirection);

            trashCollected++;

        }


    }

}
