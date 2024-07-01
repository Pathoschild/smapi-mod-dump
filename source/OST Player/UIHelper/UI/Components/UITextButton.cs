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
    internal class UITextButton : UIComponent
    {
        internal override string Id { get => base.Id; set => base.Id = null; }
        internal override Action<string, object> ValueChangedAction { get => null; set => base.ValueChangedAction = null; }
        private string text;
        internal Action action = null;

        internal UITextButton(Rectangle initialBounds, string text, EAlign align = EAlign.Center, Color? textColor = null) : base(initialBounds, null, align, textColor)
        {
            this.text = text;
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(mouseCursors, new Rectangle(Bounds.X + 20, Bounds.Y, Bounds.Width - 40, Bounds.Height), new(IsClicked? 149: 163, 338,4,9) , Color.BurlyWood);
            
            b.Draw(mouseCursors, new Rectangle(Bounds.X, Bounds.Y, 20, Bounds.Height), new(IsClicked? 145: 159, 338,3,9), Color.BurlyWood);
            b.Draw(mouseCursors, new Rectangle(Bounds.Right - 20, Bounds.Y, 20, Bounds.Height), new(IsClicked? 155: 169, 338,3,9), Color.BurlyWood);

            b.DrawString(Game1.smallFont, text, UIUtils.getCenteredText(Bounds, text, Game1.smallFont), configColor(textColor));
        }

        internal override void receiveLeftClick(int x, int y)
        {
            Game1.playSound("drumkit6");
        }

        internal override void releaseLeftClick(int x, int y)
        {
            if(containsPoint(x,y))
                action?.Invoke();
        }

        public override bool containsPoint(int x, int y)
        {
            return Bounds.Contains(x, y);
        }

        internal override void InvokeValueChange()
        {
            
        }

        protected override Color configColor(Color color, float hlFactor = 0.6F, float clckFactor = 0.8F)
        {
            return base.configColor(color, hlFactor, clckFactor);
        }
    }
}
