using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
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
			// batch together the trees in a render texture for our menu
			loadTreeTexture();
			loadSpecialTreeTexture();

			// load the custom UI element for flipping the tree
			loadFlipTexture();

			TreeTransplant.helper = helper;
			// bind to the after load handler
			helper.Events.Display.MenuChanged += OnMenuChanged;
		}

		/// <summary>
		/// Raised after a game menu is opened, closed, or replaced.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal void OnMenuChanged(object sender, MenuChangedEventArgs e)
		{
			// carpenter dialog in science house?
			if (Game1.currentLocation?.Name == "ScienceHouse" && e.NewMenu is DialogueBox && Game1.currentLocation.lastQuestionKey == "carpenter")
				handleDialogueMenu();
		}

		/// <summary>
		/// Used to override the old carpenter menu with our custom one
		/// </summary>
		internal void handleDialogueMenu()
		{
			// get the current location
			var science = Game1.currentLocation;
			// don't care if this isn't the science house
			if (science.Name != "ScienceHouse")
				return;

			// don't care if we're upgrading
			if (Game1.player.currentUpgrade != null)
				return;

			// create answer choices
			Response[] answerChoices;

			// handle if the house can still be upgraded
			if (Game1.player.HouseUpgradeLevel < 3)
				answerChoices = new[]
				{
					new Response("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop")),
					new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeHouse")),
					new Response("Tree", "Transplant Trees"),
					new Response("Construct", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Construct")),
					new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave"))
				};
			// handle when the house is fully upgraded
			else
				answerChoices = new[]
				{
					new Response("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop")),
					new Response("Tree", "Transplant Trees"),
					new Response("Construct", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Construct")),
					new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave"))
				};

			// set the custom key for our dialogue box
			Game1.currentLocation.lastQuestionKey = "custom_carpenter";

			// create the question dialogue with our custom tag
			science.createQuestionDialogue(
				Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu"),
				answerChoices,
				handleCarpenterMenuAnswer
			);
		}

		/// <summary>
		/// Handles the custom carpenter menu answer
		/// </summary>
		/// <param name="who">Farmer that answered.</param>
		/// <param name="whichAnswer">Which answer key was chosen.</param>
		internal void handleCarpenterMenuAnswer(Farmer who, string whichAnswer)
		{
			switch (whichAnswer)
			{
				case "Shop":
					Game1.player.forceCanMove();
					Game1.activeClickableMenu = new ShopMenu(Utility.getCarpenterStock(), 0, "Robin");
					break;
				case "Upgrade":
					Helper.Reflection.GetMethod(Game1.currentLocation, "houseUpgradeOffer").Invoke();
					break;
				case "Construct":
					Game1.activeClickableMenu = new CarpenterMenu(false);
					break;
				case "Tree":
					Game1.player.forceCanMove();
					Game1.activeClickableMenu = new TreeTransplantMenu();
					break;
				case "Leave":
				default:
					break;
			}
		}

		/// <summary>
		/// Used to load the tree texture by batching together the textures from XNB into a custom render target
		/// </summary>
		internal void loadTreeTexture()
		{
			// the list of seasons
			var seasons = new[] { "spring", "summer", "fall", "winter" };

			// create a render target to prepare the tree texture to
			var texture = new RenderTarget2D(Game1.graphics.GraphicsDevice, 144, 96 * seasons.Length);

			// set the render target and clear the buffer
			Game1.graphics.GraphicsDevice.SetRenderTarget(texture);
			Game1.graphics.GraphicsDevice.Clear(Color.Transparent);

			// begin drawing session
			Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

			for (int s = 0; s < seasons.Length; s++)
			{
				// loop through the three trees in the game
				for (int i = 0; i < 3; i++)
				{
					// get the current season
					string season = seasons[s];

					// spring and summer share the same texture for the pine tree
					if (i == 2 && season.Equals("summer"))
						season = "spring";

					// load the texture into memory
					string treeString = $"TerrainFeatures\\tree{i + 1}_{season}";

					// get the current tree's texture
					Texture2D currentTreeTexture = Game1.content.Load<Texture2D>(treeString);

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
						Tree.treeTopSourceRect,
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
		internal void loadSpecialTreeTexture()
		{
			// create a render target to prepare the tree texture to
			var texture = new RenderTarget2D(Game1.graphics.GraphicsDevice, 96, 96);

			// set the render target and clear the buffer
			Game1.graphics.GraphicsDevice.SetRenderTarget(texture);
			Game1.graphics.GraphicsDevice.Clear(Color.Transparent);

			// begin drawing session
			Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

			// get the special tree's texture
			Texture2D mushroomTreeTexture = Game1.content.Load<Texture2D>("TerrainFeatures\\mushroom_tree");
			Texture2D palmTreeTexture = Game1.content.Load<Texture2D>("TerrainFeatures\\tree_palm");

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
				Tree.treeTopSourceRect,
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
				Tree.treeTopSourceRect,
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
		internal void loadFlipTexture()
		{
			flipTexture = Texture2D.FromStream(
				Game1.graphics.GraphicsDevice,
				new FileStream(Path.Combine(Helper.DirectoryPath, "assets", "flip.png"), FileMode.Open));
		}
	}
}
