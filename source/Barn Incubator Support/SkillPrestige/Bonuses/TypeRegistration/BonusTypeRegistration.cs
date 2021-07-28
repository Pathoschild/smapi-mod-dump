/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

namespace SkillPrestige.Bonuses.TypeRegistration
{
    public abstract class BonusTypeRegistration : BonusType, IBonusTypeRegistration
    {
        /// <summary>
        /// This call will 'register' available professions with the bonus type class.
        /// </summary>
        public abstract void RegisterBonusTypes();
    }
}
