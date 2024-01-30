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
using StardewDruid.Dialogue;
using StardewDruid.Event.World;
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;


namespace StardewDruid.Cast
{
    public class Rite
    {

        public Dictionary<string, bool> spawnIndex;

        public string castType;

        public bool castBuffs;

        public int castLevel;

        public Vector2 castVector;

        public StardewValley.Farmer caster;

        public StardewValley.GameLocation castLocation;

        public int direction;

        //public int castDamage;

        //public int combatModifier;
       // public int difficulty;

        public Dictionary<string, int> castTask;

        public Random randomIndex;

        public Dictionary<int, int> castSource;

        public Dictionary<Vector2, Cast.CastHandle> effectCasts;

        public int moveCheck;

        public Rite()
        {

            caster = Game1.player;

            castLocation = caster.currentLocation;

            randomIndex = new();

            spawnIndex = Map.SpawnData.SpawnIndex(castLocation);

            if (spawnIndex.Count == 0 && Mod.instance.eventRegister.ContainsKey("active"))
            {

                spawnIndex = SpawnData.SpawnTemplate();

            }

            effectCasts = new();

            castSource = new(0);

        }

        //public void CastDamage()
        //{

            //castDamage = Mod.instance.DamageLevel();

            //combatModifier = Mod.instance.CombatModifier();

            //difficulty = Mod.instance.CombatModifier();

        //}

        public static string GetSlotBlessing()
        {
            string slotBlessing = "none";

            int num = Game1.player.CurrentToolIndex;

            if (Game1.player.CurrentToolIndex == 999 && Mod.instance.eventRegister.ContainsKey("transform"))
            {
                num = (Mod.instance.eventRegister["transform"] as Transform).toolIndex;

            }

            switch (num % 12)
            {
                case 0:
                    slotBlessing = "weald";
                    break;
                case 1:
                    slotBlessing = "mists";
                    break;
                case 2:
                    slotBlessing = "stars";
                    break;
                case 3:
                    slotBlessing = "fates";
                    break;
                case 4:
                    slotBlessing = "ether";
                    break;
            }

            return slotBlessing;

        }

        public void CastVector()
        {

            switch (castType)
            {

                case "mists":
                case "fates":

                    if (castLevel % 4 == 0)
                    {

                        List<int> targetList = GetTargetCursor(Game1.player.getTileLocation(), Game1.player.FacingDirection, 5);

                        direction = targetList[0];

                        castVector = new(targetList[1], targetList[2]);

                    }

                    break;

                default: // earth / stars / fates

                    direction = Game1.player.FacingDirection;

                    castVector = Game1.player.getTileLocation();

                    break;

            }

        }

        public static List<int> GetTargetCursor(Vector2 vector, int direction, int distance = 5, int threshhold = 1)
        {

            Point mousePoint = Game1.getMousePosition();

            if (mousePoint.Equals(new(0)))
            {
                return GetTargetDirectional(vector, direction, distance);

            }

            Vector2 playerPosition = Game1.player.Position;

            Vector2 viewPortPosition = Game1.viewportPositionLerp;

            Vector2 mousePosition = new(mousePoint.X + viewPortPosition.X, mousePoint.Y + viewPortPosition.Y);

            float vectorDistance = Vector2.Distance(playerPosition, mousePosition);

            if (vectorDistance <= (threshhold * 64) + 32)
            {

                return GetTargetDirectional(vector, direction, distance);

            }

            Vector2 macroVector = mousePosition - playerPosition;

            int vectorLimit = (distance * 64) + 32;

            if (vectorDistance > vectorLimit)
            {

                float adjustmentRatio = vectorLimit / vectorDistance;

                macroVector *= adjustmentRatio;

            }

            int microX = Convert.ToInt32(macroVector.X / 64);

            int microY = Convert.ToInt32(macroVector.Y / 64);

            int newDirection;

            if (Math.Abs(microY) < Math.Abs(microX))
            {
                if (microX > 0) // right
                {
                    newDirection = 1;
                }
                else // left
                {
                    newDirection = 3;

                }
            }
            else
            {
                if (microY > 0) // down
                {
                    newDirection = 2;
                }
                else // up
                {
                    newDirection = 0;

                }
            }

            List<int> targetList = new()
            {
                newDirection,
                microX + (int)vector.X,
                microY + (int)vector.Y
            };

            return targetList;

        }

        public static List<int> GetTargetDirectional(Vector2 vector, int direction, int distance = 5)
        {

            Dictionary<int, Vector2> vectorIndex = new()
            {

                [0] = vector + new Vector2(0, -distance),// up
                [1] = vector + new Vector2(distance, 0), // right
                [2] = vector + new Vector2(0, distance),// down
                [3] = vector + new Vector2(-distance, 0), // left

            };

            Vector2 targetVector = vectorIndex[direction];

            List<int> targetList = new()
            {
                direction,
                (int)targetVector.X,
                (int)targetVector.Y
            };

            return targetList;

        }

        public void CastRite()
        {

            Vector2 castPosition = castVector * 64;

            float castLimit = 480;

            if (castLocation.characters.Count > 0)
            {

                foreach (NPC riteWitness in castLocation.characters)
                {

                    if (castType == "ether")
                    {
                        break;
                    }

                    if (riteWitness is StardewValley.Monsters.Monster)
                    {
                        continue;
                    }

                    if (riteWitness is StardewDruid.Character.Character)
                    {
                        continue;
                    }

                    if (riteWitness is StardewDruid.Character.Dragon)
                    {
                        continue;
                    }

                    if (Vector2.Distance(riteWitness.Position, castPosition) < castLimit)
                    {

                        if (Mod.instance.WitnessedRite(castType, riteWitness))
                        {

                            continue;

                        }

                        if (riteWitness is Pet petPet)
                        {

                            petPet.checkAction(caster, castLocation);
                        }

                        if (Game1.NPCGiftTastes.ContainsKey(riteWitness.Name))
                        {

                            if (castType == "stars")
                            {

                                Reaction.ReactTo(riteWitness, "Stars");

                            }
                            else if (castType == "mists")
                            {

                                Reaction.ReactTo(riteWitness, "Mists");

                            }
                            else if (castType == "fates" && Mod.instance.CurrentProgress() >= 22)
                            {


                                effectCasts[riteWitness.Position] = new Cast.Fates.Trick(castVector, this, riteWitness);

                            }
                            else
                            {

                                effectCasts[riteWitness.Position] = new Cast.Weald.Villager(castVector, this, riteWitness);

                            }

                        }

                    }

                }

            }

            if (castLocation is Farm farmLocation)
            {

                foreach (KeyValuePair<long, FarmAnimal> pair in farmLocation.animals.Pairs)
                {

                    if (Vector2.Distance(pair.Value.Position, castPosition) >= castLimit)
                    {

                        continue;

                    }

                    Cast.Weald.Animal animalCast = new(castVector, this, pair.Value);

                    animalCast.CastEffect();

                }

            }

            if (castLocation is AnimalHouse animalLocation)
            {

                foreach (KeyValuePair<long, FarmAnimal> pair in animalLocation.animals.Pairs)
                {

                    if (Vector2.Distance(pair.Value.Position, castPosition) >= castLimit)
                    {

                        continue;

                    }

                    Cast.Weald.Animal animalCast = new(castVector, this, pair.Value);

                    animalCast.CastEffect();


                }

            }

            if (castBuffs)
            {

                Vector2 casterVector = caster.getTileLocation();

                if (castLocation.terrainFeatures.ContainsKey(casterVector))
                {

                    if (castLocation.terrainFeatures[casterVector] is StardewValley.TerrainFeatures.Grass)
                    {
                        Buff speedBuff = new("Druidic Freneticism", 6000, "Rite of the " + castType, 9);

                        speedBuff.buffAttributes[9] = 2;

                        speedBuff.which = 184652;

                        if (!Game1.buffsDisplay.hasBuff(184652))
                        {

                            Game1.buffsDisplay.addOtherBuff(speedBuff);

                        }

                    }

                }

            }

            switch (castType)
            {

                case "stars":

                    CastStars();

                    break;

                case "mists":

                    CastMists();

                    break;

                case "fates":

                    CastFates();

                    break;

                case "ether":

                    CastEther();

                    break;

                default: //CastEarth

                    CastWeald();

                    break;
            }

            CastEffect();

        }

