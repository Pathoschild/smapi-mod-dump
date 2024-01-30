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
using StardewValley;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;
using StardewValley.Monsters;
using StardewDruid.Cast;
using StardewDruid.Map;
using System.Linq;
using StardewDruid.Event.World;

namespace StardewDruid.Event
{
    public class BarrageHandle
    {

        public GameLocation location;

        public Vector2 destination;

        public Vector2 origin;

        public int missiles;

        public int counter;

        public int radius;

        public float damageFarmers;

        public float damageMonsters;

        public int damageEnvironment;

        public int damageDirt;

        public List<TemporaryAnimatedSprite> animations;

        public Dictionary<int, List<Vector2>> impacts;

        public string barrageColor;

        public StardewValley.Monsters.Monster monster;

        public enum barrageType
        {
            missile,
            beam,
            burn,
            fireball
        }

        public barrageType type;

        public BarrageHandle(GameLocation Location, Vector2 Destination, Vector2 Origin, int Radius = 3, int Missiles = 1, string Color = "Blue", float Farmers = -1, float Monsters = -1, int Environment = -1, int Dirt = -1)
        {

            location = Location;

            destination = Destination;

            origin = Origin;

            radius = Radius;

            damageFarmers = Farmers;

            damageMonsters = Monsters;

            damageEnvironment = Environment;

            damageDirt = Dirt;

            missiles = Missiles;

            animations = new();

            impacts = new() { [0] = new(){destination,} };

            barrageColor = Color;

            type = barrageType.missile;

        }

        public bool Update()
        {

            counter++;

            switch (type)
            {

                case barrageType.missile:

                    if (counter == 1)
                    {

                        TargetCircle(2);

                        LaunchBarrage();

                        return true;

                    }

                    if (counter == 60)
                    {

                        MissileBarrage();

                        return true;

                    }

                    if (counter == 120)
                    {

                        RadialDamage();

                        LightRadius();

                        return true;

                    }

                    if (counter == 180)
                    {

                        Shutdown();

                        return false;

                    }


                    return true;

                case barrageType.burn:

                    if (counter == 1)
                    {

                        BurnCircle();

                        BurnImpacts();

                        LightRadius();

                        return true;

                    }

                    if (counter == 180)
                    {

                        Shutdown();

                        return false;

                    }

                    if (counter % 60 == 0)
                    {

                        BurnImpacts();

                        LightRadius();

                    }

                    return true;

                case barrageType.fireball:

                    if (counter == 1)
                    {

                        TargetCircle();
                        return true;
                    }

                    if (counter == 30)
                    {

                        LaunchFireball(2);

                        return true;
                    }

                    if (counter == 50)
                    {

                        RadialDamage(1);

                        LightRadius(1);

                        return true;

                    }

                    if (counter == 70)
                    {
                        RadialDamage();

                        LightRadius();

                        return true;

                    }

                    if (counter == 120)
                    {

                        Shutdown();

                        return false;
                    }
                 
                    return true;

                case barrageType.beam:

                    if (counter == 1)
                    {

                        LaunchBeam();

                        return true;

                    }

                    if (counter == 30)
                    {

                        FadeBeam();

                        RadialDamage();

                        LightRadius();

                        return true;
                    }

                    if (counter == 90)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;

            }

            return true;
        }

        public void Shutdown()
        {

            if (animations.Count > 0)
            {

                foreach (TemporaryAnimatedSprite animatedSprite in animations)
                {

                    location.temporarySprites.Remove(animatedSprite);

                }

            }

        }

        public void OuterCircle(int duration = 1)
        {
            
            TemporaryAnimatedSprite outerCircle = new(0, 1000f * duration, 1, 1, destination * 64 - new Vector2(64 * radius, 64 * radius) + new Vector2(32, 32), false, false)
            {

                sourceRect = new(0, 0, 64, 64),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", barrageColor + "Target.png")),

                scale = 2f * radius,

                rotationChange = 0.06f,

                alpha = 0.25f,

            };

            location.temporarySprites.Add(outerCircle);

            animations.Add(outerCircle);

        }

        public void TargetCircle(int duration = 1)
        {

            TemporaryAnimatedSprite innerCircle = new(0, 1000f * duration, 1, 1, destination * 64 - new Vector2(32*radius, 32*radius) + new Vector2(32, 32), false, false)
            {

                sourceRect = new(0, 0, 64, 64),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", barrageColor + "Target.png")),

                scale = radius,

                rotationChange = -0.06f,

                alpha = 0.5f,

            };

            location.temporarySprites.Add(innerCircle);

            animations.Add(innerCircle);

        }

