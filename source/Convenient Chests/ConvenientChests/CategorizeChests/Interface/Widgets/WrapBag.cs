/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using System;
using Microsoft.Xna.Framework;

namespace ConvenientChests.CategorizeChests.Interface.Widgets
{
    /// <summary>
    /// A container that automatically positions its children in rows,
    /// wrapping to a new row as appropriate.
    /// </summary>
    class WrapBag : Widget
    {
        public WrapBag(int width)
        {
            Width = width;
        }

        protected override void OnContentsChanged()
        {
            base.OnContentsChanged();

            var x = 0;
            var y = 0;
            var lowestBottom = 0;
            foreach (var child in Children)
            {
                if (x + child.Width > Width && x > 0)
                {
                    x = 0;
                    y = lowestBottom;
                }

                child.Position = new Point(x, y);
                x += child.Width;

                lowestBottom = Math.Max(lowestBottom, child.Position.Y + child.Height);
            }

            Height = lowestBottom;
        }
    }
}