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
using StardewValley.SDKs;
using Steamworks;
using System;
using System.Collections.Generic;

namespace SteamDew.SDKs {

public class SteamDewNetHelper : SDKNetHelper {

private class Lobby {
	private ulong Steam;
	private ulong Galaxy;
	private bool FromCode;

	public Lobby() {
		CSteamID steamID = new CSteamID();
		steamID.Clear();

		this.Steam = steamID.m_SteamID;
		this.Galaxy = UInt64.MaxValue;
		this.FromCode = false;
	}

	public bool IsSteam() {
		CSteamID steamID = new CSteamID(this.Steam);
		return steamID.IsValid() && steamID.IsLobby();
	}

	public bool IsGalaxy() {
		return !this.IsSteam();
	}

	public bool IsFromCode() {
		return this.FromCode;
	}

	public CSteamID GetSteamID() {
		return new CSteamID(this.Steam);
	}

	public GalaxyID GetGalaxyID() {
		return new GalaxyID(this.Galaxy);
	}

	public void SetSteamID(CSteamID steamID) {
		this.Steam = steamID.m_SteamID;
		this.Galaxy = UInt64.MaxValue;
	}

	public void SetGalaxyID(GalaxyID galaxyID) {
		CSteamID steamID = new CSteamID();
		steamID.Clear();

		this.Steam = steamID.m_SteamID;
		this.Galaxy = galaxyID.ToUint64();
	}

	public void SetFromCode(bool fromCode) {
		this.FromCode = fromCode;
	}
}

/* Adapted from StardewValley.SDKs.GalaxyNetHelper::LobbyDataListener */
private class GalaxyLobbyUpdate : ILobbyDataListener {
	private Action<GalaxyID, GalaxyID> callback;

	public GalaxyLobbyUpdate(Action<GalaxyID, GalaxyID> callback)
	{
		this.callback = callback;
		GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerLobbyData.GetListenerType(), this);
	}

