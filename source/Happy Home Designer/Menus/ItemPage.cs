/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using HappyHomeDesigner.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Collections.Generic;

namespace HappyHomeDesigner.Menus
{
	internal class ItemPage : ScreenPage
	{
		private readonly List<ItemEntry> entries = new();
		private readonly HashSet<string> knownIDs = new();
		private readonly GridPanel Panel = new(80, 80, true);
		private readonly ClickableTextureComponent TrashSlot
			= new(new(0, 0, 48, 48), Catalog.MenuTexture, new(32, 48, 16, 16), 3f, true);

		public ItemPage(IEnumerable<ISalable> source)
		{
			foreach(var item in source)
			{
				if (item is Furniture or Wallpaper or null || item is not Item obj || item.HasTypeBigCraftable())
					continue;

				if (!knownIDs.Add(item.QualifiedItemId))
					continue;

				entries.Add(new(obj));
			}

			Panel.Items = entries;
		}

		public override int Count() 
			=> entries.Count;

		public override ClickableTextureComponent GetTab() 
			=> new(new(0, 0, 64, 64), Catalog.MenuTexture, new(64, 48, 16, 16), 4f);

		public override void draw(SpriteBatch b)
		{
			Panel.DrawShadow(b);
			Panel.draw(b);
			TrashSlot.draw(b);
		}

		public override void performHoverAction(int x, int y)
		{
			base.performHoverAction(x, y);
			Panel.performHoverAction(x, y);
		}

		public override void Resize(Rectangle region)
		{
			base.Resize(region);

			Panel.Resize(width - 36, height - 64, xPositionOnScreen + 55, yPositionOnScreen);
			TrashSlot.setPosition(
				Panel.xPositionOnScreen + Panel.width - 48 + GridPanel.BORDER_WIDTH,
				Panel.yPositionOnScreen + Panel.height + GridPanel.BORDER_WIDTH + GridPanel.MARGIN_BOTTOM
			);
		}

		public override void receiveScrollWheelAction(int direction)
		{
			Panel.receiveScrollWheelAction(direction);
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			if (Panel.TrySelect(x, y, out int index))
			{
				var item = Panel.Items[index] as ItemEntry;

				if (Game1.player.addItemToInventoryBool(item.item.getOne()) && playSound)
					Game1.playSound("pickUpItem");

				return;
			}

			if (TrashSlot.containsPoint(x, y) && Game1.player.ActiveObject.CanDelete(knownIDs))
			{
				if (Game1.player.ActiveObject == Game1.player.TemporaryItem)
					Game1.player.TemporaryItem = null;
				else
					Game1.player.removeItemFromInventory(Game1.player.ActiveObject);

				if (playSound)
					Game1.playSound("trashcan");
			}
		}

		public override bool isWithinBounds(int x, int y)
		{
			return 
				base.isWithinBounds(x, y) ||
				Panel.isWithinBounds(x, y) || 
				TrashSlot.containsPoint(x, y);
		}
	}
}
