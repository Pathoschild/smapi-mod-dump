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
using Magic.Spells;
using SpaceCore;
using StardewValley;

namespace Magic
{
    public class RewindSpell : Spell
    {
        public RewindSpell()
        :   base( SchoolId.Arcane, "rewind" )
        {
        }

        public override int getMaxCastingLevel()
        {
            return 1;
        }

        public override bool canCast(Farmer player, int level)
        {
            return base.canCast(player, level) && player.hasItemInInventory(336, 1);
        }

        public override int getManaCost(Farmer player, int level)
        {
            return 0;
        }

        public override IActiveEffect onCast(Farmer player, int level, int targetX, int targetY)
        {
            player.consumeObject(336, 1);
            Game1.timeOfDay -= 200;
            player.AddCustomSkillExperience(Magic.Skill, 25);
            return null;
        }
    }
}
