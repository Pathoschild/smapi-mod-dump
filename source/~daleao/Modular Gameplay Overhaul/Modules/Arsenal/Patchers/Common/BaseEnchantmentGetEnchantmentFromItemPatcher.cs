/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Common;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Arsenal.Enchantments;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseEnchantmentGetEnchantmentFromItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BaseEnchantmentGetEnchantmentFromItemPatcher"/> class.</summary>
    internal BaseEnchantmentGetEnchantmentFromItemPatcher()
    {
        this.Target = this.RequireMethod<BaseEnchantment>(nameof(BaseEnchantment.GetEnchantmentFromItem));
    }

    #region harmony patches

    /// <summary>Allow Garnet forging.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BaseEnchantmentGetEnchantmentFromItemTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (Globals.GarnetIndex.HasValue && Utility.IsNormalObjectAtParentSheetIndex(item, Globals.GarnetIndex.Value)
        // Between: Jade and Diamond enchantments...
        try
        {
            var tryGarnet = generator.DefineLabel();
            var garnetIndex = generator.DeclareLocal(typeof(int?));
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S),
                        new CodeInstruction(OpCodes.Newobj, typeof(JadeEnchantment).RequireConstructor()),
                    })
                .GetOperand(out var tryDiamond)
                .SetOperand(tryGarnet)
                .Match(new[] { new CodeInstruction(OpCodes.Ldarg_1) })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Globals).RequirePropertyGetter(nameof(Globals.GarnetIndex))),
                        new CodeInstruction(OpCodes.Stloc_S, garnetIndex),
                        new CodeInstruction(OpCodes.Ldloca_S, garnetIndex),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(int?).RequirePropertyGetter(nameof(Nullable<int>.HasValue))),
                        new CodeInstruction(OpCodes.Brfalse_S, tryDiamond),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Globals).RequirePropertyGetter(nameof(Globals.GarnetIndex))),
                        new CodeInstruction(OpCodes.Stloc_S, garnetIndex),
                        new CodeInstruction(OpCodes.Ldloca_S, garnetIndex),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(int?).RequirePropertyGetter(nameof(Nullable<int>.Value))),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Utility).RequireMethod(nameof(Utility.IsNormalObjectAtParentSheetIndex))),
                        new CodeInstruction(OpCodes.Brfalse_S, tryDiamond),
                        new CodeInstruction(OpCodes.Newobj, typeof(GarnetEnchantment).RequireConstructor()),
                        new CodeInstruction(OpCodes.Ret),
                    },
                    new[] { tryGarnet });
        }
        catch (Exception ex)
        {
            Log.E($"Failed allowing Garnet forging.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
