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

namespace UIHelper
{
    internal class UISlider : UIComponent
    {
        internal override object Value { get => base.Value; set => base.Value = Math.Clamp((float)value, limitLeft, limitRight); }
        internal override object PrevValue { get => base.PrevValue; set => base.PrevValue = Math.Clamp((float)value, limitLeft, limitRight); }
        
        //////////////Logic Values////////////////
        private float limitLeft;
        private float limitRight;
        //////////////////////////////////////////


        //////////////Physic Values//////////////
        private int sliderLimitLeft;
        private int sliderLimitRight;
        private int SliderCurrentPos{
            get{return btnRect.X;}
            set{btnRect.X = value;}
        }
        /////////////////////////////////////////
        
        private double convFactor;
        private Rectangle valueRect;
        private Rectangle btnRect = Rectangle.Empty;
        private Rectangle lineRect = Rectangle.Empty;
        private readonly static Rectangle buttonSource = new(432, 439, 9, 9);
        private readonly static Vector2 maxValueMeasure = Game1.smallFont.MeasureString("9999.99");
        private bool IsFloat { get; set; }

        internal UISlider(Rectangle initialBounds, object value, bool isFloat, float limitLeft, float limitRight, Color? textColor, EAlign align) : base(initialBounds, value, align, textColor)
        {
            IsFloat = isFloat;
            this.limitLeft = limitLeft;
            this.limitRight = limitRight;
            sliderLimitLeft = initialBounds.Left;
            sliderLimitRight = initialBounds.Right - (int)maxValueMeasure.X - 15 - 24;
            SliderCurrentPos = sliderLimitLeft;
            convFactor = (limitRight - limitLeft) / (double)(sliderLimitRight - sliderLimitLeft);
        }

        internal override void readjustComponentLogic()
        {
            sliderLimitLeft = XPos;
            sliderLimitRight = XPos + width - (int)maxValueMeasure.X - 15 - 24;
        }

        internal override void receiveLeftClick(int x, int y)
        {
            IsDragging = true;
            if(lineRect.Contains(x,y) && !btnRect.Contains(x, y)){
                
                GetValueFromSliderPos(x);
            }
        }

        internal override void leftClickHeld(int x, int y)
        {
            if(!IsDragging)
                return;

            GetValueFromSliderPos(x);
        }

        internal override void releaseLeftClick(int x, int y)
        {
            if(IsDragging)
                InvokeValueChange();
        }

        public override bool containsPoint(int x, int y)
        {
            return lineRect.Contains(x,y) || btnRect.Contains(x,y);
        }

        protected override Color configColor(Color color, float hlFactor = 0.7F, float clckFactor = 1.2F)
        {
            return base.configColor(color, hlFactor, clckFactor);
        }

        public override void draw(SpriteBatch b)
        {
            SetLineRectangle();
            b.Draw(Game1.fadeToBlackRect, lineRect, configColor(Color.SaddleBrown));

            string valueStr = ((float)Value).ToString(IsFloat?"F2":"0");
            string limitLeftStr = limitLeft.ToString(IsFloat?"F2":"0");
            string limitRightStr = limitRight.ToString(IsFloat?"F2":"0");

            b.DrawString(Game1.smallFont, valueStr, UIUtils.getCenteredText(GetValueRectangle(), valueStr, Game1.smallFont), textColor);
            b.DrawString(Game1.smallFont, limitLeftStr, UIUtils.getCenteredText(GetLimitLeftRectangle(), limitLeftStr, Game1.smallFont), textColor);
            b.DrawString(Game1.smallFont, limitRightStr, UIUtils.getCenteredText(GetLimitRightRectangle(), limitRightStr, Game1.smallFont), textColor);

            GetSliderPosFromValue();
            b.Draw(mouseCursors, GetButtonRectangle(), buttonSource, configColor(Color.Wheat));
        }
        private void SetLineRectangle()
        {
            lineRect = new(Bounds.X + 12, Bounds.Y + 8, Bounds.Width - 24 - 15 - (int)maxValueMeasure.X, 8);
        }
        private Rectangle GetButtonRectangle()
        {
            btnRect = new(SliderCurrentPos, Bounds.Y, 24, 24);
            return btnRect;
        }

        private Rectangle GetValueRectangle()
        {
            valueRect = new(Bounds.Right - (int)maxValueMeasure.X, Bounds.Y, (int)maxValueMeasure.X, Bounds.Height - (int)maxValueMeasure.Y - 10);
            return valueRect;
        }
        private Rectangle GetLimitLeftRectangle()
        {
            return new(Bounds.X, Bounds.Bottom - (int)maxValueMeasure.Y, (int)maxValueMeasure.X, (int)maxValueMeasure.Y);
        }
        private Rectangle GetLimitRightRectangle()
        {
            return new(valueRect.Left - 15 - (int)maxValueMeasure.X, Bounds.Bottom - (int)maxValueMeasure.Y, (int)maxValueMeasure.X, (int)maxValueMeasure.Y);
        }

        private void GetSliderPosFromValue()
        {
            SliderCurrentPos = Math.Clamp((int)(((float)Value - limitLeft) / convFactor + sliderLimitLeft), sliderLimitLeft, sliderLimitRight);
        }

        private void GetValueFromSliderPos(int x)
        {
            SliderCurrentPos = Math.Clamp(x-12, sliderLimitLeft, sliderLimitRight);
            Value = Math.Clamp((float)((SliderCurrentPos - sliderLimitLeft) * convFactor + limitLeft), limitLeft, limitRight);
        }
    }
}
