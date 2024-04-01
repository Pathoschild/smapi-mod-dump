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
using StardewDruid.Event;
using StardewDruid.Map;
using StardewDruid.Monster.Boss;
using StardewValley;
using StardewValley.Characters;
using StardewValley.GameData.Crops;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;

namespace StardewDruid.Cast.Fates
{
    public class GravityEvent : EventHandle
    {

        public bool eventActive;

        public string gravityType;

        public bool gravityInitiated;

        public Vector2 gravityAnchor;

        public Vector2 gravityCorner;

        public Vector2 gravityCenter;

        public List<StardewValley.Monsters.Monster> gravityVictims;

        public Dictionary<int, TemporaryAnimatedSprite> gravityAnimations;

        public bool cometFall;

        public int cometTimer;

        public float damage;

        public GravityEvent(Vector2 target,  int type, float Damage)
            : base(target)
        {

            int extend = type == 0 ? 6 : 0;

            gravityType = type == 1 ? "Void" : "Solar";

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 6 + extend;

            gravityAnchor = targetVector * 64;

            gravityCorner = gravityAnchor - new Vector2(64, 64);

            gravityCenter = targetVector * 64 + new Vector2(32, 48);

            gravityVictims = new();

            gravityAnimations = new();

            damage = Damage;

        }

        public override void EventTrigger()
        {

            Mod.instance.RegisterEvent(this, "gravity");

        }

        public override bool EventActive()
        {

            if (activeCounter == 9) // gravity limit
            {
                return false;

            }

            if (expireEarly)
            {

                return false;

            }

            if (targetPlayer.currentLocation.Name != targetLocation.Name)
            {

                return false;

            }

            if (expireTime <= Game1.currentGameTime.TotalGameTime.TotalSeconds)
            {

                return false;

            }

            return true;

        }

        public override void EventAbort()
        {

        }

        public override void EventRemove()
        {

            GravityEnd();

        }

