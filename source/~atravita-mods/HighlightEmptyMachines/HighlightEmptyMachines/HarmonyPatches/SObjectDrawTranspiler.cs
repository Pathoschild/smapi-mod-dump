/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using AtraBase.Toolkit;
using AtraCore.Framework.ReflectionManager;
using AtraShared.ConstantsAndEnums;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using HighlightEmptyMachines.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;

namespace HighlightEmptyMachines.HarmonyPatches;

/// <summary>
/// Transpilers against SObject's draw to color bigcraftables if they're empty or in an invalid position.
/// </summary>
[HarmonyPatch(typeof(SObject))]
internal class SObjectDrawTranspiler
{
    [MethodImpl(TKConstants.Hot)]
    private static Color BigCraftableNeedsInputLayerColor(SObject obj)
    {
        if (obj.heldObject.Value is not null)
        {
            return Color.White;
        }
        if (PFMMachineHandler.ValidMachines.TryGetValue(obj.ParentSheetIndex, out MachineStatus status))
        {
            return status switch
            {
                MachineStatus.Invalid => ModEntry.Config.InvalidColor,
                MachineStatus.Enabled => ModEntry.Config.EmptyColor,
                _ => Color.White,
            };
        }
        else if (ModEntry.Config.VanillaMachines.TryGetValue((VanillaMachinesEnum)obj.ParentSheetIndex, out bool val) && val)
        {
            if (obj is Cask cask && Game1.currentLocation is GameLocation loc && !cask.IsValidCaskLocation(loc))
            {
                return ModEntry.Config.InvalidColor;
            }
            else if (obj.ParentSheetIndex == (int)VanillaMachinesEnum.BeeHouse && BeehouseHandler.Status.Value == MachineStatus.Invalid)
            {
                return ModEntry.Config.InvalidColor;
            }
            return ModEntry.Config.EmptyColor;
        }
        return Color.White;
    }

    [MethodImpl(TKConstants.Hot)]
    private static bool ShouldDisablePulsing() => ModEntry.Config.DisablePulsing;

#pragma warning disable SA1116 // Split parameters should start on line after declaration. Reviewed
    [HarmonyPatch(nameof(SObject.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) })]
    [SuppressMessage("SMAPI.CommonErrors", "AvoidNetField:Avoid Netcode types when possible", Justification = "Only used for matching.")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                OpCodes.Ldarg_0,
                (OpCodes.Ldfld, typeof(SObject).GetCachedField(nameof(SObject.bigCraftable), ReflectionCache.FlagTypes.InstanceFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            { // Vector2 vector = this.getScale();
                OpCodes.Ldarg_0,
                (OpCodes.Callvirt, typeof(SObject).GetCachedMethod(nameof(SObject.getScale), ReflectionCache.FlagTypes.InstanceFlags)),
                SpecialCodeInstructionCases.StLoc,
            })
            .Push() // edit to Vector2 vector = ShouldDisablePulsing ? Vector2.Zero : this.getScale();
            .GetLabels(out IList<Label>? pulseLabels)
            .Advance(2)
            .DefineAndAttachLabel(out Label nopulseJump)
            .Pop()
            .DefineAndAttachLabel(out Label pulseJump)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Call, typeof(SObjectDrawTranspiler).GetCachedMethod(nameof(ShouldDisablePulsing), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Brfalse, pulseJump),
                new(OpCodes.Call, typeof(Vector2).GetCachedProperty(nameof(Vector2.Zero), ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
                new (OpCodes.Br_S, nopulseJump),
            }, withLabels: pulseLabels)
            .FindNext(new CodeInstructionWrapper[]
            {
                OpCodes.Ldarg_0,
                (OpCodes.Ldfld, typeof(Item).GetCachedField(nameof(Item.parentSheetIndex), ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Call,
                (OpCodes.Ldc_I4, 272),
                OpCodes.Bne_Un,
            })
            .Advance(4)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .FindNext(new CodeInstructionWrapper[]
            {
                (OpCodes.Call, typeof(Color).GetCachedProperty(nameof(Color.White), ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
            })
            .GetLabels(out IList<Label> colorLabels, clear: true)
            .ReplaceInstruction(OpCodes.Call, typeof(SObjectDrawTranspiler).GetCachedMethod(nameof(BigCraftableNeedsInputLayerColor), ReflectionCache.FlagTypes.StaticFlags))
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
            }, withLabels: colorLabels);

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling {original.FullDescription()}\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}