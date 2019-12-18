using Harmony;
using Microsoft.Xna.Framework;
using MTN2.Management;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.FarmerPatch {
    public class GetMailboxPositionPatch {
        private static ICustomManager customManager;

        public GetMailboxPositionPatch(ICustomManager customManager) {
            GetMailboxPositionPatch.customManager = customManager;
        }

        public static bool Prefix() {
            if (!customManager.Canon) return false;
            return true;
        }

        //public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        //    var codes = new List<CodeInstruction>(instructions);

        //    for (int i = 0; i < codes.Count - 1; i++) {
        //        if (CheckForLdc_i4_s(codes[i], 68) && CheckForLdc_i4_s(codes[i + 1], 16)) {
        //            List<Label> Labels = codes[i].labels;
        //            codes.Insert(i, new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(GetMailboxPositionPatch), "customManager")) { labels = Labels });
        //            codes[i + 1] = new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(CustomManager), "MailBoxX").GetGetMethod());
        //            codes.Insert(i + 2, new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(GetMailboxPositionPatch), "customManager")));
        //            codes[i + 3] = new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(CustomManager), "MailBoxY").GetGetMethod());
        //        }
        //    }

        //    return codes.AsEnumerable();
        //}

        private static bool CheckForLdc_i4_s(CodeInstruction code, sbyte value) {
            return (code.opcode == OpCodes.Ldc_I4_S && (sbyte)code.operand == value);
        }

        public static void Postfix(Farmer __instance, ref Point __result) {
            if (customManager.Canon) return;
            
            foreach (Building b in Game1.getFarm().buildings) {
                if (b.isCabin && b.nameOfIndoors == __instance.homeLocation) {
                    __result = b.getMailboxPosition();
                    return;
                }
            }

            __result = customManager.LoadedFarm.MailBox.PointOfInteraction.ToPoint();
        }
    }
}
