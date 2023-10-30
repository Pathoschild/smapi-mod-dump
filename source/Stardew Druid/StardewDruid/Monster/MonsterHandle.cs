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
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using xTile.Layers;
using xTile.Tiles;
using static StardewValley.Minigames.TargetGame;

namespace StardewDruid.Monster
{
    public class MonsterHandle
    {

        private Mod mod;

        private Rite riteData;

        public List<StardewValley.Monsters.Monster> monsterSpawns;

        public List<int> spawnIndex;

        public List<int> specialIndex;

        public int spawnFrequency;

        public int spawnAmplitude;

        public int spawnCounter;

        public int spawnSpecial;

        public int specialCounter;

        public Queue<StardewValley.Monsters.Monster> spawnQueue;

        public Vector2 spawnWithin;

        public Vector2 spawnRange;

        public bool firstSpawn;

        public MonsterHandle(Mod Mod, Vector2 target, Rite rite)
        {

            mod = Mod;

            BaseTarget(target);

            spawnQueue = new();

            spawnFrequency = 1;

            spawnAmplitude = 1;

            monsterSpawns = new();

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

            spawnWithin = playerVector + ((target - playerVector) / 2) - new Vector2(2,2);

            spawnRange = new Vector2(5, 5);

        }

        public int ShutDown()
        {

            SpawnCheck();

            int monstersLeft = monsterSpawns.Count;

            for (int i = monsterSpawns.Count - 1; i >= 0; i--)
            {

                TemporaryAnimatedSprite smallAnimation = new(5, monsterSpawns[i].getTileLocation() * 64f, Color.Purple * 0.75f, 8, flipped: false, 75f)
                {
                    scale = 0.75f,
                };

                riteData.castLocation.temporarySprites.Add(smallAnimation);

                riteData.castLocation.characters.Remove(monsterSpawns[i]);

                monsterSpawns.RemoveAt(i);

            }

            spawnQueue = new();

            return monstersLeft;

        }

        public void SpawnCheck()
        {

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

                if(spawnVector != new Vector2(-1))
                {

                    SpawnGround(spawnVector);

                    spawnAmount++;

                }

            }

            if(specialCounter > 30 && spawnSpecial > 0)
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

        public Vector2 SpawnVector()
        {

            Vector2 spawnVector = new(-1);

            Layer buildingLayer = riteData.castLocation.Map.GetLayer("Buildings");

            Layer backLayer = riteData.castLocation.Map.GetLayer("Back");

            Tile buildingTile;

            Tile backTile;

            Vector2 playerVector = riteData.caster.getTileLocation();

            int spawnAttempt = 0;

            while (spawnAttempt++ < 4)
            {

                int offsetX = riteData.randomIndex.Next((int)spawnRange.X);

                int offsetY = riteData.randomIndex.Next((int)spawnRange.Y);

                Vector2 offsetVector = new(offsetX, offsetY);

                spawnVector = spawnWithin + offsetVector;

                if (Math.Abs(spawnVector.X - playerVector.X) <= 1 && Math.Abs(spawnVector.Y - playerVector.Y) <= 1)
                {
                    continue;
                }

                backTile = backLayer.PickTile(new xTile.Dimensions.Location((int)spawnVector.X * 64, (int)spawnVector.Y * 64), Game1.viewport.Size);
                
                if (backTile == null)
                {
                    continue;
                }

                if (backTile.TileIndexProperties.TryGetValue("Water", out _))
                {
                    continue;
                }

                buildingTile = buildingLayer.PickTile(new xTile.Dimensions.Location((int)spawnVector.X * 64, (int)spawnVector.Y * 64), Game1.viewport.Size);

                if (buildingTile != null)
                {
                    continue;
                }

                foreach (ResourceClump resourceClump in riteData.castLocation.resourceClumps)
                {
                    if (resourceClump.occupiesTile((int)spawnVector.X, (int)spawnVector.Y))
                    {
                        continue;
                    }
                }

                if(riteData.castLocation.objects.TryGetValue(spawnVector, out var objectValue))
                {
                    continue;
                }

                Rectangle spawnRectangle = new((int)spawnVector.X * 64 + 1, (int)spawnVector.Y * 64 + 1, 62, 62);

                for (int i = 0; i < riteData.castLocation.characters.Count; i++)
                {
                    if (riteData.castLocation.characters[i] != null && riteData.castLocation.characters[i].GetBoundingBox().Intersects(spawnRectangle))
                    {
                        continue;
                    }
                }


                if (riteData.castLocation.terrainFeatures.TryGetValue(spawnVector, out var terrainValue))
                {
                    if(terrainValue is not StardewValley.TerrainFeatures.Grass)
                    {
                        continue;
                    }

                }

                if (riteData.castLocation.isBehindBush(spawnVector))
                {

                    continue;

                }

                foreach (Furniture item in riteData.castLocation.furniture)
                {
                    if (item.getBoundingBox(item.TileLocation).Contains(spawnRectangle.Center))
                    {
                        continue;
                    }

                }

                return spawnVector;

            }
  
            return spawnVector;

        }

