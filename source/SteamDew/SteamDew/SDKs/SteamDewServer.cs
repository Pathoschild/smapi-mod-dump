/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/myuusubi/SteamDew
**
*************************************************/

using StardewValley;
using StardewValley.Network;
using StardewValley.SDKs;
using Steamworks;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SteamDew.SDKs {

public class SteamDewServer : Server {

private class PeerData {
	public long FarmerID;
	public CSteamID SteamID;
	public HSteamNetConnection Conn;
	public bool Farming;

	public PeerData()
	{
		this.FarmerID = Int64.MinValue;

		this.SteamID = new CSteamID();
		this.SteamID.Clear();

		this.Conn = HSteamNetConnection.Invalid;

		this.Farming = false;
	}
}

private readonly Action<IncomingMessage, Action<OutgoingMessage>, Action> OnProcessingMessage;

private CallResult<LobbyCreated_t> LobbyCreatedCallResult;

private Callback<PersonaStateChange_t> PersonaStateChangeCallback;
private Callback<SteamNetConnectionStatusChangedCallback_t> SteamNetConnectionStatusChangedCallback;

private CSteamID Lobby;
private HSteamListenSocket Listener;
private HSteamNetPollGroup JoinGroup;
private HSteamNetPollGroup PeerGroup;

private IntPtr[] Messages;

private ServerPrivacy Privacy;

private Dictionary<string, string> LobbyData;

private Dictionary<CSteamID, PeerData> SteamPeerMap;
private Dictionary<long, PeerData> FarmerPeerMap;

private IGameServer iGameServer;

public override int connectionsCount
{
	get {
		if (this.SteamPeerMap == null) {
			return 0;
		}
		return this.SteamPeerMap.Count;
	}
}

public SteamDewServer(IGameServer gameServer, object multiplayer, Action<IncomingMessage, Action<OutgoingMessage>, Action> onProcessingMessage) : base(gameServer)
{
	this.OnProcessingMessage = onProcessingMessage;
	this.iGameServer = gameServer;
}

private PeerData FarmerToPeer(long farmerId) {
	if (!this.FarmerPeerMap.ContainsKey(farmerId)) {
		return null;
	}
	return this.FarmerPeerMap[farmerId];
}

private PeerData SteamToPeer(CSteamID steamID) {
	if (!this.SteamPeerMap.ContainsKey(steamID)) {
		return null;
	}
	return this.SteamPeerMap[steamID];
}

private bool TrySteamToFarmer(CSteamID steamID, ref long farmerId) {
	PeerData peer = this.SteamToPeer(steamID);
	if (peer == null || !peer.Farming || !this.FarmerPeerMap.ContainsKey(peer.FarmerID)) {
		return false;
	}
	farmerId = peer.FarmerID;
	return true;
}

private string SteamToUser(CSteamID steamID) {
	return Convert.ToString(steamID.m_SteamID);
}

private CSteamID UserToSteam(string userID) {
	try {
		ulong steamUL = Convert.ToUInt64(userID);
		return new CSteamID(steamUL);
	} catch(Exception) {
		CSteamID steamID = new CSteamID();
		steamID.Clear();
		return steamID;
	}
}

private string SteamToConn(CSteamID steamID)
{
	return "SN_" + this.SteamToUser(steamID);
}

private CSteamID ConnToSteam(string connID) {
	CSteamID steamID = new CSteamID();
	steamID.Clear();

	if (connID.IndexOf("SN_") != 0) {
		return steamID;
	}

	return this.UserToSteam(connID.Substring(3));
}

private void UpdateLobbyPrivacy()
{
	if (!this.Lobby.IsValid() || !this.Lobby.IsLobby()) {
		return;
	}

	ELobbyType lobbyType = ELobbyType.k_ELobbyTypePrivate;
	switch (this.Privacy) {
	case ServerPrivacy.FriendsOnly:
		lobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;
		break;
	case ServerPrivacy.Public:
		lobbyType = ELobbyType.k_ELobbyTypePublic;
		break;
	}
	SteamMatchmaking.SetLobbyType(this.Lobby, lobbyType);
}

public override void initialize()
{
	this.Lobby = new CSteamID();
	this.Lobby.Clear();

	this.Listener = HSteamListenSocket.Invalid;

	this.JoinGroup = HSteamNetPollGroup.Invalid;
	this.PeerGroup = HSteamNetPollGroup.Invalid;

	this.Messages = new IntPtr[256];

	this.Privacy = Game1.options.serverPrivacy;

	this.LobbyData = new Dictionary<string, string>();

	this.SteamPeerMap = new Dictionary<CSteamID, PeerData>();
	this.FarmerPeerMap = new Dictionary<long, PeerData>();

	SteamDew.Log($"Starting SteamDew Server");

	int maxMembers = 4 * 2;

	Multiplayer multiplayer = SteamDew.GetGameMultiplayer();
	if (multiplayer != null) {
		maxMembers = multiplayer.playerLimit * 2;
	}

	this.LobbyCreatedCallResult = CallResult<LobbyCreated_t>.Create(HandleLobbyCreated);

	this.PersonaStateChangeCallback = Callback<PersonaStateChange_t>.Create(HandlePersonaStateChange);
	this.SteamNetConnectionStatusChangedCallback = Callback<SteamNetConnectionStatusChangedCallback_t>.Create(HandleSteamNetConnectionStatusChanged);

	SteamAPICall_t steamAPICall = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivate, maxMembers);
	this.LobbyCreatedCallResult.Set(steamAPICall);
}

private string HandleLobbyCreatedHelper(LobbyCreated_t evt, bool IOFailure)
{
	if (IOFailure) {
		return "IO Failure";
	}

	switch (evt.m_eResult) {
	case EResult.k_EResultOK:
		CSteamID lobby = new CSteamID(evt.m_ulSteamIDLobby);
		if (!lobby.IsValid() || !lobby.IsLobby()) {
			return "Created Lobby is Invalid";
		}
		this.Lobby = lobby;
		return null;
	case EResult.k_EResultTimeout:
		return "Steam Timed Out";
	case EResult.k_EResultLimitExceeded:
		return "Too Many Steam Lobbies";
	case EResult.k_EResultAccessDenied:
		return "Steam Denied Access";
	case EResult.k_EResultNoConnection:
		return "No Steam Connection";			
	}

	return "Unknown Steam Failure";
}

private void HandleLobbyCreated(LobbyCreated_t evt, bool IOFailure)
{
	string lobbyError = HandleLobbyCreatedHelper(evt, IOFailure);
	if (lobbyError == null) {
		SteamNetworkingConfigValue_t[] pOptions = null;
		int nOptions = SteamDewNetUtils.GetNetworkingOptions(out pOptions);

		this.Listener = SteamNetworkingSockets.CreateListenSocketP2P(0, nOptions, pOptions);
		this.JoinGroup = SteamNetworkingSockets.CreatePollGroup();
		this.PeerGroup = SteamNetworkingSockets.CreatePollGroup();

		SteamMatchmaking.SetLobbyGameServer(this.Lobby, 0u, 0, SteamUser.GetSteamID());

		foreach (KeyValuePair<string, string> d in this.LobbyData) {
			SteamMatchmaking.SetLobbyData(this.Lobby, d.Key, d.Value);
		}

		SteamMatchmaking.SetLobbyData(this.Lobby, "protocolVersion", SteamDew.PROTOCOL_VERSION);
		SteamMatchmaking.SetLobbyData(this.Lobby, "isSteamDew", "isYes");

		SteamMatchmaking.SetLobbyJoinable(this.Lobby, true);

		UpdateLobbyPrivacy();

		SteamDew.Log($"Server successfully created lobby (ID: {this.Lobby.m_SteamID.ToString()})");
		return;
	}
	SteamDew.Log($"Server failed to create lobby ({lobbyError})");	
}

private void HandlePersonaStateChange(PersonaStateChange_t evt)
{
	CSteamID steamID = new CSteamID(evt.m_ulSteamID);
	long farmerId = 0;
	if (!this.TrySteamToFarmer(steamID, ref farmerId)) {
		return;
	}
	
	string userName = SteamFriends.GetFriendPersonaName(steamID);

	/* Adapted from StardewValley.Multiplayer::broadcastUserName(long farmerId, string userName) */
	foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers) {
		Farmer farmer = otherFarmer.Value;
		if (farmer.UniqueMultiplayerID == farmerId) {
			continue;
		}
		Game1.server.sendMessage(farmer.UniqueMultiplayerID, 16, Game1.serverHost.Value, farmerId, userName);
	}
}

