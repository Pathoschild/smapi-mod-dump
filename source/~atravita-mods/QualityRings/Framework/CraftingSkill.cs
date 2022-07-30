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
internal class CraftingSkill : Skills.Skill
{
    public CraftingSkill()
        : base("atravita.CraftingSkill")
    {
    }

    public override string GetName() => throw new NotImplementedException();
}