        public void CastEffect(bool exactCost = true)
        {

            int castCost = 0;

            //-------------------------- fire effects

            if (effectCasts.Count != 0)
            {

                foreach (KeyValuePair<Vector2, Cast.CastHandle> effectEntry in effectCasts)
                {

                    Cast.CastHandle effectHandle = effectEntry.Value;

                    Type effectType = effectHandle.GetType();

                    if (Mod.instance.activeData.castLimits.Contains(effectType))
                    {

                        continue;

                    }

                    effectHandle.CastEffect();

                    if (effectHandle.castFire)
                    {

                        castCost += effectHandle.castCost;

                    }

                    if (effectHandle.castLimit)
                    {

                        Mod.instance.activeData.castLimits.Add(effectEntry.Value.GetType());

                    }

                }

            }

            //-------------------------- effect on player

            if (castCost > 0 && exactCost)
            {

                float oldStamina = Game1.player.Stamina;

                float staminaCost = Math.Min(castCost, oldStamina - 1);

                Game1.player.Stamina -= staminaCost;

                Game1.player.checkForExhaustion(oldStamina);

            }

            effectCasts.Clear();

        }

        public void CastWeald()
        {

            int chargeFactor = castLevel % 4;

            int chargeLevel = (chargeFactor * 2) + 1;

            Layer backLayer = castLocation.Map.GetLayer("Back");

            Layer buildingLayer = castLocation.Map.GetLayer("Buildings");

            string locationName = castLocation.Name;

            int progressLevel = Mod.instance.CurrentProgress();

            float damageLevel = Mod.instance.DamageLevel();

            //---------------------------------------------
            // Weald Sound
            //---------------------------------------------


            int soundLevel = castLevel % 4;

            int pitchLevel = castLevel / 4;

            //-------------------------- sound and pitch

            if (pitchLevel <= 2)
            {

                if (soundLevel == 1)
                {

                    Game1.player.currentLocation.playSoundPitched("discoverMineral", 600 + (pitchLevel * 200));

                }

                if (soundLevel == 3)
                {

                    Game1.player.currentLocation.playSoundPitched("discoverMineral", 700 + (pitchLevel * 200));

                }

            }

            //---------------------------------------------
            // Weed destruction
            //---------------------------------------------

            if (castLocation.objects.Count() > 0 && spawnIndex["weeds"])
            {

                for (int i = 0; i < 5; i++)
                {

                    List<Vector2> weedVectors = ModUtility.GetTilesWithinRadius(castLocation, castVector, i);

                    foreach (Vector2 tileVector in weedVectors)
                    {

                        if (castLocation.objects.ContainsKey(tileVector))
                        {

                            StardewValley.Object tileObject = castLocation.objects[tileVector];

                            if (tileObject.name.Contains("Stone"))
                            {

                                if (Map.SpawnData.StoneIndex().Contains(tileObject.ParentSheetIndex))
                                {

                                    effectCasts[tileVector] = new Cast.Weald.Weed(tileVector, this, damageLevel);

                                }

                            }
                            else if (tileObject.name.Contains("Weeds") || tileObject.name.Contains("Twig"))
                            {

                                effectCasts[tileVector] = new Cast.Weald.Weed(tileVector, this, damageLevel);

                            }
                            else if (castLocation is MineShaft && tileObject is BreakableContainer)
                            {

                                effectCasts[tileVector] = new Cast.Weald.Weed(tileVector, this, damageLevel);

                            }

                        }

                        continue;

                    }

                }

            }

            if (castLocation is MineShaft || castLocation is VolcanoDungeon)
            {

                if (progressLevel >= 5)
                {

                    CastRockfall();

                    if (Game1.player.Stamina <= 15)
                    {

                        return;

                    }

                }

                return;

            }

            // ---------------------------------------------
            // Location iteration
            // ---------------------------------------------

            if (!Mod.instance.targetCasts.ContainsKey(locationName))
            {

                Mod.instance.targetCasts[locationName] = ModUtility.LocationTargets(castLocation);

            }

            if (!Mod.instance.featureCasts.ContainsKey(locationName))
            {

                Mod.instance.featureCasts[locationName] = new();

            }

            if (!Mod.instance.terrainCasts.ContainsKey(locationName))
            {

                Mod.instance.terrainCasts[locationName] = new();

            }

            // ---------------------------------------------
            // Large Feature iteration
            // ---------------------------------------------

            float castLimit = chargeLevel + 0.5f;

            if (castLocation.largeTerrainFeatures.Count > 0 && progressLevel >= 3)
            {

                foreach (LargeTerrainFeature largeTerrainFeature in castLocation.largeTerrainFeatures)
                {

                    if (largeTerrainFeature is not StardewValley.TerrainFeatures.Bush bushFeature)
                    {

                        continue;

                    }

                    Vector2 featureVector = bushFeature.tilePosition.Value;

                    if (Mod.instance.featureCasts[locationName].ContainsKey(featureVector)) // already served
                    {

                        continue;

                    }

                    if (Vector2.Distance(featureVector, castVector) < castLimit)
                    {

                        effectCasts[featureVector] = new Cast.Weald.Bush(featureVector, this, bushFeature);

                        Mod.instance.featureCasts[locationName].Add(featureVector, 1);

                    }

                }

            }

            if (castLocation.resourceClumps.Count > 0 && progressLevel >= 3)
            {

                foreach (ResourceClump resourceClump in castLocation.resourceClumps)
                {

                    Vector2 featureVector = resourceClump.tile.Value;

                    if (Mod.instance.featureCasts[locationName].ContainsKey(featureVector)) // already served
                    {

                        continue;

                    }

                    if (Vector2.Distance(featureVector, castVector) < castLimit)
                    {

                        switch (resourceClump.parentSheetIndex.Value)
                        {

                            case 600:
                            case 602:

                                effectCasts[featureVector] = new Cast.Weald.Stump(featureVector, this, resourceClump, "Farm");

                                break;

                            default:

                                effectCasts[featureVector] = new Cast.Weald.Boulder(featureVector, this, resourceClump);

                                break;

                        }
                        Mod.instance.featureCasts[locationName][featureVector] = 1;

                    }

                }

            }

            if (castLocation is Woods woodyLocation && progressLevel >= 3)
            {

                foreach (ResourceClump resourceClump in woodyLocation.stumps)
                {

                    Vector2 featureVector = resourceClump.tile.Value;

                    if (Mod.instance.featureCasts[locationName].ContainsKey(featureVector)) // already served
                    {

                        continue;

                    }

                    if (Vector2.Distance(featureVector, castVector) < castLimit)
                    {

                        effectCasts[featureVector] = new Cast.Weald.Stump(featureVector, this, resourceClump, "Woods");

                        Mod.instance.featureCasts[locationName][featureVector] = 1;

                    }

                }

            }

            // ---------------------------------------------
            // Random Effect Center Selection
            // ---------------------------------------------

            List<Vector2> centerVectors = new();

            int castAttempt = castLevel % 4;

            if (castLevel % 4 == 0)
            {

                centerVectors.Add(castVector);

            }

            List<Vector2> castSelection = ModUtility.GetTilesWithinRadius(castLocation, castVector, (castAttempt * 2) + 2); // 2, 4, 6, 8

            if (randomIndex.Next(2) == 0) { castSelection.Reverse(); } // clockwise iteration can slightly favour one side

            int castSelect = castSelection.Count; // 16, 24, 28, 32 // 12, 24, 32, 32

            if (castSelect != 0)
            {

                List<int> segmentList = new() // 2, 3, 4, 4 // 1, 2, 3, 4
                {
                    12, 12, 10, 8,
                };

                int castSegment = segmentList[castAttempt];

                List<int> cycleList = new()
                {
                    1, 2, 3, 4,
                };

                int castCycle = cycleList[castAttempt];

                int castIndex;

                for (int k = 0; k < castCycle; k++)
                {
                    int castLower = castSegment * k;

                    if (castLower + 1 >= castSelect)
                    {

                        continue;

                    }

                    int castHigher = Math.Min(castLower + castSegment, castSelection.Count);

                    castIndex = randomIndex.Next(castLower, castHigher);

                    centerVectors.Add(castSelection[castIndex]);

                }

            }

            int castRadius = 3;

            if (Mod.instance.virtualHoe.UpgradeLevel >= 3 || Mod.instance.virtualCan.UpgradeLevel >= 3)
            {

                castRadius++;

            }

            for (int v = 0; v < centerVectors.Count; v++)
            {

                Vector2 centerVector = centerVectors[v];

                Dictionary<int, List<Vector2>> tileVectors = new();

                for (int i = 0; i < castRadius; i++)
                {

                    tileVectors[i] = ModUtility.GetTilesWithinRadius(castLocation, centerVector, i);

                }

                // ---------------------------------------------
                // Small Feature Based Iteration
                // ---------------------------------------------

                List<Vector2> grassVectors = new();

                for (int i = 0; i < tileVectors.Count; i++)
                {

                    foreach (Vector2 tileVector in tileVectors[i])
                    {

                        int tileX = (int)tileVector.X;

                        int tileY = (int)tileVector.Y;

                        if (Mod.instance.targetCasts[locationName].ContainsKey(tileVector))
                        {
                            continue;
                        }

                        Tile buildingTile = buildingLayer.PickTile(new xTile.Dimensions.Location(tileX * 64, tileY * 64), Game1.viewport.Size);

                        if (buildingTile != null)
                        {

                            if (castLocation is Farm farmLocation)
                            {
                                int tileIndex = buildingTile.TileIndex;

                                if (tileIndex == 1938)
                                {
                                    effectCasts[tileVector] = new Cast.Weald.Bowl(tileVector, this);
                                }

                            }

                            if (castLocation is Beach && !Mod.instance.EffectDisabled("Fish") && progressLevel >= 4)
                            {

                                Vector2 terrainVector = new((int)(tileVector.X / 6), (int)(tileVector.Y / 6));

                                if (Mod.instance.terrainCasts[locationName].ContainsKey(terrainVector))
                                {
                                    continue;
                                }

                                List<int> tidalList = new() { 60, 61, 62, 63, 77, 78, 79, 80, 94, 95, 96, 97, 104, 287, 288, 304, 305, 321, 362, 363 };

                                if (tidalList.Contains(buildingTile.TileIndex))
                                {

                                    effectCasts[tileVector] = new Cast.Weald.Pool(tileVector, this);

                                }

                                Mod.instance.terrainCasts[locationName][terrainVector] = "Pool";

                            }

                            Mod.instance.targetCasts[locationName][tileVector] = "Building";

                            continue;


                        }

                        if (castLocation.terrainFeatures.ContainsKey(tileVector))
                        {

                            if (progressLevel >= 3)
                            {

                                TerrainFeature terrainFeature = castLocation.terrainFeatures[tileVector];

                                switch (terrainFeature.GetType().Name.ToString())
                                {

                                    case "FruitTree":

                                        StardewValley.TerrainFeatures.FruitTree fruitFeature = terrainFeature as StardewValley.TerrainFeatures.FruitTree;

                                        if (fruitFeature.growthStage.Value >= 4)
                                        {

                                            effectCasts[tileVector] = new Cast.Weald.FruitTree(tileVector, this);

                                        }
                                        else if (progressLevel >= 5)
                                        {

                                            effectCasts[tileVector] = new Cast.Weald.FruitSapling(tileVector, this);

                                        }

                                        Mod.instance.targetCasts[locationName][tileVector] = "Tree";

                                        break;

                                    case "Tree":

                                        StardewValley.TerrainFeatures.Tree treeFeature = terrainFeature as StardewValley.TerrainFeatures.Tree;

                                        if (treeFeature.growthStage.Value >= 5)
                                        {

                                            effectCasts[tileVector] = new Cast.Weald.Tree(tileVector, this);

                                            Mod.instance.targetCasts[locationName][tileVector] = "Tree";

                                        }
                                        else if (progressLevel >= 5 && treeFeature.fertilized.Value == false)
                                        {

                                            effectCasts[tileVector] = new Cast.Weald.Sapling(tileVector, this);

                                            Mod.instance.targetCasts[locationName][tileVector] = "Sapling";

                                        }

                                        break;

                                    case "Grass":


                                        StardewValley.TerrainFeatures.Grass grassFeature = terrainFeature as StardewValley.TerrainFeatures.Grass;

                                        Microsoft.Xna.Framework.Rectangle tileRectangle = new(tileX * 64 + 1, tileY * 64 + 1, 62, 62);

                                        grassFeature.doCollisionAction(tileRectangle, 2, tileVector, null, Game1.currentLocation);

                                        grassVectors.Add(tileVector);

                                        break;

                                    case "HoeDirt":

                                        if (progressLevel >= 5)
                                        {

                                            if (spawnIndex["cropseed"])
                                            {

                                                effectCasts[tileVector] = new Cast.Weald.Crop(tileVector, this);

                                            }

                                        }

                                        Mod.instance.targetCasts[locationName][tileVector] = "Hoed";

                                        break;

                                    default:

                                        Mod.instance.targetCasts[locationName][tileVector] = "Feature";

                                        break;

                                }

                            }

                            continue;

                        }

                    }

                }

                // ---------------------------------------------
                // Terrain Based Iteration
                // ---------------------------------------------

                for (int i = 0; i < tileVectors.Count; i++)
                {

                    foreach (Vector2 tileVector in tileVectors[i])
                    {

                        Vector2 terrainVector = new((int)(tileVector.X / 6), (int)(tileVector.Y / 6));

                        if (Mod.instance.terrainCasts[locationName].ContainsKey(terrainVector)) // already served
                        {

                            continue;

                        }

                        if (Mod.instance.targetCasts[locationName].ContainsKey(tileVector)) // already served
                        {

                            continue;

                        }

                        if (castLocation.objects.ContainsKey(tileVector))
                        {

                            continue;

                        }

                        if (grassVectors.Contains(tileVector))
                        {

                            effectCasts[tileVector] = new Cast.Weald.Grass(tileVector, this);

                            Mod.instance.terrainCasts[locationName][terrainVector] = "Grass";

                            continue;

                        }

                        int tileX = (int)tileVector.X;

                        int tileY = (int)tileVector.Y;

                        Tile backTile = backLayer.PickTile(new xTile.Dimensions.Location(tileX * 64, tileY * 64), Game1.viewport.Size);

                        if (backTile == null)
                        {

                            Mod.instance.targetCasts[locationName][tileVector] = "Empty";

                            continue;

                        }

                        if (castLocation is AnimalHouse)
                        {

                            if (backTile.TileIndexProperties.TryGetValue("Trough", out _))
                            {

                                effectCasts[tileVector] = new Cast.Weald.Trough(tileVector, this);

                                Mod.instance.targetCasts[locationName][tileVector] = "Trough";

                                continue;

                            }

                        }

                        string tileCheck = ModUtility.GroundCheck(castLocation, tileVector);

                        if(tileCheck == "water")
                        {

                            if (progressLevel >= 3)
                            {

                                if (spawnIndex["fishup"] && !Mod.instance.EffectDisabled("Fish"))
                                {

                                    if (castLocation.Name.Contains("Farm"))
                                    {

                                        effectCasts[tileVector] = new Cast.Weald.Pool(tileVector, this);

                                    }
                                    else
                                    {

                                        effectCasts[tileVector] = new Cast.Weald.Water(tileVector, this);

                                    }

                                    Mod.instance.terrainCasts[locationName][terrainVector] = "Water";

                                }

                            }

                        }

                        if(tileCheck != "ground")
                        {

                            return;

                        }

                        Dictionary<string, List<Vector2>> neighbourList;
                        
                        neighbourList = ModUtility.NeighbourCheck(castLocation, tileVector,0);

                        if (neighbourList.Count > 0)
                        {

                            continue;

                        }

                        neighbourList = ModUtility.NeighbourCheck(castLocation, tileVector);

                        if (neighbourList.Count > 0)
                        {

                            continue;

                        }

                        if (progressLevel >= 4)
                        {

                            if (backTile.TileIndexProperties.TryGetValue("Type", out var typeValue))
                            {

                                if (typeValue == "Dirt" || backTile.TileIndexProperties.TryGetValue("Diggable", out _))
                                {

                                    effectCasts[tileVector] = new Cast.Weald.Dirt(tileVector, this);

                                    Mod.instance.terrainCasts[locationName][terrainVector] = "Dirt";

                                    continue;

                                }

                                if (typeValue == "Grass" && backTile.TileIndexProperties.TryGetValue("NoSpawn", out _) == false)
                                {

                                    effectCasts[tileVector] = new Cast.Weald.Lawn(tileVector, this);

                                    Mod.instance.terrainCasts[locationName][terrainVector] = "Lawn";

                                    continue;

                                }

                            }

                        }

                    }

                }

                if (castLevel % 4 == 0 && v == 0)
                {

                    ModUtility.AnimateRadiusDecoration(castLocation, centerVector, "Weald", 0.75f, 0.5f, 2400f);

                }
                else if (castLevel % 4 == 2)
                {

                    ModUtility.AnimateRadiusDecoration(castLocation, centerVector, "Weald", 0.5f, 0.5f, 2400f);

                }
                else
                {

                    ModUtility.AnimateSprout(castLocation, centerVector);

                }

            }

        }

