/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using SpaceCore;

namespace CookingSkill.Framework
{
    internal class GenericProfession : Skills.Skill.Profession
    {
        /*********
        ** Fields
        *********/
        /// <summary>Get the translated profession name.</summary>
        private readonly Func<string> GetNameImpl;

        /// <summary>Get the translated profession name.</summary>
        private readonly Func<string> GetDescriptionImpl;


        /*********
        ** Accessors
        *********/
        /// <inheritdoc />
        public override string GetName()
        {
            return this.GetNameImpl();
        }

        /// <inheritdoc />
        public override string GetDescription()
        {
            return this.GetDescriptionImpl();
        }

        /// <summary>Construct an instance.</summary>
        /// <param name="skill">The parent skill.</param>
        /// <param name="id">The unique profession ID.</param>
        /// <param name="name">The translated profession name.</param>
        /// <param name="description">The translated profession description.</param>
        public GenericProfession(Skill skill, string id, Func<string> name, Func<string> description)
            : base(skill, id)
        {
            this.GetNameImpl = name;
            this.GetDescriptionImpl = description;
        }
    }
}
