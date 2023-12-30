/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/GiftWrapper
**
*************************************************/

using GiftWrapper.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SpaceCore.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Object = StardewValley.Object;
using HarmonyLib; // el diavolo nuevo

namespace GiftWrapper
{
	public sealed class ModEntry : Mod
	{
		internal static ModEntry Instance;
		internal static Config Config;
		internal static ITranslationHelper I18n => ModEntry.Instance.Helper.Translation;

		/// <summary>
		/// Mod definitions as loaded from data file.
		/// Persists between sessions.
		/// </summary>
		internal static Definitions Definitions { get; private set; }
		/// <summary>
		/// Shop data definitions as loaded from data file for host player,
		/// or from network message if client player.
		/// Does not persist between sessions.
		/// </summary>
		internal static Shop[] Shops { get; private set; }
		/// <summary>
		/// GiftItem sprite definitions as loaded from data file.
		/// Persists between sessions.
		/// Not loaded until individually referenced.
		/// </summary>
		internal static Dictionary<string, Lazy<Texture2D>> GiftSprites { get; private set; }
		/// <summary>
		/// WrapItem sprite instance.
		/// Persists between sessions.
		/// Not loaded until referenced.
		/// </summary>
		internal static Lazy<Texture2D> WrapSprite { get; private set; } = new(ModEntry.LoadWrapTexture);
		/// <summary>
		/// Whether to use IfAlwaysAvailable shop data entries instead of usual.
		/// Does not persist between sessions.
		/// </summary>
		internal static bool IsAlwaysAvailable { get; private set; }

		/// <summary>
		/// Prefix used for unique ID of most/all asset keys.
		/// </summary>
		public const string AssetPrefix = "blueberry.GiftWrapper.";
		/// <summary>
		/// Prefix used for unique ID of ModData dictionary keys.
		/// </summary>
		public const string ModDataPrefix = "blueberry.GiftWrapper.";
		/// <summary>
		/// Prefix used for unique ID of all custom items added by this mod.
		/// </summary>
		public const string ItemPrefix = "blueberry.gw.";
		/// <summary>
		/// Unique ID after prefix of WrapItem.
		/// </summary>
		public const string WrapItemName = "wrap";
		/// <summary>
		/// Unique ID after prefix of GiftItem.
		/// </summary>
		public const string GiftItemName = "gift";

		internal static readonly string GameContentDataPath = Path.Combine("Mods", ModEntry.AssetPrefix + "Assets", "Data");
		internal static readonly string GameContentWrapTexturePath = Path.Combine("Mods", ModEntry.AssetPrefix + "Assets", "Wrap");
		internal static readonly string GameContentGiftTexturePath = Path.Combine("Mods", ModEntry.AssetPrefix + "Assets", "Gifts");
		internal static readonly string GameContentMenuTexturePath = Path.Combine("Mods", ModEntry.AssetPrefix + "Assets", "Menu");
		internal static readonly string GameContentCardTexturePath = Path.Combine("Mods", ModEntry.AssetPrefix + "Assets", "Card");

		internal static readonly string LocalDataPath = Path.Combine("assets", "data");
		internal static readonly string LocalWrapTexturePath = Path.Combine("assets", "wrap");
		internal static readonly string LocalGiftTexturePath = Path.Combine("assets", "gifts");
		internal static readonly string LocalMenuTexturePath = Path.Combine("assets", "menu-{0}");
		internal static readonly string LocalCardTexturePath = Path.Combine("assets", "card");
		internal static readonly string LocalBannerTexturePath = Path.Combine("assets", "banner");

		internal static string LocalAudioPath => Path.Combine(ModEntry.Instance.Helper.DirectoryPath, "assets", "audio");


		/// <summary>
		/// Message type values assigned to network messages in multiplayer.
		/// </summary>
		public enum MessageType
		{
			ReloadShops
		}

		/// <summary>
		/// Class used for network messages in multiplayer.
		/// </summary>
		public class Message {}

