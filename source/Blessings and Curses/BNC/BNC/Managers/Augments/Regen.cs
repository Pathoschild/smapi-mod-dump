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

    public class Regen : BaseAugment
    {
        private int amt = 2;

        public Regen()
        {
            this.DisplayName = "Health Regen!";
            this.desc = "Player regens health over time.";
        }

        public override void Init() { }

        public override ActionResponse MonsterTickUpdate(Monster m)
        {
            return ActionResponse.Done;
        }

        public override ActionResponse PlayerTickUpdate()
        {
            Farmer farmer = Game1.player;

            if (Game1.player.health + amt <= Game1.player.maxHealth) { 
                Game1.player.health += amt;
                farmer.currentLocation.debris.Add(new Debris(amt, new Vector2(farmer.getStandingX() + 8, farmer.getStandingY()), Color.Green, 1f, farmer));
            }
            return ActionResponse.Done;
        }

        public override ActionResponse UpdateMonster(WarpedEventArgs e, Monster npc) { return ActionResponse.Done; }

        public override ActionResponse WarpLocation(WarpedEventArgs e) { return ActionResponse.Done; }
    }
}
