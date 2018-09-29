using MapPings.Framework.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace MapPings.Framework {

	public class PlayerMapPing : IClickableMenu {

		//TODO ONLY ONE PING
		//public Ping[] PingsArray { get; set; }
		//public Dictionary<long, LinkedList<string>> CachedPings { get; set; }
		//public LinkedList<string> CachedPingsKey { get; set; }
		//public int MaxPings { get; set; }
		public string CachedPingKey { get; set; }
		//public bool UpdatePingsPosition { get; set; }
		public Color PlayerPingColor { get; set; }
		private readonly ObjectCache Cache;

		public PlayerMapPing() : this(Color.Red) {
		}

		public PlayerMapPing(Color color) {
			//PingsArray = new Ping[maxPings];
			Cache = MemoryCache.Default;
			this.PlayerPingColor = color;
			//CachedPingsKey = new LinkedList<string>();
			//CachedPings = new Dictionary<long, LinkedList<string>>();
			//MaxPings = maxPings;
		}

		public void AddPing(Farmer player, Vector2 position, string locationName) {
			AddPing(new Ping(player, position, locationName, PlayerPingColor));
		}

		public void AddPing(Farmer player, Vector2 position, string locationName, Color color) {
			AddPing(new Ping(player, position, locationName, color));
		}

		public void AddPing(Ping ping) {

			CacheItemPolicy policy = new CacheItemPolicy() {
				RemovedCallback = new CacheEntryRemovedCallback(CacheEntryRemovedCallback),
				AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(30)
			};

			string pingKey = $"{ping.GetHashCode()}_ping";
			Cache.Set(pingKey, ping, policy);
			if(!String.IsNullOrWhiteSpace(CachedPingKey) && Cache.Contains(CachedPingKey)) {
				Cache.Remove(CachedPingKey);
			}
			CachedPingKey = pingKey;
			//CachedPingsKey.AddFirst(pingKey);
			//if(CachedPings.ContainsKey(uniqueMultiplayerID)) {

			//if(!CachedPings.TryGetValue(ping.Player.UniqueMultiplayerID, out LinkedList<string> cachedKeysList)) {
			//	CachedPings[ping.Player.UniqueMultiplayerID] = new LinkedList<string>();
			//}

			//CachedPings[ping.Player.UniqueMultiplayerID].AddFirst(pingKey);
			//}

			//while(CachedPingsKey.Count > MaxPings) {
			//	Cache.Remove(CachedPingsKey.Last());
			//	CachedPingsKey.RemoveLast();
			//}

			//while(CachedPings[ping.Player.UniqueMultiplayerID].Count > MaxPings) {
			//	Cache.Remove(CachedPings[ping.Player.UniqueMultiplayerID].Last());
			//	CachedPings[ping.Player.UniqueMultiplayerID].RemoveLast();
			//}

		}

		public void CacheEntryRemovedCallback(CacheEntryRemovedArguments arguments) {

			if(arguments.RemovedReason == CacheEntryRemovedReason.Expired) {
				//Ping ping = arguments.CacheItem.Value as Ping;
				//CachedPings[ping.Player.UniqueMultiplayerID].Remove(arguments.CacheItem.Key);
			}

		}

		public void SetPlayerPingColor(Color color) {
			PlayerPingColor = color;
			if(!String.IsNullOrWhiteSpace(CachedPingKey) && Cache[CachedPingKey] is Ping cachedPing) {
				cachedPing.AnimatedArrowSprite.color = PlayerPingColor;
			}
		}

		public void UpdatePingPosition() {
			if(!String.IsNullOrWhiteSpace(CachedPingKey) && Cache[CachedPingKey] is Ping cachedPing) {
				cachedPing.UpdatePingPosition();
			}

		}

		public override void update(GameTime time) {
			//for(int i = 0; i < PingsArray.Length - 1; i++) {
			//	PingsArray[i].update(time);
			//}

			//foreach(LinkedList<string> cachedPingsKey in CachedPings.Values) {
			//	foreach(string pingKey in cachedPingsKey) {

			if(!String.IsNullOrWhiteSpace(CachedPingKey) && Cache[CachedPingKey] is Ping cachedPing) {
				cachedPing.update(time);
			}

			//	}
			//}

		}

		public override void draw(SpriteBatch b) {
			//for(int i = 0; i < PingsArray.Length - 1; i++) {
			//	PingsArray[i].draw(b);
			//}

			//foreach(LinkedList<string> cachedPingsKey in CachedPings.Values) {
			//	foreach(string pingKey in cachedPingsKey) {

			if(!String.IsNullOrWhiteSpace(CachedPingKey) && Cache[CachedPingKey] is Ping cachedPing) {
				cachedPing.draw(b);
			}

			//	}
			//}

		}

	}

}
