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
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.IO;

namespace StardewDruid.Cast.Mists
{
    public class SapEvent : EventHandle
    {

        List<StardewValley.Monsters.Monster> victims;

        public SapEvent(Vector2 target, List<StardewValley.Monsters.Monster> Victims)
          : base(target)
        {
            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 3.0;
            victims = Victims;
        }

        public override void EventTrigger()
        {

            Mod.instance.RegisterEvent(this, "sap");

            SapEffect();

        }

        public void SapEffect()
        {

            int leech = 0;

            int damage = Math.Max(5, Mod.instance.PowerLevel() * 3);

            foreach(var monster in victims)
            {

                if (!ModUtility.MonsterVitals(monster, monster.currentLocation))
                {
                    continue;
                }

                ModUtility.AnimateGlare(monster.currentLocation, monster.Position);

                ModUtility.DamageMonsters(monster.currentLocation, new List<StardewValley.Monsters.Monster>(){ monster }, Game1.player, damage);

                leech += damage;

            }

            victims.Clear();

            int num = Math.Min(leech, Mod.instance.rite.caster.MaxStamina - (int)Mod.instance.rite.caster.stamina);

            if (num > 0)
            {

                Mod.instance.rite.caster.stamina += num;

                Rectangle healthBox = Mod.instance.rite.caster.GetBoundingBox();

                targetLocation.debris.Add(
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

    }

}
