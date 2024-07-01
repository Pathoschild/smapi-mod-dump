/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ProfeJavix/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace UIHelper
{
    internal class UIPage
    {
        internal Rectangle bgRect;
        private Rectangle titleRect = Rectangle.Empty;
        private Color titleColor, titleBorderColor, titleTextColor;
        private string title = null;
        internal bool TitleSet { get; private set; } = false;
        private Texture2D tabIcon;
        private Rectangle? tabSrcRect;
        internal bool TabActive{get; set;} = false;
        internal Rectangle tabRect = Rectangle.Empty;
        private static Rectangle tabSource = new(16, 368, 16, 16);
        private HashSet<UIComponent> components = new HashSet<UIComponent>();
        private ScrollBar scrollBar;

        ////// PHYSIC MEASURES ///////
        private int startElementPos;
        private int endElementPos;
        private int currentElementPos;
        //////////////////////////////

        internal int elementSpacing;

        internal bool HasScrollBar { get; private set; }
        private bool drawTitleBox;

        internal int lastElementPos; ///// LOGIC MEASURE

        private int alignLeft, alignRight;
        internal const int btnSpace = 20 + 50;

        internal UIPage(int elementSpacing, Rectangle bgRect, Texture2D tabIcon, bool hasScroll)
        {
            this.elementSpacing = elementSpacing;
            this.bgRect = bgRect;

            if(this.bgRect.Height < elementSpacing)
                throw new Exception("Element spacing exceeds the page limit. You should use a scrollbar.");

            lastElementPos = elementSpacing;
            HasScrollBar = hasScroll;
            SetScrollBar();
            
            this.tabIcon = tabIcon;
            InitElementPosLimits();
            SetAlignmentValues();
        }

        internal void ReadjustPage(Rectangle bgRect)
        {
            this.bgRect = bgRect;

            if (TitleSet)
                SetTitle(title, titleTextColor, drawTitleBox, titleColor, titleBorderColor);

            if (HasScrollBar)
                scrollBar.Readjust(GetScrollRect());

            SetAlignmentValues();
            InitElementPosLimits();

            foreach (UIComponent c in components)
            {
                int wdth = FitComponentWidth(c.width);
                int x = (c.Align == EAlign.None)? GetXClamped(c.XPos, wdth): GetXAligned(wdth, c.Align);
                c.XPos = x;
                c.width = wdth;
                c.readjustComponentLogic();
            }
        }

        internal void ReadjustBtns(List<BGButton> btns){
            int xPos = bgRect.Right - 30 - 64;
            btns.ForEach(b =>{
                if(b != null){
                    b.Bounds = new(xPos, bgRect.Bottom - 20 - 64, 64, 64);
                    xPos -= 64 + 30;
                }
            });
        }

        private void SetAlignmentValues()
        {
            alignLeft = HasScrollBar ? scrollBar.scrollArea.Left + 30 : bgRect.Left + 30;
            alignRight = HasScrollBar ? scrollBar.scrollArea.Right - 30 : bgRect.Right - 30;
        }

        private void InitElementPosLimits()
        {

            startElementPos = (HasScrollBar ? scrollBar.scrollArea.Top : bgRect.Top) + elementSpacing;
            endElementPos = (HasScrollBar ? scrollBar.scrollArea.Bottom : bgRect.Bottom) - elementSpacing - btnSpace;
        }

        internal void SetTitle(string title, Color? textColor, bool drawBox, Color? color, Color? borderColor)
        {
            this.title = title;
            if (title != null)
            {
                TitleSet = true;
                drawTitleBox = drawBox;
                Vector2 textSize = Game1.dialogueFont.MeasureString(title) + new Vector2(10, 10);
                Vector2 titleCentered = Utility.getTopLeftPositionForCenteringOnScreen((int)textSize.X, (int)textSize.Y);
                titleRect = new((int)titleCentered.X, bgRect.Top - 16 - 48 - (int)textSize.Y, (int)textSize.X, (int)textSize.Y);

                if (drawBox)
                {
                    titleColor = color ?? Color.Wheat;
                    titleBorderColor = borderColor ?? Color.BurlyWood;
                }

                titleTextColor = textColor ?? Color.Black;
            }
            else
                Console.Log("The title cannot be null.");
        }

        internal void SetTabIcon(Texture2D icon, Rectangle? sourceRect)
        {
            tabIcon = icon;
            tabSrcRect = sourceRect;
        }

        private void SetScrollBar()
        {

            if (HasScrollBar)
            {
                
                scrollBar = new ScrollBar(GetScrollRect());
                HasScrollBar = true;
            }
        }

        private Rectangle GetScrollRect()
        {
            int x = bgRect.X + 30 + 16;
            int y = bgRect.Y + 20 + 16;
            int wdth = bgRect.Width - 60 - 32;
            int hght = bgRect.Height - btnSpace - 40 - 32;

            return new(x, y, wdth, hght);
        }

        internal void ConfigScroll(bool scrollOwnBG, bool showScrollBtns, Color? scrollColor = null, Color? scrollBorderColor = null)
        {
            if (HasScrollBar)
            {
                scrollBar.HasOwnBG = scrollOwnBG;
                scrollBar.ShowButtons = showScrollBtns;
                scrollBar.SetColors(scrollColor, scrollBorderColor);
            }
        }

        private bool CanAddHeight(int height, bool accountElementSpace = true)
        {
            if (!HasScrollBar && startElementPos + lastElementPos + height + (accountElementSpace ? elementSpacing : 0) > endElementPos)
            {
                Console.Log("Cannot add more elements on the background. Consider using a ScrollBar");
                return false;
            }
            return true;
        }

        private void UpdateLimitPos(int height)
        {

            lastElementPos += height + elementSpacing;
            if (HasScrollBar)
            {
                int diff = lastElementPos - scrollBar.scrollArea.Height;
                scrollBar.ScrollLimit = diff > 0 ? diff : 0;
            }

        }

        internal void AddBlankSpace(int height)
        {

            AddSeparator(Color.Transparent, height, false);
        }

        internal void AddSeparator(Color? color, int height = 8, bool accountElementSpace = true)
        {
            if (!CanAddHeight(height, accountElementSpace))
                return;

            int x = HasScrollBar ? scrollBar.scrollArea.Width : bgRect.Width;
            int width = HasScrollBar ? scrollBar.scrollArea.Width : bgRect.Width;

            components.Add(new UISeparator(new(x, lastElementPos, width, height), color));
            UpdateLimitPos(8);

        }

        internal void AddComponent(UIComponent component, string id)
        {

            if (!CanAddHeight(component.height))
                return;

            component.Id = id;
            UpdateLimitPos(component.height);
            components.Add(component);
        }

        internal List<string> GetAllComponentIds(){
            List<string> ids = new();
            foreach(UIComponent c in components){
                ids.Add(c.Id);
            }
            return ids;
        }

        internal List<UIComponent> GetInteractiveComponents(){
            return components.ToList().Where(c => c.Id != null).ToList();
        }

        internal void drawTab(SpriteBatch b)
        {

            b.Draw(Game1.mouseCursors, tabRect, tabSource, Color.White);
            if (tabIcon != null)
                b.Draw(tabIcon, new Rectangle(tabRect.X + 12, tabRect.Y + 16, 40, 44), tabSrcRect, Color.White);

        }

        internal void DrawTitle(SpriteBatch b)
        {
            if (TitleSet)
            {

                if (drawTitleBox)
                {
                    UIUtils.DrawBox(b, titleRect, titleColor, 255, 0, true);
                    UIUtils.DrawBox(b, titleRect, titleBorderColor, 0, 255, true);
                }

                b.DrawString(Game1.dialogueFont, title, UIUtils.getCenteredText(titleRect, title, Game1.dialogueFont), titleTextColor);
            }
        }

        internal void DrawComponents()
        {
            DrawScrollBar();
            using (SpriteBatch b = new SpriteBatch(Game1.graphics.GraphicsDevice))
            {

                Rectangle defaultScissor = Game1.graphics.GraphicsDevice.ScissorRectangle;
                if (HasScrollBar)
                    Game1.graphics.GraphicsDevice.ScissorRectangle = scrollBar.scrollArea;

                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, HasScrollBar ? new RasterizerState { ScissorTestEnable = true } : null);

                currentElementPos = startElementPos;
                DeleteComponentsBounds();
                bool firstAdded = false;
                foreach (UIComponent c in components)
                {
                    int visibleHght = c.height;
                    if (HasScrollBar)
                    {
                        int startPos = c.LogicYPos;
                        int endPos = startPos + c.height;

                        if (endPos < scrollBar.scrollPos)
                            continue;
                        if (currentElementPos > scrollBar.scrollArea.Bottom)
                            break;

                        if (!firstAdded)
                        {
                            int diff = startPos - scrollBar.scrollPos;
                            if (diff >= 0)
                            {
                                currentElementPos = scrollBar.scrollArea.Top + diff;
                            }
                            else
                            {
                                currentElementPos -= Math.Abs(diff) + elementSpacing;
                            }
                            firstAdded = true;
                        }

                    }
                    c.Bounds = new(c.XPos, currentElementPos, c.width, c.height);
                    c.draw(b);

                    currentElementPos += c.height + elementSpacing;
                }

                b.End();
                if (HasScrollBar)
                    Game1.graphics.GraphicsDevice.ScissorRectangle = defaultScissor;
            }
        }

        private void DrawScrollBar()
        {
            if (HasScrollBar)
                using (SpriteBatch b = new SpriteBatch(Game1.graphics.GraphicsDevice))
                {
                    b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

                    scrollBar.draw(b);

                    b.End();
                }
        }

        private void DeleteComponentsBounds()
        {
            foreach (UIComponent c in components)
            {
                c.Bounds = Rectangle.Empty;
            }
        }

        internal void receiveKeyPress(Keys key)
        {
            UIComponent c = GetFocusComponent();
            if (c != null)
            {
                c.receiveKeyPress(key);
            }
        }

        internal void receiveGamePadButton(Buttons b)
        {
            //TODO
        }

        internal void performHoverAction(int x, int y)
        {
            bool contained = !HasScrollBar || scrollBar.scrollArea.Contains(x, y);

            foreach (UIComponent c in components)
            {
                c.tryHover(x, y, contained);
            }
        }

        internal void receiveLeftClick(int x, int y)
        {

            if (IsTabPointed(x, y))
            {
                return;
            }

            if (HasScrollBar && scrollBar.InScrollBarArea(x, y))
            {
                scrollBar.receiveLeftClick(x, y);
                return;
            }

            UnfocusElements();

            if(HasScrollBar && !scrollBar.scrollArea.Contains(x, y))
                return;
            
            UIComponent component = GetPointedComponent(x, y);
            if (component != null)
            {
                component.IsClicked = true;
                component.receiveLeftClick(x, y);
            }

        }

        internal void receiveRightClick(int x, int y){

            if (HasScrollBar)
                scrollBar.IsScrolling = false;
            foreach (UIComponent c in components)
            {
                c.IsDragging = false;
                c.IsClicked = false;
                c.receiveRightClick(x, y);
            }
        }

        internal void leftClickHeld(int x, int y)
        {
            if (HasScrollBar && scrollBar.IsScrolling)
            {
                scrollBar.leftClickHeld(x, y);
                return;
            }

            if(HasScrollBar && !scrollBar.scrollArea.Contains(x, y))
                return;

            UIComponent component = GetDraggedComponent();
            if (component != null)
                component.leftClickHeld(x, y);
        }

        internal void releaseLeftClick(int x, int y)
        {
            if (HasScrollBar)
                scrollBar.IsScrolling = false;
            foreach (UIComponent c in components)
            {
                c.IsClicked = false;
                c.releaseLeftClick(x, y);
                c.IsDragging = false;
                
            }

        }

        internal void receiveScrollWheelAction(int direction)
        {
            if (!HasScrollBar || scrollBar.IsScrolling || GetDraggedComponent() != null)
                return;

            scrollBar.receiveScrollWheelAction(direction);
        }

        private UIComponent GetPointedComponent(int x, int y)
        {
            return components.FirstOrDefault(c => c.containsPoint(x, y));
        }

        private UIComponent GetDraggedComponent()
        {
            return components.FirstOrDefault(c => c.IsDragging);
        }
        internal UIComponent GetFocusComponent()
        {
            return components.FirstOrDefault(c => c.IsFocus);
        }

        internal void UnfocusElements()
        {
            foreach (UIComponent c in components)
            {
                c.IsFocus = false;
            }
        }

        internal bool IsTabPointed(int x, int y)
        {
            return tabRect.Contains(x, y);
        }

        internal bool IsTitlePointed(int x, int y)
        {
            return TitleSet && titleRect.Contains(x, y);
        }

        internal int FitComponentWidth(int width)
        {
            return Math.Min(width, HasScrollBar ? scrollBar.scrollArea.Width : bgRect.Width);
        }

        internal int GetXAligned(int width, EAlign align)
        {

            int limit, diff;
            if(align == EAlign.None)
                align = EAlign.Center;

            switch (align)
            {
                case EAlign.Left:
                    limit = HasScrollBar ? scrollBar.scrollArea.Right : bgRect.Right;
                    diff = alignLeft + width - limit;
                    return alignLeft - (diff >= 0 ? diff : 0);
                case EAlign.Center:
                    limit = HasScrollBar? scrollBar.scrollArea.X: bgRect.X;
                    diff = (HasScrollBar? scrollBar.scrollArea.Width: bgRect.Width) - width;
                    return limit + diff / 2;
                case EAlign.Right:
                    limit = HasScrollBar ? scrollBar.scrollArea.Left : bgRect.Left;
                    int startX = alignRight - width;
                    diff = limit - startX;
                    return startX + (diff > 0 ? diff : 0);
                default:
                    Console.Log("Something went wrong aligning width.");
                    return -1;
            }
        }

        internal int GetXClamped(int x, int width){
            int limitLeft = HasScrollBar? scrollBar.scrollArea.Left: bgRect.Left;
            int limitRight = HasScrollBar? scrollBar.scrollArea.Right: bgRect.Right;

            x = Math.Clamp(x, limitLeft, limitRight);
            int diff = (x + width) - limitRight;

            return x - (diff > 0? diff: 0);
        }

    }
}
