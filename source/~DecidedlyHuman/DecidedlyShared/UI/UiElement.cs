/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using DecidedlyShared.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace DecidedlyShared.UI
{
    public class UiElement : ClickableTextureComponent
    {
        private readonly Alignment childAlignment;
        private readonly List<UiElement> childElements;
        private readonly Orientation childOrientation;
        private readonly int elementSpacing;
        private readonly string labelText;
        private readonly int margins = 16;
        private int currentTopIndex;
        private bool isHovered;
        private int maxTextLength = 500;
        private UiElement? okayButton;
        private UiElement? parentElement;
        private UiElement? titleBar;
        private List<UiElement> visibleElements;

        public UiElement(string name, string label, string hoverText, int width = 0, int height = 0,
                         Orientation orientation = Orientation.Vertical, Alignment childAlignment = Alignment.Middle,
                         int elementSpacing = 0) : base(new Rectangle(0, 0, 0, 0), Game1.menuTexture,
            new Rectangle(0, 256, 60, 60), 1f)
        {
            this.childElements = new List<UiElement>();
            this.bounds.Width = width;
            this.bounds.Height = height;
            this.childAlignment = childAlignment;
            this.childOrientation = orientation;
            this.elementSpacing = elementSpacing;
            this.labelText = label;
            this.visibleElements = new List<UiElement>();
        }

        public int MaximumElementsVisible { get; set; } = 4;

        public int CurrentTopIndex
        {
            get => this.currentTopIndex;
            set
            {
                if (value < 0)
                {
                    this.currentTopIndex = 0;
                    this.UpdateElements();

                    return;
                }

                if (value > this.childElements.Count - this.MaximumElementsVisible - 1)
                {
                    this.currentTopIndex = this.childElements.Count - this.MaximumElementsVisible - 1;
                    this.UpdateElements();

                    return;
                }

                this.currentTopIndex = value;
                this.UpdateElements();
            }
        }

        public void ReceiveCursorHover(int x, int y)
        {
            if (this.bounds.Contains(x, y))
                this.isHovered = true;
            else
                this.isHovered = false;

            foreach (var element in this.visibleElements)
                element.ReceiveCursorHover(x, y);
        }

        public void AddElement(UiElement element)
        {
            element.parentElement = this;
            this.childElements.Add(element);

            if (!element.labelText.Equals(""))
            {
                element.bounds.Width = (int)Game1.dialogueFont.MeasureString(element.labelText).X;
                element.bounds.Height = (int)Game1.dialogueFont.MeasureString(element.labelText).Y;
            }
        }

        public void UpdateElements()
        {
            this.visibleElements = this.childElements.GetRange(this.currentTopIndex, this.MaximumElementsVisible);

            if (this.visibleElements.Any())
            {
                this.ResizeToChildren();
                this.UpdateChildrenPositions();
            }
        }

        public void CalculateInitialSize()
        {
            int widestElement = 0;
            int tallestElement = 0;
            int cumulativeHeight = 0;

            cumulativeHeight += this.margins;

            foreach (var element in this.childElements)
            {
                if (element.bounds.Width > widestElement)
                    widestElement = element.bounds.Width;

                if (element.bounds.Height > tallestElement)
                    tallestElement = element.bounds.Height;

                cumulativeHeight += element.bounds.Height + this.elementSpacing;
            }

            cumulativeHeight += this.margins;

            this.bounds.Width = widestElement + this.margins * 2;
            this.bounds.Height = cumulativeHeight;
        }

        public void ResizeToChildren()
        {
            switch (this.childOrientation)
            {
                case Orientation.Horizontal:
                    int totalWidth = 0;
                    int highestElement = 0;

                    // Calculate the total width based on all of our elements, plus the global element spacing.
                    foreach (var element in this.visibleElements)
                    {
                        totalWidth += element.bounds.Width + this.elementSpacing;

                        if (element.bounds.Height > highestElement)
                            highestElement = element.bounds.Height;
                    }

                    // And subtract one bit of spacing.
                    totalWidth -= this.elementSpacing;

                    // Then add in our margins.
                    totalWidth += this.margins * 2;
                    highestElement += this.margins * 2;

                    this.bounds.Width = totalWidth;
                    this.bounds.Height = highestElement;
                    break;
                case Orientation.Vertical:
                    int totalHeight = 0;
                    // int widestElement = 0;

                    // Calculate the total width based on all of our elements, plus the global element spacing.
                    foreach (var element in this.visibleElements)
                        totalHeight += element.bounds.Height + this.elementSpacing;
                    // if (element.bounds.Width > widestElement)
                    //     widestElement = element.bounds.Width;
                    // And subtract one bit of spacing.
                    totalHeight -= this.elementSpacing;

                    // Then add in our margins.
                    totalHeight += this.margins * 2;
                    //widestElement += margins * 2;

                    this.bounds.Height = totalHeight;
                    // this.bounds.Width = widestElement;
                    break;
            }
        }

        public void UpdateChildrenPositions()
        {
            if (this.childOrientation == Orientation.Horizontal)
            {
                int totalPriorElementWidth = 0;

                foreach (var element in this.visibleElements)
                {
                    totalPriorElementWidth += element.bounds.Width + this.elementSpacing;

                    element.bounds.X = element.parentElement.bounds.Left + totalPriorElementWidth;

                    switch (this.childAlignment)
                    {
                        case Alignment.Top:
                            element.bounds.Y = element.parentElement.bounds.Top + totalPriorElementWidth;
                            break;
                        case Alignment.Middle:
                            element.bounds.Y = element.parentElement.bounds.Height / 2 + element.bounds.Height +
                                               totalPriorElementWidth;
                            break;
                        case Alignment.Bottom:
                            element.bounds.Y = element.parentElement.bounds.Height - element.bounds.Height +
                                               totalPriorElementWidth;
                            break;
                    }

                    totalPriorElementWidth += this.elementSpacing;
                }
            }
            else if (this.childOrientation == Orientation.Vertical)
            {
                int totalPriorElementHeight = this.margins;
                string blah = this.labelText;
                string? blah2 = this.name;

                foreach (var element in this.visibleElements)
                {
                    switch (this.childAlignment)
                    {
                        case Alignment.Left:
                            element.bounds.X = element.parentElement.bounds.Left + element.parentElement.margins;
                            element.bounds.Y = element.parentElement.bounds.Top + totalPriorElementHeight;
                            break;
                        case Alignment.Middle:
                            element.bounds.X = element.parentElement.bounds.Center.X - element.bounds.Width / 2;
                            element.bounds.Y = element.parentElement.bounds.Top + totalPriorElementHeight;
                            break;
                        case Alignment.Right:
                            element.bounds.Y = element.parentElement.bounds.Height - element.bounds.Height +
                                               totalPriorElementHeight;
                            break;
                    }

                    totalPriorElementHeight += this.elementSpacing + element.bounds.Height;
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            // If the label isn't blank, assume we're simply a text label.
            if (!this.labelText.Equals(""))
            {
                // If our parentElement is null, we're the root element, so we don't want a hover effect.
                if (this.isHovered && this.parentElement != null)
                    sb.Draw(
                        Game1.mouseCursors,
                        new Rectangle(this.parentElement.bounds.Left + 16, this.bounds.Y,
                            this.parentElement.bounds.Width - 32, this.bounds.Height),
                        new Rectangle(269, 520, 1, 1),
                        new Color(221, 148, 84)
                    );

                Drawing.DrawStringWithShadow(
                    sb,
                    Game1.dialogueFont,
                    this.labelText,
                    new Vector2(this.bounds.X, this.bounds.Y),
                    Color.Black,
                    new Color(221, 148, 84));
            }
            else
            {
                // We want to draw ourselves first, if we have a texture.
                if (this.texture != null)
                    //sb.Draw(this.texture, this.bounds, this.sourceRect, Color.White);
                    IClickableMenu.drawTextureBox(
                        sb,
                        this.texture,
                        this.sourceRect,
                        this.bounds.X,
                        this.bounds.Y,
                        this.bounds.Width,
                        this.bounds.Height,
                        Color.White,
                        1f,
                        this.labelText.Equals("") ? false : true
                    );
            }

            // And our children second, so we don't cover them.
            // foreach (UiElement element in childElements)
            // {
            //     element.Draw(sb);
            // }

            foreach (var element in this.visibleElements)
                element.Draw(sb);

            // for (int i = currentTopIndex; i < Math.Min(childElements.Count, maxElementsVisible); i++)
            // {
            //     childElements[i].Draw(sb);
            // }
        }
    }
}
