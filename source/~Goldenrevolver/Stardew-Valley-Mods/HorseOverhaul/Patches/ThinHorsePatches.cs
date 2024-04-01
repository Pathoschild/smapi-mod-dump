/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace HorseOverhaul
{
    using HarmonyLib;
    using Microsoft.Xna.Framework;
    using StardewValley;
    using StardewValley.Characters;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    internal class ThinHorsePatches
    {
        private static HorseOverhaul mod;

        internal static void ApplyPatches(HorseOverhaul horseOverhaul, Harmony harmony)
        {
            mod = horseOverhaul;

            harmony.Patch(
                original: AccessTools.Method(typeof(Horse), nameof(Horse.GetBoundingBox)),
                postfix: new HarmonyMethod(typeof(ThinHorsePatches), nameof(OverrideBoundingBox)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Horse), nameof(Horse.update), new Type[] { typeof(GameTime), typeof(GameLocation) }),
               transpiler: new HarmonyMethod(typeof(ThinHorsePatches), nameof(FixMountingAnimation)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Farmer), "setMount"),
               postfix: new HarmonyMethod(typeof(ThinHorsePatches), nameof(FixMountPosition)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Horse), nameof(Horse.checkAction)),
               transpiler: new HarmonyMethod(typeof(ThinHorsePatches), nameof(FixDismountingPosition)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Horse), nameof(Horse.update), new Type[] { typeof(GameTime), typeof(GameLocation) }),
               transpiler: new HarmonyMethod(typeof(ThinHorsePatches), nameof(FixDismountingPosition)));
        }

        // transpiler checked for 1.6.3:
        //    IL_00b4: ldc.r4 64
        //    IL_00b9: mul
        //    IL_00ba: ldc.r4 32
        //    IL_00bf: add
        public static IEnumerable<CodeInstruction> FixDismountingPosition(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var instructionsList = instructions.ToList();

                for (int i = 0; i < instructionsList.Count - 4; i++)
                {
                    if (instructionsList[i].opcode == OpCodes.Ldc_R4
                       && OperandIsAbout((float)instructionsList[i].operand, 64f)
                       && instructionsList[i + 1].opcode == OpCodes.Mul
                       && instructionsList[i + 2].opcode == OpCodes.Ldc_R4
                       && OperandIsAbout((float)instructionsList[i + 2].operand, 32f)
                       && instructionsList[i + 3].opcode == OpCodes.Add)
                    {
                        instructionsList[i + 2].opcode = OpCodes.Call;
                        instructionsList[i + 2].operand = typeof(ThinHorsePatches).GetMethod(nameof(GetDismountingXOffset));
                    }
                }

                return instructionsList.AsEnumerable();
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a transpiler patch", e);
                return instructions;
            }
        }

        public static float GetDismountingXOffset()
        {
            return mod.Config.ThinHorse ? 16f : 32f;
        }

        private static bool OperandIsAbout(float operand, float value)
        {
            return operand >= (value - 0.1f) && operand <= (value + 0.1f);
        }

        public static void FixMountPosition(Farmer __instance, Horse mount)
        {
            if (mod.Config.ThinHorse && mount != null)
            {
                __instance.xOffset += mount.FacingDirection switch
                {
                    Game1.right => 0f, // counteracts the +8 from the horse update method to arrive at +8
                    Game1.left => 4f,
                    _ => 8f,
                };
                __instance.position.X -= 24f;
            }
        }

        // postfix, so we can't get skipped by 'return false' prefixes
        public static void OverrideBoundingBox(Horse __instance, ref Rectangle __result)
        {
            if (mod.Config.ThinHorse)
            {
                __result = GetFixedBoundingBox(__instance);
            }
        }

        private static Rectangle GetFixedBoundingBox(Horse horse)
        {
            if (horse.Sprite == null)
            {
                return Rectangle.Empty;
            }

            return new Rectangle((int)horse.position.X + 32, (int)horse.position.Y + 16, 48, 32);
        }

        // transpiler checked for 1.6.3:
        //    IL_0144: ldfld int32[MonoGame.Framework]Microsoft.Xna.Framework.Rectangle::X
        //    IL_0149: ldc.i4.s 16
        //    IL_014b: add
        public static IEnumerable<CodeInstruction> FixMountingAnimation(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var instructionsList = instructions.ToList();

                for (int i = 0; i < instructionsList.Count - 2; i++)
                {
                    if (instructionsList[i].opcode == OpCodes.Ldfld
                        && (FieldInfo)instructionsList[i].operand == typeof(Rectangle).GetField(nameof(Rectangle.X))
                        && instructionsList[i + 1].opcode == OpCodes.Ldc_I4_S
                        && (sbyte)instructionsList[i + 1].operand == 16
                        && instructionsList[i + 2].opcode == OpCodes.Add)
                    {
                        instructionsList[i + 1].opcode = OpCodes.Call;
                        instructionsList[i + 1].operand = typeof(ThinHorsePatches).GetMethod(nameof(GetMountingXOffset));
                    }
                }

                return instructionsList.AsEnumerable();
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a transpiler patch", e);
                return instructions;
            }
        }

        // intentionally replace the original sbyte with an sbyte to make the transpiler more future proof
        public static sbyte GetMountingXOffset()
        {
            if (mod.Config.ThinHorse)
            {
                return -16;
            }
            else
            {
                return 16;
            }
        }
    }
}