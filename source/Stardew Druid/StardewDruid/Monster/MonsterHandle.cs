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
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Cast;
using StardewDruid.Data;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.IO;
using static StardewDruid.Monster.MonsterHandle;

namespace StardewDruid.Monster
{

    public class SpawnHandle
    {

        public MonsterHandle.bosses boss;

        public Monster.Boss.temperment temperment;

        public Monster.Boss.difficulty difficulty;

        public SpawnHandle(MonsterHandle.bosses Boss, Monster.Boss.temperment Temperment, Monster.Boss.difficulty Difficulty)
        {
            boss = Boss;
            temperment = Temperment;
            difficulty = Difficulty;

        }

        public SpawnHandle(MonsterHandle.bosses Boss, int Temperment = 4, int Difficulty = 0)
        {
            boss = Boss;
            temperment = (Monster.Boss.temperment)Temperment;
            difficulty = (Monster.Boss.difficulty)Difficulty;

        }

    }

    public class MonsterHandle
    {

        public List<StardewValley.Monsters.Monster> monsterSpawns = new();

        public int monstersLeft;

        public enum bosses
        {

            batwing,
            blobfiend,
            darkbrute,
            darkshooter,
            spectre,
            phantom,
            demonki,
            dino,
            dragon,
            gargoyle,
            goblin,
            reaper,
            rogue,
            scavenger,
            shadowfox,

        }

        public Dictionary<int,List<SpawnHandle>> spawnSchedule = new();

        public int spawnCounter;

        public Vector2 spawnWithin;

        public Vector2 spawnRange;

        public int spawnTotal;

        public GameLocation spawnLocation;

        public int spawnCombat;

        public Random randomIndex;

        public bool spawnWater;

        public bool spawnVoid;

        public MonsterHandle(Vector2 target, GameLocation location)
        {

            BaseTarget(target);

            spawnLocation = location;

            spawnCombat = Mod.instance.CombatDifficulty();

            spawnWater = false;

        }

        public void BaseTarget(Vector2 target)
        {

            spawnWithin = target - new Vector2(4, 4);

            spawnRange = new Vector2(9, 9);

        }

        public void TargetToPlayer(Vector2 target,bool precision = false)
        {

            Vector2 playerVector = Game1.player.Tile;

            if (precision)
            {

                spawnWithin = playerVector - new Vector2(1,1);

                spawnRange = new Vector2(2, 2);

                return;

            }

            spawnWithin = playerVector + ((target - playerVector) / 2) - new Vector2(2, 2);

            spawnRange = new Vector2(5, 5);

        }

        public void ShutDown()
        {

            SpawnCheck();

            for (int i = monsterSpawns.Count - 1; i >= 0; i--)
            {

                Mod.instance.iconData.AnimateQuickWarp(spawnLocation, monsterSpawns[i].Position, true);

                monsterSpawns[i].Health = 0;

                spawnLocation.characters.Remove(monsterSpawns[i]);

                monsterSpawns.RemoveAt(i);

            }

            return;

        }

        public void SpawnCheck()
        {

            for (int i = monsterSpawns.Count - 1; i >= 0; i--)
            {

                if (!ModUtility.MonsterVitals(monsterSpawns[i],spawnLocation))
                {
                    
                    monsterSpawns.RemoveAt(i);

                }

            }

            monstersLeft = monsterSpawns.Count;

        }

        public int SpawnInterval()
        {
            
            spawnCounter++;

            int spawnAmount = 0;

            Vector2 spawnVector;

            if (!spawnSchedule.ContainsKey(spawnCounter))
            {

                return 0;

            }

            for (int i = 0; i < spawnSchedule[spawnCounter].Count; i++)
            {

                for(int j = 0; j < 4; j++)
                {

                    spawnVector = SpawnVector();

                    if (spawnVector.X > 0)
                    {

                        SpawnGround(spawnVector, spawnSchedule[spawnCounter][i]);

                        spawnAmount++;

                        break;

                    }

                }

            }

            return spawnAmount;

        }

