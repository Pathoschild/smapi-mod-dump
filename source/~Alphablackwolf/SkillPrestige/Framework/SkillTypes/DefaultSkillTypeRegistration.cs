/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using SkillPrestige.Logging;
using SkillPrestige.SkillTypes;

namespace SkillPrestige.Framework.SkillTypes
{
    /// <summary>Class registers all the skill types that are available in Stardew Valley by default.</summary>
    // ReSharper disable once UnusedMember.Global - created through reflection.
    internal sealed class DefaultSkillTypeRegistration : SkillType, ISkillTypeRegistration
    {
        /*********
        ** Public methods
        *********/
        public void RegisterSkillTypes()
        {
            Logger.LogInformation("Registering default skill types...");
            Farming = new SkillType("Farming", 0);
            Fishing = new SkillType("Fishing", 1);
            Foraging = new SkillType("Foraging", 2);
            Mining = new SkillType("Mining", 3);
            Combat = new SkillType("Combat", 4);
            Logger.LogInformation("Default skill types registered.");
        }
    }
}

