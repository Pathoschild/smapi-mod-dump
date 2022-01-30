/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class ObjectPerformObjectDropInActionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectPerformObjectDropInActionPatch()
    {
        Original = RequireMethod<SObject>(nameof(SObject.performObjectDropInAction));
    }

    #region harmony patches

    /// <summary>Patch to remember initial machine state.</summary>
    [HarmonyPrefix]
    private static bool ObjectPerformObjectDropInActionPrefix(SObject __instance, ref bool __state)
    {
        __state = __instance.heldObject.Value !=
                  null; // remember whether this machine was already holding an object
        return true; // run original logic
    }

    /// <summary>
    ///     Patch to increase Gemologist mineral quality from Geode Crusher + speed up Artisan production
    ///     speed + integrate Quality Artisan Products.
    /// </summary>
    [HarmonyPostfix]
    private static void ObjectPerformObjectDropInActionPostfix(SObject __instance, bool __state, Item dropInItem,
        bool probe, Farmer who)
    {
        // if there was an object inside before running the original method, or if the machine is still empty after running the original method, then do nothing
        if (__state || __instance.heldObject.Value is null || probe) return;

        if (__instance.name.IsAnyOf("Mayonnaise Machine", "Cheese Press") && dropInItem is SObject)
        {
            // large milk/eggs give double output at normal quality
            if (dropInItem.Name.ContainsAnyOf("Large", "L."))
            {
                __instance.heldObject.Value.Stack = 2;
                __instance.heldObject.Value.Quality = SObject.lowQuality;
            }
            else
            {
                switch (dropInItem.ParentSheetIndex)
                {
                    // ostrich mayonnaise keeps giving x10 output but doesn't respect input quality without Artisan
                    case 289 when !ModEntry.ModHelper.ModRegistry.IsLoaded(
                        "ughitsmegan.ostrichmayoForProducerFrameworkMod"):
                        __instance.heldObject.Value.Quality = SObject.lowQuality;
                        break;
                    // golden mayonnaise keeps giving gives single output but keeps golden quality
                    case 928 when !ModEntry.ModHelper.ModRegistry.IsLoaded(
                        "ughitsmegan.goldenmayoForProducerFrameworkMod"):
                        __instance.heldObject.Value.Stack = 1;
                        break;
                }
            }
        }

        // if the machine doesn't belong to this player, then do nothing further
        if (Context.IsMultiplayer && __instance.owner.Value != who.UniqueMultiplayerID) return;

        if (__instance.name == "Geode Crusher" && who.HasProfession(Profession.Gemologist) &&
            (__instance.heldObject.Value.IsForagedMineral() || __instance.heldObject.Value.IsGemOrMineral()))
        {
            __instance.heldObject.Value.Quality = who.GetGemologistMineralQuality();
        }
        else if (__instance.IsArtisanMachine() && who.HasProfession(Profession.Artisan) && dropInItem is SObject dropIn)
        {
            // produce cares about input quality with low chance for upgrade
            __instance.heldObject.Value.Quality = dropIn.Quality;
            if (dropIn.Quality < SObject.bestQuality &&
                new Random(Guid.NewGuid().GetHashCode()).NextDouble() < 0.05)
                __instance.heldObject.Value.Quality +=
                    dropIn.Quality == SObject.highQuality ? 2 : 1;

            if (who.HasProfession(Profession.Artisan, true))
                __instance.MinutesUntilReady -= __instance.MinutesUntilReady / 4;
            else
                __instance.MinutesUntilReady -= __instance.MinutesUntilReady / 10;

            switch (__instance.name)
            {
                // golden mayonnaise is always iridium quality
                case "Mayonnaise Machine" when dropIn.ParentSheetIndex == 928 &&
                                               !ModEntry.ModHelper.ModRegistry.IsLoaded(
                                                   "ughitsmegan.goldenmayoForProducerFrameworkMod"):
                    __instance.heldObject.Value.Quality = SObject.bestQuality;
                    break;
                // mead cares about input honey flower type
                case "Keg" when dropIn.ParentSheetIndex == 340 && dropIn.preservedParentSheetIndex.Value > 0:
                    __instance.heldObject.Value.name = dropIn.name.Split(" Honey")[0] + " Mead";
                    __instance.heldObject.Value.preservedParentSheetIndex.Value =
                        dropIn.preservedParentSheetIndex.Value;
                    __instance.heldObject.Value.Price = dropIn.Price * 2;
                    break;
            }
        }
    }

    /// <summary>
    ///     Patch to increment Gemologist counter for geodes cracked by Geode Crusher +  + reduce prestiged Breeder
    ///     incubation time.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ObjectPerformObjectDropInActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (Game1.player.professions.Contains(<gemologist_id>))
        ///		Data.IncrementField<uint>("GemologistMineralsCollected")
        ///	After: Game1.stats.GeodesCracked++;

        var dontIncreaseGemologistCounter = iLGenerator.DefineLabel();
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Stats).PropertySetter(nameof(Stats.GeodesCracked)))
                )
                .Advance()
                .InsertProfessionCheckForLocalPlayer((int) Profession.Gemologist,
                    dontIncreaseGemologistCounter)
                .Insert(
                    new CodeInstruction(OpCodes.Ldstr, DataField.GemologistMineralsCollected.ToString()),
                    new CodeInstruction(OpCodes.Ldnull),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModData)
                            .MethodNamed(nameof(ModData.Increment), new[] {typeof(DataField), typeof(Farmer)})
                            .MakeGenericMethod(typeof(uint)))
                )
                .AddLabels(dontIncreaseGemologistCounter);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Gemologist counter increment.\nHelper returned {ex}");
            return null;
        }

        /// From: minutesUntilReady.Value /= 2
        /// To: minutesUntilReady.Value /= who.professions.Contains(100 + <breeder_id>) ? 3 : 2

        helper.ReturnToFirst();
        var i = 0;
        repeat:
        try
        {
            var notPrestigedBreeder = iLGenerator.DefineLabel();
            var resumeExecution = iLGenerator.DefineLabel();
            helper
                .FindProfessionCheck((int) Profession.Breeder, true)
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldloc_0)
                )
                .ToBufferUntil(
                    true,
                    true,
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_2)
                )
                .AddLabels(notPrestigedBreeder)
                .InsertBuffer()
                .Retreat()
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_2)
                )
                .ReplaceWith(
                    new(OpCodes.Ldc_I4_S, (int) Profession.Breeder + 100)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .SetOperand(notPrestigedBreeder)
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Ldc_I4_3),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding prestiged Breeder incubation bonus.\nHelper returned {ex}");
            return null;
        }

        // repeat injection three times
        if (++i < 3) goto repeat;

        return helper.Flush();
    }

    #endregion harmony patches
}