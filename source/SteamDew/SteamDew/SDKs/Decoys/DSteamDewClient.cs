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
using StardewValley.SDKs;
using Steamworks;

namespace SteamDew.SDKs.Decoys {

public class DSteamDewClient : GalaxyNetClient {

public CSteamID LobbyID;
public CSteamID HostID;
public ClientState State;

public DSteamDewClient(CSteamID lobby, ClientState state, CSteamID host = new CSteamID()) : base(new GalaxyID(lobby.m_SteamID))
{
	SteamDew.Log($"Created Decoy for SteamDew Client");
	this.LobbyID = lobby;
	this.State = state;
	this.HostID = host;

	SteamDew.LastClientType = SDKs.ClientType.SteamDew;
}

~DSteamDewClient()
{
	SteamDew.Log($"Destroyed Decoy for SteamDew Client");
}

} /* class DSteamDewClient */

} /* namespace SteamDew.SDKs.Decoys */