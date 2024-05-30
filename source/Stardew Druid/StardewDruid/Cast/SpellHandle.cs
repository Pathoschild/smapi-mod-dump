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
using Microsoft.VisualBasic;
using StardewValley.Characters;
using static StardewDruid.Cast.Rite;
using System.Security.Cryptography.X509Certificates;
using StardewDruid.Cast.Effect;
using System.Xml.Linq;
using xTile;
using StardewDruid.Cast.Mists;
using System.Reflection.Emit;
using StardewValley.GameData.Characters;


namespace StardewDruid.Cast
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

        public float projectileSpeed;

        public int projectileOffset;

        public float damageFarmers;

        public float damageMonsters;

        public int power;

        public int environment;

        public int terrain;

        public List<TemporaryAnimatedSprite> animations = new();

        public TemporaryAnimatedSprite cursor;

        public TemporaryAnimatedSprite missileAnimation;

        public TemporaryAnimatedSprite coreAnimation;

        public int projectileTotal;

        public int projectilePeak;

        public Vector2 impact;

        public Monster.Boss boss;

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
            orbital,
            ballistic,
            beam,
            missile,
            bolt,
            blackhole,
            echo,
        }

        public spells type;

        public enum cursorBehaviours
        {
            none,
            rotate,
            shrink,
            enlarge,

        }

        public IconData.cursors indicator = IconData.cursors.none;

        public cursorBehaviours indicatorBehaviour = cursorBehaviours.none;

        public IconData.schemes scheme = IconData.schemes.none;

        public IconData.missiles missile = IconData.missiles.none;

        public IconData.schemes core = IconData.schemes.none;

        public IconData.cursors coreCursor = IconData.cursors.none;

        public cursorBehaviours coreBehaviour = cursorBehaviours.none;

        public enum effects
        {
            sap,
            drain,
            stone,
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

        public IconData.impacts display = IconData.impacts.none;

        public IconData.schemes displayScheme = IconData.schemes.none;

        public enum sounds
        {
            none,
            explosion,
            flameSpellHit,
            shadowDie,
            boulderBreak,
            dropItemInWater,
        }

        public sounds sound = sounds.none;


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

        }

        public SpellHandle(Vector2 Destination, int Radius, IconData.impacts Display, List<effects> Added)
        {

            location = Game1.player.currentLocation;

            origin = Destination;

            destination = Destination;

            radius = Radius;

            projectile = 2;

            damageFarmers = -1f;

            damageMonsters = -1f;

            impact = Destination;

            type = spells.effect;

            display = Display;

            sound = sounds.none;

            added = Added;

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
                Convert.ToInt32(missile),
            };

            QueryData query = new()
            {
                name = type.ToString(),

                value = System.Text.Json.JsonSerializer.Serialize(array),

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

            type = (spells)spellData[5];

            scheme = (IconData.schemes)spellData[6];

            indicator = (IconData.cursors)spellData[7];

            display = (IconData.impacts)spellData[8];

            sound = sounds.none;

            projectile = spellData[9];

            missile = (IconData.missiles)spellData[10];

        }

        public void register()
        {

            Mod.instance.spellRegister.Add(this);

        }

        public bool Update()
        {

            counter++;

            if (counter == 1)
            {

                if (Context.IsMultiplayer && !external && !local && type != spells.effect)
                {

                    SpellQuery();

                }

            }

            if (counter % 10 == 0)
            {

                if (boss != null)
                {

                    if (!ModUtility.MonsterVitals(boss, location))
                    {

                        Shutdown();

                        return false;

                    }

                }

                if (monsters.Count > 0)
                {

                    for (int m = monsters.Count - 1; m >= 0; m--)
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

                        RadialDisplay();

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

                        ApplyDamage(impact, radius, damageFarmers, damageMonsters, new());

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

                case spells.bolt:

                    if (counter == 1)
                    {

                        LaunchBolt();

                        ApplyDamage(impact, radius, -1, damageMonsters, monsters);

                        return true;

                    }

                    if (counter == 30)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;

                case spells.orbital:


                    if (counter == 1)
                    {

                        Scheme();

                        Orbitals();

                        float duration = projectileIncrements * 0.25f;

                        projectileTotal = (projectileIncrements * 15);

                        TargetCircle(duration);

                        LaunchMissile();

                    }

                    if (counter < projectileTotal)
                    {

                        AdjustTrajectory(projectileTotal - counter);

                    }

                    if (counter == projectileTotal)
                    {

                        ApplyDamage(impact, radius, damageFarmers, damageMonsters, new());

                        RadialExplode();

                        RadialDisplay();

                        ApplyEffects();

                    }

                    if (counter == projectileTotal + 60)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;


                case spells.missile:
                case spells.ballistic:

                    if (counter == 1)
                    {
                        Scheme();

                        Projectiles();

                        float duration = projectileIncrements * 0.25f;

                        TargetCircle(duration);

                        InstantFire();

                    }

                    if (counter < 30)
                    {

                        AdjustTarget();

                        return true;

                    }

                    if (counter == 30)
                    {

                        projectileTrack = 1;

                        projectileTotal = (projectileIncrements * 15);

                        Game1.currentLocation.playSound("fireball");

                        LaunchMissile();

                        AdjustTrajectory();

                        return true;

                    }

                    if (counter < (30 + projectileTotal))
                    {

                        AdjustTrajectory(projectileTotal - (counter - 30));

                        if (counter % 30 == 0)
                        {

                            GrazeDamage(projectileTrack, projectileIncrements);

                            projectileTrack++;

                        }

                    }

                    if (counter == (30 + projectileTotal))
                    {

                        ApplyDamage(impact, radius, damageFarmers, damageMonsters, new());

                        RadialExplode();

                        RadialDisplay();

                        ApplyEffects();

                    }

                    if (counter == (90 + projectileTotal))
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

                case spells.echo:

                    if (counter == 1)
                    {

                        TargetCircle();

                    }

                    if(counter == 30)
                    {

                        LaunchEcho();

                        LightRadius(origin);

                    }

                    if (counter == 90)
                    {

                        ApplyDamage(impact, radius, damageFarmers, damageMonsters, new());

                        RadialDisplay();

                        ApplyEffects();

                    }

                    if (counter == 150)
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

        public void Scheme()
        {

            switch (scheme)
            {

                case IconData.schemes.stars:

                    missile = IconData.missiles.sparks;

                    coreCursor = IconData.cursors.comet;

                    coreBehaviour = cursorBehaviours.rotate;

                    indicator = IconData.cursors.stars;

                    break;

                case IconData.schemes.fates:

                    missile = IconData.missiles.sparks;

                    coreCursor = IconData.cursors.fatesCharge;

                    break;

                case IconData.schemes.rock:

                    core = scheme;

                    missile = IconData.missiles.rocks;

                    coreCursor = IconData.cursors.rock;

                    break;

                case IconData.schemes.rockTwo:

                    core = scheme;

                    missile = IconData.missiles.rocks;

                    coreCursor = IconData.cursors.rockTwo;

                    break;

                case IconData.schemes.death:

                    core = scheme;

                    indicator = IconData.cursors.death;

                    coreCursor = IconData.cursors.skull;

                    display = IconData.impacts.death;

                    break;

                case IconData.schemes.fire:
                case IconData.schemes.ether:

                    core = scheme;

                    missile = IconData.missiles.blaze;

                    indicator = IconData.cursors.arrow;

                    break;

            }

        }

        public void Orbitals()
        {

            if (projectileSpeed == 0)
            {

                projectileSpeed = 1;

            }

            projectileIncrements = 10;

            indicator = IconData.cursors.scope;

            indicatorBehaviour = cursorBehaviours.rotate;

            switch (scheme)
            {

                case IconData.schemes.stars:

                    projectileIncrements = 5;

                    projectileOffset = 60;

                    indicator = IconData.cursors.stars;

                    break;

                case IconData.schemes.rock:
                case IconData.schemes.rockTwo:

                    projectileIncrements = Mod.instance.randomIndex.Next(3, 6);

                    //projectileSpeed = 1.25f;

                    indicator = IconData.cursors.shadow;

                    indicatorBehaviour = cursorBehaviours.enlarge;

                    //coreBehaviour = cursorBehaviours.shrink;

                    break;

            }

        }

        public void Projectiles()
        {

            if (projectileSpeed == 0)
            {

                projectileSpeed = 1;

            }

            if (type == spells.ballistic)
            {

                projectilePeak = 512;

                projectileIncrements = 8;

                return;

            }

            float range = Vector2.Distance(origin, impact);

            float netSpeed = 160 * projectileSpeed;

            if (range < (netSpeed * 2))
            {

                Vector2 diff = (impact - origin) / Vector2.Distance(origin, impact) * (netSpeed * 2);

                impact = origin + diff;

                projectileIncrements = 2;

            }
            else if (range > (netSpeed * 6))
            {

                Vector2 diff = (impact - origin) / Vector2.Distance(origin, impact) * (netSpeed * 6);

                impact = origin + diff;

                projectileIncrements = 6;

            }
            else
            {

                projectileIncrements = (int)(range / netSpeed);

            }

            projectilePeak = 60;


        }

        public void TargetCircle(float duration = 1)
        {

            if (indicator == IconData.cursors.none)
            {

                return;

            }

            CursorAdditional addEffects = new() { interval = duration * 1000, scale = projectile + 1, color = Mod.instance.iconData.schemeColours[scheme], alpha = 0.5f, };

            TemporaryAnimatedSprite targetCircle = Mod.instance.iconData.CursorIndicator(location, impact, indicator, addEffects);

            if (indicator == IconData.cursors.arrow)
            {

                Vector2 diff = impact - origin;

                float rotate = (float)Math.Atan2(diff.Y, diff.X);

                targetCircle.rotation = rotate;

                targetCircle.rotationChange = 0f;

            }

            if (indicator == IconData.cursors.shadow)
            {

                targetCircle.rotationChange = 0f;

            }

            if (indicatorBehaviour == cursorBehaviours.enlarge)
            {

                targetCircle.scale = 2f - (duration * 0.5f);

                targetCircle.position += (new Vector2(8,8) * duration);

                targetCircle.motion = new Vector2(-0.008f, -0.008f);

                targetCircle.scaleChange = 0.0005f;

            }

            animations.Add(targetCircle);

            cursor = targetCircle;

        }

        public void AdjustTarget()
        {

            if (added.Contains(effects.aiming) && !external)
            {

                if (damageMonsters > 0 && monsters.Count > 0)
                {

                    if (ModUtility.MonsterVitals(monsters.First(), location))
                    {

                        impact = monsters.First().Position;

                        if (cursor != null)
                        {

                            Vector2 diff = impact - origin;

                            float rotate = (float)Math.Atan2(diff.Y, diff.X);

                            cursor.rotation = rotate;

                        }

                        Projectiles();

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

        public void AdjustTrajectory(int projectileProgress = 0)
        {

            Vector2 from = origin;

            switch(type)
            {

                case spells.orbital:

                    float netSpeed = 160 * projectileSpeed;

                    from = impact - new Vector2(projectileOffset * projectileIncrements, netSpeed * projectileIncrements);

                    break;

            }

            Vector2 appear = from;

            if(projectileProgress != projectileTotal)
            {

                Vector2 shift = (impact - from) * (projectileTotal - projectileProgress) / projectileTotal;

                appear = from + shift;

                if (projectilePeak > 0)
                {

                    float distance = Vector2.Distance(from, impact);

                    float length = distance / 2;

                    float lengthSq = (length * length);

                    float heightFr = 4 * projectilePeak;

                    float coefficient = lengthSq / heightFr;

                    int midpoint = (projectileTotal / 2);

                    float newHeight = 0;

                    if (projectileProgress != midpoint)
                    {

                        float newLength;

                        if (projectileProgress < midpoint)
                        {

                            newLength = length * (midpoint - projectileProgress) / midpoint;

                        }
                        else
                        {

                            newLength = (length * (projectileProgress - midpoint) / midpoint);

                        }

                        float newLengthSq = newLength * newLength;

                        float coefficientFr = (4 * coefficient);

                        newHeight = newLengthSq / coefficientFr;

                    }

                    appear -= new Vector2(0, projectilePeak - newHeight);

                }

            }

            float targetDepth = location.IsOutdoors ? appear.Y / 10000 + 0.001f : 999f;

            if (missile != IconData.missiles.none)
            {

                Vector2 newPosition = appear - (new Vector2(48, 48) * projectile) + new Vector2(32, 32);

                Vector2 diff = newPosition - missileAnimation.position;

                float rotate = (float)Math.Atan2(diff.Y, diff.X);

                missileAnimation.position = newPosition;

                missileAnimation.rotation = rotate;

                missileAnimation.layerDepth = targetDepth;

            }

            if (coreCursor != IconData.cursors.none)
            {

                coreAnimation.position = appear - (new Vector2(16f, 16f) * projectile) + new Vector2(32, 32);

                coreAnimation.layerDepth = targetDepth + 0.0001f;

                if (coreBehaviour == cursorBehaviours.shrink)
                {

                    coreAnimation.position += new Vector2(8, 8);

                }

            }
        }

        // ========================================= start

        public void LaunchBolt()
        {

            Mod.instance.iconData.AnimateBolt(location, new Vector2(destination.X, destination.Y - 64));

        }

        public void LaunchMissile()
        {

            float targetDepth = location.IsOutdoors ? destination.Y / 10000 + 0.001f : 999f;

            if (projectileIncrements == 0)
            {

                projectileIncrements = 1;

            }

            if(projectileIncrements > 10)
            {

                projectileIncrements = 10;

            }

            if(missile != IconData.missiles.none)
            {

                Rectangle rect = IconData.MissileRectangles(missile);

                Vector2 setat = origin - (new Vector2(48, 48) * projectile) + new Vector2(32,32);
                
                int loops = (int)Math.Ceiling(projectileIncrements * 0.5);

                missileAnimation = new(0, (projectileIncrements * 250) / (loops * 4), 4, loops, setat, false, false)
                {

                    sourceRect = rect,

                    sourceRectStartingPos = new Vector2(rect.X, rect.Y),

                    texture = Mod.instance.iconData.missileTexture,

                    scale = projectile,

                    layerDepth = targetDepth,

                    timeBasedMotion = true,

                    alpha = 0.75f,

                    color = Mod.instance.iconData.schemeColours[scheme],

                };

                location.temporarySprites.Add(missileAnimation);

                animations.Add(missileAnimation);

            }

            if (coreCursor != IconData.cursors.none)
            {

                Vector2 setattwo = origin - (new Vector2(16f, 16f) * projectile) + new Vector2(32, 32);

                Rectangle cursorRect = Mod.instance.iconData.CursorRect(coreCursor);

                coreAnimation = new(0, 250 * projectileIncrements, 1, 1, setattwo, false, false)
                {

                    sourceRect = cursorRect,

                    sourceRectStartingPos = new Vector2(cursorRect.X, cursorRect.Y),

                    texture = Mod.instance.iconData.cursorTexture,

                    scale = projectile,

                    layerDepth = targetDepth + 0.0001f,

                    timeBasedMotion = true,

                    alpha = 0.85f,

                    color = Mod.instance.iconData.schemeColours[core],

                };

                if (coreBehaviour == cursorBehaviours.rotate)
                {

                    coreAnimation.rotationChange = (float)(Math.PI / 60);

                }

                if (coreBehaviour == cursorBehaviours.shrink)
                {

                    coreAnimation.scale = projectile - 0.5f;

                }

                location.temporarySprites.Add(coreAnimation);

                animations.Add(coreAnimation);

            }

        }

        public void LaunchBeam()
        {

            Texture2D beamTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Laser.png"));

            Vector2 diff = (destination - origin) / Vector2.Distance(origin, destination);

            Vector2 middle = diff * 336f;

            impact = origin + diff * 560f;

            cursor.position = origin + diff * 640f - new Vector2(16f, 16f);

            float rotate = (float)Math.Atan2(diff.Y, diff.X);

            Vector2 setPosition = origin + middle - new Vector2(320f, 64f);

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
                color = Mod.instance.iconData.schemeColours[scheme],
            };

            location.temporarySprites.Add(beam);

            animations.Add(beam);

        }

        public void LaunchEcho()
        {

            Texture2D beamTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Echo.png"));

            Vector2 diff = (destination - origin) / Vector2.Distance(origin, destination);

            Vector2 middle = diff * 264f;

            impact = origin + diff * 450f;

            cursor.position = origin + diff * 520f - new Vector2(16f, 16f);

            float rotate = (float)Math.Atan2(diff.Y, diff.X);

            Vector2 setPosition = origin + middle - new Vector2(256f, 64f);

            TemporaryAnimatedSprite beam = new(0, 125f, 9, 1, setPosition, false, false)
            {
                sourceRect = new(0, 0, 128, 32),
                sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                texture = beamTexture,
                scale = 4f,
                timeBasedMotion = true,
                layerDepth = 999f,
                rotation = rotate,
                alpha = 0.65f,
                color = Mod.instance.iconData.schemeColours[scheme],

            };

            location.temporarySprites.Add(beam);

            animations.Add(beam);

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

                layerDepth = location.IsOutdoors ? impact.Y / 10000 + 0.001f : 999f,

                motion = (impact - origin) / 1000 - new Vector2(96, 96) / 1000,

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

                layerDepth = location.IsOutdoors ? impact.Y / 10000 + 0.001f : 990f,

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

                layerDepth = location.IsOutdoors ? impact.Y / 10000 + 0.002f : 991f,

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

            if (display != IconData.impacts.none)
            {

                Mod.instance.iconData.ImpactIndicator(location, impact, display, radius / 64, new() { color = Mod.instance.iconData.schemeColours[displayScheme], });

            }

            if (sound != sounds.none)
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

            if (reach == -1)
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

                if (individuals.Count == 0)
                {

                    individuals = ModUtility.MonsterProximity(location, new() { position }, reach + 32, true);

                }

                if (individuals.Count == 0)
                {

                    return;

                }

                bool push = false;

                foreach (effects effect in added)
                {

                    switch (effect)
                    {

                        case effects.push:

                            push = true;

                            break;

                    }

                }

                if (critical == 0f)
                {

                    List<float> criticals = Mod.instance.CombatCritical();

                    critical = criticals[0];

                    if (criticalModifier == 0f)
                    {

                        criticalModifier = criticals[1];

                    }

                }

                if (criticalModifier == 0f)
                {

                    List<float> criticals = Mod.instance.CombatCritical();

                    criticalModifier = criticals[1];

                }

                ModUtility.DamageMonsters(location, individuals, Game1.player, (int)hitmonsters, critical, criticalModifier, push);

            }

        }

        public void ApplyEffects()
        {

            foreach (effects effect in added)
            {

                switch (effect)
                {
                    case effects.sap:

                        SapEffect();

                        break;

                    case effects.stone:

                        StoneEffect();

                        break;

                    case effects.knock:
                    case effects.doom:
                    case effects.morph:

                        CurseEffect(effect);

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

                }

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

                if (environment == 0)
                {

                    environment = radius / 64;

                }

                ModUtility.Explode(location, impact / 64, Game1.player, environment, power);

            }

            if (terrain > 0)
            {

                ModUtility.Reave(location, impact / 64, Game1.player, terrain);

            }

        }

        // ========================================= effects

        public void StoneEffect()
        {

            if (external)
            {

                return;

            }


            Random randomIndex = new();

            int rockCut = randomIndex.Next(2);

            int generateAmt = Math.Max(1, randomIndex.Next(Mod.instance.PowerLevel));

            if (!Mod.instance.questHandle.IsComplete(StardewDruid.Journal.QuestHandle.wealdFive))
            {

                Mod.instance.questHandle.UpdateTask(StardewDruid.Journal.QuestHandle.wealdFive, generateAmt);

            }

            Vector2 targetVector = destination / 64;

            for (int i = 0; i < generateAmt; i++)
            {
                
                if (i == 0)
                {

                    int debris = SpawnData.RockFall(location, Game1.player, 20 - Mod.instance.PowerLevel * 2)[2];

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

            Game1.player.gainExperience(3,1);

        }

        public void SapEffect()
        {

            if (external)
            {

                return;

            }

            int leech = 0;

            int drain = 4 + Mod.instance.PowerLevel * 2;

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

                Rectangle boundingBox = monster.GetBoundingBox();

                Color color = Color.DarkGreen;

                Mod.instance.iconData.ImpactIndicator(location, monster.Position, IconData.impacts.glare, 1f, new() { color = Color.Teal, });

                location.debris.Add(new Debris(drain, new Vector2(boundingBox.Center.X + 16, boundingBox.Center.Y), color, 1f, monster));

                leech += drain;

            }

            int num = Math.Min(leech, Game1.player.MaxStamina - (int)Game1.player.stamina);

            if (num > 0)
            {

                Game1.player.stamina += num;

                Rectangle healthBox = Game1.player.GetBoundingBox();

                location.debris.Add(
                    new Debris(
                        num,
                        new Vector2(healthBox.Center.X + 16, healthBox.Center.Y),
                        Color.Teal,
                        0.75f,
                        Game1.player
                    )
                );

            }

        }

        public void CurseEffect(effects effect = effects.knock)
        {

            if (external)
            {

                return;

            }

            Curse Knockout;

            if (!Mod.instance.eventRegister.ContainsKey("curseEffect"))
            {

                Knockout = new();

                Knockout.eventId = "curseEffect";

                Knockout.EventActivate();

            }
            else
            {

                Knockout = Mod.instance.eventRegister["curseEffect"] as Curse;

            }

            if (monsters.Count == 0)
            {

                monsters = ModUtility.MonsterProximity(location, new() { impact }, radius + 32, true);

            }

            foreach (var monster in monsters)
            {

                Knockout.AddTarget(location, monster, effect);

            }


        }

        public void BurnEffect()
        {

            if (external)
            {

                return;

            }

            Immolate immolation;

            if (!Mod.instance.eventRegister.ContainsKey("immolateEffect"))
            {

                immolation = new();

                immolation.eventId = "immolateEffect";

                immolation.EventActivate();

            }
            else
            {

                immolation = Mod.instance.eventRegister["immolateEffect"] as Immolate;

            }

            int vsFarmers = 0;

            if (damageFarmers > 0)
            {

                vsFarmers = (int)(damageFarmers / 3);

            }

            int vsMonsters = 0;

            if (damageMonsters > 0)
            {

                vsMonsters = (int)(damageMonsters / 3);

            }

            immolation.RadialTarget(
                location,
                ModUtility.PositionToTile(impact),
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

            WarpStrike warpstrike;

            if (!Mod.instance.eventRegister.ContainsKey("warpstrikeEffect"))
            {

                warpstrike = new();

                warpstrike.eventId = "warpstrikeEffect";

                warpstrike.EventActivate();

            }
            else
            {

                warpstrike = Mod.instance.eventRegister["warpstrikeEffect"] as WarpStrike;
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

            Gravity gravity;

            if (!Mod.instance.eventRegister.ContainsKey("gravityEffect"))
            {
                gravity = new();

                gravity.eventId = "gravityEffect";

                gravity.EventActivate();

            }
            else
            {

                gravity = Mod.instance.eventRegister["gravityEffect"] as Gravity;
            }

            gravity.AddTarget(location, ModUtility.PositionToTile(impact), 4, radius * 2);

        }

        public void HarvestEffect()
        {

            if (external)
            {

                return;

            }

            Harvest harvest;

            if (!Mod.instance.eventRegister.ContainsKey("harvestEffect"))
            {

                harvest = new();

                harvest.eventId = "harvestEffect";

                harvest.EventActivate();

            }
            else
            {

                harvest = Mod.instance.eventRegister["harvestEffect"] as Harvest;
            }

            harvest.AddTarget(location, ModUtility.PositionToTile(impact));

        }

    }

}
