/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using Spacechase.Shared.Harmony;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace BuildableLocationsFramework.Patches
{
    /// <summary>Applies Harmony patches to <see cref="PurchaseAnimalsMenu"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class PurchaseAnimalsMenuPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<PurchaseAnimalsMenu>(nameof(PurchaseAnimalsMenu.performHoverAction)),
                transpiler: this.GetHarmonyMethod(nameof(Transpile_PerformHoverAction))
            );

            harmony.Patch(
                original: this.RequireMethod<PurchaseAnimalsMenu>(nameof(PurchaseAnimalsMenu.receiveLeftClick)),
                transpiler: this.GetHarmonyMethod(nameof(Transpile_ReceiveLeftClick))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method which transpiles <see cref="PurchaseAnimalsMenu.performHoverAction"/>.</summary>
        private static IEnumerable<CodeInstruction> Transpile_PerformHoverAction(ILGenerator gen, MethodBase original, IEnumerable<CodeInstruction> insns)
        {
            return PurchaseAnimalsMenuPatcher.Transpile(gen, original, insns);
        }

        /// <summary>The method which transpiles <see cref="PurchaseAnimalsMenu.receiveLeftClick"/>.</summary>
        private static IEnumerable<CodeInstruction> Transpile_ReceiveLeftClick(ILGenerator gen, MethodBase original, IEnumerable<CodeInstruction> insns)
        {
            return PurchaseAnimalsMenuPatcher.Transpile(gen, original, insns);
        }

        private static IEnumerable<CodeInstruction> Transpile(ILGenerator gen, MethodBase original, IEnumerable<CodeInstruction> insns)
        {
            var stage1 = CarpenterMenuPatcher.Transpile(gen, original, insns);

            var ret = new List<CodeInstruction>();

            foreach (var insn in stage1)
            {
                if (insn.opcode == OpCodes.Isinst && (Type)insn.operand == typeof(Farm))
                {
                    insn.operand = typeof(BuildableGameLocation);
                }
                ret.Add(insn);
            }

            return ret;
        }
    }
}
