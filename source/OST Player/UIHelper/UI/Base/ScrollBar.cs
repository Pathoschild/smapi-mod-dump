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
using StardewValley;
using StardewValley.Menus;

namespace UIHelper
{
    internal class ScrollBar : ClickableComponent
    {

        internal bool HasOwnBG {get; set;}
        internal bool ShowButtons {get; set;}
        internal bool IsScrolling {get; set;} = false;
        internal int CurrentBarPos {
            get{return scrollBarPosArea.Y;} 
            set{scrollBarPosArea.Y = value;}
        }
        private const int scrollPwr = 40;
        internal Rectangle scrollArea;
        private Rectangle scrollBarArea;
        private Rectangle scrollBarPosArea;
        private Color bgColor, borderColor;
        internal int scrollPos = 0;
        private int scrollLimit = 0;
        internal int ScrollLimit{
            get{return scrollLimit;}
            set{
                scrollLimit = value;
            }
        }
        private int scrollBarStart, scrollBarEnd;
        private static Rectangle upBtnSource = new(76, 72, 40, 44);
        private static Rectangle downBtnSource = new(12, 76, 40, 44);
        private static Rectangle barSource = new(435, 463, 6, 10);
        private Rectangle upBtnArea, downBtnArea;


        internal ScrollBar(Rectangle bounds, bool ownBG = false, bool showBtns = true) : base(bounds, "scrollBar")
        {
            HasOwnBG = ownBG;
            ShowButtons = showBtns;
            SetScrollUI();
            SetColors();
        }

        internal void Readjust(Rectangle bounds){

            this.bounds = bounds;
            SetScrollUI();
        }

        internal void SetScrollUI(){

            scrollArea = new(bounds.X, bounds.Y, bounds.Width - 60, bounds.Height);
            scrollBarArea = new(scrollArea.Right + 16, bounds.Top + (ShowButtons? 48 + 16: 0), 44, bounds.Height - (ShowButtons? 96 + 32: 0));
            scrollBarPosArea = new(scrollBarArea.Left+7, scrollBarArea.Top, 30, 50);

            scrollBarStart = scrollBarArea.Top;
            scrollBarEnd = scrollBarArea.Bottom-50;
            CurrentBarPos = scrollBarStart;

            if(ShowButtons){
                upBtnArea = new(scrollBarArea.Left, bounds.Top, 44, 48);
                downBtnArea = new(scrollBarArea.Left, scrollBarArea.Bottom+16, 44, 48);
            }
        }

        internal void SetColors(Color? bgColor = null, Color? borderColor = null){
            if(HasOwnBG){
                this.bgColor = bgColor ?? Color.Wheat;
                this.borderColor = borderColor ?? Color.BurlyWood;
            }
        }

        internal void receiveLeftClick(int x, int y){
            if(scrollBarArea.Contains(x,y)){
                IsScrolling = true;
                CurrentBarPos = Math.Clamp(y-scrollBarPosArea.Height/2, scrollBarStart, scrollBarEnd);
                getScrollPosProp(y);
                return;
            }

            ClickScrollBtn(x,y);
        }

        internal void leftClickHeld(int x, int y){

            if(IsScrolling){
                CurrentBarPos = Math.Clamp(y-scrollBarPosArea.Height/2, scrollBarStart, scrollBarEnd);
                getScrollPosProp(y);
                return;
            }
        }

        private void ClickScrollBtn(int x, int y){
            if(upBtnArea.Contains(x,y)){
                receiveScrollWheelAction(1);
                Game1.playSound("drumkit6");
            }
            
            if(downBtnArea.Contains(x,y)){
                receiveScrollWheelAction(-1);
                Game1.playSound("drumkit6");
            }
        }

        internal void receiveScrollWheelAction(int direction)
        {
            if(direction > 0)
                ScrollUp();
            else
                ScrollDown();
            getScrollBarPosProp();
        }

        private void ScrollUp(){
            scrollPos -= (scrollPos - scrollPwr >= 0)? scrollPwr : scrollPos;
        }

        private void ScrollDown(){
            scrollPos += (ScrollLimit - scrollPos >= scrollPwr)? scrollPwr : ScrollLimit - scrollPos;
        }

        internal void draw(SpriteBatch b)
        {
            if (HasOwnBG)
            {
                if (0 < ScrollLimit){
                    UIUtils.DrawBox(b, scrollBarArea, bgColor, 255, 0, true);
                    UIUtils.DrawBox(b, scrollBarArea, borderColor, 0, 255, true);
                }
                UIUtils.DrawBox(b, scrollArea, bgColor, 255, 0, true);
                UIUtils.DrawBox(b, scrollArea, borderColor, 0, 255, true);
            }

            if (0 < ScrollLimit)
            {
                b.Draw(Game1.mouseCursors, scrollBarPosArea, barSource, Color.White);

                if (ShowButtons)
                {
                    b.Draw(Game1.mouseCursors, upBtnArea, upBtnSource, Color.White);
                    b.Draw(Game1.mouseCursors, downBtnArea, downBtnSource, Color.White);
                }
            }
        }

        internal bool InScrollBarArea(int x, int y){
            return scrollBarArea.Contains(x,y) || upBtnArea.Contains(x,y) || downBtnArea.Contains(x, y);
        }

        private void getScrollBarPosProp()
        {
            CurrentBarPos = Math.Clamp((int)(scrollBarStart + (double)scrollPos / ScrollLimit * (scrollBarEnd - scrollBarStart)), scrollBarStart, scrollBarEnd);
        }

        private void getScrollPosProp(int y){
            CurrentBarPos = Math.Clamp(y-25, scrollBarStart, scrollBarEnd);
            scrollPos = Math.Clamp( (int)(0 + ((double)(CurrentBarPos - scrollBarStart)) / (scrollBarEnd - scrollBarStart) * (ScrollLimit - 0)), 0, ScrollLimit);
        }
    }
}