		public override void Entry(IModHelper helper)
		{
			ModEntry.Instance = this;
			ModEntry.Config = this.Helper.ReadConfig<Config>();

			if (!Enum.IsDefined(typeof(Config.Themes), ModEntry.Config.Theme))
			{
				// Pick random theme if theme not picked or defined
				List<Config.Themes> themes = Enum.GetValues(typeof(Config.Themes)).Cast<Config.Themes>().ToList();
				ModEntry.Config.Theme = themes[Game1.random.Next(themes.Count)];
			}

			this.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
		}

		private bool TryLoadApis()
		{
			// Add SpaceCore serialisation
			try
			{
				ISpaceCoreAPI spacecoreApi = this.Helper.ModRegistry
					.GetApi<ISpaceCoreAPI>
					("spacechase0.SpaceCore");
				spacecoreApi.RegisterSerializerType(typeof(GiftItem));
				spacecoreApi.RegisterSerializerType(typeof(WrapItem));
			}
			catch (Exception e)
			{
				this.Monitor.Log($"Failed to register objects with SpaceCore.{Environment.NewLine}{e}", LogLevel.Error);
				return false;
			}

			// Add GMCM config page
			this.RegisterGenericModConfigMenuPage();

			return true;
		}

		private static string Tokenised(string key, string[] contexts = null, string[] owners = null)
		{
			string friendship = ModEntry.Definitions is null
				? string.Empty
				: $" {(int)((ModEntry.Definitions.AddedFriendship - 1) * 100)}%";
			string wrap = ModEntry.I18n.Get("item.giftwrap.name");
			string gift = ModEntry.I18n.Get("item.wrappedgift.name");
			string str = ModEntry.I18n.Get(key, new {
				WrapItemName = wrap,
				GiftItemName = gift,
				OptionalFriendship = friendship,
				Owners = owners is null
					? string.Empty
					: string.Join(", ", owners),
				NumOwners = owners is null
					? 0
					: owners.Length,
				NumOthers = contexts is null
					? 0
					: owners is null
						? contexts.Length
						: contexts.Length - owners.Length
			});
			return str;
		}

