using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Harmony;
using StardewValley;

namespace WindowResize {

	[HarmonyPatch(typeof(Game1))]
	[HarmonyPatch("Window_ClientSizeChanged")]
	class WindowResizePatch {
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			//FileLog.Log("Patching Window_ClientSizeChanged Transpiler");
			List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
			for (int i = 0; i < codes.Count; i++) {
				//Find first time 1280 is used
				if (codes[i].operand is int && (int)codes[i].operand == 1280){
					//FileLog.Log("Found operand == 1280");
					//The next if branches unconditionally cause I'm lazy
					codes[i + 1].opcode = OpCodes.Br_S;

					for (; i < codes.Count; i++) {
						//Find when 720 is used next
						if (codes[i].operand is int && (int) codes[i].operand == 720) {
							//FileLog.Log("Found operand == 720");
							//The next if branches unconditionally cause I'm lazy
							codes[i + 1].opcode = OpCodes.Br_S;
							break;
						}
					}
					break;
				}
			}

			return codes.AsEnumerable();
		}
	}
}
