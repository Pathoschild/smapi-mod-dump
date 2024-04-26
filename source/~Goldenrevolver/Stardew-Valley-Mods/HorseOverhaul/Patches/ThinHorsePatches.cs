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
               original: AccessTools.Method(typeof(Character), nameof(Character.GetSpriteWidthForPositioning)),
               postfix: new HarmonyMethod(typeof(ThinHorsePatches), nameof(ChangeSpriteWidthForPositioningToOneTile)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Farmer), nameof(Farmer.showRiding)),
               prefix: new HarmonyMethod(typeof(ThinHorsePatches), nameof(FixRidingPosition)));

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

        // transpiler checked for 1.6.4:
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
                       && 64f.IsAboutEqualTo((float)instructionsList[i].operand)
                       && instructionsList[i + 1].opcode == OpCodes.Mul
                       && instructionsList[i + 2].opcode == OpCodes.Ldc_R4
                       && 32f.IsAboutEqualTo((float)instructionsList[i + 2].operand)
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

        // transpiler checked for 1.6.4:
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

        public static float GetDismountingXOffset()
        {
            return mod.Config.ThinHorse ? 16f : 32f;
        }

        public static void FixMountPosition(Farmer __instance, Horse mount)
        {
            if (mod.Config.ThinHorse && mount != null)
            {
                __instance.xOffset = mount.FacingDirection switch
                {
                    Game1.right => -4f, // counteracts the +8 from the horse update method to arrive at +4
                    Game1.left => 0,
                    _ => 4f,
                };
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

            return new Rectangle((int)horse.position.X + 8, (int)horse.position.Y + 16, 48, 32);
        }

        // postfix, so we can't get skipped by 'return false' prefixes
        public static void ChangeSpriteWidthForPositioningToOneTile(Character __instance, ref int __result)
        {
            if (mod.Config.ThinHorse && __instance is Horse)
            {
                __result = 16;
            }
        }

        public static bool FixRidingPosition(Farmer __instance)
        {
            if (!mod.Config.ThinHorse)
            {
                return true;
            }

            if (!__instance.isRidingHorse())
            {
                return false;
            }

            switch (__instance.FacingDirection)
            {
                case Game1.up:
                    __instance.FarmerSprite.setCurrentSingleFrame(113, 32000, false, false);
                    __instance.xOffset = 4f; // old: -6f, diff: +10
                    break;

                case Game1.right:
                    __instance.FarmerSprite.setCurrentSingleFrame(106, 32000, false, false);
                    __instance.xOffset = 7f; // old: -3f, diff: +10
                    break;

                case Game1.down:
                    __instance.FarmerSprite.setCurrentSingleFrame(107, 32000, false, false);
                    __instance.xOffset = 4f; // old: -6f, diff: +10
                    break;

                case Game1.left:
                    __instance.FarmerSprite.setCurrentSingleFrame(106, 32000, false, true);
                    __instance.xOffset = -2f; // old: -12f, diff: +10
                    break;
            }

            if (!__instance.isMoving())
            {
                __instance.yOffset = 0f;
                return false;
            }

            switch (__instance.mount.Sprite.currentAnimationIndex)
            {
                case 0:
                    __instance.yOffset = 0f;
                    return false;

                case 1:
                    __instance.yOffset = -4f;
                    return false;

                case 2:
                    __instance.yOffset = -4f;
                    return false;

                case 3:
                    __instance.yOffset = 0f;
                    return false;

                case 4:
                    __instance.yOffset = 4f;
                    return false;

                case 5:
                    __instance.yOffset = 4f;
                    return false;

                default:
                    return false;
            }
        }
    }
}