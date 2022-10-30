/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using DecidedlyShared.Logging;
using DecidedlyShared.Utilities;
using Microsoft.Build.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.HomeRenovations;
using StardewValley.Menus;

namespace DecidedlyShared.UIOld
{
    public class UiElement : ClickableTextureComponent
    {
        private readonly Alignment childAlignment;
        private readonly List<UiElement> childElements;
        private readonly Orientation childOrientation;
        private readonly int elementSpacing;
        private readonly string labelText;
        private readonly int margins = 16;
        private Logger? logger;
        private int currentTopIndex;
        private bool isHovered;
        private bool showScrollBar = false;
        private bool showSearchBox = false;
        private bool scrollBarBeingDragged = false;
        private string searchString = "";

        private int maxTextLength = 500;
        private UiElement? okayButton;
        private UiElement? parentElement;
        private UiElement? titleBar;
        private UiElement? scrollUpArrow;
        private UiElement? scrollDownArrow;
        private UiElement? scrollBarArea;
        private UiElement? scrollBarHandle;
        public TextBox? searchBox; // Bad.
        private List<UiElement> visibleElements;
        private Action clickAction;

        public Action OnClick
        {
            get => this.clickAction;
            set => this.clickAction = value;
        }
        public bool ShowScrollBar
        {
            get => this.showScrollBar;
            set
            {
                if (value == true)
                {
                    this.scrollUpArrow = new UiElement("ScrollUp", "", "Scroll up", 16, 16);
                    this.scrollDownArrow = new UiElement("ScrollDown", "", "Scroll down", 16, 16);
                    this.scrollBarArea = new UiElement("ScrollArea", "", "", 16, 16);
                    this.scrollBarHandle = new UiElement("ScrollBarHandle", "", "", 16, 16);
                    this.scrollUpArrow.texture = Game1.mouseCursors;
                    this.scrollDownArrow.texture = Game1.mouseCursors;
                    this.scrollBarArea.texture = Game1.mouseCursors;
                    this.scrollBarHandle.texture = Game1.mouseCursors;
                    this.scrollDownArrow.sourceRect = new Rectangle(12, 76, 40, 44);
                    this.scrollUpArrow.sourceRect = new Rectangle(76, 72, 40, 44);
                    this.scrollBarArea.sourceRect = new Rectangle(403, 383, 6, 6);
                    this.scrollBarHandle.sourceRect = new Rectangle(435, 463, 6, 10);
                    this.scrollUpArrow.OnClick += () => { this.CurrentTopIndex -= 1; };
                    this.scrollDownArrow.OnClick += () => { this.CurrentTopIndex += 1; };
                    this.scrollBarArea.OnClick += () => { };

                    this.PositionScrollElements();
                }

                this.showScrollBar = value;
            }
        }

        public bool ShowSearchBox
        {
            get => this.showSearchBox;
            set
            {
                if (value == true)
                {
                    this.searchBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"),
                        null,
                        Game1.smallFont,
                        Game1.textColor);
                }

                this.showSearchBox = value;
            }
        }

        public string SearchString
        {
            get => this.searchString;
            set
            {
                this.searchString = value;
            }
        }

        public UiElement(string name, string label, string hoverText, int width = 0, int height = 0,
                         Orientation orientation = Orientation.Vertical, Alignment childAlignment = Alignment.Middle,
                         int elementSpacing = 0, Logger? logger = null, Action? onClick = null) : base(new Rectangle(0, 0, 0, 0), Game1.menuTexture,
            new Rectangle(0, 256, 60, 60), 1f)
        {
            this.name = name;
            this.hoverText = hoverText;
            this.childElements = new List<UiElement>();
            this.bounds.Width = width;
            this.bounds.Height = height;
            this.childAlignment = childAlignment;
            this.childOrientation = orientation;
            this.elementSpacing = elementSpacing;
            this.labelText = label;
            this.visibleElements = new List<UiElement>();
            this.logger = logger;
            if (onClick != null) this.OnClick = onClick;
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
                    this.currentTopIndex = this.childElements.Count - this.MaximumElementsVisible;
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

            this.UpdateElements();
        }

