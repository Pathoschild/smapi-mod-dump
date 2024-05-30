/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tocseoj/StardewValleyMods
**
*************************************************/

using System.Text.RegularExpressions;
using Force.DeepCloner;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.GiantCrops;
using StardewValley.GameData.Objects;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace Tocseoj.Stardew.BigCropBonus;

internal class ModEntry : Mod
{
	internal ModConfig Config = null!;
	private SellingBonus SellingBonus = null!;
	private EatingBonus EatingBonus = null!;
	private readonly Dictionary<long, bool> isEating = [];
	private readonly Dictionary<long, ValueTuple<int, int>?> wasBonus = [];

	public override void Entry(IModHelper helper)
	{
		Config = helper.ReadConfig<ModConfig>();
		if (Config.EnableMod == false) {
			// TODO: This won't work with GMCM and toggling off after game is booted
			Monitor.Log("Mod is disabled. No bonus will be applied.", LogLevel.Debug);
			return;
		}
		ConfigMenu menu = new(Monitor, ModManifest, helper, Config);
		helper.Events.GameLoop.GameLaunched += (sender, e) => menu.Setup();


		SellingBonus = new(Monitor, ModManifest, helper, Config);
		EatingBonus = new(Monitor, ModManifest, helper, Config);

		helper.Events.Content.AssetRequested += OnAssetRequested;
		helper.Events.GameLoop.DayStarted += OnDayStarted;
		helper.Events.GameLoop.ReturnedToTitle += OnReturnToTitle;
		helper.Events.GameLoop.DayEnding += OnDayEnding;
		helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
		helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
		helper.Events.Player.InventoryChanged += OnInventoryChanged;
		helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
	}

	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
	{
		SellingBonus.InsertTemporaryBonusItems(e);

		// Tweak giant crop grow chance
		if (Config.GrowChance == 0.01f) return;
		if (e.NameWithoutLocale.IsEquivalentTo("Data/GiantCrops")) {
			e.Edit(asset => {
				var giantCrops = asset.AsDictionary<string, GiantCropData>().Data;
				foreach ((string key, GiantCropData data) in giantCrops) {
					data.Chance = Config.GrowChance;
				}
			}, AssetEditPriority.Default + 1);
		}
	}

	[EventPriority(EventPriority.High - 1)]
	private void OnDayStarted(object? sender, DayStartedEventArgs e)
	{
		SellingBonus.ClearTemporaryBonusItems();
	}

	private void OnReturnToTitle(object? sender, ReturnedToTitleEventArgs e)
	{
		SellingBonus.ClearTemporaryBonusItems();
	}

	private void OnDayEnding(object? sender, DayEndingEventArgs e)
	{
		SellingBonus.CalculateAndShipBonusItems();
	}

	private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
	{
		if (Game1.player.IsMainPlayer) return;
		if (e.FromModID == ModManifest.UniqueID && e.Type == "Tocseoj.Stardew.BigCropBonus.BonusItemsGenerated") {
			SellingBonus.ReceiveBonusItemsGenerated(e.ReadAs<Dictionary<string, ObjectData>>());
		}
	}