		private void RegisterGenericModConfigMenuPage()
		{
			IGenericModConfigMenuAPI api = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
			if (api is null)
				return;

			Dictionary<Config.Themes, Translation> themes = Enum
				.GetValues(typeof(Config.Themes))
				.Cast<Config.Themes>()
				.ToDictionary(
					value => value,
					value => ModEntry.I18n.Get("config.theme." + (int)value));

			api.Register(
				mod: this.ModManifest,
				reset: () => ModEntry.Config = new Config(),
				save: () => this.Helper.WriteConfig(ModEntry.Config));

			api.AddImage(
				mod: this.ModManifest,
				texture: () => this.Helper.ModContent.Load<Texture2D>(ModEntry.LocalBannerTexturePath),
				texturePixelArea: new(x: 0, y: 0, width: 106, height: 26));

			api.AddSectionTitle(
				mod: this.ModManifest,
				text: () => ModEntry.I18n.Get("config.section.shops"));

			api.AddParagraph(
				mod: this.ModManifest,
				text: () =>
				{
					string wrap = ModEntry.I18n.Get("item.giftwrap.name");
					string gift = ModEntry.I18n.Get("item.wrappedgift.name");
					string str = ModEntry.Tokenised("config.description.shopping");
					if (ModEntry.Shops is Shop[] shops && Game1.currentLocation is GameLocation location)
					{
						var contexts = shops
							.Where((Shop shop) => ModEntry.DoesShopPreconditionMatch(shop: shop, location: location) && shop.IfAlwaysAvailable == ModEntry.IsAlwaysAvailable)
							.Select((Shop shop) => shop.Context)
							.ToArray();
						var owners = contexts
							.Where((string context) => Game1.getCharacterFromName(context) is not null)
							.ToArray();
						if (owners.Any())
						{
							str += ModEntry.Tokenised("config.description.shopping.owners", owners: owners, contexts: contexts);
							if (contexts.Length - owners.Length is int diff && diff > 0)
							{
								str += ModEntry.Tokenised("config.description.shopping.others", owners: owners, contexts: contexts);
							}
						}
						else
						{
							str += ModEntry.Tokenised("config.description.shopping.none");
						}
					}

					return str;
				});

			api.AddSectionTitle(
				mod: this.ModManifest,
				text: () => ModEntry.I18n.Get("config.section.description"));
			
			api.AddParagraph(
				mod: this.ModManifest,
				text: () => ModEntry.Tokenised("config.description.usage")
					+ ModEntry.Tokenised("config.description.singleplayer")
					+ ModEntry.Tokenised("config.description.multiplayer"));

			api.AddSectionTitle(
				mod: this.ModManifest,
				text: () => ModEntry.I18n.Get("config.section.options"));
			
			// Themes
			api.AddTextOption(
				mod: this.ModManifest,
				name: () => ModEntry.I18n.Get("config.theme.name"),
				tooltip: () => ModEntry.I18n.Get("config.theme.description"),
				getValue: () => themes[ModEntry.Config.Theme],
				setValue: (string theme) =>
				{
					ModEntry.Config.Theme = (Config.Themes)Enum.Parse(typeof(Config.Themes), theme);
					ModEntry.Instance.Helper.GameContent.InvalidateCache(ModEntry.GameContentMenuTexturePath);
				},
				allowedValues: themes.Keys.Select((Config.Themes key) => key.ToString()).ToArray(),
				formatAllowedValue: (string theme) => themes[(Config.Themes)Enum.Parse(typeof(Config.Themes), theme)]);

			// Availability
			api.AddBoolOption(
				mod: this.ModManifest,
				name: () => ModEntry.I18n.Get("config.availableallyear.name"),
				tooltip: () => ModEntry.I18n.Get("config.availableallyear.description"),
				getValue: () => ModEntry.Config.AlwaysAvailable,
				setValue: (bool value) =>
				{
					ModEntry.Config.AlwaysAvailable = value;
					if (Context.IsOnHostComputer)
					{
						ModEntry.ReloadShops(isBroadcast: true);
					}
				});

			// Animations
			api.AddBoolOption(
				mod: this.ModManifest,
				name: () => ModEntry.I18n.Get("config.playanimations.name"),
				tooltip: () => ModEntry.I18n.Get("config.playanimations.description"),
				getValue: () => ModEntry.Config.PlayAnimations,
				setValue: (bool value) => ModEntry.Config.PlayAnimations = value);

			api.AddSectionTitle(
				mod: this.ModManifest,
				text: () => ModEntry.I18n.Get("config.section.multiplayer"));

			// Tooltip enabled
			api.AddBoolOption(
				mod: this.ModManifest,
				name: () => ModEntry.I18n.Get("config.giftpreviewtileenabled.name"),
				tooltip: () => ModEntry.I18n.Get("config.giftpreviewtileenabled.description"),
				getValue: () => ModEntry.Config.GiftPreviewEnabled,
				setValue: (bool value) => ModEntry.Config.GiftPreviewEnabled = value);

			// Tooltip range
			api.AddNumberOption(
				mod: this.ModManifest,
				name: () => ModEntry.I18n.Get("config.giftpreviewtilerange.name"),
				tooltip: () => ModEntry.I18n.Get("config.giftpreviewtilerange.description"),
				getValue: () => ModEntry.Config.GiftPreviewTileRange,
				setValue: (int value) => ModEntry.Config.GiftPreviewTileRange = value);

			// Tooltip fade
			const int min = 1;
			const int max = 20;
			api.AddNumberOption(
				mod: this.ModManifest,
				name: () => ModEntry.I18n.Get("config.giftpreviewfadespeed.name"),
				tooltip: () => ModEntry.I18n.Get("config.giftpreviewfadespeed.description", new { MinSpeed = min, MaxSpeed = max }),
				getValue: () => ModEntry.Config.GiftPreviewFadeSpeed,
				setValue: (int value) => ModEntry.Config.GiftPreviewFadeSpeed = value,
				min: min,
				max: max,
				interval: 1);
		}

		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			if (!this.TryLoadApis())
			{
				this.Monitor.Log("Failed to load required mods. Mod will not be loaded.", LogLevel.Error);
				return;
			}

