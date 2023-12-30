/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/NPCGeometry
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Characters;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ichortower.NPCGeometry
{
    internal sealed class ModEntry : Mod
    {
        public static IMonitor MONITOR;
        public static IModHelper HELPER;
        public static string Prefix = "ichortower.NPCGeometry";


        public override void Entry(IModHelper helper)
        {
            ModEntry.MONITOR = this.Monitor;
            ModEntry.HELPER = helper;
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: typeof(Character).GetMethod("DrawShadow",
                    BindingFlags.Instance | BindingFlags.Public),
                transpiler: new HarmonyMethod(typeof(ModEntry),
                    "Character_DrawShadow__Transpiler")
            );
            harmony.Patch(
                original: typeof(NPC).GetMethod("DrawEmote",
                    BindingFlags.Instance | BindingFlags.Public,
                    new Type[]{typeof(SpriteBatch)}),
                transpiler: new HarmonyMethod(typeof(ModEntry),
                    "NPC_DrawEmote__Transpiler")
            );
            harmony.Patch(
                original: typeof(NPC).GetMethod("DrawBreathing",
                    BindingFlags.Instance | BindingFlags.Public,
                    new Type[]{typeof(SpriteBatch), typeof(float)}),
                transpiler: new HarmonyMethod(typeof(ModEntry),
                    "NPC_DrawBreathing__Transpiler")
            );
            harmony.Patch(
                original: typeof(Game1).GetMethod("DrawCharacterEmotes",
                    BindingFlags.Instance | BindingFlags.Public),
                transpiler: new HarmonyMethod(typeof(ModEntry),
                    "Game1_DrawCharacterEmotes__Transpiler")
            );
        }


        /*
         * Helper function for the transpilers: gets a specific CustomField
         * from CharacterData, or null if not available for various reasons.
         *
         * Loading and checking the CharacterData and CustomFields is a lot
         * of instructions and null checks and LocalBuilders, and I have to do
         * it repeatedly; plus I had problems with ldfld on the CharacterData
         * object. So, this got farmed out to C#, where I just call it.
         */
        public static string GetCustomFieldValue(Character who, string key)
        {
            string val;
            if ((who as NPC)?.GetData()?.CustomFields?
                    .TryGetValue(key, out val) == true) {
                return val;
            }
            return null;
        }

        /*
         * Likewise, this helper function saves me a bunch of tedious CIL
         * work. But it also handles saving the adjusted values into the
         * target structs.
         */
        public static bool TryParseBreatheRect(string val,
                ref Microsoft.Xna.Framework.Rectangle chestBox,
                ref Microsoft.Xna.Framework.Vector2 chestPosition)
        {
            string[] split = val.Split("/");
            int x, y, width, height;
            if (split.Length < 4) {
                return false;
            }
            if (int.TryParse(split[0], out x) &&
                    int.TryParse(split[1], out y) &&
                    int.TryParse(split[2], out width) &&
                    int.TryParse(split[3], out height)) {
                chestBox.X += x;
                chestBox.Y += y;
                chestBox.Width = width;
                chestBox.Height = height;
                /* do not ask me where 19 comes from. a wizard did it */
                chestPosition = new Vector2(x+(width/2), y-19+(height/2)) * 4f;
                return true;
            }
            return false;
        }


        /*
         * For DrawShadow, we want to add an extra multiplying factor to the
         * scale parameter in the draw call.
         * Read the factor from ShadowScale if it's available. Otherwise, use
         * 1.0.
         * Then, inject the extra ldloc/mul at the right spot.
         */
        public static IEnumerable<CodeInstruction> Character_DrawShadow__Transpiler(
                IEnumerable<CodeInstruction> instructions,
                ILGenerator generator,
                MethodBase original)
        {
            LocalBuilder shadowScale = generator.DeclareLocal(typeof(float));
            LocalBuilder stringVal = generator.DeclareLocal(typeof(string));
            Label startOfOriginalCode = generator.DefineLabel();
            var codes = new List<CodeInstruction>(instructions);
            codes[0].labels.Add(startOfOriginalCode);
            /* this is the CustomFields check. goes right at the start */
            var fieldChecker = new List<CodeInstruction>(){
                new(OpCodes.Ldc_R4, 1.0f),
                new(OpCodes.Stloc, shadowScale),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldstr, Prefix + "/ShadowScale"),
                new(OpCodes.Call, typeof(ModEntry).GetMethod("GetCustomFieldValue",
                        BindingFlags.Public | BindingFlags.Static)),
                new(OpCodes.Stloc, stringVal),
                new(OpCodes.Ldloc, stringVal),
                new(OpCodes.Brfalse, startOfOriginalCode),
                new(OpCodes.Ldloc, stringVal),
                new(OpCodes.Ldloca, shadowScale),
                new(OpCodes.Call, typeof(System.Single).GetMethod("TryParse",
                        BindingFlags.Public | BindingFlags.Static,
                        new Type[]{typeof(string), typeof(float).MakeByRefType()})),
                /* we don't actually care what TryParse returns, since right
                 * after this instruction is startOfOriginalCode */
                new(OpCodes.Pop),
            };
            codes.InsertRange(0, fieldChecker);

            /*
             * to find the injection point for the extra multiply, we are
             * looking for the load of constant 40f, then finding the first
             * callvirt after that.
             * it's probably better to find the callvirt directly by checking
             * the MethodInfo operand, but this is working so far.
             */
            int target = -1;
            bool forty = false;
            for (int i = 0; i < codes.Count - 1; ++i) {
                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 40f) {
                    forty = true;
                }
                if (forty && codes[i].opcode == OpCodes.Callvirt) {
                    target = i+1;
                    break;
                }
            }
            var extraMul = new List<CodeInstruction>(){
                new(OpCodes.Ldloc, shadowScale),
                new(OpCodes.Mul),
            };
            codes.InsertRange(target, extraMul);
            return codes;
        }


        /*
	 * For NPC.DrawEmote, EmoteHeight replaces the emote bubble height
	 * calculation with a custom offset.
         */
        public static IEnumerable<CodeInstruction> NPC_DrawEmote__Transpiler(
                IEnumerable<CodeInstruction> instructions,
                ILGenerator generator,
                MethodBase original)
        {
            LocalBuilder emoteHeight = generator.DeclareLocal(typeof(int));
            LocalBuilder emoteStringVal = generator.DeclareLocal(typeof(string));
            Label noHeightField = generator.DefineLabel();
            Label foundHeightField = generator.DefineLabel();
            var codes = new List<CodeInstruction>(instructions);

            /* The emote height code. */
            var heightInjection = new List<CodeInstruction>(){
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldstr, Prefix + "/EmoteHeight"),
                new(OpCodes.Call, typeof(ModEntry).GetMethod("GetCustomFieldValue",
                        BindingFlags.Public | BindingFlags.Static)),
                new(OpCodes.Stloc, emoteStringVal),
                new(OpCodes.Ldloc, emoteStringVal),
                new(OpCodes.Brfalse, noHeightField),
                new(OpCodes.Ldloc, emoteStringVal),
                new(OpCodes.Ldloca, emoteHeight),
                new(OpCodes.Call, typeof(System.Int32).GetMethod("TryParse",
                        BindingFlags.Public | BindingFlags.Static,
                        new Type[]{typeof(string), typeof(int).MakeByRefType()})),
                new(OpCodes.Brfalse, noHeightField),
                new(OpCodes.Ldloc, emoteHeight),
                new(OpCodes.Ldc_I4_S, (SByte)4),
                new(OpCodes.Sub),
                new(OpCodes.Br_S, foundHeightField),
            };
            /* The anchor is loading the constant 32 */
            int heightTarget = -1;
            for (int i = codes.Count - 5; i >= 0; --i) {
                if (codes[i].opcode == OpCodes.Ldc_I4_S &&
                        codes[i].operand.Equals((SByte)32)) {
                    heightTarget = i+1;
                    codes[heightTarget].labels.Add(noHeightField);
                    codes[heightTarget+3].labels.Add(foundHeightField);
                    break;
                }
            }
            codes.InsertRange(heightTarget, heightInjection);
            return codes;
        }


	/*
	 * In NPC.DrawBreathing, BreatheRect picks a custom rectangle on the
	 * sprite to animate to show breathing, instead of calculating it with
	 * heuristics.
	 */
        public static IEnumerable<CodeInstruction> NPC_DrawBreathing__Transpiler(
                IEnumerable<CodeInstruction> instructions,
                ILGenerator generator,
                MethodBase original)
        {
            LocalBuilder breatheStringVal = generator.DeclareLocal(typeof(string));
            Label noBreatheRectField = generator.DefineLabel();
            Label foundBreatheRectField = generator.DefineLabel();
            var codes = new List<CodeInstruction>(instructions);

            /* The breathe rect code (see the helper functions).
	     * Note: 0 and 1 are local indexes for chestBox and chestPosition */
            var breatheInjection = new List<CodeInstruction>(){
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldstr, Prefix + "/BreatheRect"),
                new(OpCodes.Call, typeof(ModEntry).GetMethod("GetCustomFieldValue",
                        BindingFlags.Public | BindingFlags.Static)),
                new(OpCodes.Stloc, breatheStringVal),
                new(OpCodes.Ldloc, breatheStringVal),
                new(OpCodes.Brfalse, noBreatheRectField),
                new(OpCodes.Ldloc, breatheStringVal),
                new(OpCodes.Ldloca_S, (short)0),
                new(OpCodes.Ldloca_S, (short)1),
                new(OpCodes.Call, typeof(ModEntry).GetMethod("TryParseBreatheRect",
                        BindingFlags.Public | BindingFlags.Static)),
                new(OpCodes.Brfalse, noBreatheRectField),
                new(OpCodes.Br, foundBreatheRectField),
            };
            /* inject right after initial setting (chestBox = SourceRect).
	     * that means stloc.0 */
            int breatheTarget = -1;
            for (int i = 0; i < codes.Count - 1; ++i) {
                if (codes[i].opcode == OpCodes.Stloc_0) {
                    breatheTarget = i+1;
                    codes[breatheTarget].labels.Add(noBreatheRectField);
                    break;
                }
            }
	    /* add a label at the 'float breathScale =' line so we can skip
	     * to it if we found a valid BreatheRect. we need ldc.r4 0.0 */
            for (int i = breatheTarget+1; i < codes.Count; ++i) {
                if (codes[i].opcode == OpCodes.Ldc_R4 &&
                        codes[i].operand.Equals(0.0f)) {
                    codes[i].labels.Add(foundBreatheRectField);
                    break;
                }
            }
            codes.InsertRange(breatheTarget, breatheInjection);

	    return codes;
	}


        /*
         * The Game1.DrawCharacterEmotes transpiler patches the other place
         * where the game handles drawing emote bubbles (this one is used
         * during events, and the NPC.draw one is disabled).
         * The code is mostly the same, but the math is slightly different.
         */
        public static IEnumerable<CodeInstruction> Game1_DrawCharacterEmotes__Transpiler(
                IEnumerable<CodeInstruction> instructions,
                ILGenerator generator,
                MethodBase original)
        {
            LocalBuilder emoteHeight = generator.DeclareLocal(typeof(int));
            LocalBuilder emoteStringVal = generator.DeclareLocal(typeof(string));
            Label noHeightField = generator.DefineLabel();
            Label foundHeightField = generator.DefineLabel();
            var codes = new List<CodeInstruction>(instructions);

            var heightInjection = new List<CodeInstruction>(){
                new(OpCodes.Ldloc_2),
                new(OpCodes.Ldstr, Prefix + "/EmoteHeight"),
                new(OpCodes.Call, typeof(ModEntry).GetMethod("GetCustomFieldValue",
                        BindingFlags.Public | BindingFlags.Static)),
                new(OpCodes.Stloc, emoteStringVal),
                new(OpCodes.Ldloc, emoteStringVal),
                new(OpCodes.Brfalse, noHeightField),
                new(OpCodes.Ldloc, emoteStringVal),
                new(OpCodes.Ldloca, emoteHeight),
                new(OpCodes.Call, typeof(System.Int32).GetMethod("TryParse",
                        BindingFlags.Public | BindingFlags.Static,
                        new Type[]{typeof(string), typeof(int).MakeByRefType()})),
                new(OpCodes.Brfalse, noHeightField),
                new(OpCodes.Ldloca_S, (SByte)3),
                new(OpCodes.Ldflda, typeof(Vector2).GetField("Y",
                        BindingFlags.Public | BindingFlags.Instance)),
                new(OpCodes.Dup),
                new(OpCodes.Ldind_R4),
                new(OpCodes.Ldloc, emoteHeight),
                /* this +4 was also done by a wizard. it's magic */
                new(OpCodes.Ldc_I4_S, (SByte)4),
                new(OpCodes.Add),
                new(OpCodes.Ldc_I4_S, (SByte)4),
                new(OpCodes.Mul),
                new(OpCodes.Sub),
                new(OpCodes.Stind_R4),
                new(OpCodes.Br_S, foundHeightField),
            };

            /*
             * the anchor is loading the constant 140.
             * At the actual insertion point, we have to move the labels from
             * the existing instruction, because the NeedsBirdieEmoteHack check
             * branches to the same spot and would otherwise skip our code.
             */
            int heightTarget = -1;
            for (int i = 4; i < codes.Count; ++i) {
                if (codes[i].opcode == OpCodes.Ldc_R4 &&
                        codes[i].operand.Equals(140f)) {
                    heightTarget = i-4;
                    foreach (var l in codes[heightTarget].labels) {
                        heightInjection[0].labels.Add(l);
                    }
                    codes[heightTarget].labels.Clear();
                    codes[heightTarget].labels.Add(noHeightField);
                    break;
                }
            }
            /* the subsequent ldsfld is where to skip to if found */
            for (int i = heightTarget+4; i < codes.Count; ++i) {
                if (codes[i].opcode == OpCodes.Ldsfld) {
                    codes[i].labels.Add(foundHeightField);
                    break;
                }
            }
            codes.InsertRange(heightTarget, heightInjection);
            return codes;
        }


        /*
         * Not currently provided. See README.
         *
        public static void NPC_reloadSprite__Postfix(NPC __instance)
        {
            var data = __instance.GetData();
            if (data is null) {
                return;
            }

            if (data.CustomFields?.TryGetValue(Prefix + "/Scale", out var val) == true) {
                if (float.TryParse(val, out var f)) {
                    __instance.Scale = f;
                }
            }
        }
        */
    }
}
