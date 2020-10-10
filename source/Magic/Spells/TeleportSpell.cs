/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/Magic
**
*************************************************/

using Magic.Game.Interface;
using Magic.Schools;
using Magic.Spells;
using StardewValley;

namespace Magic
{
    public class TeleportSpell : Spell
    {
        public TeleportSpell()
        :   base( SchoolId.Elemental, "teleport" )
        {
        }

        public override int getMaxCastingLevel()
        {
            return 1;
        }

        public override bool canCast(Farmer player, int level)
        {
            return base.canCast(player, level) && player.currentLocation.IsOutdoors && player.mount == null && player.hasItemInInventory(Mod.ja.GetObjectId("Travel Core"), 1);
        }

        public override int getManaCost(Farmer player, int level)
        {
            return 0;
        }

        public override IActiveEffect onCast(Farmer player, int level, int targetX, int targetY)
        {
            Game1.activeClickableMenu = new TeleportMenu();
            return null;
        }
    }
}