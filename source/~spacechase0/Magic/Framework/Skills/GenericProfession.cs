/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

namespace Magic.Framework.Skills
{
    internal class GenericProfession : SpaceCore.Skills.Skill.Profession
    {
        /*********
        ** Accessors
        *********/
        public string Name { get; set; }
        public string Description { get; set; }


        /*********
        ** Public methods
        *********/
        public GenericProfession(Skill skill, string theId)
            : base(skill, theId) { }

        public override string GetName()
        {
            return this.Name;
        }

        public override string GetDescription()
        {
            return this.Description;
        }
    }
}