private void HandleConnecting(SteamNetConnectionStatusChangedCallback_t evt, CSteamID steamID)
{
	SteamDew.Log($"{steamID.m_SteamID.ToString()} connecting to server...");

	if (this.iGameServer.isUserBanned(steamID.m_SteamID.ToString())) {
		SteamDew.Log($"{steamID.m_SteamID.ToString()} is banned");
		SteamDewNetUtils.CloseConnection(evt.m_hConn);
		return;
	}

	SteamNetworkingSockets.AcceptConnection(evt.m_hConn);
}

private void HandleConnected(SteamNetConnectionStatusChangedCallback_t evt, CSteamID steamID)
{
	SteamDew.Log($"{steamID.m_SteamID.ToString()} connected to server");

	PeerData peer = new PeerData();
	peer.SteamID = steamID;
	peer.Conn = evt.m_hConn;
	peer.FarmerID = Int64.MinValue;
	peer.Farming = false;

	this.SteamPeerMap[steamID] = peer;

	SteamNetworkingSockets.SetConnectionPollGroup(evt.m_hConn, this.JoinGroup);

	this.onConnect(this.SteamToConn(steamID));

	this.iGameServer.sendAvailableFarmhands(
		"", /* TODO: Use SteamToUser(steamID) or a Galaxy ID? */
		delegate(OutgoingMessage msg) {
			SteamDewNetUtils.SendMessage(evt.m_hConn, msg, bandwidthLogger);
		}
	);
}

