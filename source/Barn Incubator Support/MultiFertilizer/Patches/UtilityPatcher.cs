/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using Microsoft.Xna.Framework;
using Spacechase.Shared.Harmony;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace MultiFertilizer.Patches
{
    /// <summary>Applies Harmony patches to <see cref="Utility"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class UtilityPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<Utility>(nameof(Utility.tryToPlaceItem)),
                transpiler: this.GetHarmonyMethod(nameof(Transpile_TryToPlaceItem))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method which transpiles <see cref="Utility.tryToPlaceItem"/>.</summary>
        private static IEnumerable<CodeInstruction> Transpile_TryToPlaceItem(ILGenerator gen, MethodBase original, IEnumerable<CodeInstruction> insns)
        {
            // TODO: Learn how to use ILGenerator

            bool stopCaring = false;
            int fertCategoryCounter = 0;

            // When we find the -19, after the next instruction:
            // Place our patched section function call. If it returns true, return from the function false.
            // Then skip the old section

            var newInsns = new List<CodeInstruction>();
            foreach (var insn in insns)
            {
                if (stopCaring)
                {
                    newInsns.Add(insn);
                    continue;
                }

                if (insn.opcode == OpCodes.Ldc_I4_S && (sbyte)insn.operand == -19)
                {
                    newInsns.Add(insn);
                    fertCategoryCounter++;
                }
                else if (fertCategoryCounter == 1)
                {
                    newInsns.Add(insn);

                    var branchPastOld = new CodeInstruction(OpCodes.Br, insn.operand);
                    branchPastOld.labels.Add(gen.DefineLabel());

                    newInsns.Add(new CodeInstruction(OpCodes.Ldarg_0));
                    newInsns.Add(new CodeInstruction(OpCodes.Ldarg_1));
                    newInsns.Add(new CodeInstruction(OpCodes.Ldarg_2));
                    newInsns.Add(new CodeInstruction(OpCodes.Ldarg_3));
                    newInsns.Add(new CodeInstruction(OpCodes.Call, PatchHelper.RequireMethod<UtilityPatcher>(nameof(TryToPlaceItemLogic))));

                    newInsns.Add(new CodeInstruction(OpCodes.Brfalse, branchPastOld.labels[0]));

                    newInsns.Add(new CodeInstruction(OpCodes.Ldc_I4_0));
                    newInsns.Add(new CodeInstruction(OpCodes.Ret));

                    newInsns.Add(branchPastOld);

                    ++fertCategoryCounter;
                    stopCaring = true;
                }
                else
                    newInsns.Add(insn);
            }

            return newInsns;
        }

        private static bool TryToPlaceItemLogic(GameLocation location, Item item, int x, int y)
        {
            string key = item.ParentSheetIndex switch
            {
                368 => Mod.KeyFert,
                369 => Mod.KeyFert,
                919 => Mod.KeyFert,
                370 => Mod.KeyRetain,
                371 => Mod.KeyRetain,
                920 => Mod.KeyRetain,
                465 => Mod.KeySpeed,
                466 => Mod.KeySpeed,
                918 => Mod.KeySpeed,
                _ => ""
            };

            Vector2 tileLocation = new Vector2(x / 64, y / 64);
            if (!location.terrainFeatures.TryGetValue(tileLocation, out TerrainFeature feature) || feature is not HoeDirt dirt)
                return true;

            if (dirt.fertilizer.Value != 0)
            {
                if (dirt.modData.ContainsKey(key))
                {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13916-2"));
                }
                return true;
            }
            return false;
        }
    }
}
