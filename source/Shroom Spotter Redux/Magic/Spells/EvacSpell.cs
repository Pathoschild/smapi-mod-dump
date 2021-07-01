/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using Magic.Schools;
using SpaceCore;
using StardewValley;

namespace Magic.Spells
{
    public class EvacSpell : Spell
    {
        public EvacSpell() : base(SchoolId.Life, "evac")
        {
        }

        public override int getMaxCastingLevel()
        {
            return 1;
        }

        public override int getManaCost(Farmer player, int level)
        {
            return 25;
        }

        public override IActiveEffect onCast(Farmer player, int level, int targetX, int targetY)
        {
            player.position.X = enterX;
            player.position.Y = enterY;
            player.AddCustomSkillExperience(Magic.Skill, 5);
            return null;
        }

        private static float enterX, enterY;
        internal static void onLocationChanged()
        {
            enterX = Game1.player.position.X;
            enterY = Game1.player.position.Y;
        }
    }
}
