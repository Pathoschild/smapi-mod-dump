/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace HorseOverhaul.Patches
{
    internal class ThinHorseDrawPatches
    {
        private static HorseOverhaul mod;

        internal static void ApplyPatches(HorseOverhaul horseOverhaul, Harmony harmony)
        {
            mod = horseOverhaul;

            harmony.Patch(
               original: AccessTools.Method(typeof(Horse), nameof(Horse.draw), new Type[] { typeof(SpriteBatch) }),
               transpiler: new HarmonyMethod(typeof(ThinHorseDrawPatches), nameof(FixHeadAndHatPosition)));
        }

        // transpiler checked for 1.6.4
        public static IEnumerable<CodeInstruction> FixHeadAndHatPosition(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var instructionsList = instructions.ToList();

                bool foundHead = false;
                bool foundHat = false;

                bool foundFirstMunch = false;
                bool foundSecondMunch = false;

                for (int i = 0; i < instructionsList.Count; i++)
                {
                    if (!foundHead)
                    {
                        if (instructionsList[i].opcode == OpCodes.Ldc_R4
                            && 48f.IsAboutEqualTo((float)instructionsList[i].operand))
                        {
                            var info = typeof(ThinHorseDrawPatches).GetMethod(nameof(GetHorseHeadXPosition));
                            var oldLables = instructionsList[i].labels;
                            instructionsList[i] = new CodeInstruction(OpCodes.Call, info)
                            {
                                labels = oldLables
                            };

                            foundHead = true;
                        }
                    }

                    // TODO clean this all up later
                    if (!foundFirstMunch || !foundSecondMunch)
                    {
                        if (instructionsList[i].opcode == OpCodes.Ldfld
                            && instructionsList[i].operand != null
                            && instructionsList[i].operand.ToString().ToLower().Contains("munchingcarrottimer"))
                        {
                            if (!foundFirstMunch)
                            {
                                foundFirstMunch = true;
                            }
                            else if (!foundSecondMunch)
                            {
                                foundSecondMunch = true;
                            }

                            for (int j = i; j < instructionsList.Count; j++)
                            {
                                if (instructionsList[j].opcode == OpCodes.Ldc_R4
                                    && 24f.IsAboutEqualTo((float)instructionsList[j].operand))
                                {
                                    var info = typeof(ThinHorseDrawPatches).GetMethod(nameof(GetHorseHeadXPositionMunchOne));
                                    var oldLables = instructionsList[j].labels;
                                    instructionsList[j] = new CodeInstruction(OpCodes.Call, info)
                                    {
                                        labels = oldLables
                                    };
                                }

                                if (instructionsList[j].opcode == OpCodes.Ldc_R4
                                    && 80f.IsAboutEqualTo((float)instructionsList[j].operand))
                                {
                                    var info = typeof(ThinHorseDrawPatches).GetMethod(nameof(GetHorseHeadXPositionMunchTwo));
                                    var oldLables = instructionsList[j].labels;
                                    instructionsList[j] = new CodeInstruction(OpCodes.Call, info)
                                    {
                                        labels = oldLables
                                    };
                                }

                                if (instructionsList[j].opcode == OpCodes.Ldc_R4
                                    && (-16f).IsAboutEqualTo((float)instructionsList[j].operand))
                                {
                                    var info = typeof(ThinHorseDrawPatches).GetMethod(nameof(GetHorseHeadXPositionMunchThree));
                                    var oldLables = instructionsList[j].labels;
                                    instructionsList[j] = new CodeInstruction(OpCodes.Call, info)
                                    {
                                        labels = oldLables
                                    };
                                }
                            }
                        }
                    }

                    if (!foundHat)
                    {
                        if (i + 2 < instructionsList.Count
                            && instructionsList[i].opcode == OpCodes.Call
                            && instructionsList[i].operand != null
                            && instructionsList[i].operand.ToString().ToLower().Contains("get_zero")
                            && instructionsList[i + 1].opcode == OpCodes.Stloc_1
                            && instructionsList[i + 2].opcode == OpCodes.Ldarg_0)
                        {
                            var info = typeof(ThinHorseDrawPatches).GetMethod(nameof(GetHatVector));
                            var oldLables = instructionsList[i].labels;
                            instructionsList[i] = new CodeInstruction(OpCodes.Call, info)
                            {
                                labels = oldLables
                            };

                            foundHat = true;
                        }
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

        public static float GetHorseHeadXPositionMunchOne()
        {
            return mod.Config.ThinHorse ? -8f : 24f;
        }

        public static float GetHorseHeadXPositionMunchTwo()
        {
            return mod.Config.ThinHorse ? 52f : 80f;
        }

        public static float GetHorseHeadXPositionMunchThree()
        {
            return mod.Config.ThinHorse ? -48f : -16f;
        }

        public static float GetHorseHeadXPosition()
        {
            return mod.Config.ThinHorse ? 16f : 48f;
        }

        private static readonly Vector2 thinHorseHatVector = new(-8f, 0f);

        public static Vector2 GetHatVector()
        {
            return mod.Config.ThinHorse ? thinHorseHatVector : Vector2.Zero;
        }
    }
}