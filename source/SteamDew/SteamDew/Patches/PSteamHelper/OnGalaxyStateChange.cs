/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/myuusubi/SteamDew
**
*************************************************/

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SteamDew.Patches.PSteamHelper {

public class OnGalaxyStateChange : Patcher {

public OnGalaxyStateChange()
{
	MethodInfo m = typeof(StardewValley.SDKs.SteamHelper).GetMethod(
		"onGalaxyStateChange", 
		BindingFlags.NonPublic | BindingFlags.Instance
	);

	this.DeclaringType = m.DeclaringType;
	this.Name = m.Name;

	this.Transpiler = new HarmonyMethod(
		this.GetType().GetMethod(
			"PatchTranspiler", 
			BindingFlags.NonPublic | BindingFlags.Static
		)
	);
}

private static IEnumerable<CodeInstruction> PatchTranspiler(IEnumerable<CodeInstruction> instructions)
{
	int state = 0;
	foreach (CodeInstruction instr in instructions) {
		switch (state) {
		case 0:
			if (instr.opcode != OpCodes.Ldstr) {
				continue;
			}
			if (!(instr.operand is string)) {
				continue;
			}
			string s = instr.operand as string;
			if (s != "Galaxy logged on") {
				continue;
			}
			instr.operand = "Galaxy Logged On. Injecting SteamDew NetHelper...";
			state = 1;
			break;
		case 1:
			if (instr.opcode != OpCodes.Newobj) {
				break;
			}
			if (!(instr.operand is ConstructorInfo)) {
				break;
			}
			ConstructorInfo c = instr.operand as ConstructorInfo;
			if (c.DeclaringType.ToString() != "StardewValley.SDKs.SteamNetHelper") {
				break;
			}
			instr.operand = typeof(SDKs.SteamDewNetHelper).GetConstructor(new Type[0]);
			state = 2;
			break;
		}
	}
	return instructions;
}

} /* class OnGalaxyStateChange */

} /* namespace SteamDew.Patcher.PSteamHelper */