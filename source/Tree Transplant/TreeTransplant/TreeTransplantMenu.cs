/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeonBlade/TreeTransplant
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;

namespace TreeTransplant
{
	public class TreeTransplantMenu : IClickableMenu
	{
		private GameLocation savedLocation;
		private Farm farm;
		private Rectangle greenSquare;
		private Rectangle redSquare;
		private TreeRenderer selectedTree;
		private Vector2 selectedTreeLocation;
		private bool canPlace = true;
		private bool[] validSpot = new bool[9];
		private ClickableTextureComponent cancelButton;
		private ClickableTextureComponent flipButton;
		private readonly string defaultText = TreeTransplant.helper.Translation.Get("Menu_DefaultText");
		private readonly string placementText = TreeTransplant.helper.Translation.Get("Menu_PlacementText");

		public TreeTransplantMenu()
		{
			// startup to the menu
			Game1.playSound("dwop");
			Game1.globalFadeToBlack(open);
			Game1.player.forceCanMove();
			resetBounds();

			// set the rectangles for green and red square
			greenSquare = new Rectangle(194, 388, 16, 16);
			redSquare = new Rectangle(210, 388, 16, 16);
		}

		/// <summary>
		/// Called on opening the menu.
		/// </summary>
		public void open()
		{
			// clean up before leaving the area
			Game1.currentLocation.cleanupBeforePlayerExit();
			// save a copy of the location we're at
			savedLocation = Game1.currentLocation;
			// move to the farm
			Game1.currentLocation = Game1.getLocationFromName("Farm");
			// reset the location for our entry
			Game1.currentLocation.resetForPlayerEntry();
			// disable the HUD
			Game1.displayHUD = false;
			// freeze the viewport
			Game1.viewportFreeze = true;
			// set the new viewport
			Game1.viewport.Location = new xTile.Dimensions.Location(49 * Game1.tileSize, 5 * Game1.tileSize);
			// pan the screen
			Game1.panScreen(0, 0);
			// don't render our character
			Game1.displayFarmer = false;
			// set the farm
			farm = Game1.currentLocation as Farm;
			// fade the screen in with no callback
			Game1.globalFadeToClear();
		}

		/// <summary>
		/// Called to close the menu.
		/// </summary>
		public void close()
		{
			if (!readyToClose())
				return;

			// no more tree selection
			selectedTree = null;
			// clean up before leaving the area
			Game1.currentLocation.cleanupBeforePlayerExit();
			// move to the farm
			Game1.currentLocation = savedLocation;
			// reset the location for our entry
			Game1.currentLocation.resetForPlayerEntry();
			// enable the HUD
			Game1.displayHUD = true;
			// unfreeze the viewport
			Game1.viewportFreeze = false;
			// render our character
			Game1.displayFarmer = true;
			// fade the screen in with no callback
			Game1.globalFadeToClear();

			// exit the menu
			exitThisMenu();
		}

		/// <summary>
		/// Called when a key is pressed
		/// </summary>
		/// <param name="key">Key.</param>
		public override void receiveKeyPress(Keys key)
		{
			// we're still fading so ignore key input
			if (Game1.IsFading())
				return;

			// handle input ensuring that invalid keys in mapping don't get checked
			if (Game1.options.doesInputListContain(Game1.options.menuButton, key))
				handleCancelAction();

			if (!Game1.options.SnappyMenus)
			{
				if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
					Game1.panScreen(0, 4);
				if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
					Game1.panScreen(0, -4);
				if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
					Game1.panScreen(4, 0);
				if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
					Game1.panScreen(-4, 0);
			}
		}

		/// <summary>
		/// Handles closing the menu.
		/// </summary>
		private void handleCancelAction()
		{
			if (readyToClose())
				Game1.globalFadeToBlack(new Game1.afterFadeFunction(close));
			else if (selectedTree != null)
			{
				selectedTree = null;
				Game1.playSound("shwip");
			}
		}

