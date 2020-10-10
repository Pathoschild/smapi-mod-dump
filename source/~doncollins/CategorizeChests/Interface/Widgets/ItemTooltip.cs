/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/doncollins/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace StardewValleyMods.CategorizeChests.Interface.Widgets
{
    /// <summary>
    /// A tooltip showing information about a particular item.
    /// </summary>
    class ItemTooltip : Widget
    {
        public ItemTooltip(string name)
        {
            var background = AddChild(new Background(Sprites.TooltipBackground));
            var label = AddChild(new Label(name, Color.Black));

            Width = background.Width = label.Width + background.Graphic.LeftBorderThickness +
                                       background.Graphic.RightBorderThickness;
            Height = background.Height = label.Height + background.Graphic.TopBorderThickness +
                                         background.Graphic.BottomBorderThickness;

            label.Position = new Point(
                background.Width / 2 - label.Width / 2,
                background.Height / 2 - label.Height / 2
            );
        }
    }
}