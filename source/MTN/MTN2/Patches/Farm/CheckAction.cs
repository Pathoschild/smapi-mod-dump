using Harmony;
using MTN2.Management;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace MTN2.Patches.FarmPatches
{
    /// <summary>
    /// REASON FOR PATCHING: To adjust for the X, Y locational checks of the starting
    /// shipping bin, for custom farm maps may have moved it.
    /// 
    /// 
    /// Patches the method Farm.checkAction to adjust for the movement
    /// of the starting shipping bin (the bin that is not classified as a building).
    /// </summary>
    public class checkActionPatch
    {
        private static ICustomManager customManager;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="customManager">The class controlling information pertaining to the customs (and the loaded customs).</param>
        public checkActionPatch(ICustomManager customManager) {
            checkActionPatch.customManager = customManager;
        }

        /// <summary>
        /// Transpiles the CLI to inject calls to <see cref="CustomManager.ShippingBinX"/> and <see cref="CustomManager.ShippingBinY"/> as well as their offsets.
        /// This removes the hardcoded values for the initial shipping bin.
        /// (The only shipping bin that is not of class building)
        /// </summary>
        /// <param name="instructions">Code Instructions (in CLI)</param>
        /// <returns></returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++) {
                if (CheckForLdfld(codes[i], "X") && CheckOtherLines(codes, i)) {
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(checkActionPatch), "customManager")));
                    codes[i + 2] = new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(CustomManager), "ShippingBinX").GetGetMethod());
                    codes.Insert(i + 6, new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(checkActionPatch), "customManager")));
                    codes[i + 7] = new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(CustomManager), "ShippingBinXOffSet").GetGetMethod());
                    codes.Insert(i + 11, new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(checkActionPatch), "customManager")));
                    codes[i + 12] = new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(CustomManager), "ShippingBinY").GetGetMethod());
                    codes.Insert(i + 16, new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(checkActionPatch), "customManager")));
                    codes[i + 17] = new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(CustomManager), "ShippingBinYOffSet").GetGetMethod());
                }
            }

            return codes.AsEnumerable();
        }

        private static bool CheckForLdfld(CodeInstruction code, string name) {
            return (code.opcode == OpCodes.Ldfld && (code.operand as FieldInfo).Name == name);
        }

        private static bool CheckForLdc_i4_s(CodeInstruction code, sbyte value) {
            return (code.opcode == OpCodes.Ldc_I4_S && (sbyte)code.operand == value);
        }

        private static bool CheckOtherLines(List<CodeInstruction> codes, int p) {
            if (!CheckForLdc_i4_s(codes[p + 1], 71)) return false;
            if (!CheckForLdfld(codes[p + 4], "X")) return false;
            if (!CheckForLdc_i4_s(codes[p + 5], 72)) return false;
            if (!CheckForLdfld(codes[p + 8], "Y")) return false;
            if (!CheckForLdc_i4_s(codes[p + 9], 13)) return false;
            if (!CheckForLdfld(codes[p + 12], "Y")) return false;
            if (!CheckForLdc_i4_s(codes[p + 13], 14)) return false;

            return true;
        }
    }
}
