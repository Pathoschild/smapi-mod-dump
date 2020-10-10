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
using System.Collections.Generic;

namespace Pong.Framework.Menus.Elements
{
    internal class ElementContainer : IDrawable
    {
        public IList<IDrawable> Elements { get; } = new List<IDrawable>();

        public void Draw(SpriteBatch b)
        {
            foreach (IDrawable element in this.Elements)
                element.Draw(b);
        }
    }
}
