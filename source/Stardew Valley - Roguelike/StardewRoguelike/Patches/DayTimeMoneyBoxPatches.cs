/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Reflection;
using System.Text;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(DayTimeMoneyBox), "draw")]
    internal class DayTimeMoneyBoxDrawPatch
    {
        public static bool Prefix(DayTimeMoneyBox __instance, SpriteBatch b)
        {
            MethodInfo updatePosition = __instance.GetType().GetMethod("updatePosition", BindingFlags.NonPublic | BindingFlags.Instance);
            updatePosition.Invoke(__instance, null);

            __instance.zoomInButton.draw(b, Color.White * ((Game1.options.desiredBaseZoomLevel >= 2f) ? 0.5f : 1f), 1f);
            __instance.zoomOutButton.draw(b, Color.White * ((Game1.options.desiredBaseZoomLevel <= 0.75f) ? 0.5f : 1f), 1f);

            __instance.drawMoneyBox(b, overrideY: 10);

            StringBuilder _hoverText = (StringBuilder)__instance.GetType().GetField("_hoverText", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            if (_hoverText.Length > 0 && __instance.isWithinBounds(Game1.getOldMouseX(), Game1.getOldMouseY()))
                IClickableMenu.drawHoverText(b, _hoverText, Game1.dialogueFont);

            return false;
        }
    }

    [HarmonyPatch(typeof(DayTimeMoneyBox), "updatePosition")]
    internal class DayTimeMoneyBoxUpdatePositionPatch
    {
        public static void Postfix(DayTimeMoneyBox __instance)
        {
            __instance.zoomInButton.bounds.X += 125;
            __instance.zoomOutButton.bounds.X += 125;

            __instance.zoomInButton.bounds.Y -= 170;
            __instance.zoomOutButton.bounds.Y -= 170;
        }
    }

    [HarmonyPatch(typeof(DayTimeMoneyBox), "drawMoneyBox")]
    internal class DayTimeMoneyBoxDrawBoxPatch
    {
        public static bool Prefix(DayTimeMoneyBox __instance, SpriteBatch b, int overrideX = -1, int overrideY = -1)
        {
            MethodInfo updatePosition = __instance.GetType().GetMethod("updatePosition", BindingFlags.NonPublic | BindingFlags.Instance);
            updatePosition.Invoke(__instance, null);

            b.Draw(Game1.mouseCursors, ((overrideY != -1) ? new Vector2((overrideX == -1) ? __instance.position.X : (overrideX), overrideY - 172) : __instance.position) + new Vector2(28 + ((__instance.moneyShakeTimer > 0) ? Game1.random.Next(-3, 4) : 0), 172 + ((__instance.moneyShakeTimer > 0) ? Game1.random.Next(-3, 4) : 0)), new Rectangle(340, 432, 65, 17), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
            __instance.moneyDial.draw(b, ((overrideY != -1) ? new Vector2((overrideX == -1) ? __instance.position.X : (overrideX), overrideY - 178) : __instance.position) + new Vector2(68 + ((__instance.moneyShakeTimer > 0) ? Game1.random.Next(-3, 4) : 0), 196 + ((__instance.moneyShakeTimer > 0) ? Game1.random.Next(-3, 4) : 0)), Game1.player.Money);
            if (__instance.moneyShakeTimer > 0)
                __instance.moneyShakeTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;

            return false;
        }
    }
}
