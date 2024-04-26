/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BadScientist/ConfirmGifts
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Quests;
using StardewValley.SpecialOrders;
using StardewValley.SpecialOrders.Objectives;
using ConfirmGifts.Frameworks.Config;
using ConfirmGifts.Frameworks.ConfigMenu;


namespace ConfirmGifts
{
	/// <summary>The mod entry point.</summary>
	internal sealed class ModEntry : Mod
	{
		/*********
        ** Private variables
        *********/
		private ModConfig Config = new ModConfig();

		/*********
        ** Public methods
        *********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			this.Config = this.Helper.ReadConfig<ModConfig>();
			helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
			helper.Events.Input.ButtonPressed += this.OnButtonPressed;
		}

		/*********
        ** Private methods
        *********/
		/// <summary>Raised after the game is launched.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
		{
			IGenericModConfigMenuApi? configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (configMenu is null) {return;}

			configMenu.Register(this.ModManifest, () => this.Config = new ModConfig(), () => this.Helper.WriteConfig(this.Config));
			configMenu.AddBoolOption(this.ModManifest,
				() => this.Config.ShowLikes,
				value => this.Config.ShowLikes = value,
				() => Helper.Translation.Get("config.show-likes.label"),
				() => Helper.Translation.Get("config.show-likes.desc"));
		}

		/// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
		{
			//ignore if player hasn't loaded a save yet
			if (!Context.IsWorldReady) {return;}

			Farmer who = Game1.player;
			if (!e.Button.IsActionButton() || !who.CanMove || who.isRidingHorse() || who.ActiveObject == null || !who.ActiveObject.canBeGivenAsGift() || who.ActiveObject.questItem.Get()) {return;}

			Vector2[] tilesToCheck = GetTilesToCheck(e.Cursor.Tile, who);

			NPC targetNPC;
			int i = 0;
			do
			{
				targetNPC = Game1.currentLocation.isCharacterAtTile(tilesToCheck[i]);
				if (targetNPC != null && targetNPC.CanReceiveGifts()) {break;}
				i++;
			} while (i < tilesToCheck.Length);

			if (targetNPC == null || !targetNPC.CanReceiveGifts() || WillCompleteQuest(who, targetNPC)) {return;}

			string likes = Config.ShowLikes ? $"({GetItemLikeLevel(who.ActiveObject, targetNPC)}) " : "";
			string question = Helper.Translation.Get("dialogue.confirmation", new {itemName = who.ActiveObject.DisplayName, likesText = likes, nPCName = targetNPC.displayName});
			who.currentLocation.createQuestionDialogue(question,
				who.currentLocation.createYesNoResponses(),
				delegate (Farmer _, string answer) {if (answer == "Yes") {targetNPC.tryToReceiveActiveObject(who);}});
		}

		private Vector2[] GetTilesToCheck(Vector2 cursorPosition, Farmer who)
		{
			Vector2 playerPosition = who.Tile;
			int direction = who.FacingDirection;

			bool cursorInRange = false;
			Vector2[] allDirections = Utility.DirectionsTileVectorsWithDiagonals;
			for (int i = 0; i < allDirections.Length; i++)
			{
				Vector2 tile = playerPosition + allDirections[i];
				if (Math.Abs(cursorPosition.X - tile.X) <= 1 && Math.Abs(cursorPosition.Y - tile.Y) <= 1)
				{
					cursorInRange = true;
					break;
				}
			}

			int num = cursorInRange ? 5 : 4;
			
			Vector2[] result = new Vector2[num];
			Vector2[] directions = Utility.DirectionsTileVectors;

			if (cursorInRange) {result[0] = cursorPosition;}
			result[num - 4] = playerPosition;
			result[num - 3] = result[num - 4] + directions[direction];
			result[num - 2] = result[num - 3] + directions[(direction + 1) % 4];
			result[num - 1] = result[num - 3] + directions[(direction + 3) % 4];

			return result;
		}

		private string GetItemLikeLevel(Item item, NPC who)
		{
			int level = who.getGiftTasteForThisItem(item);
			switch (level)
			{
				case 0:
					return Helper.Translation.Get("likes.loved");
				case 2:
					return Helper.Translation.Get("likes.liked");
				case 4:
					return Helper.Translation.Get("likes.disliked");
				case 6:
					return Helper.Translation.Get("likes.hated");
				case 7:
					return "=";
				case 8:
					return Helper.Translation.Get("likes.neutral");
				default:
					return "?";
			}
		}

		private bool WillCompleteQuest(Farmer who, NPC whoElse)
		{
			for (int i = 0; i < who.questLog.Count; i++)
			{
				Quest quest = who.questLog[i];
				
				if (quest.GetType() == typeof(ItemDeliveryQuest))
				{
					ItemDeliveryQuest itemDeliveryQuest = (ItemDeliveryQuest)quest;
					if (itemDeliveryQuest.target.Get() == whoElse.Name && itemDeliveryQuest.ItemId.Get() == who.ActiveObject.QualifiedItemId) {return true;}
				}
				
				if (quest.GetType() == typeof(LostItemQuest))
				{
					LostItemQuest lostItemQuest = (LostItemQuest)quest;
					if (lostItemQuest.npcName.Get() == whoElse.Name && lostItemQuest.ItemId.Get() == who.ActiveObject.QualifiedItemId) {return true;}
				}

				if (quest.GetType() == typeof(SecretLostItemQuest))
				{
					SecretLostItemQuest secretLostItemQuest = (SecretLostItemQuest)quest;
					if (secretLostItemQuest.npcName.Get() == whoElse.Name && secretLostItemQuest.ItemId.Get() == who.ActiveObject.QualifiedItemId) {return true;}
				}
			}

			for (int i = 0; i < who.team.specialOrders.Count; i++)
			{
				SpecialOrder specialOrder = who.team.specialOrders[i];

				for (int j = 0; j < specialOrder.objectives.Count; j++)
				{
					OrderObjective objective = specialOrder.objectives[j];

					if (objective.GetType() == typeof(DeliverObjective))
					{
						DeliverObjective deliverObjective = (DeliverObjective)objective;
						if (deliverObjective.OnItemDelivered(who, whoElse, who.ActiveObject, true) > 0) {return true;}
					}
				}
			}

			return false;
		}
	}
}