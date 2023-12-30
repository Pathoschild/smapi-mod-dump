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

namespace SteamDew.SDKs.Decoys {

public class DInviteClient : GalaxyNetClient {

public GalaxyID LobbyID;

public DInviteClient(GalaxyID lobby) : base(lobby)
{
	SteamDew.Log($"Created Decoy for Invite Client");
	this.LobbyID = lobby;

	SteamDew.LastClientType = SDKs.ClientType.Unknown;
}

~DInviteClient()
{
	SteamDew.Log($"Destroyed Decoy for Invite Client");
}

} /* class DInviteClient */

} /* namespace SteamDew.SDKs.Decoys */