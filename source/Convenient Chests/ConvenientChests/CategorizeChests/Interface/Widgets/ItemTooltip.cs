/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;

namespace ConvenientChests.CategorizeChests.Interface.Widgets
{
    /// <summary>
    /// A tooltip showing information about a particular item.
    /// </summary>
    class ItemTooltip : Widget
    {
        public ItemTooltip(string name, string description = "")
        {
            var background = AddChild(new Background(Sprites.TooltipBackground));
            var label = AddChild(new Label(name, Color.Black, Game1.dialogueFont));

            var descriptionLabel = AddChild(new Label(description, Color.Black, Game1.smallFont));

            Width = background.Width = Math.Max(label.Width, descriptionLabel.Width) + background.Graphic.LeftBorderThickness +
                                       background.Graphic.RightBorderThickness;
            Height = background.Height = label.Height + descriptionLabel.Height + background.Graphic.TopBorderThickness +
                                         background.Graphic.BottomBorderThickness;

            label.Position = new Point(
                background.Graphic.LeftBorderThickness,
                background.Graphic.TopBorderThickness
            );
            
            descriptionLabel.Position = new Point(
                background.Graphic.LeftBorderThickness,
                background.Graphic.TopBorderThickness + label.Height
                );
        }
    }
}