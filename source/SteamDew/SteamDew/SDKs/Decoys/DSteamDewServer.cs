/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/myuusubi/SteamDew
**
*************************************************/

using StardewValley.Network;
using StardewValley.SDKs;

namespace SteamDew.SDKs.Decoys {

public class DSteamDewServer : GalaxyNetServer {

public IGameServer iGameServer;

public DSteamDewServer(IGameServer gameServer) : base(gameServer)
{
	SteamDew.Log($"Created Decoy for SteamDew Server");
	this.iGameServer = gameServer;

	SteamDew.LastClientType = SDKs.ClientType.Unknown;
}

~DSteamDewServer()
{
	SteamDew.Log($"Destroyed Decoy for SteamDew Server");
}

} /* class DSteamDewServer */

} /* namespace SteamDew.SDKs.Decoys */