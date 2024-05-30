/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#if COMMON_SIMPLELAYOUT

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.Common.UI.SimpleLayout;

public class TextureNode : ISimpleNode {

	public Texture2D? Texture { get; }
	public Rectangle Source { get; }
	public float Scale { get; }
	public bool DrawShadow { get; }

	public Alignment Alignment { get; }

	public bool DeferSize => false;

	public TextureNode(Texture2D? texture, Rectangle? source = null, float scale = 1f, bool shadow = false, Alignment alignment = Alignment.None) {
		Texture = texture;
		Source = source ?? texture?.Bounds ?? Rectangle.Empty;
		Scale = scale;
		DrawShadow = shadow;
		Alignment = alignment;
	}

	public Vector2 GetSize(SpriteFont defaultFont, Vector2 containerSize) {
		if (Texture == null)
			return Vector2.Zero;

		return new Vector2(Source.Width * Scale, Source.Height * Scale);
	}

	public void Draw(SpriteBatch batch, Vector2 position, Vector2 size, Vector2 containerSize, float alpha, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor) {
		if (Texture == null)
			return;

		if (DrawShadow)
			Utility.drawWithShadow(batch, Texture, position, Source, Color.White * alpha, 0f, Vector2.Zero, Scale);
		else
			batch.Draw(Texture, position, Source, Color.White * alpha, 0f, Vector2.Zero, Scale, SpriteEffects.None, GUIHelper.GetLayerDepth(position.Y));
	}
}

#endif
