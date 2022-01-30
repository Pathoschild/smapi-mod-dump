/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Alchemy.Framework.Extensions;

#region using directives

using SpaceCore;
using StardewValley;

using Skill;

#endregion using directives

public static class FarmerExtensions
{
    public static void AddAlchemyExperience(this Farmer farmer, int howMuch)
    {
        Skills.AddExperience(farmer, AlchemySkill.InternalName, howMuch);
    }

    public static int GetAlchemyLevel(this Farmer farmer)
    {
        return Skills.GetSkillLevel(farmer, AlchemySkill.InternalName);
    }

    public static int GetTotalCurrentAlchemyExperience(this Farmer farmer)
    {
        return Skills.GetExperienceFor(farmer, AlchemySkill.InternalName);
    }


}