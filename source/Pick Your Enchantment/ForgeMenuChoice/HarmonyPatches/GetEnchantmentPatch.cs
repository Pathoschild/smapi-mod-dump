/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/ForgeMenuChoice
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using AtraBase.Toolkit.Reflection;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;

namespace ForgeMenuChoice.HarmonyPatches;

/// <summary>
/// Patch that overrides which enchantment is applied when forging.
/// This has to be a transpiler - I need only this call to GetEnchantmentFromItem to be different.
/// </summary>
[HarmonyPatch(typeof(Tool))]
internal static class GetEnchantmentPatch
{
    /// <summary>
    /// Function that substitutes in an enchantment.
    /// </summary>
    /// <param name="base_item">Tool.</param>
    /// <param name="item">Thing to enchant with.</param>
    /// <returns>Enchanment to substitute in.</returns>
    public static BaseEnchantment SubstituteEnchantment(Item base_item, Item item)
    {
        try
        {
            if (Utility.IsNormalObjectAtParentSheetIndex(item, 74) && ForgeMenuPatches.CurrentSelection is not null)
            {
                BaseEnchantment output = ForgeMenuPatches.CurrentSelection;
                ForgeMenuPatches.TrashMenu();
                return output;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in forcing selection of enchantment.\n\n{ex}", LogLevel.Error);
        }
        return BaseEnchantment.GetEnchantmentFromItem(base_item, item);
    }

    [HarmonyPatch(nameof(Tool.Forge))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindFirst(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_0),
                new(SpecialCodeInstructionCases.LdArg),
                new(OpCodes.Call, typeof(BaseEnchantment).StaticMethodNamed(nameof(BaseEnchantment.GetEnchantmentFromItem))),
                new(SpecialCodeInstructionCases.StLoc),
                new(SpecialCodeInstructionCases.LdLoc),
            })
            .Advance(2)
            .ReplaceOperand(typeof(GetEnchantmentPatch).StaticMethodNamed(nameof(GetEnchantmentPatch.SubstituteEnchantment)));
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into errors transpiling Tool.Forge to use selection.\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
}