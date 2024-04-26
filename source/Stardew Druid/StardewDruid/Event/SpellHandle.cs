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

using System.Linq;

using StardewModdingAPI;

using StardewDruid.Data;


namespace StardewDruid.Event
{
    public class SpellHandle
    {

        public GameLocation location;

        public Vector2 destination;

        public Vector2 origin;

        public int counter;

        public int radius;

        public int projectile;

        public int projectileIncrements;

        public int projectileTrack;

        public int projectileSpeed;

        public bool projectileSent;

        public int threshold;

        public float damageFarmers;

        public float damageMonsters;

        public int power;

        public int environment;

        public int terrain;

        public int debris;

        public List<TemporaryAnimatedSprite> animations = new();

        public TemporaryAnimatedSprite cursor;

        public Vector2 impact;

        public StardewDruid.Monster.Boss.Boss boss;

        public List<StardewValley.Monsters.Monster> monsters = new();

        public float critical;

        public float criticalModifier;

        public bool external;

        public bool local;

        public bool instant;

        public enum spells
        {
            effect,
            explode,
            ballistic,
            beam,
            missile,
            wisp,
            chaos,
            meteor,
            bolt,
            rockfall,
            blackhole,
        }

        public spells type;

        public IconData.cursors indicator;

        public enum schemes
        {
            none,
            fire,
            stars,
            fates,
            ether,
            death,
        }

        public schemes scheme;

        public enum effects
        {
            sap,
            drain,
            knock,
            burn,
            homing,
            aiming,
            push,
            warp,
            gravity,
            harvest,
            morph,
            doom,
        }

        public List<effects> added = new();

        public enum displays
        {
            none,
            Impact,
            Flashbang,
            Glare,
            Sparkle,
            Blaze,
            Death,

        }

        public displays display;

        public enum sounds
        {
            none,
            explosion,
            flameSpellHit,
            shadowDie,

        }

        public sounds sound;
        
        public SpellHandle(Farmer farmer, List<StardewValley.Monsters.Monster> Monsters, float damage)
        {

            location = farmer.currentLocation;

            origin = farmer.Position;

            destination = Monsters.First().Position;

            radius = 128;

            projectile = 2;

            damageFarmers = -1f;

            damageMonsters = damage;

            impact = destination;

            type = spells.explode;

            indicator = IconData.cursors.none;

            scheme = schemes.none;

            display = displays.none;

            sound = sounds.none;

            monsters = Monsters;

        }

        public SpellHandle(Farmer farmer, Vector2 Destination, int Radius, float damage)
        {

            location = farmer.currentLocation;

            origin = farmer.Position;

            destination = Destination;

            radius = Radius;

            projectile = 2;

            damageFarmers = -1f;

            damageMonsters = damage;

            impact = destination;

            type = spells.explode;

            indicator = IconData.cursors.none;

            scheme = schemes.none;

            display = displays.none;

            sound = sounds.none;

        }

        public SpellHandle(GameLocation Location, Vector2 Destination, Vector2 Origin, int Radius = 128, float vsFarmers = -1f, float vsMonsters = -1f)
        {

            location = Location;

            origin = Origin;

            destination = Destination;

            radius = Radius;

            projectile = 2;

            damageFarmers = vsFarmers;

            damageMonsters = vsMonsters;

            impact = Destination;

            type = spells.explode;

            indicator = IconData.cursors.none;

            scheme = schemes.none;

            display = displays.none;

            sound = sounds.none;

        }

        public void SpellQuery()
        {

            List<int> array = new()
            {
                (int)destination.X,
                (int)destination.Y,
                (int)origin.X,
                (int)origin.Y,
                radius,
                Convert.ToInt32(type),
                Convert.ToInt32(scheme),
                Convert.ToInt32(indicator),
                Convert.ToInt32(display),
                projectile,
            };

            QueryData query = new()
            {
                name = type.ToString(),

                value = System.Text.Json.JsonSerializer.Serialize<List<int>>(array),

                location = location.Name,

                longId = Game1.player.UniqueMultiplayerID,

            };

            Mod.instance.EventQuery(query, "SpellHandle");        
        
        }

