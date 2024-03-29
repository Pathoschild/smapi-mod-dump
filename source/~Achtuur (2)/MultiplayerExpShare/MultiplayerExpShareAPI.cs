/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Framework;
using Microsoft.Xna.Framework;

namespace MultiplayerExpShare;

internal class MultiplayerExpShareAPI : IMultiplayerExpShareAPI
{
    public void AddSkillTrailParticle(string skill_id, Color color)
    {
        ModEntry.ShareTrailParticles.Add(
            skill_id,
            new TrailParticle(
                Vector2.Zero,
                Vector2.Zero,
                ModEntry.Instance.ParticleTrailLength,
                color,
                ModEntry.Instance.ParticleSize
            )
        );
    }
}
