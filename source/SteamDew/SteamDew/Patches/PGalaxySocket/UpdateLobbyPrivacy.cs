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
using StardewValley;
using StardewValley.Network;
using StardewValley.SDKs;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SteamDew.Patches.PGalaxySocket {

public class UpdateLobbyPrivacy : Patcher {

public UpdateLobbyPrivacy()
{
	MethodInfo m = typeof(GalaxySocket).GetMethod(
		"updateLobbyPrivacy", 
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

private static string GetSocketConnectionString(GalaxySocket socket) {
	MethodInfo m = typeof(GalaxySocket).GetMethod(
		"getConnectionString",
		BindingFlags.NonPublic | BindingFlags.Instance
	);

	if (m == null) {
		SteamDew.Log($"Could not access GalaxySocket::getConnectionString()");
		return null;
	}

	object result = m.Invoke(socket, null);
	if (!(result is string)) {
		SteamDew.Log($"getConnectionString() did not return a string");
		return null;
	}

	string connectionString = result as string;
	SteamDew.Log($"Intercepted connection string: {connectionString}");

	return connectionString;
}

private static List<Server> GetServerList() {
	if (Game1.server == null) {
		SteamDew.Log($"Could not get server list: Game1.server is null");
		return null;
	}

	if (!(Game1.server is GameServer)) {
		SteamDew.Log($"Could not get server list: Game1.server is not a GameServer");
		return null;
	}

	GameServer gameServer = Game1.server as GameServer;

	FieldInfo f = typeof(GameServer).GetField(
		"servers",
		BindingFlags.NonPublic | BindingFlags.Instance
	);

	if (f == null) {
		SteamDew.Log($"Could not access the GameServer server list");
		return null;
	}

	object serversObject = f.GetValue(gameServer);
	if (!(serversObject is List<Server>)) {
		SteamDew.Log($"Could not get server list: Servers list was not a list");
		return null;
	}

	List<Server> servers = serversObject as List<Server>;
	return servers;
}

private static GalaxyID GetSocketLobby(GalaxySocket socket) {
	FieldInfo f = typeof(GalaxySocket).GetField(
		"lobby",
		BindingFlags.NonPublic | BindingFlags.Instance
	);

	if (f == null) {
		SteamDew.Log($"Could not access GalaxySocket lobby");
		return null;
	}

	object lobbyObject = f.GetValue(socket);
	if (!(lobbyObject is GalaxyID)) {
		SteamDew.Log($"GalaxySocket lobby was not a GalaxyID");
		return null;
	}

	GalaxyID lobby = lobbyObject as GalaxyID;
	return lobby;
}

private static bool UpdateConnectionString(GalaxySocket socket)
{
	string connectionString = UpdateLobbyPrivacy.GetSocketConnectionString(socket);
	List<Server> servers = UpdateLobbyPrivacy.GetServerList();
	GalaxyID lobby = UpdateLobbyPrivacy.GetSocketLobby(socket);

	if (connectionString == null || servers == null || lobby == null) {
		return false;
	}

	bool foundSteamDew = false;
	foreach (Server server in servers) {
		if (!(server is SDKs.SteamDewServer)) {
			continue;
		}
		SteamDew.Log($"Found our SteamDewServer. Updating connection string.");

		server.setLobbyData("connect", connectionString);
		foundSteamDew = true;
		break;
	}
	
	if (!foundSteamDew) {
		SteamDew.Log($"Could not find our SteamDewServer.");
		return false;
	}

	Steamworks.CSteamID steamID = Steamworks.SteamUser.GetSteamID();
	string hostString = Convert.ToString(steamID.m_SteamID);
	GalaxyInstance.Matchmaking().SetLobbyData(lobby, "SteamDewHost", hostString);

	return true;
}

private static IEnumerable<CodeInstruction> PatchTranspiler(IEnumerable<CodeInstruction> instructions)
{
	int state = 0;
	foreach (CodeInstruction instr in instructions) {
		MethodInfo m;
		switch (state) {
		case 0:
			if (instr.opcode != OpCodes.Callvirt) {
				continue;
			}
			if (!(instr.operand is MethodInfo)) {
				continue;
			}
			m = instr.operand as MethodInfo;
			if (!m.DeclaringType.Equals(typeof(Galaxy.Api.IMatchmaking))) {
				continue;
			}
			if (m.Name != "SetLobbyType") {
				continue;
			}
			state = 1;
			continue;
		case 1:
			OpCode c = instr.opcode;
			object o = instr.operand;
			instr.opcode = OpCodes.Nop;
			instr.operand = null;
			if (c != OpCodes.Ldstr) {
				continue;
			}
			if (!(o is string)) {
				continue;
			}
			string s = o as string;
			if (s != "connect") {
				continue;
			}
			state = 2;
			continue;
		case 2:
			if (instr.opcode != OpCodes.Call) {
				continue;
			}
			if (!(instr.operand is MethodInfo)) {
				continue;
			}
			m = instr.operand as MethodInfo;
			if (!m.DeclaringType.Equals(typeof(Steamworks.SteamMatchmaking))) {
				instr.opcode = OpCodes.Nop;
				instr.operand = null;
				continue;
			}
			if (m.Name != "SetLobbyData") {
				instr.opcode = OpCodes.Nop;
				instr.operand = null;
				continue;
			}
			instr.operand = typeof(UpdateLobbyPrivacy).GetMethod(
				"UpdateConnectionString", 
				BindingFlags.NonPublic | BindingFlags.Static
			);
			return instructions;
		}

	}
	return instructions;
}

} /* class UpdateLobbyPrivacy */

} /* namespace SteamDew.Patcher.PGalaxySocket */