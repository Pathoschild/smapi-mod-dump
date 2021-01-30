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
using StardewModdingAPI.Events;
using StardewValley.Monsters;
using System;
namespace BNC.Managers.Augments
{

    public class Health : BaseAugment
    {
        private float boost = 1.5f;

        public Health()
        {
            this.DisplayName = "Monster Health Increase!";
            this.desc = "Monster have 1.5x more health.";
        }

        public override void Init() { }

        public override ActionResponse MonsterTickUpdate(Monster m)
        {
            return ActionResponse.Done;
        }

        public override ActionResponse PlayerTickUpdate()
        {
            return ActionResponse.Done;
        }

        public BaseAugment setBoostAmount(float x)
        {
            this.boost = x;
            return this;
        }

        public override ActionResponse UpdateMonster(WarpedEventArgs e, Monster m)
        {
            m.MaxHealth = (int)System.Math.Round(m.MaxHealth * boost);
            m.Health = m.MaxHealth;
            return ActionResponse.Done;
        }

        public override ActionResponse WarpLocation(WarpedEventArgs e) { return ActionResponse.Done; }
    }
}
