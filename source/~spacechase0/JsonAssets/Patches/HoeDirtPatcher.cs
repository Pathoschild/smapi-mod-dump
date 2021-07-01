/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using Spacechase.Shared.Harmony;
using SpaceShared;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;

namespace JsonAssets.Patches
{
    /// <summary>Applies Harmony patches to <see cref="HoeDirt"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class HoeDirtPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<HoeDirt>(nameof(HoeDirt.dayUpdate)),
                transpiler: this.GetHarmonyMethod(nameof(Transpile_DayUpdate))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method which transpiles <see cref="HoeDirt.dayUpdate"/> to remove the <see cref="HoeDirt.destroyCrop"/> call.</summary>
        /// <remarks>Withering isn't needed here since crops will wither separately if needed, so this simplifies Json Assets' winter crop logic elsewhere.</remarks>
        private static IEnumerable<CodeInstruction> Transpile_DayUpdate(IEnumerable<CodeInstruction> insns)
        {
            bool happened = false;
            var newInsns = new List<CodeInstruction>();
            foreach (var instr in insns)
            {
                if ((instr.opcode == OpCodes.Call || instr.opcode == OpCodes.Callvirt) && (instr.operand as MethodInfo)?.Name == nameof(HoeDirt.destroyCrop))
                {
                    for (int i = 0; i < 4; i++) // remove the four args to the destroyCrop method
                        newInsns.Add(new CodeInstruction(OpCodes.Pop));
                    happened = true;
                    continue;
                }
                newInsns.Add(instr);
            }

            if (!happened)
                Log.Error($"{nameof(Transpile_DayUpdate)} patching failed: couldn't find {nameof(HoeDirt.destroyCrop)} call in the {nameof(HoeDirt)}.{nameof(HoeDirt.dayUpdate)} method.");

            return newInsns;
        }
    }
}
