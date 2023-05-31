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
using static HarmonyLib.Code;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace MultiplayerMod.Framework.Patch.Mobile
{
    public class MobileScrollbar
    {
        public IReflectedMethod sliderRunnerContainsMethod { get; }
        public IReflectedMethod sliderContainsMethod { get; }
        public IReflectedMethod setYMethod { get; }
        public IReflectedMethod setPercentageMethod { get; }
        public IReflectedMethod drawMethod { get; }
        public object _value { get; }
        public MobileScrollbar(int x, int y, int width, int height, int additionalWidthLeft = 0, int additionalWidthRight = 0, bool showArrows = false)
        {
            _value = typeof(IClickableMenu).Assembly.GetType("StardewValley.Menus.MobileScrollbar").CreateInstance<object>(new object[] { x, y, width, height, additionalWidthLeft, additionalWidthRight, showArrows }, new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(bool) });
            sliderRunnerContainsMethod = ModUtilities.Helper.Reflection.GetMethod(_value, "sliderRunnerContains");
            sliderContainsMethod = ModUtilities.Helper.Reflection.GetMethod(_value, "sliderContains");
            setYMethod = ModUtilities.Helper.Reflection.GetMethod(_value, "setY");
            setPercentageMethod = ModUtilities.Helper.Reflection.GetMethod(_value, "setPercentage");
            drawMethod = ModUtilities.Helper.Reflection.GetMethod(_value, "draw");
        }
        public bool sliderRunnerContains(int x, int y)
        {
            return sliderRunnerContainsMethod.Invoke<bool>(x, y);
        }
        public bool sliderContains(int x, int y)
        {
            return sliderContainsMethod.Invoke<bool>(x, y);
        }
        public float setY(int newY)
        {
            return setYMethod.Invoke<float>(newY);
        }
        public void setPercentage(float newPercent)
        {
            setPercentageMethod.Invoke(newPercent);
        }
        public void draw(SpriteBatch b)
        {
            drawMethod.Invoke(b);
        }
    }
}
