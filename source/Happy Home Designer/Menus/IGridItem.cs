/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using System;

namespace HappyHomeDesigner.Menus
{
	public interface IGridItem
	{
		public void Draw(SpriteBatch batch, int x, int y);

		public bool ToggleFavorite(bool playSound);

		public string GetName();
	}
}