        public void LightRadius(int impact = 0)
        {
            Vector2 source;

            if (impact < 0)
            {

                source = origin;

            }
            else
            {

                if (!impacts.ContainsKey(impact))
                {

                    return;

                }

                if(impacts[impact].Count == 0)
                {

                    return;

                }

                source = impacts[impact].First();

            }

            TemporaryAnimatedSprite lightCircle = new(23, 200f, 6, 1, new Vector2(source.X * 64f, source.Y * 64f), false, Game1.random.NextDouble() < 0.5)
            {
                texture = Game1.mouseCursors,
                light = true,
                lightRadius = 3f,
                lightcolor = Color.Black,
                alphaFade = 0.03f,
                Parent = location,
            };

            location.temporarySprites.Add(lightCircle);

            animations.Add(lightCircle);


        }

        public void BurnCircle()
        {

            List<Vector2> impactVectors;
            
            Dictionary<Vector2,TemporaryAnimatedSprite> burnSprites = new();

            Texture2D flameTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", barrageColor + "Fire.png"));

            for (int i = 0; i < Math.Min(3,radius + 1); i++)
            {

                impactVectors = ModUtility.GetTilesWithinRadius(location, destination, i);

                float scale = 2 - (i * 0.5f);

                foreach (Vector2 vector in impactVectors)
                {

                    Vector2 position = new((vector.X * 64) + (i * 8), (vector.Y * 64) + (i * 8));

                    TemporaryAnimatedSprite burnSprite =  new(0, 100, 4, 6, position, false, false)
                    {

                        sourceRect = new(0, 0, 32, 32),

                        sourceRectStartingPos = new Vector2(0, 0),

                        texture = flameTexture,

                        extraInfoForEndBehavior = 99999,

                        scale = scale,

                        layerDepth = vector.Y / 10000,

                        alphaFade = 0.001f,

                    };

                    burnSprites.Add(vector,burnSprite);

                }

            }

            for(int i = location.temporarySprites.Count - 1; i >= 0; i--)
            {

                TemporaryAnimatedSprite sprite = location.temporarySprites.ElementAt(i);

                if (sprite.extraInfoForEndBehavior == 99999)
                {

                    Vector2 localVector = new((int)(sprite.Position.X/64),(int)(sprite.Position.Y/64));

                    if (burnSprites.ContainsKey(localVector))
                    {

                        if(sprite.scale > burnSprites[localVector].scale)
                        {
                            burnSprites[localVector].scale = sprite.scale;
                            burnSprites[localVector].Position = sprite.Position;
                        }

                        /*if(burnSprites[localVector].scale < 2f)
                        {
                            burnSprites[localVector].scale += 0.5f;
                            burnSprites[localVector].Position -= new Vector2(8,8);

                        }*/

                        location.temporarySprites.Remove(sprite);

                    }
                    else
                    {

                        animations.Add(sprite);

                    }

                }

            }

            foreach(KeyValuePair< Vector2,TemporaryAnimatedSprite> spritePair in burnSprites)
            {

                location.temporarySprites.Add(spritePair.Value);

                animations.Add(spritePair.Value);

            }


        }

        public void BurnImpacts()
        {

            List<Vector2> burnVectors = new();

            for (int i = animations.Count - 1; i >= 0; i--)
            {

                TemporaryAnimatedSprite sprite = animations.ElementAt(i);

                if(sprite.scale >= 2)
                {

                    if (!location.temporarySprites.Contains(sprite))
                    {

                        animations.RemoveAt(i);

                        continue;

                    }

                    Vector2 localVector = new((int)(sprite.Position.X / 64), (int)(sprite.Position.Y / 64));

                    burnVectors.Add(localVector);

                }

            }

            if (!Mod.instance.eventRegister.ContainsKey("immolate"))
            {

                new Immolate(Game1.player.Position, Mod.instance.NewRite(false)).EventTrigger();

            }

            Immolate immolateEvent = Mod.instance.eventRegister["immolate"] as Immolate;

            immolateEvent.expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 10;

            if (damageFarmers > 0)
            {

                List<Farmer> farmerVictims = new();

                foreach(Vector2 vector in burnVectors)
                {
                    
                    List<Farmer> foundVictims = ModUtility.FarmerProximity(location, vector * 64, radius, true);

                    if(foundVictims.Count > 0)
                    {

                        foreach(Farmer found in foundVictims)
                        {

                            if (!farmerVictims.Contains(found))
                            {

                                farmerVictims.Add(found);

                            }

                        }

                    }

                }

                foreach(Farmer victim in  farmerVictims)
                {

                    if(immolateEvent.farmerVictims.ContainsKey(victim))
                    {

                        immolateEvent.farmerVictims[victim].timer = 3;

                        immolateEvent.farmerVictims[victim].damage = (int)damageFarmers;

                    }
                    else
                    {

                        immolateEvent.farmerVictims.Add(victim, new((int)damageFarmers));
                    
                    }

                }

            }

            if (damageMonsters > 0)
            {

                List<StardewValley.Monsters.Monster> monsterVictims = new();

                foreach (Vector2 vector in burnVectors)
                {

                    List<StardewValley.Monsters.Monster> foundVictims = ModUtility.MonsterProximity(location, vector * 64, radius, true);

                    if (foundVictims.Count > 0)
                    {

                        foreach (StardewValley.Monsters.Monster found in foundVictims)
                        {

                            if (!monsterVictims.Contains(found))
                            {

                                monsterVictims.Add(found);

                            }

                        }

                    }

                }

                foreach (StardewValley.Monsters.Monster victim in monsterVictims)
                {

                    if (immolateEvent.monsterVictims.ContainsKey(victim))
                    {

                        immolateEvent.monsterVictims[victim].timer = 3;

                        immolateEvent.monsterVictims[victim].damage = (int)damageMonsters;

                    }
                    else
                    {

                        immolateEvent.monsterVictims.Add(victim, new((int)damageMonsters));

                    }

                }

            }

        }

