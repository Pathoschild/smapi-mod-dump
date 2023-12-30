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
using Steamworks;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SteamDew.Patches.PFarmhandMenu {

public class Update : Patcher {

public Update()
{
	this.DeclaringType = typeof(StardewValley.Menus.FarmhandMenu);
	this.Name = nameof(StardewValley.Menus.FarmhandMenu.update);

	this.Transpiler = new HarmonyMethod(
		this.GetType().GetMethod(
			"PatchTranspiler", 
			BindingFlags.NonPublic | BindingFlags.Static
		)
	);
}

private static void ReceiveMessages(StardewValley.Menus.FarmhandMenu menu) {
	if (!(menu.client is SDKs.InviteClient)) {
		menu.client.receiveMessages();
		return;
	}

	SDKs.InviteClient inviteClient = menu.client as SDKs.InviteClient;

	StardewValley.Multiplayer multiplayer = null;

	switch (inviteClient.State) {
	case SDKs.ClientType.SteamDew:
	case SDKs.ClientType.Galaxy:
		multiplayer = SteamDew.GetGameMultiplayer();
		break;
	case SDKs.ClientType.Unknown:
	default:
		return;
	}

	if (multiplayer == null) {
		SteamDew.Log($"Invite Client could not inject real client: Game1.multiplayer was null");
		return;
	}

	if (inviteClient.State == SDKs.ClientType.SteamDew) {
		SteamDew.Log($"Replacing the Invite Client with a SteamDew Client");

		CSteamID lobby = new CSteamID();
		lobby.Clear();

		menu.client = multiplayer.InitClient(
			new SDKs.Decoys.DSteamDewClient(
				lobby, 
				SDKs.ClientState.FoundHost,
				inviteClient.SteamDewHost
			)
		);
	} else {
		SteamDew.Log($"Replacing the Invite Client with a Galaxy Client");
		menu.client = multiplayer.InitClient(
			new SDKs.Decoys.DGalaxyNetClient(inviteClient.Lobby)
		);
	}
}

private static IEnumerable<CodeInstruction> PatchTranspiler(IEnumerable<CodeInstruction> instructions)
{
	int pos = 0;
	int callPos = 0;
	foreach (CodeInstruction instr in instructions) {
		++pos;
		if (instr.opcode != OpCodes.Callvirt) {
			continue;
		}
		if (!(instr.operand is MethodInfo)) {
			continue;
		}
		MethodInfo m = instr.operand as MethodInfo;
		if (m.DeclaringType != typeof(StardewValley.Network.Client)) {
			continue;
		}
		if (m.Name != "receiveMessages") {
			continue;
		}
		callPos = pos;
		break;
	}

	if (callPos == 0) {
		SteamDew.Log("Could not find receiveMessages() call in FarmhandMenu::update(...)");
		return instructions;
	}

	int ldPos = callPos - 2;
	if (ldPos < 1) {
		SteamDew.Log("FarmhandMenu::update(...) does not pass enough args to receiveMessages()");
		return instructions;
	}

	pos = 0;
	foreach (CodeInstruction instr in instructions) {
		++pos;
		if (pos < ldPos) {
			continue;
		}
		int offset = pos - ldPos;
		switch (offset) {
		case 0:
			continue;
		case 1:
			instr.opcode = OpCodes.Nop;
			instr.operand = null;
			continue;
		case 2:
			instr.opcode = OpCodes.Call;
			instr.operand = typeof(Update).GetMethod(
				"ReceiveMessages", 
				BindingFlags.NonPublic | BindingFlags.Static
			);
			return instructions;
		}
	}

	return instructions;
}

} /* class Update */

} /* namespace SteamDew.Patcher.PFarmhandMenu */