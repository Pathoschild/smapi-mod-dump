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
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace StardewDruid.Monster
{
    public class MonsterHandle
    {

        private Rite riteData;

        public List<StardewValley.Monsters.Monster> monsterSpawns;

        public List<int> spawnIndex;

        public List<int> specialIndex;

        public int spawnFrequency;

        public int spawnAmplitude;

        public int spawnCounter;

        public int spawnSpecial;

        public int specialCounter;

        public Vector2 spawnWithin;

        public Vector2 spawnRange;

        public bool firstSpawn;

        public List<MonsterSpawn> spawnHandles;

        public int spawnTotal;

        public MonsterHandle(Vector2 target, Rite rite)
        {

            BaseTarget(target);

            spawnFrequency = 1;

            spawnAmplitude = 1;

            monsterSpawns = new();

            spawnHandles = new();

            spawnIndex = new() { 99, };

            riteData = rite;

        }

        public void BaseTarget(Vector2 target)
        {

            spawnWithin = target - new Vector2(4, 4);

            spawnRange = new Vector2(9, 9);

        }

        public void TargetToPlayer(Vector2 target)
        {

            Vector2 playerVector = riteData.caster.getTileLocation();

            spawnWithin = playerVector + ((target - playerVector) / 2) - new Vector2(2, 2);

            spawnRange = new Vector2(5, 5);

        }

        public int ShutDown()
        {

            SpawnCheck();

            int monstersLeft = monsterSpawns.Count;

            for (int i = monsterSpawns.Count - 1; i >= 0; i--)
            {

                ModUtility.AnimateQuickWarp(riteData.castLocation, monsterSpawns[i].Position, "Void");

                riteData.castLocation.characters.Remove(monsterSpawns[i]);

                monsterSpawns.RemoveAt(i);

            }

            spawnHandles = new();

            return monstersLeft;

        }

        public void SpawnCheck()
        {

            for (int i = spawnHandles.Count - 1; i >= 0; --i)
            {

                if (spawnHandles[i].spawnComplete)
                {

                    monsterSpawns.Add(spawnHandles[i].targetMonster);

                    spawnHandles.RemoveAt(i);

                }

            }

            for (int i = monsterSpawns.Count - 1; i >= 0; i--)
            {

                if (monsterSpawns[i].Health <= 0 && (monsterSpawns[i].currentLocation == null || !monsterSpawns[i].currentLocation.characters.Contains(monsterSpawns[i])))
                {
                    
                    monsterSpawns.RemoveAt(i);

                }

            }

        }

        public int SpawnInterval()
        {

            spawnCounter++;

            specialCounter++;

            if (spawnFrequency >= 3 && spawnCounter == 2 && !firstSpawn)
            {
                firstSpawn = true;

            }
            else if (spawnFrequency > spawnCounter)
            {

                return 0;

            }

            SpawnCheck();

            spawnCounter = 0;

            int spawnAmount = 0;

            Vector2 spawnVector;

            for (int i = 0; i < spawnAmplitude; i++)
            {

                spawnVector = SpawnVector();

                if (spawnVector != new Vector2(-1))
                {

                    SpawnGround(spawnVector);

                    spawnAmount++;

                    spawnTotal++;

                }

            }

            if (specialCounter > 30 && spawnSpecial > 0)
            {

                spawnVector = SpawnVector();

                if (spawnVector != new Vector2(-1))
                {

                    SpawnGround(spawnVector, true);

                    spawnAmount++;

                    spawnSpecial--;

                    specialCounter = 0;

                }

            }

            return spawnAmount;

        }

        public Vector2 SpawnVector(int spawnLimit = 4, int fromX = -1, int fromY = -1, int spawnX = -1, int spawnY = -1)
        {

            Vector2 spawnVector = new(-1);

            Vector2 playerVector = riteData.caster.getTileLocation();

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

                int offsetX = riteData.randomIndex.Next(spawnX);

                int offsetY = riteData.randomIndex.Next(spawnY);

                Vector2 offsetVector = new(offsetX, offsetY);

                spawnVector = fromVector + offsetVector;

                if (Math.Abs(spawnVector.X - playerVector.X) <= 1 && Math.Abs(spawnVector.Y - playerVector.Y) <= 1)
                {
                    continue;
                }

                if (ModUtility.GroundCheck(riteData.castLocation, spawnVector) != "ground")
                {
                    continue;
                }

                if (ModUtility.NeighbourCheck(riteData.castLocation, spawnVector, 0).Count > 0)
                {
                    continue;
                }

                return spawnVector;

            }

            return spawnVector;

        }

        public StardewValley.Monsters.Monster SpawnGround(Vector2 spawnVector, bool special = false)
        {

            int spawnMob;

            if (special)
            {
                spawnMob = specialIndex[riteData.randomIndex.Next(specialIndex.Count)];

            }
            else
            {
                spawnMob = spawnIndex[riteData.randomIndex.Next(spawnIndex.Count)];

            }

            StardewValley.Monsters.Monster theMonster = MonsterData.CreateMonster(spawnMob, spawnVector, riteData.combatModifier, Mod.instance.PartyHats());

            monsterSpawns.Add(theMonster);

            spawnTotal++;

            MonsterSpawn monsterSpawn = new(riteData.castLocation, theMonster);

            monsterSpawn.InitiateMonster(150);

            spawnHandles.Add(monsterSpawn);

            ModUtility.AnimateQuickWarp(riteData.castLocation, spawnVector * 64 - new Vector2(0, 32), "Void");

            // ------------------------------ monster

            return theMonster;

        }

        public StardewValley.Monsters.Monster SpawnTerrain(Vector2 spawnVector, Vector2 terrainVector, bool splash)
        {

            int spawnMob = spawnIndex[riteData.randomIndex.Next(spawnIndex.Count)];

            StardewValley.Monsters.Monster theMonster = MonsterData.CreateMonster(spawnMob, spawnVector, riteData.combatModifier, Mod.instance.PartyHats());

            Vector2 fromPosition = new(terrainVector.X * 64, terrainVector.Y * 64);

            Vector2 toPosition = new(spawnVector.X * 64, spawnVector.Y * 64);

            float animationInterval = 125f;

            float motionX = (toPosition.X - fromPosition.X) / 1000;

            float compensate = 0.555f;

            float motionY = (toPosition.Y - fromPosition.Y) / 1000 - compensate;

            float animationSort = float.Parse("0.0" + terrainVector.X.ToString() + terrainVector.Y.ToString());

            Color monsterColor = Color.White;

            if (theMonster is GreenSlime)
            {
                GreenSlime slimeMonster = (GreenSlime)theMonster;

                monsterColor = slimeMonster.color.Value;
            }

            MonsterSpawn monsterSpawn = new(riteData.castLocation, theMonster);

            monsterSpawn.InitiateMonster(1000);

            spawnHandles.Add(monsterSpawn);

            // ----------------------------- animation

            string textureName = theMonster.Sprite.textureName.Value;

            Rectangle targetRectangle = theMonster.Sprite.SourceRect;

            TemporaryAnimatedSprite monsterSprite = new(textureName, targetRectangle, animationInterval, 4, 2, fromPosition, flicker: false, flipped: false, animationSort, 0f, monsterColor, 4f, 0f, 0f, 0f)
            {

                motion = new Vector2(motionX, motionY),

                acceleration = new Vector2(0f, 0.001f),

                timeBasedMotion = true,

            };

            riteData.castLocation.temporarySprites.Add(monsterSprite);

            ModUtility.AnimateQuickWarp(riteData.castLocation, fromPosition, "Void");

            if (splash)
            {

                ModUtility.AnimateSplash(riteData.castLocation, terrainVector, true);

            }

            // ------------------------------ monster

            return theMonster;

        }

    }

}
