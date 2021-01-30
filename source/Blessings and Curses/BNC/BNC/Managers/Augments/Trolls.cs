/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using BNC.TwitchApp;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using System;

namespace BNC.Managers.Augments
{
public class Trolls : BaseAugment
    {
        private int amt = 1;
        private int offsetTick = 0;
        public Trolls()
        {
            this.DisplayName = "Mobs Health Regen!";
            this.desc = "Player regens health over time.";
        }

        public override void Init() {
            this.hasMonsterTick = true;
        }

        public override ActionResponse MonsterTickUpdate(Monster m)
        {
            if (offsetTick++ >= 1)
            {
                if (m.health + amt <= m.maxHealth)
                {
                    m.Health += amt;
                    m.currentLocation.debris.Add(new Debris(amt, new Vector2(m.getStandingX() + 8, m.getStandingY()), Color.Green, 1f, m));
                }
                offsetTick = 0;
            }
            return ActionResponse.Done;
        }

        public override ActionResponse PlayerTickUpdate()   {  return ActionResponse.Done;   }

        public override ActionResponse UpdateMonster(WarpedEventArgs e, Monster npc) { return ActionResponse.Done; }

        public override ActionResponse WarpLocation(WarpedEventArgs e) { return ActionResponse.Done; }
    }
}
