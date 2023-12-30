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
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SteamDew.Patches.PSMultiplayer {

public class InitServer : Patcher {

public InitServer()
{
	MethodInfo m = SteamDew.SMultiplayerType.GetMethod(
		"InitServer", 
		BindingFlags.Public | BindingFlags.Instance
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

private static Server InitCustomServer_4_0_0(Server server, object multiplayer, Action<IncomingMessage, Action<OutgoingMessage>, Action> onProcessingMessage)
{
	if (server is SDKs.Decoys.DGalaxyNetServer) {
		SteamDew.Log("Intercepted the decoy Galaxy Server");

		SDKs.Decoys.DGalaxyNetServer galaxyServer;
		galaxyServer = (server as SDKs.Decoys.DGalaxyNetServer);

		return (Server) Activator.CreateInstance(
			SteamDew.SGalaxyNetServerType,
			new object[] {
				galaxyServer.iGameServer,
				multiplayer,
				onProcessingMessage
			}
		);
	}

	if (server is SDKs.Decoys.DSteamDewServer) {
		SteamDew.Log("Intercepted the decoy SteamDew Server");

		SDKs.Decoys.DSteamDewServer steamServer;
		steamServer = (server as SDKs.Decoys.DSteamDewServer);

		return new SDKs.SteamDewServer(
			steamServer.iGameServer,
			multiplayer,
			onProcessingMessage
		);
	}

	return server;
}

private static Server InitCustomServer_3_18_6(IGameServer gameServer, Server server, object multiplayer, Action<IncomingMessage, Action<OutgoingMessage>, Action> onProcessingMessage)
{
	return InitServer.InitCustomServer_4_0_0(server, multiplayer, onProcessingMessage);
}

private static IEnumerable<CodeInstruction> PatchTranspiler_3_18_6(IEnumerable<CodeInstruction> instructions)
{
	SteamDew.Log("Patching SMultiplayer::InitServer(...) for SMAPI 3.18.6");

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
			if (s != "Can't initialize GOG networking client: the required 'gameServer' field wasn't found.") {
				continue;
			}
			state = 1;
			continue;
		case 1:
			if (instr.opcode != OpCodes.Stloc_1) {
				continue;
			}
			instr.opcode = OpCodes.Nop;
			instr.operand = null;
			state = 2;
			continue;
		case 2:
			if (instr.opcode != OpCodes.Ldloc_1) {
				continue;
			}
			instr.opcode = OpCodes.Ldarg_1;
			instr.operand = null;
			state = 3;
			continue;
		case 3:
			if (instr.opcode != OpCodes.Newobj) {
				continue;
			}
			if (!(instr.operand is ConstructorInfo)) {
				continue;
			}
			ConstructorInfo c = instr.operand as ConstructorInfo;
			if (!c.DeclaringType.Equals(SteamDew.SGalaxyNetServerType)) {
				continue;
			}
			instr.opcode = OpCodes.Call;
			instr.operand = typeof(InitServer).GetMethod(
				"InitCustomServer_3_18_6", 
				BindingFlags.NonPublic | BindingFlags.Static
			);
			return instructions;
		}
	}
	return instructions;
}

private static IEnumerable<CodeInstruction> PatchTranspiler_4_0_0(IEnumerable<CodeInstruction> instructions)
{
	SteamDew.Log("Patching SMultiplayer::InitServer(...) for SMAPI 4.0.0");

	int state = 0;

	foreach (CodeInstruction instr in instructions) {
		switch (state) {
		case 0:
			if (instr.opcode != OpCodes.Ret) {
				continue;
			}
			state = 1;
			continue;
		case 1:
			if (instr.opcode != OpCodes.Ldfld) {
				continue;
			}
			if (!(instr.operand is FieldInfo)) {
				continue;
			}
			FieldInfo f = instr.operand as FieldInfo;
			if (f.DeclaringType != typeof(Server)) {
				continue;
			}
			if (f.FieldType != typeof(IGameServer)) {
				continue;
			}
			if (f.Name != "gameServer") {
				continue;
			}
			instr.opcode = OpCodes.Nop;
			instr.operand = null;
			state = 2;
			continue;
		case 2:
			if (instr.opcode != OpCodes.Newobj) {
				continue;
			}
			if (!(instr.operand is ConstructorInfo)) {
				continue;
			}
			ConstructorInfo c = instr.operand as ConstructorInfo;
			if (!c.DeclaringType.Equals(SteamDew.SGalaxyNetServerType)) {
				continue;
			}
			instr.opcode = OpCodes.Call;
			instr.operand = typeof(InitServer).GetMethod(
				"InitCustomServer_4_0_0", 
				BindingFlags.NonPublic | BindingFlags.Static
			);
			return instructions;
		}
	}
	return instructions;
}

private static IEnumerable<CodeInstruction> PatchTranspiler(IEnumerable<CodeInstruction> instructions)
{
	foreach (CodeInstruction instr in instructions) {
		if (instr.opcode != OpCodes.Ldfld) {
			continue;
		}
		if (!(instr.operand is FieldInfo)) {
			continue;
		}
		FieldInfo f = instr.operand as FieldInfo;
		if (f.DeclaringType != typeof(Server)) {
			continue;
		}
		if (f.FieldType != typeof(IGameServer)) {
			continue;
		}
		if (f.Name != "gameServer") {
			continue;
		}
		return InitServer.PatchTranspiler_4_0_0(instructions);
	}
	return InitServer.PatchTranspiler_3_18_6(instructions);
}

} /* class InitServer */

} /* namespace SteamDew.Patcher.PSMultiplayer */