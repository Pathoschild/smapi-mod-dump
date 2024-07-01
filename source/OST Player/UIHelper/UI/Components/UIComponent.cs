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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace UIHelper
{
    internal abstract class UIComponent : ClickableTextureComponent
    {

        protected readonly static Texture2D mouseCursors = Game1.mouseCursors;
        internal virtual string Id { get; set; }

        protected object val;
        internal virtual object Value {
            get{return val;} 
            set{val = value;}
        }

        protected object prevVal;
        internal virtual object PrevValue{
            get{return prevVal;} 
            set{prevVal = value;}
        }
        internal virtual Rectangle Bounds{
            get{return bounds;}
            set{bounds = value;}
        }
        internal virtual EAlign Align{
            get{return align;} 
            set{
                if(value == EAlign.None)
                    align = EAlign.Center;
                else
                    align = value;
            }
        }
        protected EAlign align;
        protected Color textColor;

        internal int XPos{get; set;}
        internal int LogicYPos {get; set;} = 0;
        internal bool IsHighlighted { get; set; } = false;
        internal bool IsClicked { get; set; } = false;
        internal bool IsDragging {get; set;} = false;
        protected bool focused = false;
        internal virtual bool IsFocus {
            get{return focused;}
            set{focused = value;}
        }

        internal int width, height;

        protected Action<string, object> valueChangedAction = null;
        internal virtual Action<string, object> ValueChangedAction{
            get{return valueChangedAction;}
            set{valueChangedAction += value;}
        }

        protected UIComponent(Rectangle initialBounds, object value = null, EAlign align = EAlign.Center, Color? textColor = null) : base(Rectangle.Empty, null, new(), 1f)
        {
            XPos = initialBounds.X;
            LogicYPos = initialBounds.Y;
            width = initialBounds.Width;
            height = initialBounds.Height;
            Value = value;
            PrevValue = value;
            Align = align;
            this.textColor = textColor ?? Color.Black;
        }

        public override void draw(SpriteBatch b){

        }

        public override bool containsPoint(int x, int y)
        {
            return false;
        }

        internal virtual void tryHover(int x, int y, bool scrollContained){
            IsHighlighted = containsPoint(x, y) && scrollContained;
        }

        internal virtual void receiveKeyPress(Keys key){
            
        }

        internal virtual void receiveLeftClick(int x, int y){

        }

        internal virtual void leftClickHeld(int x, int y){

        }

        internal virtual void releaseLeftClick(int x, int y){
            
        }

        internal virtual void receiveScrollWheelAction(int direction){
            
        }

        internal virtual void receiveRightClick(int x, int y){
            
        }

        internal virtual void readjustComponentLogic(){
            
        }

        internal virtual void InvokeValueChange(){
            valueChangedAction?.Invoke(Id, Value);
        }

        protected virtual Color configColor(Color color, float hlFactor = 0.8f, float clckFactor = 1.3f)
        {
            return IsHighlighted? UIUtils.getHighLightColor(color, IsClicked? clckFactor: hlFactor): color;
        }
        protected virtual Rectangle GetBoundsWithBorder(bool withBorder){
            return withBorder? new(Bounds.X+16, Bounds.Y+16, Bounds.Width-32, Bounds.Height-32): Bounds;
        }
    }
}
