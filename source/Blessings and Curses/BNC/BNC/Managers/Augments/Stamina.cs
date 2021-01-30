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

    public class Stamina : BaseAugment
    {
        private int amt = 2;

        public Stamina()
        {
            this.DisplayName = "Stamina Regen!";
            this.desc = "Player regens stamina over time.";
        }

        public override void Init() { }

        public override ActionResponse MonsterTickUpdate(Monster m)
        {
            return ActionResponse.Done;
        }

        public override ActionResponse PlayerTickUpdate()
        {
            Farmer farmer = Game1.player;

            if (Game1.player.Stamina + amt <= Game1.player.MaxStamina) { 
                Game1.player.Stamina += amt;
                Game1.staminaShakeTimer = 100 * 1;
            }

            return ActionResponse.Done;
        }

        public override ActionResponse UpdateMonster(WarpedEventArgs e, Monster npc) { return ActionResponse.Done; }

        public override ActionResponse WarpLocation(WarpedEventArgs e) { return ActionResponse.Done; }
    }
}
