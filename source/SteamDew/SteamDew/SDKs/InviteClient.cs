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
using StardewValley.Network;
using Steamworks;
using System;

namespace SteamDew.SDKs {

public class InviteClient : Client {

private class LobbyDataRetreive : ILobbyDataRetrieveListener {
	private Action<GalaxyID, bool> callback;

	public LobbyDataRetreive(Action<GalaxyID, bool> callback)
	{
		this.callback = callback;
	}

	public override void OnLobbyDataRetrieveSuccess(GalaxyID lobbyID)
	{
		if (callback != null) {
			callback(lobbyID, true);
		}
	}

	public override void OnLobbyDataRetrieveFailure(GalaxyID lobbyID, ILobbyDataRetrieveListener.FailureReason failureReason)
	{
		if (callback != null) {
			callback(lobbyID, false);
		}
	}
}

private LobbyDataRetreive LobbyDataRetreiveCallback;

private readonly Action<IncomingMessage, Action<OutgoingMessage>, Action> OnProcessingMessage;
private readonly Action<OutgoingMessage, Action<OutgoingMessage>, Action> OnSendingMessage;

public readonly GalaxyID Lobby;

public ClientType State;

public CSteamID SteamDewHost;

public InviteClient(GalaxyID lobby, Action<IncomingMessage, Action<OutgoingMessage>, Action> onProcessingMessage, Action<OutgoingMessage, Action<OutgoingMessage>, Action> onSendingMessage)
{
	SteamDew.Log($"Created Invite Client to determine the lobby type");

	this.LobbyDataRetreiveCallback = new LobbyDataRetreive(HandleLobbyDataRetreive);

	this.OnProcessingMessage = onProcessingMessage;
	this.OnSendingMessage = onSendingMessage;

	this.Lobby = lobby;

	this.State = ClientType.Unknown;

	this.SteamDewHost = new CSteamID();
	this.SteamDewHost.Clear();
}

~InviteClient() {
	SteamDew.Log($"Destroyed Invite Client");
}

void HandleLobbyDataRetreive(GalaxyID lobbyID, bool success)
{
	if (!success) {
		this.timedOut = true;
		return;
	}

	string steamDewHost = GalaxyInstance.Matchmaking().GetLobbyData(lobbyID, "SteamDewHost");
	CSteamID steamID = new CSteamID();
	steamID.Clear();

	if (steamDewHost != "") {
		try {
			ulong decoded = Convert.ToUInt64(steamDewHost);
			steamID = new CSteamID(decoded);
		} catch (Exception) {

		}
	}

	if (steamID.IsValid()) {
		SteamDew.Log($"Invite Client found a SteamDew Host");
		this.SteamDewHost = steamID;
		this.State = ClientType.SteamDew;
	} else {
		SteamDew.Log($"Invite Client did not find a SteamDew Host. Defaulting to Galaxy.");
		this.State = ClientType.Galaxy;
	}
}

protected override void connectImpl() {
	SteamDew.Log($"Invite Client attempting to query Galaxy Lobby data");
	GalaxyInstance.Matchmaking().RequestLobbyData(this.Lobby, this.LobbyDataRetreiveCallback);
}

public override void sendMessage(OutgoingMessage message) {

}

protected override void receiveMessagesImpl() {

}

public override string getUserID()
{
	return "";
}

protected override string getHostUserName()
{
	return "";
}

public override void disconnect(bool neatly = true) {

}

} /* class InviteClient */

} /* namespace SteamDew.SDKs */