        public void CastRockfall()
        {

            if (Mod.instance.CurrentProgress() < 6)
            {

                return;

            }


            if (!Mod.instance.rockCasts.ContainsKey(castLocation.Name))
            {

                Mod.instance.rockCasts[castLocation.Name] = 10;

            };

            int castAttempt = (castLevel % 5);

            List<Vector2> castSelection = ModUtility.GetTilesWithinRadius(castLocation, castVector, castAttempt + 2); // 2, 3, 4, 5, 6

            if (randomIndex.Next(2) == 0) { castSelection.Reverse(); } // clockwise iteration can slightly favour one side

            int castSelect = castSelection.Count; // 12, 16, 24, 28, 32

            float damageLevel = Mod.instance.DamageLevel();

            if (castSelect != 0)
            {

                List<int> segmentList = new() // 2, 2, 3, 4, 4
                {
                    6, 8, 8, 7, 8,
                };

                int castSegment = segmentList[castAttempt];

                List<int> cycleList = new()
                {
                    2, 2, 3, 4, 4,
                };

                int castCycle = cycleList[castAttempt];

                int castIndex;

                Vector2 newVector;

                for (int k = 0; k < castCycle; k++)
                {
                    int castLower = castSegment * k;

                    if (castLower + 1 >= castSelect)
                    {

                        continue;

                    }

                    int castHigher = Math.Min(castLower + castSegment, castSelection.Count);

                    castIndex = randomIndex.Next(castLower, castHigher);

                    newVector = castSelection[castIndex];

                    effectCasts[newVector] = new Cast.Weald.Rockfall(newVector, this, damageLevel);

                }

            }

            int specialChance = Mod.instance.rockCasts[castLocation.Name];

            Mod.instance.rockCasts[castLocation.Name] = Math.Min(specialChance + 1, 50);

        }

