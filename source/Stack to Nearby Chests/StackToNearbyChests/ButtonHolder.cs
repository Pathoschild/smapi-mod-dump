using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace StackToNearbyChests
{
	class ButtonHolder
	{
		private const int buttonID = 1070000;//A random number

		internal static Texture2D ButtonIcon { private get; set; }

		private static ClickableTextureComponent button;
		private static InventoryPage inventoryPage;
		private static bool drawHoverText = false;

		//When InventoryPage constructed, create a new button
		public static void Constructor(InventoryPage inventoryPage, int x, int y, int width, int height)
		{
			ButtonHolder.inventoryPage = inventoryPage;

			button = new ClickableTextureComponent("", 
				new Rectangle(inventoryPage.xPositionOnScreen + width, inventoryPage.yPositionOnScreen + height / 3 - 64 + 8 + 80, 64, 64), 
				"",
				"Stack to nearby chests", 
				ButtonIcon, 
				Rectangle.Empty, 
				4f,
				false)
			{
				myID = buttonID,
				downNeighborID = 105,
				leftNeighborID = 11,
				upNeighborID = 106
			};

			inventoryPage.organizeButton.downNeighborID = buttonID;
			inventoryPage.trashCan.upNeighborID = buttonID;
		}
		
		public static void ReceiveLeftClick(int x, int y)
		{
			if (button != null && button.containsPoint(x, y))
				StackLogic.StackToNearbyChests(ModEntry.Config.Radius);
		}

		public static void PerformHoverAction(int x, int y)
		{
			button.tryHover(x, y);
			drawHoverText = button.containsPoint(x, y);
		}

		public static void PopulateClickableComponentsList(InventoryPage inventoryPage)
		{
			inventoryPage.allClickableComponents.Add(button);
		}

		//Run before drawing hover texts. Use for drawing the button.
		public static void TrashCanDrawn(ClickableTextureComponent textureComponent, SpriteBatch spriteBatch)
		{
			if (inventoryPage != null && inventoryPage.trashCan == textureComponent)
				//Trash can was just drawn on the InventoryPage
				button?.draw(spriteBatch);
		}


		//This is run after drawing everything else in InventoryPage. Use for drawing hover text (on top of everything)
		public static void PostDraw(SpriteBatch spriteBatch)
		{
			if (drawHoverText)
				IClickableMenu.drawToolTip(spriteBatch, button.hoverText, "", null, false, -1, 0, -1, -1, null, -1);
		}
	}
}
