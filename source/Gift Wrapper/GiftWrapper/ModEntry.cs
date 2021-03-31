/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/GiftWrapper
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using SpaceCore.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using xTile.Dimensions;

namespace GiftWrapper
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance;
		internal static Config Config;
		internal ITranslationHelper i18n => Helper.Translation;
		internal static IJsonAssetsAPI JsonAssets;

		public const string AssetPrefix = "blueberry.gw.";
		public const string GiftWrapName = "giftwrap";
		public const string WrappedGiftName = "wrappedgift";

		internal static readonly string GameContentTexturePath = @$"{AssetPrefix}Assets/Sprites";
		internal static readonly string LocalTexturePath = Path.Combine("assets", "sprites");
		internal static readonly string ContentPackPath = Path.Combine("assets", "ContentPack");
		internal const int GiftWrapFriendshipBoost = 25;
		internal const int WrappedGiftToolsSheetIndex = 3; // Wrapped gift replaces (hopefully) unused tool Lantern/Lamp/Sconce/Candle/Light
		internal enum GiftType
		{
			BedFurniture,
			Furniture,
			BigCraftable,
			MeleeWeapon,
			Hat,
			Boots,
			Clothing,
			Ring,
			Object
		}

		public override void Entry(IModHelper helper)
		{
			Instance = this;
			Config = Helper.ReadConfig<Config>();

			Helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
			Helper.Events.Input.ButtonPressed += this.Input_ButtonPressed;
			Helper.Events.Display.MenuChanged += this.Display_MenuChanged;
			SpaceEvents.BeforeGiftGiven += this.SpaceEvents_BeforeGiftGiven;

			AssetManager assetManager = new AssetManager();
			Helper.Content.AssetLoaders.Add(assetManager);
			Helper.Content.AssetEditors.Add(assetManager);
		}

		private void LoadApis()
		{
			// Add Json Assets items
			JsonAssets = Helper.ModRegistry.GetApi<IJsonAssetsAPI>("spacechase0.JsonAssets");
			if (JsonAssets == null)
			{
				Monitor.Log("Can't access the Json Assets API. Is the mod installed correctly?", LogLevel.Error);
				return;
			}
			JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, ContentPackPath));

			// Add GMCM config page
			this.RegisterGenericModConfigMenuPage();
		}

		private void RegisterGenericModConfigMenuPage()
		{
			IGenericModConfigMenuAPI api = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
			if (api == null)
				return;

			api.RegisterModConfig(ModManifest,
				revertToDefault: () => Config = new Config(),
				saveToFile: () => Helper.WriteConfig(Config));
			api.RegisterSimpleOption(ModManifest,
				optionName: i18n.Get("config.availableallyear.name"),
				optionDesc: i18n.Get("config.availableallyear.description"),
				optionGet: () => Config.AvailableAllYear,
				optionSet: (bool value) => Config.AvailableAllYear = value);
			api.RegisterSimpleOption(ModManifest,
				optionName: i18n.Get("config.invertmousebuttons.name"),
				optionDesc: i18n.Get("config.invertmousebuttons.description"),
				optionGet: () => Config.InteractUsingToolButton,
				optionSet: (bool value) => Config.InteractUsingToolButton = value);
		}

		private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
		{
			this.LoadApis();
		}

		private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
		{
			bool isWinterStarPeriod = Game1.currentSeason == "winter" && Game1.dayOfMonth >= 18;
			if (e.NewMenu != null && e.NewMenu is ShopMenu shop && shop.portraitPerson != null && shop.portraitPerson.Name == "Pierre"
				&& (Config.AvailableAllYear || isWinterStarPeriod))
			{
				// Gift wrap is purchaseable from Pierre throughout the Secret Friend gifting event
				int id = JsonAssets.GetObjectId(AssetPrefix + GiftWrapName);
				StardewValley.Object o = new StardewValley.Object(id, initialStack: 1);
				float price = o.Price * Game1.MasterPlayer.difficultyModifier;
				if (Game1.dayOfMonth > 25)
				{
					// Sell gift wrap at clearance prices between the end of the event and the end of the year
					// For an 80g base price, clearance price is 30g
					price *= 0.4f;
				}
				shop.itemPriceAndStock.Add(o, new int[] { (int)(price * 0.2) * 5, int.MaxValue });
				if (isWinterStarPeriod)
				{
					// Gift wrap appears at the top of the shop stock over the Winter Star festival
					shop.forSale.Insert(0, o);
				}
				else
				{
					// If using the available-all-year config option, place gift wrap further down the list at other times of year
					ISalable item = shop.forSale.FirstOrDefault(i => i.Name == "Bouquet") ?? shop.forSale.Last(i => i.Name.EndsWith("Sapling"));
					if (item != null)
					{
						int index = shop.forSale.IndexOf(item) + 1;
						shop.forSale.Insert(index, o);
					}
					else
					{
						shop.forSale.Add(o);
					}
				}
			}
		}

		private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (ModEntry.PlayerAgencyLostCheck())
				return;

			// Menu interactions
			if (Game1.activeClickableMenu is GameMenu gameMenu && gameMenu.GetCurrentPage() is InventoryPage inventoryPage && e.Button.IsActionButton())
			{
				Item hoverItem = inventoryPage.inventory.hover(Game1.getOldMouseX(), Game1.getOldMouseY(), Game1.player.CursorSlotItem);
				if (Game1.player.CursorSlotItem != null
					&& (Game1.player.CursorSlotItem.Name == AssetPrefix + WrappedGiftName
						|| Game1.player.CursorSlotItem.Name == i18n.Get("item." + WrappedGiftName + ".name"))
					&& Game1.player.CursorSlotItem.Category == -22
					&& hoverItem != null && hoverItem is FishingRod)
				{
					// Prevent tackle-method wrapped gifts from being attached to fishing rods with a tackle attachment slot
					Helper.Input.Suppress(e.Button);
					Game1.playSound("cancel");
					return;
				}
			}


			Vector2 screenPos = new Vector2(e.Cursor.ScreenPixels.X, e.Cursor.ScreenPixels.Y);
			Location tilePos = new Location((int)e.Cursor.ScreenPixels.X, (int)e.Cursor.ScreenPixels.Y);
			if (Game1.activeClickableMenu != null // No active menus or onscreen menus
				|| Game1.onScreenMenus.Any(menu => menu.isWithinBounds((int)screenPos.X, (int)screenPos.Y))
				|| Game1.currentLocation.checkAction(tilePos, Game1.viewport, Game1.player))
			{
				return;
			}

			// World interactions
			if (Game1.currentLocation.Objects.ContainsKey(e.Cursor.GrabTile)
				&& Game1.currentLocation.Objects[e.Cursor.GrabTile] is StardewValley.Object o)
			{
				if (o.Name == AssetPrefix + GiftWrapName)
				{
					// Open the gift wrap menu from placed gift wrap when left-clicking
					if (this.IsInteractButton(e.Button))
					{
						Game1.activeClickableMenu = new GiftWrapMenu(e.Cursor.GrabTile);
					}
					else if (this.IsPlacementButton(e.Button))
					{
						if (Game1.player.couldInventoryAcceptThisItem(o))
						{
							Game1.player.addItemToInventory(o);
							Game1.playSound("pickUpItem");
							Game1.currentLocation.Objects.Remove(e.Cursor.GrabTile);
						}
						else
						{
							Game1.playSound("cancel");
						}
					}
				}
				else if (o.Name == AssetPrefix + WrappedGiftName || o.Name == i18n.Get("item." + WrappedGiftName + ".name"))
				{
					// Unwrap the placed gift and pop out the actual gift when left-clicking
					if (this.IsInteractButton(e.Button))
					{
						// Add actual gift to inventory and remove wrapped gift object
						Item actualGift = this.UnpackItem(modData: o.modData, recipientName: Game1.player.Name);
						Game1.currentLocation.playSound("getNewSpecialItem");
						if (actualGift == null)
						{
							// this shouldn't happen
							Game1.playSound("sheep");
						}

						if (actualGift == null || Game1.createItemDebris(actualGift, Game1.player.Position, -1) != null)
						{
							Game1.currentLocation.Objects.Remove(e.Cursor.GrabTile);
						}
						else
						{
							Game1.playSound("cancel");
							Monitor.Log($"Couldn't open the {o.DisplayName} at {e.Cursor.GrabTile.ToString()} (gift is a {actualGift.DisplayName})", LogLevel.Debug);
						}
					}
					else if (this.IsPlacementButton(e.Button))
					{
						Item wrappedGift = this.GetWrappedGift(o.modData);
						if (wrappedGift == null)
						{
							// this shouldn't happen
							Game1.playSound("sheep");
						}

						if (wrappedGift == null || Game1.player.addItemToInventory(wrappedGift) == null)
						{
							Game1.currentLocation.Objects.Remove(e.Cursor.GrabTile);
						}
						else
						{
							Game1.playSound("cancel");
						}
					}
				}
			}
			else
			{
				// Place held gift wrap and wrapped gifts on the ground when left-clicking
				bool canPlaceHere = Game1.currentLocation.isTileLocationTotallyClearAndPlaceableIgnoreFloors(e.Cursor.GrabTile)
					&& !Game1.currentLocation.Objects.ContainsKey(e.Cursor.GrabTile);
				if (this.IsPlacementButton(e.Button) && canPlaceHere)
				{
					const string placementSound = "throwDownITem"; // not a typo
					if (Game1.player.ActiveObject != null && Game1.player.ActiveObject.Name == AssetPrefix + GiftWrapName)
					{
						Helper.Input.Suppress(e.Button);
						Game1.playSound(placementSound); 
						Game1.currentLocation.Objects[e.Cursor.GrabTile] = Game1.player.ActiveObject.getOne() as StardewValley.Object;
						--Game1.player.ActiveObject.Stack;
						if (Game1.player.ActiveObject.Stack < 1)
						{
							Game1.player.removeItemFromInventory(Game1.player.ActiveObject);
						}
					}
					else if (Game1.player.CurrentItem != null
						&& (Game1.player.CurrentItem.Name == AssetPrefix + WrappedGiftName
							|| Game1.player.CurrentItem.Name == i18n.Get("item." + WrappedGiftName + ".name")))
					{
						Helper.Input.Suppress(e.Button);
						this.PlaceWrappedGift(
							wrappedGift: Game1.player.CurrentItem,
							location: Game1.currentLocation,
							tilePosition: e.Cursor.GrabTile,
							sound: placementSound);
						return;
					}
				}
			}
		}
		
		private void SpaceEvents_BeforeGiftGiven(object sender, EventArgsBeforeReceiveObject e)
		{
			// Ignore NPC gifts that aren't going to be accepted
			if (!e.Npc.canReceiveThisItemAsGift(e.Gift)
				|| !Game1.player.friendshipData.ContainsKey(e.Npc.Name)
				|| Game1.player.friendshipData[e.Npc.Name].GiftsThisWeek > 1
				|| Game1.player.friendshipData[e.Npc.Name].GiftsToday > 0)
			{
				return;
			}

			if (e.Gift.Name == AssetPrefix + WrappedGiftName)
			{
				// Cancel the wrapped gift NPC gift
				e.Cancel = true;

				Item actualGift = this.UnpackItem(modData: e.Gift.modData, recipientName: null);
				if (!(actualGift is StardewValley.Object o) || o.bigCraftable.Value || !o.canBeGivenAsGift() || actualGift.Stack > 1)
				{
					// Ignore actual gifts that are invalid NPC gifts, eg. Tools
					// Ignore actual gifts wrapped as part of large stacks, as items are typically only able to be given as gifts one-at-a-time
					Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1803"));
					Game1.playSound("cancel");
					return;
				}

				// Redeliver the NPC gift as the actual gift
				e.Npc.receiveGift(o: actualGift as StardewValley.Object, giver: Game1.player,
					updateGiftLimitInfo: true, friendshipChangeMultiplier: 1, showResponse: true);

				// Add bonus friendship for having given them a wrapped gift
				Game1.player.changeFriendship(amount: GiftWrapFriendshipBoost, n: e.Npc);

				// Remove wrapped gift from inventory
				Game1.player.removeItemFromInventory(e.Gift);
			}
		}

		private bool IsInteractButton(SButton button)
		{
			return (button.IsActionButton() && !Config.InteractUsingToolButton)
				|| (button.IsUseToolButton() && Config.InteractUsingToolButton);
		}

		private bool IsPlacementButton(SButton button)
		{
			return (button.IsUseToolButton() && !Config.InteractUsingToolButton)
				|| (button.IsActionButton() && Config.InteractUsingToolButton);
		}

		public Item GetWrappedGift(ModDataDictionary modData)
		{
			// Object-based solution for wrapped gifts:
			Item wrappedGift = new StardewValley.Object(parentSheetIndex: JsonAssets.GetObjectId(AssetPrefix + WrappedGiftName), initialStack: 1)
			{
				// Tackle category: Cannot be stacked higher than 1, which solves the issue of modData folding.
				// The unfortunate side-effect is that they can, however, be attached to rods. We'll sort this out in ButtonPressed.
				Category = -22
			};
			if (modData != null)
			{
				wrappedGift.modData = modData;
			}
			return wrappedGift;
		}

		public bool PlaceWrappedGift(Item wrappedGift, GameLocation location, Vector2 tilePosition, string sound)
		{
			if (!string.IsNullOrEmpty(sound))
				Game1.playSound(sound);

			if (wrappedGift == null || location == null
				|| (location.Objects.ContainsKey(tilePosition) && location.Objects[tilePosition] != null))
			{
				return false;
			}

			StardewValley.Object placedGift = new StardewValley.Object(parentSheetIndex: JsonAssets.GetObjectId(AssetPrefix + WrappedGiftName), initialStack: 1);
			if (wrappedGift.modData != null)
			{
				placedGift.modData = wrappedGift.modData;
			}
			location.Objects[tilePosition] = placedGift;

			if (Game1.player.Items.Contains(wrappedGift))
			{
				Game1.player.removeItemFromInventory(wrappedGift);
			}

			return true;
		}

		public void PackItem(ref Item wrappedGift, Item giftToWrap, Vector2 placedGiftWrapPosition, bool showMessage)
		{
			if (Game1.player.couldInventoryAcceptThisItem(wrappedGift))
			{
				bool isDefined = Enum.IsDefined(typeof(GiftType), giftToWrap.GetType().Name);
				bool isBigCraftable = giftToWrap is StardewValley.Object bc && bc.bigCraftable.Value;

				// Define all the data to be serialised into the wrapped gift's modData
				long giftSender = Game1.player.UniqueMultiplayerID;
				int giftId = giftToWrap is Hat hat
						? hat.which.Value
						: giftToWrap is MeleeWeapon weapon
							? Game1.content.Load<Dictionary<int, string>>(@"Data/weapons").First(pair => pair.Value.Split('/')[0] == weapon.Name).Key
							: giftToWrap is Boots boots
								? boots.indexInTileSheet.Value
								: giftToWrap.ParentSheetIndex;
				int giftType = isBigCraftable
						? (int)GiftType.BigCraftable
						: isDefined
							? (int)Enum.Parse(typeof(GiftType), giftToWrap.GetType().Name)
							: -1;
				int giftStack = giftToWrap is StardewValley.Object
						? giftToWrap.Stack
						: -1;
				int giftQuality = giftToWrap is StardewValley.Object o
						? o.Quality
						: giftToWrap is Boots boots1
							? boots1.appliedBootSheetIndex.Value
							: -1;
				string giftColour = giftToWrap is Clothing c
						? string.Join("/", new [] { c.clothesColor.Value.R, c.clothesColor.Value.G, c.clothesColor.Value.B, c.clothesColor.Value.A })
						: giftToWrap is Boots boots2
							? boots2.indexInColorSheet.ToString()
							: "-1";

				// Convert the gift item's modData into a serialisable form to be added to the wrapped gift's modData
				Dictionary<string, string> giftDataRaw = new Dictionary<string, string>();
				foreach (var pair in giftToWrap.modData.FieldDict)
					giftDataRaw.Add(pair.Key, pair.Value.Value);
				string giftDataSerialised = JsonConvert.SerializeObject(giftDataRaw);

				if (giftType == -1)
				{
					// Avoid adding items with undefined behaviour
					Game1.showRedMessage(i18n.Get("error.wrapping", new { ItemName = wrappedGift.DisplayName }));
				}
				else if (Game1.currentLocation.Objects.Remove(placedGiftWrapPosition))
				{
					// Add all fields into wrapped gift's modData
					wrappedGift.modData[AssetPrefix + "giftsender"] = giftSender.ToString();
					wrappedGift.modData[AssetPrefix + "giftid"] = giftId.ToString();
					wrappedGift.modData[AssetPrefix + "gifttype"] = giftType.ToString();
					wrappedGift.modData[AssetPrefix + "giftstack"] = giftStack.ToString();
					wrappedGift.modData[AssetPrefix + "giftquality"] = giftQuality.ToString();
					wrappedGift.modData[AssetPrefix + "giftcolour"] = giftColour;
					wrappedGift.modData[AssetPrefix + "giftdata"] = giftDataSerialised;

					if (showMessage)
					{
						string message = i18n.Get("message.giftwrapped", new
							{
								WrappedGiftName = wrappedGift.DisplayName,
								ItemName = giftToWrap.DisplayName
							});
						Game1.addHUDMessage(new HUDMessage("", 1, add: true, Game1.textColor, wrappedGift));
					}
				}
			}
		}

		public Item UnpackItem(ModDataDictionary modData, string recipientName)
		{
			string[] fields = new[] { "giftsender", "giftid", "gifttype", "giftstack", "giftquality", "giftcolour", "giftdata" };
			if (fields.Any(field => !modData.ContainsKey(AssetPrefix + field)))
			{
				return null;
			}

			// Parse the wrapped gift's serialised modData fields to use in rebuilding its gift item
			long giftSender = long.Parse(modData[AssetPrefix + fields[0]]);
			int giftId = int.Parse(modData[AssetPrefix + fields[1]]);
			int giftType = int.Parse(modData[AssetPrefix + fields[2]]);
			int giftStack = int.Parse(modData[AssetPrefix + fields[3]]);
			int giftQuality = int.Parse(modData[AssetPrefix + fields[4]]);
			string giftColour = modData[AssetPrefix + fields[5]];
			string giftData = modData[AssetPrefix + fields[6]];
			Item actualGift = null;
			switch (giftType)
			{
				case (int)GiftType.BedFurniture:
					actualGift = new BedFurniture(which: giftId, tile: Vector2.Zero);
					break;
				case (int)GiftType.Furniture:
					actualGift = new Furniture(which: giftId, tile: Vector2.Zero);
					break;
				case (int)GiftType.BigCraftable:
					actualGift = new StardewValley.Object(tileLocation: Vector2.Zero, parentSheetIndex: giftId, isRecipe: false);
					break;
				case (int)GiftType.MeleeWeapon:
					actualGift = new MeleeWeapon(spriteIndex: giftId);
					break;
				case (int)GiftType.Hat:
					actualGift = new Hat(which: giftId);
					break;
				case (int)GiftType.Boots:
					actualGift = new Boots(which: giftId); // todo: test boots colour
					((Boots)actualGift).appliedBootSheetIndex.Set(giftQuality);
					((Boots)actualGift).indexInColorSheet.Set(int.Parse(giftColour));
					break;
				case (int)GiftType.Clothing:
					int[] colourSplit = giftColour.Split('/').ToList().ConvertAll(int.Parse).ToArray();
					Color colour = new Color(r: colourSplit[0], g: colourSplit[1], b: colourSplit[2], a: colourSplit[3]);
					actualGift = new Clothing(item_index: giftId);
					((Clothing)actualGift).clothesColor.Set(colour);
					break;
				case (int)GiftType.Ring:
					actualGift = new Ring(which: giftId);
					break;
				case (int)GiftType.Object:
					actualGift = new StardewValley.Object(parentSheetIndex: giftId, initialStack: giftStack) { Quality = giftQuality };
					break;
			}

			if (actualGift == null)
			{
				return null;
			}

			Dictionary<string, string> giftDataDeserialised = ((Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(giftData)).ToObject<Dictionary<string, string>>();
			if (giftDataDeserialised != null)
			{
				// Apply serialised mod data back to the gifted item
				actualGift.modData.Set(giftDataDeserialised);
			}

			if (recipientName != null && Game1.player.UniqueMultiplayerID != giftSender)
			{
				// Show a message to all players to celebrate this wonderful event
				Multiplayer multiplayer = Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
				multiplayer.globalChatInfoMessage(AssetPrefix + "message.giftopened",
					recipientName, // Recipient's name
					Game1.getFarmer(giftSender).Name, // Sender's name
					giftStack > 1 ? giftStack.ToString() : "a", // Gift quantity
					actualGift.DisplayName); // Gift name
			}

			return actualGift;
		}

		/// <summary>
		/// Checks whether the player has agency during gameplay, cutscenes, and input sessions.
		/// </summary>
		public static bool PlayerAgencyLostCheck()
		{
			// HOUSE RULES
			return Game1.game1 == null || Game1.currentLocation == null || Game1.player == null // No unplayable games
					|| !Game1.game1.IsActive // No alt-tabbed game state
					|| (Game1.eventUp && Game1.currentLocation.currentEvent != null && !Game1.currentLocation.currentEvent.playerControlSequence) // No event cutscenes
					|| Game1.nameSelectUp || Game1.IsChatting || Game1.dialogueTyping || Game1.dialogueUp
					|| (Game1.keyboardDispatcher != null && Game1.keyboardDispatcher.Subscriber != null) // No text inputs
					|| Game1.player.UsingTool || Game1.pickingTool || Game1.numberOfSelectedItems != -1 // No tools in use
					|| Game1.fadeToBlack; // None of that
		}
	}
}
