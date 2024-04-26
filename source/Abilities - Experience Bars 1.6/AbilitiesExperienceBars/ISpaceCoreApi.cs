/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sandman534/Abilities-Experience-Bars
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace AbilitiesExperienceBars
{
    // https://github.com/spacechase0/StardewValleyMods/blob/develop/SpaceCore/Api.cs
    public interface ISpaceCoreApi
    {
        string[] GetCustomSkills();
        int GetLevelForCustomSkill(Farmer farmer, string skill);
        int GetExperienceForCustomSkill(Farmer farmer, string skill);
        Texture2D GetSkillIconForCustomSkill(string skill);
        Texture2D GetSkillPageIconForCustomSkill(string skill);
    }
}
