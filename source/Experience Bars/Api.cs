/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/ExperienceBars
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ExperienceBars
{
    public interface IApi
    {
        void DrawExperienceBar(Texture2D icon, int level, float percentFull, Color color);
        void SetDrawLuck(bool luck);
    }

    public class Api : IApi
    {
        public void DrawExperienceBar(Texture2D icon, int level, float percentFull, Color color)
        {
            Mod.renderSkillBar(Mod.Config.X, Mod.expBottom, icon, new Rectangle(0, 0, icon.Width, icon.Height), level, percentFull, color);
            Mod.expBottom += 40;
        }

        public void SetDrawLuck(bool luck)
        {
            Mod.renderLuck = luck;
        }
    }
}
