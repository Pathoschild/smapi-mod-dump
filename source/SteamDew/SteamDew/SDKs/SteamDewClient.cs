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

public class SteamDewClient : Client {

private readonly Action<IncomingMessage, Action<OutgoingMessage>, Action> OnProcessingMessage;
private readonly Action<OutgoingMessage, Action<OutgoingMessage>, Action> OnSendingMessage;

private CallResult<LobbyEnter_t> LobbyEnterCallResult;

private Callback<SteamNetConnectionStatusChangedCallback_t> SteamNetConnectionStatusChangedCallback;

private CSteamID Lobby;
private CSteamID HostID;
private ClientState State;

private HSteamNetConnection Conn;

private IntPtr[] Messages;

public SteamDewClient(CSteamID lobby, ClientState state, CSteamID host, Action<IncomingMessage, Action<OutgoingMessage>, Action> onProcessingMessage, Action<OutgoingMessage, Action<OutgoingMessage>, Action> onSendingMessage)
{
	this.OnProcessingMessage = onProcessingMessage;
	this.OnSendingMessage = onSendingMessage;

	this.LobbyEnterCallResult = CallResult<LobbyEnter_t>.Create(HandleLobbyEnter);

	this.SteamNetConnectionStatusChangedCallback = Callback<SteamNetConnectionStatusChangedCallback_t>.Create(HandleSteamNetConnectionStatusChanged);

	this.Lobby = lobby;
	this.HostID = host;

	this.State = state;

	this.Conn = HSteamNetConnection.Invalid;

	this.Messages = new IntPtr[256];
}

~SteamDewClient()
{
	this.SteamNetConnectionStatusChangedCallback.Unregister();
}

private void HandleHost(CSteamID hostID) {
	this.HostID = hostID;

	SteamNetworkingConfigValue_t[] pOptions = null;
	int nOptions = SteamDewNetUtils.GetNetworkingOptions(out pOptions);

	SteamNetworkingIdentity identity = new SteamNetworkingIdentity();
	identity.Clear();
	identity.SetSteamID(hostID);
	this.Conn = SteamNetworkingSockets.ConnectP2P(ref identity, 0, nOptions, pOptions);
}

private string HandleLobby(CSteamID lobby) {
	if (!lobby.IsValid() || !lobby.IsLobby()) {
		SteamMatchmaking.LeaveLobby(lobby);
		return $"Invalid Lobby ID: {lobby.m_SteamID.ToString()}";
	}

	if (!this.Lobby.Equals(lobby)) {
		SteamMatchmaking.LeaveLobby(lobby);
		return $"Joined Wrong Lobby (ID: {lobby.m_SteamID.ToString()}";
	}

	string lobbyVersion = SteamMatchmaking.GetLobbyData(lobby, "protocolVersion");

	if (lobbyVersion == "") {
		return "Missing Protocol Version";
	}

	if (lobbyVersion != SteamDew.PROTOCOL_VERSION) {
		return $"Protocol Mismatch (Local: {SteamDew.PROTOCOL_VERSION}, Remote: {lobbyVersion})";
	}

	string isSteamDew = SteamMatchmaking.GetLobbyData(lobby, "isSteamDew");

	if (isSteamDew == "") {
		return "Not a SteamDew Server (Report this, it should never happen)";
	}

	uint ip = 0u;
	ushort port = 0;
	CSteamID hostID = new CSteamID();
	hostID.Clear();

	if (SteamMatchmaking.GetLobbyGameServer(lobby, out ip, out port, out hostID)) {
		SteamMatchmaking.LeaveLobby(lobby);
		if (hostID.IsValid()) {
			this.HandleHost(hostID);
			return null;
		}
	}

	return $"Invalid Server ID: {hostID.m_SteamID.ToString()}";
}

private string HandleLobbyEnterHelper(LobbyEnter_t evt, bool IOFailure)
{
	if (IOFailure) {
		return "IO Failure";
	}

	if (evt.m_EChatRoomEnterResponse != ((uint) EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)) {
		return "Failed to join lobby";
	}

	CSteamID lobby = new CSteamID(evt.m_ulSteamIDLobby);
	return this.HandleLobby(lobby);
}

private void HandleLobbyEnter(LobbyEnter_t evt, bool IOFailure)
{
	string lobbyError = HandleLobbyEnterHelper(evt, IOFailure);
	if (lobbyError == null) {
		return;
	}

	string failMsg = StardewValley.Game1.content.LoadString("Strings\\UI:CoopMenu_Failed");
	this.connectionMessage = $"{failMsg} ({lobbyError})";

	SteamDew.Log($"Error joining lobby ({lobbyError})");
}

private void HandleConnecting(SteamNetConnectionStatusChangedCallback_t evt) {
	SteamDew.Log($"Client connecting to server (ID: {this.HostID.m_SteamID.ToString()})...");
}

private void HandleConnected(SteamNetConnectionStatusChangedCallback_t evt) {
	SteamDew.Log($"Client connected to server (ID: {this.HostID.m_SteamID.ToString()})");
}

private void HandleDisconnected(SteamNetConnectionStatusChangedCallback_t evt) {
	SteamDew.Log($"Client disconnected from server (ID: {this.HostID.m_SteamID.ToString()})");

	this.timedOut = true;
	this.pendingDisconnect = StardewValley.Multiplayer.DisconnectType.HostLeft;
}

private void HandleSteamNetConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t evt) {
	if (evt.m_hConn != this.Conn) {
		return;
	}
	switch (evt.m_info.m_eState) {
	case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting:
		this.HandleConnecting(evt);
		return;
	case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected:
		this.HandleConnected(evt);
		return;
	case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer:
	case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally:
		this.HandleDisconnected(evt);
		return;
	}
}

