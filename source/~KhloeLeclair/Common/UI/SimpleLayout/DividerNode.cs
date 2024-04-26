/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.Common.UI.SimpleLayout;

public class DividerNode : ISimpleNode {

	internal static Texture2D? DefaultTexture;
	internal static Rectangle? HorizontalSource;
	internal static Rectangle? VerticalSource;
	internal static float? Scale;

	public LayoutNode Parent { get; }

	public Alignment Alignment => Alignment.Top;

	public bool DeferSize => false;

	public Texture2D? Source { get; }
	public Rectangle? SourceRectVert { get; }
	public Rectangle? SourceRectHoriz { get; }

	public bool Thick { get; }
	public Color? ThinColor { get; }


	public DividerNode(LayoutNode parent, bool isThick, Color? thinColor = null, Texture2D? source = null, Rectangle? sourceRectVert = null, Rectangle? sourceRectHoriz = null) {
		Thick = isThick;
		ThinColor = thinColor;
		Parent = parent;
		Source = source;
		SourceRectHoriz = sourceRectHoriz;
		SourceRectVert = sourceRectVert;
	}

	public Vector2 GetSize(SpriteFont defaultFont, Vector2 containerSize) {
		int thickness = Thick ? 16 : 8;
		return Parent.Direction switch {
			LayoutDirection.Horizontal => new Vector2(16, 0),
			_ => new Vector2(0, Thick ? 16 : 8)
		};
	}

	public void Draw(SpriteBatch batch, Vector2 position, Vector2 size, Vector2 containerSize, float alpha, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor) {
		if (Parent.Direction == LayoutDirection.Horizontal)
			DrawVertical(batch, position, containerSize.Y, alpha);
		else
			DrawHorizontal(batch, position, containerSize.X, alpha);
	}

	private void DrawVertical(SpriteBatch batch, Vector2 position, float height, float alpha) {
		if (Thick)
			batch.Draw(
				Source ?? DefaultTexture ?? Game1.menuTexture,
				new Rectangle(
					(int) position.X - 24,
					(int) position.Y - 8,
					64,
					(int) height + 16
				),
				SourceRectVert ?? VerticalSource ?? Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 26),
				Color.White * alpha
			);

		else
			batch.Draw(
				Game1.staminaRect,
				new Rectangle(
					(int) position.X + 7,
					(int) position.Y,
					2,
					(int) height
				),
				(ThinColor ?? (Color.Black * 0.4f)) * alpha
			);
	}

	private void DrawHorizontal(SpriteBatch batch, Vector2 position, float width, float alpha) {
		if (Thick)
			batch.Draw(
				Source ?? DefaultTexture ?? Game1.menuTexture,
				new Rectangle(
					(int) position.X - 8,
					(int) position.Y - 24,
					(int) width + 16,
					64
				),
				SourceRectHoriz ?? HorizontalSource ?? Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 25),
				Color.White * alpha
			);

		else
			batch.Draw(
				Game1.staminaRect,
				new Rectangle(
					(int) position.X,
					(int) position.Y + 3,
					(int) width,
					2
				),
				(ThinColor ?? (Color.Black * 0.4f)) * alpha
			);
	}
}
