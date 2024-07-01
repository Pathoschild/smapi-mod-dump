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
using StardewDruid.Journal;
using StardewValley.Network;
using StardewValley.Quests;
using StardewValley.Tools;
using StardewValley.Locations;
using System.Diagnostics.Metrics;
using StardewValley.GameData.HomeRenovations;
using System.Net.Mail;
using StardewDruid.Cast.Fates;


namespace StardewDruid.Cast
{
    public class SpellHandle
    {

        public GameLocation location;

        public Vector2 destination;

        public Vector2 origin;

        public bool queried;

        public int counter;

        public int radius;

        public int projectile;

        public int projectileIncrements;

        public int projectileTrack;

        public float projectileSpeed;

        public int projectileOffset;

        public bool projectileDirect;

        public float damageFarmers;

        public float damageMonsters;

        public int power;

        public int environment;

        public int terrain;

        public List<TemporaryAnimatedSprite> animations = new();

        public TemporaryAnimatedSprite cursor;

        public TemporaryAnimatedSprite shadow;

        public List<TemporaryAnimatedSprite> construct = new();

        //public TemporaryAnimatedSprite missileAnimation;

        //public TemporaryAnimatedSprite coreAnimation;

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

        public bool shutdown;

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
            crate,
            teleport,
            warpstrike,
            status,
            levitate,
            trick,
        }

        public spells type;

        public IconData.cursors indicator = IconData.cursors.none;

        public IconData.schemes scheme = IconData.schemes.none;

        public IconData.missiles missile = IconData.missiles.none;

        public enum effects
        {
            none,
            sap,
            drain,
            stone,
            embers,
            homing,
            aiming,
            push,
            gravity,
            harvest,
            knock,
            morph,
            mug,
            daze,
            doom,
            immolate,
            blind,
            capture,
        }

        public List<effects> added = new();

        public IconData.impacts display = IconData.impacts.none;

        public enum sounds
        {
            none,
            silent,
            explosion,
            flameSpellHit,
            shadowDie,
            boulderBreak,
            dropItemInWater,
            discoverMineral,
            swordswipe,
            thunder_small,
            thunder,
        }

        public sounds sound = sounds.none;

        public enum lightEffects
        {
            none,
            shadow,
            blaze,

        }

        public lightEffects lighting = lightEffects.none;


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

