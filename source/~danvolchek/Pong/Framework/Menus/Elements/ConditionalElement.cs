/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Pong.Framework.Common;

namespace Pong.Framework.Menus.Elements
{
    internal class ConditionalElement : IDrawable
    {
        private readonly IDrawable element;
        private readonly DrawCondition condition;

        public ConditionalElement(IDrawable element, DrawCondition condition)
        {
            this.element = element;
            this.condition = condition;
        }

        public IDrawable GetElementForHighlight()
        {
            return this.condition() ? this.element : null;
        }

        public void Draw(SpriteBatch b)
        {
            if (this.condition())
                this.element.Draw(b);
        }

        public delegate bool DrawCondition();
    }
}
