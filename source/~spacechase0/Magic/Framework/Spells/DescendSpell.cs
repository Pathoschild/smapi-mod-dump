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
using SpaceCore;
using StardewValley;
using StardewValley.Locations;

namespace Magic.Framework.Spells
{
    internal class DescendSpell : Spell
    {
        public DescendSpell()
            : base(SchoolId.Elemental, "descend") { }

        public override bool CanCast(Farmer player, int level)
        {
            return base.CanCast(player, level) && player.currentLocation is MineShaft ms && ms.mineLevel != MineShaft.quarryMineShaft;
        }

        public override int GetManaCost(Farmer player, int level)
        {
            return 15;
        }

        public override IActiveEffect OnCast(Farmer player, int level, int targetX, int targetY)
        {
            if (player != Game1.player)
                return null;

            var ms = player.currentLocation as MineShaft;
            if (ms == null)
                return null;

            int target = ms.mineLevel + 1 + 2 * level;
            if (ms.mineLevel <= 120 && target >= 120)
            {
                // We don't want the player to go through the bottom of the
                // original mine and into the skull cavern.
                target = 120;
            }

            Game1.enterMine(target);

            player.AddCustomSkillExperience(Magic.Skill, 5);
            return null;
        }
    }
}
