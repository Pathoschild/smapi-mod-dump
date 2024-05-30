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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Companions;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;


namespace StardewDruid.Cast.Effect
{
    public class Curse : EventHandle
    {

        public Dictionary<StardewValley.Monsters.Monster, CurseTarget> curseTargets = new();

        public Curse()
          : base()
        {

        }

        public void AddTarget(GameLocation location, StardewValley.Monsters.Monster monster, SpellHandle.effects effect)
        {

            if (!ModUtility.MonsterVitals(monster, location))
            {

                return;

            }

            if (monster.isGlider.Value)
            {

                return;

            }

            if (StardewDruid.Monster.MonsterHandle.BossMonster(monster))
            {
                
                monster.stunTime.Set(Math.Max(monster.stunTime.Value, 500));
                
                return;
            
            }

            if (curseTargets.ContainsKey(monster))
            {

                return;

            }

            int timer = 2;

            switch (effect)
            {

                case SpellHandle.effects.morph:
                case SpellHandle.effects.doom:

                    timer = 5;

                    curseTargets.Add(monster, new(location, monster, timer, CurseTarget.curseEffects.morph));

                    break;

                case SpellHandle.effects.knock:

                    curseTargets.Add(monster, new(location, monster, timer, CurseTarget.curseEffects.knock));

                    break;


            }

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + timer + 1;


        }

        public override void EventRemove()
        {

            for (int g = curseTargets.Count - 1; g >= 0; g--)
            {

                KeyValuePair<StardewValley.Monsters.Monster, CurseTarget> knockTarget = curseTargets.ElementAt(g);

                knockTarget.Value.ShutDown();

            }

            base.EventRemove();
        }

        public override void EventDecimal()
        {

            for (int g = curseTargets.Count - 1; g >= 0; g--)
            {

                KeyValuePair<StardewValley.Monsters.Monster, CurseTarget> knockTarget = curseTargets.ElementAt(g);

                if (!knockTarget.Value.Update())
                {

                    curseTargets.Remove(knockTarget.Key);

                }

            }

        }

    }
    public class CurseTarget
    {

        public GameLocation location;

        public StardewValley.Monsters.Monster victim;

        public TemporaryAnimatedSprite animation;

        public int timer;

        public float rotation;

        public enum curseEffects
        {
            knock,
            morph,
            reap,
        }

        public curseEffects type;

        public CurseTarget(GameLocation Location, StardewValley.Monsters.Monster Victim, int Timer, curseEffects Type = curseEffects.knock)
        {

            location = Location;

            victim = Victim;

            timer = Timer * 10;

            type = Type;

            switch(type)
            {
                
                case curseEffects.knock:

                    rotation = Victim.rotation;

                    Victim.Halt();

                    Victim.stunTime.Set(Math.Max(Victim.stunTime.Value, Timer*1000));

                    if (Victim.Sprite.getWidth() > 16)
                    {

                        Victim.rotation = (float)(Math.PI);

                    }
                    else
                    {

                        Victim.rotation = (float)(Math.PI / 2);

                    }

                break;

                case curseEffects.morph:
                case curseEffects.reap:

                    TargetFace();

                    break;

            }

        }

        public void ShutDown()
        {
            switch (type)
            {

                case curseEffects.knock:

                    victim.rotation = rotation;

                    break;

                case curseEffects.morph:
                case curseEffects.reap:

                    location.temporarySprites.Remove(animation);

                    ModUtility.HitMonster(location, Game1.player, victim, victim.Health+1, true);

                    break;

            }
            

        }

        public bool Update()
        {

            if (!ModUtility.MonsterVitals(victim, location))
            {

                return false;

            }

            if (victim is StardewValley.Monsters.Mummy mummy)
            {

                if (mummy.reviveTimer.Value > 0)
                {

                    ModUtility.HitMonster(location, Game1.player, mummy, 1, false);

                    return false;

                }

            }

            timer--;

            if(timer <= 0)
            {

                ShutDown();

                return false;

            }

            switch (type)
            {

                case curseEffects.knock:

                    victim.Halt();

                    if(victim.stunTime.Value <= 0)
                    {

                        victim.stunTime.Set(timer * 100);

                    }

                    break;

                case curseEffects.morph:
                case curseEffects.reap:

                    if (!location.temporarySprites.Contains(animation))
                    {

                        TargetFace();

                    }

                    Rectangle box = victim.GetBoundingBox();

                    Point center = box.Center;

                    animation.Position = center.ToVector2() - new Vector2(32, 128);

                    animation.alpha = 1f - (timer * 0.025f);

                    break;

            }


            return true;

        }

        public void TargetFace()
        {

            Rectangle box = victim.GetBoundingBox();

            Point center = box.Center;

            TemporaryAnimatedSprite warpTarget = new(0, timer*100, 1, 1, center.ToVector2() - new Vector2(32, 128), false, false)
            {

                sourceRect = new(0, 0, 32, 32),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Polymorph.png")),

                scale = 2f,

                layerDepth = 999f,

                alpha = 1f - (timer * 0.025f),

            };

            location.temporarySprites.Add(animation);

            animation = warpTarget;

        }

    }

}
