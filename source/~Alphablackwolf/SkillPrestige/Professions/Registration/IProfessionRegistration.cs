/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

namespace SkillPrestige.Professions.Registration
{
    public interface IProfessionRegistration
    {
        /// <summary>
        /// This call will 'register' available professions with the profession class.
        /// </summary>
        void RegisterProfessions();
    }
}