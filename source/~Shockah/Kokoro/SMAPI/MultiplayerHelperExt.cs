/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;

namespace Shockah.Kokoro.SMAPI;

public static class MultiplayerHelperExt
{
	public static Farmer GetPlayer(this IMultiplayerPeer peer)
		=> Game1.getAllFarmers().First(p => p.UniqueMultiplayerID == peer.PlayerID);

	public static Farmer GetPlayer(this ModMessageReceivedEventArgs args)
		=> Game1.getAllFarmers().First(p => p.UniqueMultiplayerID == args.FromPlayerID);
}