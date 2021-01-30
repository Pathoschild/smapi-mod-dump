/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using System.Collections.Generic;
using BNC.TwitchApp;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using xTile.Dimensions;
using System;
namespace BNC.Managers.Augments
{

    public class Phasing : BaseAugment
    {
        private int ticks = 0;
        private bool teleporting = false;
        private IEnumerator<Point> teleportationPath;

        public Phasing()
        {
            this.DisplayName = "Phasing Out!";
            this.desc = "Player randomly moves thru dimensions.";
        }

        public override void Init() { }

        public override ActionResponse MonsterTickUpdate(Monster m)
        {
            return ActionResponse.Done;
        }

        public override ActionResponse PlayerTickUpdate()
        {
            Farmer farmer = Game1.player;

            if (Game1.random.NextDouble() <= 0.1f)
            {
                farmer.temporarilyInvincible = true;
                farmer.isGlowing = true;
                farmer.startGlowing(Color.White, false, 0.5f);

            }
            else if(farmer.temporarilyInvincible == false && farmer.isGlowing)
            {
                farmer.stopGlowing();
                farmer.isGlowing = false;
            }

            return ActionResponse.Done;
        }

        public void OnRemove() {
            Farmer farmer = Game1.player;
            if (farmer.isGlowing)
            {
                farmer.stopGlowing();
                farmer.isGlowing = false;
            }
        }

        public override ActionResponse UpdateMonster(WarpedEventArgs e, Monster npc) { return ActionResponse.Done; }

        public override ActionResponse WarpLocation(WarpedEventArgs e) { return ActionResponse.Done; }
    }
}
