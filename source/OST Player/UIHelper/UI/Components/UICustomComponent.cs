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
using StardewValley.Menus;

namespace UIHelper
{
    internal class UICustomComponent : UIComponent
    {
        internal override object Value {
            get{return base.Value;}
            set{
                base.Value = value;
                InvokeValueChange();
            }
        }
        internal override EAlign Align { get => align; set => align = value; }
        private ClickableTextureComponent customComponent;
        internal Action<int, int> leftClickAction, releaseLeftClickAction, leftClickHeldAction, rightClickAction;
        internal Action<int> scrollWheelAction;
        internal Action<Keys> keyPressAction;
        internal bool IsClickable{get; set;}
        internal override Rectangle Bounds { 
            get => customComponent.bounds; 
            set => customComponent.bounds = value; 
        }
        internal UICustomComponent(ClickableTextureComponent customComponent, object value, EAlign align) : base(customComponent.bounds, value, align)
        {
            this.customComponent = customComponent;
        }

        internal void SetLeftClickAction(Action<int, int> leftClickAction){
            this.leftClickAction = leftClickAction;
        }

        public override void draw(SpriteBatch b)
        {
            customComponent.draw(b);
        }

        internal override void tryHover(int x, int y, bool scrollContained)
        {
            if(scrollContained)
                customComponent.tryHover(x, y);
            else
                customComponent.tryHover(int.MaxValue, int.MaxValue);
            
        }

        public override bool containsPoint(int x, int y)
        {
            return customComponent.containsPoint(x, y);
        }

        internal override void receiveLeftClick(int x, int y)
        {
            leftClickAction?.Invoke(x, y);
        }

        internal override void releaseLeftClick(int x, int y)
        {
            releaseLeftClickAction?.Invoke(x, y);
        }

        internal override void leftClickHeld(int x, int y)
        {
            leftClickHeldAction?.Invoke(x, y);
        }

        internal override void receiveScrollWheelAction(int direction)
        {
            scrollWheelAction?.Invoke(direction);
        }

        internal override void receiveKeyPress(Keys key)
        {
            keyPressAction?.Invoke(key);
        }

        internal override void receiveRightClick(int x, int y)
        {
            rightClickAction?.Invoke(x, y);
        }

    }
}
