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
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Xml.Linq;
using xTile.Dimensions;

namespace StardewDruid.Event.World
{
    public class Gravity : EventHandle
    {

        public bool eventActive;

        public string gravityType;

        public bool gravityInitiated;

        public Vector2 gravityAnchor;

        public Vector2 gravityCorner;

        public Vector2 gravityCenter;

        public List<StardewValley.Monsters.Monster> gravityVictims;

        public Dictionary<int,TemporaryAnimatedSprite> gravityAnimations;

        public Gravity(Vector2 target, Rite rite, int type)
            : base(target, rite)
        {

            int extend = type == 0 ? 6 : 0;

            gravityType = type == 0 ? "Solar" : "Void";

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 6 + extend;

            //source = Source;

            gravityAnchor = targetVector * 64;

            //gravityCorner = gravityAnchor - new Vector2(32, 32);

            gravityCorner = gravityAnchor - new Vector2(64, 64);

            gravityCenter = targetVector * 64 + new Vector2(32, 48);

            gravityVictims = new();

            gravityAnimations = new();

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

            if(targetPlayer.currentLocation.Name != targetLocation.Name)
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
            
            if(gravityAnimations.Count > 0)
            {
                
                foreach(KeyValuePair<int,TemporaryAnimatedSprite> animation in gravityAnimations)
                {
                    
                    targetLocation.temporarySprites.Remove(animation.Value);
                
                }
                
            }

            gravityVictims.Clear();

            if (gravityAnimations.ContainsKey(2))
            {

                /*TemporaryAnimatedSprite GravityAnimation = new(0, 100f, 4, 1, gravityCorner, false, false)
                {

                    sourceRect = new(0, 64, 32, 32),

                    sourceRectStartingPos = new Vector2(0, 64),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Gravity" + gravityType + ".png")),

                    scale = 4f,

                    layerDepth = 0.0002f,

                };

                targetLocation.temporarySprites.Add(GravityAnimation);

                TemporaryAnimatedSprite nightAnimation = new(0, 400f, 1, 1, gravityCorner, false, false)
                {

                    sourceRect = new(0, 0, 32, 32),

                    sourceRectStartingPos = new Vector2(0, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Blackhole.png")),

                    scale = 4f,

                    layerDepth = 0.0001f,

                    rotationChange = -0.12f,
                };

                targetLocation.temporarySprites.Add(nightAnimation);*/

                TemporaryAnimatedSprite nightAnimation = new(0, 1000f, 1, 1, gravityCorner, false, false)
                {

                    sourceRect = new(0, 0, 64, 64),

                    sourceRectStartingPos = new Vector2(0, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Gravity.png")),

                    scale = 4f,

                    layerDepth = 0.0001f,

                    rotationChange = -0.06f,

                    alphaFade = 0.001f,

                    timeBasedMotion = true,

                    scaleChange = -0.002f,

                };

                targetLocation.temporarySprites.Add(nightAnimation);


            }

            gravityAnimations.Clear();

        }

        public override void EventDecimal()
        {

            if (gravityVictims.Count > 0)
            {

                for(int m = gravityVictims.Count - 1; m >= 0; m--)
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

                    float moveX = (difference.X < 0) ? -1 * speed * factorX : speed * factorX;

                    float moveY = (difference.Y < 0) ? -1 * speed * factorY : speed * factorY;

                    if (!victim.isGlider.Value)
                    {

                        Microsoft.Xna.Framework.Rectangle boundingBox = victim.GetBoundingBox();

                        boundingBox.X += (int)(moveX);
                        boundingBox.Y += (int)(moveY);

                        if (targetLocation.isCollidingPosition(boundingBox, Game1.viewport, isFarmer: false, 0, glider: false, victim, pathfinding: false))
                        {
                            continue;
                        }

                    }

                    victim.position.X += moveX;

                    victim.position.Y += moveY;

                    // targetAnimation

                    Microsoft.Xna.Framework.Rectangle box = victim.GetBoundingBox();

                    Point center = box.Center;

                    Vector2 position = center.ToVector2() + new Vector2(-16, 32);

                    WarpAnimation(position);

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

                /*TemporaryAnimatedSprite startAnimation = new(0, 100f, 4, 1, gravityCorner, false, false)
                {

                    sourceRect = new(0, 0, 32, 32),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Gravity" + gravityType + ".png")),

                    scale = 4f,

                    layerDepth = 0.0002f,

                };

                targetLocation.temporarySprites.Add(startAnimation);

                gravityAnimations[0] = startAnimation;

                TemporaryAnimatedSprite nightAnimation = new(0, 9999f, 1, 1, gravityCorner, false, false)
                {

                    sourceRect = new(0, 0, 32, 32),

                    sourceRectStartingPos = new Vector2(0, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Blackhole.png")),

                    scale = 4f,

                    layerDepth = 0.0001f,

                    rotationChange = -0.12f,

                };

                targetLocation.temporarySprites.Add(nightAnimation);

                gravityAnimations[1] = nightAnimation;

                TemporaryAnimatedSprite initialAnimation = new(0, 9999f, 1, 1, gravityCorner, false, false)
                {

                    sourceRect = new(0, 32, 32, 32),

                    sourceRectStartingPos = new Vector2(0, 32),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Gravity" + gravityType + ".png")),

                    scale = 4f,

                    delayBeforeAnimationStart = 400,

                    layerDepth = 0.0003f,

                    rotationChange = -0.08f,

                };

                targetLocation.temporarySprites.Add(initialAnimation);

                gravityAnimations[2] = initialAnimation;

                return;*/

                Vector2 targetPosition = gravityCorner;

                Vector2 playerPosition = riteData.caster.Position - new Vector2(0, 32);

                float xOffset = (targetPosition.X - playerPosition.X);

                float yOffset = (targetPosition.Y - playerPosition.Y);

                float motionX = xOffset / 1000;

                //float compensate = 0.555f;

                //float motionY = (yOffset / 1000) - compensate;
                float motionY = yOffset / 1000;

                float animationSort = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString()) + 2;

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

                };

                targetLocation.temporarySprites.Add(startAnimation);

                gravityAnimations[0] = startAnimation;

                TemporaryAnimatedSprite staticAnimation = new(0, 99999f, 1, 1, gravityCorner, false, false)
                {

                    sourceRect = new(0, 0, 64, 64),

                    sourceRectStartingPos = new Vector2(0, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Gravity.png")),

                    scale = 3f,

                    layerDepth = 0.0001f,

                    rotationChange = -0.06f,

                    timeBasedMotion = true,

                    delayBeforeAnimationStart = 1000,

                };

                targetLocation.temporarySprites.Add(staticAnimation);

                gravityAnimations[1] = staticAnimation;

                return;

            }

            // -------------------------------------------------
            // Crops
            // -------------------------------------------------

            int targetRadius = activeCounter - 1;

            List<Vector2> tileVectors = ModUtility.GetTilesWithinRadius(targetLocation, targetVector, targetRadius);

            foreach (Vector2 tileVector in tileVectors)
            {

                if (targetLocation.terrainFeatures.ContainsKey(tileVector))
                {

                    if (targetLocation.terrainFeatures[tileVector] is StardewValley.TerrainFeatures.HoeDirt hoeDirt)
                    {

                        if (hoeDirt.crop != null)
                        {
                            if (
                                (int)hoeDirt.crop.currentPhase.Value >= hoeDirt.crop.phaseDays.Count - 1 &&
                                (!hoeDirt.crop.fullyGrown.Value || (int)hoeDirt.crop.dayOfCurrentPhase.Value <= 0)
                                && !hoeDirt.crop.dead.Value
                                && (int)hoeDirt.crop.indexOfHarvest.Value != 0)
                            {

                                if (ExtractCrop(hoeDirt, hoeDirt.crop, tileVector))
                                {

                                    hoeDirt.destroyCrop(tileVector, false, targetLocation);

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

                    if(riteWitness is not StardewValley.Monsters.Monster monster)
                    {

                        continue;

                    }

                    if (gravityVictims.Contains(monster))
                    {
                        continue;
                    }

                    float pullDistance = Vector2.Distance(monster.Position, gravityCenter);

                    float pullLimit = 720f;

                    if (riteData.castTask.ContainsKey("masterGravity"))
                    {

                        pullLimit = 960f;

                    }

                    if (pullDistance >= pullLimit)
                    {
                        continue;
                    }

                    gravityVictims.Add(monster);

                    monster.Halt();

                    monster.stunTime = 1000 * (6 - activeCounter);

                    if (riteData.blessingList["fates"] >= 5)
                    {

                        for(int i = 0; i < 5;  i++)
                        {

                            string eventName = "daze" + i.ToString();

                            if (!Mod.instance.eventRegister.ContainsKey(eventName))
                            {

                                Event.World.Daze dazeEvent = new(targetVector,riteData,monster,i);

                                dazeEvent.EventTrigger();
                                
                                break;
                            
                            }

                        }

                    }

                }

            }

        }

        public bool ExtractCrop(HoeDirt soil, StardewValley.Crop crop, Vector2 tileVector)
        {

            int qualityMax = 0;
            
            int quantityMax = 0;

            if(crop.chanceForExtraCrops.Value > 0)
            {
                quantityMax++;
            }

            if (Game1.player.FarmingLevel >= 10)
            {
                quantityMax++;
            }

            if (Mod.instance.virtualHoe.UpgradeLevel >= 3)
            {
                quantityMax++;
            }

            if (soil.fertilizer.Value > 0)
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

            for(int i = 0; i < quantity; i++)
            {

                int quality = randomIndex.Next(0, 3 + qualityMax);

                if (quality >= 3) { quality = 4; }

                if ((int)crop.indexOfHarvest.Value == 771 || (int)crop.indexOfHarvest.Value == 889)
                {

                    quality = 0;

                }

                StardewValley.Object extract = (crop.programColored.Value ? new ColoredObject(crop.indexOfHarvest.Value, 1, crop.tintColor.Value)
                {

                    Quality = quality

                } : new StardewValley.Object(crop.indexOfHarvest.Value, 1, isRecipe: false, -1, quality));
                
                PopulateObject(extract,tileVector);

            }

            int num6 = Convert.ToInt32(Game1.objectInformation[crop.indexOfHarvest.Value].Split('/')[1]);

            float num7 = (float)(16.0 * Math.Log(0.018 * (double)num6 + 1.0, Math.E));

            Game1.player.gainExperience(0, (int)Math.Round(num7));

            if ((int)crop.regrowAfterHarvest.Value == -1)
            {

                return true;

            }

            crop.fullyGrown.Value = true;

            if (crop.dayOfCurrentPhase.Value == (int)crop.regrowAfterHarvest.Value)
            {

                crop.updateDrawMath(tileVector*64);

            }

            crop.dayOfCurrentPhase.Value = crop.regrowAfterHarvest.Value;

            return false;

        }

        public void PopulateObject(StardewValley.Object extract, Vector2 extractVector)
        {
            
            Cast.Throw throwObject = new(Game1.player, gravityCenter, extract, extractVector * 64);

            throwObject.itemDebris = true;

            throwObject.ThrowObject();
        
        }

        public void WarpAnimation(Vector2 position)
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
