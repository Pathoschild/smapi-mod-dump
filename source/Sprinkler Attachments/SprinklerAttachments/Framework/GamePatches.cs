/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mushymato/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HarmonyLib;
using StardewValley;
using StardewObject = StardewValley.Object;
using StardewModdingAPI;
using StardewValley.Objects;
using System.Reflection.Emit;

namespace SprinklerAttachments.Framework
{

    internal static class GamePatches
    {

        internal static void Apply(Harmony harmony)
        {
            try
            {
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(StardewObject), nameof(StardewObject.performObjectDropInAction)),
                    postfix: new HarmonyMethod(typeof(GamePatches), nameof(Object_performObjectDropInAction_Postfix))
                );
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(StardewObject), nameof(StardewObject.checkForAction)),
                    postfix: new HarmonyMethod(typeof(GamePatches), nameof(Object_checkForAction_Postfix))
                );
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(StardewObject), nameof(StardewObject.updateWhenCurrentLocation)),
                    postfix: new HarmonyMethod(typeof(GamePatches), nameof(Object_updateWhenCurrentLocation_Postfix))
                );
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(StardewObject), nameof(StardewObject.GetModifiedRadiusForSprinkler)),
                    postfix: new HarmonyMethod(typeof(GamePatches), nameof(Object_GetModifiedRadiusForSprinkler_PostFix))
                );
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(StardewObject), nameof(StardewObject.draw), new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
                    transpiler: new HarmonyMethod(typeof(GamePatches), nameof(Object_draw_Transpiler))
                );
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(Chest), nameof(Chest.GetActualCapacity)),
                    postfix: new HarmonyMethod(typeof(GamePatches), nameof(Chest_GetActualCapacity_Postfix))
                );

            }
            catch (Exception err)
            {
                ModEntry.Log($"Failed to patch SprinklerAttachments:\n{err}", LogLevel.Error);
            }
        }

        private static void Object_performObjectDropInAction_Postfix(StardewObject __instance, Item dropInItem, bool probe, Farmer who, ref bool __result, bool returnFalseIfItemConsumed = false)
        {
            try
            {
                if (!__result && SprinklerAttachment.TryAttachToSprinkler(__instance, dropInItem, probe))
                    __result = true;
            }
            catch (Exception err)
            {
                ModEntry.Log($"Error in Object_performObjectDropInAction_Postfix:\n{err}", LogLevel.Error);
            }
        }

        private static void Object_checkForAction_Postfix(StardewObject __instance, Farmer who, ref bool __result, bool justCheckingForActivity = false)
        {
            try
            {
                if (!__result && SprinklerAttachment.CheckForAction(__instance, who, justCheckingForActivity))
                    __result = true;
            }
            catch (Exception err)
            {
                ModEntry.Log($"Error in Object_checkForAction_Postfix:\n{err}", LogLevel.Error);
            }
        }

        private static void Object_GetModifiedRadiusForSprinkler_PostFix(StardewObject __instance, ref int __result)
        {
            try
            {
                __result = SprinklerAttachment.GetModifiedRadiusForSprinkler(__instance, __result);
            }
            catch (Exception err)
            {
                ModEntry.Log($"Error in Object_GetModifiedRadiusForSprinkler_PostFix:\n{err}", LogLevel.Error);
            }
        }

        private static void Object_updateWhenCurrentLocation_Postfix(StardewObject __instance, GameTime time)
        {
            try
            {
                SprinklerAttachment.UpdateWhenCurrentLocation(__instance, time);
            }
            catch (Exception err)
            {
                ModEntry.Log($"Error in Object_updateWhenCurrentLocation_Postfix:\n{err}", LogLevel.Error);
            }
        }

        private static IEnumerable<CodeInstruction> Object_draw_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            try
            {
                CodeMatcher matcher = new(instructions, generator);

                // IL_0c79: ldarg.0
                // IL_0c7a: ldfld class Netcode.NetBool StardewValley.Object::readyForHarvest
                // IL_0c7f: call bool Netcode.NetBool::op_Implicit(class Netcode.NetBool)
                // IL_0c84: brfalse IL_0f33

                matcher = matcher.Start()
                .MatchStartForward(new CodeMatch[]{
                    // new(OpCodes.Ldarg_0),
                    // new(OpCodes.Call, AccessTools.PropertyGetter(typeof(Item), nameof(Item.SpecialVariable))),
                    // new(OpCodes.Ldc_I4, 999999),
                    // new(OpCodes.Bne_Un)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, AccessTools.DeclaredField(typeof(StardewObject), nameof(StardewObject.readyForHarvest))),
                    new(OpCodes.Call),
                    new(OpCodes.Brfalse)
                })
                .CreateLabel(out Label lbl1)
                .MatchEndBackwards(new CodeMatch[]{
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, AccessTools.DeclaredField(typeof(StardewObject), nameof(StardewObject.heldObject))),
                    new(OpCodes.Callvirt),
                    new(OpCodes.Brfalse),
                    new(OpCodes.Call),
                })
                .Insert(new CodeInstruction[]{
                    new(OpCodes.Ldarg_0), // StardewObject __instance
                    new(OpCodes.Ldarg_1), // SpriteBatch spriteBatch
                    new(OpCodes.Ldarg_2), // int x
                    new(OpCodes.Ldarg_3), // int y
                    new(OpCodes.Ldarg_S, 4), // float alpha = 1f
                    new(OpCodes.Call, AccessTools.Method(typeof(SprinklerAttachment), nameof(SprinklerAttachment.DrawAttachment))),
                    new(OpCodes.Brtrue_S, lbl1)
                })
                ;

                return matcher.Instructions();
            }
            catch (Exception err)
            {
                ModEntry.Log($"Error in Object_draw_Transpiler:\n{err}", LogLevel.Error);
                return instructions;
            }
        }

        private static void JustDraw()
        {
            ModEntry.Log($"JustDraw", LogLevel.Debug);
        }

        private static bool ShouldDraw(StardewObject? heldObject)
        {
            ModEntry.Log($"ShouldDraw {heldObject?.Name}", LogLevel.Debug);
            return true;
        }
        private static void Chest_GetActualCapacity_Postfix(Chest __instance, ref int __result)
        {
            try
            {
                __result = SprinklerAttachment.GetActualCapacity(__instance, __result);
            }
            catch (Exception err)
            {
                ModEntry.Log($"Error in Chest_GetActualCapacity_Postfix:\n{err}", LogLevel.Error);
            }
        }
    }
}
