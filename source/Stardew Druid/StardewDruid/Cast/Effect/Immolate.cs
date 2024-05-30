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
using StardewDruid.Data;
using StardewDruid.Event;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using xTile.Dimensions;
using xTile.Tiles;
using static StardewDruid.Cast.SpellHandle;


namespace StardewDruid.Cast.Effect
{
    public class Immolate : EventHandle
    {

        public int skipCounter;

        public Dictionary<StardewValley.Monsters.Monster, BurnTarget> monsterVictims = new();

        public Dictionary<Farmer, BurnTarget> farmerVictims = new();

        public Dictionary<Vector2, EmberTarget> embers = new();

        public Dictionary<string,Texture2D> burnTextures = new();

        public bool immolate;

        public Immolate()
        {

        }

        public override void EventInterval()
        {
            
            // ===================================================
            // check embers

            for (int e = embers.Count - 1; e >= 0; e--)
            {

                KeyValuePair<Vector2, EmberTarget> ember = embers.ElementAt(e);

                if(ember.Value.expire <= Game1.currentGameTime.TotalGameTime.TotalSeconds)
                {

                    ember.Value.Shutdown();

                    embers.Remove(ember.Key);

                    continue;

                }

            }

            // ===================================================
            // add victims

            foreach (Farmer farmer in ModUtility.GetFarmersInLocation(location))
            {

                Vector2 farmerTile = farmer.Tile;

                if (farmerVictims.ContainsKey(farmer))
                {

                    continue;

                }

                if (!embers.ContainsKey(farmerTile))
                {

                    continue;

                }

                if (embers[farmerTile].damageFarmer <= 0)
                {

                    continue;
                
                }

                farmerVictims.Add(farmer, new(embers[farmerTile].damageFarmer, 3-embers[farmerTile].grade, embers[farmerTile].scheme));

            }

            foreach (StardewValley.Monsters.Monster monster in ModUtility.GetMonstersInLocation(location, true))
            {

                Vector2 monsterTile = monster.Tile;

                if (monsterVictims.ContainsKey(monster))
                {

                    continue;

                }

                if (!embers.ContainsKey(monsterTile))
                {

                    continue;

                }

                if (embers[monsterTile].damageMonster <= 0)
                {

                    continue;

                }

                monsterVictims.Add(monster, new(embers[monsterTile].damageMonster, 3 - embers[monsterTile].grade, embers[monsterTile].scheme));

            }

            // ===================================================
            // damage farmers

            for (int f = farmerVictims.Count - 1; f >= 0; f--)
            {

                KeyValuePair<Farmer, BurnTarget> victim = farmerVictims.ElementAt(f);

                ModUtility.DamageFarmers(new() { victim.Key, }, victim.Value.damage, null);

                Microsoft.Xna.Framework.Rectangle victimBox = victim.Key.GetBoundingBox();

                int burnSize = 3 - victim.Value.timer;

                TemporaryAnimatedSprite burnAnimation = new(0, 125, 4, 1, victimBox.Center.ToVector2() - new Vector2(16,16), false, false)
                {

                    sourceRect = new(0, burnSize * 32, 32, 32),

                    sourceRectStartingPos = new(0, burnSize * 32),

                    texture = burnTextures[victim.Value.scheme],

                    scale = 1f,

                    layerDepth = victimBox.Top / 10000 + 0.0002f,

                    alpha = 0.5f,
                };

                victim.Key.currentLocation.TemporarySprites.Add(burnAnimation);

                victim.Value.timer--;

                if (victim.Value.timer <= 0)
                {

                    farmerVictims.Remove(victim.Key);

                    continue;

                }

            }

            // ====================================================
            // damage monsters

            for (int f = monsterVictims.Count - 1; f >= 0; f--)
            {

                KeyValuePair<StardewValley.Monsters.Monster, BurnTarget> victim = monsterVictims.ElementAt(f);

                if (!ModUtility.MonsterVitals(victim.Key, location))
                {
                    
                    monsterVictims.Remove(victim.Key);

                    continue;

                }

                Vector2 monsterPosition = victim.Key.Position;

                ModUtility.DamageMonsters(location, new() { victim.Key, }, Game1.player, victim.Value.damage);

                if (!ModUtility.MonsterVitals(victim.Key, location))
                {

                    if (new Random().Next(5) == 0)
                    {

                        if (!burnTextures.ContainsKey("Immolate"))
                        {

                            burnTextures.Add("Immolate", Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Immolation.png")));

                        }

                        Vector2 targetPosition = Game1.player.Position;

                        TemporaryAnimatedSprite immolation = new(0, 125f, 5, 1, monsterPosition - new Vector2(32, 64), false, monsterPosition.Y < targetPosition.Y)
                        {

                            sourceRect = new(0, 0, 32, 32),

                            sourceRectStartingPos = new Vector2(0, 0),

                            texture = burnTextures["Immolate"],

                            scale = 4f, //* size,

                            layerDepth = monsterPosition.Y / 10000 + 0.001f,

                            alphaFade = 0.002f,

                        };

                        location.temporarySprites.Add(immolation);

                        int barbeque = SpawnData.RandomBarbeque();

                        ThrowHandle throwMeat = new(Game1.player, monsterPosition, barbeque);

                        throwMeat.register();

                    }

                    victim.Value.timer = 0;

                }

                if(victim.Value.timer > 0)
                {
                    
                    Microsoft.Xna.Framework.Rectangle victimBox = victim.Key.GetBoundingBox();

                    int burnSize = 3 - victim.Value.timer;

                    TemporaryAnimatedSprite burnAnimation = new(0, 125, 4, 1, victimBox.Center.ToVector2() - new Vector2(16, 16), false, false)
                    {

                        sourceRect = new(0, burnSize * 32, 32, 32),

                        sourceRectStartingPos = new(0, burnSize * 32),

                        texture = burnTextures[victim.Value.scheme],

                        scale = 1f,

                        layerDepth = victimBox.Top / 10000 + 0.0002f,

                        alpha = 0.5f,
                    };

                    victim.Key.currentLocation.TemporarySprites.Add(burnAnimation);

                }

                victim.Value.timer--;

                if (victim.Value.timer <= 0)
                {

                    monsterVictims.Remove(victim.Key);

                    continue;

                }

            }

        }

        public void RadialTarget(GameLocation location, Vector2 origin, int damageFarmers, int damageMonsters, IconData.schemes scheme)
        {

            if(location.Name != location.Name)
            {

                EventLocation();

            }

            string colour = "Red";

            /*if (scheme == IconData.schemes.ether)
            {

                colour = "Blue";

            }*/

            if (!burnTextures.ContainsKey(colour))
            {

                burnTextures.Add(colour, Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", colour + "Embers.png")));

            }

            for (int i = 0; i < 3; i++)
            {

                List<Vector2> burnVectors = ModUtility.GetTilesWithinRadius(location, origin, i);

                foreach (Vector2 burnVector in burnVectors)
                {

                    if (embers.ContainsKey(burnVector))
                    {

                        EmberTarget existing = embers[burnVector];

                        if(existing.grade > i)
                        {

                            existing.Upgrade();

                        }

                        existing.Reset();


                    }
                    else
                    {

                        TemporaryAnimatedSprite burnAnimation = new(0, 125, 4, 6, burnVector * 64 + new Vector2(8), false, false)
                        {

                            sourceRect = new(0, i * 32, 32, 32),

                            sourceRectStartingPos = new(0, i * 32),

                            texture = burnTextures[colour],

                            scale = 1.75f,

                            layerDepth = burnVector.Y / 10000,

                            alpha = 0.65f,
                        };

                        location.TemporarySprites.Add(burnAnimation);

                        animations.Add(burnAnimation);

                        embers.Add(burnVector,new(location,burnVector, burnAnimation, i, damageFarmers, damageMonsters, colour));

                    }

                }

            }

            expireTime += 5;

        }

    }

    public class BurnTarget
    {

        public int damage;

        public int timer;

        public string scheme;

        public BurnTarget(int Damage, int Timer = 3, string Scheme = "Red")
        {

            damage = Damage;

            timer = Timer;

            scheme = Scheme;

        }

    }

    public class EmberTarget
    {

        public Vector2 tile;

        public int grade;

        public string scheme;

        public TemporaryAnimatedSprite animation;

        public int damageFarmer;

        public int damageMonster;

        public GameLocation location;

        public double expire;

        public EmberTarget(GameLocation Location, Vector2 Tile, TemporaryAnimatedSprite Animation, int Grade = 0, int vsFarmer = 0, int vsMonster = 0, string Scheme = "Red")
        {

            tile = Tile;

            grade = Grade;

            expire = Game1.currentGameTime.TotalGameTime.TotalSeconds + 3;

            damageFarmer = vsFarmer;

            damageMonster = vsMonster;

            animation = Animation;

            location = Location;

            scheme = Scheme;

        }

        public void Reset()
        {

            animation.reset();

            expire = Game1.currentGameTime.TotalGameTime.TotalSeconds + 3;

        }

        public void Upgrade()
        {

            if(grade == 0) { return; }

            grade--;

            animation.sourceRect = new(0, grade * 32, 32, 32);

            animation.sourceRectStartingPos = new(0, grade * 32);

            Reset();

        }

        public void Shutdown()
        {

            location.TemporarySprites.Remove(animation);

        }


    }

}