        public Vector2 SpawnVector(int spawnLimit = 4, int fromX = -1, int fromY = -1, int spawnX = -1, int spawnY = -1)
        {

            Vector2 spawnVector = new(-1);

            Vector2 playerVector = Game1.player.Tile;

            int spawnAttempt = 0;
            
            Vector2 fromVector = new(fromX, fromY); 

            if (fromX == -1)
            {

                fromVector = spawnWithin;

            }

            if (spawnX == -1)
            {

                spawnX = (int)spawnRange.X;

                spawnY = (int)spawnRange.Y;

            }

            while (spawnAttempt++ < spawnLimit)
            {

                int offsetX = Mod.instance.randomIndex.Next(spawnX);

                int offsetY = Mod.instance.randomIndex.Next(spawnY);

                Vector2 offsetVector = new(offsetX, offsetY);

                spawnVector = fromVector + offsetVector;

                if (Math.Abs(spawnVector.X - playerVector.X) <= 1 && Math.Abs(spawnVector.Y - playerVector.Y) <= 1)
                {
                    continue;
                }

                string groundCheck = ModUtility.GroundCheck(spawnLocation, spawnVector, true);

                if (groundCheck == "void" && !spawnVoid)
                {

                    continue;

                }
                else if (groundCheck == "water" && !spawnWater)
                {

                    continue;

                }
                else if (groundCheck != "ground")
                {
                    
                    continue;
               
                }
                else if (ModUtility.NeighbourCheck(spawnLocation, spawnVector, 0, 0).Count > 0)
                {
                    
                    continue;
                
                }

                return spawnVector;

            }

            return spawnVector;

        }

        public StardewValley.Monsters.Monster SpawnGround(Vector2 spawnVector, SpawnHandle spawnProfile)
        {

            StardewValley.Monsters.Monster theMonster = CreateMonster(spawnProfile.boss, spawnVector, spawnCombat, spawnProfile.difficulty, spawnProfile.temperment);

            monsterSpawns.Add(theMonster);

            spawnLocation.characters.Add(theMonster);

            theMonster.currentLocation = spawnLocation;

            theMonster.update(Game1.currentGameTime, spawnLocation);

            spawnTotal++;

            Mod.instance.iconData.AnimateQuickWarp(spawnLocation, spawnVector * 64 - new Vector2(0, 32));

            return theMonster;

        }

        public void SpawnImport(StardewValley.Monsters.Monster theMonster)
        {

            monsterSpawns.Add(theMonster);

            spawnLocation.characters.Add(theMonster);

            theMonster.currentLocation = spawnLocation;

            theMonster.update(Game1.currentGameTime, spawnLocation);

            spawnTotal++;

            Mod.instance.iconData.AnimateQuickWarp(spawnLocation, theMonster.Position - new Vector2(0, 32));

        }

        public static StardewDruid.Monster.Boss CreateMonster(
            bosses spawnMob, 
            Vector2 spawnVector, 
            int combatModifier = -1, 
            Boss.difficulty difficulty = Boss.difficulty.basic, 
            Boss.temperment temperment = Boss.temperment.cautious
        )
        {

            if (combatModifier == -1)
            {

                combatModifier = Mod.instance.CombatDifficulty();

            }

            System.Random randomise = new();

            StardewDruid.Monster.Boss theMonster;

            switch (spawnMob)
            {

                default:
                case bosses.batwing:

                    theMonster = new Batwing(spawnVector, combatModifier);

                    break;

                case bosses.blobfiend:

                    theMonster = new Blobfiend(spawnVector, combatModifier);

                    break;

                case bosses.darkbrute:

                    theMonster = new DarkBrute(spawnVector, combatModifier);

                    break;

                case bosses.darkshooter:

                    theMonster = new DarkShooter(spawnVector, combatModifier);

                    break;

                case bosses.spectre:

                    theMonster = new Spectre(spawnVector, combatModifier);

                    break;

                case bosses.phantom:

                    theMonster = new Phantom(spawnVector, combatModifier);

                    break;

                    /*case bosses.gargoyle:

                        theMonster = new Gargoyle(spawnVector, combatModifier);

                        break;

                    case bosses.demonki:

                        theMonster = new Demonki(spawnVector, combatModifier);

                        break;

                    case bosses.dino:

                        theMonster = new Dino(spawnVector, combatModifier);

                        break;

                    case bosses.scavenger:

                        theMonster = new Scavenger(spawnVector, combatModifier);

                        break;

                    case bosses.shadowfox:

                        theMonster = new Shadowfox(spawnVector, combatModifier);

                        break;

                    case bosses.rogue:

                        theMonster = new Rogue(spawnVector, combatModifier);

                        break;

                    case bosses.goblin:

                        theMonster = new Goblin(spawnVector, combatModifier);

                        break;

                    case bosses.dragon:

                        string dragon = "Purple";

                        switch (randomise.Next(4))
                        {

                            case 1:

                                dragon = "Red"; break;

                            case 2:

                                dragon = "Blue"; break;

                            case 3:

                                dragon = "Black"; break;

                        }

                        theMonster = new Dragon(spawnVector, combatModifier, dragon + "Dragon");

                        break;*/

            }

            theMonster.SetMode((int)difficulty);

            switch (temperment)
            {

                case Monster.Boss.temperment.random:

                    theMonster.RandomTemperment();

                    break;

                default:

                    theMonster.tempermentActive = temperment;

                    break;

            }

            return theMonster;

        }

        public static Texture2D MonsterTexture(string characterName)
        {

            Texture2D characterTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", characterName + ".png"));

            return characterTexture;

        }

    }

}
