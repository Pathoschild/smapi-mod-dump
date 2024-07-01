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
    internal abstract class BGButton
    {
        internal Rectangle Bounds{get; set;}
        internal bool Highlighted{get; set;} = false;
        internal bool Clicked{get; set;} = false;

        protected Rectangle btnSrc = Rectangle.Empty;
        protected readonly Texture2D icon = Game1.mouseCursors;

        protected Action action;


        internal BGButton(Rectangle bounds, Action closeAction, Action retAction)
        {
            Bounds = bounds;
            action += closeAction;
            action += retAction;
        }

        internal void draw(SpriteBatch b){

            b.Draw(icon, Bounds, btnSrc, ConfigColor(Color.Wheat));
        }

        private Color ConfigColor(Color color, float hlFactor = 0.8F, float clckFactor = 1.3F)
        {
            return Highlighted? UIUtils.getHighLightColor(color, Clicked? clckFactor: hlFactor): color;
        }

        internal void performLeftClick(){

            action?.Invoke();
        }

        internal bool containsPoint(int x, int y){
            return Bounds.Contains(x, y);
        }
        internal void Unclick(){
            Clicked = false;
        }

        internal void performHoverAction(int x, int y)
        {
            Highlighted = containsPoint(x, y);
        }
    }
}