        public void SpawnGround(Vector2 spawnVector, bool special = false)
        {

            int spawnMob;

            if (special)
            {
                spawnMob = specialIndex[riteData.randomIndex.Next(specialIndex.Count)];
                //mod.Log($"Special: {spawnMob}");
            }
            else
            {
                spawnMob = spawnIndex[riteData.randomIndex.Next(spawnIndex.Count)];
                //mod.Log($"Spawn: {spawnMob}");
            }

            StardewValley.Monsters.Monster theMonster = MonsterData.CreateMonster(spawnMob, spawnVector, riteData.combatModifier, mod.PartyHats());

            spawnQueue.Enqueue(theMonster);

            TemporaryAnimatedSprite smallAnimation = new(5, spawnVector*64, Color.Purple * 0.75f, 8, flipped: false, 75f)
            {
                scale = 0.75f,
            };

            riteData.castLocation.temporarySprites.Add(smallAnimation);

            DelayedAction.functionAfterDelay(ManifestMonster, 150);

        }

        public void SpawnTerrain(Vector2 spawnVector, Vector2 terrainVector, bool splash)
        {

            int spawnMob = spawnIndex[riteData.randomIndex.Next(spawnIndex.Count)];

            StardewValley.Monsters.Monster theMonster = MonsterData.CreateMonster(spawnMob, spawnVector, riteData.combatModifier, mod.PartyHats());

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

            string textureName = theMonster.Sprite.textureName.Value;

            Rectangle targetRectangle = theMonster.Sprite.SourceRect;

            TemporaryAnimatedSprite monsterSprite = new(textureName, targetRectangle, animationInterval, 4, 2, fromPosition, flicker: false, flipped: false, animationSort, 0f, monsterColor, 4f, 0f, 0f, 0f)
            {

                motion = new Vector2(motionX, motionY),

                acceleration = new Vector2(0f, 0.001f),

                timeBasedMotion = true,

            };

            riteData.castLocation.temporarySprites.Add(monsterSprite);

            TemporaryAnimatedSprite smallAnimation = new(5, fromPosition, Color.Purple * 0.75f, 8, flipped: false, 75f)
            {
                scale = 0.75f,
            };

            riteData.castLocation.temporarySprites.Add(smallAnimation);

            spawnQueue.Enqueue(theMonster);

            if (splash)
            {

                ModUtility.AnimateSplash(riteData.castLocation, terrainVector, true);

            }

            DelayedAction.functionAfterDelay(ManifestMonster, 1000);

        }

        public void ManifestMonster()
        {

            if (spawnQueue.Count > 0)
            {

                StardewValley.Monsters.Monster theMonster = spawnQueue.Dequeue();

                riteData.castLocation.characters.Add(theMonster);

                theMonster.update(Game1.currentGameTime, riteData.castLocation);

                monsterSpawns.Add(theMonster);

            }

        }

    }

}
