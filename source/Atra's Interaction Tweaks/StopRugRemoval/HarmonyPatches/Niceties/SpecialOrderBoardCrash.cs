/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StopRugRemoval
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using AtraBase.Toolkit.Reflection;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using Netcode;
using StardewValley.Menus;

namespace StopRugRemoval.HarmonyPatches.Niceties;

/// <summary>
/// Transpiles the special order board to avoid a crash possible if the orders
/// were changed between when they were originally loaded
/// and now.
/// </summary>
[HarmonyPatch(typeof(SpecialOrdersBoard))]
internal static class SpecialOrderBoardCrash
{
    private static void ShowWarning(SpecialOrder order)
        => Game1.showRedMessage(I18n.InvalidSpecialOrder(order.questKey.Value));

    [HarmonyPatch(nameof(SpecialOrdersBoard.receiveLeftClick))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(SpecialOrdersBoard).InstanceFieldNamed(nameof(SpecialOrdersBoard.leftOrder))),
                new(SpecialCodeInstructionCases.StLoc),
            })
            .Advance(2);

            CodeInstruction? leftorder = helper.CurrentInstruction.ToLdLoc();

            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Call, typeof(SpecialOrder).StaticMethodNamed(nameof(SpecialOrder.GetSpecialOrder))),
                new(OpCodes.Callvirt, typeof(NetList<SpecialOrder, NetRef<SpecialOrder>>).InstanceMethodNamed("Add")),
            })
            .Advance(1)
            .DefineAndAttachLabel(out Label leftJumpPast)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Dup),
                new(OpCodes.Brtrue_S, leftJumpPast),
                leftorder,
                new(OpCodes.Call, typeof(SpecialOrderBoardCrash).StaticMethodNamed(nameof(ShowWarning))),
                new(OpCodes.Pop),
                new(OpCodes.Pop),
                new(OpCodes.Ret),
            });

            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(SpecialOrdersBoard).InstanceFieldNamed(nameof(SpecialOrdersBoard.rightOrder))),
                new(SpecialCodeInstructionCases.StLoc),
            })
            .Advance(2);

            CodeInstruction? rightorder = helper.CurrentInstruction.ToLdLoc();

            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Call, typeof(SpecialOrder).StaticMethodNamed(nameof(SpecialOrder.GetSpecialOrder))),
                new(OpCodes.Callvirt, typeof(NetList<SpecialOrder, NetRef<SpecialOrder>>).InstanceMethodNamed("Add")),
            })
            .Advance(1)
            .DefineAndAttachLabel(out Label rightJumpPast)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Dup),
                new(OpCodes.Brtrue_S, rightJumpPast),
                rightorder,
                new(OpCodes.Call, typeof(SpecialOrderBoardCrash).StaticMethodNamed(nameof(ShowWarning))),
                new(OpCodes.Pop),
                new(OpCodes.Pop),
                new(OpCodes.Ret),
            });

            helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiling special order board to avoid crash.\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}