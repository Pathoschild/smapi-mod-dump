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

using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.Common.UI.SimpleLayout;


public enum LayoutDirection {
	Vertical,
	Horizontal
}


public interface ILayoutNode : ISimpleNode {

	ISimpleNode[]? Children { get; set; }

	Vector2 MinSize { get; set; }

	int Margin { get; set; }

	LayoutDirection Direction { get; }

}


public class LayoutNode : ILayoutNode {

#if DEBUG
	private static readonly Color[] DEBUG_COLORS = [
		Color.Pink,
		Color.Blue,
		Color.Red,
		Color.Purple,
		Color.Gray,
		Color.Gold,
		Color.Fuchsia,
		Color.Orange
	];
#endif

	private ISimpleNode[]? _Children;
	private Vector2[]? Sizes;
	private int _Margin;

	private Vector2 _MinSize = Vector2.Zero;

	public Alignment Alignment { get; set; } = Alignment.None;

	public ISimpleNode[]? Children {
		get => _Children;
		set {
			_Children = value;
			Sizes = null;
		}
	}

	public Vector2 MinSize {
		get => _MinSize;
		set {
			_MinSize = value;
			Sizes = null;
		}
	}

	public int Margin {
		get => _Margin;
		set {
			_Margin = value;
			Sizes = null;
		}
	}

	public LayoutDirection Direction { get; }

	public bool DeferSize => _Children != null && _Children.Any(val => val.DeferSize);

	public LayoutNode(LayoutDirection direction, ISimpleNode[]? children, int margin = 0, Vector2? minSize = null, Alignment alignment = Alignment.None) {
		Direction = direction;
		Children = children;
		Margin = margin;
		MinSize = minSize ?? Vector2.Zero;
		Alignment = alignment;
	}

	public Vector2 GetSize(SpriteFont defaultFont, Vector2 containerSize) {
		float width = 0;
		float height = 0;

		switch (Direction) {
			case LayoutDirection.Horizontal:
				height = _MinSize.Y;
				break;
			case LayoutDirection.Vertical:
			default:
				width = _MinSize.X;
				break;
		}

		Vector2 size = new(width, height);

		bool initial = true;
		int count = _Children?.Length ?? 0;
		Sizes = new Vector2[count];
		bool had_deferred = false;
		bool deferred = false;

		while (_Children != null) {
			for (int i = 0; i < count; i++) {
				ISimpleNode node = _Children[i];
				if (node == null)
					continue;

				if (!deferred && node.DeferSize) {
					had_deferred = true;
					continue;
				} else if (deferred && !node.DeferSize)
					continue;

				Vector2 nsize = Sizes[i] = node.GetSize(defaultFont, size);

				float nsX = (float) Math.Ceiling(nsize.X);
				float nsY = (float) Math.Ceiling(nsize.Y);

				switch (Direction) {
					case LayoutDirection.Horizontal:
						// Add widths, take maximum height.
						if (nsX > 0) {
							if (initial)
								initial = false;
							else
								width += Margin;
							width += nsX;
							size.X = width;
						}

						if (nsY > height)
							height = size.Y = nsY;

						break;

					case LayoutDirection.Vertical:
					default:
						// Add heights, take maximum width.
						if (nsY > 0) {
							if (initial)
								initial = false;
							else
								height += Margin;

							height += nsY;
							size.Y = height;
						}

						if (nsX > width)
							width = size.X = nsX;

						break;
				}
			}

			if (deferred || !had_deferred)
				break;

			deferred = true;
		}

		if (size.X < _MinSize.X)
			size.X = _MinSize.X;

		if (size.Y < _MinSize.Y)
			size.Y = _MinSize.Y;

		return size;
	}

	public void Draw(SpriteBatch batch, Vector2 position, Vector2 ownSize, Vector2 containerSize, float alpha, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor) {
#if DEBUG
		int d_idx = 0;
		bool draw_debug = Game1.oldKBState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F3);
#endif

		if (_Children == null || Sizes == null)
			return;

		// Draw our children.
		bool initial = true;
		int count = _Children.Length;

