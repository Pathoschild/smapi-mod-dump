/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/HighlightEmptyMachines
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using AtraBase.Toolkit;
using AtraBase.Toolkit.Reflection;
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
        if (ModEntry.Config.VanillaMachines.TryGetValue((VanillaMachinesEnum)obj.ParentSheetIndex, out bool val) && val)
        {
            if (obj is Cask cask && Game1.currentLocation is GameLocation loc && !cask.IsValidCaskLocation(loc))
            {
                return ModEntry.Config.InvalidColor;
            }
            return ModEntry.Config.EmptyColor;
        }
        else if (PFMMachineHandler.ValidMachines.TryGetValue(obj.ParentSheetIndex, out PFMMachineStatus status))
        {
            return status switch
            {
                PFMMachineStatus.Invalid => ModEntry.Config.InvalidColor,
                PFMMachineStatus.Enabled => ModEntry.Config.EmptyColor,
                _ => Color.White,
            };
        }
        return Color.White;
    }

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
                new (OpCodes.Ldarg_0),
                new (OpCodes.Ldfld, typeof(SObject).InstanceFieldNamed(nameof(SObject.bigCraftable))),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new (OpCodes.Ldarg_0),
                new (OpCodes.Ldfld, typeof(Item).InstanceFieldNamed(nameof(Item.parentSheetIndex))),
                new (OpCodes.Call),
                new (OpCodes.Ldc_I4, 272),
                new (OpCodes.Bne_Un),
            })
            .Advance(4)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .FindNext(new CodeInstructionWrapper[]
            {
                new (OpCodes.Call, typeof(Color).StaticPropertyNamed(nameof(Color.White)).GetGetMethod()),
            })
            .GetLabels(out IList<Label> colorLabels, clear: true)
            .ReplaceInstruction(OpCodes.Call, typeof(SObjectDrawTranspiler).StaticMethodNamed(nameof(SObjectDrawTranspiler.BigCraftableNeedsInputLayerColor)))
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
            }, withLabels: colorLabels);
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling SObject.draw\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}