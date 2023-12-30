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
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SteamDew.Patches.PGameServer {

public class Ctor : Patcher {

public Ctor()
{
	this.DeclaringType = typeof(GameServer);
	this.Name = null;
	this.Parameters = new Type[] { typeof(bool) };

	this.Transpiler = new HarmonyMethod(
		this.GetType().GetMethod(
			"PatchTranspiler", 
			BindingFlags.NonPublic | BindingFlags.Static
		)
	);
}

private static void AddServers(List<Server> servers, GameServer gameServer) {
	SteamDew.Log("Hooked GameServer Constructor. Injecting our servers.");

	Multiplayer multiplayer = SteamDew.GetGameMultiplayer();
	if (multiplayer == null) {
		SteamDew.Log($"Could not inject servers: Game1.multiplayer was null");
		return;
	}

	servers.Add(multiplayer.InitServer(new SDKs.Decoys.DSteamDewServer(gameServer)));
	servers.Add(multiplayer.InitServer(new SDKs.Decoys.DGalaxyNetServer(gameServer)));
}

private static IEnumerable<CodeInstruction> PatchTranspiler(IEnumerable<CodeInstruction> instructions)
{
	int state = 0;
	foreach (CodeInstruction instr in instructions) {
		switch (state) {
			case 0:
				if (instr.opcode != OpCodes.Brfalse_S) {
					continue;
				}
				state = 1;
				continue;
			case 1:
			case 2:
			case 3:
			case 4:
				if (state < 3 && instr.opcode == OpCodes.Ldarg_0) {
					++state;
					continue;
				} else if (instr.opcode != OpCodes.Call && instr.opcode != OpCodes.Callvirt) {
					continue;
				}
				if (state == 3) {
					instr.opcode = OpCodes.Call;
					instr.operand = typeof(Ctor).GetMethod(
						"AddServers", 
						BindingFlags.NonPublic | BindingFlags.Static
					);
					state = 4;
					continue;
				}
				instr.opcode = OpCodes.Nop;
				instr.operand = null;
				continue;
		}
	}
	return instructions;
}

} /* class Ctor */

} /* namespace SteamDew.Patcher.PGameServer */