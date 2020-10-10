namespace GiftDecline
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Common;
	using StardewValley;

	/// <summary>Utility functions for gift handling.</summary>
	internal static class NpcHelper
	{
		private const char XnbFieldSeparator = '/';
		private const char XnbGiftSeparator = ' ';

		private static readonly int[] GiftTasteLevels =
		{
			NPC.gift_taste_love, NPC.gift_taste_like, NPC.gift_taste_neutral, NPC.gift_taste_dislike, NPC.gift_taste_hate,
		};

		private static readonly Dictionary<string, string> DefaultTastesXnb = new Dictionary<string, string>();
		private static readonly Dictionary<string, Dictionary<int, int>> DefaultTastesMap = new Dictionary<string, Dictionary<int, int>>();
		private static readonly Dictionary<string, Dictionary<int, int>> GiftsReceived = new Dictionary<string, Dictionary<int, int>>();

		/// <summary>Get the next lower gift taste level for an item from an NPC.</summary>
		/// <param name="npc">NPC for getting their current gift taste.</param>
		/// <param name="item">Item to reduce the interest for.</param>
		/// <returns>Gift taste level reduced by one.</returns>
		public static int GetReduceGiftTaste(NPC npc, Item item)
		{
			int tasteLevel = npc.getGiftTasteForThisItem(item);
			switch (tasteLevel)
			{
				case NPC.gift_taste_love:    return NPC.gift_taste_like;
				case NPC.gift_taste_like:    return NPC.gift_taste_neutral;
				case NPC.gift_taste_neutral: return NPC.gift_taste_dislike;
				case NPC.gift_taste_dislike: return NPC.gift_taste_hate;
				case NPC.gift_taste_hate:    return NPC.gift_taste_hate;
				default: throw new Exception("GetReduceGiftTaste: Unknown gift taste level \"" + tasteLevel + "\".");
			}
		}

		/// <summary>Set the taste level for an item of an NPC. Is capped at a configurable minimum.</summary>
		/// <param name="npc">NPC of whom to overwrite the taste level.</param>
		/// <param name="item">Item affected by the change.</param>
		/// <param name="desiredGiftTaste">Taste level to set it to.</param>
		/// <returns>The actual value it got set to.</returns>
		public static int SetGiftTasteLevel(NPC npc, Item item, int desiredGiftTaste)
		{
			int currentGiftTaste = npc.getGiftTasteForThisItem(item);
			if (desiredGiftTaste == currentGiftTaste) return currentGiftTaste;

			int itemId = item.ParentSheetIndex;
			int originalGiftTaste;

			if (!HasGiftTasteBeenOverwritten(npc.Name, itemId))
			{
				originalGiftTaste = currentGiftTaste;
				StoreDefaultGiftTasteForItem(npc.Name, itemId, currentGiftTaste);
			}
			else
			{
				originalGiftTaste = DefaultTastesMap[npc.Name][itemId];
			}

			int reduction = GetCurrentReduction(npc, item);
			int maxReduction = ConfigHelper.Config.MaxReduction;
			if (reduction >= maxReduction)
			{
				string logDontAdjust = $"Not adjusting gift taste for {npc.Name}, item {GetItemString(item)} : ";
				logDontAdjust += "MaxReduction (" + maxReduction + ") has been reached.";
				logDontAdjust += " (" + originalGiftTaste + " -> " + currentGiftTaste + ")";
				Logger.Trace(logDontAdjust);
				return currentGiftTaste;
			}

			// In case a previous configuration allowed for the taste level to drop further than is allowed now
			// Limit it to the current cap
			int clampedNewGiftTaste = GetClampedGiftTasteOverwrite(originalGiftTaste, desiredGiftTaste);
			if (clampedNewGiftTaste != desiredGiftTaste)
			{
				string logClamped = $"Limiting gift taste adjustment for {npc.Name}, item {GetItemString(item)} from ";
				logClamped += GetGiftTasteString(desiredGiftTaste) + " to " + GetGiftTasteString(clampedNewGiftTaste);
				Logger.Trace(logClamped);
				desiredGiftTaste = clampedNewGiftTaste;
			}

			if (clampedNewGiftTaste == currentGiftTaste) return currentGiftTaste;

			string logDoAdjust = $"Adjusting gift taste for {npc.Name}, item #{itemId} ({item.Name}) : ";
			logDoAdjust += GetGiftTasteString(currentGiftTaste) + " -> " + GetGiftTasteString(desiredGiftTaste);
			Logger.Trace(logDoAdjust);

			// Original index refers to the reaction text. The corresponding items for this come afterwards.
			int currentGiftTasteIndex = currentGiftTaste + 1;
			int desiredGiftTasteIndex = desiredGiftTaste + 1;

			string itemIdString = itemId.ToString();
			string[] giftTasteData = Game1.NPCGiftTastes[npc.Name].Split(XnbFieldSeparator);

			List<string> oldGiftTasteData = giftTasteData[currentGiftTasteIndex].Split(XnbGiftSeparator).ToList();
			if (oldGiftTasteData.Contains(itemIdString))
			{
				oldGiftTasteData.RemoveAt(oldGiftTasteData.IndexOf(itemIdString));
				giftTasteData[currentGiftTasteIndex] = string.Join(XnbGiftSeparator.ToString(), oldGiftTasteData);
			}

			List<string> newGiftTasteData = giftTasteData[desiredGiftTasteIndex].Split(XnbGiftSeparator).ToList();
			newGiftTasteData.Add(itemIdString);
			giftTasteData[desiredGiftTasteIndex] = string.Join(XnbGiftSeparator.ToString(), newGiftTasteData);

			Game1.NPCGiftTastes[npc.Name] = string.Join(XnbFieldSeparator.ToString(), giftTasteData);

			return desiredGiftTaste;
		}

		/// <summary>Check if a given NPC can receive gifts.</summary>
		/// <param name="npc">NPC to check.</param>
		/// <returns>Wether or not the NPC can receive gifts.</returns>
		public static bool AcceptsGifts(NPC npc)
		{
			// npc.CanSocialize -> Dwarf has this on false
			// npc.getGiftTasteForThisItem(item) -> Throws when used on characters that can't receive gifts, e.g. Gunther
			// npc.canReceiveThisItemAsGift(item) -> true for everyone
			return npc.Birthday_Day != 0; // valid birthdays start from 1
		}

		/// <summary>Save the default gift tastes of all NPCs to be able to reset them, should the need arise.</summary>
		public static void StoreDefaultGiftTastes()
		{
			if (DefaultTastesXnb.Count > 0) return;

			foreach (string name in Game1.NPCGiftTastes.Keys)
			{
				DefaultTastesXnb.Add(name, Game1.NPCGiftTastes[name]);
			}
		}

		/// <summary>Reset the gift taste of all NPCs.</summary>
		public static void ResetGiftTastes()
		{
			if (DefaultTastesXnb.Count == 0) throw new Exception("Cannot restore default tastes. They are not yet stored.");

			foreach (string name in DefaultTastesXnb.Keys)
			{
				Game1.NPCGiftTastes[name] = DefaultTastesXnb[name];
			}
		}

		/// <summary>Check wether the friendship level differs from the last known state.</summary>
		/// <param name="npc">NPC whose friendship to store.</param>
		/// <param name="item">Item to maybe have received.</param>
		/// <returns>Wether or not the friendship level differs.</returns>
		public static bool HasJustReceivedGift(NPC npc, Item item)
		{
			var giftedItems = Game1.player.giftedItems;
			if (!giftedItems.ContainsKey(npc.Name)) return false;

			var giftedToNpc = giftedItems[npc.Name];
			if (!giftedToNpc.ContainsKey(item.ParentSheetIndex)) return false;

			if (!GiftsReceived.ContainsKey(npc.Name)) return true;
			if (!GiftsReceived[npc.Name].ContainsKey(item.ParentSheetIndex)) return true;

			int lastAmount = GiftsReceived[npc.Name][item.ParentSheetIndex];
			int currentAmount = giftedToNpc[item.ParentSheetIndex];

			bool didReceiveGift = lastAmount != currentAmount;
			if (didReceiveGift)
			{
				StoreReceivedGift(npc.Name, item.ParentSheetIndex, currentAmount);
			}

			return didReceiveGift;
		}

		/// <summary>Get and store the current friendship level of a list of NPCs.</summary>
		/// <param name="npcCollection">NPCs whose friendship to store.</param>
		public static void StoreAmountOfGiftsReceived(IEnumerable<NPC> npcCollection)
		{
			IEnumerator<NPC> characters = npcCollection.GetEnumerator();
			while (characters.MoveNext())
			{
				NPC npc = characters.Current;

				var giftedItems = Game1.player.giftedItems;
				if (!giftedItems.ContainsKey(npc.Name)) continue;

				var giftedToNpc = giftedItems[npc.Name];
				foreach (int itemId in giftedToNpc.Keys)
				{
					StoreReceivedGift(npc.Name, itemId, giftedToNpc[itemId]);
				}
			}
		}

		/// <summary>Get a readable version of an item.</summary>
		/// <param name="item">Item to stringify.</param>
		/// <returns>Stringified item.</returns>
		public static string GetItemString(Item item)
		{
			return "#" + item.ParentSheetIndex + " (" + item.Name + ")";
		}

		private static void StoreReceivedGift(string npcName, int itemId, int amount)
		{
			if (!GiftsReceived.ContainsKey(npcName))
			{
				GiftsReceived.Add(npcName, new Dictionary<int, int>());
			}

			GiftsReceived[npcName][itemId] = amount;
		}

		private static void StoreDefaultGiftTasteForItem(string npcName, int itemId, int giftTaste)
		{
			Logger.Trace("Storing default gift taste: " + npcName + " -> #" + itemId + " = " + giftTaste);
			if (!DefaultTastesMap.ContainsKey(npcName))
			{
				DefaultTastesMap.Add(npcName, new Dictionary<int, int>());
			}

			DefaultTastesMap[npcName][itemId] = giftTaste;
		}

		private static bool HasGiftTasteBeenOverwritten(string npcName, int itemId)
		{
			if (!DefaultTastesMap.ContainsKey(npcName)) return false;
			if (!DefaultTastesMap[npcName].ContainsKey(itemId)) return false;
			return true;
		}

		/// <summary>Get how by how much the teaste of a gift has been reduced already.</summary>
		private static int GetCurrentReduction(NPC npc, Item item)
		{
			string npcName = npc.Name;
			int itemId = item.ParentSheetIndex;
			if (!HasGiftTasteBeenOverwritten(npcName, itemId)) return 0;

			int originalTasteLevel = DefaultTastesMap[npc.Name][itemId];
			int currentTasteLevel = npc.getGiftTasteForThisItem(item);

			int originalTasteIndex = Array.IndexOf(GiftTasteLevels, originalTasteLevel);
			int currentTasteIndex = Array.IndexOf(GiftTasteLevels, currentTasteLevel);
			return currentTasteIndex - originalTasteIndex;
		}

		/// <summary>Get the target taste reduction, limited by the config setting.</summary>
		private static int GetClampedGiftTasteOverwrite(int fromTaste, int toTaste)
		{
			int fromIndex = Array.IndexOf(GiftTasteLevels, fromTaste);
			int toIndex = Array.IndexOf(GiftTasteLevels, toTaste);

			int reduction = toIndex - fromIndex;
			int maxReduction = ConfigHelper.Config.MaxReduction;
			if (reduction > maxReduction)
			{
				toIndex = fromIndex + maxReduction;
			}

			return GiftTasteLevels[toIndex];
		}

		/// <summary>Get a readable version of a gift taste.</summary>
		private static string GetGiftTasteString(int tasteLevel)
		{
			string label;
			switch (tasteLevel)
			{
				case NPC.gift_taste_love:    label = "Love"; break;
				case NPC.gift_taste_like:    label = "Like"; break;
				case NPC.gift_taste_neutral: label = "Neutral"; break;
				case NPC.gift_taste_dislike: label = "Dislike"; break;
				case NPC.gift_taste_hate:    label = "Hate"; break;
				default: throw new Exception("GetGiftTasteString: Unknown gift taste level \"" + tasteLevel + "\".");
			}

			return label + "(" + tasteLevel + ")";
		}
	}
}