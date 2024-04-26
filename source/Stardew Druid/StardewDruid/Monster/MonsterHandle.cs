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
using StardewDruid.Monster.Boss;
using StardewDruid.Monster.Template;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.IO;

namespace StardewDruid.Monster
{
    public class MonsterHandle
    {

        //private Rite riteData;

        public List<StardewValley.Monsters.Monster> monsterSpawns;

        public int monstersLeft;

        public List<int> spawnIndex;

        public int spawnFrequency;

        public int spawnAmplitude;

        public int spawnCounter;

        public bool spawnChampion;

        public int championCounter;

        public int championInterval;

        public int championAmount;

        public int championLimit;

        public Vector2 spawnWithin;

        public Vector2 spawnRange;

        //public List<MonsterSpawn> spawnHandles;

        public int spawnTotal;

        public GameLocation spawnLocation;

        public int spawnCombat;

        public Random randomIndex;

        public bool spawnWater;

        public MonsterHandle(Vector2 target, GameLocation location)
        {

            BaseTarget(target);

            spawnFrequency = 1;

            spawnAmplitude = 1;

            monsterSpawns = new();

            //spawnHandles = new();

            spawnIndex = new() { 99, };

            spawnLocation = location;

            randomIndex = new();

            spawnCombat = Mod.instance.CombatDifficulty();

            spawnWater = false;

            //riteData = rite;

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

            //ModUtility.LogMonsters(monsterSpawns);

            for (int i = monsterSpawns.Count - 1; i >= 0; i--)
            {

                ModUtility.AnimateQuickWarp(spawnLocation, monsterSpawns[i].Position, true);

                monsterSpawns[i].Health = 0;

                spawnLocation.characters.Remove(monsterSpawns[i]);

                monsterSpawns.RemoveAt(i);

            }

            //spawnHandles = new();

            return;

        }

        public void SpawnCheck()
        {

            //monstersLeft = monsterSpawns.Count;

            /*for (int i = spawnHandles.Count - 1; i >= 0; --i)
            {

                if (spawnHandles[i].spawnComplete)
                {

                    monsterSpawns.Add(spawnHandles[i].targetMonster);

                    spawnHandles.RemoveAt(i);

                }

            }*/

            for (int i = monsterSpawns.Count - 1; i >= 0; i--)
            {

                if (!ModUtility.MonsterVitals(monsterSpawns[i],spawnLocation))
                {
                    
                    monsterSpawns.RemoveAt(i);

                    //monstersLeft--;

                }

            }

            monstersLeft = monsterSpawns.Count;

        }

