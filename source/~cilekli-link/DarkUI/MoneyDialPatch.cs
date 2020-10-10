/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cilekli-link/SDVMods
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DarkUI
{
    [HarmonyPatch(typeof(MoneyDial))]
    [HarmonyPatch("draw")]
    class MoneyDialDrawer
    { 
        private static int speed;
        private static int soundTimer;
        private static int moneyMadeAccumulator;
        private static int moneyShineTimer;
        static bool Prefix(SpriteBatch b, Vector2 position, int target, MoneyDial __instance, bool ___playSounds)
        { 
            if (__instance.previousTargetValue != target)
            {
                speed = (target - __instance.currentValue) / 100;
                __instance.previousTargetValue = target;
                soundTimer = Math.Max(6, 100 / (Math.Abs(speed) + 1));
            }
            if (moneyShineTimer > 0 && __instance.currentValue == target)
                moneyShineTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            if (moneyMadeAccumulator > 0)
            {
                moneyMadeAccumulator -= (Math.Abs(speed / 2) + 1) * (__instance.animations.Count <= 0 ? 100 : 1);
                if (moneyMadeAccumulator <= 0)
                    moneyShineTimer = __instance.numDigits * 60;
            }
            if (moneyMadeAccumulator > 2000)
                Game1.dayTimeMoneyBox.moneyShakeTimer = 100;
            if (__instance.currentValue != target)
            {
                __instance.currentValue += speed + (__instance.currentValue < target ? 1 : -1);
                if (__instance.currentValue < target)
                    moneyMadeAccumulator += Math.Abs(speed);
                --soundTimer;
                if (Math.Abs(target - __instance.currentValue) <= speed + 1 || speed != 0 && Math.Sign(target - __instance.currentValue) != Math.Sign(speed))
                    __instance.currentValue = target;
                if (soundTimer <= 0)
                {
                    if (__instance.currentValue < target && ___playSounds)
                        Game1.playSound("moneyDial");
                    soundTimer = Math.Max(6, 100 / (Math.Abs(speed) + 1));
                    if (Game1.random.NextDouble() < 0.4)
                    {
                        if (target > __instance.currentValue)
                            __instance.animations.Add(new TemporaryAnimatedSprite(Game1.random.Next(10, 12), position + new Vector2((float)Game1.random.Next(30, 190), (float)Game1.random.Next(-32, 48)), Color.Gold, 8, false, 100f, 0, -1, -1f, -1, 0));
                        else if (target < __instance.currentValue)
                            __instance.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(356, 449, 1, 1), 999999f, 1, 44, position + new Vector2((float)Game1.random.Next(160), (float)Game1.random.Next(-32, 32)), false, false, 1f, 0.01f, Color.White, (float)(Game1.random.Next(1, 3) * 4), -1f / 1000f, 0.0f, 0.0f, false)
                            {
                                motion = new Vector2((float)Game1.random.Next(-30, 40) / 10f, (float)Game1.random.Next(-30, -5) / 10f),
                                acceleration = new Vector2(0.0f, 0.25f)
                            });
                    }
                }
            }
            for (int index = __instance.animations.Count - 1; index >= 0; --index)
            {
                if (__instance.animations[index].update(Game1.currentGameTime))
                    __instance.animations.RemoveAt(index);
                else
                    __instance.animations[index].draw(b, true, 0, 0, 1f);
            }
            int num1 = 0;
            int num2 = (int)Math.Pow(10.0, (double)(__instance.numDigits - 1));
            bool flag = false;
            for (int index = 0; index < __instance.numDigits; ++index)
            {
                int num3 = __instance.currentValue / num2 % 10;
                if (num3 > 0 || index == __instance.numDigits - 1)
                    flag = true;
                if (flag)
                    b.Draw(Game1.mouseCursors, position + new Vector2((float)num1, Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is ShippingMenu) || __instance.currentValue < 1000000 ? 0.0f : (float)Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 100.530967712402 + (double)index) * (float)(__instance.currentValue / 1000000)), new Rectangle?(new Rectangle(286, 502 - num3 * 8, 5, 8)), Color.White, 0.0f, Vector2.Zero, (float)(4.0 + (moneyShineTimer / 60 == __instance.numDigits - index ? 0.300000011920929 : 0.0)), SpriteEffects.None, 1f);
                num1 += 24;
                num2 /= 10;
            }
            return false;
        }
    } 
}
