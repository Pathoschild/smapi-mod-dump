/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/siweipancc/TreeTransplant
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;

namespace TreeTransplant
{
	/// <summary>
	/// The mod entry class called by SMAPI.
	/// </summary>
	public class TreeTransplant : Mod
	{
		public static Texture2D treeTexture;
		public static Texture2D specialTreeTexture;
		public static Texture2D flipTexture;
		public static IModHelper helper;

		/// <summary>
		/// The mod entry point, called after the mod is first loaded.
		/// </summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			TreeTransplant.helper = helper;

			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.Display.MenuChanged += OnMenuChanged;
		}

		/// <summary>
		/// Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
		{
			// batch together the trees in a render texture for our menu
			loadTreeTexture();
			loadSpecialTreeTexture();

			// load the custom UI element for flipping the tree
			loadFlipTexture();
		}

		/// <summary>
		/// Raised after a game menu is opened, closed, or replaced.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
		{
			// carpenter dialog in science house?
			if (Game1.currentLocation?.Name == "ScienceHouse" && e.NewMenu is DialogueBox && Game1.currentLocation.lastQuestionKey == "carpenter" && Game1.IsMasterGame)
				handleDialogueMenu();
		}

		/// <summary>
		/// Used to override the old carpenter menu with our custom one
		/// </summary>
		private void handleDialogueMenu()
		{
			// get the current location
			var science = Game1.currentLocation;
			// don't care if this isn't the science house
			if (science.Name != "ScienceHouse")
				return;

			// don't care if we're upgrading
			if (Game1.getFarm().buildings.Any(x => x.isUnderConstruction()))
				return;

			// create answer choices
			var options = new List<Response>() { new Response("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop")) };