        public SpellHandle(GameLocation Location, List<int> spellData)
        {

            location = Location;

            destination = new Vector2(spellData[0], spellData[1]);

            origin = new Vector2(spellData[2], spellData[3]);

            radius = spellData[4];

            impact = destination;

            damageFarmers = -1;

            damageMonsters = -1;

            external = true;

            type = (spells)Enum.Parse(typeof(spells), spellData[5].ToString());

            scheme = (schemes)Enum.Parse(typeof(schemes), spellData[6].ToString());

            indicator = (IconData.cursors)Enum.Parse(typeof(IconData.cursors), spellData[7].ToString());

            display = (displays)Enum.Parse(typeof(displays), spellData[8].ToString());

            sound = sounds.none;

            projectile = spellData[9];

        }

        public bool Update()
        {

            counter++;

            if (counter == 1) 
            {
                
                if(Context.IsMultiplayer && !external && !local){

                    SpellQuery();
                
                }
                
            }

            if(counter % 10 == 0)
            {

                if(boss != null)
                {
                    
                    if (!ModUtility.MonsterVitals(boss, location))
                    {

                        Shutdown();

                        return false;

                    }

                }

                if(monsters.Count > 0)
                {

                    for(int m = monsters.Count - 1; m >= 0; m--)
                    {

                        if (!ModUtility.MonsterVitals(monsters[m], location))
                        {

                            monsters.Remove(monsters[m]);

                        }

                    }

                }

            }

            switch (type)
            {

                case spells.effect:

                    if (counter == 1)
                    {

                        ApplyEffects();

                    }

                    if (counter == 120)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;


                case spells.explode:

                    if (counter == 1)
                    {
                        if (instant)
                        {

                            counter = 15;

                        }
                        else
                        {
                            
                            TargetCircle(0.35f);

                        }

                    }

                    if (counter == 15)
                    {

                        ApplyDamage(impact, radius, damageFarmers, damageMonsters,new());

                        RadialExplode();

                        RadialDisplay();

                        ApplyEffects();

                    }

                    if (counter == 120)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;

                case spells.meteor:

                    if (counter == 1)
                    {

                        origin = destination - new Vector2(320, 640);

                        LaunchMissile();

                        TargetCircle();

                        return true;

                    }

                    if (counter == 60)
                    {

                        ApplyDamage(impact, radius, damageFarmers, damageMonsters, new());

                        RadialExplode();

                        RadialDisplay();

                        return true;

                    }

                    if (counter == 120)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;


                case spells.bolt:

                    if (counter == 1)
                    {

                        LaunchBolt();

                        ApplyDamage(impact, radius, -1, damageMonsters, new());

                        return true;

                    }

                    if (counter == 30)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;

                case spells.rockfall:

                    if (counter == 1)
                    {

                        LaunchRockfall();

                        return true;

                    }

                    if (counter == 36)
                    {

                        ApplyDamage(impact, radius, damageFarmers, damageMonsters, new());

                        RadialExplode();

                        ModUtility.AnimateImpact(location, impact, 1);

                        RockDebris();

                        return true;

                    }

                    if (counter == 60)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;

                case spells.ballistic:


                    if (counter == 1)
                    {

                        Scheme();

                        TargetCircle(3);

                    }

                    if (counter >= 60 && counter % 20 == 0)
                    {

                        MissileOnScreen();

                    }

                    if (counter == 180)
                    {

                        ApplyDamage(impact, radius, damageFarmers, damageMonsters, new());

                        RadialExplode();

                        RadialDisplay();

                    }

                    if (counter == 240)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;


                case spells.missile:

                    if (counter == 1)
                    {
                        Scheme();

                        AdjustMissile();

                        TargetCircle();

                        AdjustTarget();

                        InstantFire();

                    }
                    else if (counter < 30)
                    {

                        AdjustTarget();

                    }

                    if (counter == 30)
                    {

                        AdjustSpeed();

                        LaunchMissile();

                        return true;
                    
                    }

                    if(counter < 300)
                    {

                        if(counter % 20 == 0)
                        {

                            GrazeDamage(projectileTrack,projectileIncrements);

                            projectileTrack++;

                        }

                    }

                    if (counter == 300)
                    {

                        ApplyDamage(impact, radius, damageFarmers, damageMonsters, new());

                        RadialExplode();

                        RadialDisplay();

                    }

                    if (counter == 360)
                    {

                        Shutdown();

                        return false;
                    }

                    return true;

                case spells.beam:

                    if (counter == 1)
                    {
                        Scheme();

                        TargetCircle();

                        LaunchBeam();

                        LightRadius(origin);

                    }

                    if (counter == 45)
                    {

                        GrazeDamage(1, 3, 3);

                    }

                    if (counter == 60)
                    {

                        GrazeDamage(1, 3, 3);

                    }

                    if (counter == 75)
                    {

                        ApplyDamage(impact, radius, damageFarmers, damageMonsters, new());

                        RadialDisplay();

                        ApplyEffects();

                    }

                    if (counter == 130)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;

                case spells.chaos:

                    if (counter == 1)
                    {

                        LaunchChaos();

                    }

                    if (counter == 10)
                    {

                        GrazeDamage(1, 2);

                    }

                    if (counter == 15)
                    {

                        ApplyDamage(impact, radius, damageFarmers, damageMonsters, new());

                        RadialDisplay();

                    }

                    if (counter == 60)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;

                case spells.blackhole:

                    if (counter == 1)
                    {

                        LaunchBlackhole();

                    }

                    if (counter == 60)
                    {

                        ApplyEffects();

                    }

                    if (counter == 300)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;

            }

            return true;
        }

