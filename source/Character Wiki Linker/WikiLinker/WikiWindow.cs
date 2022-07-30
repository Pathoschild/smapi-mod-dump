/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JudeRV/SDV_WikiLinker
**
*************************************************/

using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace MenuTesting
{
	public class WikiWindow : IClickableMenu
	{

		const string baseUrl = "https://stardewvalleywiki.com/";

		string hoverText = "";

		readonly static int UIWidth = 980;

		readonly static int UIHeight = 786;

		readonly int UIPosX = (int)(Game1.viewport.Width * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 - (UIWidth / 2);

		readonly int UIPosY = (int)(Game1.viewport.Height * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 - (UIHeight / 2) - 27;

		static Rectangle PortraitSelection = new Rectangle(0, 0, 64, 64);

		/*This is really complicated but I'm not changing it cause I'm proud of it
		 * 1. It gets a list of all the characters' names from NPCDispositions
		 * 2. It removes Marlon, cause he's in there for some reason, idk, you can't give him gifts so fuck em
		 * 3. It checks if you wanna hide villagers you haven't met. If you do, then filter out the ones you haven't met
		 * 4. It sorts the list alphabetically, just so it's consistent and a bit easier to find a character in
		 */
		List<string> VillagerList = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions").Keys
			.Where(x => x != "Marlon")
			.Where(x => !ModEntry.HideUnmetVillagers || Game1.player.friendshipData.ContainsKey(x))
			.OrderBy(x => x)
			.ToList();

		Dictionary<string, ClickableTextureComponent> SociableVillagers = new Dictionary<string, ClickableTextureComponent>();

		readonly int villagersPerRow = 7;

		readonly static float buttonScale = 2f;

		readonly int buttonDimension = (int)(PortraitSelection.Width * buttonScale);

		Rectangle hoverBackground = new Rectangle();

		//Initializes menu
		public WikiWindow()
		{
			initialize(UIPosX, UIPosY, UIWidth, UIHeight, true);

			//Starting X & Y coords for first portrait. This gets modified for each portrait to create the grid seen in the window
			int ctcPosX = UIPosX + 34;
			int ctcPosY = UIPosY + 101;
			foreach (string character in VillagerList)
			{
				CreateCharacterButton(character, ref ctcPosX, ref ctcPosY);
			}
		}

		//Draws everything to the screen
		public override void draw(SpriteBatch b)
		{
			//screen fade
			b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

			// dialogue box
			Game1.drawDialogueBox(UIPosX, UIPosY, UIWidth, UIHeight, false, true);

			if (!hoverBackground.IsEmpty)
			{
				b.Draw(Game1.fadeToBlackRect, hoverBackground, Color.Black * 0.25f);
			}
			//Character Portraits
			foreach (KeyValuePair<string, ClickableTextureComponent> kvp in SociableVillagers)
			{
				kvp.Value.draw(b);
			}

			if (!string.IsNullOrEmpty(hoverText))
			{
				drawHoverText(b, hoverText, Game1.smallFont);
			}

			upperRightCloseButton.draw(b);
			drawMouse(b);
		}

		//Handles left-clicking on items with the cursor
		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			if (upperRightCloseButton.containsPoint(x, y))
			{
				if (playSound) Game1.playSound("bigDeSelect");
				Game1.exitActiveMenu();
				Game1.player.CanMove = true;
			}
			foreach (KeyValuePair<string, ClickableTextureComponent> kvp in SociableVillagers)
			{
				if (kvp.Value.containsPoint(x, y))
				{
					if (playSound) Game1.playSound("bigSelect");
					OpenWikiPage(kvp.Key);
					break;
				}
			}
		}

		//Handles what to do when cursor is hovering over an item
		public override void performHoverAction(int x, int y)
		{
			//Basically just does the hover animation for the close button in the top right
			base.performHoverAction(x, y);

			//Shows what character you're hovering over and displays their name next to your cursor
			hoverBackground = new Rectangle();
			hoverText = "";
			foreach (KeyValuePair<string, ClickableTextureComponent> kvp in SociableVillagers)
			{
				if (kvp.Value.containsPoint(x, y))
				{
					hoverBackground = kvp.Value.bounds;
					hoverText = kvp.Key;
					break;
				}
			}
		}

		//Handles creating the ClickableTextureComponents for each villager
		void CreateCharacterButton(string character, ref int ctcPosX, ref int ctcPosY)
		{
			//Centers the last row
			int howManyLeftIncludingThisOne = VillagerList.Count - VillagerList.IndexOf(character);
			if (howManyLeftIncludingThisOne < villagersPerRow)
			{
				if (VillagerList.IndexOf(character) % villagersPerRow == 0 || VillagerList.IndexOf(character) == 0)
				{
					ctcPosX = UIPosX + (UIWidth / 2) - (int)(buttonDimension * (howManyLeftIncludingThisOne / 2f));
				}
			}

			//Creates the actual buttons for all the characters. Uses position variables, then updates them for next button
			try
			{
				ClickableTextureComponent characterButton = new ClickableTextureComponent(
					new Rectangle(ctcPosX, ctcPosY, buttonDimension, buttonDimension), //bounds for clickable button part
					Game1.content.Load<Texture2D>($"Portraits/{(character == "Leo" ? "ParrotBoy" : character)}"), //File from which to get portrait
					PortraitSelection, //Section of portrait file to use
					buttonScale); //Scale of portrait when drawing it
				SociableVillagers.Add(character, characterButton);
				ctcPosX += (int)(characterButton.sourceRect.Width * characterButton.scale);
				if (ctcPosX >= UIPosX + UIWidth - ((int)(characterButton.sourceRect.Width * characterButton.scale) + 5))
				{
					ctcPosY += (int)(characterButton.sourceRect.Height * characterButton.scale) + 2;
					ctcPosX = UIPosX + 34;
				}
			}
			catch
			{
				Game1.chatBox.addErrorMessage($"Failed to add {character} to list");
			}
		}

		//Opens a new tab in your default browser with the url to the wiki page of the character you clicked on
		void OpenWikiPage(string name)
		{
			Process.Start(new ProcessStartInfo()
			{
				UseShellExecute = true,
				FileName = $"{baseUrl}{name}"
			});
		}
	}
}
