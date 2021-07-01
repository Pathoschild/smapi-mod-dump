/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/prismaticpride
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;

namespace PrismaticPride
{
	public class ColorSetMenu : IClickableMenu
	{
		internal protected static IModHelper Helper => ModEntry.Instance.Helper;
		internal protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		internal protected static ColorData ColorData => ModEntry.Instance.colorData;

		public const int BackButtonID = 101;
		public const int ForwardButtonID = 102;
		public const int OkButtonID = 103;
		public const int CancelButtonID = 104;

		public const int Width = 640;
		public const int Height = 192;

		public ClickableTextureComponent backButton;
		public ClickableTextureComponent forwardButton;
		public ClickableTextureComponent okButton;
		public ClickableTextureComponent cancelButton;

		private string selectedKey;

		public ColorSetMenu ()
			: base (Game1.uiViewport.Width / 2 - 320,
				Game1.uiViewport.Height - 64 - Height, Width, Height)
		{
			backButton = new ClickableTextureComponent (
				new Rectangle (xPositionOnScreen - 128 - 4, yPositionOnScreen + 85, 48, 44),
				Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
			{
				myID = BackButtonID,
				rightNeighborID = ForwardButtonID
			};
			forwardButton = new ClickableTextureComponent (
				new Rectangle (xPositionOnScreen + Width + 16 + 64, yPositionOnScreen + 85, 48, 44),
				Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
			{
				myID = ForwardButtonID,
				leftNeighborID = BackButtonID,
				rightNeighborID = OkButtonID
			};
			okButton = new ClickableTextureComponent ("OK",
				new Rectangle(xPositionOnScreen + width + 128 + 8, yPositionOnScreen + Height - 128, 64, 64),
				null, null, Game1.mouseCursors, new Rectangle (175, 379, 16, 15), 4f)
			{
				myID = OkButtonID,
				leftNeighborID = ForwardButtonID,
				rightNeighborID = CancelButtonID
			};
			cancelButton = new ClickableTextureComponent ("Cancel",
				new Rectangle (xPositionOnScreen + width + 192 + 12, yPositionOnScreen + Height - 128, 64, 64),
				null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet (Game1.mouseCursors, 47), 1f)
			{
				myID = CancelButtonID,
				leftNeighborID = OkButtonID
			};

			Game1.playSound ("bigSelect");
			selectedKey = ColorData.currentSet?.key;

			if (Game1.options.SnappyMenus)
			{
				populateClickableComponentList ();
				snapToDefaultClickableComponent ();
			}
		}

		public override void snapToDefaultClickableComponent ()
		{
			currentlySnappedComponent = getComponentWithID (OkButtonID);
			snapCursorToCurrentSnappedComponent ();
		}

		public override void gameWindowSizeChanged (Rectangle oldBounds,
			Rectangle newBounds)
		{
			base.gameWindowSizeChanged (oldBounds, newBounds);
			xPositionOnScreen = Game1.uiViewport.Width / 2 - 320;
			yPositionOnScreen = Game1.uiViewport.Height - 64 - Height;
			backButton = new ClickableTextureComponent (new Rectangle (
				xPositionOnScreen - 128 - 4, yPositionOnScreen + 85, 48, 44),
				Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
			forwardButton = new ClickableTextureComponent (new Rectangle (
				xPositionOnScreen + Width + 16 + 64, yPositionOnScreen + 85, 48, 44),
				Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
			okButton = new ClickableTextureComponent ("OK", new Rectangle (
				xPositionOnScreen + width + 128 + 8, yPositionOnScreen + Height - 128, 64, 64),
				null, null, Game1.mouseCursors, new Rectangle(175, 379, 16, 15), 4f);
			cancelButton = new ClickableTextureComponent ("Cancel", new Rectangle (
				xPositionOnScreen + width + 192 + 12, yPositionOnScreen + Height - 128, 64, 64),
				null, null, Game1.mouseCursors,
				Game1.getSourceRectForStandardTileSheet (Game1.mouseCursors, 47), 1f);
		}

		public override void performHoverAction (int x, int y)
		{
			base.performHoverAction (x, y);
			okButton.tryHover (x, y);
			cancelButton.tryHover (x, y);
			backButton.tryHover (x, y);
			forwardButton.tryHover (x, y);
		}

		public override void receiveLeftClick (int x, int y,
			bool playSound = true)
		{
			base.receiveLeftClick (x, y, playSound);

			if (okButton.containsPoint (x, y))
			{
				ColorData.currentSet = ColorData.sets[selectedKey];
				Game1.playSound ("select");
				return;
			}

			if (cancelButton.containsPoint (x, y))
			{
				exitThisMenu ();
				return;
			}

			List<string> keys = ColorData.sets.Keys.ToList ();
			int index = keys.IndexOf (selectedKey);

			if (backButton.containsPoint (x, y))
			{
				--index;
				if (index < 0)
					index = keys.Count - 1;
				selectedKey = keys[index];
				Game1.playSound ("shwip");
				backButton.scale = backButton.baseScale - 1f;
				return;
			}

			if (forwardButton.containsPoint (x, y))
			{
				++index;
				index %= keys.Count;
				selectedKey = keys[index];
				Game1.playSound ("shwip");
				forwardButton.scale = forwardButton.baseScale - 1f;
				return;
			}
		}

		public override void draw (SpriteBatch b)
		{
			base.draw (b);

			string sampleText = "Summer (The Sun Can Bend An Orange Sky)";
			int textWidth = (int) Game1.dialogueFont.MeasureString (sampleText).X;
			IClickableMenu.drawTextureBox (b,
				xPositionOnScreen + width / 2 - textWidth / 2 - 16,
				yPositionOnScreen + 64 - 4, textWidth + 32, 80, Color.White);

			if (ColorData.sets.ContainsKey (selectedKey))
			{
				string displayName = ColorData.sets[selectedKey].displayName;
				Utility.drawTextWithShadow (b, displayName, Game1.dialogueFont,
					new Vector2 ((float) (xPositionOnScreen + width / 2) -
						Game1.dialogueFont.MeasureString (displayName).X / 2f,
						yPositionOnScreen + height / 2 - 16),
					Game1.textColor);
			}

			okButton.draw (b);
			cancelButton.draw (b);
			forwardButton.draw (b);
			backButton.draw (b);

			SpriteText.drawStringWithScrollCenteredAt (b,
				Helper.Translation.Get ("ColorSetMenu.title"),
				xPositionOnScreen + width / 2, yPositionOnScreen - 32);

			drawMouse (b);
		}
	}
}