        public void CastMists()
        {

            int chargeLevel = (castLevel % 4) + 1;

            int progressLevel = Mod.instance.CurrentProgress();

            List<Vector2> centerVectors = new();

            // ---------------------------------------------
            // Mists Sound
            // ---------------------------------------------

            if (castLevel == 1)
            {

                Game1.player.currentLocation.playSoundPitched("thunder_small", 600 + (new Random().Next(5) * 200));

            }

            // ---------------------------------------------
            // Map Feature Iteration
            // ---------------------------------------------

            string locationName = castLocation.Name.ToString();

            if (!Mod.instance.specialCasts.ContainsKey(locationName))
            {

                Mod.instance.specialCasts[locationName] = new();

            }

            List<string> specialCasts = Mod.instance.specialCasts[locationName];

            float castLimit = chargeLevel * 2.25f;

            Vector2 warpVector = Map.WarpData.WarpVectors(castLocation);

            if (warpVector != Vector2.One && !specialCasts.Contains("warp"))
            {

                if (Vector2.Distance(castVector, warpVector) <= castLimit)
                {

                    int targetIndex = Map.WarpData.WarpTotems(castLocation);

                    effectCasts[warpVector] = new Cast.Mists.Totem(warpVector, this, targetIndex);

                    Mod.instance.specialCasts[locationName].Add("warp");

                    centerVectors.Add(warpVector);

                }

            }

            if(progressLevel >= 10)
            {

                Vector2 fireVector = Map.FireData.FireVectors(castLocation);

                if (fireVector != Vector2.One && !specialCasts.Contains("fire"))
                {

                    if (Vector2.Distance(castVector, fireVector) <= castLimit)
                    {

                        effectCasts[fireVector] = new Cast.Mists.Campfire(fireVector, this);

                        Mod.instance.specialCasts[locationName].Add("fire");

                        centerVectors.Add(fireVector);

                    }

                }

            }

            if (castLocation.resourceClumps.Count > 0)
            {

                foreach (ResourceClump resourceClump in castLocation.resourceClumps)
                {

                    Vector2 featureVector = resourceClump.tile.Value;

                    if (Vector2.Distance(featureVector, castVector) <= castLimit)
                    {

                        switch (resourceClump.parentSheetIndex.Value)
                        {

                            case 600:
                            case 602:

                                effectCasts[featureVector] = new Cast.Mists.Stump(featureVector, this, resourceClump, "Farm");

                                break;

                            default:

                                effectCasts[featureVector] = new Cast.Mists.Boulder(featureVector, this, resourceClump);

                                break;

                        }

                        centerVectors.Add(featureVector);

                    }

                }

            }

            if (castLocation is Woods woodyLocation)
            {

                foreach (ResourceClump resourceClump in woodyLocation.stumps)
                {

                    Vector2 featureVector = resourceClump.tile.Value;

                    if (Vector2.Distance(featureVector, castVector) <= castLimit)
                    {

                        effectCasts[featureVector] = new Cast.Mists.Stump(featureVector, this, resourceClump, "Woods");

                        centerVectors.Add(featureVector);

                    }

                }
            }

            if (castLocation is Forest forestLocation)
            {

                if (forestLocation.log != null)
                {

                    Vector2 featureVector = forestLocation.log.tile.Value;


                    if (Vector2.Distance(featureVector, castVector) <= castLimit)
                    {

                        effectCasts[featureVector] = new Cast.Mists.Stump(featureVector, this, forestLocation.log, "Log");

                        centerVectors.Add(featureVector);

                    }

                }


            }

            // ---------------------------------------------
            // Water effect
            // ---------------------------------------------

            if (progressLevel >= 11 && chargeLevel == 1)
            {

                if (spawnIndex["fishspot"])
                {

                    if (ModUtility.WaterCheck(castLocation, castVector))
                    {
                        
                        effectCasts[castVector] = new Cast.Mists.Water(castVector, this);

                        centerVectors.Add(castVector);

                    }

                }

                if (castLocation is VolcanoDungeon volcanoLocation)
                {
                    int tileX = (int)castVector.X;
                    int tileY = (int)castVector.Y;

                    if (volcanoLocation.waterTiles[tileX, tileY] && !volcanoLocation.cooledLavaTiles.ContainsKey(castVector))
                    {

                        effectCasts[castVector] = new Cast.Mists.Lava(castVector, this);

                        centerVectors.Add(castVector);

                    }

                }

            }

            // ---------------------------------------------
            // Monster iteration
            // ---------------------------------------------

            if (progressLevel >= 12)
            {

                int smiteCount = 0;

                int smiteLimit = Mod.instance.PowerLevel();

                Vector2 castPosition = castVector * 64;

                float smiteThreshold;

                float smiteRange = Vector2.Distance(caster.Position, castPosition);

                float damageLevel = Mod.instance.DamageLevel();

                foreach (NPC nonPlayableCharacter in castLocation.characters)
                {

                    if (nonPlayableCharacter is StardewValley.Monsters.Monster monsterCharacter)
                    {

                        float monsterDifference = Vector2.Distance(monsterCharacter.Position, castPosition);

                        smiteThreshold = 256f;

                        if(monsterCharacter.Sprite.SpriteWidth > 16)
                        {
                            smiteThreshold += 64f;
                        }

                        if (monsterCharacter.Sprite.SpriteWidth > 32)
                        {
                            smiteThreshold += 64f;
                        }

                        if (monsterDifference < smiteThreshold)
                        {

                            Vector2 monsterVector = monsterCharacter.getTileLocation();

                            effectCasts[monsterVector] = new Cast.Mists.Smite(monsterVector, this, monsterCharacter, damageLevel);

                            centerVectors.Add(monsterVector);

                            smiteCount++;

                            break;

                        }

                        float monsterProximity = Vector2.Distance(monsterCharacter.Position, caster.Position);

                        if(monsterDifference < smiteRange && monsterProximity < smiteThreshold)
                        {

                            Vector2 monsterVector = monsterCharacter.getTileLocation();

                            effectCasts[monsterVector] = new Cast.Mists.Smite(monsterVector, this, monsterCharacter, damageLevel);

                            centerVectors.Add(monsterVector);

                            smiteCount++;

                            break;

                        }

                    }

                    if (smiteCount == smiteLimit)
                    {
                        break;
                    }
 
                }

                for (int i = 0; i < 1; i++)
                {

                    if (smiteCount == 0 || Mod.instance.eventRegister.ContainsKey("veil"))
                    {

                        break;

                    }

                    StardewDruid.Event.World.Veil veilEvent = new(caster.Position, this);
                    
                    veilEvent.EventTrigger();

                }

            }

            // ---------------------------------------------
            // Tile iteration
            // ---------------------------------------------
            
            List<Vector2> tileVectors;

            for (int i = 0; i < 2; i++)
            {

                int castRange = (chargeLevel * 2) - 2 + i;

                tileVectors = ModUtility.GetTilesWithinRadius(castLocation, castVector, castRange);

                List<Vector2> betweenVectors = ModUtility.GetTilesBetweenVectors(castLocation, caster.Position, castVector*64);

                tileVectors.AddRange(betweenVectors);

                foreach (Vector2 tileVector in tileVectors)
                {

                    if (castLocation.objects.Count() > 0)
                    {

                        if (castLocation.objects.ContainsKey(tileVector))
                        {

                            StardewValley.Object targetObject = castLocation.objects[tileVector];

                            if (castLocation.IsFarm && targetObject.bigCraftable.Value && targetObject.ParentSheetIndex == 9)
                            {

                                if (progressLevel < 10 || targetObject.MinutesUntilReady > 1)
                                {
                                    continue;
                                }

                                for(int j = 0; j <= Mod.instance.PowerLevel(); j++)
                                {
                                    if (specialCasts.Contains("rod" + j.ToString()))
                                    {

                                        continue;

                                    }

                                    effectCasts[tileVector] = new Cast.Mists.Rod(tileVector, this);

                                    Mod.instance.specialCasts[locationName].Add("rod" + j.ToString());

                                    centerVectors.Add(tileVector);

                                    break;

                                }

                            }
                            else if (targetObject.Name.Contains("Campfire"))
                            {

                                string fireLocation = castLocation.Name;

                                if (!specialCasts.Contains("campfire") && progressLevel >= 10)
                                {
                                    effectCasts[tileVector] = new Cast.Mists.Campfire(tileVector, this);

                                    Mod.instance.specialCasts[locationName].Add("campfire");

                                    centerVectors.Add(tileVector);

                                }
                            }
                            else if (targetObject is Torch && targetObject.ParentSheetIndex == 93) // crafted candle torch
                            {

                                if (progressLevel >= 13 && !Mod.instance.eventRegister.ContainsKey("active"))
                                {
                                    if (spawnIndex["portal"])
                                    {

                                        effectCasts[tileVector] = new Cast.Mists.Portal(tileVector, this);

                                        centerVectors.Add(tileVector);

                                    }

                                }

                            }
                            else if (targetObject.IsScarecrow())
                            {

                                string scid = "scarecrow_" + tileVector.X.ToString() + "_" + tileVector.Y.ToString();

                                if (progressLevel >= 10 && !Game1.isRaining && !specialCasts.Contains(scid))
                                {

                                    effectCasts[tileVector] = new Cast.Mists.Scarecrow(tileVector, this);

                                    Mod.instance.specialCasts[locationName].Add(scid);

                                    centerVectors.Add(tileVector);

                                }

                            }
                            else if (targetObject.Name.Contains("Artifact Spot")) //&& Mod.instance.virtualHoe.UpgradeLevel >= 3)
                            {

                                effectCasts[tileVector] = new Cast.Mists.Artifact(tileVector, this);

                                centerVectors.Add(tileVector);

                            }

                            continue;

                        }

                    }

                    if (castLocation.terrainFeatures.ContainsKey(tileVector))
                    {

                        if (progressLevel >= 9)
                        {

                            if (castLocation.terrainFeatures[tileVector] is StardewValley.TerrainFeatures.Tree treeFeature)
                            {

                                if (treeFeature.stump.Value)
                                {

                                    effectCasts[tileVector] = new Cast.Mists.Tree(tileVector, this);

                                    centerVectors.Add(tileVector);

                                }

                            }

                        }

                        continue;

                    }

                }

            }

            if (centerVectors.Count == 0 && castLevel % 4 == 0)
            {

                centerVectors.Add(castVector);

            }

            foreach (var centerVector in centerVectors)
            {

                ModUtility.AnimateRadiusDecoration(castLocation, centerVector, "Mists", 1f, 0.5f, 1000f);

            }

        }