        public int SpawnInterval()
        {
            
            spawnCounter--;

            if (spawnCounter > 0)
            {

                return 0;

            }

            spawnCounter = spawnFrequency;

            if (championInterval != 0)
            {

                if (championAmount == championLimit)
                {

                    championInterval = 0;

                }

                championCounter++;

                if(championCounter == championInterval)
                {

                    spawnChampion = true;

                    championAmount++;

                    championCounter = 0;

                }

            }

            int spawnAmount = 0;

            Vector2 spawnVector;

            for (int i = 0; i < spawnAmplitude; i++)
            {

                spawnVector = SpawnVector();

                if (spawnVector.X >= 0)
                {

                    SpawnGround(spawnVector);

                    spawnAmount++;

                    spawnTotal++;

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

                int offsetX = randomIndex.Next(spawnX);

                int offsetY = randomIndex.Next(spawnY);

                Vector2 offsetVector = new(offsetX, offsetY);

                spawnVector = fromVector + offsetVector;

                if (Math.Abs(spawnVector.X - playerVector.X) <= 1 && Math.Abs(spawnVector.Y - playerVector.Y) <= 1)
                {
                    continue;
                }

                string groundCheck = ModUtility.GroundCheck(spawnLocation, spawnVector, true);

                if (groundCheck == "water" && !spawnWater)
                {

                    continue;

                }
                else if (groundCheck != "ground")
                {
                    continue;
                }

                if (ModUtility.NeighbourCheck(spawnLocation, spawnVector, 0, 0).Count > 0)
                {
                    continue;
                }

                return spawnVector;

            }

            return spawnVector;

        }

        public StardewValley.Monsters.Monster SpawnGround(Vector2 spawnVector)
        {

            int spawnMob = spawnIndex[randomIndex.Next(spawnIndex.Count)];

            StardewValley.Monsters.Monster theMonster = CreateMonster(spawnMob, spawnVector, spawnCombat, spawnChampion);

            if (spawnChampion) { spawnChampion = false; }

            monsterSpawns.Add(theMonster);

            spawnLocation.characters.Add(theMonster);

            theMonster.currentLocation = spawnLocation;

            theMonster.update(Game1.currentGameTime, spawnLocation);

            spawnTotal++;

            ModUtility.AnimateQuickWarp(spawnLocation, spawnVector * 64 - new Vector2(0, 32));

            return theMonster;

        }

        public void SpawnImport(StardewValley.Monsters.Monster theMonster)
        {

            monsterSpawns.Add(theMonster);

            spawnLocation.characters.Add(theMonster);

            theMonster.currentLocation = spawnLocation;

            theMonster.update(Game1.currentGameTime, spawnLocation);

            spawnTotal++;

            ModUtility.AnimateQuickWarp(spawnLocation, theMonster.Position - new Vector2(0, 32));

        }

        public static StardewValley.Monsters.Monster CreateMonster(int spawnMob, Vector2 spawnVector, int combatModifier = -1, bool champion = false)
        {

            if (combatModifier == -1)
            {

                combatModifier = Mod.instance.CombatDifficulty();

            }

            System.Random randomise = new();

            StardewValley.Monsters.Monster theMonster;

            switch (spawnMob)
            {

                default:
                case 0: // Bat

                    if (champion)
                    {

                        theMonster = new BigBat(spawnVector, combatModifier);

                        break;

                    }

                    theMonster = new StardewDruid.Monster.Template.Bat(spawnVector, combatModifier);

                    break;

                case 1: // Shadow Brute

                    if (champion)
                    {

                        theMonster = new StardewDruid.Monster.Template.Shooter(spawnVector, combatModifier);

                        break;

                    }

                    theMonster = new Shadow(spawnVector, combatModifier);

                    break;

                case 2: // Green Slime

                    if (champion)
                    {

                        if (randomise.Next(2) == 0)
                        {


                            theMonster = new BlobSlime(spawnVector, combatModifier);

                        }
                        else
                        {

                            theMonster = new StardewDruid.Monster.Template.BigSlime(spawnVector, combatModifier);

                        }

                        break;

                    }

                    theMonster = new Slime(spawnVector, combatModifier);

                    break;

                case 3: // Skeleton

                    theMonster = new StardewDruid.Monster.Template.Skeleton(spawnVector, combatModifier);

                    break;

                case 4: // Golem

                    theMonster = new Golem(spawnVector, combatModifier);

                    break;

                case 5: // DustSpirit

                    theMonster = new Spirit(spawnVector, combatModifier);

                    break;

                case 6: // Gargoyle

                    theMonster = new Gargoyle(spawnVector, combatModifier);

                    if (!champion)
                    {

                        (theMonster as Gargoyle).SetMode(0);

                    }

                    (theMonster as StardewDruid.Monster.Boss.Boss).RandomTemperment();

                    /*string scheme = randomise.Next(2) == 0 ? "Solar" : "Void";

                    (theMonster as Gargoyle).netScheme.Set(scheme);

                    (theMonster as Gargoyle).SchemeLoad();*/

                    break;

                case 7: // Demonki

                    theMonster = new Demonki(spawnVector, combatModifier);

                    if (!champion)
                    {

                        (theMonster as Demonki).SetMode(0);

                    }

                    (theMonster as StardewDruid.Monster.Boss.Boss).RandomTemperment();

                    break;

                case 8:

                    theMonster = new Dino(spawnVector, combatModifier);

                    (theMonster as Dino).SetMode(0);

                    if (champion)
                    {

                        (theMonster as Dino).SetMode(2);

                    }

                    (theMonster as StardewDruid.Monster.Boss.Boss).RandomTemperment();

                    break;

                case 9:

                    if (randomise.Next(2) == 0)
                    {

                        theMonster = new Scavenger(spawnVector, combatModifier);

                    }
                    else
                    {

                        theMonster = new Shadowfox(spawnVector, combatModifier);

                    }

                    (theMonster as StardewDruid.Monster.Boss.Boss).SetMode(1);

                    (theMonster as StardewDruid.Monster.Boss.Boss).RandomTemperment();

                    break;

                case 10:

                    if (randomise.Next(2) == 0)
                    {

                        theMonster = new Rogue(spawnVector, combatModifier);

                    }
                    else
                    {

                        theMonster = new Goblin(spawnVector, combatModifier);

                    }

                    (theMonster as StardewDruid.Monster.Boss.Boss).SetMode(1);

                    (theMonster as StardewDruid.Monster.Boss.Boss).RandomTemperment();

                    break;

                case 11:

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

                    (theMonster as Dragon).SetMode(1);

                    (theMonster as StardewDruid.Monster.Boss.Boss).RandomTemperment();

                    break;

            }

            return theMonster;

        }

        public static bool BossMonster(StardewValley.Monsters.Monster monster)
        {

            if (monster is StardewDruid.Monster.Boss.Boss)
            {

                return true;

            }

            List<System.Type> customMonsters = new()
            {
                typeof(BigBat),
                typeof(StardewDruid.Monster.Template.Shooter),
                typeof(StardewDruid.Monster.Template.BigSlime),

            };

            if (customMonsters.Contains(monster.GetType()))
            {

                return true;

            }

            return false;

        }

        public static Texture2D MonsterTexture(string characterName)
        {

            if (characterName == "Dinosaur")
            {

                return Game1.content.Load<Texture2D>("Characters\\Monsters\\Pepper Rex");

            }

            Texture2D characterTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", characterName + ".png"));

            return characterTexture;

        }

    }

}
