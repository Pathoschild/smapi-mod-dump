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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Companions;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace StardewDruid.Cast.Mists
{
    public class BurstEvent : EventHandle
    {

        List<StardewValley.Monsters.Monster> victims;

        List<StardewValley.Monsters.Monster> knocked;

        public BurstEvent(Vector2 target, List<StardewValley.Monsters.Monster> Victims)
          : base(target)
        {

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 3.0;

            victims = Victims;

            knocked = new();

        }

        public override void EventTrigger()
        {

            Mod.instance.RegisterEvent(this, "burst");

            BurstEffect();

        }

        public void BurstEffect()
        {

            int damage = Mod.instance.DamageLevel() / 4;

            foreach(var monster in victims)
            {

                if (!ModUtility.MonsterVitals(monster, monster.currentLocation))
                {
                    continue;
                }

                ModUtility.AnimateImpact(monster.currentLocation, monster.Position + new Vector2(32), 2, 0, "Flashbang");

                monster.currentLocation.playSound("magma_sprite_hit");

                ModUtility.DamageMonsters(monster.currentLocation, new List<StardewValley.Monsters.Monster>(){ monster }, Game1.player, damage);

                KnockVictim(monster);

            }

            victims.Clear();

        }

        public void KnockVictim(StardewValley.Monsters.Monster monster)
        {

            if(monster is StardewDruid.Monster.Boss.Boss)
            {

                return;

            }

            knocked.Add(monster);

            monster.Halt();

            monster.stunTime.Set(Math.Max(monster.stunTime.Value,3000));

            if (monster.isGlider.Value)
            {

                return;

            }

            if (monster.Sprite.getWidth() > 16)
            {

                monster.rotation = (float)(Math.PI);

            }
            else
            {

                monster.rotation = (float)(Math.PI / 2);

            }

        }

        public void ReleaseVictims()
        {

            for(int m = knocked.Count-1; m >= 0; m--)
            {

                StardewValley.Monsters.Monster monster = knocked[m];

                if (!ModUtility.MonsterVitals(monster, monster.currentLocation))
                {
                    continue;
                }

                monster.stunTime.Set(0);

                monster.rotation = 0;

            }

            knocked.Clear();

        }

        public override void EventRemove()
        {

            victims.Clear();

            knocked.Clear();

            base.EventRemove();
        }

        public override void EventDecimal()
        {
            for (int m = knocked.Count - 1; m >= 0; m--)
            {

                StardewValley.Monsters.Monster monster = knocked[m];

                if (!ModUtility.MonsterVitals(monster, monster.currentLocation))
                {
                    knocked.RemoveAt(m);

                    continue;
                }

                monster.Halt();

            }
        }

        public override void EventInterval()
        {

            activeCounter++;

            if(activeCounter == 3)
            {

                ReleaseVictims();

            }

        }

    }

}
