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
	using Common;
	using StardewModdingAPI;
	using StardewModdingAPI.Events;
	using StardewValley;
	using StardewValley.Quests;

	/// <summary>Main class.</summary>
	internal class ModEntry : Mod
	{
		private bool isInDialog = false;

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			Logger.Init(this.Monitor);
			MultiplayerHelper.Init(this.Helper.Multiplayer);
			SaveGameHelper.Init(this.Helper.Data);
			ConfigHelper.Init(this.Helper);

			SaveGameHelper.AddResetCommand(helper);

			helper.Events.Display.MenuChanged += (object sender, MenuChangedEventArgs e) =>
				EventHandler.OnMenuChanged(ref this.isInDialog, e);

			helper.Events.Multiplayer.ModMessageReceived += (object sender, ModMessageReceivedEventArgs e) =>
				EventHandler.OnModMessageReceived(e);

			helper.Events.Multiplayer.PeerConnected += (object sender, PeerConnectedEventArgs e) =>
				EventHandler.OnPeerConnected(e);

			helper.Events.Player.InventoryChanged += (object sender, InventoryChangedEventArgs e) =>
				EventHandler.OnInventoryChanged(this.OnItemRemoved, e);

			helper.Events.Player.Warped += (object sender, WarpedEventArgs e) =>
				EventHandler.OnWarped();

			helper.Events.GameLoop.DayEnding += (object sender, DayEndingEventArgs e) =>
				EventHandler.OnDayEnding();

			helper.Events.GameLoop.ReturnedToTitle += (object sender, ReturnedToTitleEventArgs e) =>
				EventHandler.OnReturnedToTitle();

			helper.Events.GameLoop.Saving += (object sender, SavingEventArgs e) =>
				EventHandler.OnSaving();

			helper.Events.GameLoop.SaveLoaded += (object sender, SaveLoadedEventArgs e) =>
				EventHandler.OnSaveLoaded();

			helper.Events.World.NpcListChanged += (object sender, NpcListChangedEventArgs e) =>
				EventHandler.OnNpcListChanged(e);
		}

		private void OnItemRemoved(Item item)
		{
			if (!this.isInDialog) return;

			if (!(item is StardewValley.Object)) return;
			if (!item.canBeGivenAsGift()) return; // e.g. Tools or any placable object

			IEnumerator<NPC> npcEnumerator = Game1.player.currentLocation.characters.GetEnumerator();
			while (npcEnumerator.MoveNext())
			{
				NPC npc = npcEnumerator.Current;
				if (NpcHelper.AcceptsGifts(npc) && NpcHelper.HasJustReceivedGift(npc, item))
				{
					Logger.Trace(npc.Name + " received gift #" + item.ItemId + " (" + item.Name + ")");
					SaveGameHelper.HandleReceivedGift(npc, item);
					return;
				}
			}

			IEnumerator<Quest> questEnumerator = Game1.player.questLog.GetEnumerator();
			while (questEnumerator.MoveNext())
			{
				// Daily quests have no ID
				if (questEnumerator.Current.id.Value == null && questEnumerator.Current is ItemDeliveryQuest itemDeliveryQuest && itemDeliveryQuest.hasReward())
				{
					// item.ItemId is just a number, while the quest ItemId has a prefix
					if (itemDeliveryQuest.ItemId.Value == $"(O){item.ItemId}")
					{
						Logger.Trace("Handed over quest item: " + item.Name);
						return;
					}
				}
			}

			Logger.Trace("It appears a gift has been given to someone, but I can't determine to whom :(");
			Logger.Trace("Maybe it was a non-daily quest item? (" + item.Name + ")");
		}
	}
}