/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceCore.Interface
{
    public abstract class TabMenu
    {
        public abstract string Name { get; }
        public readonly TabbedMenu Parent;

        public TabMenu( TabbedMenu parent )
        {
            Parent = parent;
        }

        public virtual void update(GameTime gt)
        {
        }

        public abstract void draw(SpriteBatch b);

        public virtual void mouseMove(int x, int y)
        {
        }

        public virtual void leftClick(int x, int y)
        {
        }
    }
}
