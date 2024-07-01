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
    internal class UILabel : UIComponent
    {
        internal override string Id { get => base.Id; set => base.Id = null; }
        protected bool hasBox;
        private string text;
        private Color innerColor;
        protected Color borderColor;
        internal override Action<string, object> ValueChangedAction { get => null; set => base.ValueChangedAction = null; }

        internal UILabel(string text, Rectangle initialBounds, bool hasBox, Color? textColor, Color? innerColor, Color? borderColor, EAlign align) : base(initialBounds, null, align, textColor)
        {
            this.text = text ?? "";
            this.hasBox = hasBox;
            if(hasBox){
                this.innerColor = innerColor ?? Color.Wheat;
                this.borderColor = borderColor ?? Color.BurlyWood;
            }
        }

        public override void draw(SpriteBatch b)
        {
            if(hasBox){
                UIUtils.DrawBox(b, Bounds, innerColor, 255, 0);
                UIUtils.DrawBox(b, Bounds, borderColor, 0);
            }
            b.DrawString(Game1.smallFont, text, UIUtils.getCenteredText(GetBoundsWithBorder(hasBox), text, Game1.smallFont), textColor);
        }

        internal override void InvokeValueChange()
        {
            
        }
    }
}
