using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;

/*
 *
 * authors note
 * update all the manifest files u idiot
 * THAT GOES FOR THE CHARACTER PACK TOO
 *
 */

namespace SailorStyles_Clothing
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance;

		internal Config Config;
		internal ITranslationHelper i18n => Helper.Translation;
		
		private NPC _catNpc;
		private Dictionary<ISalable, int[]> _catShopStock;
		private static IJsonAssetsApi _ja;

		public override void Entry(IModHelper helper)
		{
			Instance = this;
			Config = helper.ReadConfig<Config>();

			helper.Events.Input.ButtonReleased += OnButtonReleased;
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.DayStarted += OnDayStarted;
			helper.Events.GameLoop.DayEnding += OnDayEnding;

			helper.Content.AssetLoaders.Add(new Editors.NpcLoader(helper));
			helper.Content.AssetEditors.Add(new Editors.AnimsEditor(helper));
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
				TryGrabTileAction();

			if (keyPressed.ToSButton().Equals(Config.DebugWarpKey) && Config.DebugMode)
				DebugWarpPlayer();
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
			
			foreach (var pack in Const.HatPacks)
				_ja.LoadAssets(Path.Combine(Helper.DirectoryPath, "Assets", Const.HatsDir, pack));
			foreach (var pack in Const.ClothingPacks)
				_ja.LoadAssets(Path.Combine(Helper.DirectoryPath, "Assets", Const.ClothingDir, pack));
		}
		
		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			Log.D($"Today's {Game1.shortDayDisplayNameFromDayOfSeason(Game1.dayOfMonth)} the "
			      + $"{Game1.dayOfMonth}th of {Game1.CurrentSeasonDisplayName}: "
			      + $"{(ShouldAddCatShop() ? "nice day for a cat" : "nothing special")}",
				Config.DebugMode);
			if (ShouldAddCatShop())
			{
				var location = Game1.getLocationFromName(Const.LocationTarget);
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
			var location = Game1.getLocationFromName(Const.LocationTarget);
			RemoveNpcs(location);
			RemoveTiles(location);
		}

		#endregion

		#region Manager Methods

		private void TryGrabTileAction()
		{
			var grabTile = new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y)
			               / Game1.tileSize;
			if (!Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
				grabTile = Game1.player.GetGrabTile();
			var tile = Game1.currentLocation.map.GetLayer("Buildings").PickTile(new Location(
				(int)grabTile.X * Game1.tileSize, (int)grabTile.Y * Game1.tileSize), Game1.viewport.Size);

			if (CheckForTileAction(tile, Const.CatId))
				CatShop();
		}

		public static bool CheckForTileAction(Tile tile, string value)
		{
			if (tile == null)
				return false;
			tile.Properties.TryGetValue("Action", out var action);
			if (action != null)
			{
				var strArray = ((string)action).Split(' ');
				var args = new string[strArray.Length - 1];
				Array.Copy(strArray, 1, args, 0, args.Length);
				if (strArray[0].Equals(value))
					return true;
			}

			return false;
		}

		public static bool ShouldAddCatShop()
		{
			var facts = Game1.dayOfMonth % 7 <= 1 || 
			            (Instance.Config.DebugMode && Instance.Config.DebugAlwaysCaturday);
			Log.D($"Cat should be added: {(facts ? "yes!" : "no..")}");
			return facts;
		}

		private void AddNpcs(GameLocation location)
		{
			Log.D("Adding cat..",
				Config.DebugMode);

			if (location == null || location.getCharacterFromName(Const.CatId) != null)
				return;
			_catNpc = new NPC(
				new AnimatedSprite(Const.CatSprite, 0, 16, 32),
				new Vector2(Const.CatX, Const.CatY) * 64.0f,
				Const.LocationTarget,
				2,
				Const.CatId,
				false,
				null,
				Helper.Content.Load<Texture2D>($@"Portraits/{Const.CatId}",
					ContentSource.GameContent));
			ForceNPCSchedule(_catNpc);
			location.addCharacter(_catNpc);
		}

		private void ForceNPCSchedule(NPC npc)
		{
			npc.Schedule = npc.getSchedule(Game1.dayOfMonth);
			npc.scheduleTimeToTry = 9999999;
			npc.ignoreScheduleToday = false;
			npc.followSchedule = true;
		}

		private void RemoveNpcs(GameLocation location)
		{
			if (location.getCharacterFromName(Const.CatId) != null)
			{
				Log.D("Removing cat..",
					Config.DebugMode);
				location.characters.Remove(_catNpc);
			}
			_catNpc = null;
		}

		private void AddTiles(GameLocation location)
		{
			Log.D($"Adding to {location.NameOrUniqueName}",
				Config.DebugMode);

			var map = location.Map;
			const BlendMode mode = BlendMode.Additive;
			var sheet = map.GetTileSheet("outdoors");
			var layer = map.GetLayer("Buildings");
			if (layer.Tiles[Const.CatX, Const.CatY] == null)
				// Adds a magic blank buildings tile to hold the CatShop property if none is there (there usually isnt)
				layer.Tiles[Const.CatX, Const.CatY] = new StaticTile(layer, sheet, mode, Const.DummyTileIndex);
			layer.Tiles[Const.CatX, Const.CatY].Properties["Action"] = new PropertyValue(Const.CatId);
		}

		private void RemoveTiles(GameLocation location)
		{
			var map = location.Map;
			var layer = map?.GetLayer("Buildings");
			var tile = layer?.Tiles[Const.CatX, Const.CatY];
			if (tile == null)
				return;
			Log.D($"Cleaning up {location.NameOrUniqueName}",
				Config.DebugMode);
			if (CheckForTileAction(tile, Const.CatId))
				tile.Properties["Action"] = null;
			if (tile.TileIndex == Const.DummyTileIndex)
				layer.Tiles[Const.CatX, Const.CatY] = null;
		}
		#endregion

		#region CatShop Methods

		private void CatShopRestock()
		{
			_catShopStock.Clear();
			PopulateShop(true);
			PopulateShop(false);
		}

		private string GetContentPackId(string name)
		{
			return Regex.Replace(Const.ContentPackPrefix + name,
				"[^a-zA-Z0-9_.]", "");
		}

		private void PopulateShop(bool isHat)
		{
			try
			{
				var random = new Random();
				var stock = new List<string>();
				var contentPacks = isHat
					? Const.HatPacks
					: Const.ClothingPacks;
				
				foreach (var contentPack in contentPacks)
				{
					var contentPackId = GetContentPackId(contentPack);
					var contentNames = isHat
						? _ja.GetAllHatsFromContentPack(contentPackId)
						: _ja.GetAllClothingFromContentPack(contentPackId);
					if (contentPack.EndsWith("Kimono"))
						contentNames.RemoveAll(n => n.EndsWith("wer"));
					if (contentNames == null || contentNames.Count < 1)
					{
						Log.E($"Failed to populate content from {contentPack}\n({contentPackId}).");
						throw new NullReferenceException();
					}

					Log.D($"Stocking content from {contentPack}\n({contentPackId}).",
						Config.DebugMode);

					stock.Clear();
					var currentQty = 0;
					var goalQty = Math.Max(1, contentNames.Count / Const.CatShopQtyRatio);
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
							_catShopStock.Add(new StardewValley.Objects.Hat(_ja.GetHatId(name)), new[]
								{ Const.PackCosts[contentPack], 1 });
						else
							_catShopStock.Add(new StardewValley.Objects.Clothing(_ja.GetClothingId(name)), new[]
								{ Const.PackCosts[contentPack], 1 });
					}
				}
			}
			catch (Exception ex)
			{
				Log.E("Sailor Styles failed to populate the clothes shop."
					+ " Did you install the clothing folders, or did I break something?");
				Log.E("Exception logged:\n" + ex);
			}
		}

		private void CatShop()
		{
			Game1.playSound("cat");
			
			var random = new Random((int)((long)Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed));
			var whichDialogue = Const.ShopDialogueRoot + random.Next(5);
			if (whichDialogue.EndsWith("5"))
				whichDialogue += $".{Game1.currentSeason}";
			var text = i18n.Get(whichDialogue);
			
			Game1.activeClickableMenu = new ShopMenu(_catShopStock);
			((ShopMenu) Game1.activeClickableMenu).portraitPerson = _catNpc;
			((ShopMenu)Game1.activeClickableMenu).potraitPersonDialogue
				= Game1.parseText(text, Game1.dialogueFont, 304);
		}

		#endregion

		#region Debug Methods

		private static void DebugWarpPlayer()
		{
			Log.D($"Pressed {Instance.Config.DebugWarpKey} : Warped {Game1.player.Name}.");
			if (Game1.player.currentLocation.Name.Equals(Const.LocationTarget))
				Game1.warpFarmer("FarmHouse", 30, 15, 2);
			else
				Game1.warpFarmer(Const.LocationTarget, 31, 97, 2);
		}

		#endregion
	}
}
