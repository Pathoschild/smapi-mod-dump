/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/geko_x/stardew-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Util;

namespace CritterCatcher {
	class ModEntry : Mod {

		public static ModEntry INSTANCE;
		public static IModHelper modhelper;
		public static ITranslationHelper i18n;

		private static readonly List<Critter> critters = new List<Critter>();

		private GameLocation currentLocation;

		public ToolBugNet bugnet;
		public ObjectCritter critterObj;

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper) {
			INSTANCE = this;
			modhelper = helper;
			i18n = helper.Translation;

			Monitor.Log("Mod Entry", LogLevel.Trace);

			modhelper.Events.GameLoop.GameLaunched += OnGameLaunched;
			//modhelper.Events.Input.ButtonPressed += OnButtonPressed;
			modhelper.Events.Player.Warped += OnPlayerWarped;

			modhelper.Events.Display.Rendered += OnRendered;
		}

		void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
			//ObjectCritter critter = new ObjectCritter();
			//critter.Init();

			Monitor.Log("Adding items", LogLevel.Info);

			bugnet = new ToolBugNet();
			bugnet.Init("Bugnet");
			Monitor.Log($"Added Bug Net with ID: {bugnet.itemId}", LogLevel.Info);

			critterObj = new ObjectCritter();
			critterObj.Init();

			Monitor.Log($"Added Critter Bag with ID: {critterObj.itemId}", LogLevel.Info);


			//var platoHelper = Helper.GetPlatoHelper();

			//string bugNetToolString = "Geko_X:CritterCatcher:Bugnet/" + 10 + "/-99/Basic -20/" + i18n.Get("name_bugnet") + "/" + i18n.Get("name_bugnet");

			//platoHelper.Content.GetSaveIndex(
			//		"Geko_X.CritterCatcher.Bugnet",
			//		() => Game1.objectInformation,
			//		(handle) => handle.Value == bugNetToolString,
			//		(handle) => platoHelper.Content.Injections.InjectDataInsert("Data//ObjectInformation", handle.Index, bugNetToolString));

			//BugNet.LoadTextures(platoHelper);
			//BugNet.TileIndex = ((Game1.toolSpriteSheet.Width / 16) * (Game1.toolSpriteSheet.Height / 16)) + 99;
			//platoHelper.Harmony.PaztchTileDraw("Geko_X.CritterCatcher.Bugnet", () => Game1.toolSpriteSheet, BugNet.Texture, null, BugNet.TileIndex);

			//platoHelper.Harmony.LinkContruction<StardewValley.Tools.GenericTool, BugNet>();
			//platoHelper.Harmony.LinkTypes(typeof(StardewValley.Tools.GenericTool), typeof(BugNet));

			Monitor.Log("Done", LogLevel.Info);

			//Helper.Events.Display.MenuChanged += (s, ev) => {
			//	if (ev.NewMenu is ShopMenu shop && shop.portraitPerson.Name == config.Shop) {
			//		var sale = SeedBagTool.GetNew(platoHelper);

			//		if (!shop.itemPriceAndStock.Keys.Any(k => k is Tool t && t.netName.Value.Contains("SeedBag") || k.DisplayName == sale.DisplayName || k.DisplayName == i18n.Get("Name"))) {
			//			shop.itemPriceAndStock.Add(sale, new int[2] { config.Price, 1 });
			//			shop.forSale.Add(sale);
			//		}
			//	}
			//};

		}

		//void OnButtonPressed(object sender, ButtonPressedEventArgs e) {

		//	if (!Context.IsWorldReady)
		//		return;

		//	if (!Context.IsPlayerFree)
		//		return;

		//	if (e.Button.IsUseToolButton()) {

		//		Vector2 tilePos = this.Helper.Input.GetCursorPosition().Tile;
		//		AttemptCatchCritter(tilePos);
		//	}
		//}

		private void OnRendered(object sender, RenderedEventArgs e) {
			if (!Context.IsWorldReady)
				return;

			if (!Context.IsPlayerFree)
				return;

			ICursorPosition cursorPos = this.Helper.Input.GetCursorPosition();

			Critter c = GetCrittersAtPosition(cursorPos.AbsolutePixels, Game1.tileSize);

			string str = "Critter: None";
			if (c != null) {
				str = "Critter: " + c.GetType().ToString();
			}

			str += $"({cursorPos.AbsolutePixels})";

			//string str = $"Mouse: {cursorPos.AbsolutePixels}";

			Game1.spriteBatch.DrawString(Game1.smallFont, str, cursorPos.ScreenPixels, Color.Black);
		}


		private void OnPlayerWarped(object sender, WarpedEventArgs e) {

			if (!e.IsLocalPlayer)
				return;

			currentLocation = e.NewLocation;
			UpdateCritterListForLocation();

		}

		private void UpdateCritterListForLocation(bool shouldClear = true) {
			if (currentLocation == null)
				return;

			if (shouldClear)
				critters.Clear();

			foreach(var c in modhelper.Reflection.GetField<List<Critter>>(currentLocation, "critters").GetValue()) {
				if(	c is Birdie ||
					c is Butterfly ||
					c is CalderaMonkey ||
					c is Firefly ||
					c is CrabCritter ||
					c is Crow ||
					c is Frog ||
					c is Owl ||
					c is Rabbit ||
					c is Seagull || 
					c is Squirrel ||
					c is Squirrel) {

					AddToCritterList(c);

				}
			}

			//Monitor.Log("Critter list", LogLevel.Info);

			//foreach (var c in critters) {
			//	Monitor.Log($"Critter: {c.GetType()} | {c.position} | {c.sprite.textureName} - {c.baseFrame}", LogLevel.Info);
			//}
		}

		private void AddToCritterList(Critter c) {
			critters.Add(c);
		}

		private Critter GetCrittersAtPosition(Vector2 position, int radius = 1) {

			UpdateCritterListForLocation(true);

			int rX1 = (int)position.X - radius;
			int rX2 = (int)position.X + radius;
			int rY1 = (int)position.Y - radius;
			int rY2 = (int)position.Y + radius;

			//Monitor.Log($"Getting critter in area ({rX1}, {rY1}) - ({rX2}, {rY2})", LogLevel.Info);

			foreach (var c in critters) {
				float x = c.position.X;
				float y = c.position.Y;

				//Monitor.Log($"Checking position {c.position}", LogLevel.Info);

				if (rX1 < x && x < rX2 && rY1 < y && y < rY2) {
					Monitor.Log($"Found critter: {c}", LogLevel.Info);
					return c;
				}
			}

			//Monitor.Log("Nothing found", LogLevel.Info);
			return null;
		}

		public Critter AttemptCatchCritter(Vector2 tileLocation) {
			Vector2 position = new Vector2(
								tileLocation.X * Game1.tileSize + (Game1.tileSize / 2),
								tileLocation.Y * Game1.tileSize + (Game1.tileSize / 2));

			Critter c = GetCrittersAtPosition(position, (int)(Game1.tileSize * 1.5f));

			Monitor.Log($"Attempt Catch Critter - TilePos: {tileLocation} Position: {position} Critter: {c}", LogLevel.Info);

			if (c == null)
				return null;

			Monitor.Log($"Caught a critter: {c}", LogLevel.Info);

			modhelper.Reflection.GetField<List<Critter>>(currentLocation, "critters").GetValue().Remove(c);
			return c;
		}
	}
}
