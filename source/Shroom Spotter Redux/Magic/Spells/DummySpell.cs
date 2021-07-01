/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using SpaceShared;
using StardewValley;

namespace Magic.Spells
{
    public class DummySpell : Spell
    {
        public DummySpell(string school, string id) : base(school, id)
        {
        }

        public override int getManaCost(Farmer player, int level)
        {
            return 0;
        }

        public override IActiveEffect onCast(Farmer player, int level, int targetX, int targetY)
        {
            Log.debug($"{player.Name} cast {Id}.");
            return null;
        }
    }
}