protected override void connectImpl()
{
	SteamDew.Log($"Client connecting to lobby (ID: {this.Lobby.m_SteamID.ToString()})...");

	switch (this.State) {
	case ClientState.JoiningLobby:
		SteamAPICall_t steamAPICall = SteamMatchmaking.JoinLobby(this.Lobby);
		this.LobbyEnterCallResult.Set(steamAPICall);
		break;
	case ClientState.JoinedLobby:
		this.HandleLobby(this.Lobby);
		break;
	case ClientState.FoundHost:
		this.HandleHost(this.HostID);
		break;
	}	
}

public override void disconnect(bool neatly = true)
{
	if (this.Conn == HSteamNetConnection.Invalid) {
		return;
	}

	SteamDew.Log($"Client disconnecting from server (ID: {this.HostID.m_SteamID.ToString()})...");
	SteamDewNetUtils.CloseConnection(this.Conn);

	this.connectionMessage = null;
}

protected override void receiveMessagesImpl()
{
	if (this.Conn == HSteamNetConnection.Invalid) {
		return;
	}

	int msgCount = SteamNetworkingSockets.ReceiveMessagesOnConnection(this.Conn, this.Messages, this.Messages.Length);
	for (int m = 0; m < msgCount; ++m) {
		IncomingMessage msg = new IncomingMessage();
		HSteamNetConnection msgConn = HSteamNetConnection.Invalid;

		SteamDewNetUtils.HandleSteamMessage(this.Messages[m], msg, out msgConn, bandwidthLogger);

		IncomingMessage msgTemp = msg;
		this.OnProcessingMessage(
			msgTemp, 
			delegate(OutgoingMessage outgoing) {
				SteamDewNetUtils.SendMessage(this.Conn, outgoing, bandwidthLogger);
			},
			delegate {
				this.processIncomingMessage(msgTemp);
			}
		);
	}

	SteamNetworkingSockets.FlushMessagesOnConnection(this.Conn);
}

public override void sendMessage(OutgoingMessage message)
{
	if (this.Conn == HSteamNetConnection.Invalid) {
		return;
	}
	this.OnSendingMessage(
		message,
		delegate(OutgoingMessage outgoing) {
			SteamDewNetUtils.SendMessage(this.Conn, outgoing, bandwidthLogger);
		},
		delegate {
			SteamDewNetUtils.SendMessage(this.Conn, message, bandwidthLogger);
		}	
	);
}

public override string getUserID()
{
	return Convert.ToString(GalaxyInstance.User().GetGalaxyID().ToUint64());
}

protected override string getHostUserName()
{
	if (this.HostID.IsValid()) {
		return SteamFriends.GetFriendPersonaName(this.HostID);
	}
	return "???";
}

public override float GetPingToHost()
{
	SteamNetworkingQuickConnectionStatus status;
	SteamNetworkingSockets.GetQuickConnectionStatus(this.Conn, out status);
	return status.m_nPing;
}

} /* class SteamDewClient */

} /* namespace SteamDew.SDKs */