			// handle if the house can still be upgraded
			if (Game1.IsMasterGame)
			{
				if (Game1.player.HouseUpgradeLevel < 3)
					options.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeHouse")));
				else if ((Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") || Game1.MasterPlayer.mailReceived.Contains("JojaMember") || Game1.MasterPlayer.hasCompletedCommunityCenter()) && (Game1.getLocationFromName("Town") as Town).daysUntilCommunityUpgrade.Value <= 0)
				{
					if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade") || !Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
						options.Add(new Response("CommunityUpgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
				}
			}
			else if (Game1.player.HouseUpgradeLevel < 3)
				options.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeCabin")));

			if (Game1.player.HouseUpgradeLevel >= 2)
			{
				if (Game1.IsMasterGame)
					options.Add(new Response("Renovate", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateHouse")));
				else
					options.Add(new Response("Renovate", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateCabin")));
			}

			options.Add(new Response("Tree", helper.Translation.Get("Carpenter_Option")));
			options.Add(new Response("Construct", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Construct")));
			options.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave")));
		
			// set the custom key for our dialogue box
			Game1.currentLocation.lastQuestionKey = "custom_carpenter";

			// create the question dialogue with our custom tag
			science.createQuestionDialogue(
				Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu"),
				options.ToArray(),
				handleCarpenterMenuAnswer
			);
		}

		/// <summary>
		/// Handles the custom carpenter menu answer
		/// </summary>
		/// <param name="who">Farmer that answered.</param>
		/// <param name="whichAnswer">Which answer key was chosen.</param>
		private void handleCarpenterMenuAnswer(Farmer who, string whichAnswer)
		{
			switch (whichAnswer)
			{
				case "Shop":
					Game1.player.forceCanMove();
					Utility.TryOpenShopMenu(Game1.shop_carpenter, Game1.builder_robin);
					break;
				case "Upgrade":
					Helper.Reflection.GetMethod(Game1.currentLocation, "houseUpgradeOffer").Invoke();
					break;
				case "CommunityUpgrade":
					Helper.Reflection.GetMethod(Game1.currentLocation, "communityUpgradeOffer").Invoke();
					break;
				case "Construct":
					Game1.activeClickableMenu = new CarpenterMenu(Game1.builder_robin,null);
					break;
				case "Renovate":
					Game1.player.forceCanMove();
					HouseRenovation.ShowRenovationMenu();
					break;
				case "Tree":
					Game1.activeClickableMenu = new TreeTransplantMenu();
					break;
				// ReSharper disable once RedundantCaseLabel
				case "Leave":
				default:
					break;
			}
		}

		/// <summary>
		/// Used to load the tree texture by batching together the textures from XNB into a custom render target
		/// </summary>
		private void loadTreeTexture()
		{
			// the list of seasons
			var seasons = new[] { "spring", "summer", "fall", "winter" };
			var trees = new[] { Tree.bushyTree, Tree.leafyTree, Tree.pineTree, Tree.mahoganyTree };

			// create a render target to prepare the tree texture to
			var texture = new RenderTarget2D(Game1.graphics.GraphicsDevice, 48 * trees.Length, 96 * seasons.Length);

			// set the render target and clear the buffer
			Game1.graphics.GraphicsDevice.SetRenderTarget(texture);
			Game1.graphics.GraphicsDevice.Clear(Color.Transparent);

			// begin drawing session
			Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

			// Get source rectangle for tree tops.
			var treeTopSourceRect =Tree.treeTopSourceRect;

			for (var s = 0; s < seasons.Length; s++)
			{
				// loop through the three trees in the game
				for (var i = 0; i < trees.Length; i++)
				{
					// get the current season
					var season = seasons[s];

					// spring and summer share the same texture for the pine tree
					if (trees[i] == Tree.pineTree && season.Equals("summer"))
						season = "spring";

					// load the texture into memory
					var treeString = $"TerrainFeatures\\tree{trees[i]}_{season}";

					// get the current tree's texture
					var currentTreeTexture = Game1.content.Load<Texture2D>(treeString);

					// draw the trunk of the tree
					Game1.spriteBatch.Draw(
						currentTreeTexture,
						new Vector2((48 * i) + 16, (96 * (s + 1)) - 32),
						Tree.stumpSourceRect,
						Color.White);

					// draw the top of the tree
					Game1.spriteBatch.Draw(
						currentTreeTexture,
						new Vector2(48 * i, 96 * s),
						treeTopSourceRect,
						Color.White);
				}
			}
			Game1.spriteBatch.End();

			// reset the render target back to the back buffer
			Game1.graphics.GraphicsDevice.SetRenderTarget(null);

			// create memory stream to save texture as PNG
			var stream = new MemoryStream();
			texture.SaveAsPng(stream, texture.Width, texture.Height);

			// return our tree texture
			treeTexture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream);
		}

		/// <summary>
		/// Used to load the special tree texture by batching together the textures from XNB into a custom render target
		/// </summary>
		private void loadSpecialTreeTexture()
		{
			// create a render target to prepare the tree texture to
			var texture = new RenderTarget2D(Game1.graphics.GraphicsDevice, 96, 96);

			// set the render target and clear the buffer
			Game1.graphics.GraphicsDevice.SetRenderTarget(texture);
			Game1.graphics.GraphicsDevice.Clear(Color.Transparent);

			// begin drawing session
			Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

			// get the special tree's texture
			var mushroomTreeTexture = Game1.content.Load<Texture2D>("TerrainFeatures\\mushroom_tree");
			var palmTreeTexture = Game1.content.Load<Texture2D>("TerrainFeatures\\tree_palm");

			// Get source rectangle for tree tops.
			var treeTopSourceRect = Tree.treeTopSourceRect;

			// draw the trunk of the tree
			Game1.spriteBatch.Draw(
				palmTreeTexture,
				new Vector2(16, 64),
				Tree.stumpSourceRect,
				Color.White);

			// draw the top of the tree
			Game1.spriteBatch.Draw(
				palmTreeTexture,
				new Vector2(0, 0),
				treeTopSourceRect,
				Color.White);

			// draw the trunk of the tree
			Game1.spriteBatch.Draw(
				mushroomTreeTexture,
				new Vector2(64, 64),
				Tree.stumpSourceRect,
				Color.White);

			// draw the top of the tree
			Game1.spriteBatch.Draw(
				mushroomTreeTexture,
				new Vector2(48, 0),
				treeTopSourceRect,
				Color.White);

			Game1.spriteBatch.End();

			// reset the render target back to the back buffer
			Game1.graphics.GraphicsDevice.SetRenderTarget(null);

			// create memory stream to save texture as PNG
			var stream = new MemoryStream();
			texture.SaveAsPng(stream, texture.Width, texture.Height);

			// return our tree texture
			specialTreeTexture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream);
		}

		/// <summary>
		/// Used to load the texture for the flip tree UI element
		/// </summary>
		private void loadFlipTexture()
		{
			IModContentHelper modContentHelper = Helper.ModContent;
			flipTexture = modContentHelper.Load<Texture2D>("/assets/flip.png");
		}
	}
}
