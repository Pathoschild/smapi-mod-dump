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
    internal class UISeparator : UIComponent
    {
        internal override string Id { get => base.Id; set => base.Id = null; }
        private Color color;
        internal override Action<string, object> ValueChangedAction { get => null; set => base.ValueChangedAction = null; }


        internal UISeparator(Rectangle initialBounds, Color? color) : base(initialBounds)
        {
            this.color = color ?? Color.BurlyWood;
        }

        public override void draw(SpriteBatch b)
        
        {
            b.Draw(Game1.fadeToBlackRect, Bounds, color);
        }

        internal override void InvokeValueChange()
        {
            
        }
    }
}
