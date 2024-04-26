/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

namespace SkillPrestige.Professions
{
    /// <summary>Represents special handling for professions where Stardew Valley applies the profession's effects in a custom manner.</summary>
    public interface IProfessionSpecialHandling
    {
        /*********
        ** Methods
        *********/
        /// <summary>Apply effects for the profession.</summary>
        void ApplyEffect();

        /// <summary>Remove effects for the profession.</summary>
        void RemoveEffect();
    }
}
