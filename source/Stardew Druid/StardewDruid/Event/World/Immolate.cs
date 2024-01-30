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
using StardewDruid.Map;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace StardewDruid.Event.World
{
    public class Immolate : EventHandle
    {

        public int skipCounter;

        public Dictionary<StardewValley.Monsters.Monster, Immolated> monsterVictims;

        public Dictionary<Farmer, Immolated> farmerVictims;

        public bool immolate;

        public Immolate(Vector2 target, Rite rite)
            : base(target, rite)
        {

            monsterVictims = new();

            farmerVictims = new();

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 10;

        }

        public override void EventTrigger()
        {

            Mod.instance.RegisterEvent(this, "immolate");

        }

        public override void EventInterval()
        {

            for(int f = farmerVictims.Count -1; f >= 0; f--)
            {

                KeyValuePair<Farmer, Immolated> victim = farmerVictims.ElementAt(f);

                victim.Value.timer--;

                if(victim.Value.timer <= 0)
                {

                    farmerVictims.Remove(victim.Key);

                    continue;

                }

                ModUtility.DamageFarmers(targetLocation, new() { victim.Key, }, victim.Value.damage, null);

            }


            for (int f = monsterVictims.Count - 1; f >= 0; f--)
            {

                KeyValuePair<StardewValley.Monsters.Monster, Immolated> victim = monsterVictims.ElementAt(f);

                if (!ModUtility.MonsterVitals(victim.Key,targetLocation))
                {
                    monsterVictims.Remove(victim.Key);

                    continue;

                }

                victim.Value.timer--;

                if (victim.Value.timer <= 0)
                {

                    monsterVictims.Remove(victim.Key);

                    continue;

                }

                Vector2 monsterPosition = victim.Key.Position;

                ModUtility.DamageMonsters(targetLocation, new() { victim.Key, }, riteData.caster, victim.Value.damage, false);

                if (riteData.castTask.ContainsKey("masterBlast"))
                {

                    if (new Random().Next(5) == 0)
                    {

                        if (!ModUtility.MonsterVitals(victim.Key, targetLocation))
                        {

                            Vector2 targetPosition = riteData.caster.Position;

                            TemporaryAnimatedSprite immolation = new(0, 125f, 5, 1, monsterPosition - new Vector2(32, 64), false, monsterPosition.Y < targetPosition.Y)
                            {

                                sourceRect = new(0, 0, 32, 32),

                                sourceRectStartingPos = new Vector2(0, 0),

                                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Immolation.png")),

                                scale = 4f, //* size,

                                layerDepth = monsterPosition.Y / 640000,

                                alphaFade = 0.002f,

                            };

                            targetLocation.temporarySprites.Add(immolation);

                            int barbeque = SpawnData.RandomBarbeque();

                            Throw throwMeat = new(riteData.caster, monsterPosition, barbeque);

                            throwMeat.ThrowObject();

                        }

                    }

                }

            }

        }

    }

    public class Immolated
    {

        public int damage;

        public int timer;

        public Immolated(int Damage, int Timer = 3)
        {

            damage = Damage;

            timer = Timer;

        }

    }



}