	private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
	{
		if (Game1.activeClickableMenu != null) {
			// IClickableMenu.drawHoverText(e.SpriteBatch, "Test in moderation.", Game1.smallFont);
			// Monitor.Log($"Active menu: {Game1.activeClickableMenu}");
			if (Game1.player.stats.Get("Book_PriceCatalogue") != 0 && Game1.activeClickableMenu is ItemGrabMenu itemGrabMenu) {
				if (itemGrabMenu.hoveredItem != null && itemGrabMenu.hoveredItem is SObject validItem) {
					Dictionary<string, int> cropList = GetBigCrops();
					// The preserve index might break with custom crops...
					string matchedBigCropId = "";
					if (cropList.ContainsKey(validItem.QualifiedItemId)) {
						matchedBigCropId = validItem.QualifiedItemId;
					} else if (cropList.ContainsKey($"(O){validItem.preservedParentSheetIndex}")) {
						matchedBigCropId = $"(O){validItem.preservedParentSheetIndex}";
					}
					if (matchedBigCropId != "") {
						float modifier = Config.SellModifier * cropList[matchedBigCropId];
						int bonusMoney = (int)(validItem.Price * modifier);

						IClickableMenu.drawToolTip(
							e.SpriteBatch,
							$"{itemGrabMenu.hoveredItem.getDescription()}(+{Math.Truncate(modifier*100)}%)",
							itemGrabMenu.hoveredItem.DisplayName,
							itemGrabMenu.hoveredItem,
							Game1.player.CursorSlotItem != null,
							moneyAmountToShowAtBottom: (validItem.Price + bonusMoney) * validItem.Stack);
					}
				}
			}
			else if (Game1.activeClickableMenu is GameMenu gameMenu) {
				IClickableMenu page = gameMenu.pages[gameMenu.currentTab];
				if (page is InventoryPage inventoryPage) {
					if (inventoryPage.hoveredItem != null && inventoryPage.hoveredItem is SObject validItem) {
						Dictionary<string, int> cropList = GetBigCrops();
						// The preserve index might break with custom crops...
						string matchedBigCropId = "";
						if (cropList.ContainsKey(validItem.QualifiedItemId)) {
							matchedBigCropId = validItem.QualifiedItemId;
						} else if (cropList.ContainsKey($"(O){validItem.preservedParentSheetIndex}")) {
							matchedBigCropId = $"(O){validItem.preservedParentSheetIndex}";
						}
						if (matchedBigCropId != "") {
							float sellModifier = Config.SellModifier * cropList[matchedBigCropId];
							float eatModifier = Config.EatModifier * cropList[matchedBigCropId];
							// int bonusMoney = (int)(validItem.Price * modifier);

							SObject hovered = (SObject)inventoryPage.hoveredItem.DeepClone();
							hovered.Price += (int)Math.Round(hovered.Price * sellModifier);
							hovered.Edibility += (int)Math.Round(hovered.Edibility * eatModifier);

							IClickableMenu.drawToolTip(
								e.SpriteBatch,
								$"{hovered.getDescription()}(+{Math.Truncate(sellModifier*100)}%)",
								hovered.DisplayName,
								hovered,
								Game1.player.CursorSlotItem != null);
						}
					}
				}
			}
			else if (Game1.activeClickableMenu is ShopMenu shopMenu) {
				if (shopMenu.hoveredItem != null) {
					// buying an item
				}
				else if (!string.IsNullOrEmpty(shopMenu.hoverText)) {
					Vector2 mousePosition = Helper.Input.GetCursorPosition().GetScaledScreenPixels();
					foreach (ClickableComponent slot in shopMenu.inventory.inventory) {
						if (slot.bounds.Contains(mousePosition)) {
							Item hoveredItem = Game1.player.Items[slot.myID];
							if (hoveredItem != null && hoveredItem is SObject validItem) {
								Dictionary<string, int> cropList = GetBigCrops();
								// The preserve index might break with custom crops...
								string matchedBigCropId = "";
								if (cropList.ContainsKey(validItem.QualifiedItemId)) {
									matchedBigCropId = validItem.QualifiedItemId;
								} else if (cropList.ContainsKey($"(O){validItem.preservedParentSheetIndex}")) {
									matchedBigCropId = $"(O){validItem.preservedParentSheetIndex}";
								}
								if (matchedBigCropId != "") {
									float modifier = Config.SellModifier * cropList[matchedBigCropId];
									int bonusMoney = (int)(validItem.Price * modifier);

									IClickableMenu.drawHoverText(
										e.SpriteBatch,
										// Melon (+10%) x1
										$"{validItem.DisplayName} (+{Math.Truncate(modifier*100)}%) x{validItem.Stack}",
										Game1.smallFont,
										moneyAmountToDisplayAtBottom: shopMenu.hoverPrice + (bonusMoney * validItem.Stack));
								}
							}
						}
					}
				}
			}
			// todo: hover toolbar
		}
	}

