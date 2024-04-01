/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using MageDelve.Mana;
using StardewValley;

namespace MageDelve.Skill
{
    internal class ManaCapProfession : GenericProfession
    {
        private int amount;

        /*********
        ** Public methods
        *********/
        public ManaCapProfession(ArcanaSkill skill, string theId, int amt)
            : base(skill, theId)
        {
            amount = amt;
        }

        public override void DoImmediateProfessionPerk()
        {
            Game1.player.SetMaxMana(Game1.player.GetMaxMana() + amount);
        }
    }
}
