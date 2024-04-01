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
using StardewDruid.Cast.Ether;
using Force.DeepCloner;
using StardewDruid.Cast.Weald;
using static System.Net.Mime.MediaTypeNames;
using StardewValley.GameData.HomeRenovations;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using StardewModdingAPI;


namespace StardewDruid.Event
{
    public class SpellHandle
    {

        public GameLocation location;

        public Vector2 destination;

        public Vector2 origin;

        public int missiles;

        public int counter;

        public int radius;

        public float damageFarmers;

        public float damageMonsters;

        public int power;

        public int environment;

        public int terrain;

        public int debris;

        public List<TemporaryAnimatedSprite> animations;
        
        public List<TemporaryAnimatedSprite> aoes;

        public Vector2 impact;

        public StardewValley.Monsters.Monster monster;

        public float critical;

        public bool external;

        public enum barrages
        {
            ballistic,
            beam,
            burn,
            fireball,
            wisp,
            chaos,
            meteor,
            bolt,
            rockfall,
        }

        public barrages type;

        public enum indicators
        {
            target,
            stars,
            fates,
            death,
        }

        public indicators indicator;

        public enum schemes
        {
            fire,
            stars,
            fates,
            ether,
            death,
        }

        public schemes scheme;

        public SpellHandle(GameLocation Location, Vector2 Destination, Vector2 Origin, int Radius = 2, int Missiles = 1, float Farmers = -1, float Monsters = -1, int Power = -1)
        {

            location = Location;

            origin = Origin;

            destination = Destination;

            radius = Radius;

            damageFarmers = Farmers;

            damageMonsters = Monsters;

            power = Power;

            environment = radius;

            terrain = 0;

            debris = 0;

            missiles = Missiles;

            animations = new();

            aoes = new();

            impact = Destination;

            type = barrages.fireball;

            scheme = schemes.fire;

            indicator = indicators.target;

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

        public bool Update()
        {

            counter++;

            if (counter == 1 && Context.IsMultiplayer && !external)
            {

                SpellQuery();

            }

            switch (type)
            {

                case barrages.meteor:

                    if (counter == 1)
                    {

                        indicator = indicators.stars;

                        scheme = schemes.stars;

                        origin = destination - new Vector2 (320, 640);

                        LaunchMissile();

                        TargetCircle();

                        return true;

                    }

                    if (counter == 60)
                    {

                        RadialDamage();

                        LightRadius(destination);

                        RadialImpact(radius);

                        if (radius > 5)
                        {
                            Game1.currentLocation.playSound("explosion");
                        }
                        else
                        {
                            Game1.currentLocation.playSound("flameSpellHit");
                        }
                        return true;

                    }

                    if (counter == 120)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;


                case barrages.bolt:

                    if (counter == 1)
                    {

                        LaunchBolt();

                        return true;

                    }

                    if (counter == 15)
                    {

                        LocalDamage();

                        return true;

                    }

                    if (counter == 30)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;

                case barrages.rockfall:

                    if (counter == 1)
                    {

                        LaunchRockfall();

                        return true;

                    }

                    if (counter == 36)
                    {

                        RadialDamage();

                        RadialImpact();

                        RockDebris();

                        return true;

                    }

                    if (counter == 60)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;

                case barrages.ballistic:

                    
                    if (counter == 1)
                    {
                        
                        AdjustTarget();
                        
                        TargetCircle(2);

                        return true;

                    }

                    if(counter == 60)
                    {

                        LaunchMissile();

                        return true;

                    }

                    if (counter == 120)
                    {

                        RadialDamage();

                        LightRadius(destination);

                        RadialImpact();

                        Game1.currentLocation.playSound("flameSpellHit",destination,800+new Random().Next(7)*100);

                        return true;

                    }

                    if (counter == 180)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;

                case barrages.burn:

                    if (counter == 1)
                    {

                        BurnCircle();

                        BurnImpacts();

                        LightRadius(origin);

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

                        LightRadius(origin);

                    }

                    return true;

                case barrages.fireball:

                    if (counter == 1)
                    {
                        AdjustTarget();
                        TargetCircle();
                        return true;
                    }

                    if (counter == 30)
                    {

                        LaunchMissile();

                        return true;
                    }

                    if (counter == 40)
                    {

                        GrazeDamage(1,4);

                        return true;

                    }

                    if (counter == 70)
                    {
                        RadialDamage();

                        LightRadius(destination);

                        RadialImpact(radius-1);

                        return true;

                    }

                    if (counter == 120)
                    {

                        Shutdown();

                        return false;
                    }
                 
                    return true;

                case barrages.beam:

                    if (counter == 1)
                    {

                        LaunchBeam();

                        LightRadius(origin);

                        return true;

                    }

                    if (counter == 30)
                    {

                        RadialDamage();

                        LightRadius(destination);

                        return true;
                    }

                    if (counter == 90)
                    {

                        Shutdown();

                        return false;

                    }

                    return true;

                case barrages.chaos:

                    if (counter == 1)
                    {

                        LaunchChaos();

                        return true;

                    }

                    if (counter == 30)
                    {

                        RadialDamage();

                        LightRadius(destination);

                        RadialImpact(3);

                        return true;

                    }

                    if (counter == 60)
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

            if (aoes.Count > 0)
            {

                foreach (TemporaryAnimatedSprite animatedSprite in animations)
                {

                    location.temporarySprites.Remove(animatedSprite);

                }

            }
        }
        
        public void TargetCircle(int duration = 1)
        {

            TemporaryAnimatedSprite innerCircle = ModUtility.AnimateCursor(location, destination, indicator.ToString(), duration*1000);

            animations.Add(innerCircle);

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

        public string ColourSchemes(string id)
        {

            if(id == "ether") { return "Blue"; }

            return "Red";

        }

        public void BurnCircle()
        {

            List<Vector2> impactVectors;
            
            Dictionary<Vector2,TemporaryAnimatedSprite> burnSprites = new();

            string colour = ColourSchemes(scheme.ToString());

            Texture2D flameTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", colour+"Embers.png"));

            for (int i = 0; i < Math.Min(3,radius + 1); i++)
            {

                impactVectors = ModUtility.GetTilesWithinRadius(location, new Vector2((int)(destination.X/64),(int)(destination.Y/64)), i);

                foreach (Vector2 vector in impactVectors)
                {

                    Vector2 position = new((vector.X * 64), (vector.Y * 64));

                    TemporaryAnimatedSprite burnSprite =  new(0, 150, 4, 8, position, false, false)
                    {

                        sourceRect = new(0, i * 32, 32, 32),

                        sourceRectStartingPos = new(0, i * 32),

                        texture = flameTexture,

                        scale = 2f,

                        extraInfoForEndBehavior = 99999,

                        layerDepth = vector.Y / 10000,

                        alpha = 0.5f,

                    };

                    burnSprites.Add(vector,burnSprite);

                }

            }

            for (int i = location.temporarySprites.Count - 1; i >= 0; i--)
            {

                TemporaryAnimatedSprite sprite = location.temporarySprites.ElementAt(i);

                if (sprite.extraInfoForEndBehavior == 99999)
                {

                    Vector2 localVector;

                    //localVector = new((int)(sprite.Position.X / 64), (int)(sprite.Position.Y / 64));

                    localVector = new((int)((sprite.Position.X) / 64), (int)((sprite.Position.Y) / 64));

                    if (burnSprites.ContainsKey(localVector))
                    {

                        if(sprite.sourceRect.Y < burnSprites[localVector].sourceRect.Y)
                        {
                            burnSprites[localVector].sourceRect = sprite.sourceRect;
                            burnSprites[localVector].sourceRectStartingPos = sprite.sourceRectStartingPos;
                            burnSprites[localVector].Position = sprite.Position;
                            burnSprites[localVector].timer = sprite.timer;
                        }

                        location.temporarySprites.Remove(sprite);

                    }
                    else
                    {

                        aoes.Add(sprite);

                    }

                }

            }

            foreach(KeyValuePair< Vector2,TemporaryAnimatedSprite> spritePair in burnSprites)
            {

                location.temporarySprites.Add(spritePair.Value);

                aoes.Add(spritePair.Value);

            }

        }

        public void BurnImpacts()
        {

            if(external) { return; }

            List<Vector2> burnVectors = new();

            for (int i = aoes.Count - 1; i >= 0; i--)
            {

                TemporaryAnimatedSprite sprite = aoes.ElementAt(i);

                if (!location.temporarySprites.Contains(sprite))
                {

                    aoes.RemoveAt(i);

                    continue;

                }

                Vector2 localVector = new((int)(sprite.Position.X / 64), (int)(sprite.Position.Y / 64));

                burnVectors.Add(localVector*64);

            }

            if (!Mod.instance.eventRegister.ContainsKey("immolate"))
            {

                new Immolate(Game1.player.Position).EventTrigger();

            }

            Immolate immolateEvent = Mod.instance.eventRegister["immolate"] as Immolate;

            immolateEvent.expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 10;

            if (damageFarmers > 0)
            {

                List<Farmer> farmerVictims = new();

                foreach(Vector2 vector in burnVectors)
                {
                    
                    List<Farmer> foundVictims = ModUtility.FarmerProximity(location, vector, radius, true);

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

                List<StardewValley.Monsters.Monster> monsterVictims = ModUtility.MonsterProximity(location, burnVectors, radius, true);

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

        public Rectangle OverlayRectangles(string id)
        {

            Microsoft.Xna.Framework.Rectangle rect = new(-1, 0, 32, 32);

            switch (id)
            {

                case "stars":

                    rect = new(0, 32, 32, 32);

                    break;

                case "fates":

                    rect = new(96, 64, 32, 32);

                    break;

            }

            return rect;

        }

        public void LaunchBolt()
        {
            
            ModUtility.AnimateBolt(location, new Vector2(destination.X, destination.Y - 64), 600 + new Random().Next(1, 8) * 100);

        }

        public void AdjustTarget(int threshold = 384)
        {

            if (Vector2.Distance(origin, destination) < threshold)
            {

                Vector2 diff = ((destination - origin) / Vector2.Distance(origin, destination)) * threshold;

                destination = origin + diff;

                impact = destination;

            }

        }

        public void LaunchMissile()
        {

            Game1.currentLocation.playSound("fireball");

            float targetDepth = location.IsOutdoors ? (destination.Y / 640000) + 0.00001f : 999f;

            Vector2  diff = (destination - origin);
            
            float rotate = (float)Math.Atan2(diff.Y, diff.X);

            Vector2 motion = diff / 900;

            Rectangle rect = MissileRectangles(scheme.ToString());

            Vector2 setat = origin - (new Vector2(48,48)*radius);

            TemporaryAnimatedSprite missile = new(0, 75, 4, 3, setat, false, false)
            {

                sourceRect = rect,

                sourceRectStartingPos = new Vector2(rect.X, rect.Y),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Missiles.png")),

                scale = radius,

                layerDepth = targetDepth,

                timeBasedMotion = true,

                alpha = 0.65f,

                rotation = rotate,

                motion = motion,

            };

            location.temporarySprites.Add(missile);

            animations.Add(missile);

            Rectangle overlay = OverlayRectangles(scheme.ToString());

            if(overlay.X == -1)
            {

                return;

            }

            Vector2 setattwo = origin - (new Vector2(16, 16) * radius);

            TemporaryAnimatedSprite cursorAnimation = new(0, 900, 1, 1, setattwo, false, false)
            {

                sourceRect = overlay,

                sourceRectStartingPos = new Vector2(overlay.X, overlay.Y),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Cursors.png")),

                scale = radius,

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

            Vector2 diff = (destination - origin) / Vector2.Distance(origin, destination) * 384;

            float rotate = (float)Math.Atan2(diff.Y, diff.X);

            Vector2 setPosition = origin + diff - new Vector2(320,64);

            TemporaryAnimatedSprite beam = new(0, 125f, 8, 1, setPosition, false, false)
            {
                sourceRect = new(0, 0, 160, 32),
                sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                texture = beamTexture,
                scale = 4f,
                timeBasedMotion = true,
                layerDepth = 999f,
                rotation = rotate,
                alpha = 0.75f,

            };

            location.temporarySprites.Add(beam);

            animations.Add(beam);

        }

        public void LaunchChaos()
        {

            Texture2D beamTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Chaos.png"));

            Vector2 ratio = (destination - origin) / Vector2.Distance(origin, destination);

            Vector2 diff = ratio * 240;

            Vector2 terminus = origin + (ratio * 360);

            Vector2 zone = new((int)(terminus.X / 64), (int)(terminus.Y / 64));

            radius = 4;

            float rotate = (float)Math.Atan2(diff.Y, diff.X);

            Vector2 setPosition = origin + diff - new Vector2(240, 96);

            TemporaryAnimatedSprite beam = new(0, 120f, 5, 1, setPosition, false, false)
            {
                sourceRect = new(0, 0, 160, 64),
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

        public void RadialDamage(int hit = 0)
        {

            int zone = radius;

            if (type == barrages.burn)
            {
                zone = radius - 1;
            }

            if (damageFarmers > 0)
            {

                ModUtility.DamageFarmers(location, ModUtility.FarmerProximity(location, impact, zone, true), (int)damageFarmers, monster);
                
            }

            if (damageMonsters > 0)
            {
                    
                ModUtility.DamageMonsters(location, ModUtility.MonsterProximity(location, new() { impact}, zone, true), Game1.player, (int)damageMonsters, true);

            }

            if (power > 0)
            {

                ModUtility.Explode(location, impact/64, Game1.player, environment, power);

            }

            if(terrain > 0)
            {
                
                ModUtility.Explode(location, impact/64, Game1.player, terrain);

            }

        }

        public void GrazeDamage(int piece, int division)
        {

            Vector2 diff = (destination - origin) / division * piece;

            Vector2 current = origin + diff;

            int zone = radius / 2;

            if (damageFarmers > 0)
            {

                ModUtility.DamageFarmers(location, ModUtility.FarmerProximity(location, current, zone, true), (int)damageFarmers, monster);

            }

            if (damageMonsters > 0)
            {

                ModUtility.DamageMonsters(location, ModUtility.MonsterProximity(location, new() { current }, zone, true), Game1.player, (int)damageMonsters, true);

            }

        }

        public void LocalDamage()
        {

            int damageApplied = (int)(damageMonsters * 0.7);

            bool critApplied = false;

            float critDamage = ModUtility.CalculateCritical(damageApplied, critical);

            if (critDamage > 0)
            {

                damageApplied = (int)critDamage;

                critApplied = true;

            }

            List<int> diff = ModUtility.CalculatePush(location, monster, Game1.player.Position, 64);

            ModUtility.HitMonster(location, Game1.player, monster, damageApplied, critApplied, diffX: diff[0], diffY: diff[1]);

        }

        public void RadialImpact(int reach = 1)
        {

            ModUtility.AnimateImpact(location, impact, reach);

        }

        public void LaunchRockfall()
        {

            List<int> indexes = Map.SpawnData.RockFall(location, Game1.player, Mod.instance.rockCasts[Mod.instance.rite.castLocation.Name]);

            int objectIndex = indexes[0];

            int scatterIndex = indexes[1];

            if(debris == 1)
            {

                debris = indexes[2];

            }

            ModUtility.AnimateRockfall(location, destination/64, 0, objectIndex, scatterIndex);

        }

        public void RockDebris()
        {
            if (debris == 0)
            {
                
                return;

            }

            Random randomIndex = new();

            int rockCut = randomIndex.Next(2);

            int generateAmt = 1 + randomIndex.Next(Mod.instance.virtualPick.UpgradeLevel) / 2;

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

            if (!Mod.instance.rite.castTask.ContainsKey("masterRockfall"))
            {

                Mod.instance.UpdateTask("lessonRockfall", generateAmt);

            }

        }

    }

}
