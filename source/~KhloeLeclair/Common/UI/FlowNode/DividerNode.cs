/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#if COMMON_FLOW

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.Common.UI.FlowNode;

public struct DividerNode : IFlowNode {

	public Color? Color { get; }
	public Color? ShadowColor { get; }
	public float Size { get; }
	public float Padding { get; }
	public float ShadowOffset { get; }

	public Alignment Alignment => Alignment.None;
	public object? Extra { get; }
	public string? UniqueId { get; }

	public Func<IFlowNodeSlice, int, int, bool>? OnClick => null;
	public Func<IFlowNodeSlice, int, int, bool>? OnHover => null;
	public Func<IFlowNodeSlice, int, int, bool>? OnRightClick => null;

	public DividerNode(
		Color? color = null,
		Color? shadowColor = null,
		float size = 4f,
		float padding = 14f,
		float shadowOffset = 2f,
		object? extra = null,
		string? id = null
	) {
		Color = color;
		ShadowColor = shadowColor;
		Size = size;
		Padding = padding < 0 ? 0f : padding;
		ShadowOffset = shadowOffset;
		Extra = extra;
		UniqueId = id;
	}

	public bool? WantComponent(IFlowNodeSlice slice) {
		return false;
	}

	public ClickableComponent? UseComponent(IFlowNodeSlice slice) {
		return null;
	}

	public bool IsEmpty() {
		return Size <= 0;
	}

	public IFlowNodeSlice? Slice(IFlowNodeSlice? last, SpriteFont font, float maxWidth, float remaining, IFlowNodeSlice? nextSlice) {
		if (last != null)
			return null;

		return new UnslicedNode(
			this,
			maxWidth,
			Size + Padding + Padding,
			WrapMode.None
		);
	}

	public void Draw(IFlowNodeSlice slice, SpriteBatch batch, Vector2 position, float scale, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor, CachedFlowLine line, CachedFlow flow) {
		if (IsEmpty())
			return;

		int shadowOffset = (int) ShadowOffset;
		int x = (int) position.X;
		int y = (int) position.Y + (int) Padding;

		if (shadowOffset != 0)
			batch.Draw(
				Game1.uncoloredMenuTexture,
				new Rectangle(
					x - shadowOffset, y + shadowOffset,
					(int) slice.Width, (int) Size
				),
				new Rectangle(16, 272, 28, 28),
				ShadowColor ?? defaultShadowColor ?? Game1.textShadowColor
			);

		batch.Draw(
			Game1.uncoloredMenuTexture,
			new Rectangle(
				x, y,
				(int) slice.Width, (int) Size
			),
			new Rectangle(16, 272, 28, 28),
			Color ?? defaultColor ?? Game1.textColor
		);
	}

	public override bool Equals(object? obj) {
		return obj is DividerNode node &&
			   EqualityComparer<Color?>.Default.Equals(Color, node.Color) &&
			   EqualityComparer<Color?>.Default.Equals(ShadowColor, node.ShadowColor) &&
			   Size == node.Size &&
			   Padding == node.Padding &&
			   ShadowOffset == node.ShadowOffset &&
			   Alignment == node.Alignment &&
			   EqualityComparer<object>.Default.Equals(Extra, node.Extra) &&
			   UniqueId == node.UniqueId &&
			   EqualityComparer<Func<IFlowNodeSlice, int, int, bool>>.Default.Equals(OnClick, node.OnClick) &&
			   EqualityComparer<Func<IFlowNodeSlice, int, int, bool>>.Default.Equals(OnHover, node.OnHover) &&
			   EqualityComparer<Func<IFlowNodeSlice, int, int, bool>>.Default.Equals(OnRightClick, node.OnRightClick);
	}

	public override int GetHashCode() {
		HashCode hash = new();
		hash.Add(Color);
		hash.Add(ShadowColor);
		hash.Add(Size);
		hash.Add(Padding);
		hash.Add(ShadowOffset);
		hash.Add(Alignment);
		hash.Add(Extra);
		hash.Add(UniqueId);
		hash.Add(OnClick);
		hash.Add(OnHover);
		hash.Add(OnRightClick);
		return hash.ToHashCode();
	}

	public static bool operator ==(DividerNode left, DividerNode right) {
		return left.Equals(right);
	}

	public static bool operator !=(DividerNode left, DividerNode right) {
		return !(left == right);
	}
}

#endif