        public void Scheme()
        {

            switch (type)
            {

                case spells.ballistic:

                    projectileIncrements = 6;

                    switch (scheme)
                    {

                        case schemes.ether:

                            indicator = IconData.cursors.blueTarget;

                            break;

                        case schemes.death:

                            indicator = IconData.cursors.death;

                            break;

                        default:

                            indicator = IconData.cursors.redTarget;

                            break;

                    }

                    break;

                default:

                    switch (scheme)
                    {

                        case schemes.ether:

                            indicator = IconData.cursors.blueArrow;

                            break;

                        case schemes.death:

                            indicator = IconData.cursors.death;

                            break;

                        default:

                            indicator = IconData.cursors.redArrow;

                            break;

                    }

                    break;

            }


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

        // ========================================= setup

        public void AdjustMissile()
        {
            
            if (!added.Contains(effects.aiming))
            {
                float range = Vector2.Distance(origin, impact);

                if (range < 192)
                {

                    Vector2 diff = ((impact - origin) / Vector2.Distance(origin, impact)) * 192;

                    impact = origin + diff;

                }

                if (range > 192 * 6)
                {

                    Vector2 diff = ((impact - origin) / Vector2.Distance(origin, impact)) * (192 * 6);

                    impact = origin + diff;

                }

            }

        }


        public void MissileOnScreen()
        {

            if (projectileSent)
            {
                return;

            }

            origin = impact - new Vector2(0, 192 * projectileIncrements);

            if (Utility.isOnScreen(origin,0))
            {

                LaunchMissile();

                return;

            }

            projectileIncrements--;

        }


        public void TargetCircle(float duration = 1)
        {

            if(indicator == IconData.cursors.none)
            {

                return;

            }

            TemporaryAnimatedSprite innerCircle = Mod.instance.iconData.CursorIndicator(location, impact, indicator, duration * 1000);

            if(type == spells.missile || type == spells.beam)
            {

                innerCircle.rotationChange = 0f;

                Vector2 diff = (impact - origin);

                float rotate = (float)Math.Atan2(diff.Y, diff.X);

                innerCircle.rotation = rotate;

            }

            animations.Add(innerCircle);

            cursor = innerCircle;

        }

        public void AdjustTarget()
        {

            if (added.Contains(effects.aiming) && !external)
            {

                Vector2 adjust = Vector2.Zero;

                if (damageMonsters > 0 && monsters.Count > 0)
                {

                    if (ModUtility.MonsterVitals(monsters.First(), location))
                    {

                        adjust = monsters.First().Position - impact;

                    }

                }

                if (adjust != Vector2.Zero)
                {

                    impact += adjust;

                    if (cursor != null)
                    {

                        cursor.position += adjust;

                        float rotate = (float)Math.Atan2(adjust.Y, adjust.X);

                        cursor.rotation += rotate;

                    }

                }

            }

        }

        public void InstantFire(int counterUpdate = 30)
        {

            if (instant)
            {

                counter = counterUpdate;

            }

        }

        public void AdjustSpeed()
        {

            if(projectileSpeed == 0)
            {

                projectileSpeed = 1;

            }

            float distance = Vector2.Distance(impact, origin);

            projectileIncrements = (int)(distance / (192 * projectileSpeed));

            projectileTrack = 1;

            counter = 300 - (projectileIncrements * 20);

        }

        public string ColourSchemes(string id)
        {

            if(id == "ether") { return "Blue"; }

            return "Red";

        }

        public Rectangle MissileRectangles(string id)
        {

            Microsoft.Xna.Framework.Rectangle rect = new(0, 0, 96, 96);

            switch (id)
            {

                case "stars":

                    rect.Y += 96;

                    break;

                case "fates":

                    rect.Y += 192;

                    break;

                case "ether":

                    rect.Y += 288;

                    break;

                case "death":

                    rect.Y += 384;

                    break;

            }

            return rect;

        }

        public IconData.cursors SchemeCursor(schemes schemeId)
        {

            switch (schemeId.ToString())
            {

                case "stars":

                    return IconData.cursors.comet;

                case "fates":

                    return IconData.cursors.fatesCharge;

            }

            return IconData.cursors.none;

        }
        
        // ========================================= start

        public void LaunchBolt()
        {
            
            ModUtility.AnimateBolt(location, new Vector2(destination.X, destination.Y - 64));

        }

        public void LaunchMissile()
        {

            projectileSent = true;

            Game1.currentLocation.playSound("fireball");

            float targetDepth = location.IsOutdoors ? (destination.Y / 640000) + 0.00001f : 999f;

            Vector2 diff = (impact - origin);

            if (projectileIncrements == 0)
            {

                projectileIncrements = 3;

            }

            Vector2 motion = diff / (projectileIncrements * 300);

            float rotate = (float)Math.Atan2(diff.Y, diff.X);

            Rectangle rect = MissileRectangles(scheme.ToString());

            Vector2 setat = origin - (new Vector2(48, 48) * projectile);

            TemporaryAnimatedSprite missile = new(0, 75, 4, projectileIncrements, setat, false, false)
            {

                sourceRect = rect,

                sourceRectStartingPos = new Vector2(rect.X, rect.Y),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Missiles.png")),

                scale = projectile,

                layerDepth = targetDepth,

                timeBasedMotion = true,

                alpha = 0.65f,

                rotation = rotate,

                motion = motion,

            };

            location.temporarySprites.Add(missile);

            animations.Add(missile);

            IconData.cursors cursorId = SchemeCursor(scheme);

            if(cursorId == IconData.cursors.none)
            {

                return;

            }

            Vector2 setattwo = origin - (new Vector2(16, 16) * projectile);

            Microsoft.Xna.Framework.Rectangle cursorRect = Mod.instance.iconData.CursorRect(cursorId);

            TemporaryAnimatedSprite cursorAnimation = new(0, 300 * projectileIncrements, 1, 1, setattwo, false, false)
            {

                sourceRect = cursorRect,

                sourceRectStartingPos = new Vector2(cursorRect.X, cursorRect.Y),

                texture = Mod.instance.iconData.cursorTexture,

                scale = projectile,

                layerDepth = targetDepth + 0.0001f,

                timeBasedMotion = true,

                alpha = 0.75f,

                rotationChange = (float)(Math.PI / 60),

                motion = motion,

            };

            location.temporarySprites.Add(cursorAnimation);

            animations.Add(cursorAnimation);

        }

        public void LaunchBeam()
        {

            Texture2D beamTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Laser.png"));

            Vector2 diff = (destination - origin) / Vector2.Distance(origin, destination);

            Vector2 middle = diff * 336;

            impact = origin + (diff * 560);

            cursor.position = origin + (diff * 640) - new Vector2(16,16);

            float rotate = (float)Math.Atan2(diff.Y, diff.X);

            Vector2 setPosition = origin + middle - new Vector2(320,64);

            TemporaryAnimatedSprite beam = new(0, 125f, 12, 1, setPosition, false, false)
            {
                sourceRect = new(0, 0, 160, 32),
                sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                texture = beamTexture,
                scale = 4f,
                timeBasedMotion = true,
                layerDepth = 999f,
                rotation = rotate,
                alpha = 0.65f,

            };

            location.temporarySprites.Add(beam);

            animations.Add(beam);

        }

        public void LaunchChaos()
        {

            Texture2D beamTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Chaos.png"));

            Vector2 ratio = (destination - origin) / Vector2.Distance(origin, destination);

            Vector2 diff = ratio * 192;

            impact = origin + (ratio * 384);

            float rotate = (float)Math.Atan2(diff.Y, diff.X);

            Vector2 setPosition = origin + diff - new Vector2(192, 96);

            TemporaryAnimatedSprite beam = new(0, 75f, 6, 1, setPosition, false, false)
            {
                sourceRect = new(0, 0, 128, 64),
                sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                texture = beamTexture,
                scale = 3f,
                timeBasedMotion = true,
                layerDepth = 999f,
                rotation = rotate,
                alphaFade = 0.001f,

            };

            location.temporarySprites.Add(beam);


        }

        public void LaunchRockfall()
        {

            List<int> indexes = SpawnData.RockFall(location, Game1.player, 20 - (Mod.instance.PowerLevel * 2));

            int objectIndex = indexes[0];

            int scatterIndex = indexes[1];

            if (debris == 1)
            {

                debris = indexes[2];

            }

            ModUtility.AnimateRockfall(location, destination / 64, 0, objectIndex, scatterIndex);

        }

        public void LaunchBlackhole()
        {

            Texture2D gravity = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Gravity.png"));

            TemporaryAnimatedSprite startAnimation = new(0, 1000f, 1, 1, origin, false, false)
            {

                sourceRect = new(0, 0, 64, 64),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = gravity,

                scale = 2f,

                scaleChange = 0.002f,

                layerDepth = location.IsOutdoors ? (impact.Y / 640000) + 0.00001f : 999f,

                motion = (impact - origin) / 1000 - (new Vector2(96,96) / 1000),

                timeBasedMotion = true,

                rotationChange = -0.06f,

                alpha = 0.75f,

            };

            location.temporarySprites.Add(startAnimation);

            animations.Add(startAnimation);

            TemporaryAnimatedSprite staticAnimation = new(0, 99999f, 1, 1, impact - new Vector2(96, 96), false, false)
            {

                sourceRect = new(0, 0, 64, 64),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = gravity,

                scale = 4f,

                layerDepth = location.IsOutdoors ? (impact.Y / 640000) + 0.00001f : 990f,

                rotationChange = -0.06f,

                timeBasedMotion = true,

                delayBeforeAnimationStart = 1000,

                alpha = 0.75f,

            };

            location.temporarySprites.Add(staticAnimation);

            animations.Add(staticAnimation);

            TemporaryAnimatedSprite bandAnimation = new(0, 9999f, 1, 1, impact - new Vector2(96, 96), false, false)
            {

                sourceRect = new(64, 0, 64, 64),

                sourceRectStartingPos = new Vector2(64, 0),

                texture = gravity,

                scale = 4f,

                layerDepth = location.IsOutdoors ? (impact.Y / 640000) + 0.00002f : 991f,

                timeBasedMotion = true,

                delayBeforeAnimationStart = 1000,

                alpha = 0.75f,

            };

            location.temporarySprites.Add(bandAnimation);

            animations.Add(bandAnimation);

        }


        // ========================================= end

        public void RadialDisplay()
        {

            LightRadius(impact);

            if (display != displays.none)
            {

                ModUtility.AnimateImpact(location, impact, (int)(radius/64) - 1, 0, display.ToString());

            }

            if(sound != sounds.none)
            {

                Game1.currentLocation.playSound(sound.ToString(), null, 800 + new Random().Next(7) * 100);

            }

        }

        public void LightRadius(Vector2 source)
        {

            TemporaryAnimatedSprite lightCircle = new(23, 200f, 6, 1, source, false, Game1.random.NextDouble() < 0.5)
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

        public void GrazeDamage(int piece, int division, float reach = -1)
        {

            if (external)
            {

                return;

            }

            if(reach == -1)
            {

                reach = projectile;

            }

            Vector2 diff = (impact - origin) / division * piece;

            Vector2 current = origin + diff;

            ApplyDamage(current, reach * 32, (int)(damageFarmers / 2), (int)(damageMonsters / 2), new());

        }

        public void ApplyDamage(Vector2 position, float reach, float hitfarmers, float hitmonsters, List<StardewValley.Monsters.Monster> individuals)
        {
            
            if (external)
            {

                return;

            }

            if (hitfarmers > 0 && boss != null)
            {

                List<Farmer> farmers = ModUtility.FarmerProximity(location, new() { position }, reach + 32, true);

                ModUtility.DamageFarmers(farmers, (int)hitfarmers, boss);

            }
            
            if (hitmonsters > 0)
            {

                if(individuals.Count == 0)
                {

                    individuals = ModUtility.MonsterProximity(location, new() { position }, reach + 32, true);

                }

                if(individuals.Count == 0)
                {

                    return;

                }

                bool push = false;

                foreach(effects effect in added)
                {

                    switch (effect)
                    {

                        case effects.push:

                            push = true;

                            break;

                    }

                }

                if(critical == 0)
                {
                    
                    critical = 0.1f;

                }

                if(criticalModifier == 0)
                {
                    
                    criticalModifier = 1f;

                }

                ModUtility.DamageMonsters(location, individuals, Game1.player, (int)hitmonsters, critical, criticalModifier, push);

            }

        }

        public void ApplyEffects()
        {

            foreach (effects effect in added)
            {

               /* switch (effect)
                {
                    case effects.sap:

                        SapEffect();

                        break;

                    case effects.knock:
                    case effects.doom:
                    case effects.morph:

                        KnockEffect(effect);

                        break;

                    case effects.burn:

                        BurnEffect();

                        break;

                    case effects.warp:

                        WarpEffect();

                        break;

                    case effects.gravity:

                        GravityEffect();

                        break;

                    case effects.harvest:

                        HarvestEffect();

                        break;

                }*/

            }


        }

        public void RadialExplode()
        {

            if (external)
            {

                return;

            }

            if (power > 0)
            {

                if(environment == 0)
                {

                    environment = (int)(radius / 64);

                }

                ModUtility.Explode(location, impact / 64, Game1.player, environment, power);

            }

            if (terrain > 0)
            {

                ModUtility.Reave(location, impact / 64, Game1.player, terrain);

            }

        }

        // ========================================= effects

        public void RockDebris()
        {

            if (external)
            {

                return;

            }

            if (debris == 0)
            {

                return;

            }

            Random randomIndex = new();

            int rockCut = randomIndex.Next(2);

            int generateAmt = Math.Max(1, randomIndex.Next(Mod.instance.PowerLevel));

            Vector2 targetVector = destination / 64;

            for (int i = 0; i < generateAmt; i++)
            {
                if (i == 0)
                {

                    if (Game1.player.professions.Contains(21) && rockCut == 0)
                    {

                        Game1.createObjectDebris("382", (int)targetVector.X, (int)targetVector.Y);

                    }
                    else if (Game1.player.professions.Contains(19) && rockCut == 0)
                    {

                        Game1.createObjectDebris(debris.ToString(), (int)targetVector.X, (int)targetVector.Y);

                    }

                    Game1.createObjectDebris(debris.ToString(), (int)targetVector.X, (int)targetVector.Y);

                }
                else
                {

                    Game1.createObjectDebris("390", (int)targetVector.X, (int)targetVector.Y);

                }

            }

        }

        public void SapEffect()
        {

            if (external)
            {

                return;

            }

            int leech = 0;

            int drain = Mod.instance.PowerLevel * 3;

            if (monsters.Count == 0)
            {

                monsters = ModUtility.MonsterProximity(location, new() { impact }, radius + 32, true);

            }

            foreach (var monster in monsters)
            {

                if (!ModUtility.MonsterVitals(monster, monster.currentLocation))
                {
                    continue;
                }

                ModUtility.AnimateGlare(monster.currentLocation, monster.Position, Color.Teal);

                Microsoft.Xna.Framework.Rectangle boundingBox = monster.GetBoundingBox();

                Color color = Color.DarkGreen;

                location.debris.Add(new Debris(drain, new Vector2(boundingBox.Center.X + 16, boundingBox.Center.Y), color, 1f, monster));

                leech += drain;

            }

            int num = Math.Min(leech, Mod.instance.rite.caster.MaxStamina - (int)Mod.instance.rite.caster.stamina);

            if (num > 0)
            {

                Mod.instance.rite.caster.stamina += num;

                Rectangle healthBox = Mod.instance.rite.caster.GetBoundingBox();

                location.debris.Add(
                    new Debris(
                        num,
                        new Vector2(healthBox.Center.X + 16, healthBox.Center.Y),
                        Color.Teal,
                        0.75f,
                        Mod.instance.rite.caster
                    )
                );

            }

        }

        /*public void KnockEffect(effects effect = effects.knock)
        {

            if (external)
            {

                return;

            }

            Cast.Stars.Curse Knockout;

            if (!Mod.instance.eventRegister.ContainsKey("curse"))
            {

                Knockout = new();

                Knockout.EventTrigger();

            }
            else
            {

                Knockout = Mod.instance.eventRegister["curse"] as Curse;

            }

            if (monsters.Count == 0)
            {

                monsters = ModUtility.MonsterProximity(location, new() { impact }, radius + 32, true);

            }

            foreach (var monster in monsters)
            {

                Knockout.AddTarget(location, monster, effect.ToString());

            }


        }

        public void BurnEffect()
        {

            if (external)
            {

                return;

            }

            Cast.Ether.Immolate immolation;

            if (!Mod.instance.eventRegister.ContainsKey("immolate"))
            {

                immolation = new();

                immolation.EventTrigger();

            }
            else
            {

                immolation = Mod.instance.eventRegister["immolate"] as Immolate;

            }

            int vsFarmers = 0;

            if(damageFarmers > 0)
            {

                vsFarmers = (int)(damageFarmers / 3);

            }

            int vsMonsters = 0;

            if(damageMonsters > 0)
            {

                vsMonsters = (int)(damageMonsters / 3);

            }

            immolation.RadialTarget(
                location,
                new((int)(impact.X/64),(int)(impact.Y/64)),
                vsFarmers,
                vsMonsters,
                scheme
            );

        }

        public void WarpEffect()
        {

            if (external)
            {

                return;

            }

            Cast.Fates.WarpStrike warpstrike;

            if (!Mod.instance.eventRegister.ContainsKey("warpstrike"))
            {

                warpstrike = new();

                warpstrike.EventTrigger();

            }
            else
            {

                warpstrike = Mod.instance.eventRegister["warpstrike"] as WarpStrike;
            }

            if (monsters.Count == 0)
            {

                monsters = ModUtility.MonsterProximity(location, new() { impact }, radius + 32, true);

            }

            foreach (var monster in monsters)
            {
                
                warpstrike.AddTarget(location, monster, 5);

            }

        }

        public void GravityEffect()
        {

            if (external)
            {

                return;

            }

            Cast.Fates.Gravity gravity;

            if (!Mod.instance.eventRegister.ContainsKey("gravity"))
            {

                gravity = new();

                gravity.EventTrigger();

            }
            else
            {

                gravity = Mod.instance.eventRegister["gravity"] as Gravity;
            }

            gravity.AddTarget(location, ModUtility.PositionToTile(impact), 4, radius * 2);

        }

        public void HarvestEffect()
        {

            if (external)
            {

                return;

            }

            Cast.Fates.Harvest harvest;

            if (!Mod.instance.eventRegister.ContainsKey("harvest"))
            {

                harvest = new();

                harvest.EventTrigger();

            }
            else
            {

                harvest = Mod.instance.eventRegister["harvest"] as Harvest;
            }

            harvest.AddTarget(location, ModUtility.PositionToTile(impact));

        }*/

    }

}
