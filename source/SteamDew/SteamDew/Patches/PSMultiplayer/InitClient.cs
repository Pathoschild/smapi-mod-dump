/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/myuusubi/SteamDew
**
*************************************************/

using Galaxy.Api;
using HarmonyLib;
using StardewValley.Network;
using StardewValley.SDKs;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SteamDew.Patches.PSMultiplayer {

public class InitClient : Patcher {

public InitClient()
{
	MethodInfo m = SteamDew.SMultiplayerType.GetMethod(
		"InitClient", 
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

private static Client InitCustomClient_4_0_0(Client client, Action<IncomingMessage, Action<OutgoingMessage>, Action> onProcessingMessage, Action<OutgoingMessage, Action<OutgoingMessage>, Action> onSendingMessage)
{
	if (client is SDKs.Decoys.DGalaxyNetClient) {
		SteamDew.Log("Intercepted the decoy Galaxy Client");

		SDKs.Decoys.DGalaxyNetClient galaxyClient;
		galaxyClient = (client as SDKs.Decoys.DGalaxyNetClient);

		GalaxyID lobby = galaxyClient.LobbyID;

		return (Client) Activator.CreateInstance(
			SteamDew.SGalaxyNetClientType,
			new object[] {
				lobby,
				onProcessingMessage,
				onSendingMessage
			}
		);
	}

	if (client is SDKs.Decoys.DSteamDewClient) {
		SteamDew.Log("Intercepted the decoy SteamDew Client");

		SDKs.Decoys.DSteamDewClient steamDewClient;
		steamDewClient = (client as SDKs.Decoys.DSteamDewClient);

		return new SDKs.SteamDewClient(
			steamDewClient.LobbyID,
			steamDewClient.State,
			steamDewClient.HostID,
			onProcessingMessage,
			onSendingMessage
		);
	}

	if (client is SDKs.Decoys.DInviteClient) {
		SteamDew.Log("Intercepted the decoy Invite Client");

		SDKs.Decoys.DInviteClient inviteClient;
		inviteClient = (client as SDKs.Decoys.DInviteClient);

		GalaxyID lobby = inviteClient.LobbyID;

		return new SDKs.InviteClient(lobby, onProcessingMessage, onSendingMessage);
	}

	return client;
}

private static Client InitCustomClient_3_18_6(GalaxyID address, Client client, Action<IncomingMessage, Action<OutgoingMessage>, Action> onProcessingMessage, Action<OutgoingMessage, Action<OutgoingMessage>, Action> onSendingMessage)
{
	return InitClient.InitCustomClient_4_0_0(client, onProcessingMessage, onSendingMessage);
}

private static IEnumerable<CodeInstruction> PatchTranspiler_3_18_6(IEnumerable<CodeInstruction> instructions)
{
	SteamDew.Log("Patching SMultiplayer::InitClient(...) for SMAPI 3.18.6");

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
			if (s != "Can't initialize GOG networking client: no valid address found.") {
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
			if (!c.DeclaringType.Equals(SteamDew.SGalaxyNetClientType)) {
				continue;
			}
			instr.opcode = OpCodes.Call;
			instr.operand = typeof(InitClient).GetMethod(
				"InitCustomClient_3_18_6", 
				BindingFlags.NonPublic | BindingFlags.Static
			);
			return instructions;
		}
	}
	return instructions;
}

private static IEnumerable<CodeInstruction> PatchTranspiler_4_0_0(IEnumerable<CodeInstruction> instructions)
{
	SteamDew.Log("Patching SMultiplayer::InitClient(...) for SMAPI 4.0.0");

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
			if (instr.opcode != OpCodes.Ldloc_1) {
				continue;
			}
			instr.opcode = OpCodes.Ldarg_1;
			instr.operand = null;
			state = 2;
			continue;
		case 2:
			if (instr.opcode != OpCodes.Ldfld) {
				continue;
			}
			if (!(instr.operand is FieldInfo)) {
				continue;
			}
			FieldInfo f = instr.operand as FieldInfo;
			if (f.DeclaringType != typeof(GalaxyNetClient)) {
				continue;
			}
			if (f.FieldType != typeof(GalaxyID)) {
				continue;
			}
			if (f.Name != "lobbyId") {
				continue;
			}
			instr.opcode = OpCodes.Nop;
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
			if (!c.DeclaringType.Equals(SteamDew.SGalaxyNetClientType)) {
				continue;
			}
			instr.opcode = OpCodes.Call;
			instr.operand = typeof(InitClient).GetMethod(
				"InitCustomClient_4_0_0", 
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
		if (f.DeclaringType != typeof(GalaxyNetClient)) {
			continue;
		}
		if (f.FieldType != typeof(GalaxyID)) {
			continue;
		}
		if (f.Name != "lobbyId") {
			continue;
		}
		return InitClient.PatchTranspiler_4_0_0(instructions);
	}
	return InitClient.PatchTranspiler_3_18_6(instructions);
}

} /* class InitClient */

} /* namespace SteamDew.Patcher.PSMultiplayer */