        public SpellHandle(StardewValley.Monsters.Monster Monster, IconData.displays Status, int Timer)
        {

            location = Game1.player.currentLocation;

            monsters = new() { Monster, };

            origin = Monster.Position;

            destination = Monster.Position;

            impact = destination;

            type = spells.status;

            projectile = (int)Status;

            radius = Timer;

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

            if (counter <= 0)
            {

                return true;

            }

            if (Context.IsMultiplayer && !queried)
            {

                if (!external && !local && type != spells.effect)
                {

                    SpellQuery();

                }

                queried = true;

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

                        ApplyEffects(impact);

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

                        ApplyEffects(impact);

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

                    }

                    AdjustBolt();

                    if(counter == 5)
                    {

                        RadialDisplay();

                        ApplyDamage(impact, radius, -1, damageMonsters, monsters);

                        ApplyEffects(impact);

                    }

                    if (counter == 60)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;

                case spells.orbital:


                    if (counter == 1)
                    {

                        Orbitals();

                        float duration = projectileIncrements * 0.25f;

                        projectileTotal = (projectileIncrements * 15);

                        TargetCircle(duration);

                        TargetLighting(duration);

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

                        ApplyEffects(impact);

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
                        MissileScheme();

                        Projectiles();

                        float duration = projectileIncrements * 0.25f;

                        TargetCircle(duration);

                        TargetLighting(duration);

                        AdjustTarget();

                        if (instant)
                        {

                            counter = 30;

                        }

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

                        if(missile == IconData.missiles.fireball)
                        {

                            Game1.currentLocation.playSound("fireball");

                        }

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

                        ApplyEffects(impact);

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

                        LaunchBeam();

                        LightRadius(origin);

                    }

                    if (counter == 30)
                    {

                        GrazeDamage(1, 3, 3, true);

                    }

                    if (counter == 45)
                    {

                        GrazeDamage(2, 3, 3, true);

                    }

                    if (counter == 60)
                    {

                        ApplyDamage(impact, radius, damageFarmers, damageMonsters, new());

                        RadialDisplay();

                        ApplyEffects(impact);

                    }

                    if (counter == 130)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;

                case spells.echo:

                    if(counter == 30)
                    {

                        LaunchEcho();

                        LightRadius(origin);

                    }

                    if (counter == 90)
                    {

                        ApplyDamage(impact, radius, damageFarmers, damageMonsters, new());

                        RadialDisplay();

                        ApplyEffects(impact);

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

                        ApplyEffects(impact);

                    }

                    if (counter == 240)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;

                case spells.crate:

                    if (counter == 1)
                    {

                        CrateCreate();

                    }

                    if (counter == 61)
                    {

                        Shutdown();

                        CrateOpen();

                        location.playSound("doorCreak");

                    }

                    if (counter == 91)
                    {

                        Shutdown();

                        CrateRelease();

                        location.playSound("yoba");

                    }

                    if(counter >= 300)
                    {
                        Shutdown();

                        return false;

                    }

                    return true;

                case spells.teleport:

                    if(counter == 1)
                    {
                        TeleportStart();

                        if (instant)
                        { 
                            
                            counter = 30; 
                        
                        }

                    }

                    if(counter == 30)
                    {

                        TeleportPlayer();

                    }

                    if(counter == 60)
                    {
                        Shutdown();

                        return false;

                    }

                    return true;

                case spells.status:

                    if(counter == 1)
                    {
                        if (external)
                        {

                            monsters = ModUtility.MonsterProximity(location, new() { origin }, 64, true);

                            if(monsters.Count == 0)
                            {

                                return false;

                            }

                        }

                        CreateStatus();

                    }

                    if (monsters.Count == 0)
                    {

                        return false;

                    }

                    AttachStatus();

                    if (counter == radius)
                    {

                        return false;

                    }

                    return true;

                case spells.warpstrike:

                    if (counter == 1)
                    {
                        
                        if (external)
                        {

                            monsters = ModUtility.MonsterProximity(location, new() { origin }, 64, true);


                        }

                        if (monsters.Count == 0)
                        {

                            return false;

                        }

                        impact = monsters.First().Position;

                        Mod.instance.iconData.AnimateWarpStrike(location, impact, projectile);

                        monsters.First().stunTime.Set(Math.Max(monsters.First().stunTime.Value, 500));

                    }

                    if (counter == 12)
                    {

                        ApplyDamage(impact, radius, damageFarmers, damageMonsters, monsters);

                        if(display != IconData.impacts.none)
                        {

                            Mod.instance.iconData.AnimateWarpSlash(location, impact);

                        }

                    }

                    if(counter == 15)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;

                case spells.trick:

                    TrickDisplay();

                    Shutdown();

                    return false;

            }

