/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CookingSkill.Framework
{
    public interface ExperienceBarsApi
    {
        void DrawExperienceBar(Texture2D icon, int level, float percentFull, Color color);
        void SetDrawLuck(bool luck);
    }
}
