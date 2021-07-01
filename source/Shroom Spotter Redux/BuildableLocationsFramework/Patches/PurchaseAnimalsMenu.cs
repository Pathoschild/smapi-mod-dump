/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace BuildableLocationsFramework.Patches
{
    public static class PurchaseAnimalsMenuTranspileCommon
    {
        public static IEnumerable<CodeInstruction> Transpiler( ILGenerator gen, MethodBase original, IEnumerable<CodeInstruction> insns )
        {
            var stage1 = CarpenterMenuTranspileCommon.Transpiler( gen, original, insns );

            var ret = new List<CodeInstruction>();

            foreach ( var insn in stage1 )
            {
                if ( insn.opcode == OpCodes.Isinst && ( Type ) insn.operand == typeof( Farm ) )
                {
                    insn.operand = typeof( BuildableGameLocation );
                }
                ret.Add( insn );
            }

            return ret;
        }
    }

    [HarmonyPatch( typeof( PurchaseAnimalsMenu ), "performHoverAction" )]
    public static class PurchaseAnimalsMenuHoverPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler( ILGenerator gen, MethodBase original, IEnumerable<CodeInstruction> insns )
        {
            return PurchaseAnimalsMenuTranspileCommon.Transpiler( gen, original, insns );
        }
    }

    [HarmonyPatch( typeof( PurchaseAnimalsMenu ), "receiveLeftClick" )]
    public static class PurchaseAnimalsMenuLeftClickPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler( ILGenerator gen, MethodBase original, IEnumerable<CodeInstruction> insns )
        {
            return PurchaseAnimalsMenuTranspileCommon.Transpiler( gen, original, insns );
        }
    }
}