        public void CastStars()
        {

            //int castAttempt = (castLevel % 6);
            int castAttempt = (castLevel % 4);

            List<Vector2> meteorVectors = new();

            List<Vector2> castSelection = ModUtility.GetTilesWithinRadius(castLocation, castVector, castAttempt + 2); // 2,3,4,5,6,7

            if (randomIndex.Next(2) == 0) { castSelection.Reverse(); }

            int castSelect = castSelection.Count; // 12, 16, 24, 28, 32, 28

            if (castSelect == 0)
            {

                return;

            }

            List<int> segmentList = new() // 1, 1, 2, 3, 3, 4
            {
                12, 16, 12, 9, //11, 7
            };

            int castSegment = segmentList[castAttempt];

            List<int> cycleList = new()
            {
                1, 1, 2, 3, //3, //4
            };

            int castCycle = cycleList[castAttempt];

            int castIndex;

            Vector2 newVector;

            float damageLevel = Mod.instance.DamageLevel();

            for (int k = 0; k < castCycle; k++)
            {

                int castLower = castSegment * k;

                if (castLower + 2 >= castSelect)
                {

                    continue;

                }

                int monsterIndex = Math.Min(castLower + castSegment / 2, castSelection.Count - 1);

                Vector2 monsterTest = castSelection[monsterIndex];

                bool priorityCast = false;

                string difficulty = Mod.instance.DifficultyLevel();

                int difficultyLevel = 1;

                switch (difficulty)
                {

                    case "medium":

                        difficultyLevel = 2;

                        break;

                    case "hard":

                        difficultyLevel = 3;

                        break;

                }

                if (difficultyLevel < 2)
                {

                    foreach (NPC character in this.castLocation.characters)
                    {

                        if (character is StardewValley.Monsters.Monster monster && Vector2.Distance(monster.Position, monsterTest * 64f) < 192.0)
                        {

                            Vector2 tileLocation = monster.getTileLocation();

                            effectCasts[tileLocation] = new Cast.Stars.Meteor(tileLocation, this, damageLevel);

                            meteorVectors.Add(tileLocation);

                            priorityCast = true;

                            break;

                        }

                    }
                
                }


                int castHigher = Math.Min(castLower + castSegment, castSelection.Count);

                if (castLocation.objects.Count() > 0 && !priorityCast && difficultyLevel < 3)
                {

                    for (int j = castLower; j < castHigher; j++)
                    {

                        newVector = castSelection[j];

                        if (castLocation.objects.ContainsKey(newVector))
                        {

                            StardewValley.Object tileObject = castLocation.objects[newVector];

                            if (tileObject.name.Contains("Stone"))
                            {

                                effectCasts[newVector] = new Cast.Stars.Meteor(newVector, this, damageLevel);

                                meteorVectors.Add(newVector);

                                priorityCast = true;

                                break;

                            }

                        }

                    }


                }

                if (!priorityCast)
                {

                    castIndex = randomIndex.Next(castLower, castHigher);

                    newVector = castSelection[castIndex];

                    effectCasts[newVector] = new Cast.Stars.Meteor(newVector, this, damageLevel);

                    meteorVectors.Add(newVector);

                }

                if (!castTask.ContainsKey("masterMeteor"))
                {

                    Mod.instance.UpdateTask("lessonMeteor", 1);

                }

            }

            for (int a = 0; a < 1; a++)
            {

                if (!castTask.ContainsKey("masterGravity"))
                {

                    break;

                }

                if (!Mod.instance.eventRegister.ContainsKey("gravity"))
                {

                    break;

                }

                if (castLevel != 0)
                {

                    break;

                }

                if (Mod.instance.eventRegister.ContainsKey("comet"))
                {

                    break;

                }

                Vector2 cometVector = (Mod.instance.eventRegister["gravity"] as Gravity).targetVector;

                Event.World.Comet cometEvent = new(cometVector, this, damageLevel);

                cometEvent.EventTrigger();

            }

        }

