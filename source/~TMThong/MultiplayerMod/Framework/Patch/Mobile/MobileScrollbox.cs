/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewModdingAPI;
using System.Diagnostics;

namespace MultiplayerMod.Framework.Patch.Mobile
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class MobileScrollbox
    {
        public IReflectedMethod updateMethod { get; }
        public IReflectedMethod setYOffsetForScrollMethod { get; }
        public IReflectedMethod setUpForScrollBoxDrawingMethod { get; }
        public IReflectedMethod finishScrollBoxDrawingMethod { get; }
        public IReflectedMethod setMaxYOffsetMethod { get; }
        public IReflectedMethod receiveLeftClickMethod { get; }
        public IReflectedMethod releaseLeftClickMethod { get; }
        public IReflectedMethod leftClickHeldMethod { get; }
        public IReflectedMethod receiveScrollWheelActionMethod { get; }
        public IReflectedMethod getYOffsetForScrollMethod { get; }
        public IReflectedField<bool> havePanelScrolledField { get; }
        public object _value { get; }
        public MobileScrollbox(int boxX, int boxY, int boxWidth, int boxHeight, int boxContentHeight, Rectangle clipRect, MobileScrollbar scrollBar = null)
        {
            _value = typeof(IClickableMenu).Assembly.GetType("StardewValley.Menus.MobileScrollbox").CreateInstance<object>(new object[] { boxX, boxY, boxWidth, boxHeight, boxContentHeight, clipRect, scrollBar._value });
            updateMethod = ModUtilities.Helper.Reflection.GetMethod(_value, "update");
            setYOffsetForScrollMethod = ModUtilities.Helper.Reflection.GetMethod(_value, "setYOffsetForScroll");
            setUpForScrollBoxDrawingMethod = ModUtilities.Helper.Reflection.GetMethod(_value, "setUpForScrollBoxDrawing");
            finishScrollBoxDrawingMethod = ModUtilities.Helper.Reflection.GetMethod(_value, "finishScrollBoxDrawing");
            setMaxYOffsetMethod = ModUtilities.Helper.Reflection.GetMethod(_value, "setMaxYOffset");
            receiveLeftClickMethod = ModUtilities.Helper.Reflection.GetMethod(_value, "receiveLeftClick");
            releaseLeftClickMethod = ModUtilities.Helper.Reflection.GetMethod(_value, "releaseLeftClick");
            leftClickHeldMethod = ModUtilities.Helper.Reflection.GetMethod(_value, "leftClickHeld");
            receiveScrollWheelActionMethod = ModUtilities.Helper.Reflection.GetMethod(_value, "receiveScrollWheelAction");
            getYOffsetForScrollMethod = ModUtilities.Helper.Reflection.GetMethod(_value, "getYOffsetForScroll");
            havePanelScrolledField = ModUtilities.Helper.Reflection.GetField<bool>(_value, "havePanelScrolled");
        }
        public void update(GameTime time)
        {
            updateMethod.Invoke(time);
        }
        public void setYOffsetForScroll(int offset)
        {
            setYOffsetForScrollMethod.Invoke(offset);
        }
        public void setUpForScrollBoxDrawing(SpriteBatch b, float scale = 1f)
        {
            setUpForScrollBoxDrawingMethod.Invoke(b, scale);
        }
        public void finishScrollBoxDrawing(SpriteBatch b, float scale = 1f)
        {
            finishScrollBoxDrawingMethod.Invoke(b, scale);
        }
        public void setMaxYOffset(int offset)
        {
            setMaxYOffsetMethod.Invoke(offset);
        }
        public void receiveLeftClick(int x, int y)
        {
            receiveLeftClickMethod.Invoke(x, y);
        }
        public void releaseLeftClick(int x, int y)
        {
            releaseLeftClickMethod.Invoke(x, y);
        }
        public void leftClickHeld(int x, int y)
        {
            leftClickHeldMethod.Invoke(x, y);
        }
        public void receiveScrollWheelAction(int direction)
        {
            receiveScrollWheelActionMethod.Invoke(direction);
        }
        public int getYOffsetForScroll()
        {
            return getYOffsetForScrollMethod.Invoke<int>();
        }
        public bool havePanelScrolled
        {
            get
            {
                return havePanelScrolledField.GetValue();
            }
            set
            {
                havePanelScrolledField.SetValue(value);
            }
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}