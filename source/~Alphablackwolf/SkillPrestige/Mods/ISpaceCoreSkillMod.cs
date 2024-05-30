/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

namespace SkillPrestige.Mods
{
    /// <summary>Interface that all skill mods need to implement in order to register with Skill Prestige.</summary>
    public interface ISpaceCoreSkillMod : ISkillMod
    {
        //ID to lookup space core skill
        string SpaceCoreSkillId { get; }
    }
}