		/// <summary>
		/// Called on game update
		/// </summary>
		/// <param name="time">The current game time.</param>
		public override void update(GameTime time)
		{
			// call the base class update method
			base.update(time);
			// if we're still fading then just ignore
			if (Game1.globalFade)
				return;

			// get X and Y relative to the viewport
			int x = Game1.getOldMouseX(ui_scale: false) + Game1.viewport.X;
			int y = Game1.getOldMouseY(ui_scale: false) + Game1.viewport.Y;

			// checks if we moved more than a tile size and if so pan the screen
			if (x - Game1.viewport.X < 64)
				Game1.panScreen(-8, 0);
			else if (x - (Game1.viewport.X + Game1.viewport.Width) >= -128)
				Game1.panScreen(8, 0);
			if (y - Game1.viewport.Y < 64)
				Game1.panScreen(0, -8);
			else if (y - (Game1.viewport.Y + Game1.viewport.Height) >= -64)
				Game1.panScreen(0, 8);

			// calculate valid placement for cursor tiles and for actual placement status
			calculateValidPlacement();
		}

		/// <summary>
		/// Called to calculate the placement of the selected tree.
		/// </summary>
		private void calculateValidPlacement()
		{
			// don't care if there's no tree of course
			if (selectedTree == null)
				return;

			// reset the placement
			canPlace = true;
			validSpot = new bool[] {
				true, true, true,
				true, true, true,
				true, true, true
			};

			// get the mouse position (don't use the x and y from the function)
			var mouseX = Game1.getOldMouseX(ui_scale: false);
			var mouseY = Game1.getOldMouseY(ui_scale: false);

			// get cursor tile location
			var tileLocation = new Vector2(
				(Game1.viewport.X + mouseX) / Game1.tileSize,
				(Game1.viewport.Y + mouseY) / Game1.tileSize
			);

			// get fast reference to tile location coordinates
			var tileX = (int)tileLocation.X;
			var tileY = (int)tileLocation.Y;

			// check the center tile to see if its valid
			if (!isTileValid(tileX, tileY))
			{
				canPlace = false;
				validSpot[4] = false;
			}

			// the IDs of the tileLocations mapped to the validSpot index
			int[] ids = {
				3,5,7,
				1,  0,
				2,8,6
			};

			// get the surrounding tile locations based on what tree we have
			Vector2[] tileLocations;
			if (selectedTree.tree.getTerrainFeature() is Tree)
				tileLocations = Utility.getAdjacentTileLocations(tileLocation).ToArray();
			else if (selectedTree.tree.getTerrainFeature() is FruitTree)
				tileLocations = Utility.getSurroundingTileLocationsArray(tileLocation);
			else
				throw new Exception("Selected tree is somehow not of Tree or FruitTree");

			// is this a normal tree
			bool isNormalTree = tileLocations.Length == 4;

			for (int i = 0; i < tileLocations.Length; i++)
			{
				bool flag = isTileValid((int)tileLocations[i].X, (int)tileLocations[i].Y, isNormalTree || (!isNormalTree && selectedTree.tree.isAdult()));
				validSpot[ids[i]] = flag;
				if (!flag)
					canPlace = flag;
			}
		}

		/// <summary>
		/// Is the tile valid to place the tree on
		/// </summary>
		/// <returns><c>true</c>, if tile is valid for the tree, <c>false</c> otherwise.</returns>
		/// <param name="tileX">Tile x.</param>
		/// <param name="tileY">Tile y.</param>
		/// <param name="onlyTrees">If set to <c>true</c> only care about other trees.</param>
		private bool isTileValid(int tileX, int tileY, bool onlyTrees = false)
		{
			var tileLocation = new Vector2(tileX, tileY);

			// only care about searching for the trees
			if (onlyTrees)
				return !doesTileContainTerrainFeature(tileLocation, onlyTrees);

			// get properties of tiles
			bool isWater = farm.isOpenWater(tileX, tileY);
			bool isOccupied = farm.isTileOccupiedForPlacement(tileLocation);
			bool isPassable = farm.isTilePassable(new xTile.Dimensions.Location(tileX, tileY), Game1.viewport);
			bool isPlaceable = farm.isTilePlaceable(tileLocation);
			bool noSpawnAll = farm.doesTileHaveProperty(tileX, tileY, "NoSpawn", "Back") == "All";
			bool hasTF = farm.terrainFeatures.ContainsKey(tileLocation);
			bool canceling = tileLocation == selectedTreeLocation;

			return (!isWater && isPassable && isPlaceable && !noSpawnAll && !isOccupied && !hasTF) || canceling;
		}

