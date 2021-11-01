/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CustomCommunityCentre
**
*************************************************/

using CustomCommunityCentre;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommunityKitchen
{
    public static class GusDeliveryService
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IReflectionHelper Reflection => ModEntry.Instance.Helper.Reflection;

		// Saloon delivery service
		public const string ShopOwner = "Gus";
		public const string ShopLocation = "Saloon";
		public static readonly string ItemDeliveryModDataKey = $"{ModEntry.Instance.ModManifest.UniqueID}.ItemDelivery";

		public static Lazy<Texture2D> DeliveryTexture;
		public static Lazy<Chest> ItemDeliveryChest;
		public static int DeliveryStartTime;
		public static int DeliveryEndTime;

		public const int DeliveryTimeDelta = 1;
		public const int SaloonOpeningTime = 1200;
		public const int SaloonClosingTime = 2400;

		public static string DeliveryTextureAssetKey = CommunityKitchen.AssetManager.DeliverySpritesAssetKey;
		public static bool IsSaloonDeliverySurchargeActive => !Bundles.IsCommunityCentreCompleteEarly(Bundles.CC);
		public const int SaloonDeliverySurcharge = 50;

		// Mail data
		public static string MailSaloonDeliverySurchargeWaived => $"{ModEntry.Instance.ModManifest.UniqueID}.saloonDeliverySurchargeWaived";


		internal static void RegisterEvents()
		{
			Helper.Events.GameLoop.DayStarted += GusDeliveryService.GameLoop_DayStarted;
			Helper.Events.GameLoop.TimeChanged += GusDeliveryService.GameLoop_TimeChanged;
			Helper.Events.Input.ButtonPressed += GusDeliveryService.Input_ButtonPressed;
			Helper.Events.Display.MenuChanged += GusDeliveryService.Display_MenuChanged;
		}

		internal static void AddConsoleCommands(string cmd)
		{
			Helper.ConsoleCommands.Add(
				name: cmd + "delivery",
				documentation: $"Open a new Saloon delivery menu.",
				callback: (string s, string[] args) =>
				{
					GusDeliveryService.OpenDeliveryMenu();
				});
			Helper.ConsoleCommands.Add(
				name: cmd + "gus",
				documentation: $"Create a new {nameof(GusOnABike)} on the farm.",
				callback: (string s, string[] args) =>
				{
					GusOnABike.Create();
				});
		}

		internal static void SaveLoadedBehaviours()
		{
			// Reload lazy assets per save
			GusDeliveryService.ResetDeliveryTexture();
			GusDeliveryService.ResetDeliveryChest();
		}

		internal static void DayStartedBehaviours()
		{
			// . . .
		}

		private static void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
		{
			if (Bundles.IsCommunityCentreCompleteEarly(Bundles.CC))
			{
				// Add mail on Community Centre completion for Saloon delivery service surcharge fee waived
				if (!Game1.player.hasOrWillReceiveMail(GusDeliveryService.MailSaloonDeliverySurchargeWaived))
				{
					Game1.addMail(GusDeliveryService.MailSaloonDeliverySurchargeWaived);
				}
			}
		}

		private static void GameLoop_TimeChanged(object sender, TimeChangedEventArgs e)
		{
			if (GusDeliveryService.ItemDeliveryChest.IsValueCreated
				&& GusDeliveryService.ItemDeliveryChest.Value.items.Any()
				&& !(Game1.activeClickableMenu is CommunityKitchen.ShopMenuNoInventory)
				&& (e.NewTime < GusDeliveryService.SaloonOpeningTime
					|| e.NewTime > GusDeliveryService.SaloonClosingTime
					|| e.NewTime >= GusDeliveryService.DeliveryEndTime))
			{
				if (!GusOnABike.IsGusOnFarm())
				{
					if (Game1.currentLocation is Farm)
					{
						GusOnABike.Create();
					}
					else
					{
						GusOnABike.Honk(isOnFarm: false);
						GusDeliveryService.AddDeliveryChests();
					}
				}
			}
		}

		private static void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			// In-game interactions
			if (!Game1.game1.IsActive || Game1.currentLocation == null || !Context.IsWorldReady)
				return;

			// . . .

			// World interactions
			if (!Context.CanPlayerMove)
				return;

			if (e.Button.IsActionButton())
			{
				// Object actions
				Game1.currentLocation.Objects.TryGetValue(e.Cursor.GrabTile, out StardewValley.Object o);
				if (o != null)
				{
					// Open Saloon delivery giftbox chests
					if (o is Chest c)
					{
						GusDeliveryService.TryOpenDeliveryChest(location: Game1.currentLocation, chest: c, button: e.Button);
					}
				}
			}
		}

		private static void Display_MenuChanged(object sender, MenuChangedEventArgs e)
		{
			if (e.OldMenu is TitleMenu || e.NewMenu is TitleMenu || Game1.currentLocation == null || Game1.player == null)
				return;

			// Handle Saloon Delivery telephone dialogue menu
			const string questionKey = "telephone";
			if (e.NewMenu is DialogueBox dialogueBox
				&& Game1.currentLocation.lastQuestionKey == questionKey
				&& dialogueBox.characterDialogue?.speaker?.Name == GusDeliveryService.ShopOwner
				&& (Kitchen.HasOrWillReceiveKitchenCompletedMail() || Kitchen.IsKitchenComplete(Bundles.CC)))
			{
				if ((e.OldMenu == null || !(e.OldMenu is CommunityKitchen.ShopMenuNoInventory))
					&& (GusOnABike.IsGusOnFarm() || GusOnABike.WhereGus() != GusDeliveryService.ShopLocation))
				{
					// Replace phonecall with dummy dialogue if Gus is already delivering food
					Game1.activeClickableMenu = new DialogueBox(ModEntry.i18n.Get("dialogue.phone.invalid"));
				}
				else
				{
					// Add delivery menu option to phonecall
					GusDeliveryService.TryOpenDeliveryMenu();
				}
				return;
			}
		}

		internal static void ResetDeliveryTexture()
		{
			GusDeliveryService.DeliveryTexture = new Lazy<Texture2D>(delegate
			{
				Texture2D texture = Game1.content.Load<Texture2D>(GusDeliveryService.DeliveryTextureAssetKey);
				return texture;
			});
		}

		internal static void ResetDeliveryChest()
		{
			GusDeliveryService.ItemDeliveryChest = new Lazy<Chest>(delegate
			{
				Chest chest = new (playerChest: true, tileLocation: new Vector2(CustomCommunityCentre.ModEntry.DummyId));
				return chest;
			});
		}

		internal static void TryOpenDeliveryMenu()
		{
			bool isNotReady = GameLocation.AreStoresClosedForFestival()
				|| Game1.timeOfDay < GusDeliveryService.SaloonOpeningTime || Game1.timeOfDay >= GusDeliveryService.SaloonClosingTime
				|| (Game1.getCharacterFromName(GusDeliveryService.ShopOwner)?.dayScheduleName.Value == "fall_4" && Game1.timeOfDay >= 1700);
			if (isNotReady)
				return;

			// Add question responses after dialogue
			List<Response> responses = new()
			{
				new Response(
					responseKey: $"{ModEntry.Instance.ModManifest.UniqueID}_Telephone_delivery",
					responseText: ModEntry.i18n.Get($"response.phone.delivery{(GusDeliveryService.IsSaloonDeliverySurchargeActive ? "_surcharge" : "")}")),
				new Response(
					responseKey: "HangUp",
					responseText: Game1.content.LoadString(@"Strings/Characters:Phone_HangUp"))
			};
			Game1.afterFadeFunction phoneDialogue = delegate
			{
				Game1.currentLocation.createQuestionDialogue(
					question: Game1.content.LoadString(@"Strings/Characters:Phone_SelectOption"),
					answerChoices: responses.ToArray(),
					afterDialogueBehavior: delegate (Farmer who, string questionAndAnswer)
					{
						static string getAnswer(string questionAndAnswer)
						{
							return questionAndAnswer.Split(new char[] { '_' }, 2).Last();
						}
						string[] answers = responses
							.Take(responses.Count - 1)
							.Select(r => getAnswer(r.responseKey))
							.ToArray();
						string answer = getAnswer(questionAndAnswer);
						// is 'answer' even a word? what the
						if (answer == answers[0])
						{
							// Saloon Delivery

							GusDeliveryService.OpenDeliveryMenu();
						}
						/*
						else if (answer == answers[1])
						{
							// Daily Dish

							// Load new phone dialogue
							const string messageRoot = @"Strings/Characters:Phone_Gus_Open";
							string message = Game1.dishOfTheDay != null
								? Game1.content.LoadString(messageRoot + ((Game1.random.NextDouble() < 0.01) ? "_Rare" : ""),
									Game1.dishOfTheDay.DisplayName)
								: Game1.content.LoadString(messageRoot + "_NoDishOfTheDay");
							Game1.drawDialogue(dialogueBox.characterDialogue.speaker, message);
						}
						*/
					});
			};
			Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(Game1.afterDialogues, phoneDialogue);
		}

		internal static void TryOpenDeliveryChest(GameLocation location, Chest chest, SButton button)
		{
			if (chest.modData.TryGetValue(GusDeliveryService.ItemDeliveryModDataKey, out string s) && long.TryParse(s, out long id))
			{
				if (id != Game1.player.UniqueMultiplayerID)
				{
					Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Objects:ParsnipSeedPackage_SomeoneElse"));
				}
				else
				{
					Game1.currentLocation.playSound("Ship");
					foreach (Item item in chest.items)
					{
						if (item != null)
						{
							Game1.createItemDebris(item, chest.TileLocation * Game1.tileSize, -1, location);
						}
					}
					chest.items.Clear();
					chest.clearNulls();

					TemporaryAnimatedSprite sprite = new (
						textureName: @"LooseSprites/Giftbox",
						sourceRect: new Rectangle(
							0,
							chest.giftboxIndex.Value * StardewValley.Object.spriteSheetTileSize * 2,
							StardewValley.Object.spriteSheetTileSize,
							StardewValley.Object.spriteSheetTileSize * 2),
						animationInterval: 80f,
						animationLength: 11,
						numberOfLoops: 1,
						position: (chest.TileLocation * Game1.tileSize) - new Vector2(0f, Game1.tileSize - 12f),
						flicker: false,
						flipped: false,
						layerDepth: chest.TileLocation.Y / 10000f,
						alphaFade: 0f,
						color: Color.White,
						scale: Game1.pixelZoom,
						scaleChange: 0f,
						rotation: 0f,
						rotationChange: 0f)
					{
						destroyable = false,
						holdLastFrame = true
					};
					if (location.netObjects.ContainsKey(chest.TileLocation) && location.netObjects[chest.TileLocation] == chest)
					{
						CustomCommunityCentre.ModEntry.Instance.GetMultiplayer().broadcastSprites(location, sprite);
						location.removeObject(chest.TileLocation, showDestroyedObject: false);
					}
					else
					{
						location.temporarySprites.Add(sprite);
					}
				}

				Helper.Input.Suppress(button);
			}
		}

		internal static void OpenDeliveryMenu()
		{
			// Open a limited-stock saloon shop for the player
			Dictionary<ISalable, int[]> itemPriceAndStock = Utility.getSaloonStock()
				.Where(pair => pair.Key is Item i && (i is not StardewValley.Object o || !o.IsRecipe))
				.ToDictionary(pair => pair.Key, pair => pair.Value);

			CommunityKitchen.ShopMenuNoInventory shopMenu = new (
				itemPriceAndStock: itemPriceAndStock,
				currency: 0,
				who: GusDeliveryService.ShopOwner,
				on_purchase: delegate (ISalable item, Farmer farmer, int amount)
				{
					Game1.player.team.synchronizedShopStock.OnItemPurchased(
						shop: SynchronizedShopStock.SynchedShop.Saloon,
						item: item,
						amount: amount);

					// Vanish the item and add it to the dummy delivery chest
					((CommunityKitchen.ShopMenuNoInventory)Game1.activeClickableMenu).heldItem = null;
					Item i = ((Item)item).getOne();
					i.Stack = amount;
					((CommunityKitchen.ShopMenuNoInventory)Game1.activeClickableMenu).AddToOrderDisplay(item: i);
					GusDeliveryService.ItemDeliveryChest.Value.addItem(i);

					return false;
				});
			shopMenu.exitFunction = delegate
			{
				if (shopMenu.DeliveryItemsAndCounts.Any())
				{
					if (GusDeliveryService.IsSaloonDeliverySurchargeActive)
					{
						Game1.player.Money -= GusDeliveryService.SaloonDeliverySurcharge;
					}

					GusDeliveryService.DeliveryStartTime = Game1.timeOfDay;
					int deliveryEndTime = Utility.ModifyTime(
						timestamp: GusDeliveryService.DeliveryStartTime,
						minutes_to_add: GusDeliveryService.DeliveryTimeDelta * 10);
					GusDeliveryService.DeliveryEndTime = deliveryEndTime;

					Item item = shopMenu.DeliveryItemsAndCounts.Keys.ToArray()[Game1.random.Next(0, shopMenu.DeliveryItemsAndCounts.Count)];

					const string key = "dialogue.phone.delivery.";
					int count = ModEntry.i18n.GetTranslations()
						.Count(t => t.Key.ToString().StartsWith(key));
					int whichMessage = Game1.random.Next(0, count);
					string message = ModEntry.i18n.Get(
						key: $"{key}{whichMessage}",
						tokens: new { ItemName = item.DisplayName });
					Game1.drawDialogue(speaker: shopMenu.portraitPerson, dialogue: message);
				}
			};
			Game1.activeClickableMenu = shopMenu;
		}

		internal static void AddDeliveryChests()
		{
			Farm farm = Game1.getFarm();

			Vector2 mailboxPosition = Utility.PointToVector2(Game1.player.getMailboxPosition());
			Vector2 chestPosition = CustomCommunityCentre.ModEntry.FindFirstPlaceableTileAroundPosition(
				location: farm,
				tilePosition: mailboxPosition,
				o: new Chest(),
				maxIterations: 100);
			Chest deliveryChest = new (coins: 0, items: GusDeliveryService.ItemDeliveryChest.Value.items.ToList(), location: chestPosition, giftbox: true);
			deliveryChest.modData[GusDeliveryService.ItemDeliveryModDataKey] = Game1.player.UniqueMultiplayerID.ToString();
			farm.Objects.Add(chestPosition, deliveryChest);

			Bundles.BroadcastPuffSprites(
				multiplayer: null,
				location: farm,
				tilePosition: chestPosition);

			GusDeliveryService.ResetDelivery();
		}

		internal static void ResetDelivery()
		{
			GusDeliveryService.ItemDeliveryChest.Value.items.Clear();
			GusDeliveryService.DeliveryStartTime = GusDeliveryService.DeliveryEndTime = -1;
		}
	}
}
