/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using Magic.Framework.Schools;
using Magic.Framework.Spells.Effects;
using SpaceCore;
using StardewValley;

namespace Magic.Framework.Spells
{
    internal class SpiritSpell : Spell
    {
        /*********
        ** Public methods
        *********/
        public SpiritSpell()
            : base(SchoolId.Eldritch, "spirit") { }

        public override int GetManaCost(Farmer player, int level)
        {
            return 50;
        }

        public override int GetMaxCastingLevel()
        {
            return 1;
        }

        public override IActiveEffect OnCast(Farmer player, int level, int targetX, int targetY)
        {
            player.AddCustomSkillExperience(Magic.Skill, 25);
            return new SpiritEffect(player);
        }
    }
}