		/// <summary>
		/// Checks tile location for trees.
		/// </summary>
		/// <returns><c>true</c>, if tile contains a tree, <c>false</c> otherwise.</returns>
		/// <param name="tileLocation">Tile location.</param>
		private bool doesTileContainTree(Vector2 tileLocation)
		{
			return doesTileContainTerrainFeature(tileLocation, true);
		}

		/// <summary>
		/// Checks if there is a terrain feature in the selected tile location
		/// </summary>
		/// <returns><c>true</c>, if tile contain terrain feature, <c>false</c> otherwise.</returns>
		/// <param name="tileLocation">Tile location.</param>
		/// <param name="onlyTrees">If set to <c>true</c> only check for trees.</param>
		private bool doesTileContainTerrainFeature(Vector2 tileLocation, bool onlyTrees = false)
		{
			// if the tile location is the same than our selected tree we will immediately just return false
			if (tileLocation == selectedTreeLocation)
				return false;

			bool hasTerrainFeature = farm.terrainFeatures.ContainsKey(tileLocation);
			if (!hasTerrainFeature)
				return false;
			else if (!onlyTrees)
				return true;
			var tf = farm.terrainFeatures[tileLocation];

			bool isTree = (tf is Tree || tf is FruitTree);
			// make exception for smaller trees
			if (tf is Tree)
				return (tf as Tree).growthStage.Value > 4;
			return isTree;
		}

		/// <summary>
		/// Called when left click is performed in our menu
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="playSound">If set to <c>true</c> play sound.</param>
		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			// invoke base class method
			base.receiveLeftClick(x, y, playSound);

			// if we are fading then ignore
			if (Game1.globalFade)
				return;

			// get the mouse position (don't use the x and y from the function)
			var mouseX = Game1.getOldMouseX(ui_scale: false);
			var mouseY = Game1.getOldMouseY(ui_scale: false);

			// get cursor tile location
			var tileLocation = new Vector2(
				(Game1.viewport.X + mouseX) / Game1.tileSize,
				(Game1.viewport.Y + mouseY) / Game1.tileSize
			);

			if (cancelButton.containsPoint(x, y))
			{
				handleCancelAction();
				return;
			}

			if (flipButton.containsPoint(x, y))
			{
				flipTree();
				return;
			}

			if (!canPlace && selectedTree != null)
			{
				Notifier.Message(TreeTransplant.helper.Translation.Get("Menu_InvalidPlace"));
				Game1.playSound("cancel");
			}
			else if (selectedTree != null && tileLocation == selectedTreeLocation)
			{
				if (selectedTree.tree.flipped == selectedTree.flipped)
					Game1.playSound("shwip");
				else
					Game1.playSound("dirtyHit");
				selectedTree.propFlip();
				selectedTree = null;
			}
			else if (Game1.currentLocation.terrainFeatures.ContainsKey(tileLocation))
			{
				// get our terrain feature
				var terrainFeature = Game1.currentLocation.terrainFeatures[tileLocation];
				// make sure its the type we care about
				if (terrainFeature is FruitTree || terrainFeature is Tree)
				{
					if (terrainFeature is Tree && (terrainFeature as Tree).tapped.Value)
					{
						Notifier.Message(TreeTransplant.helper.Translation.Get("Menu_InvalidCap"));
						Game1.playSound("cancel");
						return;
					}
					// set the selected tree                    
					selectedTree = new TreeRenderer(terrainFeature);
					selectedTreeLocation = tileLocation;
					Game1.playSound("hoeHit");
				}
			}
			else if (selectedTree != null)
			{
				// remove the original 
				Game1.currentLocation.terrainFeatures.Remove(selectedTreeLocation);
				// perform the flip that's done in memory
				selectedTree.propFlip();
				// add a new one in the spot selected
				Game1.currentLocation.terrainFeatures.Add(tileLocation, selectedTree.tree.getTerrainFeature());
				// play sound of seed placement
				Game1.playSound("dirtyHit");
				// deselect the tree
				selectedTree = null;
				selectedTreeLocation = Vector2.Zero;
			}
		}

