/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.NewMachinesMod.Menu
{
    public sealed class MachineInfoMenu : GridToolTip
    {
        public override bool NeedDraw()
        {
            if (Game1.activeClickableMenu != null || !IsAltPressed()) return false;
            var o = GetHoveredObject();
            return o != null && o.bigCraftable && o.ParentSheetIndex != (int)CraftableID.Chest;
        }

        public override void Draw()
        {
            Clear();
            var o = GetHoveredObject();

            var row = 0;
            RegisterCell(new TextMenuCell(row, 0, Aligment.Center, o.Name) {ColumnSpan = 2});
            ++row;

            if (o.minutesUntilReady > 0)
            {
                RegisterCell(new IconMenuCell(row, 0, Aligment.Center, Game1.mouseCursors, new Rectangle(434, 475, 9, 9), Game1.tileSize / 2));
                RegisterCell(new TextMenuCell(row, 1, Aligment.VerticalCenter, GetTimeLeftText(o.minutesUntilReady)));
                ++row;
            }

            if (o.heldObject != null)
            {
                RegisterCell(new ItemMenuCell(row, 0, Aligment.Center, o.heldObject));
                RegisterCell(new TextMenuCell(row, 1, Aligment.VerticalCenter, o.heldObject.Name));
                ++row;

                if (o.heldObject.sellToStorePrice() > 0)
                {
                    RegisterCell(new IconMenuCell(row, 0, Aligment.Center, Game1.mouseCursors, new Rectangle(193, 373, 9, 9), Game1.tileSize / 2));
                    RegisterCell(new TextMenuCell(row, 1, Aligment.VerticalCenter, GetHeldPrice(o.heldObject)));
                    ++row;
                }
            }

            Recalculate();

            var x0 = Game1.getMouseX() - Game1.tileSize;
            var y0 = Game1.getMouseY() + Game1.tileSize;
            Draw(x0, y0);
        }

        private static bool IsAltPressed()
        {
            var state = Keyboard.GetState();
            return state.IsKeyDown(Keys.LeftAlt) || state.IsKeyDown(Keys.RightAlt);
        }

        private static Object GetHoveredObject()
        {
            return Game1.currentLocation.getObjectAt(NormalizedMouseX, NormalizedMouseY);
        }

        private static string GetTimeLeftText(int minutesLeft)
        {
            var time = new TimeSpan(0, minutesLeft, 0);
            if (time.Days > 0) return $"{time.Days}d {time.Hours:D2}h {time.Minutes:D2}m";
            if (time.Hours > 0) return $"{time.Hours}h {time.Minutes:D2}m";
            return $"{time.Minutes}m";
        }

        private static string GetHeldPrice(Object heldObject)
        {
            var price = heldObject.sellToStorePrice();
            var stack = heldObject.Stack;
            if (stack == 1) return $"{price}g";
            return $"{price * stack}g ({price}g x{stack})";
        }
    }
}
