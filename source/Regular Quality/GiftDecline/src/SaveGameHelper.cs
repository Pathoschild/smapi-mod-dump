/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/desto-git/smapi-RegularQuality
**
*************************************************/

namespace GiftDecline
{
	using System.Collections.Generic;
	using System.Linq;
	using Common;
	using StardewModdingAPI;
	using StardewValley;

	/// <summary>Methods to alter the save game data.</summary>
	internal static class SaveGameHelper
	{
		/// <summary>Identifier to store and retrive this mod's data from the save game.</summary>
		public const string Key = "GiftDecline";

		private static IDataHelper dataHelper;

		/// <summary>Save game data. This is only persisted for the main player.</summary>
		public static ModData SaveState { get; set; }

		/// <summary>Initialize the helper.</summary>
		/// <param name="helper">Instance to use for reading from and writing to the save file.</param>
		public static void Init(IDataHelper helper)
		{
			dataHelper = helper;
		}

		/// <summary>Write the current save state to the game's save file.</summary>
		public static void WriteToFile()
		{
			dataHelper.WriteSaveData(Key, SaveState);
		}

		/// <summary>Load an existing save state, or create a new one.</summary>
		public static void LoadFromFileOrInitialize()
		{
			SaveState = dataHelper.ReadSaveData<ModData>(Key) ?? new ModData();
		}

		/// <summary>Send the save state to all peers.</summary>
		public static void SyncWithPeers()
		{
			MultiplayerHelper.SendMessage(SaveState, Key);
		}

		/// <summary>Clear the list of gift taste differences.</summary>
		public static void ResetSaveState()
		{
			Logger.Trace("Resetting save state");
			NpcHelper.ResetGiftTastes();

			SaveState.GiftTasteOverwrites = new Dictionary<string, Dictionary<string, int>>();
			SaveState.GiftTasteDeclineBuffer = new Dictionary<string, Dictionary<string, int>>();
		}

		/// <summary>Set the gift taste overwrite value for an npc and item.</summary>
		/// <param name="npcName">NPC name.</param>
		/// <param name="itemId">Item id.</param>
		/// <param name="giftTaste">Overwritten gift taste value.</param>
		public static void SetGiftTaste(string npcName, string itemId, int giftTaste)
		{
			if (!SaveState.GiftTasteOverwrites.ContainsKey(npcName))
			{
				SaveState.GiftTasteOverwrites.Add(npcName, new Dictionary<string, int>());
			}

			SaveState.GiftTasteOverwrites[npcName][itemId] = giftTaste;
		}

		/// <summary>Update the buffer .</summary>
		/// <param name="npc">NPC.</param>
		/// <param name="item">Item.</param>
		public static void HandleReceivedGift(NPC npc, Item item)
		{
			string npcName = npc.Name;
			string itemId = item.ItemId;

			bool didBufferExceed = BumpGiftAmount(npcName, item);
			if (didBufferExceed)
			{
				int newGiftTaste = NpcHelper.GetReduceGiftTaste(npc, item);
				SetGiftTaste(npc.Name, itemId, newGiftTaste);
			}

			if (Context.IsMultiplayer)
			{
				// In single player, adjusting gift tastes is handled at the end of the day
				// so the player can see the actual reaction to that gift in the social tab.
				// But in Multiplayer, this delay causes some headaches when debugging some scenarios.
				// So just apply it right away. This way all players are also in sync.
				Apply();
			}

			SyncWithPeers();
		}

		/// <summary>Apply the current overwrites to the game.</summary>
		public static void Apply()
		{
			foreach (string npcName in SaveState.GiftTasteOverwrites.Keys)
			{
				NPC npc = Game1.getCharacterFromName(npcName);
				if (npc == null)
				{
					Logger.Trace("Skipping unknown NPC \"" + npcName + "\".");
					continue;
				}

				// .ToList because the save state is potentially "repaired" in this block
				foreach (string itemId in SaveState.GiftTasteOverwrites[npcName].Keys.ToList())
				{
					Item item = ItemRegistry.Create(itemId);
					if (item == null)
					{
						Logger.Trace("Skipping unknown item \"" + itemId + "\" for NPC \"" + npcName + "\".");
						continue;
					}

					int currentTaste = SaveState.GiftTasteOverwrites[npcName][itemId];
					int actualSetTaste = NpcHelper.SetGiftTasteLevel(npc, item, currentTaste);
					if (actualSetTaste != currentTaste)
					{
						// fix save state, it is not valid with the current configuration
						SaveState.GiftTasteOverwrites[npcName][itemId] = actualSetTaste;
					}
				}
			}
		}

		/// <summary>Add command to reset the save state of the mod.</summary>
		/// <param name="helper">Helper object through which the command can be added.</param>
		public static void AddResetCommand(IModHelper helper)
		{
			helper.ConsoleCommands.Add(
				"reset_gift_tastes",
				"Reset gift taste of all NPCs to their default value.",
				(string _, string[] __) => ResetCommandHandler());
		}

		private static void ResetCommandHandler()
		{
			if (SaveState == null)
			{
				Logger.Error("No data to reset. Are you still in the main menu?");
				return;
			}

			ResetSaveState();
			Logger.Info("Success");
		}

		/// <summary>Bump the amount this NPC has received this item.</summary>
		/// <returns>Wether or not the gift exceeded the declining threshold.</returns>
		private static bool BumpGiftAmount(string npcName, Item item)
		{
			string itemId = item.itemId.ToString();

			if (!SaveState.GiftTasteDeclineBuffer.ContainsKey(npcName))
			{
				SaveState.GiftTasteDeclineBuffer.Add(npcName, new Dictionary<string, int>());
			}

			int existingAmount = 0;
			if (SaveState.GiftTasteDeclineBuffer[npcName].ContainsKey(itemId))
			{
				existingAmount = SaveState.GiftTasteDeclineBuffer[npcName][itemId];
			}

			int newAmount = existingAmount + 1;
			bool didExceedBuffer = newAmount >= ConfigHelper.Config.ReduceAfterXGifts;

			if (didExceedBuffer)
			{
				string logExceeded = "Gifting threshold (" + ConfigHelper.Config.ReduceAfterXGifts + ") reached for ";
				logExceeded += npcName + ", item " + NpcHelper.GetItemString(item);
				Logger.Trace(logExceeded);
				SaveState.GiftTasteDeclineBuffer[npcName].Remove(itemId);
			}
			else
			{
				Logger.Trace(npcName + " received item " + NpcHelper.GetItemString(item) + ". Total: " + newAmount);
				SaveState.GiftTasteDeclineBuffer[npcName][itemId] = newAmount;
			}

			return didExceedBuffer;
		}
	}
}