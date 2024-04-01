/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

namespace MageDelve.Skill
{
    public class GenericProfession : SpaceCore.Skills.Skill.Profession
    {
        /*********
        ** Public methods
        *********/
        public GenericProfession(ArcanaSkill skill, string theId)
            : base(skill, theId) { }

        public override string GetName()
        {
            return I18n.GetByKey("profession." + Id + ".name");
        }

        public override string GetDescription()
        {
            return I18n.GetByKey("profession." + Id + ".description");
        }
    }
}