        public void CastFates()
        {

            string locationName = castLocation.Name;

            int progressLevel = Mod.instance.CurrentProgress();

            if (!Mod.instance.specialCasts.ContainsKey(locationName))
            {

                Mod.instance.specialCasts[locationName] = new();

            }

            List<string> specialCasts = Mod.instance.specialCasts[locationName];

            //int useSource = ChooseSource();

            if (!Mod.instance.targetCasts.ContainsKey(locationName))
            {

                Mod.instance.targetCasts[locationName] = ModUtility.LocationTargets(castLocation);

            }

            // ---------------------------------------------
            // Enchant
            // ---------------------------------------------

            if (castLocation.objects.Count() > 0 && progressLevel >= 23)
            {

                int castAttempt = (castLevel % 8);

                List<Vector2> tileVectors = ModUtility.GetTilesWithinRadius(castLocation, castVector, castAttempt + 1); // 1, 2, 3, 4, 5, 6, 7, 8

                List<Vector2> betweenVectors = ModUtility.GetTilesBetweenVectors(castLocation, caster.Position, castVector * 64);

                tileVectors.AddRange(betweenVectors);

                List<string> craftIndex = Map.SpawnData.MachineList();

                List<Vector2> objectVectors = new();

                foreach (Vector2 tileVector in tileVectors)
                {

                    if (Mod.instance.targetCasts[locationName].ContainsKey(tileVector))
                    {

                        continue;

                    }

                    if (castLocation.objects.ContainsKey(tileVector))
                    {

                        StardewValley.Object targetObject = castLocation.objects[tileVector];

                        if (craftIndex.Contains(targetObject.Name))
                        {

                            if (targetObject.heldObject.Value == null || targetObject.MinutesUntilReady > 10)
                            {

                                objectVectors.Add(tileVector);

                            }

                            continue;

                        }

                    }

                }

                if (objectVectors.Count > 0)
                {

                    List<int> cycleList = new()
                    {
                       1, 2, 2, 3, 3, 4, 4, 5,
                    };

                    int castCycle = cycleList[castAttempt];

                    int useSource = ChooseSource();

                    for (int i = 0; i < castCycle; i++)
                    {

                        if (useSource == -1)
                        {

                            break;

                        }

                        int selectedIndex = randomIndex.Next(objectVectors.Count);

                        Vector2 tileVector = objectVectors[selectedIndex];

                        StardewValley.Object targetObject = castLocation.objects[tileVector];

                        effectCasts[tileVector] = new Cast.Fates.Enchant(tileVector, this, targetObject, useSource);

                        Mod.instance.targetCasts[locationName][tileVector] = "Machine";

                        useSource = ConsumeSource(useSource);

                        objectVectors.RemoveAt(selectedIndex);

                        if (objectVectors.Count == 0)
                        {
                            break;
                        }

                    }

                    return;

                }

            }

            // ---------------------------------------------
            // Escape
            // ---------------------------------------------

            for (int a = 0; a < 1; a++)
            {

                if (castLevel == 0) { break; }

                if (Mod.instance.eventRegister.ContainsKey("escape")) { break; }

                if (castLocation.warps.Count() <= 0) { if (castLocation is not MineShaft) { break; } }

                Event.World.Escape escapeEvent = new(caster.Position, this);

                escapeEvent.EventTrigger();

            }

            // ---------------------------------------------
            // Gravity
            // ---------------------------------------------

            for (int a = 0; a < 1; a++)
            {

                if (progressLevel < 24) { break; }

                if (!spawnIndex["gravity"]) { break; }

                if (castLocation.objects.Count() <= 0) { break; }

                if (castLevel % 2 == 1) { break; }

                if (Mod.instance.eventRegister.ContainsKey("gravity")) { break; }

                //List<int> targetList = GetTargetCursor(caster.getTileLocation(), caster.FacingDirection, 5);

                //Vector2 wellVector = new(targetList[1], targetList[2]);

                Vector2 wellVector = castVector;

                for (int i = 0; i < 3; i++)
                {

                    List<Vector2> wellVectors = ModUtility.GetTilesWithinRadius(castLocation, wellVector, i);

                    foreach (Vector2 tileVector in wellVectors)
                    {

                        if (!castLocation.objects.ContainsKey(tileVector)) { continue; }

                        StardewValley.Object targetObject = castLocation.objects[tileVector];

                        if (!targetObject.IsScarecrow()) { continue; }

                        string scid = "gravity_" + castLocation.Name + "_" + tileVector.X.ToString() + "_" + tileVector.Y.ToString();

                        if (specialCasts.Contains(scid)) { continue; }

                        effectCasts[tileVector] = new Cast.Fates.Gravity(tileVector, this, 0);

                        Mod.instance.specialCasts[locationName].Add(scid);

                        return;

                    }

                }

            }

            // ---------------------------------------------
            // Gravity - Monster
            // ---------------------------------------------

            for (int a = 0; a < 1; a++)
            {
                if (progressLevel < 24) { break; }

                if (castLocation.characters.Count <= 0) { break; }

                if (Mod.instance.eventRegister.ContainsKey("gravity")) { break; }

                //List<int> targetList = GetTargetCursor(caster.getTileLocation(), caster.FacingDirection, 5);

                //Vector2 wellVector = new(targetList[1], targetList[2]);

                Vector2 wellVector = castVector;

                foreach (NPC nonPlayableCharacter in castLocation.characters)
                {

                    if (nonPlayableCharacter is not StardewValley.Monsters.Monster monsterCharacter) { continue; }

                    if (monsterCharacter.Health <= 0 || monsterCharacter.IsInvisible) { continue; }

                    float monsterDifference = Vector2.Distance(monsterCharacter.Position, wellVector * 64);

                    if (monsterDifference > 640f) { continue; }

                    effectCasts[wellVector] = new Cast.Fates.Gravity(wellVector, this, 1);

                    return;

                }

            }
            // ---------------------------------------------
            // Gravity - Teahouse
            // ---------------------------------------------

            for (int a = 0; a < 1; a++)
            {
                if (progressLevel < 24) { break; }

                if (!spawnIndex["teahouse"]) { break; }

                if (Mod.instance.eventRegister.ContainsKey("gravity")) { break; }

                //List<int> targetList = GetTargetCursor(caster.getTileLocation(), caster.FacingDirection, 5);

                //Vector2 wellVector = new(targetList[1], targetList[2]);

                Vector2 wellVector = castVector;

                string scid = "gravity_" + castLocation.Name + "_" + wellVector.X.ToString() + "_" + wellVector.Y.ToString();

                if (specialCasts.Contains(scid)) { continue; }

                effectCasts[wellVector] = new Cast.Fates.Gravity(wellVector, this,0);

                Mod.instance.specialCasts[locationName].Add(scid);

                return;

            }


            // ---------------------------------------------
            // Whisk
            // ---------------------------------------------

            for (int a = 0; a < 1; a++)
            {

                if (effectCasts.Count > 0) { break; }

                if (Mod.instance.eventRegister.ContainsKey("whisk")) { break; }

                if (!spawnIndex["whisk"]) { break; }

                if (castLevel != 0) { break; }

                Dictionary<int, Vector2> whiskVectors = new()
                {

                    [0] = new Vector2(0, -2),

                    [1] = new Vector2(2, 0),

                    [2] = new Vector2(0, 2),

                    [3] = new Vector2(-2, 0),

                };

                int whiskDirection = caster.facingDirection;

                if (caster.movementDirections.Count > 0)
                {

                    whiskDirection = caster.movementDirections.ElementAt(0);

                }

                Vector2 whiskSegment = whiskVectors[whiskDirection];

                int whiskRange = 8;

                if (castTask.ContainsKey("masterWhisk"))
                {

                    whiskRange += 2;

                }

                Vector2 originVector = caster.getTileLocation();

                for (int i = whiskRange; i > 3; i--)
                {

                    //Vector2 whiskDestiny = castVector + (whiskSegment * i);

                    Vector2 whiskDestiny = originVector + (whiskSegment * i);

                    if (i == whiskRange)
                    {
                        List<int> targetList = GetTargetCursor(caster.getTileLocation(), whiskDirection, 16, 8);

                        whiskDestiny = new(targetList[1], targetList[2]);

                    }

                    if (ModUtility.GroundCheck(castLocation, whiskDestiny) != "ground")
                    {

                        continue;

                    }

                    Microsoft.Xna.Framework.Rectangle boundingBox = caster.GetBoundingBox();

                    int diffX = (int)(boundingBox.X - caster.Position.X);

                    int diffY = (int)(boundingBox.Y - caster.Position.Y);

                    boundingBox.X = (int)(whiskDestiny.X * 64) + diffX;

                    boundingBox.Y = (int)(whiskDestiny.Y * 64) + diffY;

                    if (castLocation.isCollidingPosition(boundingBox, Game1.viewport, isFarmer: false, 0, glider: false, caster, pathfinding: false))
                    {
                        continue;

                    }

                    //effectCasts[whiskDestiny] = new Cast.Fates.Whisk(castVector, this, whiskDestiny);

                    effectCasts[whiskDestiny] = new Cast.Fates.Whisk(originVector, this, whiskDestiny);

                    break;

                }

            }

            if (effectCasts.Count > 0 && Mod.instance.eventRegister.ContainsKey("escape"))
            {

                if (Mod.instance.eventRegister["escape"].activeCounter >= 2)
                {

                    Mod.instance.eventRegister["escape"].expireEarly = true;

                }

            }

        }