        public void GravityEnd()
        {

            if (gravityAnimations.Count > 0)
            {

                foreach (KeyValuePair<int, TemporaryAnimatedSprite> animation in gravityAnimations)
                {

                    targetLocation.temporarySprites.Remove(animation.Value);

                }

            }

            gravityVictims.Clear();

            if (gravityAnimations.ContainsKey(2))
            {

                TemporaryAnimatedSprite nightAnimation = new(0, 1000f, 1, 1, gravityCorner, false, false)
                {

                    sourceRect = new(0, 0, 64, 64),

                    sourceRectStartingPos = new Vector2(0, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Gravity.png")),

                    scale = 3f,

                    layerDepth = 0.0001f,

                    rotationChange = -0.06f,

                    alphaFade = 0.001f,

                    timeBasedMotion = true,

                    motion = new(0.064f,0.064f),

                    scaleChange = -0.002f,

                    alpha = 0.75f,

                };

                targetLocation.temporarySprites.Add(nightAnimation);


            }

            gravityAnimations.Clear();

        }

        public override void EventDecimal()
        {

            if (gravityVictims.Count > 0)
            {

                for (int m = gravityVictims.Count - 1; m >= 0; m--)
                {

                    StardewValley.Monsters.Monster victim = gravityVictims[m];

                    if (!ModUtility.MonsterVitals(victim, targetLocation))
                    {

                        gravityVictims.RemoveAt(m);

                        continue;

                    }

                    Vector2 difference = gravityCenter - victim.Position;

                    float distance = Vector2.Distance(gravityCenter, victim.Position);

                    if (distance <= 32f)
                    {

                        continue;

                    }

                    float factorX = 1f;

                    float factorY = 1f;

                    if (Math.Abs(difference.X) > Math.Abs(difference.Y))
                    {
                        factorY = difference.Y / difference.X;
                    }
                    else
                    {
                        factorX = difference.X / difference.Y;
                    }

                    int speed = 12 + victim.Speed;

                    float moveX = difference.X < 0 ? -1 * speed * factorX : speed * factorX;

                    float moveY = difference.Y < 0 ? -1 * speed * factorY : speed * factorY;

                    if (!victim.isGlider.Value)
                    {

                        Rectangle boundingBox = victim.GetBoundingBox();

                        boundingBox.X += (int)moveX;
                        boundingBox.Y += (int)moveY;

                        if (targetLocation.isCollidingPosition(boundingBox, Game1.viewport, isFarmer: false, 0, glider: false, victim, pathfinding: false))
                        {
                            continue;
                        }

                    }

                    victim.position.X += moveX;

                    victim.position.Y += moveY;

                    // targetAnimation

                    Rectangle box = victim.GetBoundingBox();

                    Point center = box.Center;

                    Vector2 position = center.ToVector2() + new Vector2(-16, 32);

                    TargetAnimation(position);

                }

            }

        }

        public override void EventInterval()
        {

            activeCounter++;

            // -------------------------------------------------
            // Animation
            // -------------------------------------------------

            if (activeCounter == 1)
            {

                if (Mod.instance.rite.chargeActive && Mod.instance.rite.chargeType == "stars")
                {
                    
                    Mod.instance.rite.CastComet();

                }

                Vector2 targetPosition = gravityCorner;

                Vector2 playerPosition = Mod.instance.rite.caster.Position - new Vector2(0, 32);

                float xOffset = targetPosition.X - playerPosition.X;

                float yOffset = targetPosition.Y - playerPosition.Y;

                float motionX = xOffset / 1000;

                float motionY = yOffset / 1000;

                float animationSort = 0.001f;

                TemporaryAnimatedSprite startAnimation = new(0, 1000f, 1, 1, playerPosition, false, false)
                {

                    sourceRect = new(0, 0, 64, 64),

                    sourceRectStartingPos = new Vector2(0, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Gravity.png")),

                    scale = 1f,

                    scaleChange = 0.002f,

                    layerDepth = animationSort,

                    motion = new Vector2(motionX, motionY),

                    timeBasedMotion = true,

                    rotationChange = -0.06f,

                    alpha = 0.75f,

                };

                targetLocation.temporarySprites.Add(startAnimation);

                gravityAnimations[0] = startAnimation;

                TemporaryAnimatedSprite staticAnimation = new(0, 99999f, 1, 1, gravityCorner, false, false)
                {

                    sourceRect = new(0, 0, 64, 64),

                    sourceRectStartingPos = new Vector2(0, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Gravity.png")),

                    scale = 3f,

                    layerDepth = animationSort,

                    rotationChange = -0.06f,

                    timeBasedMotion = true,

                    delayBeforeAnimationStart = 1000,

                    alpha = 0.75f,

                };

                targetLocation.temporarySprites.Add(staticAnimation);

                gravityAnimations[1] = staticAnimation;

                TemporaryAnimatedSprite bandAnimation = new(0, 9999f,1,1, gravityCorner, false, false)
                {

                    sourceRect = new(64, 0, 64, 64),

                    sourceRectStartingPos = new Vector2(64, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Gravity.png")),

                    scale = 3f,

                    layerDepth = animationSort*2,

                    //rotationChange = -0.06f,

                    timeBasedMotion = true,

                    delayBeforeAnimationStart = 1000,

                    alpha = 0.75f,

                };

                targetLocation.temporarySprites.Add(bandAnimation);

                gravityAnimations[2] = bandAnimation;

                return;

            }

            // -------------------------------------------------
            // Crops
            // -------------------------------------------------

            int targetRadius = activeCounter - 1;

            List<Vector2> tileVectors = ModUtility.GetTilesWithinRadius(targetLocation, targetVector, targetRadius);

            if (targetRadius == 1) { tileVectors.Add(targetVector); }

            foreach (Vector2 tileVector in tileVectors)
            {

                if (targetLocation.terrainFeatures.ContainsKey(tileVector))
                {

                    if (targetLocation.terrainFeatures[tileVector] is HoeDirt hoeDirt)
                    {

                        if (hoeDirt.crop != null)
                        {
                            if (
                                hoeDirt.crop.currentPhase.Value >= hoeDirt.crop.phaseDays.Count - 1 &&
                                (!hoeDirt.crop.fullyGrown.Value || hoeDirt.crop.dayOfCurrentPhase.Value <= 0)
                                && !hoeDirt.crop.dead.Value
                                && hoeDirt.crop.indexOfHarvest.Value != null)
                            {

                                if (ExtractCrop(hoeDirt, hoeDirt.crop, tileVector))
                                {

                                    hoeDirt.destroyCrop(true);

                                }

                            }

                        }

                    }

                }

            }

            // -------------------------------------------------
            // Characters
            // -------------------------------------------------

            if (targetLocation.characters.Count > 0 && targetRadius != 0)
            {

                foreach (NPC riteWitness in targetLocation.characters)
                {

                    if (riteWitness is not StardewValley.Monsters.Monster monster)
                    {

                        continue;

                    }

                    if (gravityVictims.Contains(monster))
                    {

                        continue;

                    }

                    float pullDistance = Vector2.Distance(monster.Position, gravityCenter);

                    float pullLimit = 516f;

                    if (pullDistance >= pullLimit)
                    {
                        continue;
                    }

                    gravityVictims.Add(monster);

                    if (!MonsterData.BossMonster(monster))
                    {

                        monster.Halt();

                        monster.stunTime.Set(1000 * (6 - activeCounter));

                    }

                    if (Mod.instance.CurrentProgress() >= 25)
                    {

                        for (int i = 0; i < 5; i++)
                        {

                            string eventName = "daze" + i.ToString();

                            if (!Mod.instance.eventRegister.ContainsKey(eventName))
                            {

                                Daze dazeEvent = new(targetVector, monster, i, damage);

                                dazeEvent.EventTrigger();

                                break;

                            }

                        }

                    }

                }

            }

        }

        public bool ExtractCrop(HoeDirt soil, Crop crop, Vector2 tileVector)
        {

            int qualityMax = 0;

            int quantityMax = 0;

            CropData data = crop.GetData();

            if(data != null)
            {

                if (data.ExtraHarvestChance > 0)
                {
                    quantityMax++;
                }

                if (data.HarvestMaxIncreasePerFarmingLevel > 0f)
                {
                    quantityMax++;
                }

                if(data.HarvestMaxStack > 1)
                {
                    quantityMax++;
                }

            }

            if (Game1.player.FarmingLevel >= 10)
            {
                quantityMax++;
            }

            if (Mod.instance.virtualHoe.UpgradeLevel >= 3)
            {
                quantityMax++;
            }

            if (soil.HasFertilizer())
            {
                qualityMax++;
            }

            if (Game1.player.FarmingLevel >= 5)
            {
                qualityMax++;
            }

            if (Mod.instance.virtualCan.UpgradeLevel >= 3)
            {
                qualityMax++;
            }

            Game1.player.currentLocation.playSound("harvest");

            int quantity = randomIndex.Next(1, 2 + quantityMax);

            StardewValley.Object extract = null;

            for (int i = 0; i < quantity; i++)
            {

                int quality = randomIndex.Next(0, 3 + qualityMax);

                if (quality >= 3) { quality = 4; }

                if (crop.indexOfHarvest.Value.Contains("771") || crop.indexOfHarvest.Value.Contains("889"))
                {

                    quality = 0;

                }

                extract = crop.programColored.Value ? new ColoredObject(crop.indexOfHarvest.Value, 1, crop.tintColor.Value)
                {

                    Quality = quality

                } : new StardewValley.Object(crop.indexOfHarvest.Value, 1, isRecipe: false, -1, quality);

                if(extract.Category == -80) // abort if flower
                {

                    return false;

                }

                PopulateObject(extract, tileVector);

            }

            if(extract != null)
            {

                int num6 = extract.Price;

                float num7 = (float)(16.0 * Math.Log(0.018 * num6 + 1.0, Math.E));

                Game1.player.gainExperience(0, (int)Math.Round(num7));

            }

            int num8 = data?.RegrowDays ?? (-1);

            if (num8 <= 0)
            {
                return true;
            }

            crop.fullyGrown.Value = true;

            if (crop.dayOfCurrentPhase.Value == num8)
            {
                
                crop.updateDrawMath(tileVector * 64);

            }

            crop.dayOfCurrentPhase.Value = num8;

            return false;

        }

        public void PopulateObject(StardewValley.Object extract, Vector2 extractVector)
        {

            Throw throwObject = new(Game1.player, gravityCenter, extract, extractVector * 64);

            throwObject.itemDebris = true;

            throwObject.ThrowObject();

        }

        public void TargetAnimation(Vector2 position)
        {

            TemporaryAnimatedSprite warpTarget = new(0, 100f, 1, 1, position, false, false)
            {

                sourceRect = new(0, 0, 32, 32),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Target.png")),

                scale = 1.5f,

                layerDepth = 0.0001f,

                alpha = 0.75f,

                color = new(0.9f, 0.6f, 0.1f, 1f),

                rotation = (float)Math.PI,

            };

            targetLocation.temporarySprites.Add(warpTarget);

        }

    }

}