        public void ResetCircle()
        {

            animations[0].reset();

            animations[1].reset();

        }

        public void LaunchBarrage()
        {

            impacts = new();

            Random random = new();

            List<Vector2> scatters = new()
            {

                new(0,-1),
                new(0,-2),
                new(1,0),
                new(2,0),
                new(0,1),
                new(0,2),
                new(-1,0),
                new(-2,0),

            };

            Texture2D missileTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", barrageColor + "Missile.png"));

            for (int i = 0; i < missiles; i++)
            {

                if (i == 0)
                {
                    impacts.Add(i,new() { destination, });

                    continue;
                }

                Vector2 scatter = destination + scatters[random.Next(scatters.Count)];

                impacts.Add(i, new() { scatter, });

            }

            for (int i = 0; i < impacts.Count; i++)
            {

                Vector2 impact = impacts[i][0];

                Vector2 originPosition = origin * 64 - new Vector2(0,128);

                Vector2 destinationPosition = impact * 64;


                float motionX = (destinationPosition.X - originPosition.X) / 2000;

                float motionY = -0.64f;

                float rotate;

                if (motionX < 0.001f)
                {
                    rotate = 0 - (float)(Math.PI * 0.55f);
                }
                else
                {
                    rotate = 0 - (float)(Math.PI * 0.45f);
                }

                TemporaryAnimatedSprite missile = new(0, 125, 4, 2, originPosition - new Vector2(16, 0), false, false)
                {

                    sourceRect = new(0, 0, 32, 16),

                    sourceRectStartingPos = new Vector2(0, 0),

                    texture = missileTexture,

                    scale = 3f,

                    rotation = rotate,


                    motion = new Vector2(motionX, motionY),

                    //flipped = flip,
                    //verticalFlipped = flip,

                    timeBasedMotion = true,
                    alpha = 0.65f,
                };

                location.temporarySprites.Add(missile);

                animations.Add(missile);

            }

        }

        public void MissileBarrage()
        {

            Texture2D missileTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", barrageColor + "Missile.png"));

            for (int i = 0; i < impacts.Count; i++)
            {


                Vector2 impact = impacts[i][0];

                Vector2 destinationPosition = impact * 64;

                Vector2 originPosition = origin * 64;
                
                Vector2 dropPosition = destinationPosition - ((destinationPosition - originPosition) / 2) - new Vector2(0,768);

                float motionX = (destinationPosition.X - originPosition.X) / 2000;

                float motionY = 0.704f;

                float rotate;

                if (motionX < 0.001f)
                {
                    rotate = (float)(Math.PI * 0.55f);
                }
                else
                {
                    rotate = (float)(Math.PI * 0.45f);
                }

                TemporaryAnimatedSprite missile = new(0, 125, 4, 2, dropPosition - new Vector2(16, 0), false, false)
                {

                    sourceRect = new(0, 0, 32, 16),

                    sourceRectStartingPos = new Vector2(0, 0),

                    texture = missileTexture,

                    scale = 3f,

                    rotation = rotate,

                    motion = new Vector2(motionX, motionY),

                    //flipped = flip,

                    //verticalFlipped = flip,

                    timeBasedMotion = true,

                    alpha = 0.65f,

                };

                location.temporarySprites.Add(missile);

                animations.Add(missile);


            }

        }