        public int ChooseSource()
        {

            if(castSource.Count <= 0)
            {

                castSource = new()
                {
                    [768] = -1,
                    [769] = -1
                };

                for (int i = 0; i < Game1.player.Items.Count; i++)
                {

                    Item checkItem = Game1.player.Items[i];

                    // ignore empty slots
                    if (checkItem == null)
                    {

                        continue;

                    }

                    int itemIndex = checkItem.ParentSheetIndex;

                    if (itemIndex == 768)
                    {
                        castSource[768] = i;

                        if (castSource[769] != -1) { break; }
                    }

                    if (itemIndex == 769)
                    {
                        castSource[769] = i;

                        if (castSource[768] != -1) { break; }
                    }

                }

            }

            if (castSource[768] == -1)
            {

                if (castSource[769] == -1)
                {

                    Mod.instance.CastMessage("Not enough solar or void essence");

                    return -1;

                }

                return 769;

            }

            if (castSource[769] == -1)
            {

                return 768;

            }

            return (randomIndex.Next(2) == 0) ? 768 : 769;

        }

        public int ConsumeSource(int useSource, int consumption = 1)
        {

            Game1.player.Items[castSource[useSource]].Stack -= consumption;

            if (Game1.player.Items[castSource[useSource]].Stack <= 0)
            {

                Game1.player.Items[castSource[useSource]] = null;

                castSource[useSource] = -1;

            }

            return ChooseSource();

        }

