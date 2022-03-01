/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.Common {
	public static class GUIHelpers {

		public enum Side {
			Up,
			Down,
			Left,
			Right
		}


		public static float GetLayerDepth(float yPos) {
			return yPos / 10000f;
		}


		public static void MoveComponents(Side side, int spacing = -1, params ClickableComponent[] components) {
			if (spacing < 0)
				spacing = IClickableMenu.borderWidth;

			ClickableComponent last = null;

			foreach(ClickableComponent cmp in components) {
				if (cmp == null)
					continue;

				if (last != null) { 
					int x = last.bounds.X, y = last.bounds.Y;

					switch(side) {
						case Side.Up:
							y = y - spacing - cmp.bounds.Height;
							break;
						case Side.Down:
							y = y + last.bounds.Height + spacing;
							break;
						case Side.Left:
							x = x - spacing - cmp.bounds.Width;
							break;
						case Side.Right:
							x = x + last.bounds.Width + spacing;
							break;
						default:
							return;
					}

					if (x != cmp.bounds.X || y != cmp.bounds.Y)
						cmp.bounds = new Rectangle(x, y, cmp.bounds.Width, cmp.bounds.Height);
				}

				last = cmp;
			}
		}

	}
}
