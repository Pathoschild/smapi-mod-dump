/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmod-24h-harmony/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Harmony;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using PatcherHelper;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;

namespace _24H_Harmony
    {
    public class Patcher
        {
        private class WantOpCache_24H : WantOpCache
            {
            internal WantOp
                call_getcurlang = new WantOp(OpCodes.Call, value: "get_CurrentLanguageCode"),
                ldc_i4_s_100 = new WantOp(OpCodes.Ldc_I4_S, (sbyte)100),
                ldc_i4_s_24 = new WantOp(OpCodes.Ldc_I4_S, (sbyte)24),
                fld__hours = new WantOp(OpCodes.Ldfld, value: "_hours"),
                fld__timetext = new WantOp(OpCodes.Ldfld, value: "_timeText")
                ;
            }

        private static IMonitor Monitor;
        private static WantOpCache_24H wopc;
        private static HarmonyPatcherHelper harp;

        public static void Initialize(string uniqueID, IMonitor monitor) {
            Monitor = monitor;
            wopc = new WantOpCache_24H();
            harp = new HarmonyPatcherHelper(uniqueID, Monitor, typeof(Patcher));
            }

        public static void Execute() {
            // For DayTimeMoneyBox.draw(), targetMethod needs to be hardcoded because method is private.
            // Furthermore, there are 2 draw() overrides, so we need to choose the one with the right signature
            harp.TryPatching(typeof(DayTimeMoneyBox), "draw", transpiler: nameof(Menus_DayTimeMoneyBox_draw_transp), targetParams: new Type[] { typeof(SpriteBatch) });

            harp.TryPatching(typeof(Game1), nameof(Game1.getTimeOfDayString), prefix: nameof(Patcher.G1_getTimeOfDayString_prefix));
            }

        private static void DebugLog(string msg) => Monitor.Log(msg, LogLevel.Debug);

        public static IEnumerable<CodeInstruction> Menus_DayTimeMoneyBox_draw_transp(IEnumerable<CodeInstruction> oldInstructions) {
            var walker = new InstructionsWalker(oldInstructions);

            /*
                // [212 7 - 212 26]
                IL_037b: ldarg.0      // this
                IL_037c: ldfld        class [mscorlib]System.Text.StringBuilder StardewValley.Menus.DayTimeMoneyBox::_hours
                IL_0381: callvirt     instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Clear()
                IL_0386: pop

                // [213 7 - 213 59]
                IL_0387: call         valuetype StardewValley.LocalizedContentManager/LanguageCode StardewValley.LocalizedContentManager::get_CurrentLanguageCode()
                IL_038c: stloc.s      V_8

                IL_038e: ldloc.s      V_8
                IL_0390: switch       (IL_0487, IL_0487, IL_0430, IL_03ce, IL_0430, IL_0430, IL_0430, IL_0430, IL_0430, IL_0487, IL_0487, IL_0430, IL_0430)
                IL_03c9: br           IL_0487
             */
            var target1 = new WantOp[] {
                wopc.ldarg_0,
                wopc.fld__hours,
                wopc.callv_Clear,
                wopc.pop,
                wopc.call_getcurlang,
                wopc.stloc_s,
                wopc.ldloc_s,  // <== replace this, offset = +6
                wopc.@switch,
                wopc.br
                };
            // "ru" uses 24h
            var replacer1 = new CodeInstruction(OpCodes.Ldc_I4, (int)LocalizedContentManager.LanguageCode.ru);
            walker
                .GoFind(target1)
                .ReplaceAt(relative_pos: 6, with: replacer1, assert_previous: wopc.ldloc_s);

            /*
                // [260 7 - 260 46]
                IL_051c: ldarg.0      // this
                IL_051d: ldfld        class [mscorlib]System.Text.StringBuilder StardewValley.Menus.DayTimeMoneyBox::_timeText
                IL_0522: ldarg.0      // this
                IL_0523: ldfld        class [mscorlib]System.Text.StringBuilder StardewValley.Menus.DayTimeMoneyBox::_padZeros
                IL_0528: call         class [mscorlib]System.Text.StringBuilder StardewValley.StringBuilderFormatEx::AppendEx(class [mscorlib]System.Text.StringBuilder, class [mscorlib]System.Text.StringBuilder)
                IL_052d: pop

                // [261 7 - 261 59]
                IL_052e: call         valuetype StardewValley.LocalizedContentManager/LanguageCode StardewValley.LocalizedContentManager::get_CurrentLanguageCode()
                IL_0533: brfalse.s    IL_053e
             */
            var target2 = new WantOp[] {
                wopc.ldarg_0,
                wopc.fld__timetext,
                wopc.ldarg_0,
                new WantOp(OpCodes.Ldfld, value: "_padZeros"),
                wopc.call_AppendEx,
                wopc.pop,
                wopc.call_getcurlang,  // <== start patching here (offset = +6)
                wopc.brfalse_s
                };
            walker
                .GoFind(target2)
                .GoForward(6, assert_op: wopc.call_getcurlang)
                .FetchLocation(out int patch2_pos)
                .GoFetchOp(out CodeInstruction patch2_orig_op, assert_op: wopc.call_getcurlang)
                ;
            //walker.GoForward(7, assert_op: wopc.brfalse_s);
            //Monitor.Log(walker.CurrentOp().Repr(), LogLevel.Warn);

            /*
                // [330 7 - 330 61]
                IL_07f7: ldloc.0      // font
                IL_07f8: ldarg.0      // this
                IL_07f9: ldfld        class [mscorlib]System.Text.StringBuilder StardewValley.Menus.DayTimeMoneyBox::_timeText
                IL_07fe: callvirt     instance valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Vector2 [Microsoft.Xna.Framework.Graphics]Microsoft.Xna.Framework.Graphics.SpriteFont::MeasureString(class [mscorlib]System.Text.StringBuilder)
                IL_0803: stloc.3      // vector2_3

                // [331 7 - 331 421]
                IL_0804: ldloca.s     vector2_4
                IL_0806: ldarg.0      // this
                IL_0807: ldflda       valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Rectangle StardewValley.Menus.DayTimeMoneyBox::sourceRect
                IL_080c: ldfld        int32 [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Rectangle::X
                IL_0811: conv.r4
                IL_0812: ldc.r4       0.55
                IL_0817: mul
                IL_0818: ldloc.3      // vector2_3
                IL_0819: ldfld        float32 [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Vector2::X
                IL_081e: ldc.r4       2
             */
            var target2_target = new WantOp[] {
                wopc.ldloc_0,
                wopc.ldarg_0,
                wopc.fld__timetext,
                new WantOp(OpCodes.Callvirt, value: "MeasureString"),
                wopc.stloc_3,
                wopc.ldloca_s,
                wopc.ldarg_0,
                new WantOp(OpCodes.Ldflda, value: "sourceRect"),
                wopc.fld_X,
                wopc.conv_r4,
                wopc.ldc_r4,
                wopc.mul
                };
            walker
                .GoFind(target2_target)
                .GoFetchOp(out CodeInstruction target2_targetop);

            //foreach (var l in target2_targetop.labels) {
            //    Monitor.Log(l.ToString(), LogLevel.Warn);
            //    }
            var replacer2 = new CodeInstruction(OpCodes.Br, target2_targetop.labels[0]);
            walker.ReplaceAt(absolute_pos: patch2_pos, with: replacer2, assert_previous: new WantOp(patch2_orig_op));

            return walker.Instructions;
            }

        public static bool G1_getTimeOfDayString_prefix(ref string __result, int time) {
            try {
                // ':0000' is string-interpolation's way of saying "4 digits, pad left with zero if shorter"
                // See: https://dotnetfiddle.net/tj9wUm
                __result = $"{time:0000}".Insert(2, ":");
                // This is _definitely_ much faster than the original method of dividing by 100 + getting remainder,
                // with possible cost of _slightly_ more memory usage. Like a few bytes' worth; no reason to fuss about.

                return false; // do NOT run original logic
                }
            catch (Exception ex) {
                Monitor.Log($"Failed in G1_getTimeOfDayString_prefix:\n{ex}", LogLevel.Error);
                Monitor.Log("Reverting to unpatched behavior!", LogLevel.Warn);
                return true; // run original logic
                }
            }

        }
    }
