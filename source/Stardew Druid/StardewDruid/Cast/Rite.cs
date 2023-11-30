/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Cast.Earth;
using StardewDruid.Cast.Stars;
using StardewDruid.Map;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using static StardewValley.Menus.CharacterCustomization;
using static StardewValley.Minigames.CraneGame;

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

        public int castDamage;

        public int combatModifier;

        public Dictionary<string, int> blessingList;

        public Dictionary<string, int> castTask;

        public Dictionary<string, int> castToggle;

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

            effectCasts = new();

        }

        public void CastDamage(string combatDifficulty = "medium")
        {

            castDamage = Mod.instance.DamageLevel();

            int monsterDifficulty = 1;

            switch (combatDifficulty)
            {
                case "hard":

                    monsterDifficulty = 3;
                    break;

                case "medium":

                    monsterDifficulty = 2;
                    break;

            }

            int blessingLevel = 1;

            if (blessingList.ContainsKey("water"))
            {
                blessingLevel = 2;
            }

            if (blessingList.ContainsKey("stars"))
            {
                blessingLevel = 3;
            }

            int combatPower = monsterDifficulty + blessingLevel;

            combatModifier = Math.Min(Game1.player.CombatLevel + 5, 15) * (int)Math.Pow(combatPower, 2);


        }

        public void CastVector()
        {

            switch (castType)
            {

                case "water":

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

            int castIndex = (castLevel % 4) + 1;

            Vector2 castPosition = castVector * 64;

            float castLimit = (castIndex * 128) + 32f;

            if (castType == "stars")
            {
                castLimit = 720;

            }

            if (castType == "fates")
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

                if (ChooseSource() == -1)
                {

                    //Game1.addHUDMessage(new("Unable to cast without essence",""));

                    Mod.instance.CastMessage("Some Rite of the Fate effects require essence");

                    //return;

                }

            }

            if (castLocation.characters.Count > 0)
            {

                foreach (NPC riteWitness in castLocation.characters)
                {

                    if (riteWitness is StardewValley.Monsters.Monster)
                    {
                        continue;
                    }

                    if(riteWitness is StardewDruid.Character.Character)
                    {
                        continue;
                    }

                    int source = -1;

                    if (castType == "fates")
                    {
                        
                        source = ChooseSource();

                        if (source == -1) { continue; }

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

                        if (riteWitness.isVillager())
                        {

                            if (castType == "stars")
                            {

                                riteWitness.doEmote(8);

                                Game1.addHUDMessage(new HUDMessage($"{riteWitness.Name} could get hurt by the impacts!", 3));

                                return;

                            }
                            else if (castType == "water")
                            {

                                riteWitness.doEmote(16);

                                Game1.addHUDMessage(new HUDMessage($"{riteWitness.Name} is disturbed by the storm cloud", 2));

                            }
                            else if (castType == "fates" && blessingList["fates"] >= 2)
                            {

                                Cast.Fates.Trick villagerCast = new(castVector, this, riteWitness, source);

                                villagerCast.CastEffect();

                                ConsumeSource(source);

                            }
                            else
                            {

                                Cast.Earth.Villager villagerCast = new(castVector, this, riteWitness);

                                villagerCast.CastEffect();

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

                    Cast.Earth.Animal animalCast = new(castVector, this, pair.Value);

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

                    Cast.Earth.Animal animalCast = new(castVector, this, pair.Value);

                    animalCast.CastEffect();


                }

            }

            if (castBuffs)
            {

                Buff magnetBuff = new("Druidic Magnetism", 6000, "Rite of the " + castType, 8);

                magnetBuff.buffAttributes[8] = 128;

                magnetBuff.which = 184651;

                if (!Game1.buffsDisplay.hasBuff(184651))
                {

                    Game1.buffsDisplay.addOtherBuff(magnetBuff);

                }

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

                case "water":
                    
                    ModUtility.AnimateWaterCast(castVector, castLevel);
                    
                    CastWater(); 
                    
                    break;

                case "fates":

                    CastFates();

                    break;

                default: //CastEarth
                    
                    ModUtility.AnimateEarthCast(castLevel);
                    
                    CastEarth();

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

        public void CastEarth()
        {

            int chargeFactor = castLevel % 4;

            int chargeLevel = (chargeFactor * 2) + 1;

            Layer backLayer = castLocation.Map.GetLayer("Back");

            Layer buildingLayer = castLocation.Map.GetLayer("Buildings");

            int blessingLevel = blessingList["earth"];

            string locationName = castLocation.Name;

            //---------------------------------------------
            // Weed destruction
            //---------------------------------------------

            if (castLocation.objects.Count() > 0 && blessingLevel >= 1 && spawnIndex["weeds"])
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

                                    effectCasts[tileVector] = new Cast.Earth.Weed(tileVector, this);

                                }

                            }
                            else if (tileObject.name.Contains("Weeds") || tileObject.name.Contains("Twig"))
                            {

                                effectCasts[tileVector] = new Cast.Earth.Weed(tileVector, this);

                            }
                            else if (castLocation is MineShaft && tileObject is BreakableContainer)
                            {

                                effectCasts[tileVector] = new Cast.Earth.Weed(tileVector, this);

                            }

                        }

                        continue;

                    }

                }

            }

            if (castLocation is MineShaft || castLocation is VolcanoDungeon)
            {

                if (blessingLevel >= 5)
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

            if (castLocation.largeTerrainFeatures.Count > 0 && blessingLevel >= 2)
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

                        effectCasts[featureVector] = new Cast.Earth.Bush(featureVector, this, bushFeature);
                       
                        Mod.instance.featureCasts[locationName].Add(featureVector, 1);

                    }

                }

            }

            if (castLocation.resourceClumps.Count > 0 && blessingLevel >= 2)
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

                                effectCasts[featureVector] = new Cast.Earth.Stump(featureVector, this, resourceClump, "Farm");

                                break;

                            default:

                                effectCasts[featureVector] = new Cast.Earth.Boulder(featureVector, this, resourceClump);

                                break;

                        }
                        Mod.instance.featureCasts[locationName][featureVector] = 1;

                    }

                }

            }

            if (castLocation is Woods woodyLocation && blessingLevel >= 2)
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

                        effectCasts[featureVector] = new Cast.Earth.Stump(featureVector, this, resourceClump, "Woods");

                        Mod.instance.featureCasts[locationName][featureVector] = 1;

                    }

                }

            }

            // ---------------------------------------------
            // Random Effect Center Selection
            // ---------------------------------------------

            List<Vector2> centerVectors = new();
            
            int castAttempt = castLevel % 4;

            if(castLevel % 4 == 0)
            {

                centerVectors.Add(castVector);

            }

            //List<Vector2> castSelection = ModUtility.GetTilesWithinRadius(castLocation, castVector, castAttempt + 3); // 1, 3, 5, 7
            List<Vector2> castSelection = ModUtility.GetTilesWithinRadius(castLocation, castVector, (castAttempt * 2) + 2); // 2, 4, 6, 8

            if (randomIndex.Next(2) == 0) { castSelection.Reverse(); } // clockwise iteration can slightly favour one side

            int castSelect = castSelection.Count; // 16, 24, 28, 32 // 12, 24, 32, 32

            if (castSelect != 0)
            {

                List<int> segmentList = new() // 2, 3, 4, 4 // 1, 2, 3, 4
                {
                    //8, 8, 7, 8,
                    12, 12, 10, 8,
                };

                int castSegment = segmentList[castAttempt];

                List<int> cycleList = new()
                {
                   //2, 3, 4, 4,
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

            //foreach (Vector2 centerVector in centerVectors)
            for (int v = 0; v < centerVectors.Count; v++)
            {

                Vector2 centerVector = centerVectors[v];

                Dictionary<int, List<Vector2>> tileVectors = new();

                for (int i = 0; i < castRadius ; i++)
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

                        Tile buildingTile = buildingLayer.PickTile(new Location(tileX * 64, tileY * 64), Game1.viewport.Size);

                        if (buildingTile != null)
                        {

                            if (castLocation is Farm farmLocation)
                            {
                                int tileIndex = buildingTile.TileIndex;

                                if (tileIndex == 1938)
                                {
                                    effectCasts[tileVector] = new Cast.Earth.Bowl(tileVector, this);
                                }

                            }

                            if (castLocation is Beach && !castToggle.ContainsKey("forgetFish"))
                            {

                                Vector2 terrainVector = new((int)(tileVector.X / 6), (int)(tileVector.Y / 6));

                                if (Mod.instance.terrainCasts[locationName].ContainsKey(terrainVector))
                                {
                                    continue;
                                }

                                List<int> tidalList = new() { 60, 61, 62, 63, 77, 78, 79, 80, 94, 95, 96, 97, 104, 287, 288, 304, 305, 321, 362, 363 };

                                if (tidalList.Contains(buildingTile.TileIndex))
                                {

                                    effectCasts[tileVector] = new Cast.Earth.Pool(tileVector, this);

                                }

                                Mod.instance.terrainCasts[locationName][terrainVector] = "Pool";

                            }

                            Mod.instance.targetCasts[locationName][tileVector] = "Building";

                            continue;


                        }

                        if (castLocation.terrainFeatures.ContainsKey(tileVector))
                        {

                            if (blessingLevel >= 2)
                            {

                                TerrainFeature terrainFeature = castLocation.terrainFeatures[tileVector];

                                switch (terrainFeature.GetType().Name.ToString())
                                {

                                    case "FruitTree":

                                        StardewValley.TerrainFeatures.FruitTree fruitFeature = terrainFeature as StardewValley.TerrainFeatures.FruitTree;

                                        if (fruitFeature.growthStage.Value >= 4)
                                        {

                                            effectCasts[tileVector] = new Cast.Earth.FruitTree(tileVector, this);

                                        }
                                        else if (blessingList["earth"] >= 4)
                                        {

                                            effectCasts[tileVector] = new Cast.Earth.FruitSapling(tileVector, this);

                                        }

                                        Mod.instance.targetCasts[locationName][tileVector] = "Tree";

                                        break;

                                    case "Tree":

                                        StardewValley.TerrainFeatures.Tree treeFeature = terrainFeature as StardewValley.TerrainFeatures.Tree;

                                        if (treeFeature.growthStage.Value >= 5)
                                        {

                                            effectCasts[tileVector] = new Cast.Earth.Tree(tileVector, this);

                                            Mod.instance.targetCasts[locationName][tileVector] = "Tree";

                                        }
                                        else if (blessingList["earth"] >= 4 && treeFeature.fertilized.Value == false)
                                        {

                                            effectCasts[tileVector] = new Cast.Earth.Sapling(tileVector, this);

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

                                        if (blessingList["earth"] >= 4)
                                        {

                                            if (spawnIndex["cropseed"])
                                            {

                                                effectCasts[tileVector] = new Cast.Earth.Crop(tileVector, this);

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

                            effectCasts[tileVector] = new Cast.Earth.Grass(tileVector, this);

                            Mod.instance.terrainCasts[locationName][terrainVector] = "Grass";

                            continue;

                        }

                        int tileX = (int)tileVector.X;

                        int tileY = (int)tileVector.Y;

                        Tile backTile = backLayer.PickTile(new Location(tileX * 64, tileY * 64), Game1.viewport.Size);

                        if (backTile == null)
                        {

                            Mod.instance.targetCasts[locationName][tileVector] = "Empty";

                            continue;

                        }

                        if (castLocation is AnimalHouse)
                        {

                            if (backTile.TileIndexProperties.TryGetValue("Trough", out _))
                            {

                                effectCasts[tileVector] = new Cast.Earth.Trough(tileVector, this);

                                Mod.instance.targetCasts[locationName][tileVector] = "Trough";

                                continue;

                            }

                        }

                        Dictionary<string, List<Vector2>> neighbourList = ModUtility.NeighbourCheck(castLocation, tileVector);

                        if (neighbourList.Count > 0)
                        {

                            continue;

                        }

                        if (backTile.TileIndexProperties.TryGetValue("Water", out _))
                        {

                            if (blessingLevel >= 2)
                            {

                                if (spawnIndex["fishup"] && !castToggle.ContainsKey("forgetFish"))
                                {

                                    if (castLocation.Name.Contains("Farm"))
                                    {

                                        effectCasts[tileVector] = new Cast.Earth.Pool(tileVector, this);

                                    }
                                    else
                                    {

                                        effectCasts[tileVector] = new Cast.Earth.Water(tileVector, this);

                                    }

                                    Mod.instance.terrainCasts[locationName][terrainVector] = "Water";

                                }

                            }

                            continue;

                        }

                        if (blessingLevel >= 3)
                        {

                            if (backTile.TileIndexProperties.TryGetValue("Type", out var typeValue))
                            {

                                if (typeValue == "Dirt" || backTile.TileIndexProperties.TryGetValue("Diggable", out _))
                                {

                                    effectCasts[tileVector] = new Cast.Earth.Dirt(tileVector, this);

                                    Mod.instance.terrainCasts[locationName][terrainVector] = "Dirt";

                                    continue;

                                }

                                if (typeValue == "Grass" && backTile.TileIndexProperties.TryGetValue("NoSpawn", out _) == false)
                                {

                                    effectCasts[tileVector] = new Cast.Earth.Lawn(tileVector, this);

                                    Mod.instance.terrainCasts[locationName][terrainVector] = "Lawn";

                                    continue;

                                }

                            }

                        }

                    }

                }

                if (castLevel % 4 == 0 && v == 0)
                {

                    ModUtility.AnimateRadiusDecoration(castLocation, centerVector, "Earth", 0.75f, 0.5f, 2400f);

                }
                else if (castLevel % 4 == 2)
                {

                    ModUtility.AnimateRadiusDecoration(castLocation, centerVector, "Earth", 0.5f, 0.5f, 2400f);

                }
                else
                {

                    ModUtility.AnimateSprout(castLocation, centerVector);

                }

            }

        }

        public void CastRockfall()
        {

            if (blessingList["earth"] < 5)
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

                    effectCasts[newVector] = new Cast.Earth.Rockfall(newVector, this);

                }

            }

            int specialChance = Mod.instance.rockCasts[castLocation.Name];

            Mod.instance.rockCasts[castLocation.Name] = Math.Min(specialChance + 1, 50);

        }

        public void CastWater()
        {

            int chargeLevel = (castLevel % 4) + 1;

            int blessingLevel = blessingList["water"];

            List<Vector2> centerVectors = new();

            // ---------------------------------------------
            // Map Feature Iteration
            // ---------------------------------------------

            string locationName = castLocation.Name.ToString();

            float castLimit = chargeLevel * 2.25f;

            Vector2 warpVector = Map.WarpData.WarpVectors(castLocation);

            if (warpVector != Vector2.One && !Mod.instance.warpCasts.Contains(locationName))
            {

                if(Vector2.Distance(castVector, warpVector) <= castLimit)
                {
                    
                    //if (Mod.instance.warpCasts.Contains(locationName))
                    //{

                    //    Game1.addHUDMessage(new HUDMessage($"Already extracted {locationName} warp power today", 3));

                    //}
                    //else
                    //{
                        
                        int targetIndex = Map.WarpData.WarpTotems(castLocation);

                        effectCasts[warpVector] = new Cast.Water.Totem(warpVector, this, targetIndex);

                        Mod.instance.warpCasts.Add(locationName);

                        centerVectors.Add(warpVector);

                    //}

                }

            }

            Vector2 fireVector = Map.FireData.FireVectors(castLocation);

            if (fireVector != Vector2.One && !Mod.instance.fireCasts.Contains(locationName))
            {
                
                if (Vector2.Distance(castVector, fireVector) <= castLimit)
                {

                    //if (Mod.instance.fireCasts.Contains(locationName))
                    //{

                     //   Game1.addHUDMessage(new HUDMessage($"Already ignited {locationName} camp fire today", 3));

                    //}
                    //else
                    //{

                        effectCasts[fireVector] = new Cast.Water.Campfire(fireVector, this);

                        Mod.instance.fireCasts.Add(locationName);

                        centerVectors.Add(fireVector);

                    //}

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

                                effectCasts[featureVector] = new Cast.Water.Stump(featureVector, this, resourceClump, "Farm");

                                break;

                            default:

                                effectCasts[featureVector] = new Cast.Water.Boulder(featureVector, this, resourceClump);

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

                        effectCasts[featureVector] = new Cast.Water.Stump(featureVector, this, resourceClump, "Woods");

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

                        effectCasts[featureVector] = new Cast.Water.Stump(featureVector, this, forestLocation.log, "Log");

                        centerVectors.Add(featureVector);

                    }

                }


            }

            // ---------------------------------------------
            // Water effect
            // ---------------------------------------------

            if (blessingLevel >= 3 && chargeLevel == 1)
            {

                if (spawnIndex["fishspot"])
                {

                    if (ModUtility.WaterCheck(castLocation, castVector))
                    {

                        effectCasts[castVector] = new Cast.Water.Water(castVector, this);

                        centerVectors.Add(castVector);

                    }

                }

                if (castLocation is VolcanoDungeon volcanoLocation)
                {
                    int tileX = (int)castVector.X;
                    int tileY = (int)castVector.Y;

                    if (volcanoLocation.waterTiles[tileX, tileY] && !volcanoLocation.cooledLavaTiles.ContainsKey(castVector))
                    {

                        effectCasts[castVector] = new Cast.Water.Lava(castVector, this);

                        centerVectors.Add(castVector);

                    }

                }

            }

            // ---------------------------------------------
            // Monster iteration
            // ---------------------------------------------

            if (blessingLevel >= 4)
            {

                int smiteCount = 0;

                Vector2 castPosition = castVector * 64;

                float castOffset = (chargeLevel * 128) + 32f;

                float castThreshold = Math.Max(0, castOffset - 320f);

                foreach (NPC nonPlayableCharacter in castLocation.characters)
                {

                    if (nonPlayableCharacter is StardewValley.Monsters.Monster monsterCharacter)
                    {

                        float monsterDifference = Vector2.Distance(monsterCharacter.Position, castPosition);

                        if (monsterDifference < castOffset && monsterDifference > castThreshold)
                        {

                            Vector2 monsterVector = monsterCharacter.getTileLocation();

                            effectCasts[monsterVector] = new Cast.Water.Smite(monsterVector, this, monsterCharacter);

                            centerVectors.Add(monsterVector);

                            smiteCount++;

                            break;

                        }

                    }

                    if (smiteCount == chargeLevel)
                    {
                        break;
                    }

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

                foreach (Vector2 tileVector in tileVectors)
                {

                    if (castLocation.objects.Count() > 0)
                    {

                        if (castLocation.objects.ContainsKey(tileVector))
                        {

                            StardewValley.Object targetObject = castLocation.objects[tileVector];

                            if (castLocation.IsFarm && targetObject.bigCraftable.Value && targetObject.ParentSheetIndex == 9)
                            {

                                if (Mod.instance.warpCasts.Contains("rod"))
                                {

                                    //Game1.addHUDMessage(new HUDMessage("Already powered a lightning rod today", 3));

                                }
                                else if (blessingLevel >= 2)
                                {

                                    effectCasts[tileVector] = new Cast.Water.Rod( tileVector, this);

                                    Mod.instance.warpCasts.Add("rod");

                                    centerVectors.Add(tileVector);

                                }

                            }
                            else if (targetObject.Name.Contains("Campfire"))
                            {

                                string fireLocation = castLocation.Name;

                                if (Mod.instance.fireCasts.Contains(fireLocation))
                                {

                                    //Game1.addHUDMessage(new HUDMessage($"Already ignited {fireLocation} camp fire today", 3));

                                }
                                else if (blessingLevel >= 2)
                                {

                                    effectCasts[tileVector] = new Cast.Water.Campfire( tileVector, this);

                                    Mod.instance.fireCasts.Add(fireLocation);

                                    centerVectors.Add(tileVector);

                                }
                            }
                            else if (targetObject is Torch && targetObject.ParentSheetIndex == 93) // crafted candle torch
                            {

                                if (blessingLevel >= 5 && !Mod.instance.eventRegister.ContainsKey("active"))
                                {
                                    if (spawnIndex["portal"])
                                    {

                                        effectCasts[tileVector] = new Cast.Water.Portal( tileVector, this);

                                        centerVectors.Add(tileVector);

                                    }

                                }

                            }
                            else if (targetObject.IsScarecrow())
                            {

                                string scid = "scarecrow_" + tileVector.X.ToString() + "_" + tileVector.Y.ToString();

                                if (blessingLevel >= 2 && !Game1.isRaining && !Mod.instance.warpCasts.Contains(scid))
                                {

                                    effectCasts[tileVector] = new Cast.Water.Scarecrow( tileVector, this,Math.Max(4, Mod.instance.virtualCan.UpgradeLevel));

                                    Mod.instance.warpCasts.Add(scid);

                                    centerVectors.Add(tileVector);

                                }

                            }
                            else if (targetObject.Name.Contains("Artifact Spot") && Mod.instance.virtualHoe.UpgradeLevel >= 3)
                            {

                                effectCasts[tileVector] = new Cast.Water.Artifact( tileVector, this);

                                centerVectors.Add(tileVector);

                            }

                            continue;

                        }

                    }

                    if (castLocation.terrainFeatures.ContainsKey(tileVector))
                    {

                        if (blessingLevel >= 1)
                        {

                            if (castLocation.terrainFeatures[tileVector] is StardewValley.TerrainFeatures.Tree treeFeature)
                            {

                                if (treeFeature.stump.Value)
                                {

                                    effectCasts[tileVector] = new Cast.Water.Tree( tileVector, this);

                                    centerVectors.Add(tileVector);

                                }

                            }

                        }

                        continue;

                    }

                }

            }

            if(centerVectors.Count == 0)
            {

                centerVectors.Add(castVector);

            }

            foreach(var centerVector in centerVectors)
            {

                ModUtility.AnimateRadiusDecoration(castLocation, centerVector, "Water", 1f, 0.5f, 1000f);

            }

        }

        public void CastStars()
        {

            if (blessingList["stars"] < 1)
            {

                return;

            }

            int castAttempt = (castLevel % 6);

            List<Vector2> meteorVectors = new();

            List<Vector2> castSelection = ModUtility.GetTilesWithinRadius(castLocation, castVector, castAttempt+2); // 2,3,4,5,6,7

            if (randomIndex.Next(2) == 0) { castSelection.Reverse(); }

            int castSelect = castSelection.Count; // 12, 16, 24, 28, 32, 28

            if (castSelect == 0)
            {

                return;

            }

            List<int> segmentList = new() // 1, 1, 2, 3, 3, 4
            {
                12, 16, 12, 9, 11, 7
            };

            int castSegment = segmentList[castAttempt];

            List<int> cycleList = new()
            {
                1, 1, 2, 3, 3, 4
            };

            int castCycle = cycleList[castAttempt];

            int castIndex;

            Vector2 newVector;

            int addedRange = 1;

            if (Mod.instance.virtualAxe.UpgradeLevel >= 3 && Mod.instance.virtualPick.UpgradeLevel >= 3)
            {

                addedRange++;

            }

            /*if (Mod.instance.virtualPick.UpgradeLevel >= 3)
            {

                addedRange++;

            }*/

            int damageRadius = 1 + addedRange;

            //int damageRadius = 1 + addedRange;

            for (int k = 0; k < castCycle; k++)
            {

                int castLower = castSegment * k;

                if(castLower + 2 >= castSelect)
                {

                    continue;

                }

                int castHigher = Math.Min(castLower + castSegment, castSelection.Count);

                bool priorityCast = false;

                if (castLocation.objects.Count() > 0 && castTask.ContainsKey("masterMeteor"))
                {

                    for (int j = castLower; j < castHigher; j++)
                    {

                        newVector = castSelection[j];

                        if (castLocation.objects.ContainsKey(newVector))
                        {

                            StardewValley.Object tileObject = castLocation.objects[newVector];

                            if (tileObject.name.Contains("Stone"))
                            {

                                effectCasts[newVector] = new Cast.Stars.Meteor( newVector, this, damageRadius);

                                meteorVectors.Add(newVector);

                                priorityCast = true;

                                break;

                            }

                        }

                    }

                    if (priorityCast)
                    {

                        continue;

                    }

                }

                if (!castTask.ContainsKey("masterMeteor"))
                {

                    Mod.instance.UpdateTask("lessonMeteor", 1);

                }

                castIndex = randomIndex.Next(castLower, castHigher);

                newVector = castSelection[castIndex];

                effectCasts[newVector] = new Cast.Stars.Meteor( newVector, this, damageRadius);

                meteorVectors.Add(newVector);

            }

            foreach(var vector in meteorVectors)
            {

                ModUtility.AnimateRadiusDecoration(castLocation, vector, "Stars", 0.75f + (addedRange * 0.25f), 1f, 1000);

            }

        }

        public void CastFates()
        {

            string locationName = castLocation.Name;

            int blessingLevel = blessingList["fates"];

            int useSource = ChooseSource();

            if (!Mod.instance.targetCasts.ContainsKey(locationName))
            {

                Mod.instance.targetCasts[locationName] = ModUtility.LocationTargets(castLocation);

            }

            // ---------------------------------------------
            // Escape
            // ---------------------------------------------

            for (int a = 0; a < 1; a++)
            {

                if (Mod.instance.eventRegister.ContainsKey("escape")) { break; }

                if (castLevel != 0) { break; }

                if (castLocation.warps.Count() <= 0) { if (castLocation is not MineShaft) { break; }  }

                Event.World.Escape escapeEvent = new(caster.Position, this);

                escapeEvent.EventTrigger();

            }

            // ---------------------------------------------
            // Gravity
            // ---------------------------------------------

            for (int a = 0; a < 1; a++)
            {

                if (blessingLevel < 4) { break; }

                if (!spawnIndex["gravity"]) { break; }

                if (castLocation.objects.Count() <= 0) { break; }

                if (castLevel % 2 == 1) { break; }

                if (Mod.instance.eventRegister.ContainsKey("gravity")) { break; }

                List<int> targetList = GetTargetCursor(caster.getTileLocation(), caster.FacingDirection, 5);

                Vector2 wellVector = new(targetList[1], targetList[2]);

                for (int i = 0; i < 3; i++)
                {

                    List<Vector2> wellVectors = ModUtility.GetTilesWithinRadius(castLocation, wellVector, i);

                    foreach (Vector2 tileVector in wellVectors)
                    {

                        if (!castLocation.objects.ContainsKey(tileVector)) { continue; }

                        StardewValley.Object targetObject = castLocation.objects[tileVector];

                        if (!targetObject.IsScarecrow()) { continue; }

                        string scid = "gravity_" + tileVector.X.ToString() + "_" + tileVector.Y.ToString();

                        if (Mod.instance.warpCasts.Contains(scid)) { continue; }

                        effectCasts[tileVector] = new Cast.Fates.Gravity(tileVector, this, 0);

                        Mod.instance.warpCasts.Add(scid);

                        return;

                    }

                }

            }

            // ---------------------------------------------
            // Gravity - Monster
            // ---------------------------------------------

            for (int a = 0; a < 1; a++)
            {
                if (blessingLevel < 4) { break; }

                if (castLocation.characters.Count <= 0) { break; }

                if (Mod.instance.eventRegister.ContainsKey("gravity")) { break; }

                List<int> targetList = GetTargetCursor(caster.getTileLocation(), caster.FacingDirection, 5);

                Vector2 wellVector = new(targetList[1], targetList[2]);

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
            // Enchant
            // ---------------------------------------------

            if (castLocation.objects.Count() > 0 && blessingList["fates"] >= 3)
            {

                int castAttempt = (castLevel % 8);

                List<Vector2> centerVectors = ModUtility.GetTilesWithinRadius(castLocation, castVector, castAttempt + 1); // 1, 2, 3, 4, 5, 6, 7, 8

                List<string> craftIndex = Map.SpawnData.MachineList();

                List<Vector2> objectVectors = new();

                foreach (Vector2 tileVector in centerVectors)
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

                }

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

                for (int i = whiskRange; i > 3; i--)
                {

                    Vector2 whiskDestiny = castVector + (whiskSegment * i);

                    if (i == whiskRange)
                    {
                        List<int> targetList = GetTargetCursor(caster.getTileLocation(), whiskDirection, 16, 8);

                        whiskDestiny = new(targetList[1], targetList[2]);
                    }

                    if(!ModUtility.GroundCheck(castLocation, whiskDestiny, false))
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

                    effectCasts[whiskDestiny] = new Cast.Fates.Whisk(castVector, this, whiskDestiny);

                    break;

                }

            }

            if(effectCasts.Count > 0 && Mod.instance.eventRegister.ContainsKey("escape"))
            {

                if(Mod.instance.eventRegister["escape"].activeCounter >= 2)
                {

                    Mod.instance.eventRegister["escape"].expireEarly = true;

                }

            }


        }

        public int ChooseSource()
        {

            if (castSource[768] == -1)
            {

                if (castSource[769] == -1)
                {   
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

    }

}