        public void LaunchFireball(int hits = 1)
        {

            Texture2D missileTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", barrageColor + "Missile.png"));

            impacts = new();

            Vector2 diff = (destination - origin);

            List<Vector2> hitVectors = new();

            for (int i = 0; i < hits; i++)
            {

                if(i == 0)
                {

                    hitVectors.Add(destination);
                
                    continue;
                
                }

                hitVectors.Add(origin + ((diff / hits) * i));

            }

            impacts.Add(0, hitVectors);

            Vector2 destinationPosition = destination * 64;

            Vector2 originPosition = origin * 64;

            Vector2 diffPosition = (destinationPosition - originPosition);

            Vector2 motion = diffPosition / 750;

            originPosition.Y -= 8 * radius;

            float rotate = (float)Math.Atan2(motion.Y, motion.X);

            TemporaryAnimatedSprite missile = new(0, 175f, 4, 1, originPosition, false, false)
            {
                sourceRect = new(0, 0, 32, 16),
                sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                texture = missileTexture,
                scale = 1f + radius,
                timeBasedMotion = true,
                layerDepth = 999f,
                motion = motion,
                rotation = rotate,
                alpha = 0.5f,

            };

            location.temporarySprites.Add(missile);

            animations.Add(missile);

        }

        public void LaunchBeam()
        {

            Texture2D beamTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", barrageColor + "Beam.png"));

            Vector2 originPosition = origin * 64;

            Vector2 destinationPosition = destination * 64;

            Vector2 diff = (destinationPosition - originPosition) / Vector2.Distance(originPosition, destinationPosition) * 300;

            float rotate = (float)Math.Atan2(diff.Y, diff.X);

            Vector2 setPosition = originPosition + diff - new Vector2(240,48);

            TemporaryAnimatedSprite beam = new(0, 125f, 4, 1, setPosition, false, false)
            {
                sourceRect = new(0, 0, 160, 32),
                sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                texture = beamTexture,
                scale = 3f,
                timeBasedMotion = true,
                layerDepth = 999f,
                rotation = rotate,

            };

            location.temporarySprites.Add(beam);

            animations.Add(beam);

            TemporaryAnimatedSprite beam2 = new(0, 500f, 1, 1, setPosition, false, false)
            {
                sourceRect = new(0, 128, 160, 32),
                sourceRectStartingPos = new Vector2(0.0f, 128f),
                texture = beamTexture,
                scale = 3f,
                timeBasedMotion = true,
                layerDepth = 999f,
                rotation = rotate,
                delayBeforeAnimationStart = 500,
            };

            location.temporarySprites.Add(beam2);

            animations.Add(beam2);

        }

        public void FadeBeam()
        {

            Texture2D beamTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", barrageColor + "Beam.png"));

            Vector2 originPosition = origin * 64;

            Vector2 destinationPosition = destination * 64;

            Vector2 diff = (destinationPosition - originPosition) / Vector2.Distance(originPosition, destinationPosition) * 300;

            float rotate = (float)Math.Atan2(diff.Y, diff.X);

            Vector2 setPosition = originPosition + diff - new Vector2(240, 48);

            TemporaryAnimatedSprite beam = new(0, 500f, 1, 1, setPosition, false, false)
            {
                sourceRect = new(0, 128, 160, 32),
                sourceRectStartingPos = new Vector2(0.0f, 128f),
                texture = beamTexture,
                scale = 3f,
                timeBasedMotion = true,
                layerDepth = 999f,
                rotation = rotate,
                alphaFade = 0.002f,
            };

            location.temporarySprites.Add(beam);

            animations.Add(beam);

        }

        public void RadialDamage(int hit = 0)
        {

            int zone;

            for(int i = 0; i < impacts.Count; i++)
            {

                if (impacts[i].Count == 0)
                {

                    continue;

                }

                Vector2 impact = impacts[i][hit];

                if (type == barrageType.burn)
                {
                    zone = radius - 1;
                }
                else if (i == 0 && hit == 0)
                {
                    zone = radius;
                }
                else
                {
                    zone = radius - 1;

                }

                if (damageFarmers > 0)
                {

                    ModUtility.DamageFarmers(location, ModUtility.FarmerProximity(location, impact * 64, zone, true), (int)damageFarmers, monster);
                
                }

                if (damageMonsters > 0)
                {
                    
                    ModUtility.DamageMonsters(location, ModUtility.MonsterProximity(location, impact * 64, zone, true), Game1.player, (int)damageMonsters, true);

                }

                if(damageEnvironment > 0)
                {

                    ModUtility.Explode(location, impact, Game1.player, zone, damageEnvironment, damageDirt);

                }

                if ((type == barrageType.missile || type == barrageType.fireball) && hit == 0)
                {
                    
                    ModUtility.AnimateImpact(location, impact, zone - 1, barrageColor);
                
                }

            }

        }

    }

}
