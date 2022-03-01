/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Leclair.Stardew.Common.UI.SimpleLayout {
	public interface ISimpleNode {

		// Alignment
		Alignment Alignment { get; }

		// Size
		bool DeferSize { get; }
		Vector2 GetSize(SpriteFont defaultFont, Vector2 containerSize);

		// Rendering
		void Draw(SpriteBatch batch, Vector2 position, Vector2 size, Vector2 containerSize, float alpha, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor);

	}
}
