/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using Magic.Framework.Game.Interface;
using Magic.Framework.Schools;
using StardewValley;

namespace Magic.Framework.Spells
{
    internal class TeleportSpell : Spell
    {
        public TeleportSpell()
            : base(SchoolId.Elemental, "teleport") { }

        public override int GetMaxCastingLevel()
        {
            return 1;
        }

        public override bool CanCast(Farmer player, int level)
        {
            return base.CanCast(player, level) && player.currentLocation.IsOutdoors && player.mount == null && player.hasItemInInventory(Mod.Ja.GetObjectId("Travel Core"), 1);
        }

        public override int GetManaCost(Farmer player, int level)
        {
            return 0;
        }

        public override IActiveEffect OnCast(Farmer player, int level, int targetX, int targetY)
        {
            Game1.activeClickableMenu = new TeleportMenu();
            return null;
        }
    }
}
