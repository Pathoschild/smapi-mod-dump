/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace Magic.Framework.Skills
{
    internal class ManaCapProfession : GenericProfession
    {
        /*********
        ** Public methods
        *********/
        public ManaCapProfession(Skill skill, string theId)
            : base(skill, theId) { }

        public override void DoImmediateProfessionPerk()
        {
            Game1.player.SetMaxMana(Game1.player.GetMaxMana() + 500);
        }
    }
}