private void HandleDisconnected(SteamNetConnectionStatusChangedCallback_t evt, CSteamID steamID)
{
	SteamDew.Log($"{steamID.m_SteamID.ToString()} disconnected from server");

	this.onDisconnect(this.SteamToConn(steamID));

	long farmerId = 0;
	if (this.TrySteamToFarmer(steamID, ref farmerId)) {
		this.playerDisconnected(farmerId);
	}

	this.SteamPeerMap.Remove(steamID);

	SteamDewNetUtils.CloseConnection(evt.m_hConn);
}

private void HandleSteamNetConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t evt) {
	if (evt.m_info.m_identityRemote.IsInvalid()) {
		SteamDewNetUtils.CloseConnection(evt.m_hConn);
		return;
	}

	CSteamID steamID = evt.m_info.m_identityRemote.GetSteamID();
	if (!steamID.IsValid()) {
		SteamDewNetUtils.CloseConnection(evt.m_hConn);
		return;
	}

	switch (evt.m_info.m_eState) {
	case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting:
		this.HandleConnecting(evt, steamID);
		return;
	case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected:
		this.HandleConnected(evt, steamID);
		return;
	case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer:
	case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally:
		this.HandleDisconnected(evt, steamID);
		return;
	}
}

public override void setPrivacy(ServerPrivacy privacy)
{
	this.Privacy = privacy;
	UpdateLobbyPrivacy();
}

public override void stopServer()
{
	SteamDew.Log($"Stopping SteamDew server");

	foreach (KeyValuePair<CSteamID, PeerData> peer in this.SteamPeerMap) {
		SteamDewNetUtils.CloseConnection(peer.Value.Conn);
	}

	if (this.Lobby.IsValid() && this.Lobby.IsLobby()) {
		SteamMatchmaking.LeaveLobby(this.Lobby);	
	}
	
	if (this.Listener != HSteamListenSocket.Invalid) {
		SteamNetworkingSockets.CloseListenSocket(this.Listener);
		this.Listener = HSteamListenSocket.Invalid;
	}

	if (this.PeerGroup != HSteamNetPollGroup.Invalid) {
		SteamNetworkingSockets.DestroyPollGroup(this.PeerGroup);
		this.PeerGroup = HSteamNetPollGroup.Invalid;
	}

	if (this.JoinGroup != HSteamNetPollGroup.Invalid) {
		SteamNetworkingSockets.DestroyPollGroup(this.JoinGroup);
		this.JoinGroup = HSteamNetPollGroup.Invalid;
	}

	this.PersonaStateChangeCallback.Unregister();
	this.SteamNetConnectionStatusChangedCallback.Unregister();
}

