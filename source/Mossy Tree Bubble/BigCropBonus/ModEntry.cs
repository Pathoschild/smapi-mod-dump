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
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.GiantCrops;
using StardewValley.GameData.Objects;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace Tocseoj.Stardew.BigCropBonus
{
  public sealed class ModConfig {
		/// <summary>Whether to enable test mode, which makes giant crops always spawn (where valid).</summary>
		public bool TestMode { get; set; } = false;

		public KeybindList TraceLog { get; set; } = new KeybindList(SButton.T);

		/// <summary>The percent increase in value of giant crops.</summary>
		public float PercentIncrease { get; set; } = 0.1f;
	}

	/// <summary>The mod entry point.</summary>
	internal sealed class ModEntry : Mod {
		/*********
		** Fields
		*********/
		/// <summary>The mod configuration.</summary>
		private ModConfig Config = null!; // set in Entry

		/// <summary>Objects that need to be created using AssetRequested.</summary>
		private readonly Dictionary<string, ObjectData> objectsNeedingCreated = new();

		/// <summary>Shipping bins in the world.</summary>
		private readonly Dictionary<long, List<ValueTuple<Inventory?,Chest?>>> cachedShippingBins = new();

		private readonly Dictionary<long, bool> isEating = new();
		private readonly Dictionary<long, ValueTuple<int, int>?> wasBonus = new();

		/*********
		** Public methods
		*********/
		/// <inheritdoc/>
		public override void Entry(IModHelper helper) {
			Config = helper.ReadConfig<ModConfig>();

			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.Content.AssetRequested += OnAssetRequested;
			helper.Events.GameLoop.DayEnding += OnDayEnding;
			helper.Events.GameLoop.DayStarted += OnDayStarted;
			helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
			helper.Events.Player.InventoryChanged += OnInventoryChanged;
			helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			if (Config.TestMode) {
				Monitor.Log("Test mode is enabled. Giant crops will always spawn.", LogLevel.Debug);
				helper.Events.Input.ButtonPressed += OnButtonPressed;
			}
		}

		/*********
		** Private methods
		*********/
		/// <inheritdoc cref="IContentEvents.GameLaunched"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e){
		 		// get Generic Mod Config Menu's API (if it's installed)
				var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
				if (configMenu is null)
						return;

				// register mod
				configMenu.Register(
						mod: ModManifest,
						reset: () => Config = new ModConfig(),
						save: () => Helper.WriteConfig(Config)
				);

				// add some config options
				configMenu.AddBoolOption(
						mod: ModManifest,
						name: () => "Test Mode",
						tooltip: () => "Whether to enable test mode, which makes giant crops always spawn (where valid).",
						getValue: () => Config.TestMode,
						setValue: value => Config.TestMode = value
				);
				configMenu.AddKeybindList(
						mod: ModManifest,
						name: () => "Trace to Log",
						tooltip: () => "Keybind for debugging.",
						getValue: () => Config.TraceLog,
						setValue: value => Config.TraceLog = value
				);
				configMenu.AddNumberOption(
						mod: ModManifest,
						name: () => "Percent (%) Increase",
						tooltip: () => "The percent (%) increase in value of giant crops.",
						getValue: () => (float)Math.Truncate(Config.PercentIncrease * 100),
						setValue: value => Config.PercentIncrease = (float)Math.Round(value / 100, 2)
				);
		}

		/// <inheritdoc cref="IContentEvents.AssetRequested"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
			if (e.NameWithoutLocale.IsEquivalentTo("Data/GiantCrops")) {
				e.Edit(asset => {
					var giantCrops = asset.AsDictionary<string, GiantCropData>().Data;
					foreach ((string key, GiantCropData data) in giantCrops) {
						if (Config.TestMode) {
							data.Chance = 1;
						}
					}
				}, AssetEditPriority.Default + 1);
			} else if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects")) {
				if (objectsNeedingCreated.Count == 0)
					return;

				e.Edit(asset => {
					var objects = asset.AsDictionary<string, ObjectData>().Data;
					foreach ((string key, ObjectData data) in objectsNeedingCreated) {
						objects[key] = data;
					}
					objectsNeedingCreated.Clear();
				});
			}
		}

		/// <inheritdoc cref="IGameLoopEvents.DayStarted"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		[EventPriority(EventPriority.High - 1)]
		private void OnDayStarted(object? sender, DayStartedEventArgs e) {
			objectsNeedingCreated.Clear();
			Helper.GameContent.InvalidateCache("Data/Objects");
		}

		/// <inheritdoc cref="IGameLoopEvents.DayEnding"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnDayEnding(object? sender, DayEndingEventArgs e) {
			if (!Game1.player.IsMainPlayer && !Game1.player.useSeparateWallets) return;

			Dictionary<string, int>? cropList = HowManyGiantCrops();
			if (cropList == null) return;

			// These two dictionaries have matching keys (make this a tuple/class?)
			Dictionary<string, SObject> cachedObjects = new();
			Dictionary<string, float> totalValue = new();
			Dictionary<string, int> totalQuantity = new();

			Inventory? primaryBin = cachedShippingBins[Game1.player.UniqueMultiplayerID].First().Item1;
			Chest? primaryChest = cachedShippingBins[Game1.player.UniqueMultiplayerID].First().Item2; //can you even remove your shipping bin?
			// TODO: Support multiplayer (when useSeparateWallets is true)
			foreach ((Inventory? shippingBin, Chest? miniBin) in cachedShippingBins[Game1.player.UniqueMultiplayerID]) {
				if (miniBin != null && miniBin.separateWalletItems.TryGetValue(Game1.player.UniqueMultiplayerID, out Inventory items)) {
					// items = miniBin.separateWalletItems.TryGetValue(Game1.player.UniqueMultiplayerID, out Inventory inv) ? inv : miniBin.Items;
					// print all item names
					Monitor.Log($"Items in mini shipping bin: {string.Join(", ", items.Select(item => item.DisplayName))}");
					foreach (Item item in items) {
						if (item is SObject validItem) {
							// The preserve index might break with custom crops...
							string matchedBigCropId = "";
							string cachedIdentifier = "";
							if (cropList.ContainsKey(validItem.QualifiedItemId)) {
								matchedBigCropId = validItem.QualifiedItemId;
								cachedIdentifier = validItem.QualifiedItemId;
							} else if (cropList.ContainsKey($"(O){validItem.preservedParentSheetIndex}")) {
								matchedBigCropId = $"(O){validItem.preservedParentSheetIndex}";
								cachedIdentifier = $"{validItem.QualifiedItemId}x{validItem.preservedParentSheetIndex}";
							}

							if (matchedBigCropId != "") {
								Monitor.Log($"Found {validItem.Name} in mini shipping bin which matches {matchedBigCropId}.");

								// hmm, this is all i needed to do huh...
								// validItem.Price += (int)(validItem.Price * Config.PercentIncrease);

								// uh... how do we handle unique stacks?
								// just going to include quality as the second part of the identifier
								// but this will be very fragile code
								string comboIdentifier = $"Q{validItem.Quality}_{validItem.QualifiedItemId}";
								if (!totalValue.ContainsKey(comboIdentifier)) {
									totalQuantity[comboIdentifier] = 0;
									float modifier = Config.PercentIncrease * cropList[matchedBigCropId];
									totalValue[comboIdentifier] = modifier;
									cachedObjects[comboIdentifier] = validItem;
								}
								// quantity to re-add to primaryBin later
								// TODO: Each player is creating a copy of the bonus which duplicates the value
								Monitor.Log($"Reducing {validItem.Name} by {validItem.Stack} in mini shipping bin.");
								totalQuantity[comboIdentifier] += items.ReduceId(validItem.QualifiedItemId, validItem.Stack);
								// todo how to reduce a net inventory?
							}
						}
					}
					items.RemoveEmptySlots();
				}
				if (shippingBin != null) {
					Monitor.Log($"Items in shipping bin: {string.Join(", ", shippingBin.Select(item => item.DisplayName))}");
					foreach (Item item in shippingBin) {
						if (item is SObject validItem) {
							// The preserve index might break with custom crops...
							string matchedBigCropId = "";
							string cachedIdentifier = "";
							if (cropList.ContainsKey(validItem.QualifiedItemId)) {
								matchedBigCropId = validItem.QualifiedItemId;
								cachedIdentifier = validItem.QualifiedItemId;
							} else if (cropList.ContainsKey($"(O){validItem.preservedParentSheetIndex}")) {
								matchedBigCropId = $"(O){validItem.preservedParentSheetIndex}";
								cachedIdentifier = $"{validItem.QualifiedItemId}x{validItem.preservedParentSheetIndex}";
							}

							if (matchedBigCropId != "") {
								Monitor.Log($"Found {validItem.Name} in shipping bin which matches {matchedBigCropId}.");

								// hmm, this is all i needed to do huh...
								// validItem.Price += (int)(validItem.Price * Config.PercentIncrease);

								// uh... how do we handle unique stacks?
								// just going to include quality as the second part of the identifier
								// but this will be very fragile code
								string comboIdentifier = $"Q{validItem.Quality}_{validItem.QualifiedItemId}";
								if (!totalValue.ContainsKey(comboIdentifier)) {
									totalQuantity[comboIdentifier] = 0;
									float modifier = Config.PercentIncrease * cropList[matchedBigCropId];
									totalValue[comboIdentifier] = modifier;
									cachedObjects[comboIdentifier] = validItem;
								}
								// quantity to re-add to primaryBin later
								// TODO: Each player is creating a copy of the bonus which duplicates the value
								// todo how to reduce a net inventory?
								Monitor.Log($"Reducing {validItem.Name} by {validItem.Stack} in shipping bin.");
								totalQuantity[comboIdentifier] += shippingBin.ReduceId(validItem.QualifiedItemId, validItem.Stack);
							}
						}
					}
				}
			}

			foreach ((string refItemId, float refItemValue) in totalValue) {
				string generatedItemId = $"Tocseoj.BigCropBonus_{refItemId}";
				ObjectData generatedObjectData = new() {
					Name = $"{cachedObjects[refItemId].Name} Bonus",
					DisplayName = $"{Math.Round(refItemValue * 100)}% big crop bonus",
					Description = "Your bonus for having a Giant Crop.",
					Type = cachedObjects[refItemId].Type,
					Category = cachedObjects[refItemId].Category,
					Price = (int)Math.Ceiling(cachedObjects[refItemId].Price * refItemValue),
					Texture = null,
					SpriteIndex = 26,
				};
				objectsNeedingCreated[generatedItemId] = generatedObjectData;
			}
			Helper.GameContent.InvalidateCache("Data/Objects");
			foreach (var refItem in totalValue) {
				string generatedItemId = $"Tocseoj.BigCropBonus_{refItem.Key}";
				SObject refItemObject = cachedObjects[refItem.Key];
				refItemObject.Stack = totalQuantity[refItem.Key];
        SObject bonus = new(generatedItemId, totalQuantity[refItem.Key], false, -1, cachedObjects[refItem.Key].Quality);
				if (primaryBin != null) {
					Monitor.Log($"Adding {totalQuantity[refItem.Key]} {refItemObject.Name} to primary shipping bin.");
					primaryBin.Add(refItemObject);
					primaryBin.Add(bonus);
				}
				else if (primaryChest != null) {
					Monitor.Log($"Adding {totalQuantity[refItem.Key]} {refItemObject.Name} to primary chest.");
					primaryChest.addItem(refItemObject);
					primaryChest.addItem(bonus);
				}
			}
			cachedShippingBins.Clear();

			Monitor.Log($"Total value: {string.Join(", ", totalValue.Select(pair => $"{pair.Key}: {pair.Value}"))}");
		}

		/// <summary>Get all giant crops in the game.</summary>
		private Dictionary<string, int>? HowManyGiantCrops() {
			if (!Game1.player.IsMainPlayer && !Game1.player.useSeparateWallets) return null;

			Dictionary<string, int> cropTypeCounts = new();
			Dictionary<string, string> cropTypeNames = new();

			// Looping through all locations and not just farm types to support any mods that allow crops to grow elsewhere
			// Plus this is only going to be running on day end so it should be fine
			cachedShippingBins.Clear();
			if (!cachedShippingBins.ContainsKey(Game1.player.UniqueMultiplayerID))
				cachedShippingBins[Game1.player.UniqueMultiplayerID] = new();
			Utility.ForEachLocation(location => {
				foreach (GiantCrop giantCrop in location.resourceClumps.OfType<GiantCrop>()) {
					GiantCropData? giantCropItem = giantCrop.GetData();
					if (giantCropItem != null) {
						// Note the key is the source items id (for a Giant melon, it is '(O)254')
						if (!cropTypeCounts.ContainsKey(giantCropItem.FromItemId)) {
							cropTypeCounts[giantCropItem.FromItemId] = 0;
							cropTypeNames[giantCropItem.FromItemId] = giantCropItem.Condition;
						}
						cropTypeCounts[giantCropItem.FromItemId]++;
					}
				}
				// cache Farm location shippingBins and Object.Chests where SpecialChestType is Chest.SpecialChestTypes.MiniShippingBin
				if (location is Farm farm) {
					if (Game1.getFarm() == farm) {
						cachedShippingBins[Game1.player.UniqueMultiplayerID].Insert(0, ((Inventory)farm.getShippingBin(Game1.player), null));
					} else {
						cachedShippingBins[Game1.player.UniqueMultiplayerID].Add(((Inventory)farm.getShippingBin(Game1.player), null));
					}
				}
				// getting all mini shipping bins
				foreach (SObject objects in location.objects.Values) {
					if (objects is Chest chest && chest.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin) {
						if (Game1.player.useSeparateWallets && chest.separateWalletItems.TryGetValue(Game1.player.UniqueMultiplayerID, out Inventory items)) {
							Monitor.Log($"Found separate wallet items for {Game1.player.UniqueMultiplayerID}.");
							cachedShippingBins[Game1.player.UniqueMultiplayerID].Add((null, chest));
						}
					}
				}

				return true;
			});
			Monitor.Log($"Giant crops count: {string.Join(", ", cropTypeCounts.Select(pair => $"{pair.Key}: {pair.Value}"))}");
			Monitor.Log($"Giant crops conditions: {string.Join(", ", cropTypeNames.Select(pair => $"{pair.Key}: {pair.Value}"))}");
			return cropTypeCounts;
		}

		/// <inheritdoc cref="IInputEvents.ButtonPressed"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnButtonPressed(object? sender, ButtonPressedEventArgs e) {
			if (Config.TestMode && Config.TraceLog.JustPressed()) {
				HowManyGiantCrops();
				Game1.player.health = 1;
				Game1.player.Stamina = 1;
				Monitor.Log($"Number of shipping bins: {cachedShippingBins.Aggregate(0, (acc, pair) => acc + pair.Value.Count)}");
			}
		}

		/// <inheritdoc cref="IDisplayEvents.RenderedActiveMenu"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e) {
			if (Game1.activeClickableMenu != null) {
				// IClickableMenu.drawHoverText(e.SpriteBatch, "Test in moderation.", Game1.smallFont);
				// Monitor.Log($"Active menu: {Game1.activeClickableMenu}");
				if (Game1.player.stats.Get("Book_PriceCatalogue") != 0 && Game1.activeClickableMenu is ItemGrabMenu itemGrabMenu) {
					if (itemGrabMenu.hoveredItem != null && itemGrabMenu.hoveredItem is SObject validItem) {
						Dictionary<string, int> cropList = GetGiantCrops();
						// The preserve index might break with custom crops...
						string matchedBigCropId = "";
						if (cropList.ContainsKey(validItem.QualifiedItemId)) {
							matchedBigCropId = validItem.QualifiedItemId;
						} else if (cropList.ContainsKey($"(O){validItem.preservedParentSheetIndex}")) {
							matchedBigCropId = $"(O){validItem.preservedParentSheetIndex}";
						}
						if (matchedBigCropId != "") {
							float modifier = Config.PercentIncrease * cropList[matchedBigCropId];
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
							Dictionary<string, int> cropList = GetGiantCrops();
							// The preserve index might break with custom crops...
							string matchedBigCropId = "";
							if (cropList.ContainsKey(validItem.QualifiedItemId)) {
								matchedBigCropId = validItem.QualifiedItemId;
							} else if (cropList.ContainsKey($"(O){validItem.preservedParentSheetIndex}")) {
								matchedBigCropId = $"(O){validItem.preservedParentSheetIndex}";
							}
							if (matchedBigCropId != "") {
								float modifier = Config.PercentIncrease * cropList[matchedBigCropId];
								// int bonusMoney = (int)(validItem.Price * modifier);

								SObject hovered = (SObject)inventoryPage.hoveredItem.DeepClone();
								hovered.Price += (int)Math.Round(hovered.Price * modifier);
								hovered.Edibility += (int)Math.Round(hovered.Edibility * modifier);

								IClickableMenu.drawToolTip(
									e.SpriteBatch,
									$"{hovered.getDescription()}(+{Math.Truncate(modifier*100)}%)",
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
									Dictionary<string, int> cropList = GetGiantCrops();
									// The preserve index might break with custom crops...
									string matchedBigCropId = "";
									if (cropList.ContainsKey(validItem.QualifiedItemId)) {
										matchedBigCropId = validItem.QualifiedItemId;
									} else if (cropList.ContainsKey($"(O){validItem.preservedParentSheetIndex}")) {
										matchedBigCropId = $"(O){validItem.preservedParentSheetIndex}";
									}
									if (matchedBigCropId != "") {
										float modifier = Config.PercentIncrease * cropList[matchedBigCropId];
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

		/// <inheritdoc cref="IPlayerEvents.InventoryChanged"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e) {
			if (!e.IsLocalPlayer) return;

			foreach (Item item in e.Removed) {
				// Monitor.Log($"Removed item: {item.DisplayName}");
				if (isEating.ContainsKey(Game1.player.UniqueMultiplayerID) && isEating[Game1.player.UniqueMultiplayerID] == true && item is SObject food) {
					Monitor.Log($"TODO: Eating {food.DisplayName}.");
				}
				SoldItem(e.Player, item, item.Stack);
			}
			foreach (ItemStackSizeChange stackChange in e.QuantityChanged) {
				// Monitor.Log($"Quantity changed: {stackChange.Item.DisplayName} from {stackChange.OldSize} to {stackChange.NewSize}");
				int soldCount = stackChange.OldSize - stackChange.NewSize;
				if (isEating.ContainsKey(Game1.player.UniqueMultiplayerID) && isEating[Game1.player.UniqueMultiplayerID] == true && stackChange.Item is SObject food) {
					Monitor.Log($"Eating {food.DisplayName}.");
					Dictionary<string, int> cropList = GetGiantCrops();
					// The preserve index might break with custom crops...
					string matchedBigCropId = "";
					if (cropList.ContainsKey(food.QualifiedItemId)) {
						matchedBigCropId = food.QualifiedItemId;
					} else if (cropList.ContainsKey($"(O){food.preservedParentSheetIndex}")) {
						matchedBigCropId = $"(O){food.preservedParentSheetIndex}";
					}
					if (matchedBigCropId != "") {
						Monitor.Log($"Ate {food.DisplayName} which matches a giant crop {matchedBigCropId}.");
						float modifier = Config.PercentIncrease * cropList[matchedBigCropId];

						int staminaToHeal = (int)Math.Ceiling(food.staminaRecoveredOnConsumption() * modifier);
						int healthToHeal = (int)Math.Ceiling(food.healthRecoveredOnConsumption() * modifier);

						Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + staminaToHeal);
						Game1.player.health = Math.Min(Game1.player.maxHealth, Game1.player.health + healthToHeal);

						wasBonus[Game1.player.UniqueMultiplayerID] = (food.staminaRecoveredOnConsumption() + staminaToHeal, food.healthRecoveredOnConsumption() + healthToHeal);
					}
				}
				else if (Game1.currentSpeaker != null && stackChange.Item is SObject gift) {
					// TODO check if gifted
					Dictionary<string, int> cropList = GetGiantCrops();
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
						float modifier = Config.PercentIncrease * cropList[matchedBigCropId];

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
							if (Config.TestMode)
								Game1.hudMessages.Add(new HUDMessage($"{Game1.currentSpeaker.Name} gained {friendshipAmount * modifier} bonus friendship points.", 10500f, true) { whatType = 2 });
						}
					}

				} else {
					SoldItem(e.Player, stackChange.Item, soldCount);
				}
			}
		}

		/// <summary>Handle selling an item.</summary>
		/// <param name="item">The item being sold.</param>
		/// <param name="count">The number of items being sold.</param>
		private void SoldItem(Farmer player, Item item, int count) {
			if (Game1.activeClickableMenu is ShopMenu shopMenu) {
				if (item is SObject validItem) {
					Dictionary<string, int> cropList = GetGiantCrops();
					// The preserve index might break with custom crops...
					string matchedBigCropId = "";
					if (cropList.ContainsKey(validItem.QualifiedItemId)) {
						matchedBigCropId = validItem.QualifiedItemId;
					} else if (cropList.ContainsKey($"(O){validItem.preservedParentSheetIndex}")) {
						matchedBigCropId = $"(O){validItem.preservedParentSheetIndex}";
					}
					if (matchedBigCropId != "") {
						Monitor.Log($"Sold {count} {item.DisplayName}(s) for {item.sellToStorePrice()}g ea. which matches a giant crop {matchedBigCropId}.");

						float modifier = Config.PercentIncrease * cropList[matchedBigCropId];
						int bonusMoney = (int)(validItem.Price * modifier);
						ShopMenu.chargePlayer(player, shopMenu.currency, -bonusMoney * count);

						Monitor.Log($"Bonus money: {bonusMoney} * {count} = {bonusMoney * count}g");
					}
				}
			}
		}

		/// <summary>Get giant crops (only).</summary>
		private static Dictionary<string, int> GetGiantCrops() {
			Dictionary<string, int> cropTypeCounts = new();
			Utility.ForEachLocation(location => {
				foreach (GiantCrop giantCrop in location.resourceClumps.OfType<GiantCrop>()) {
					GiantCropData? giantCropItem = giantCrop.GetData();
					if (giantCropItem != null) {
						// Note the key is the source items id (for a Giant melon, it is '(O)254')
						if (!cropTypeCounts.ContainsKey(giantCropItem.FromItemId)) {
							cropTypeCounts[giantCropItem.FromItemId] = 0;
						}
						cropTypeCounts[giantCropItem.FromItemId]++;
					}
				}
				return true;
			});
			return cropTypeCounts;
		}

		/// <inheritdoc cref="IGameLoopEvents.UpdateTicked"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e) {
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
							int number = Convert.ToInt32(match.Value);
							if (message.whatType == 4) {
								Monitor.Log($"Stamina: {number} -> {wasBonus[Game1.player.UniqueMultiplayerID].Value.Item1}");
								message.message = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3116", wasBonus[Game1.player.UniqueMultiplayerID].Value.Item1);
							} else if (message.whatType == 5) {
								Monitor.Log($"Health: {number} -> {wasBonus[Game1.player.UniqueMultiplayerID].Value.Item2}");
								message.message = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3118", wasBonus[Game1.player.UniqueMultiplayerID].Value.Item2);
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

		/// <summary>Get all NPCs which have relationship data.</summary>
		/// <remarks>Derived from the <see cref="SocialPage"/> constructor.</remarks>
		private static IEnumerable<NPC> GetSocialCharacters() {
			foreach (NPC npc in Utility.getAllCharacters()) {
				if (npc.CanSocialize || Game1.player.friendshipData.ContainsKey(npc.Name))
					yield return npc;
			}
		}
	}
}
