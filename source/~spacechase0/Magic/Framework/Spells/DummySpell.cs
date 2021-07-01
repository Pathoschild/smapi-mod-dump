/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using SpaceShared;
using StardewValley;

namespace Magic.Framework.Spells
{
    internal class DummySpell : Spell
    {
        public DummySpell(string school, string id)
            : base(school, id) { }

        public override int GetManaCost(Farmer player, int level)
        {
            return 0;
        }

        public override IActiveEffect OnCast(Farmer player, int level, int targetX, int targetY)
        {
            Log.Debug($"{player.Name} cast {this.Id}.");
            return null;
        }
    }
}
