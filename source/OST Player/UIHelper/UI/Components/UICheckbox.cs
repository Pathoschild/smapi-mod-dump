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
    internal class UICheckbox: UIComponent
    {
        private readonly static Rectangle checkedSource = new(236, 425, 9, 9);
        private readonly static Rectangle uncheckedSource = new(227, 425, 9, 9);
        private Rectangle chbxRect;
        private Rectangle descRect;
        private string description;

        private bool IsChecked {
            get{return (bool)Value;}
            set{Value = value;}
        }

        internal UICheckbox(Rectangle initialBounds, bool isChecked, Color? textColor, string description, EAlign align): base(initialBounds, isChecked, align, textColor){
            this.description = description;
        }

        public override void draw(SpriteBatch b)
        {

            b.Draw(mouseCursors, GetCheckboxRect(), IsChecked? checkedSource: uncheckedSource, configColor(Color.Wheat));
            b.DrawString(Game1.smallFont, description, UIUtils.getCenteredText(GetDescriptionRect(), UIUtils.FitTextInArea(description, descRect.Width, Game1.smallFont), Game1.smallFont), textColor);
        }

        public override bool containsPoint(int x, int y)
        {
            return Bounds.Contains(x, y);
        }

        internal override void receiveLeftClick(int x, int y)
        {
            if(chbxRect.Contains(x,y)){
                Game1.playSound("drumkit6");
                IsChecked = !IsChecked;
                InvokeValueChange();
            }
        }

        private Rectangle GetCheckboxRect(){
            chbxRect = new(Bounds.X, Bounds.Center.Y - 18, 36, 36);
            return chbxRect;
        }
        private Rectangle GetDescriptionRect(){
            descRect = new(chbxRect.Right + 10, Bounds.Y, Bounds.Width - chbxRect.Width - 10, Bounds.Height);
            return descRect;
        }

        public override void tryHover(int x, int y, float maxScaleIncrease = 0.1F)
        {
            base.tryHover(x, y, maxScaleIncrease);
        }

        protected override Color configColor(Color color, float hlFactor = 1.2f, float clckFactor = 1.5f)
        {
            return base.configColor(color, hlFactor, clckFactor);
        }
    }
}