			// Event handlers
			this.Helper.Events.Content.AssetRequested += AssetManager.OnAssetRequested;
			this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
			this.Helper.Events.Display.MenuChanged += this.OnMenuChanged;
			this.Helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
			this.Helper.Events.Multiplayer.ModMessageReceived += this.OnMessageReceived;
			SpaceEvents.BeforeGiftGiven += this.OnGiftGiven;

			// Gift data
			Data.Data data = this.Helper.GameContent.Load<Data.Data>(ModEntry.GameContentDataPath);
			ModEntry.Definitions = data.Definitions;
			ModEntry.GiftSprites = data.Styles.Values
				.Select((Style style) => style.Texture ?? ModEntry.GameContentGiftTexturePath)
				.Distinct()
				.ToList()
				.ToDictionary(
					(string path) => path,
					(string path) => new Lazy<Texture2D>(() => ModEntry.Instance.Helper.GameContent.Load<Texture2D>(path)));

			// Audio
			foreach (string id in data.Audio.Keys)
			{
				SoundEffect[] sounds = data.Audio[id].Select((string path) =>
				{
					path = Path.Combine(ModEntry.LocalAudioPath, $"{path}.wav");
					using FileStream stream = new(path, FileMode.Open);
					return SoundEffect.FromStream(stream);
				}).ToArray();
				CueDefinition cue = new(
					cue_name: id,
					sound_effects: sounds,
					category_id: Game1.audioEngine.GetCategoryIndex("Sound"));
				Game1.soundBank.AddCue(cue);
			}
			
			// Patches
			Harmony harmony = new(id: this.ModManifest.UniqueID);
			harmony.Patch(
				original: AccessTools.Method(type: typeof(Event), name: nameof(Event.chooseSecretSantaGift)),
				prefix: new HarmonyMethod(methodType: typeof(ModEntry), methodName: nameof(ModEntry.TrySecretSantaGift)));
		}

		/// <summary>
		/// Interactions for data receiving.
		/// </summary>
		private void OnMessageReceived(object sender, ModMessageReceivedEventArgs e)
		{
			if (e.FromModID == this.ModManifest.UniqueID && e.Type == MessageType.ReloadShops.ToString())
			{
				// Reload if notified by other players of shop data changes
				ModEntry.ReloadShops(isBroadcast: false);
			}
		}

		/// <summary>
		/// Interactions for session data.
		/// </summary>
		private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
		{
			// Reset data for all unique instances of this mod
			// Splitscreen clients are ignored here,
			// as they share the data of the host player
			if (Context.IsMainPlayer || !Context.IsOnHostComputer)
			{
				ModEntry.ResetSessionData();
			}
		}

