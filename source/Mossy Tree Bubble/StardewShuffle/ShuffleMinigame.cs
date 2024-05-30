/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tocseoj/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Minigames;
using SObject = StardewValley.Object;

namespace Tocseoj.Stardew.StardewShuffle;
internal class ShuffleMinigame(IMonitor Monitor, IManifest ModManifest, IModHelper Helper, ModConfig Config)
	: ModComponent(Monitor, ModManifest, Helper, Config), IMinigame
{
	private bool quitMinigame = false;
	private Texture2D backgroundSprite = null!;
	private Texture2D springCrops = null!;
	const int GAME_WIDTH = 320; // Hardcoded size of assets/StardewShufflePlayingTable.png

	const int GAME_HEIGHT = 160; // Hardcoded size of assets/StardewShufflePlayingTable.png

	const float SCALE = 4f;
	private readonly float CropScale = 2f;
	private readonly Rectangle frontOfCardSourceRect = new(384, 396, 15, 15);
	private readonly Rectangle backOfCardSourceRect = new(399, 396, 15, 15);
	// 60x76 56x76 52x76 56x76
	private List<Rectangle> inPlayRects = [
		// Top row
		new(49, 3, 44, 70),
		new(112, 3, 44, 70),
		new(170, 3, 44, 70),
		new(228, 3, 44, 70),
		// Bottom row
		new(49, 87, 44, 70),
		new(112, 87, 44, 70),
		new(170, 87, 44, 70),
		new(228, 87, 44, 70),
	];
	// private readonly Rectangle playerDeckRect = new(292, 63, 29, 52);
	private Rectangle playerDeckRect = new(292, 63, 70, 44);
	private Vector2 playerHandPoint = new(100, 200);
	private Rectangle opponentDeckRect = new(-42, 47, 70, 44);
	private Vector2 opponentHandPoint = new(300, 0);
	private Rectangle scaledPlayerDeckRect = Rectangle.Empty;
	private Vector2 scaledPlayerHandPoint = Vector2.Zero;
	private Rectangle scaledOpponentDeckRect = Rectangle.Empty;
	private Vector2 scaledOpponentHandPoint = Vector2.Zero;
	private List<Rectangle> scaledInPlayRects = null!;
	private Rectangle scaledGameRect = Rectangle.Empty;
	private Rectangle scaledWindowRect = Rectangle.Empty;
	private Rectangle playerHandLocation = Rectangle.Empty;
	private Vector2 upperLeft = Vector2.Zero; // unused
	private List<int[]> playerCards = null!; // example from CalicoJack.cs

	private Dictionary<string, Texture2D> cardTextures = []; // unused
	private TemporaryAnimatedSpriteList animations = [];
	private class Participant() {
		public int Money = 0;
	}
	private class GameState() {
		public Participant Player = new();
		public Participant Opponent = new();
	}
	private class Card(SObject item, bool flipped = false, string? name = null, string? description = null, int? price = null, int? cropIndex = null)
	{
		public SObject Item = item;
		public int? CropIndex = cropIndex;
		private readonly string? name = name;
		public string Name {
			get { return name ?? Item.DisplayName; }
		}
		private readonly string? description = description;
		public string Description {
			get { return description ?? Item.getDescription(); }
		}
		private readonly int? price = price;
		public int Price {
			get { return price ?? Item.Price; }
		}

		public bool Flipped = flipped;
	}
	// private List<ValueTuple<Card?, Rectangle>> table = null!;
	private List<Card> playerDeck = [];
	private List<Card> playerHand = [];
	private List<Card> playerDiscardPile = [];
	private List<Card?> playerCardsInPlay = [null, null, null, null];
	private Card? heldCard = null;
	private List<Card> opponentDeck = [];
	private List<Card> opponentHand = [];
	private List<Card> opponentDiscardPile = [];
	private List<Card?> opponentCardsInPlay = [null, null, null, null];
	private GameState gameState = new();
	public void Start()
	{
		if (!Context.IsWorldReady) {
			Monitor.Log("You need to load a save first!", LogLevel.Info);
			return;
		}
		quitMinigame = false;
		backgroundSprite = Helper.ModContent.Load<Texture2D>("assets/StardewShufflePlayingTable.png");
		springCrops = Game1.content.Load<Texture2D>("Maps\\springobjects");
		changeScreenSize();
		playerHandLocation = new(scaledWindowRect.Left + 30, scaledWindowRect.Bottom - 120, 420, 120);
		playerCards ??= [[1, 400], [2, 400], [3, 400], [4, 400], [5, 400]];
		opponentCardsInPlay = [null, new Card(new SObject("190", 1, false, -1, SObject.lowQuality)), null, null];
		playerCardsInPlay = [null, null, new Card(new("254", 1, false, -1, SObject.lowQuality)), new Card(new("254", 1, false, -1, SObject.lowQuality))];
		playerHand = [
			new Card(new("479", 1, false, -1, SObject.lowQuality)),
			new Card(new("479", 1, false, -1, SObject.lowQuality)),
			new Card(new("479", 1, false, -1, SObject.lowQuality)),
		];
		opponentHand = [
			new Card(new("474", 1, false, -1, SObject.lowQuality), true),
			new Card(new("474", 1, false, -1, SObject.lowQuality), true),
			new Card(new("474", 1, false, -1, SObject.lowQuality), true),
			new Card(new("474", 1, false, -1, SObject.lowQuality), true),
			new Card(new("474", 1, false, -1, SObject.lowQuality), true),
			new Card(new("474", 1, false, -1, SObject.lowQuality), true),
			new Card(new("474", 1, false, -1, SObject.lowQuality), true),
			new Card(new("474", 1, false, -1, SObject.lowQuality), true),
			new Card(new("474", 1, false, -1, SObject.lowQuality), true),
			new Card(new("474", 1, false, -1, SObject.lowQuality), true),
		];
		gameState = new();
		Game1.currentMinigame = this;
	}
	public void AddConsoleCommand()
	{
		Helper.ConsoleCommands.Add("sshuffle", "Start a game of Stardew Shuffle.", (string command, string[] args) => {
			if (args.Length > 0 && args[0] == "--test-crane") {
				if (!Context.IsWorldReady)
					return;
				// Fixed resolution at 4x scale
				Game1.currentMinigame = new CraneGame();
				return;
			}
			if (args.Length > 0 && args[0] == "--test-boat") {
				if (!Context.IsWorldReady)
					return;
				// Will fill whole screen, but things will be cut off
				Game1.currentMinigame = new BoatJourney();
				return;
			}
			if (args.Length > 0 && args[0] == "--test-plane") {
				if (!Context.IsWorldReady)
					return;
				// unused/test animation?
				Game1.currentMinigame = new PlaneFlyBy();
				return;
			}
			if (args.Length > 0 && args[0] == "--test-game") {
				if (!Context.IsWorldReady)
					return;
				// card game
				Game1.currentMinigame = new CalicoJack();
				return;
			}
			if (args.Length > 0 && args[0] == "--shwip") {
				if (!Context.IsWorldReady)
					return;
				Game1.playSound("shwip");
				return;
			}
			if (args.Length > 0 && args[0] == "--version") {
				Monitor.Log($"Stardew Shuffle v{ModManifest.Version}", LogLevel.Info);
				return;
			}
			if (args.Length > 0 && args[0] == "--help") {
				Monitor.Log($"Usage: {command}", LogLevel.Info);
				Monitor.Log("Start a game of Stardew Shuffle.", LogLevel.Info);
				Monitor.Log("Options:", LogLevel.Info);
				Monitor.Log("--version: Display the version of Stardew Shuffle.", LogLevel.Info);
				Monitor.Log("--help: Display this help message.", LogLevel.Info);
				Monitor.Log("--test-crane: Run the crane game for comparison testing.", LogLevel.Info);
				Monitor.Log("--test-boat: Run the boat journey for comparison testing.", LogLevel.Info);
				Monitor.Log("--test-plane: Run the plane flyby for comparison testing.", LogLevel.Info);
				Monitor.Log("--test-game: Run CalicoJack card game for comparison testing.", LogLevel.Info);
				Monitor.Log("--shwip: Play the shwip sound.", LogLevel.Info);
				return;
			}
			if (args.Length > 0) {
				Monitor.Log("Invalid arguments. Use --help for more information.", LogLevel.Info);
				return;
			}
			Start();
		});
	}
	private Texture2D LoadTextureFromPath(string path)
	{
		if (!cardTextures.ContainsKey(path))
			cardTextures[path] = Helper.ModContent.Load<Texture2D>(path);
		return cardTextures[path];
	}

	private void DrawCard(SpriteBatch b, Card c, Vector2 v) {
		DrawCard(b, c, new Rectangle((int)v.X, (int)v.Y, (int)(44 * SCALE), (int)(70 * SCALE)));
	}
	private void DrawCard(SpriteBatch b, Card c, Rectangle r)
	{
		if (c.Flipped) {
			// Card frame
			IClickableMenu.drawTextureBox(b, Game1.mouseCursors, backOfCardSourceRect, r.Left, r.Top, r.Width, r.Height, Color.White, SCALE);
		}
		else {
			// Card frame
			IClickableMenu.drawTextureBox(b, Game1.mouseCursors, frontOfCardSourceRect, r.Left, r.Top, r.Width, r.Height, Color.White, SCALE);
			// Sprite
			c.Item.drawInMenu(b, r.Center.ToVector2() - new Vector2(32,32), CropScale);
			// Could use this for animation flourish later
			// if (c.CropIndex != null) {
			// 	int cropIndex = (int)c.CropIndex;
			// 	b.Draw(Game1.cropSpriteSheet, r.Center.ToVector2() - new Vector2(32,64), Game1.getSourceRectForStandardTileSheet(Game1.cropSpriteSheet, cropIndex, 16, 32), Color.White, 0f, Vector2.Zero, SCALE, SpriteEffects.None, 0.95f);
			// }
			// Title
			if (c.Name.Contains("Seeds")) {
				// string cropName = c.Name[..^6];
				SpriteText.drawStringHorizontallyCenteredAt(b, c.Name.First() + "." + "Seed", r.Center.X + 6, r.Top + 28);
			}
			else if (c.Name.Length > 6) {
				SpriteText.drawStringHorizontallyCenteredAt(b, c.Name[..6] + ".", r.Center.X + 6, r.Top + 28);
			}
			else {
				SpriteText.drawStringHorizontallyCenteredAt(b, c.Name, r.Center.X, r.Top + 28);
			}
			// Price
			SpriteText.drawStringHorizontallyCenteredAt(b, c.Price.ToString(), r.Center.X, r.Bottom - 74);
			b.Draw(Game1.debrisSpriteSheet, new Vector2(r.Center.X + 55, r.Bottom - 50), Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, 8, 16, 16), Color.White, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 0.95f);
		}
	}
	public bool tick(GameTime time)
	{
		if (time.TotalGameTime.TotalSeconds % 5 < 0.0001)
			Monitor.Log($"Ticking...{time.TotalGameTime.TotalSeconds}");
		if (quitMinigame)
			unload();

		// for flipping card animation
		for (int k = 0; k < playerCards.Count; k++)
		{
			if (playerCards[k][1] > 0)
			{
				playerCards[k][1] -= time.ElapsedGameTime.Milliseconds;
				if (playerCards[k][1] <= 0)
				{
					playerCards[k][1] = 0;
				}
			}
		}

		return quitMinigame;
	}
	public bool overrideFreeMouseMovement()
	{
		Monitor.Log("Overriding free mouse movement");
		return false;
	}
	public bool doMainGameUpdates()
	{
		// Monitor.Log("Doing main game updates"); // Runs every tick
		return false;
	}
	public void receiveLeftClick(int x, int y, bool playSound = true)
	{
		bool actionedOnThisClick = false;
		// Monitor.Log($"Received left click at {x}, {y}. Playing sound: {playSound}");
		if (scaledPlayerDeckRect.Contains(x, y)) {
			Monitor.Log("Clicked player deck");
			// playerHand.Add(new Card(new("479", 1, false, -1, SObject.lowQuality)));
			string itemId = Game1.random.Next(1, 1000).ToString();
			playerHand.Add(new Card(new(itemId, 1, false, -1, SObject.lowQuality)));
			actionedOnThisClick = true;
		}
		else if (scaledOpponentDeckRect.Contains(x, y)) {
			Monitor.Log("Clicked opponent deck");
			opponentHand.Add(new Card(new("474", 1, false, -1, SObject.lowQuality), true));
			actionedOnThisClick = true;
		}
		else {
			List<Card?> cardsInPlay = [..opponentCardsInPlay, ..playerCardsInPlay];
			for (int i = 0; i < scaledInPlayRects.Count; i++) {
				Rectangle r = scaledInPlayRects[i];
				if (r.Contains(x, y)) {
					Card? c = cardsInPlay[i];
					if (c != null && heldCard == null) {
						// Grab card
						// Monitor.Log($"Picked up card {c.Name}");
						// heldCard = c;
						// if (i < 4) {
						// 	opponentCardsInPlay[i] = null;
						// }
						// else {
						// 	playerCardsInPlay[i - 4] = null;
						// }
						if (c.Name.Contains("Seeds")) {
							// Grow Card
							c.CropIndex += 1;
						}
						else {
							// Sell card
							Monitor.Log($"Sold card {c.Name}");
							Vector2 positionOfMoneyCount;
							if (i < 4) {
								opponentCardsInPlay[i] = null;
								gameState.Opponent.Money += c.Price;
								string opponentMoney = gameState.Opponent.Money.ToString() + "  ";
								positionOfMoneyCount = new(scaledWindowRect.Left + 80, scaledWindowRect.Top + 70);
							}
							else {
								playerCardsInPlay[i - 4] = null;
								gameState.Player.Money += c.Price;
								string playerMoney = gameState.Player.Money.ToString() + "  ";
								positionOfMoneyCount = new(scaledWindowRect.Right - 80 - SpriteText.getWidthOfString(playerMoney), scaledWindowRect.Bottom - 125);
							}
							actionedOnThisClick = true;
							// Gain money animation
							Vector2 snappedPosition = new(r.X + 22 * SCALE, r.Y + 30 * SCALE);
							int coins = 2 + c.Price / 50;
							for (int j = 0; j < coins; j++)
							{
								animations.Add(new TemporaryAnimatedSprite("TileSheets\\debris", new Rectangle(Game1.random.Next(2) * 16, 64, 16, 16), 9999f, 1, 999, snappedPosition + new Vector2(32f, 32f), flicker: false, flipped: false)
								{
									alphaFade = 0.025f,
									motion = new Vector2(Game1.random.Next(-3, 4), -4f),
									acceleration = new Vector2(0f, 0.5f),
									delayBeforeAnimationStart = j * 25,
									scale = 2f
								});
								animations.Add(new TemporaryAnimatedSprite("TileSheets\\debris", new Rectangle(Game1.random.Next(2) * 16, 64, 16, 16), 9999f, 1, 999, snappedPosition + new Vector2(32f, 32f), flicker: false, flipped: false)
								{
									scale = 4f,
									alphaFade = 0.025f,
									delayBeforeAnimationStart = j * 50,
									motion = Utility.getVelocityTowardPoint(new Point((int)snappedPosition.X + 32, (int)snappedPosition.Y + 32), positionOfMoneyCount, 8f),
									acceleration = Utility.getVelocityTowardPoint(new Point((int)snappedPosition.X + 32, (int)snappedPosition.Y + 32), positionOfMoneyCount, 0.5f)
								});
							}
							// TODO; play sound
						}

					}
					else if (c == null && heldCard != null) {
						// Place card
						Monitor.Log($"Placed card {heldCard.Name}");
						if (heldCard.Name.Contains("Seeds")) {
							heldCard.CropIndex = 0;
						}
						if (i < 4) {
							opponentCardsInPlay[i] = heldCard;
						}
						else {
							playerCardsInPlay[i - 4] = heldCard;
						}
						heldCard = null;
						actionedOnThisClick = true;
					}
				}
			}
			Card? grabThisCard = null;
			for (int i = 0; i < playerHand.Count; i++) {
				Card c = playerHand[i];
				Rectangle r = new(scaledWindowRect.Left + 50 + i * 100, scaledWindowRect.Bottom - 75, (int)(44 * SCALE), (int)(70 * SCALE));
				if (r.Contains(x, y)) {
					if (heldCard == null) {
						// Grab card
						grabThisCard = c;
					}
					// else if (!actionedOnThisClick && heldCard != null) {
					// 	// Place card in hand
					// 	Monitor.Log($"Placed card {heldCard.Name}");
					// 	playerHand.Insert(i, heldCard);
					// 	heldCard = null;
					// 	actionedOnThisClick = true;
					// }
				}
			}
			if (grabThisCard != null) {
				Monitor.Log($"Picked up card {grabThisCard.Name}");
				playerHand.Remove(grabThisCard);
				heldCard = grabThisCard;
				actionedOnThisClick = true;
			}
			if (!actionedOnThisClick && heldCard != null) {
				if (playerHandLocation.Contains(x, y)) {
					Monitor.Log("Clicked player hand");
					playerHand.Add(heldCard);
						heldCard = null;
					actionedOnThisClick = true;
				}
			}
		}
	}
	public void leftClickHeld(int x, int y)
	{
		// Monitor.Log($"Left click held at {x}, {y}");
	}
	public void releaseLeftClick(int x, int y)
	{
		// Monitor.Log($"Released left click at {x}, {y}");
	}
	public void receiveRightClick(int x, int y, bool playSound = true)
	{
		Monitor.Log($"Received right click at {x}, {y}. Playing sound: {playSound}");
	}
	public void releaseRightClick(int x, int y)
	{
		Monitor.Log($"Released right click at {x}, {y}");
	}
	public void receiveKeyPress(Keys k)
	{
		Monitor.Log($"Received key press: {k}");
		if (k == Keys.Escape)
      quitMinigame = true;
	}
	public void receiveKeyRelease(Keys k)
	{
		Monitor.Log($"Received key release: {k}");
	}
	public void draw(SpriteBatch b)
	{
		b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
		b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.graphics.GraphicsDevice.Viewport.Width, Game1.graphics.GraphicsDevice.Viewport.Height), new Color(234, 145, 78)); // TODO: test split screen?
		b.Draw(backgroundSprite, scaledGameRect.Location.ToVector2(), backgroundSprite.Bounds, Color.White, 0f, Vector2.Zero, SCALE, SpriteEffects.None, 1f);

		List<Card?> cardsInPlay = [..opponentCardsInPlay, ..playerCardsInPlay];
		// In-play layer
		for (int i = 0; i < scaledInPlayRects.Count; i++) {
			Rectangle r = scaledInPlayRects[i];

			Card? c = cardsInPlay[i];
			if (c == null)
				continue;

			DrawCard(b, c, r);
		}
		// Deck piles
		IClickableMenu.drawTextureBox(b, Game1.mouseCursors, backOfCardSourceRect, scaledPlayerDeckRect.Left, scaledPlayerDeckRect.Top, scaledPlayerDeckRect.Width, scaledPlayerDeckRect.Height, Color.White, SCALE);
		IClickableMenu.drawTextureBox(b, Game1.mouseCursors, backOfCardSourceRect, scaledOpponentDeckRect.Left, scaledOpponentDeckRect.Top, scaledOpponentDeckRect.Width, scaledOpponentDeckRect.Height, Color.PaleVioletRed, SCALE);
		// Player Hand
		// card dimensions: 44x70 (*SCALE)
		for (int i = 0; i < playerHand.Count; i++) {
			Card c = playerHand[i];
			Rectangle r = new(scaledWindowRect.Left + 50 + i * 100, scaledWindowRect.Bottom - 100, (int)(44 * SCALE), (int)(70 * SCALE));
			DrawCard(b, c, r);
		}
		for (int i = 0; i < opponentHand.Count; i++) {
			Card c = opponentHand[i];
			Vector2 v = new(scaledWindowRect.Right - 200 - i * 50, scaledWindowRect.Top - 200);
			DrawCard(b, c, v);
		}

		// Rectangle window = new(0, 0, Game1.graphics.GraphicsDevice.Viewport.Width, Game1.graphics.GraphicsDevice.Viewport.Height);
		// IClickableMenu.drawTextureBox(b, Game1.mouseCursors, backOfCardSourceRect, scaledWindowRect.Left, scaledWindowRect.Top, scaledWindowRect.Width, scaledWindowRect.Height, Color.White, SCALE);

		// Helper overlay
		if (heldCard != null) {
			if (playerHand.Count > 3) {
				// between 100 and 110
				playerHandLocation.Width = 110 + 103 * playerHand.Count;
			}
			else {
				playerHandLocation.Width = 420;
			}
			IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), playerHandLocation.Left, playerHandLocation.Top, playerHandLocation.Width, playerHandLocation.Height, Color.LightCoral, SCALE, drawShadow: false);
		}

		// UI layer
		string playerMoney = gameState.Player.Money.ToString() + "  ";
		SpriteText.drawStringWithScrollBackground(b, playerMoney, scaledWindowRect.Right - 80 - SpriteText.getWidthOfString(playerMoney), scaledWindowRect.Bottom - 125);
		b.Draw(Game1.debrisSpriteSheet, new Vector2(scaledWindowRect.Right - 100, scaledWindowRect.Bottom - 98), Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, 8, 16, 16), Color.White, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 0.95f);

		string opponentMoney = gameState.Opponent.Money.ToString() + "  ";
		SpriteText.drawStringWithScrollBackground(b, opponentMoney, scaledWindowRect.Left + 80, scaledWindowRect.Top + 70);
		b.Draw(Game1.debrisSpriteSheet, new Vector2(scaledWindowRect.Left + 60 + SpriteText.getWidthOfString(opponentMoney), scaledWindowRect.Top + 97), Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, 8, 16, 16), Color.White, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 0.95f);

		// Tooltip layer
		// TODO; make sure when cards overlap we are showing the correct tooltip
		for (int i = 0; i < scaledInPlayRects.Count; i++) {
			Rectangle r = scaledInPlayRects[i];

			Card? c = cardsInPlay[i];
			if (c == null)
				continue;

			if (r.Contains(Game1.getOldMouseX(), Game1.getOldMouseY())) {
				IClickableMenu.drawHoverText(b, c.Description, Game1.smallFont, heldCard != null ? 40 : 0, heldCard != null ? 40 : 0, moneyAmountToDisplayAtBottom: c.Price, boldTitleText: c.Name);
			}
		}
		Card? hoveringCard = null;
		for (int i = 0; i < playerHand.Count; i++) {
			Card c = playerHand[i];
			Rectangle r = new(scaledWindowRect.Left + 50 + i * 100, scaledWindowRect.Bottom - 75, (int)(44 * SCALE), (int)(70 * SCALE));
			if (r.Contains(Game1.getOldMouseX(), Game1.getOldMouseY())) {
				hoveringCard = c;
			}
		}
		if (hoveringCard != null) {
			IClickableMenu.drawHoverText(b, hoveringCard.Description, Game1.smallFont, heldCard != null ? 40 : 0, heldCard != null ? 40 : 0, moneyAmountToDisplayAtBottom: hoveringCard.Price, boldTitleText: hoveringCard.Name);
		}

		// Sell animation
		for (int j = this.animations.Count - 1; j >= 0; j--)
		{
			if (this.animations[j].update(Game1.currentGameTime))
			{
				this.animations.RemoveAt(j);
			}
			else
			{
				this.animations[j].draw(b, localPosition: true);
			}
		}

		// Held item on cursor
		heldCard?.Item.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 16, Game1.getOldMouseY() + 16), 1f);

		// From CalicoJack.cs
		if (Game1.IsMultiplayer) {
			Utility.drawTextWithColoredShadow(b, Game1.getTimeOfDayString(Game1.timeOfDay), Game1.dialogueFont, new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width - Game1.dialogueFont.MeasureString(Game1.getTimeOfDayString(Game1.timeOfDay)).X - 16f, Game1.graphics.GraphicsDevice.Viewport.Height - Game1.dialogueFont.MeasureString(Game1.getTimeOfDayString(Game1.timeOfDay)).Y - 10f), Color.White, Color.Black * 0.2f);
		}
		b.End();
	}
	public void changeScreenSize()
	{
		Monitor.Log("Changing screen size");
		float pixel_zoom_adjustment = 1f / Game1.options.zoomLevel;
		Rectangle localMultiplayerWindow = Game1.game1.localMultiplayerWindow;
		float w = localMultiplayerWindow.Width;
		float h = localMultiplayerWindow.Height;
		Vector2 tmp = new Vector2(w / 2f, h / 2f) * pixel_zoom_adjustment;
		scaledWindowRect = new(localMultiplayerWindow.X, localMultiplayerWindow.Y, (int)(w * pixel_zoom_adjustment), (int)(h * pixel_zoom_adjustment));
		tmp.X -= GAME_WIDTH / 2 * SCALE;
		tmp.Y -= GAME_HEIGHT / 2 * SCALE;
		upperLeft = tmp; // no longer needed
		scaledGameRect = new((int)tmp.X, (int)tmp.Y, (int)(GAME_WIDTH * SCALE), (int)(GAME_HEIGHT * SCALE));
		scaledInPlayRects = [];
		foreach (Rectangle r in inPlayRects) {
			scaledInPlayRects.Add(new((int)(r.X * SCALE + tmp.X), (int)(r.Y * SCALE + tmp.Y), (int)(r.Width * SCALE), (int)(r.Height * SCALE)));
		}
		scaledPlayerDeckRect = new((int)(playerDeckRect.X * SCALE + tmp.X), (int)(playerDeckRect.Y * SCALE + tmp.Y), (int)(playerDeckRect.Width * SCALE), (int)(playerDeckRect.Height * SCALE));
		scaledOpponentDeckRect = new((int)(opponentDeckRect.X * SCALE + tmp.X), (int)(opponentDeckRect.Y * SCALE + tmp.Y), (int)(opponentDeckRect.Width * SCALE), (int)(opponentDeckRect.Height * SCALE));

		// Test
		Rectangle window = new(0, 0, Game1.graphics.GraphicsDevice.Viewport.Width, Game1.graphics.GraphicsDevice.Viewport.Height);
		Monitor.Log($"Window: {window}");
		scaledPlayerHandPoint = new(window.Left + 8, window.Bottom);
		scaledOpponentHandPoint = new(window.Right - 8, window.Top - 50 * SCALE);
	}
	public void unload()
	{
		Monitor.Log("Unloading");
	}
	public void receiveEventPoke(int data)
	{
		Monitor.Log($"Received event poke: {data}");
	}
	public string minigameId()
	{
		Monitor.Log("Getting minigame ID");
		return "Tocseoj.StardewShuffle";
	}
	public bool forceQuit()
	{
		Monitor.Log("Force quitting!");
		return quitMinigame;
	}
}