        public void UpdateElements()
        {
            if (this.searchString == "")
            {
                this.visibleElements = this.childElements.GetRange(Math.Max(this.currentTopIndex, 0),
                    Math.Min(this.MaximumElementsVisible, this.childElements.Count));
            }
            else
            {
                List<UiElement>? firstElements = new List<UiElement>();

                foreach (UiElement element in this.childElements)
                {
                    if (element.labelText.Contains(this.SearchString) || element.name.Contains(this.SearchString))
                        firstElements.Add(element);
                }

                this.visibleElements = firstElements.GetRange(Math.Max(this.currentTopIndex, 0),
                    Math.Min(this.MaximumElementsVisible, firstElements.Count));
            }

            if (this.visibleElements.Any())
            {
                this.ResizeToChildren();
                this.UpdateChildrenPositions();
                if (this.ShowScrollBar)
                    this.PositionScrollElements();
            }

            if (this.ShowSearchBox)
            {
                this.searchBox.X = this.bounds.Left + this.bounds.Width / 2 - this.searchBox.Width / 2;
                this.searchBox.Y = this.bounds.Top - 40;
            }
        }

        private void UpdateSearchBox()
        {
            if (this.ShowSearchBox)
            {
                this.searchBox.Width = this.bounds.Width / 2;
                this.searchBox.Update();
            }
        }