		/// <summary>
		/// Interactions for data sharing.
		/// </summary>
		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			ModEntry.ReloadShops(isBroadcast: false);
		}

		/// <summary>
		/// Interactions for shop menus.
		/// </summary>
		private void OnMenuChanged(object sender, MenuChangedEventArgs e)
		{
			// Add items to shop stock
			if (e.NewMenu is ShopMenu menu && ModEntry.IsShopAllowed(menu: menu, location: Game1.currentLocation) is Shop shop)
			{
				ModEntry.AddToShop(menu: menu, shop: shop, item: new WrapItem());
			}
		}

		/// <summary>
		/// Interactions for wrapped gifts.
		/// </summary>
		private void OnGiftGiven(object sender, EventArgsBeforeReceiveObject e)
		{
			// Ignore NPC gifts that aren't going to be accepted
			if (!ModEntry.IsNpcAllowed(player: Game1.player, npc: e.Npc, gift: e.Gift))
				return;

			if (e.Gift is GiftItem gift)
			{
				// Cancel the wrapped gift NPC gift
				e.Cancel = true;

				Definitions definitions = this.Helper.GameContent.Load<Data.Data>(ModEntry.GameContentDataPath).Definitions;

				Item item = gift.ItemInGift.Value;
				if (!ModEntry.IsNpcGiftAllowed(item))
				{
					// Ignore actual gifts that are invalid NPC gifts, eg. Tools
					// Ignore actual gifts wrapped as part of large stacks, as items are typically only able to be given as gifts one-at-a-time
					Game1.showRedMessage(message: Game1.content.LoadString(definitions.InvalidGiftStringPath));
					Game1.playSound(definitions.InvalidGiftSound);
					return;
				}

				// Redeliver the NPC gift as the actual gift
				e.Npc.receiveGift(
					o: item as Object,
					giver: Game1.player,
					updateGiftLimitInfo: true,
					friendshipChangeMultiplier: definitions.AddedFriendship,
					showResponse: true);

				// Remove wrapped gift from inventory
				Game1.player.removeItemFromInventory(e.Gift);
			}
		}

		/// <summary>
		/// Harmony prefix method.
		/// Allows gifted items to be gifted to the player's secret friend during the Winter Star event.
		/// </summary>
		/// <param name="i">Item chosen as a gift.</param>
		/// <param name="who">Player choosing the gift.</param>
		private static void TrySecretSantaGift(ref Item i, Farmer who)
		{
			if (i is GiftItem gift)
			{
				if (gift.IsItemInside && Utility.highlightSantaObjects(i: gift.ItemInGift.Value))
				{
					// Unwrap valid gifts before given to the player's secret friend
					i = gift.ItemInGift.Value;
				}
			}
		}

		/// <summary>
		/// Regenerates shop data based on config files.
		/// </summary>
		/// <param name="isBroadcast">Whether to notify online farmers of shop data changes.</param>
		private static void ReloadShops(bool isBroadcast)
		{
			const string shopKey = ModEntry.ModDataPrefix + "Shops";
			const string availabilityKey = ModEntry.ModDataPrefix + "AlwaysAvailable";

			// Load mod definitions for all unique instances of this mod
			// i.e. host computer and each client

			string str;
			Data.Data data = ModEntry.Instance.Helper.GameContent.Load<Data.Data>(ModEntry.GameContentDataPath);
			if (Context.IsMainPlayer)
			{
				// Master player defines the values for a game session,
				// which are broadcast to farmhands via the player's ModData field

				// Shops
				ModEntry.Shops = data.Shops;
				string json = JsonConvert.SerializeObject(data.Shops);
				Game1.player.modData[shopKey] = json;

				// Availability
				ModEntry.IsAlwaysAvailable = ModEntry.Config.AlwaysAvailable;
				Game1.player.modData[ModEntry.ModDataPrefix + availabilityKey] = ModEntry.IsAlwaysAvailable.ToString();
			}
			else if (!Context.IsOnHostComputer)
			{
				// Remote players defer to master player for mod definitions,
				// fetching from the master player's ModData field whenever possible

				// Shops
				ModEntry.Shops = Game1.MasterPlayer.modData.TryGetValue(shopKey, out str)
					&& JsonConvert.DeserializeObject<Shop[]>(str) is Shop[] shops
					? shops
					: data.Shops;

				// Availability
				ModEntry.IsAlwaysAvailable = Game1.MasterPlayer.modData.TryGetValue(availabilityKey, out str)
					&& bool.TryParse(str, out bool isAlwaysAvailable)
					? isAlwaysAvailable
					: ModEntry.Config.AlwaysAvailable;
			}

			if (isBroadcast)
			{
				// Notify other instances of this mod that shop data has changed
				long[] farmerIDs = Game1.getOnlineFarmers()
					.Select((Farmer farmer) => farmer.UniqueMultiplayerID)
					.Where((long id) => id != Game1.player.UniqueMultiplayerID)
					.ToArray();
				ModEntry.Instance.Helper.Multiplayer.SendMessage<Message>(
					message: new Message(),
					messageType: MessageType.ReloadShops.ToString(),
					modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID },
					playerIDs: farmerIDs);
			}
		}

		/// <summary>
		/// Clears data relevant to the last-played saved game
		/// to prevent it appearing out of context.
		/// </summary>
		private static void ResetSessionData()
		{
			ModEntry.Shops = null;
			ModEntry.IsAlwaysAvailable = false;
		}

		/// <summary>
		/// Adds an item to a given shop menu with properties from a given shop entry. 
		/// </summary>
		/// <param name="menu">Shop menu to add to.</param>
		/// <param name="shop">Shop entry with sale data.</param>
		/// <param name="item">Item to add to shop.</param>
		public static void AddToShop(ShopMenu menu, Shop shop, ISalable item)
		{
			const int priceRounding = 5;
			float price = item.salePrice() * shop.PriceMultiplier;
			int priceRounded = (int)(price * (1f / priceRounding)) * priceRounding;
			int index = shop.AddAtItem?.FirstOrDefault((string name)
				=> menu.forSale.Any((ISalable i) => i.Name == name))
				is string name ? menu.forSale.FindIndex((ISalable i) => i.Name == name) + 1 : 0;

			menu.itemPriceAndStock.Add(item, new int[] { priceRounded, int.MaxValue });
			if (index >= 0)
				menu.forSale.Insert(index, item);
			else
				menu.forSale.Add(item);
		}

		public static Texture2D LoadWrapTexture()
		{
			return ModEntry.Instance.Helper.GameContent.Load<Texture2D>(ModEntry.GameContentWrapTexturePath);
		}

		public static string GetThemedTexturePath()
		{
			return string.Format(ModEntry.LocalMenuTexturePath, ModEntry.Config.Theme);
		}

		public static Texture2D GetStyleTexture(Style style)
		{
			return ModEntry.GiftSprites[style.Texture ?? ModEntry.GameContentGiftTexturePath].Value;
		}

		public static bool IsItemAllowed(Item item)
		{
			return item is not (null or WrapItem or GiftItem) && item.canBeTrashed();
		}

		public static bool IsLocationAllowed(GameLocation location)
		{
			return location is not (null or Mine or MineShaft or VolcanoDungeon or BeachNightMarket or MermaidHouse or AbandonedJojaMart)
				&& !location.isTemp();
		}

		public static bool IsTileAllowed(GameLocation location, Vector2 tile)
		{
			return location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(tile)
				&& !location.Objects.ContainsKey(tile)
				&& location.isCharacterAtTile(tile) is null
				&& location.isTileOccupiedByFarmer(tile) is null;
		}

		public static bool IsNpcAllowed(Farmer player, NPC npc, Item gift)
		{
			return npc.canReceiveThisItemAsGift(i: gift)
				&& player.friendshipData.TryGetValue(npc.Name, out Friendship data)
				&& data.GiftsThisWeek < 2
				&& data.GiftsToday == 0;
		}

		public static bool IsNpcGiftAllowed(Item item)
		{
			return item is Object o && o.canBeGivenAsGift() && o.Stack == 1;
		}

		public static Shop IsShopAllowed(ShopMenu menu, GameLocation location)
		{
			return ModEntry.Shops?.FirstOrDefault((Shop shop)
				=> (shop.Context is null
					|| shop.Context == menu.storeContext
					|| shop.Context == menu.portraitPerson?.Name)
				&& ModEntry.DoesShopPreconditionMatch(shop: shop, location: location)
				&& shop.IfAlwaysAvailable == ModEntry.IsAlwaysAvailable);
		}

		public static bool DoesShopPreconditionMatch(Shop shop, GameLocation location)
		{
			int id = ModEntry.Definitions.EventConditionId;
			return shop.Conditions is null
					|| shop.Conditions.Length == 0
					|| shop.Conditions.Any((string s) => location.checkEventPrecondition($"{id}/{s}") == id);
		}
	}
}
