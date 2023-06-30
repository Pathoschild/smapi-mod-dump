/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using Microsoft.Xna.Framework;

namespace MultiplayerExpShare;

public interface IMultiplayerExpShareAPI
{
    /// <summary>
    /// Add trail particle for a skill with id <paramref name="skill_id"/>.
    /// The <paramref name="color"/> will be the color of the head of the particle, the tail is always white.
    /// 
    /// If you have made a SpaceCore based skill mod, use the SpaceCore skill ID.
    /// 
    /// The vanilla Skill ids are "Farming", "Foraging", "Fishing", "Mining" and "Combat".
    /// </summary>
    /// <param name="skill_id">ID of skill </param>
    /// <param name="color">Color of particle head</param>
    public void AddSkillTrailParticle(string skill_id, Color color);
}
