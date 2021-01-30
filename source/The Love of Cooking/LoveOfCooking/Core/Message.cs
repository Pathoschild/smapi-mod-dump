/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using StardewValley;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace LoveOfCooking.Core
{
	public class Message
	{
		public enum Type {
			HostToPeer,
			HostToAll,
			PeerToHost,
			PeerToAll
		};

		public readonly Dictionary<string, string> Data;

		public Message() {}

		[JsonConstructor]
		public Message(KeyValuePair<string, string>[] data)
		{
			Data = data.ToDictionary(pair => pair.Key, pair => pair.Value);
		}

		public void Send(Type type)
		{
			var recipients = type switch
			{
				_ => Game1.getAllFarmhands().Select(farmer => farmer.UniqueMultiplayerID).ToArray()
			};
			ModEntry.Instance.Helper.Multiplayer.SendMessage(
				this, type.ToString(), new[] { ModEntry.Instance.ModManifest.UniqueID }, recipients);
		}
	}
}