		float x = position.X;
		float y = position.Y;

		float extra = Direction switch {
			LayoutDirection.Horizontal => containerSize.X - ownSize.X,
			_ => containerSize.Y - ownSize.Y,
		};

		float perlayout = 0f;

		if (extra > 0) {
			int spaces = 0;
			int layouts = 0;
			foreach (ISimpleNode node in _Children) {
				if (node is SpaceNode space && space.Expand)
					spaces++;
				else if (node is LayoutNode)
					layouts++;
			}

			if (spaces > 0) {
				float perspace = (float) Math.Floor(extra / spaces);
				for (int i = 0; i < count; i++) {
					ISimpleNode node = _Children[i];
					if (node is SpaceNode space && space.Expand) {
						switch (Direction) {
							case LayoutDirection.Horizontal:
								Sizes[i].X = perspace + (float) Math.Ceiling(space.Size);
								break;
							case LayoutDirection.Vertical:
							default:
								Sizes[i].Y = perspace + (float) Math.Ceiling(space.Size);
								break;
						}
					}
				}
			} else if (layouts > 0 && Direction == LayoutDirection.Horizontal)
				perlayout = (float) Math.Floor(extra / layouts);
		}

		for (int i = 0; i < count; i++) {
			ISimpleNode node = _Children[i];
			if (node == null)
				continue;

			Vector2 size = Sizes[i];
			if (node is LayoutNode && perlayout > 0)
				size = Direction switch {
					LayoutDirection.Horizontal => new(size.X + perlayout, size.Y),
					_ => new(size.X, size.Y + perlayout)
				};

			if (_Margin != 0)
				switch (Direction) {
					// Add margin to X axis.
					case LayoutDirection.Horizontal:
						if (size.X > 0) {
							if (initial)
								initial = false;
							else
								x += _Margin;
						}
						break;

					// Add margin to Y axis.
					case LayoutDirection.Vertical:
					default:
						if (size.Y > 0) {
							if (initial)
								initial = false;
							else
								y += _Margin;
						}
						break;
				}

			float offsetX = 0;
			float offsetY = 0;

			Alignment align = node.Alignment;
			if (align == Alignment.None && Direction == LayoutDirection.Horizontal)
				align = Alignment.VCenter;

			if (align.HasFlag(Alignment.Left)) {
				/* nothing ~ */
			} else if (align.HasFlag(Alignment.HCenter))
				offsetX += (float) Math.Ceiling((ownSize.X - size.X) / 2);
			else if (align.HasFlag(Alignment.Right))
				offsetX += (float) Math.Ceiling(ownSize.X - size.X);

			if (align.HasFlag(Alignment.Top)) {
				/* nothing ~ */
			} else if (align.HasFlag(Alignment.VCenter))
				offsetY += (float) Math.Ceiling((ownSize.Y - size.Y) / 2);
			else if (align.HasFlag(Alignment.Bottom))
				offsetY += (float) Math.Ceiling(ownSize.Y - size.Y);

			node.Draw(
				batch,
				new Vector2(x + offsetX, y + offsetY),
				size,
				ownSize,
				alpha,
				defaultFont,
				defaultColor,
				defaultShadowColor
			);

			// Debugging
#if DEBUG
			if (draw_debug) {
				batch.Draw(
					Game1.uncoloredMenuTexture,
					new Rectangle((int) (x + offsetX), (int) (y + offsetY), (int) (size.X == 0 ? ownSize.X : size.X), (int) (size.Y == 0 ? ownSize.Y : size.Y)),
					new Rectangle(16, 272, 28, 28),
					DEBUG_COLORS[d_idx] * 0.5f
				);

				d_idx = (d_idx + 1) % DEBUG_COLORS.Length;
			}
#endif

			// Move X and Y.
			switch (Direction) {
				// Add horizontal size.
				case LayoutDirection.Horizontal:
					x += (float) Math.Ceiling(size.X);
					break;

				// Add vertical size.
				case LayoutDirection.Vertical:
				default:
					y += (float) Math.Ceiling(size.Y);
					break;
			}
		}
	}
}

#endif