            return true;
        }

        public void Shutdown()
        {

            shutdown = true;

            if (animations.Count > 0)
            {

                foreach (TemporaryAnimatedSprite animatedSprite in animations)
                {

                    location.temporarySprites.Remove(animatedSprite);

                }

                animations.Clear();

            }

            if(construct.Count > 0)
            {

                foreach (TemporaryAnimatedSprite animatedSprite in construct)
                {

                    location.temporarySprites.Remove(animatedSprite);

                }

                construct.Clear();

            }

        }

        // ========================================= missile effects

        public void MissileScheme()
        {

            switch (missile)
            {

                case IconData.missiles.death:


                    indicator = IconData.cursors.death;

                    display = IconData.impacts.death;

                    scheme = IconData.schemes.death;

                    break;

            }

        }

        public void Orbitals()
        {

            //indicator = IconData.cursors.scope;

            switch (missile)
            {

                case IconData.missiles.meteor:

                    projectileIncrements = 4;

                    projectileSpeed = 1.5f;

                    projectileOffset = -60 + (30*Mod.instance.randomIndex.Next(6));

                    indicator = IconData.cursors.stars;

                    if(display == IconData.impacts.none)
                    {

                        display = IconData.impacts.bomb;

                    }

                    lighting = lightEffects.shadow;

                    break;

                case IconData.missiles.rockfall:

                    if (projectileSpeed == 0)
                    {

                        projectileSpeed = 1;

                    }

                    projectileIncrements = 10;

                    projectileIncrements = Mod.instance.randomIndex.Next(3, 6);

                    indicator = IconData.cursors.none;

                    lighting = lightEffects.shadow;

                    break;

                case IconData.missiles.death:

                    if (projectileSpeed == 0)
                    {

                        projectileSpeed = 1;

                    }

                    projectileIncrements = 10;

                    indicator = IconData.cursors.death;

                    display = IconData.impacts.death;

                    scheme = IconData.schemes.death;

                    break;

                case IconData.missiles.fireball:

                    projectileIncrements = 4;

                    if(projectileSpeed == 0)
                    {

                        projectileSpeed = 1.5f;

                    }

                    projectileOffset = -60 + (30 * Mod.instance.randomIndex.Next(6));

                    break;

                default:


                    if (projectileSpeed == 0)
                    {

                        projectileSpeed = 1;

                    }

                    projectileIncrements = 10;

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

            if(projectilePeak == 0)
            {
                
                projectilePeak = 60;

            }

        }

        public void TargetCircle(float duration = 1)
        {

            if (indicator == IconData.cursors.none)
            {

                return;

            }

            CursorAdditional addEffects = new() { interval = duration * 1000, scale = projectile, scheme = scheme, alpha = 0.4f, };

            cursor = Mod.instance.iconData.CursorIndicator(location, impact, indicator, addEffects);

            animations.Add(cursor);


        }

        public void TargetLighting(float duration = 1)
        {


            if (lighting == lightEffects.none)
            {

                return;

            }

            CursorAdditional addEffects = new() { interval = duration * 1000, scale = projectile, alpha = 0.35f, rotation = 0f,};

            shadow = Mod.instance.iconData.CursorIndicator(location, impact, IconData.cursors.shadow, addEffects);

            shadow.rotationChange = 0f;

            if (type == spells.orbital)
            {

                shadow.scale = projectile - (duration * 0.5f);

                shadow.position += (new Vector2(8, 8) * duration);

                shadow.motion = new Vector2(-0.008f, -0.008f);

                shadow.scaleChange = 0.0005f;

            }

            animations.Add(shadow);

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

                        Projectiles();

                    }

                }

            }

        }

        public void LaunchMissile()
        {

            float targetDepth = location.IsOutdoors ? destination.Y / 10000 + 0.001f : 999f;

            if (projectileIncrements == 0)
            {

                projectileIncrements = 1;

            }

            if (projectileIncrements > 10)
            {

                projectileIncrements = 10;

            }

            construct = Mod.instance.iconData.MissileConstruct(location, missile, origin, projectile, projectileIncrements, targetDepth, scheme);

        }

        public void AdjustTrajectory(int projectileProgress = 0)
        {

            if(construct.Count == 0)
            {

                return;

            }

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

            Vector2 newPosition = appear - (new Vector2(48, 48) * construct.First().scale) + new Vector2(32, 32);

            Vector2 diff = newPosition - construct.First().position;

            float rotate = (float)Math.Atan2(diff.Y, diff.X);

            for(int i = construct.Count - 1; i >= 0; i--)
            {

                newPosition = appear - (new Vector2(48, 48) * construct[i].scale) + new Vector2(32, 32);

                construct[i].position = newPosition;

                if (construct[i].rotationChange == 0f)
                {

                    construct[i].rotation = rotate;

                }
                else
                {

                    if(origin.X > impact.X)
                    {

                        construct[i].flipped = true;

                    }

                }

                if(type != spells.orbital)
                {

                    construct[i].layerDepth = targetDepth + (i * 0.0001f);

                }

            }

            if(lighting != lightEffects.none)
            {

                if(shadow != null)
                {
                    Vector2 shadowing = new Vector2(projectileOffset * projectileIncrements, 0) * projectileProgress / projectileTotal;

                    shadow.Position = impact - shadowing + new Vector2(32,32) - new Vector2(16 * shadow.scale, 16 * shadow.scale);
   
                }

            }

            /*if (missile != IconData.missiles.none)
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

            }*/

        }


        // ========================================= other combat effects

        public void LaunchBolt()
        {

            if(scheme == IconData.schemes.none)
            {

                scheme = IconData.schemes.bolt;

            }

            construct = Mod.instance.iconData.BoltAnimation(location, new Vector2(destination.X, destination.Y - 64), scheme, projectile);

            animations.AddRange(construct);

            if(sound == sounds.none)
            {

                sound = sounds.flameSpellHit;

            }

        }

        public void AdjustBolt()
        {

            for (int i = 0; i < 3; i++)
            {

                int factor = (5 - counter);

                construct[i].alpha = 1 - Math.Abs(factor) * 0.03f;

            }

        }

        public void LaunchBeam()
        {

            Vector2 diff = (destination - origin) / Vector2.Distance(origin, destination);

            //Vector2 diff = ModUtility.PathFactor(origin, destination);

            Vector2 middle = diff * 320f;

            impact = origin + diff * 560f;

            float rotate = (float)Math.Atan2(diff.Y, diff.X);

            Vector2 setPosition = origin + middle - new Vector2(320f, 64f);

            TemporaryAnimatedSprite beam = new(0, 100f, 12, 1, setPosition, false, false)
            {
                sourceRect = new(0, 0, 160, 32),
                sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                texture = Mod.instance.iconData.laserTexture,
                scale = 4f,
                timeBasedMotion = true,
                layerDepth = 999f,
                rotation = rotate,
                alpha = 0.9f,
                color = Mod.instance.iconData.SchemeColour(scheme),
            };

            location.temporarySprites.Add(beam);

            animations.Add(beam);

            TemporaryAnimatedSprite beamInner = new(0, 100f, 12, 1, setPosition, false, false)
            {
                sourceRect = new(0, 384, 160, 32),
                sourceRectStartingPos = new Vector2(0.0f, 384.0f),
                texture = Mod.instance.iconData.laserTexture,
                scale = 4f,
                timeBasedMotion = true,
                layerDepth = 999f,
                rotation = rotate,
                alpha = 0.9f,
            };

            location.temporarySprites.Add(beamInner);

            animations.Add(beam);

        }

        public void LaunchEcho()
        {

            Vector2 diff = (destination - origin) / Vector2.Distance(origin, destination);

            Vector2 middle = diff * 264f;

            impact = origin + diff * 450f;

            float rotate = (float)Math.Atan2(diff.Y, diff.X);

            Vector2 setPosition = origin + middle - new Vector2(256f, 64f);

            TemporaryAnimatedSprite beam = new(0, 125f, 9, 1, setPosition, false, false)
            {
                sourceRect = new(0, 0, 128, 32),
                sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                texture = Mod.instance.iconData.echoTexture,
                scale = 4f,
                timeBasedMotion = true,
                layerDepth = 999f,
                rotation = rotate,
                alpha = 0.65f,
                color = Mod.instance.iconData.SchemeColour(scheme),

            };

            location.temporarySprites.Add(beam);

            animations.Add(beam);

        }

        public void LaunchBlackhole()
        {

            //TemporaryAnimatedSprite startAnimation = new(0, 1000f, 1, 1, origin, false, false)
            /*TemporaryAnimatedSprite startAnimation = new(0, 1000f, 1, 1, impact - new Vector2(16,16), false, false)
            {

                sourceRect = new(0, 0, 64, 64),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = Mod.instance.iconData.gravityTexture,

                scale = 1.5f,

                scaleChange = 0.0015f,

                layerDepth = location.IsOutdoors ? impact.Y / 10000 + 0.001f : 999f,

                //motion = (impact - origin) / 1000 - new Vector2(96, 96) / 1000,

                motion =  new Vector2(-48, -48) / 1000,

                timeBasedMotion = true,

                rotationChange = -0.06f,

                alpha = 0.75f,

            };

            location.temporarySprites.Add(startAnimation);

            animations.Add(startAnimation);*/

            TemporaryAnimatedSprite staticAnimation = new(0, 99999f, 1, 1, impact - new Vector2(64, 64), false, false)
            {

                sourceRect = new(0, 0, 64, 64),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = Mod.instance.iconData.gravityTexture,

                scale = 3f,

                layerDepth = location.IsOutdoors ? impact.Y / 10000 + 0.001f : 990f,

                rotationChange = -0.06f,

                timeBasedMotion = true,

                //delayBeforeAnimationStart = 1000,

                alpha = 0.75f,

            };

            location.temporarySprites.Add(staticAnimation);

            animations.Add(staticAnimation);

            TemporaryAnimatedSprite bandAnimation = new(0, 9999f, 1, 1, impact - new Vector2(64, 64), false, false)
            {

                sourceRect = new(64, 0, 64, 64),

                sourceRectStartingPos = new Vector2(64, 0),

                texture = Mod.instance.iconData.gravityTexture,

                scale = 3f,

                layerDepth = location.IsOutdoors ? impact.Y / 10000 + 0.002f : 991f,

                timeBasedMotion = true,

                //delayBeforeAnimationStart = 1000,

                alpha = 0.75f,

            };

            location.temporarySprites.Add(bandAnimation);

            animations.Add(bandAnimation);

            TemporaryAnimatedSprite cloudAnimation = Mod.instance.iconData.ImpactIndicator(location, impact, IconData.impacts.spiral, 8f, new() { loops = 10, flip = true, alpha = 0.05f, layer = location.IsOutdoors ? impact.Y / 10000 : 990f });

            //cloudAnimation.delayBeforeAnimationStart = 1000;

            animations.Add(cloudAnimation);

        }

        // ========================================= end

        public void RadialDisplay()
        {

            LightRadius(impact);

            if (display != IconData.impacts.none)
            {

                float radial = Math.Min(6,(int)(radius / 64)) - 0.5f;

                Mod.instance.iconData.ImpactIndicator(location, impact, display, radial, new() { alpha = 0.5f, scheme = scheme,});

            }

            if (sound != sounds.none && sound != sounds.silent)
            {

                Game1.currentLocation.playSound(sound.ToString());

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

        public void GrazeDamage(int piece, int division, float reach = -1, bool effects = false)
        {

            if (!Context.IsMainPlayer)
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

            if (effects)
            {
                
                ApplyEffects(current);

            }

        }

        public void ApplyDamage(Vector2 position, float reach, float hitfarmers, float hitmonsters, List<StardewValley.Monsters.Monster> individuals)
        {

            if (!Context.IsMainPlayer)
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

                ModUtility.DamageMonsters(individuals, Game1.player, (int)hitmonsters, critical, criticalModifier, push);

            }

        }

        public void ApplyEffects(Vector2 zone)
        {
            
            if (!Context.IsMainPlayer)
            {

                return;

            }

            foreach (effects effect in added)
            {

                switch (effect)
                {
                    case effects.drain:

                        DrainEffect();

                        break;

                    case effects.sap:

                        SapEffect();

                        break;

                    case effects.stone:

                        StoneEffect();

                        break;

                    case effects.knock:
                    case effects.morph:
                    case effects.mug:
                    case effects.daze:
                    case effects.doom:
                    case effects.immolate:

                        CurseEffect(effect);

                        break;

                    case effects.embers:

                        EmberEffect(zone);

                        break;

                    case effects.gravity:

                        GravityEffect();

                        break;

                    case effects.harvest:

                        HarvestEffect();

                        break;

                    case effects.capture:

                        CaptureEffect();

                        break;


                }

            }


        }

        public void RadialExplode()
        {

            if (!Context.IsMainPlayer)
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

            int leech = 0;

            float impes = 0.1f;

            if (Mod.instance.herbalData.applied.ContainsKey(HerbalData.herbals.impes))
            {

                impes = 0.1f * Mod.instance.herbalData.applied[HerbalData.herbals.impes].level;

            }

            int drain = Math.Min(4,(int)(Mod.instance.CombatDamage() * impes));

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

        public void DrainEffect()
        {


            Vector2 mistCorner = Game1.player.Position - new Vector2(96, 128);

            for (int i = 0; i < 4; i++)
            {

                for (int j = 0; j < 4; j++)
                {

                    if ((i == 0 || i == 5) && (j == 0 || j == 5))
                    {
                        continue;
                    }

                    Vector2 glowVector = mistCorner + new Vector2(i * 32, j * 32);

                    TemporaryAnimatedSprite glowSprite = new TemporaryAnimatedSprite(0, 3000f, 1, 1, glowVector, false, false)
                    {
                        sourceRect = new Rectangle(88, 1779, 30, 30),
                        sourceRectStartingPos = new Vector2(88, 1779),
                        texture = Game1.mouseCursors,
                        motion = new(0.016f * (Mod.instance.randomIndex.Next(2) == 0 ? 1 : -1) * Mod.instance.randomIndex.Next(1, 4), 0.016f * (Mod.instance.randomIndex.Next(2) == 0 ? 1 : -1) * Mod.instance.randomIndex.Next(1, 4)),
                        scale = 4f,
                        layerDepth = 999f,
                        timeBasedMotion = true,
                        alpha = 1f,
                        alphaFade = 0.0005f,
                        color = new Color(0.75f, 0.75f, 1f, 1f),
                    };

                    Game1.player.currentLocation.temporarySprites.Add(glowSprite);

                }

            }

            float impes = 0.1f;

            if (Mod.instance.herbalData.applied.ContainsKey(HerbalData.herbals.impes))
            {

                impes = 0.1f * Mod.instance.herbalData.applied[HerbalData.herbals.impes].level;

            }

            int drain = Math.Min(10, (int)(damageMonsters * impes));

            int num = Math.Min(drain, Game1.player.maxHealth - Game1.player.health);

            if (num > 0)
            {

                Game1.player.health += num;

                Rectangle healthBox = Game1.player.GetBoundingBox();

                Game1.player.currentLocation.debris.Add(
                    new Debris(
                        num,
                        new Vector2(healthBox.Center.X + 16, healthBox.Center.Y),
                        Color.Green,
                        1f,
                        Game1.player
                    )
                );

            }

        }

        public void CurseEffect(effects effect = effects.knock)
        {

            Curse curseEffect;

            if (!Mod.instance.eventRegister.ContainsKey("curse"))
            {

                curseEffect = new();

                curseEffect.eventId = "curse";

                curseEffect.EventActivate();

            }
            else
            {

                curseEffect = Mod.instance.eventRegister["curse"] as Curse;

            }

            if (monsters.Count == 0)
            {

                List<StardewValley.Monsters.Monster> monsterTargets = ModUtility.MonsterProximity(location, new() { impact }, radius + 32, true);

                foreach (var monster in monsterTargets)
                {

                    curseEffect.AddTarget(location, monster, effect);

                }

                return;

            }

            foreach (var monster in monsters)
            {

                curseEffect.AddTarget(location, monster, effect);

            }

        }

        public void EmberEffect(Vector2 zone)
        {

            Ember ember;

            if (!Mod.instance.eventRegister.ContainsKey("emberEffect"))
            {

                ember = new();

                ember.eventId = "emberEffect";

                ember.EventActivate();

            }
            else
            {

                ember = Mod.instance.eventRegister["emberEffect"] as Ember;

            }

            int vsFarmers = 0;

            if (damageFarmers > 0)
            {

                vsFarmers = (int)(damageFarmers / 5);

            }

            int vsMonsters = 0;

            if (damageMonsters > 0)
            {

                vsMonsters = (int)(damageMonsters / 5);

            }

            ember.RadialTarget(
                location,
                ModUtility.PositionToTile(zone),
                vsFarmers,
                vsMonsters,
                scheme
            );

        }

        public void GravityEffect()
        {

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

        public void CaptureEffect()
        {

            foreach(StardewDruid.Character.Character character in ModUtility.GetFriendsInLocation(location))
            {

                if (character.netDazeActive.Value)
                {

                    continue;

                }

                if (!character.ChangeBehaviour(true))
                {

                    continue;

                }

                if(Vector2.Distance(character.Position,impact) <= radius + 32)
                {

                    character.ResetActives();

                    character.netHaltActive.Set(true);

                    character.LookAtTarget(origin);

                    character.netDazeActive.Set(true);

                    character.idleTimer = radius * 2;

                }

            }

        }

        // ========================================= crate

        public void CrateCreate()
        {

            TemporaryAnimatedSprite crate = new(0, 1000, 1, 1, origin + new Vector2(16, 0), false, false)
            {

                sourceRect = new(0, 0, 32, 64),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = Mod.instance.iconData.crateTexture,

                scale = 1f, //* size,

                layerDepth = origin.Y / 10000,

                scaleChange = 0.001f,

                motion = new Vector2(-0.016f, -0.064f),

                timeBasedMotion = true,

            };

            location.temporarySprites.Add(crate);

            animations.Add(crate);

        }

        public void CrateOpen()
        {

            TemporaryAnimatedSprite crateOpen = new(0, 167, 3, 1, origin - new Vector2(0, 64), false, false)
            {

                sourceRect = new(0, 0, 32, 64),

                sourceRectStartingPos = new Vector2(64, 0),

                texture = Mod.instance.iconData.crateTexture,

                scale = 2f,

                layerDepth = origin.Y / 10000,

            };

            location.temporarySprites.Add(crateOpen);

            animations.Add(crateOpen);

        }

        public void CrateRelease()
        {

            TemporaryAnimatedSprite crateOpen = new(0, 3000, 1, 1, origin - new Vector2(0, 64), false, false)
            {

                sourceRect = new(64, 0, 32, 64),

                sourceRectStartingPos = new Vector2(64, 0),

                texture = Mod.instance.iconData.crateTexture,

                scale = 2f,

                layerDepth = origin.Y / 10000,

            };

            location.temporarySprites.Add(crateOpen);

            animations.Add(crateOpen);

            StardewValley.Item candidate = SpawnData.CrateTreasure(location,impact);

            if(candidate is MeleeWeapon candidateWeapon)
            {

                new ThrowHandle(Game1.player, origin, candidateWeapon).register();

            }
            else if (candidate is StardewValley.Object candidateObject)
            {

                new ThrowHandle(Game1.player, origin, candidateObject).register();

            }

            Mod.instance.iconData.ImpactIndicator(location, origin - new Vector2(0, 32), IconData.impacts.sparkle, 2, new() { layer = origin.Y / 10000 + 0.001f,});

        }

        // ========================================= teleport

        public void TeleportStart()
        {

            Mod.instance.iconData.AnimateQuickWarp(Game1.player.currentLocation, origin, true);

        }

        public void TeleportPlayer()
        {
            if (!external)
            {

                Game1.player.Position = destination;

            }

            Mod.instance.iconData.AnimateQuickWarp(Game1.player.currentLocation, destination);

        }

        // ========================================= status

        public void CreateStatus()
        {

            cursor = Mod.instance.iconData.StatusIndicator(location, monsters.First().Position, (IconData.displays)projectile);

        }

        public void AttachStatus()
        {

            Microsoft.Xna.Framework.Rectangle monsterRect = monsters.First().GetBoundingBox();

            cursor.reset();

            cursor.Position = new(monsterRect.Center.X - 24, monsterRect.Top - 80);

        }

        // ========================================= trick

        public void TrickDisplay()
        {

            switch (projectile)
            {

                default:
                case 0:
                    ModUtility.AnimateRandomCritter(location, ModUtility.PositionToTile(impact));
                    break;

                case 1:
                    ModUtility.AnimateRandomFish(location, ModUtility.PositionToTile(impact));
                    break;

                case 2:
                    ModUtility.AnimateButterflySpray(location, ModUtility.PositionToTile(impact));
                    break;
            }

        }

    }

}