	private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
	{
		if (!e.IsLocalPlayer) return;

		foreach (Item item in e.Removed) {
			HandleItemRemoved(item, item.Stack, e.Player);
			// Monitor.Log($"Removed item: {item.DisplayName}");
		}
		foreach (ItemStackSizeChange stackChange in e.QuantityChanged) {
			// Monitor.Log($"Quantity changed: {stackChange.Item.DisplayName} from {stackChange.OldSize} to {stackChange.NewSize}");
			int soldCount = stackChange.OldSize - stackChange.NewSize;
			int changeCount = stackChange.OldSize - stackChange.NewSize;
			if (changeCount > 0)
				HandleItemRemoved(stackChange.Item, changeCount, e.Player);

			if (e.Player.isEating && stackChange.Item is SObject food) {
				Monitor.Log($"Eating {food.DisplayName}.");
				Dictionary<string, int> cropList = GetBigCrops();
				// The preserve index might break with custom crops...
				string matchedBigCropId = "";
				if (cropList.ContainsKey(food.QualifiedItemId)) {
					matchedBigCropId = food.QualifiedItemId;
				} else if (cropList.ContainsKey($"(O){food.preservedParentSheetIndex}")) {
					matchedBigCropId = $"(O){food.preservedParentSheetIndex}";
				}
				if (matchedBigCropId != "") {
					Monitor.Log($"Ate {food.DisplayName} which matches a giant crop {matchedBigCropId}.");
					float modifier = Config.EatModifier * cropList[matchedBigCropId];

					int staminaToHeal = (int)Math.Ceiling(food.staminaRecoveredOnConsumption() * modifier);
					int healthToHeal = (int)Math.Ceiling(food.healthRecoveredOnConsumption() * modifier);

					Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + staminaToHeal);
					Game1.player.health = Math.Min(Game1.player.maxHealth, Game1.player.health + healthToHeal);

					wasBonus[Game1.player.UniqueMultiplayerID] = (food.staminaRecoveredOnConsumption() + staminaToHeal, food.healthRecoveredOnConsumption() + healthToHeal);
				}
			}
			else if (Game1.currentSpeaker != null && stackChange.Item is SObject gift) {
				// TODO check if gifted
				Dictionary<string, int> cropList = GetBigCrops();
				// The preserve index might break with custom crops...
				string matchedBigCropId = "";
				if (cropList.ContainsKey(gift.QualifiedItemId)) {
					matchedBigCropId = gift.QualifiedItemId;
				} else if (cropList.ContainsKey($"(O){gift.preservedParentSheetIndex}")) {
					matchedBigCropId = $"(O){gift.preservedParentSheetIndex}";
				} else if (gift.Category == SObject.CookingCategory) {
					CraftingRecipe recipe = new(gift.Name, true);
					foreach (string ingredient in recipe.recipeList.Keys) {
						string? qualifiedIngredientId = ItemRegistry.QualifyItemId(ingredient);
						if (qualifiedIngredientId != null && cropList.ContainsKey(qualifiedIngredientId)) {
							matchedBigCropId = qualifiedIngredientId;
							break;
						}
					}
				}
				if (matchedBigCropId != "") {
					float modifier = Config.GiftModifier * cropList[matchedBigCropId];

					if (Game1.currentSpeaker.CanReceiveGifts()) {
						float qualityChangeMultipler = 1f;
						switch (gift.Quality)
						{
						case SObject.medQuality:
							qualityChangeMultipler = 1.1f;
							break;
						case SObject.highQuality:
							qualityChangeMultipler = 1.25f;
							break;
						case SObject.bestQuality:
							qualityChangeMultipler = 1.5f;
							break;
						}
						float friendshipChangeMultiplier = 1f;
						if (Game1.currentSpeaker.isBirthday())
						{
							friendshipChangeMultiplier = 8f;
						}
						if (Game1.currentSpeaker.getSpouse() != null && Game1.currentSpeaker.getSpouse().Equals(Game1.player))
						{
							friendshipChangeMultiplier /= 2f;
						}
						int tasteForItem = Game1.currentSpeaker.getGiftTasteForThisItem(gift);
						var friendshipAmount = tasteForItem switch {
							NPC.gift_taste_stardroptea => Math.Min(750f, 250f * friendshipChangeMultiplier),
							NPC.gift_taste_love => 80f * friendshipChangeMultiplier * qualityChangeMultipler,
							NPC.gift_taste_hate => -40f * friendshipChangeMultiplier,
							NPC.gift_taste_like => 45f * friendshipChangeMultiplier * qualityChangeMultipler,
							NPC.gift_taste_dislike => -20f * friendshipChangeMultiplier,
							_ => 20f * friendshipChangeMultiplier,
						};
						Game1.player.changeFriendship((int)Math.Ceiling(friendshipAmount * modifier), Game1.currentSpeaker);
						Monitor.Log($"The power of big crops compels {Game1.currentSpeaker.Name}. Bonus friendship gained: {friendshipAmount} * {modifier} = {friendshipAmount * modifier}");
					}
				}

			} else {
				SoldItem(e.Player, stackChange.Item, soldCount);
			}
		}
	}

	private void HandleItemRemoved(Item item, int stack, Farmer player) {
		if (player.isEating) {
			EatingBonus.AteItem(item, player, stack);
		}
		else if (Game1.currentSpeaker != null && item is SObject gift) {
			// TODO check if gifted
			Dictionary<string, int> cropList = GetBigCrops();
			// The preserve index might break with custom crops...
			string matchedBigCropId = "";
			if (cropList.ContainsKey(gift.QualifiedItemId)) {
				matchedBigCropId = gift.QualifiedItemId;
			} else if (cropList.ContainsKey($"(O){gift.preservedParentSheetIndex}")) {
				matchedBigCropId = $"(O){gift.preservedParentSheetIndex}";
			} else if (gift.Category == SObject.CookingCategory) {
				CraftingRecipe recipe = new(gift.Name, true);
				foreach (string ingredient in recipe.recipeList.Keys) {
					string? qualifiedIngredientId = ItemRegistry.QualifyItemId(ingredient);
					if (qualifiedIngredientId != null && cropList.ContainsKey(qualifiedIngredientId)) {
						matchedBigCropId = qualifiedIngredientId;
						break;
					}
				}
			}
			if (matchedBigCropId != "") {
				float modifier = Config.GiftModifier * cropList[matchedBigCropId];

				if (Game1.currentSpeaker.CanReceiveGifts()) {
					float qualityChangeMultipler = 1f;
					switch (gift.Quality)
					{
					case SObject.medQuality:
						qualityChangeMultipler = 1.1f;
						break;
					case SObject.highQuality:
						qualityChangeMultipler = 1.25f;
						break;
					case SObject.bestQuality:
						qualityChangeMultipler = 1.5f;
						break;
					}
					float friendshipChangeMultiplier = 1f;
					if (Game1.currentSpeaker.isBirthday())
					{
						friendshipChangeMultiplier = 8f;
					}
					if (Game1.currentSpeaker.getSpouse() != null && Game1.currentSpeaker.getSpouse().Equals(Game1.player))
					{
						friendshipChangeMultiplier /= 2f;
					}
					int tasteForItem = Game1.currentSpeaker.getGiftTasteForThisItem(gift);
					var friendshipAmount = tasteForItem switch {
						NPC.gift_taste_stardroptea => Math.Min(750f, 250f * friendshipChangeMultiplier),
						NPC.gift_taste_love => 80f * friendshipChangeMultiplier * qualityChangeMultipler,
						NPC.gift_taste_hate => -40f * friendshipChangeMultiplier,
						NPC.gift_taste_like => 45f * friendshipChangeMultiplier * qualityChangeMultipler,
						NPC.gift_taste_dislike => -20f * friendshipChangeMultiplier,
						_ => 20f * friendshipChangeMultiplier,
					};
					Game1.player.changeFriendship((int)Math.Ceiling(friendshipAmount * modifier), Game1.currentSpeaker);
					Monitor.Log($"The power of big crops compels {Game1.currentSpeaker.Name}. Bonus friendship gained: {friendshipAmount} * {modifier} = {friendshipAmount * modifier}");
				}
			}

		} else {
			SoldItem(player, item, stack);
		}
	}

	private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
	{
		if (Game1.player.isEating) {
			isEating[Game1.player.UniqueMultiplayerID] = true;
		} else {
			if (isEating.ContainsKey(Game1.player.UniqueMultiplayerID) && isEating[Game1.player.UniqueMultiplayerID] == true && wasBonus.ContainsKey(Game1.player.UniqueMultiplayerID) && wasBonus[Game1.player.UniqueMultiplayerID] != null) {
				Monitor.Log($"Bonus! HudMessages: {Game1.hudMessages.Count}");
				foreach (HUDMessage message in Game1.hudMessages) {
					// get number from message.mesage string using regex
					Regex regex = new(@"\d+");
					Match match = regex.Match(message.message);
					Monitor.Log($"Match: {match.Value}");
					// change match.Value to integer
					try {
						var bonus = wasBonus[Game1.player.UniqueMultiplayerID];
						if (bonus != null) {
							int stamina = bonus.Value.Item1;
							int health = bonus.Value.Item2;
							int number = Convert.ToInt32(match.Value);
							if (message.whatType == 4) {
								Monitor.Log($"Stamina: {number} -> {stamina}");
								message.message = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3116", stamina);
							} else if (message.whatType == 5) {
								Monitor.Log($"Health: {number} -> {health}");
								message.message = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3118", health);
							}
						}
					}
					catch (FormatException) {
						Monitor.Log($"Unable to convert '{match.Value}' to an integer.");
					}
				}
				wasBonus[Game1.player.UniqueMultiplayerID] = null;
			}
			isEating[Game1.player.UniqueMultiplayerID] = false;
		}
	}

	private static Dictionary<string, int> GetBigCrops()
	{
		Dictionary<string, int> cropTypeCounts = [];

		Utility.ForEachLocation(location => {
			foreach (GiantCrop giantCrop in location.resourceClumps.OfType<GiantCrop>()) {
				GiantCropData? giantCropItem = giantCrop.GetData();
				if (giantCropItem == null) continue;

				// Note: the key is the source items id (i.e. for a Giant melon, it is '(O)254' which is for Melon)
				if (!cropTypeCounts.ContainsKey(giantCropItem.FromItemId)) {
					cropTypeCounts[giantCropItem.FromItemId] = 0;
				}
				cropTypeCounts[giantCropItem.FromItemId] += 1;
			}
			return true;
		});
		return cropTypeCounts;
	}

	private void SoldItem(Farmer player, Item item, int count)
	{
		if (Game1.activeClickableMenu is ShopMenu shopMenu) {
			if (item is SObject validItem) {
				Dictionary<string, int> cropList = GetBigCrops();
				// The preserve index might break with custom crops...
				string matchedBigCropId = "";
				if (cropList.ContainsKey(validItem.QualifiedItemId)) {
					matchedBigCropId = validItem.QualifiedItemId;
				} else if (cropList.ContainsKey($"(O){validItem.preservedParentSheetIndex}")) {
					matchedBigCropId = $"(O){validItem.preservedParentSheetIndex}";
				}
				if (matchedBigCropId != "") {
					Monitor.Log($"Sold {count} {item.DisplayName}(s) for {item.sellToStorePrice()}g ea. which matches a giant crop {matchedBigCropId}.");

					float modifier = Config.SellModifier * cropList[matchedBigCropId];
					int bonusMoney = (int)(validItem.Price * modifier);
					ShopMenu.chargePlayer(player, shopMenu.currency, -bonusMoney * count);

					Monitor.Log($"Bonus money: {bonusMoney} * {count} = {bonusMoney * count}g");
				}
			}
		}
	}
	private static IEnumerable<NPC> GetSocialCharacters()
	{
		foreach (NPC npc in Utility.getAllCharacters()) {
			if (npc.CanSocialize || Game1.player.friendshipData.ContainsKey(npc.Name))
				yield return npc;
		}
	}
}