	public override void OnLobbyDataUpdated(GalaxyID lobbyID, GalaxyID memberID)
	{
		if (callback != null) {
			callback(lobbyID, memberID);
		}
	}
}

private GalaxyLobbyUpdate GalaxyLobbyUpdateCallback;

private Callback<LobbyDataUpdate_t> LobbyDataUpdateCallback;
private Callback<GameLobbyJoinRequested_t> GameLobbyJoinRequestedCallback;
private Callback<SteamRelayNetworkStatus_t> SteamRelayNetworkStatusCallback;

private CallResult<LobbyEnter_t> LobbyEnterCallResult;

private List<LobbyUpdateListener> LobbyUpdateListeners;

private Dictionary<GalaxyID, CSteamID> GalaxyLobbySteamIDMap = new Dictionary<GalaxyID, CSteamID>();

private Lobby RequestedLobby;

public SteamDewNetHelper()
{
	SteamDew.Log($"SteamDew NetHelper injected & instantiated");

	this.LobbyUpdateListeners = new List<LobbyUpdateListener>();

	this.GalaxyLobbySteamIDMap = new Dictionary<GalaxyID, CSteamID>();

	this.GalaxyLobbyUpdateCallback = new GalaxyLobbyUpdate(HandleGalaxyLobbyUpdate);

	this.GameLobbyJoinRequestedCallback = Callback<GameLobbyJoinRequested_t>.Create(HandleGameLobbyJoinRequested);
	this.LobbyDataUpdateCallback = Callback<LobbyDataUpdate_t>.Create(HandleLobbyDataUpdate);
	this.SteamRelayNetworkStatusCallback = Callback<SteamRelayNetworkStatus_t>.Create(HandleSteamRelayNetworkStatus);

	this.LobbyEnterCallResult = CallResult<LobbyEnter_t>.Create(HandleSteamLobbyEnter);

	this.RequestedLobby = null;

	CSteamID launchLobby = new CSteamID();

	bool foundLobby = false;

	string[] args = Environment.GetCommandLineArgs();
	for (int i = 0; i < args.Length - 1; i++) {
		if (args[i] != "+connect_lobby") {
			continue;
		}
		try {
			CSteamID steamID = new CSteamID(Convert.ToUInt64(args[i + 1]));
			launchLobby = steamID;
			foundLobby = true;
			break;
		} catch (Exception) {
			SteamDew.Log($"Failed to convert argument for +connect_lobby");
			continue;
		}
	}

	if (foundLobby) {
		this.RequestLobby(launchLobby);
	}

	SteamNetworkingUtils.InitRelayNetworkAccess();
}

private void InviteAccepted() {
	StardewValley.Multiplayer multiplayer = SteamDew.GetGameMultiplayer();
	if (multiplayer == null) {
		SteamDew.Log($"Could not accept invite: Game1.multiplayer was null");
		return;
	}
	multiplayer.inviteAccepted();
}

/* Adapted from StardewValley.SDKs.GalaxyNetHelper::parseConnectionString(...) */
public static GalaxyID ParseGalaxyString(string galaxyString)
{
	if (galaxyString == null || galaxyString == "") {
		return null;
	}
	if (galaxyString.StartsWith("-connect-lobby-")) {
		return new GalaxyID(Convert.ToUInt64(galaxyString.Substring("-connect-lobby-".Length)));
	}
	if (galaxyString.StartsWith("+connect_lobby ")) {
		return new GalaxyID(Convert.ToUInt64(galaxyString.Substring("+connect_lobby".Length + 1)));
	}
	return null;
}

private void HandleSteamLobbyEnter(LobbyEnter_t evt, bool IOFailure)
{
	if (IOFailure) {
		SteamDew.Log($"IO failure on joining requested lobby");
		this.RequestedLobby = null;
		return;
	}

	if (evt.m_EChatRoomEnterResponse != ((uint) EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)) {
		SteamDew.Log($"Failed to join requested lobby");
		this.RequestedLobby = null;
		return;
	}

	CSteamID lobby = new CSteamID(evt.m_ulSteamIDLobby);
	string isSteamDew = SteamMatchmaking.GetLobbyData(lobby, "isSteamDew");
	if (isSteamDew != "isYes") {
		string connect = SteamMatchmaking.GetLobbyData(lobby, "connect");
		CSteamID owner = SteamMatchmaking.GetLobbyOwner(lobby);

		SteamMatchmaking.LeaveLobby(lobby);

		GalaxyID galaxyID = SteamDewNetHelper.ParseGalaxyString(connect);
		if (galaxyID == null) {
			SteamDew.Log("Requested lobby has an invalid connect string");
			return;
		}

		this.GalaxyLobbySteamIDMap[galaxyID] = owner;

		SteamDew.Log("Joining Requested Galaxy Lobby");

		this.RequestedLobby = new Lobby();
		this.RequestedLobby.SetGalaxyID(galaxyID);
		this.InviteAccepted();
		return;
	}

	SteamDew.Log("Joining Requested SteamDew Lobby");
	this.RequestedLobby = new Lobby();
	this.RequestedLobby.SetSteamID(lobby);
	this.InviteAccepted();
}

private void RequestLobby(CSteamID steamID)
{
	SteamDew.Log($"Requesting to join lobby (ID: {steamID.m_SteamID.ToString()})");

	if (!steamID.IsValid() || !steamID.IsLobby()) {
		string l = steamID.m_SteamID.ToString();
		SteamDew.Log($"The requested Lobby ID ({l}) is invalid");
		return;
	}

	SteamAPICall_t steamAPICall = SteamMatchmaking.JoinLobby(steamID);
	this.LobbyEnterCallResult.Set(steamAPICall);
}

private void HandleGameLobbyJoinRequested(GameLobbyJoinRequested_t evt)
{
	this.RequestLobby(evt.m_steamIDLobby);
}

private void HandleGalaxyLobbyUpdate(GalaxyID lobbyID, GalaxyID memberID)
{
	Lobby lobby = new Lobby();
	lobby.SetGalaxyID(lobbyID);

	foreach (LobbyUpdateListener l in this.LobbyUpdateListeners) {
		l.OnLobbyUpdate(lobby);
	}
}

private void HandleLobbyDataUpdate(LobbyDataUpdate_t evt)
{
	ulong lobbyID = evt.m_ulSteamIDLobby;
	CSteamID steamID = new CSteamID(lobbyID);

	string isSteamDew = SteamMatchmaking.GetLobbyData(steamID, "isSteamDew");
	if (isSteamDew != "isYes") {
		string connect = SteamMatchmaking.GetLobbyData(steamID, "connect");
		GalaxyID galaxyID = SteamDewNetHelper.ParseGalaxyString(connect);
		if (galaxyID != null) {
			CSteamID owner = SteamMatchmaking.GetLobbyOwner(steamID);
			this.GalaxyLobbySteamIDMap[galaxyID] = owner;
			GalaxyInstance.Matchmaking().RequestLobbyData(galaxyID);
		}
		return;
	}

	Lobby lobby = new Lobby();
	lobby.SetSteamID(steamID);

	foreach (LobbyUpdateListener l in this.LobbyUpdateListeners) {
		l.OnLobbyUpdate(lobby);
	}
}

private void HandleSteamRelayNetworkStatus(SteamRelayNetworkStatus_t evt) {
	if(evt.m_eAvail == ESteamNetworkingAvailability.k_ESteamNetworkingAvailability_Current) {
		SteamDew.Log("Steam Datagram Relay is now available");
	}
}

public string GetUserID() 
{
	/* return Convert.ToString(SteamUser.GetSteamID().m_SteamID); */
	return Convert.ToString(GalaxyInstance.User().GetGalaxyID().ToUint64());
}

private Client CreateClientHelper(object lobby, ClientState state = ClientState.JoiningLobby)
{
	if (!(lobby is Lobby)) {
		SteamDew.Log($"Could not create client: Not a Lobby object");
		return null;
	}

	StardewValley.Multiplayer multiplayer = SteamDew.GetGameMultiplayer();
	if (multiplayer == null) {
		SteamDew.Log($"Could not create client: Game1.multiplayer was null");
		return null;
	}

	Lobby l = lobby as Lobby;
	if (l.IsGalaxy()) {
		GalaxyID galaxyID = l.GetGalaxyID();
		if (!galaxyID.IsValid()) {
			SteamDew.Log($"Could not create client: Invalid Galaxy Lobby {galaxyID.ToUint64().ToString()}");
			return null;
		}
		if (l.IsFromCode()) {
			return multiplayer.InitClient(new Decoys.DInviteClient(galaxyID));
		} else {
			return multiplayer.InitClient(new Decoys.DGalaxyNetClient(galaxyID));
		}
	} else {
		CSteamID steamID = l.GetSteamID();
		return multiplayer.InitClient(new Decoys.DSteamDewClient(steamID, state));
	}
}

public Client CreateClient(object lobby) 
{
	return CreateClientHelper(lobby);
}

public Client GetRequestedClient() 
{
	if (this.RequestedLobby == null) {
		SteamDew.Log($"GetRequestedClient() failed: requested lobby was null");
		return null;
	}

	Lobby lobby = this.RequestedLobby;
	this.RequestedLobby = null;

	if (lobby.IsSteam()) {
		CSteamID steamID = lobby.GetSteamID();
		if (!steamID.IsValid() || !steamID.IsLobby()) {
			SteamDew.Log($"Could not create client: Invalid Steam Lobby {steamID.m_SteamID.ToString()}");
			return null;
		}
		return CreateClientHelper(lobby, ClientState.JoinedLobby);
	}

	return CreateClientHelper(lobby);
}

public Server CreateServer(IGameServer gameServer) 
{
	SteamDew.Log("Tried to call SteamDewNetHelper::CreateServer(...). This should not happen.");
	return null;
}

public void AddLobbyUpdateListener(LobbyUpdateListener listener) 
{
	this.LobbyUpdateListeners.Add(listener);
}

public void RemoveLobbyUpdateListener(LobbyUpdateListener listener) 
{
	this.LobbyUpdateListeners.Remove(listener);
}

public void RequestFriendLobbyData() 
{
	EFriendFlags flags = EFriendFlags.k_EFriendFlagImmediate;
	int count = SteamFriends.GetFriendCount(flags);
	for (int i = 0; i < count; i++) {
		FriendGameInfo_t gameInfo;
		SteamFriends.GetFriendGamePlayed(SteamFriends.GetFriendByIndex(i, flags), out gameInfo);
		if(gameInfo.m_gameID.AppID() != SteamUtils.GetAppID()) {
			continue;
		}
		SteamMatchmaking.RequestLobbyData(gameInfo.m_steamIDLobby);
	}
}

public string GetLobbyData(object lobby, string key) 
{
	if (!(lobby is Lobby)) {
		return "";
	}
	Lobby l = lobby as Lobby;

	if (l.IsSteam()) {
		CSteamID steamLobby = l.GetSteamID();
		return SteamMatchmaking.GetLobbyData(steamLobby, key);
	} else {
		GalaxyID galaxyLobby = l.GetGalaxyID();
		if (!galaxyLobby.IsValid()) {
			SteamDew.Log($"Tried to GetLobbyData for invalid Galaxy Lobby: {galaxyLobby.ToUint64().ToString()}");
			return "";
		}
		return GalaxyInstance.Matchmaking().GetLobbyData(galaxyLobby, key);
	}
}

public string GetLobbyOwnerName(object lobby) 
{
	if (!(lobby is Lobby)) {
		return "???";
	}
	Lobby l = lobby as Lobby;

	if (l.IsSteam()) {
		CSteamID steamLobby = l.GetSteamID();
		CSteamID owner = SteamMatchmaking.GetLobbyOwner(steamLobby);
		return SteamFriends.GetFriendPersonaName(owner);
	} else {
		GalaxyID galaxyLobby = l.GetGalaxyID();
		if (!galaxyLobby.IsValid()) {
			SteamDew.Log($"Tried to GetLobbyOwnerName for invalid Galaxy Lobby: {galaxyLobby.ToUint64().ToString()}");
			return "???";
		}
		string ownerName = "???";
		try {
			GalaxyID owner = GalaxyInstance.Matchmaking().GetLobbyOwner(galaxyLobby);
			ownerName = GalaxyInstance.Friends().GetFriendPersonaName(owner);
		} catch(Exception) {
			if (this.GalaxyLobbySteamIDMap.ContainsKey(galaxyLobby)) {
				CSteamID owner = this.GalaxyLobbySteamIDMap[galaxyLobby];
				ownerName = SteamFriends.GetFriendPersonaName(owner);
			}
		}
		return ownerName;
	}
}

public bool SupportsInviteCodes() 
{
	return true;
}

public object GetLobbyFromInviteCode(string inviteCode) 
{
	ulong decoded = 0ul;
	try {
		decoded = Base36.Decode(inviteCode);
	} catch(FormatException) {
		SteamDew.Log($"Invite is not valid Base36: {inviteCode}");
		return null;
	}

	if (decoded == 0L || decoded >> 56 != 0L) {
		return null;
	}

	GalaxyID lobbyID = GalaxyID.FromRealID(GalaxyID.IDType.ID_TYPE_LOBBY, decoded);
	if (lobbyID.IsValid()) {
		Lobby lobby = new Lobby();
		lobby.SetGalaxyID(lobbyID);
		lobby.SetFromCode(true);
		return lobby;
	}

	SteamDew.Log($"Invite is not a valid Galaxy Lobby ID: {inviteCode}");
	return null;
}

public void ShowInviteDialog(object lobby) 
{
	SteamFriends.ActivateGameOverlayInviteDialog(new CSteamID((ulong) lobby));
}

public void MutePlayer(string userId, bool mute) 
{
	if (mute) {
		SteamDew.Log($"Tried to mute player: {userId}; Not supported on Steam.");
	} else {
		SteamDew.Log($"Tried to unmute player: {userId}; Not supported on Steam.");
	}
}

public bool IsPlayerMuted(string userId) 
{
	return false;
}

public void ShowProfile(string userId) 
{
	SteamDew.Log($"Tried to show profile: {userId}; Not supported on Steam.");
}

} /* class SteamDewNetHelper */

} /* namespace SteamDew.SDKs */