/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/Magic
**
*************************************************/

using Magic.Schools;
using StardewValley;
using System.Collections.Generic;
using System;
using SpaceCore;

namespace Magic.Spells
{
    public class LuckStealSpell : Spell
    {
        public LuckStealSpell() : base(SchoolId.Eldritch, "lucksteal")
        {
        }

        public override int getManaCost(Farmer player, int level)
        {
            return 0;
        }

        public override int getMaxCastingLevel()
        {
            return 1;
        }

        public override bool canCast(Farmer player, int level)
        {
            return base.canCast(player, level) && player.team.sharedDailyLuck.Value != 0.12;
        }

        public override IActiveEffect onCast(Farmer player, int level, int targetX, int targetY)
        {
            var num = Game1.random.Next(player.friendshipData.Count());
            var friendshipData = player.friendshipData[new List<string>(player.friendshipData.Keys)[num]];
            friendshipData.Points = Math.Max(0, friendshipData.Points - 250);
            player.team.sharedDailyLuck.Value = 0.12;
            Game1.playSound("death");
            player.AddCustomSkillExperience(Magic.Skill, 50);

            return null;
        }
    }
}