private void HandleFarmhandRequest(IncomingMessage msg, HSteamNetConnection msgConn, CSteamID steamID)
{
	Multiplayer multiplayer = SteamDew.GetGameMultiplayer();
	if (multiplayer == null) {
		SteamDewNetUtils.CloseConnection(msgConn);
		return;
	}

	PeerData peer = this.SteamToPeer(steamID);
	if (peer == null) {
		SteamDewNetUtils.CloseConnection(msgConn);
		return;
	}

	if (peer.Farming) {
		return;
	}

	NetFarmerRoot farmer = multiplayer.readFarmer(msg.Reader);
	long farmerId = farmer.Value.UniqueMultiplayerID;

	SteamDew.Log($"Server received farmhand request (Peer ID: {steamID.m_SteamID.ToString()}, Farmer ID: {farmerId})");

	this.iGameServer.checkFarmhandRequest(
		"", /* TODO: Use SteamToUser(steamID) or a Galaxy ID? */
		SteamToConn(steamID), 
		farmer, 
		delegate(OutgoingMessage msg) {
			SteamDewNetUtils.SendMessage(msgConn, msg, bandwidthLogger);
		},
		delegate {
			SteamDew.Log($"Server accepted new farmhand (Peer ID: {steamID.m_SteamID.ToString()}, Farmer ID: {farmerId})");

			SteamNetworkingSockets.SetConnectionUserData(msgConn, farmerId);
			SteamNetworkingSockets.SetConnectionPollGroup(msgConn, this.PeerGroup);

			peer.FarmerID = farmerId;
			peer.Farming = true;

			this.FarmerPeerMap[farmerId] = peer;			
		}
	);
}

private void PollFarmhandRequests()
{
	int msgCount = SteamNetworkingSockets.ReceiveMessagesOnPollGroup(this.JoinGroup, this.Messages, this.Messages.Length);
	for (int m = 0; m < msgCount; ++m) {
		IncomingMessage msg = new IncomingMessage();
		HSteamNetConnection msgConn = HSteamNetConnection.Invalid;

		SteamDewNetUtils.HandleSteamMessage(this.Messages[m], msg, out msgConn, bandwidthLogger);

		SteamNetConnectionInfo_t info;
		SteamNetworkingSockets.GetConnectionInfo(msgConn, out info);

		if (info.m_identityRemote.IsInvalid()) {
			SteamDewNetUtils.CloseConnection(msgConn);
			continue;
		}

		CSteamID steamID = info.m_identityRemote.GetSteamID();
		if (!steamID.IsValid()) {
			SteamDewNetUtils.CloseConnection(msgConn);
			continue;
		}

		PeerData peer = this.SteamToPeer(steamID);
		if (peer == null || peer.Conn != msgConn) {
			SteamDewNetUtils.CloseConnection(msgConn);
			continue;
		}

		this.OnProcessingMessage(
			msg,
			delegate(OutgoingMessage outgoing) {
				this.sendMessage(peer, outgoing);
			},
			delegate {
				if (msg.MessageType == 2) {
					this.HandleFarmhandRequest(msg, msgConn, steamID);
				}
			}
		);
	}
}

private void PollFarmers()
{
	int msgCount = SteamNetworkingSockets.ReceiveMessagesOnPollGroup(this.PeerGroup, this.Messages, this.Messages.Length);
	for (int m = 0; m < msgCount; ++m) {
		IncomingMessage msg = new IncomingMessage();
		HSteamNetConnection msgConn = HSteamNetConnection.Invalid;

		SteamDewNetUtils.HandleSteamMessage(this.Messages[m], msg, out msgConn, bandwidthLogger);

		long farmerId = SteamNetworkingSockets.GetConnectionUserData(msgConn);
		PeerData peer = this.FarmerToPeer(farmerId);

		if (peer == null || peer.Conn != msgConn) {
			SteamDewNetUtils.CloseConnection(msgConn);
			continue;
		}

		if (msg.MessageType == 2) {
			continue;
		}

		this.OnProcessingMessage(msg, 
			delegate(OutgoingMessage outgoing) {
				this.sendMessage(peer, outgoing);
			},
			delegate {
				this.iGameServer.processIncomingMessage(msg);
			}
		);
	}
}

