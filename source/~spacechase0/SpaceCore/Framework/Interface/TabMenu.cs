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

namespace SpaceCore.Framework.Interface
{
    internal abstract class TabMenu
    {
        public abstract string Name { get; }
        public readonly TabbedMenu Parent;

        public virtual void Update(GameTime gt) { }

        public abstract void Draw(SpriteBatch b);

        public virtual void MouseMove(int x, int y) { }

        public virtual void LeftClick(int x, int y) { }

        protected TabMenu(TabbedMenu parent)
        {
            this.Parent = parent;
        }
    }
}