		/// <summary>
		/// The menu's draw method
		/// </summary>
		/// <param name="b">SpriteBatch component from the game.</param>
		public override void draw(SpriteBatch b)
		{
			// invoke the base class draw method
			base.draw(b);

			// if we are fading then don't bother drawing
			if (Game1.globalFade)
				return;

			// get cursor tile location
			var tileLocation = new Vector2(
				(Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / Game1.tileSize,
				(Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / Game1.tileSize
			);

			// start drawing in world and not UI
			Game1.StartWorldDrawInUI(b);

			// draw a tree if we have one
			if (selectedTree != null)
			{
				selectedTree.draw(b, tileLocation);

				for (int y = 0; y < 3; y++)
				{
					for (int x = 0; x < 3; x++)
					{
						Vector2 tile = tileLocation;
						tile.X += -1 + x;
						tile.Y += -1 + y;

						// draw the selection box
						b.Draw(
							Game1.mouseCursors,
							Game1.GlobalToLocal(Game1.viewport, tile * Game1.tileSize),
							validSpot[x + y * 3] ? greenSquare : redSquare,
							Color.White,
							0.0f,
							Vector2.Zero,
							Game1.pixelZoom,
							SpriteEffects.None,
							0.999f
						);
					}
				}
			}

			// stop drawing in world
			Game1.EndWorldDrawInUI(b);

			// draw the cancel button
			cancelButton.draw(b);
			// draw the flip button
			flipButton.draw(b);

			// draw the scroll with text
			var t = selectedTree != null ? placementText : defaultText;
			SpriteText.drawStringWithScrollBackground(b, t, Game1.uiViewport.Width / 2 - SpriteText.getWidthOfString(t) / 2, 16);

			// draw the cursor
			drawMouse(b);
		}

		/// <summary>
		/// Called when resizing the game window to reposition the UI elements
		/// </summary>
		void resetBounds()
		{
			cancelButton = new ClickableTextureComponent(
				new Rectangle(
					xPositionOnScreen + Game1.uiViewport.Width - borderWidth - spaceToClearSideBorder - 64,
					yPositionOnScreen + Game1.uiViewport.Height - 128,
					64,
					64
				),
				Game1.mouseCursors,
				Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47, -1, -1),
				1.0f
			); ;

			flipButton = new ClickableTextureComponent(
				new Rectangle(
					xPositionOnScreen + Game1.uiViewport.Width - borderWidth - spaceToClearSideBorder - 144,
					yPositionOnScreen + Game1.uiViewport.Height - 128,
					64,
					64
				),
				TreeTransplant.flipTexture,
				new Rectangle(0, 0, 64, 64),
				1.0f
			);
		}

		/// <summary>
		/// Checks to see if the menu is ready to be closed
		/// </summary>
		/// <returns><c>true</c>, if there's no selected tree, <c>false</c> otherwise.</returns>
		public override bool readyToClose()
		{
			return base.readyToClose() && selectedTree == null;
		}

		/// <summary>
		/// Called on right-clicking the menu.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="playSound">If set to <c>true</c> play sound.</param>
		public override void receiveRightClick(int x, int y, bool playSound = true)
		{
			flipTree();
		}

		/// <summary>
		/// Flips the selected tree.
		/// </summary>
		public void flipTree()
		{
			if (selectedTree != null)
				selectedTree.flipped = !selectedTree.flipped;
		}

		/// <summary>
		/// Called when the game window size is changed.
		/// </summary>
		/// <param name="oldBounds">Old bounds.</param>
		/// <param name="newBounds">New bounds.</param>
		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
		{
			resetBounds();
		}

		/// <summary>
		/// Called when the menu is being hovered with the mouse.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public override void performHoverAction(int x, int y)
		{
			// use the try hover on our UI buttons
			cancelButton.tryHover(x, y);
			flipButton.tryHover(x, y);

			base.performHoverAction(x, y);
		}
	}
}