public override void receiveMessages()
{
	if (!this.connected()) {
		return;
	}

	this.PollFarmhandRequests();
	this.PollFarmers();

	foreach (KeyValuePair<CSteamID, PeerData> peer in this.SteamPeerMap) {
		SteamNetworkingSockets.FlushMessagesOnConnection(peer.Value.Conn);
	}
}

private void sendMessage(PeerData peer, OutgoingMessage message)
{
	if (!this.connected()) {
		return;
	}
	if (peer.Conn == HSteamNetConnection.Invalid) {
		return;
	}
	SteamDewNetUtils.SendMessage(peer.Conn, message, bandwidthLogger);
}

public override void sendMessage(long peerId, OutgoingMessage message)
{
	PeerData peer = this.FarmerToPeer(peerId);
	if (peer == null) {
		return;
	}
	this.sendMessage(peer, message);
}

public override bool connected()
{
	if (this.Listener == HSteamListenSocket.Invalid) {
		return false;
	}
	if (this.PeerGroup == HSteamNetPollGroup.Invalid) {
		return false;
	}
	return true;
}

public override bool canOfferInvite()
{
	return this.connected();
}

public override void offerInvite()
{
	if (!this.connected()) {
		return;
	}
	if (!this.Lobby.IsValid() || !this.Lobby.IsLobby()) {
		return;
	}
	SteamFriends.ActivateGameOverlayInviteDialog(this.Lobby);
}

public override string getInviteCode()
{
	return null;
}

public override string getUserId(long farmerId)
{
	PeerData peer = this.FarmerToPeer(farmerId);
	if (peer == null) {
		return null;
	}
	return this.SteamToUser(peer.SteamID);
}

public override bool hasUserId(string userId)
{
	CSteamID steamID = this.UserToSteam(userId);
	PeerData peer = this.SteamToPeer(steamID);
	if (peer == null || !peer.Farming) {
		return false;
	}
	return true;
}

public override float getPingToClient(long farmerId)
{
	PeerData peer = this.FarmerToPeer(farmerId);
	if (peer == null) {
		return -1.0f;
	}
	SteamNetworkingQuickConnectionStatus status;
	SteamNetworkingSockets.GetQuickConnectionStatus(
		peer.Conn,
		out status
	);
	return status.m_nPing;
}

public override bool isConnectionActive(string connection_id)
{
	CSteamID steamID = this.ConnToSteam(connection_id);
	PeerData peer = this.SteamToPeer(steamID);
	if (peer == null) {
		return false;
	}
	return true;
}

public override string getUserName(long farmerId)
{
	PeerData peer = this.FarmerToPeer(farmerId);
	if (peer == null) {
		return null;
	}
	return SteamFriends.GetFriendPersonaName(peer.SteamID);
}

public override void setLobbyData(string key, string value)
{
	if (this.LobbyData == null) {
		return;
	}
	this.LobbyData[key] = value;
	if (!this.Lobby.IsValid() || !this.Lobby.IsLobby()) {
		return;
	}
	SteamMatchmaking.SetLobbyData(this.Lobby, key, value);
}

public override void kick(long disconnectee)
{
	base.kick(disconnectee);
	PeerData peer = this.FarmerToPeer(disconnectee);
	if (peer == null) {
		return;
	}
	Farmer player = Game1.player;
	object[] data = new object[0];
	sendMessage(peer, new OutgoingMessage(23, player, data));
}

public override void playerDisconnected(long disconnectee)
{
	PeerData peer = this.FarmerToPeer(disconnectee);
	if (peer == null) {
		return;
	}
	base.playerDisconnected(disconnectee);
	this.FarmerPeerMap.Remove(disconnectee);
}

} /* class SteamDewServer */

} /* namespace SteamDew.SDKs */