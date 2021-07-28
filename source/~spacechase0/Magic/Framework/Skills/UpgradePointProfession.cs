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
    internal class UpgradePointProfession : GenericProfession
    {
        /*********
        ** Public methods
        *********/
        public UpgradePointProfession(Skill skill, string theId)
            : base(skill, theId) { }

        public override void DoImmediateProfessionPerk()
        {
            Game1.player.GetSpellBook().UseSpellPoints(-2);
        }
    }
}