        private void PositionScrollElements()
        {
            if (this.scrollDownArrow == null || this.scrollUpArrow == null)
                return;

            this.scrollBarArea.bounds =
                new Rectangle(this.bounds.Right + 16, this.bounds.Top + 22, 48, this.bounds.Height - 44);

            this.scrollBarHandle.bounds.Width = 48;
            this.scrollBarHandle.bounds.Height = 44;
            this.scrollBarHandle.bounds.X = this.bounds.Right + 16;
                // new Rectangle(this.bounds.Right + 16, this.bounds.Top, 48, this.bounds.Height);
            // this.scrollUpArrow.bounds = new Rectangle(this.bounds.Right + 8, this.bounds.Top, 40, 44);
            // this.scrollDownArrow.bounds = new Rectangle(this.bounds.Right + 8, this.bounds.Bottom - 44, 40, 44);

            // int barSteps = Math.Max(this.childElements.Count - this.MaximumElementsVisible, 1);
            // //int barSteps = Math.Max(this.childElements.Count - this.MaximumElementsVisible, 1);
            // // int barSteps = this.scrollBarArea.bounds.Height / this.childElements.Count;
            // int barStepSize = (this.scrollBarArea.bounds.Height / barSteps);
            // int barStepSize = Math.Max((this.scrollBarArea.bounds.Height - this.scrollBarHandle.bounds.Height / Math.Max(barSteps, 1)), 1);

            // if (this.CurrentTopIndex == this.childElements.Count - this.MaximumElementsVisible)
            // if (this.CurrentTopIndex == 0)
            //     this.scrollBarHandle.bounds.Y = this.scrollBarArea.bounds.Top;
            // else
            //     this.scrollBarHandle.bounds.Y = this.scrollBarArea.bounds.Top + this.CurrentTopIndex * barStepSize -
            //                                     this.scrollBarHandle.bounds.Height / 2 - 4;
            // else
            //     this.scrollBarHandle.bounds.Y = this.scrollBarArea.bounds.Top + this.CurrentTopIndex * barStepSize;

            // if (this.CurrentTopIndex == 0)
            //     this.scrollBarHandle.bounds.Y = this.scrollBarArea.bounds.Top + this.CurrentTopIndex * barStepSize;
            // else
            //     this.scrollBarHandle.bounds.Y = this.scrollBarArea.bounds.Top + (this.CurrentTopIndex * barStepSize) - this.scrollBarHandle.bounds.Height;

            // if (this.CurrentTopIndex == this.childElements.Count - 12)
            //     this.scrollBarHandle.bounds.Y = this.scrollBarArea.bounds.Top + this.CurrentTopIndex * barStepSize -
            //                                     barStepSize;
            // else

            this.scrollBarHandle.bounds.Y = UiHelpers.GetYPositionInScrollArea(this.childElements.Count,
                this.MaximumElementsVisible, this.scrollBarArea.bounds.Height,
                this.scrollBarArea.bounds.Top, this.CurrentTopIndex);

            // this.scrollBarHandle.bounds.Y = this.scrollBarArea.bounds.Top + this.CurrentTopIndex * barStepSize;

            // this.scrollBarHandle.bounds.Y = this.scrollBarArea.bounds.Top;
            // this.scrollBarHandle.bounds.Height = this.scrollBarArea.bounds.Height / this.CurrentTopIndex == 0 ? 1 : this.CurrentTopIndex;

            // TODO: REMOVE THIS OUT OF DEBUG.
            #if DEBUG
            if (this.logger != null)
            {
                // this.logger.Log($"Current top index: {this.CurrentTopIndex}", LogLevel.Info);
                // this.logger.Log($"Child elements: {this.childElements.Count}", LogLevel.Info);
                // this.logger.Log($"Max visible elements: {this.MaximumElementsVisible}", LogLevel.Info);
                // this.logger.Log($"Main UI height: {this.bounds.Height}", LogLevel.Info);
                // this.logger.Log($"Scroll bar area height: {this.scrollBarArea.bounds.Height}", LogLevel.Info);
                // this.logger.Log($"Scroll bar handle height: {this.scrollBarHandle.bounds.Height}", LogLevel.Info);
                // this.logger.Log($"Scroll bar handle Y: {this.scrollBarHandle.bounds.Y}", LogLevel.Info);
                // this.logger.Log($"Scroll bar handle Y to hit bottom of bar: {this.scrollBarArea.bounds.Bottom - this.scrollBarHandle.bounds.Height}", LogLevel.Info);
                // this.logger.Log($"Elements outside of visible area: {this.childElements.Count - this.MaximumElementsVisible}", LogLevel.Info);
                // this.logger.Log($"Bar steps: {barSteps}", LogLevel.Info);
                // this.logger.Log($"Bar step size: {barStepSize}", LogLevel.Info);
                // this.logger.Log($"", LogLevel.Info);
                // this.logger.Log($"", LogLevel.Info);
            }
            #endif
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

        public void Draw(SpriteBatch sb, float scale = 1f, bool isScrollHandle = false)
        {
            // TODO: REMOVE THIS OUT OF DEBUG.
#if DEBUG
            this.PositionScrollElements();
#endif

            this.UpdateSearchBox();

            // if (this.showScrollBar)
            // {
            //     IClickableMenu.drawTextureBox(sb, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
            //         this.scrollBarArea.bounds.X, this.scrollBarArea.bounds.Y, this.scrollBarArea.bounds.Width,
            //         this.scrollBarArea.bounds.Height, Color.White, 4f, drawShadow: false);
            //     this.scrollBarHandle?.Draw(sb, 4f, true);
            // }

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
                if (isScrollHandle)
                {
                    // sb.Draw(
                    //     this.texture,
                    //     this.bounds,
                    //     this.sourceRect,
                    //     Color.White);

                    // IClickableMenu.drawTextureBox(
                    //     sb,
                    //     this.texture,
                    //     new Rectangle(this.sourceRect.X, this.sourceRect.Y, this.sourceRect.Width, this.sourceRect.Height + 8),
                    //     this.bounds.X,
                    //     this.bounds.Y,
                    //     this.bounds.Width,
                    //     this.bounds.Height,
                    //     Color.White,
                    //     scale,
                    //     this.labelText.Equals("") ? false : true
                    // );

                    IClickableMenu.drawTextureBox(sb, Game1.mouseCursors, new Rectangle(403, 383, 6, 6),
                        this.bounds.X, this.bounds.Y, this.bounds.Width,
                        this.bounds.Height, Color.SandyBrown, 4f, drawShadow: false);
                }

                // We want to draw ourselves first, if we have a texture.
                if (this.texture != null && !isScrollHandle)
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
                        scale,
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

            // Now we draw our scroll arrows.
            if (this.showScrollBar)
            {


                IClickableMenu.drawTextureBox(sb, Game1.mouseCursors, new Rectangle(403, 383, 6, 6),
                    this.scrollBarArea.bounds.X, this.scrollBarArea.bounds.Y, this.scrollBarArea.bounds.Width,
                    this.scrollBarArea.bounds.Height, Color.White, 4f, drawShadow: false);
                this.scrollBarHandle?.Draw(sb, 4f, true);
                // this.scrollUpArrow?.Draw(sb);
                // this.scrollDownArrow?.Draw(sb);
            }

            if (this.ShowSearchBox)
            {
                this.searchBox.Draw(sb, false);
            }

            //     for (int i = currentTopIndex; i < Math.Min(childElements.Count, maxElementsVisible); i++)
            //     {
            //         childElements[i].Draw(sb);
            //     }
            // }
        }

        public void LeftClickHeld(int x, int y)
        {
            if (this.scrollBarBeingDragged)
            {
                int newIndex = UiHelpers.GetTopIndexFromYPosition(
                    this.childElements.Count,
                    this.MaximumElementsVisible,
                    this.scrollBarArea.bounds.Height,
                    this.scrollBarArea.bounds.Top, y - 48);
                this.CurrentTopIndex = newIndex;

                // this.scrollBarHandle.bounds.Y = UiHelpers.GetYPositionInScrollArea(
                //     this.childElements.Count, this.MaximumElementsVisible,
                //     this.scrollBarArea.bounds.Height, this.scrollBarArea.bounds.Top,
                //     this.CurrentTopIndex);
            }
        }

        public void LeftClickRelease(int x, int y)
        {
            this.scrollBarBeingDragged = false;
        }

        public void LeftClick(int x, int y)
        {
            // if (!this.ShowScrollBar)
            // {
            //     UiElement? hitChildElement = null;
            //
            //     foreach (UiElement element in this.childElements)
            //     {
            //         if (element.containsPoint(x, y))
            //         {
            //             hitChildElement = element;
            //             break;
            //         }
            //     }
            //
            //     // If the child element was hit, we pass the click coordinates into it so it can check its children.
            //     if (hitChildElement != null)
            //         hitChildElement.LeftClick(x, y);
            //     else
            //     {
            //         // If the child element wasn't hit, we can assume the click was for us, so we consume it.
            //         this.ConsumeLeftClick();
            //     }
            //
            //     // Finally, we want to check our scroll arrows.
            //     if (this.ShowScrollBar)
            //     {
            //         if (this.scrollUpArrow.containsPoint(x, y))
            //             this.scrollUpArrow.OnClick.Invoke();
            //         if (this.scrollDownArrow.containsPoint(x, y))
            //             this.scrollDownArrow.OnClick.Invoke();
            //     }
            // }
            // else
            // {

            if (this.ShowScrollBar)
            {
                this.scrollBarBeingDragged = this.scrollBarArea.bounds.Contains(x, y);

                if (this.scrollBarBeingDragged)
                    return;
            }

            if (this.ShowSearchBox)
            {

            }

            UiElement? hitChildElement = null;

            foreach (UiElement element in this.visibleElements)
            {
                if (element.containsPoint(x, y))
                {
                    hitChildElement = element;
                    break;
                }
            }

            // If the child element was hit, we pass the click coordinates into it so it can check its children.
            if (hitChildElement != null)
                hitChildElement.LeftClick(x, y);
            else
            {
                // If the child element wasn't hit, we can assume the click was for us, so we consume it.
                this.ConsumeLeftClick();
            }

            // // Finally, we want to check our scroll arrows.
            // if (this.ShowScrollBar)
            // {
            //     if (this.scrollUpArrow.containsPoint(x, y))
            //         this.scrollUpArrow.OnClick.Invoke();
            //     if (this.scrollDownArrow.containsPoint(x, y))
            //         this.scrollDownArrow.OnClick.Invoke();
            // }
            // }
        }

        private void ConsumeLeftClick()
        {
            // We're consuming a click, so we want to execute our clickAction action if we have one.

            if (this.clickAction != null)
                this.clickAction.Invoke();
        }
    }
}