        public void CastEther()
        {
            
            int progressLevel = Mod.instance.CurrentProgress();

            if (!Mod.instance.specialCasts.ContainsKey(castLocation.Name))
            {

                Mod.instance.specialCasts[castLocation.Name] = new();

            }

            List<string> specialCasts = Mod.instance.specialCasts[castLocation.Name];

            for (int index = 0; index < 1; ++index)
            {

                if (castLevel != 0)
                {

                    break;
                }

                if (Mod.instance.eventRegister.ContainsKey("transform"))
                {

                    Mod.instance.eventRegister["transform"].AttemptAbort();

                    break;

                }

                int extend = 120;

                if (Mod.instance.TaskList().ContainsKey("masterTransform"))
                {
                    extend = 180;
                }

                Transform transform = new Transform(caster.Position, this, extend);

                if (progressLevel >= 29)
                {
                    transform.leftActive = true;

                }

                if (progressLevel >= 30)
                {
                    transform.rightActive = true;

                }

                transform.EventTrigger();

                ModUtility.AnimateRadiusDecoration(castLocation, caster.getTileLocation() - new Vector2(0, 1), "Ether", 0.75f, 1.25f, 500f, 999f);

                if (progressLevel < 32)
                {

                    break;

                }

                if (!spawnIndex["crate"])
                {

                    break;

                }

                if (specialCasts.Contains("crate"))
                {
                
                    break;
                
                }
                    
                if(Mod.instance.eventRegister.ContainsKey("crate"))
                {

                    if (Mod.instance.eventRegister["crate"].targetLocation.Name != castLocation.Name)
                    {

                        Mod.instance.eventRegister["crate"].EventAbort();

                        Mod.instance.eventRegister["crate"].EventRemove();

                    }
                    else
                    {

                        break;

                    }

                }

                Crate treasure = new Crate(this);

                treasure.EventTrigger();

            }

            for (int a = 0; a < 1; a++)
            {

                if (castLevel == 0) { 
                    
                    break; 
                
                }

                if (Mod.instance.eventRegister.ContainsKey("escape")) {

                    break; 
                
                }

                if (castLocation.warps.Count() <= 0) { if (castLocation is not MineShaft) { break; } }

                Event.World.Escape escapeEvent = new(caster.Position, this);

                escapeEvent.EventTrigger();

            }

        }

    }

}