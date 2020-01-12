using System;
using System.Collections.Generic;
using System.IO;

using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using xTile.Dimensions;
using xTile.ObjectModel;

/*
 *
 * authors note
 *
 * update all the manifest and content-pack files u idiot
 *
 */

/*
 * todo
 *
 * create an npc for artemis to dodge the custom house layering issues
 *
 */

namespace SailorStyles_Clothing
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance;

		internal Config Config;
		internal ITranslationHelper i18n => Helper.Translation;
		
		private static IJsonAssetsApi _jsonAssets;
		
		private NPC _catNPC;
		private NPC _cateNPC;
		internal static bool Cate;

		private Dictionary<ISalable, int[]> _catShopStock;

		public override void Entry(IModHelper helper)
		{
			Instance = this;
			Config = helper.ReadConfig<Config>();

			helper.Events.Input.ButtonReleased += OnButtonReleased;
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.DayStarted += OnDayStarted;
			helper.Events.Player.Warped += OnWarped;

			helper.Content.AssetLoaders.Add(new Editors.CustomNPCLoader(helper, Config.debugMode));
			helper.Content.AssetEditors.Add(new Editors.AnimDescEditor(helper, Config.debugMode));
			helper.Content.AssetEditors.Add(new Editors.MapEditor(helper, Config.debugMode));
		}
		
		private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
		{
			e.Button.TryGetKeyboard(out var keyPressed);

			if (Game1.activeClickableMenu != null || Game1.player.UsingTool || Game1.pickingTool || Game1.menuUp ||
			    (Game1.eventUp && !Game1.currentLocation.currentEvent.playerControlSequence) || Game1.nameSelectUp ||
			    Game1.numberOfSelectedItems != -1)
				return;

			if (e.Button.IsActionButton())
			{
				// thanks sundrop
				var grabTile = new Vector2(
					               Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y) / Game1.tileSize;
				if (!Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
					grabTile = Game1.player.GetGrabTile();
				var tile = Game1.currentLocation.map.GetLayer("Buildings").PickTile(new Location(
					(int)grabTile.X * Game1.tileSize, (int)grabTile.Y * Game1.tileSize), Game1.viewport.Size);
				PropertyValue action = null;
				tile?.Properties.TryGetValue("Action", out action);
				if (action != null)
				{
					var strArray = ((string)action).Split(' ');
					var args = new string[strArray.Length - 1];
					Array.Copy(strArray, 1, args, 0, args.Length);
					if (strArray[0].Equals(Const.CatID))
						CatShop();
				}
			}

			// debug junk
			if (keyPressed.ToSButton().Equals(Config.debugWarpKey) && Config.debugMode)
			{
				DebugWarpPlayer();
			}
		}

		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			_catShopStock = new Dictionary<ISalable, int[]>();

			_jsonAssets = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
			if (_jsonAssets == null)
			{
				Log.E("Can't access the Json Assets API. Is the mod installed correctly?");
				return;
			}
			
			var objFolder = new DirectoryInfo(Path.Combine(Helper.DirectoryPath, Const.JAShirtsDir));
			foreach (var subfolder in objFolder.GetDirectories())
				_jsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, Const.JAShirtsDir, subfolder.Name));

			objFolder = new DirectoryInfo(Path.Combine(Helper.DirectoryPath, Const.JAHatsDir));
			foreach (var subfolder in objFolder.GetDirectories())
				_jsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, Const.JAHatsDir, subfolder.Name));
		}
		
		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			CatShopRestock();

			// mmmmswsswsss
			/*
			var random = new Random();
			var randint = random.Next(Const.CateRate);
			Cate = randint == 0 || (Config.debugMode && Config.debugCate);
			Log.D("CateRate: " + randint + "/" + Const.CateRate + ", " + Cate,
				Config.debugMode);
			*/
		}
		
		private void OnWarped(object sender, WarpedEventArgs e)
		{
			ResetLocation(e);
		}

		private void ResetLocation(WarpedEventArgs e)
		{
			Log.D($"Resetting location and NPC data ({Const.LocationTarget})",
				Config.debugMode);

			Helper.Content.InvalidateCache(Const.AnimDescs);

			if (e.OldLocation.Name.Equals(Const.LocationTarget))
			{
				RemoveNPCs();
			}

			if (!e.NewLocation.Name.Equals(Const.LocationTarget))
				return;

			if (_catNPC == null && (Game1.dayOfMonth % 7 <= 1 || Config.debugMode))
				AddNPCs();

			Helper.Content.InvalidateCache(Path.Combine("Maps", Const.LocationTarget));
		}

		private void RemoveNPCs()
		{
			Log.D($"Removing NPCs at {Const.LocationTarget}",
				Config.debugMode);

			Game1.getLocationFromName(Const.LocationTarget).characters.Remove(_catNPC);
			_catNPC = null;

			Game1.getLocationFromName(Const.LocationTarget).characters.Remove(_cateNPC);
			_cateNPC = null;
		}

		private void AddNPCs()
		{
			Log.D($"Adding NPCs for {Const.LocationTarget}",
				Config.debugMode);

			_catNPC = new NPC(
				new AnimatedSprite(Const.CatSprite, 0, 16, 32),
				new Vector2(Const.CatX, Const.CatY) * 64.0f,
				Const.LocationTarget,
				2,
				Const.CatID,
				false,
				null,
				Helper.Content.Load<Texture2D>($@"Portraits/{Const.CatID}",
					ContentSource.GameContent));
			
			Game1.getLocationFromName(Const.LocationTarget).addCharacter(_catNPC);

			_catNPC.Schedule = _catNPC.getSchedule(Game1.dayOfMonth);
			_catNPC.scheduleTimeToTry = 9999999;
			_catNPC.ignoreScheduleToday = false;
			_catNPC.followSchedule = true;
			
			Log.D($"Cat name     : {_catNPC.Name}");
			Log.D($"Cat position : {_catNPC.position.X}, {_catNPC.position.Y}");
			Log.D($"Cat texture  : {Const.CatSprite}");

			Log.D("Cat schedule : ");
			if (_catNPC.Schedule != null)
				foreach (var entry in _catNPC.Schedule)
					Log.D($"{entry.Key}: {entry.Value.endOfRouteBehavior}");
			else
				Log.D("null");

			// ahahaha
			/*
			_cateNPC = new NPC(
				new AnimatedSprite("Characters\\Bouncer", 0, 64, 128),
				new Vector2(-64000f, 128f),
				Const.LocationTarget,
				2,
				Const.CatID + "e",
				false,
				null,
				Helper.Content.Load<Texture2D>(Const.CatePortrait));
			*/
		}

		private void CatShopRestock()
		{
			_catShopStock.Clear();
			/*
			Log.D("JA Hat IDs:",
				Config.debugMode);
			foreach (var id in JsonAssets.GetAllHatIds())
				Log.D($"{id.Key}: {id.Value}",
					Config.debugMode);
			*/
			PopulateShop(Const.JAHatsDir, 0);
			/*
			Log.D("JA Shirt IDs:",
				Config.debugMode);
			foreach (var id in JsonAssets.GetAllClothingIds())
				Log.D($"{id.Key}: {id.Value}",
					Config.debugMode);
			*/
			PopulateShop(Const.JAShirtsDir, 1);
		}
		
		private void PopulateShop(string dir, int type)
		{
			try
			{
				var stock = new List<int>();
				var random = new Random();

				var objFolder = new DirectoryInfo(Path.Combine(Helper.DirectoryPath, dir));
				var firstFolder = objFolder.GetDirectories()[0].GetDirectories()[0].GetDirectories()[0];
				var lastFolder = objFolder.GetDirectories()[objFolder.GetDirectories().Length - 1];
				lastFolder = lastFolder.GetDirectories()[0].GetDirectories()[lastFolder.GetDirectories()[0]
					.GetDirectories().Length - 1];

				Log.D($"CatShop first object: {firstFolder.Name}",
					Config.debugMode);
				Log.D($"CatShop last object: {lastFolder.Name}",
					Config.debugMode);

				var firstObject = 0;
				var lastObject = 0;

				switch(type)
				{
					case 0:
						firstObject = _jsonAssets.GetHatId(firstFolder.Name);
						lastObject = _jsonAssets.GetHatId(lastFolder.Name);
						break;
					case 1:
						firstObject = _jsonAssets.GetClothingId(firstFolder.Name);
						lastObject = _jsonAssets.GetClothingId(lastFolder.Name);
						break;
					default:
						Log.E("The CatShop hit a dead end. This feature wasn't finished!");
						throw new NotImplementedException();
				}

				var goalQty = (lastObject - firstObject) / Const.CatShopQtyRatio;

				Log.D("CatShop Restock bounds:",
					Config.debugMode);
				Log.D($"index: {firstObject}, end: {lastObject}, goalQty: {goalQty}",
					Config.debugMode);

				while (stock.Count < goalQty)
				{
					var id = random.Next(firstObject, lastObject);
					if (!stock.Contains(id))
						stock.Add(id);
				}

				foreach (var id in stock)
				{
					switch (type)
					{
						case 0:
							_catShopStock.Add(
								new StardewValley.Objects.Hat(id), new[]
								{ Const.ClothingCost, 1 });
							break;
						case 1:
							_catShopStock.Add(
								new StardewValley.Objects.Clothing(id), new[]
								{ Const.ClothingCost, 1 });
							break;
						default:
							Log.E("The CatShop hit a dead end. This feature wasn't finished!");
							throw new NotImplementedException();
					}
				}
			}
			catch (Exception ex)
			{
				Log.E("Sailor Styles failed to populate the clothes shop."
					+ "Did you remove all the clothing folders, or did I do something wrong?");
				Log.E("Exception logged:\n" + ex);
			}
		}

		private void CatShop()
		{
			Game1.playSound("cat");
			
			var text = (string) null;

			if (!Cate)
			{
				var random = new Random((int)((long)Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed));
				var whichDialogue = Const.ShopDialogueRoot + random.Next(5);
				if (whichDialogue.EndsWith("5"))
					whichDialogue += $".{Game1.currentSeason}";
				text = i18n.Get(whichDialogue);
			}
			else
			{
				// bllblblbl
				text = i18n.Get(Const.ShopDialogueRoot + "Cate");
			}
			
			Game1.activeClickableMenu = new ShopMenu(_catShopStock);
			((ShopMenu)Game1.activeClickableMenu).portraitPerson
				= Cate ? _cateNPC : _catNPC;
			((ShopMenu)Game1.activeClickableMenu).potraitPersonDialogue
				= Game1.parseText(text, Game1.dialogueFont, 304);
		}

		private static void DebugWarpPlayer()
		{
			Game1.warpFarmer(Const.LocationTarget, 31, 97, 2);
			Log.D($"Warped {Game1.player.Name} to the CatShop.");
		}
	}
}
