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

    public class Blood : BaseAugment
    {
        private int ticks = 0;

        public Blood()
        {
            this.DisplayName = "Bleeding Out!";
            this.desc = "Player loses health over time.";
        }

        public override void Init() { }

        public override ActionResponse MonsterTickUpdate(Monster m)
        {
            return ActionResponse.Done;
        }

        public override ActionResponse PlayerTickUpdate()
        {
            Farmer farmer = Game1.player;

            if (farmer.health > (farmer.maxHealth * 0.25) && ticks++ > 1)
            {
                farmer.health -= 1;
                farmer.currentLocation.debris.Add(new Debris(1, new Vector2(farmer.getStandingX() + 8, farmer.getStandingY()), Color.Red, 1f, farmer));
                //farmer.currentLocation.playSound("ow");
                Game1.hitShakeTimer = 100 * 1;
                ticks = 0;
            }

            return ActionResponse.Done;
        }

        public override ActionResponse UpdateMonster(WarpedEventArgs e, Monster npc) { return ActionResponse.Done; }

        public override ActionResponse WarpLocation(WarpedEventArgs e) { return ActionResponse.Done; }
    }
}
