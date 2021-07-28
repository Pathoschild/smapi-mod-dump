/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

namespace SkillPrestige.Professions.Registration
{
    public abstract class ProfessionRegistration : Profession, IProfessionRegistration
    {
        /// <summary>
        /// This call will 'register' available professions with the profession class.
        /// </summary>
        public abstract void RegisterProfessions();

        /// <summary>
        /// Returns a level available at of 0, as this class is used solely to handle registration of static members of it's base class that are all declared in partial classes.
        /// </summary>
        public override int LevelAvailableAt => 0;
    }
}
