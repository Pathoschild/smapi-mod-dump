using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using xTile.ObjectModel;
using xTile.Tiles;

namespace SailorStyles
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance;

		internal Config Config;
		internal ITranslationHelper i18n => Helper.Translation;
		
		private NPC _catNpc;
		private Dictionary<ISalable, int[]> _catShopStock;
		private static IJsonAssetsApi _ja;

		public override object GetApi()
		{
			return new Api();
		}

		public override void Entry(IModHelper helper)
		{
			Instance = this;
			Config = helper.ReadConfig<Config>();

			helper.Events.Input.ButtonReleased += OnButtonReleased;
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.DayStarted += OnDayStarted;
			helper.Events.GameLoop.DayEnding += OnDayEnding;

			helper.Content.AssetLoaders.Add(new Editors.NpcLoader(helper));
			helper.Content.AssetEditors.Add(new Editors.NpcLoader(helper));

			if (Config.EnableHairstyles)
				helper.Content.AssetEditors.Add(new Editors.HairstylesEditor(helper));
		}
		
		#region Game Events

		private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
		{
			e.Button.TryGetKeyboard(out var keyPressed);

			if (Game1.activeClickableMenu != null || Game1.player.UsingTool || Game1.pickingTool || Game1.menuUp
			    || (Game1.eventUp && !Game1.currentLocation.currentEvent.playerControlSequence)
			    || Game1.nameSelectUp ||Game1.numberOfSelectedItems != -1)
				return;

			if (e.Button.IsActionButton())
			{
				var tile = Utility.Vector2ToPoint(e.Cursor.GrabTile);
				if (Game1.currentLocation.doesEitherTileOrTileIndexPropertyEqual(
					tile.X, tile.Y, "Action", "Buildings", ModConsts.CatId))
					CatShop();
			}
		}

		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			_catShopStock = new Dictionary<ISalable, int[]>();

			_ja = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
			if (_ja == null)
			{
				Log.E("Can't access the Json Assets API. Is the mod installed correctly?");
				return;
			}
			
			foreach (var pack in ModConsts.HatPacks)
				_ja.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets", ModConsts.HatsDir, pack));
			foreach (var pack in ModConsts.ClothingPacks)
				_ja.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets", ModConsts.ClothingDir, pack));
		}
		
		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			Log.D($"Today's {Game1.shortDayDisplayNameFromDayOfSeason(Game1.dayOfMonth)} the "
			      + $"{Game1.dayOfMonth}th of {Game1.CurrentSeasonDisplayName}: "
			      + $"{(ShouldAddCatShop() ? "nice day for a cat" : "nothing special")}",
				Config.DebugMode);
			if (ShouldAddCatShop())
			{
				var location = Game1.getLocationFromName(ModConsts.CatLocation);
				AddTiles(location);
				AddNpcs(location);
				CatShopRestock();

				Log.D($"Cat status: {(_catNpc != null ? "cute" : "nowhere")}",
					Config.DebugMode);
				Log.D($"Shop status: {(_catShopStock.Count > 0 ? "fluffy" : "skinny")}",
					Config.DebugMode);
			}
		}
		
		private void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			var location = Game1.getLocationFromName(ModConsts.CatLocation);
			RemoveNpcs(location);
			RemoveTiles(location);
		}

		#endregion

		#region Manager Methods

		public bool ShouldAddCatShop()
		{
			var facts = Game1.dayOfMonth % 7 <= 1 || 
			            (Instance.Config.DebugMode && Instance.Config.DebugCaturday);
			Log.D($"Cat should be added: {(facts ? "yes!" : "no..")}",
				Config.DebugMode);
			return facts;
		}

		private void AddNpcs(GameLocation location)
		{
			Log.D("Adding cat..",
				Config.DebugMode);

			if (location == null || location.getCharacterFromName(ModConsts.CatId) != null)
				return;
			_catNpc = new NPC(
				new AnimatedSprite(ModConsts.CatSpritesheet, 0, 16, 32),
				new Vector2(ModConsts.CatX, ModConsts.CatY) * 64.0f,
				ModConsts.CatLocation,
				2,
				ModConsts.CatId,
				false,
				null,
				Helper.Content.Load<Texture2D>(
					Path.Combine("Portraits", ModConsts.CatId),
					ContentSource.GameContent));
			ForceNpcSchedule(_catNpc);
			location.addCharacter(_catNpc);
		}

		private void ForceNpcSchedule(NPC npc)
		{
			try
			{
				npc.Schedule = npc.getSchedule(Game1.dayOfMonth);
				npc.ignoreScheduleToday = false;
				npc.followSchedule = true;
				if (Constants.TargetPlatform != GamePlatform.Android)
					npc.scheduleTimeToTry = 9999999;
				else
					Helper.Reflection.GetField<int>(npc, "scheduleTimeToTry").SetValue(9999999);
			}
			catch (Exception e)
			{
				Log.D($"Caught exception in ForceNpcSchedule:\n{e}",
					Config.DebugMode);
			}
		}

		private void RemoveNpcs(GameLocation location)
		{
			if (location.getCharacterFromName(ModConsts.CatId) != null)
			{
				Log.D("Removing cat..",
					Config.DebugMode);
				location.characters.Remove(_catNpc);
			}
			_catNpc = null;
		}

		private void AddTiles(GameLocation location)
		{
			var map = location.Map;
			var sheet = map.GetTileSheet("outdoors");
			var layer = map.GetLayer("Buildings");
			const BlendMode mode = BlendMode.Additive;
			if (layer.Tiles[ModConsts.CatX, ModConsts.CatY] == null)
				// Adds a magic blank buildings tile to hold the CatShop property if none is there
				// (there usually isnt, and its probably a problem for the cat if there is)
				layer.Tiles[ModConsts.CatX, ModConsts.CatY] = new StaticTile(layer, sheet, mode, ModConsts.DummyTileIndex);
			layer.Tiles[ModConsts.CatX, ModConsts.CatY].Properties["Action"] = new PropertyValue(ModConsts.CatId);
		}

		private void RemoveTiles(GameLocation location)
		{
			var map = location.Map;
			var layer = map?.GetLayer("Buildings");
			var tile = layer?.Tiles[ModConsts.CatX, ModConsts.CatY];
			if (tile == null)
				return;
			Log.D($"Cleaning up {location.NameOrUniqueName}",
				Config.DebugMode);
			if (Game1.currentLocation.doesEitherTileOrTileIndexPropertyEqual(
				ModConsts.CatX, ModConsts.CatY, "Action", "Buildings", ModConsts.CatId))
				tile.Properties["Action"] = null;
			if (tile.TileIndex == ModConsts.DummyTileIndex)
				layer.Tiles[ModConsts.CatX, ModConsts.CatY] = null;
		}

		#endregion

		#region CatShop Methods

		private void CatShopRestock()
		{
			_catShopStock.Clear();
			PopulateShop(true);
			PopulateShop(false);
		}

		private static string GetContentPackId(string name)
		{
			return Regex.Replace(ModConsts.ContentPackPrefix + name,
				"[^a-zA-Z0-9_.]", "");
		}

		private int GetContentPackCost(string name)
		{
			return ModConsts.ClothingPackCosts.ContainsKey(name)
				? ModConsts.ClothingPackCosts[name]
				: ModConsts.DefaultClothingCost;
		}

		private void PopulateShop(bool isHat)
		{
			try
			{
				var random = new Random();
				var stock = new List<string>();
				var contentPacks = isHat
					? ModConsts.HatPacks
					: ModConsts.ClothingPacks.Concat(Config.ExtraContentPacksToSellInTheShop);
				
				foreach (var contentPack in contentPacks)
				{
					var contentPackId = !ModConsts.HatPacks.Contains(contentPack)
					                    && !ModConsts.ClothingPacks.Contains(contentPack)
						? contentPack 
						: GetContentPackId(contentPack);
					var contentNames = isHat
						? _ja.GetAllHatsFromContentPack(contentPackId)
						: _ja.GetAllClothingFromContentPack(contentPackId);
					if (contentNames == null || contentNames.Count < 1)
					{
						if (!ModConsts.HatPacks.Contains(contentPack) && !ModConsts.ClothingPacks.Contains(contentPack))
						{
							Log.D($"Did not add content from {contentPack} (as {contentPackId}): Not found.",
								Config.DebugMode);
							continue;
						}
						Log.E($"Failed to populate content from {contentPack}\n({contentPackId}).");
						throw new NullReferenceException();
					}
					if (contentPack.EndsWith("Kimono"))
						contentNames.RemoveAll(n => n.EndsWith("wer"));

					Log.D($"Stocking content from {contentPack}\n({contentPackId}).",
						Config.DebugMode);

					stock.Clear();
					var currentQty = 0;
					var goalQty = Math.Max(1, contentNames.Count / ModConsts.CatShopQtyRatio);
					while (currentQty < goalQty)
					{
						var name = contentNames[random.Next(contentNames.Count - 1)];

						if (stock.Contains(name))
							continue;
						++currentQty;
						stock.Add(name);

						if (!contentPack.EndsWith("Kimono"))
							continue;
						++currentQty;
						stock.Add(name.Replace("Upp", "Low"));
					}

					foreach (var name in stock)
					{
						Log.D($"CatShop: Adding {name}",
							Config.DebugMode);
						if (isHat)
							_catShopStock.Add(new StardewValley.Objects.Hat(
								_ja.GetHatId(name)), new[]
								{ GetContentPackCost(contentPack), 1 });
						else
							_catShopStock.Add(new StardewValley.Objects.Clothing(
								_ja.GetClothingId(name)), new[]
								{ GetContentPackCost(contentPack), 1 });
					}
				}
			}
			catch (Exception ex)
			{
				Log.E("Sailor Styles failed to populate the clothes shop."
					+ " Did you remove the clothing folders, or did I break something?");
				Log.E("Exception logged:\n" + ex);
			}
		}

		private void CatShop()
		{
			Game1.playSound("cat");
			
			var random = new Random((int)((long)Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed));
			var whichDialogue = ModConsts.ShopDialogueRoot + random.Next(6);
			if (whichDialogue.EndsWith("5"))
				whichDialogue += $".{Game1.currentSeason}";
			var text = i18n.Get(whichDialogue);
			
			Game1.activeClickableMenu = new ShopMenu(_catShopStock);
			((ShopMenu) Game1.activeClickableMenu).portraitPerson = _catNpc;
			((ShopMenu)Game1.activeClickableMenu).potraitPersonDialogue
				= Game1.parseText(text, Game1.dialogueFont, 304);
		}

		#endregion
	}
}
