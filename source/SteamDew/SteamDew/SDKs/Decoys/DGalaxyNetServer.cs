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

public class DGalaxyNetServer : GalaxyNetServer {

public IGameServer iGameServer;

public DGalaxyNetServer(IGameServer gameServer) : base(gameServer)
{
	SteamDew.Log($"Created Decoy for Galaxy Server");
	this.iGameServer = gameServer;

	SteamDew.LastClientType = SDKs.ClientType.Unknown;
}

~DGalaxyNetServer()
{
	SteamDew.Log($"Destroyed Decoy for Galaxy Server");
}

} /* class DGalaxyNetServer */

} /* namespace SteamDew.SDKs.Decoys */