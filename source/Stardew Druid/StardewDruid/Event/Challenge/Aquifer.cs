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
using StardewDruid.Monster;
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

        public BossBat bossMonster;

        public List<Vector2> ladderPlacement;

        public Aquifer(Vector2 target, Rite rite, Quest quest)
            : base(target, rite, quest)
        {
            ladderPlacement = new List<Vector2>();
        }

        public override void EventTrigger()
        {

            challengeSpawn = new() { 99, };

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

                    Tile backTile = backLayer.PickTile(new Location((int)castVector.X * 64, (int)castVector.Y * 64), Game1.viewport.Size);

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

            Game1.addHUDMessage(new HUDMessage($"Stand your ground until the rite completes", "2"));

            SetTrack("tribal");

            Mod.instance.RegisterEvent(this, "active");

        }

        public override void MinutesLeft(int minutes)
        {
            Game1.addHUDMessage(new HUDMessage($"{minutes} minutes left until cleanup complete", "2"));
        }

        public override void RemoveMonsters()
        {

            if (bossMonster != null)
            {

                riteData.castLocation.characters.Remove(bossMonster);

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

                    Game1.addHUDMessage(new HUDMessage($"Try again to collect more trash", ""));

                    Mod.instance.ReassignQuest(questData.name);

                }
                else
                {

                    Game1.addHUDMessage(new HUDMessage($"Collected {trashCollected} pieces of trash!", ""));

                    List<string> NPCIndex = VillagerData.VillagerIndex("mountain");

                    Game1.addHUDMessage(new HUDMessage($"You have gained favour with the mountain residents and their friends", ""));

                    Mod.instance.CompleteQuest(questData.name);

                    if (!questData.name.Contains("Two"))
                    {

                        UpdateFriendship(NPCIndex);

                        Mod.instance.dialogue["Effigy"].specialDialogue["journey"] = new() { "I sense a change", "The rite disturbed the bats. ALL the bats." };

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
            if (ladderPlacement.Count > 0)
            {
                Layer layer = targetLocation.map.GetLayer("Buildings");
                int x = (int)ladderPlacement.First<Vector2>().X;
                int y = (int)ladderPlacement.First<Vector2>().Y;
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

            List<Vector2> rockFalls = new();

            riteData.castLevel = randomIndex.Next(2, 4);

            riteData.CastRockfall();

            riteData.CastEffect();

            if (activeCounter == 20)
            {
                StardewValley.Monsters.Monster theMonster = MonsterData.CreateMonster(11, new(30, 13), riteData.combatModifier);

                bossMonster = theMonster as BossBat;

                bossMonster.posturing = true;

                riteData.castLocation.characters.Add(bossMonster);

                bossMonster.update(Game1.currentGameTime, riteData.castLocation);

            }

            if (activeCounter <= 20)
            {
                return;
            }

            if (bossMonster.Health >= 1)
            {
                switch (activeCounter)
                {

                    case 22: bossMonster.showTextAboveHead("...farmer..."); break;

                    case 24: bossMonster.showTextAboveHead("...you tresspass..."); break;

                    case 26: bossMonster.showTextAboveHead("cheeep cheep"); break;

                    case 28: bossMonster.showTextAboveHead("...your kind..."); break;

                    case 30: bossMonster.showTextAboveHead("...defile waters..."); break;

                    case 32: bossMonster.showTextAboveHead("cheeep cheep"); break;

                    case 34: bossMonster.showTextAboveHead("She Who Screeches Over Seas", 3000); break;

                    case 37: bossMonster.showTextAboveHead("...is angry..."); break;

                    case 39: bossMonster.showTextAboveHead("CHEEEP"); bossMonster.posturing = false; bossMonster.focusedOnFarmers = true; break;

                    case 56:

                        bossMonster.showTextAboveHead("...rocks hurt...");

                        break;

                    case 57:

                        Cast.Weald.Rockfall rockFall = new(bossMonster.getTileLocation(), riteData);

                        rockFall.challengeCast = true;

                        rockFall.CastEffect();

                        break;

                    case 58:

                        bossMonster.showTextAboveHead("CHEEE--- aack");

                        break;

                    case 59:

                        bossMonster.takeDamage(999, 0, 0, false, 999, targetPlayer);

                        break;

                    default: break;

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

                throwObject.objectInstance = new Ring(objectIndex);

            }
            else if (trashCollected == 16)
            {

                objectIndex = 519;

                throwObject = new(targetPlayer, trashVector * 64, objectIndex, 0);

                throwObject.objectInstance = new Ring(objectIndex);

            }
            else
            {

                throwObject = new(targetPlayer, trashVector * 64, objectIndex, 0);

            }

            throwObject.ThrowObject();

            targetPlayer.currentLocation.playSound("pullItemFromWater");

            bool targetDirection = targetPlayer.getTileLocation().X >= trashVector.X;

            Utility.addSprinklesToLocation(targetLocation, (int)trashVector.X, (int)trashVector.Y, 2, 2, 1000, 200, new Color(0.8f, 1f, 0.8f, 0.75f));

            ModUtility.AnimateSplash(targetLocation, trashVector, targetDirection);

            trashCollected++;

        }

        public void RemoveLadders()
        {
            Layer layer = targetLocation.map.GetLayer("Buildings");
            for (int index1 = 0; index1 < layer.LayerHeight; ++index1)
            {
                for (int index2 = 0; index2 < layer.LayerWidth; ++index2)
                {
                    if (layer.Tiles[index2, index1] != null && layer.Tiles[index2, index1].TileIndex == 173)
                    {
                        layer.Tiles[index2, index1] = null;
                        Game1.player.TemporaryPassableTiles.Clear();
                        if (ladderPlacement.Count == 0)
                            ladderPlacement.Add(new Vector2(index2, index1));
                    }
                }
            }
        }

    }

}
