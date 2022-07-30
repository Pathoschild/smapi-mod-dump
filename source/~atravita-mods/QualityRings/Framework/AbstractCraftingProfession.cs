/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using SpaceCore;

namespace QualityRings.Framework;
internal abstract class AbstractCraftingProfession : Skills.Skill.Profession
{

    protected AbstractCraftingProfession(CraftingSkill skill, string id)
        : base(skill, id)
    {
    }

    public override string GetDescription() => throw new NotImplementedException();

    public override string GetName() => throw new NotImplementedException();
}
