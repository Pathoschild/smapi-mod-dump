/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

namespace SkillPrestige.Framework.SkillTypes
{
    /// <summary>Allows for a registration of a skill type with the base skill type class.</summary>
    internal interface ISkillTypeRegistration
    {
        /// <summary>Implementations of this method should initialize static implementations of skill types in the SkillType class.</summary>
        void RegisterSkillTypes();
    }
}
