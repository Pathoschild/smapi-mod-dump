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

    public class Tired : BaseAugment
    {
        public Tired()
        {
            this.DisplayName = "Getting Tired!";
            this.desc = "Player loses stamina over time.";
        }
        public override void Init() { }

        public override ActionResponse MonsterTickUpdate(Monster m)
        {
            return ActionResponse.Done;
        }

        public override ActionResponse PlayerTickUpdate()
        {
            Farmer farmer = Game1.player;

            if (farmer.stamina > (farmer.maxStamina * 0.1))
            {
                farmer.stamina -= 1;
                farmer.currentLocation.debris.Add(new Debris(1, new Vector2(farmer.getStandingX() + 8, farmer.getStandingY()), Color.Orange, 1f, farmer));
                Game1.staminaShakeTimer = 100 * 1;
            }

            return ActionResponse.Done;
        }

        public override ActionResponse UpdateMonster(WarpedEventArgs e, Monster npc) { return ActionResponse.Done;  }

        public override ActionResponse WarpLocation(WarpedEventArgs e)   { return ActionResponse.Done; }
    }
}
