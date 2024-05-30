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
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.Common.UI.FlowNode;

public struct SpriteNode : IFlowNode {
	public SpriteInfo? Sprite { get; }
	public float Scale { get; }
	public float Size { get; }
	public int Frame { get; set; }
	public Alignment Alignment { get; }
	public object? Extra { get; }
	public string? UniqueId { get; }

	public int Quantity { get; }

	public bool NoComponent { get; }
	public Func<IFlowNodeSlice, int, int, bool>? OnClick { get; }
	public Func<IFlowNodeSlice, int, int, bool>? OnHover { get; }
	public Func<IFlowNodeSlice, int, int, bool>? OnRightClick { get; }

	public SpriteNode(
		SpriteInfo? sprite,
		float scale,
		Alignment? align = null,
		Func<IFlowNodeSlice, int, int, bool>? onClick = null,
		Func<IFlowNodeSlice, int, int, bool>? onHover = null,
		Func<IFlowNodeSlice, int, int, bool>? onRightClick = null,
		bool noComponent = false,
		float size = 16,
		int frame = -1,
		object? extra = null,
		string? id = null,
		int quantity = 0
	) {
		Sprite = sprite;
		Scale = scale;
		Size = size;
		Alignment = align ?? Alignment.None;
		OnClick = onClick;
		OnHover = onHover;
		OnRightClick = onRightClick;
		NoComponent = noComponent;
		Frame = frame;
		Extra = extra;
		UniqueId = id;
		Quantity = quantity;
	}

	public ClickableComponent? UseComponent(IFlowNodeSlice slice) {
		return null;
	}

	public bool? WantComponent(IFlowNodeSlice slice) {
		if (NoComponent)
			return false;
		return null;
	}

	[MemberNotNullWhen(false, nameof(Sprite))]
	public bool IsEmpty() {
		return Sprite == null || Scale <= 0;
	}

	public IFlowNodeSlice? Slice(IFlowNodeSlice? last, SpriteFont font, float maxWidth, float remaining, IFlowNodeSlice? nextSlice) {
		if (last != null)
			return null;

		float height = Size * Scale;
		float width = Size * Scale;

		if (Quantity > 0) {
			float qScale = (float) Math.Round(Scale * 0.75f);
			float qX = width - Utility.getWidthOfTinyDigitString(Quantity, qScale) + qScale;
			if (qX < 0)
				width -= qX;
		}

		return new UnslicedNode(this, width, height, WrapMode.None);
	}

	public void Draw(IFlowNodeSlice slice, SpriteBatch batch, Vector2 position, float scale, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor, CachedFlowLine line, CachedFlow flow) {
		if (IsEmpty())
			return;

		Sprite.Draw(batch, position, scale * Scale, Frame, Size);

		// Draw Quantity
		if (Quantity > 0) {
			float itemSize = Size * Scale;
			float offsetX = 0;

			float qScale = (float) Math.Round(Scale * 0.75f);
			float qX = position.X + itemSize - Utility.getWidthOfTinyDigitString(Quantity, qScale) + qScale;
			float qY = position.Y + itemSize - 6f * qScale + 2f;

			if (qX < position.X)
				offsetX = position.X - qX;

			Utility.drawTinyDigits(Quantity, batch, new Vector2(qX + offsetX, qY), qScale, 1f, Color.White);
		}

	}

	public override bool Equals(object? obj) {
		return obj is SpriteNode node &&
			   EqualityComparer<SpriteInfo>.Default.Equals(Sprite, node.Sprite) &&
			   Scale == node.Scale &&
			   Size == node.Size &&
			   Frame == node.Frame &&
			   Alignment == node.Alignment &&
			   Quantity == node.Quantity &&
			   EqualityComparer<object>.Default.Equals(Extra, node.Extra) &&
			   UniqueId == node.UniqueId &&
			   NoComponent == node.NoComponent &&
			   EqualityComparer<Func<IFlowNodeSlice, int, int, bool>>.Default.Equals(OnClick, node.OnClick) &&
			   EqualityComparer<Func<IFlowNodeSlice, int, int, bool>>.Default.Equals(OnHover, node.OnHover) &&
			   EqualityComparer<Func<IFlowNodeSlice, int, int, bool>>.Default.Equals(OnRightClick, node.OnRightClick);
	}

	public override int GetHashCode() {
		HashCode hash = new();
		hash.Add(Sprite);
		hash.Add(Scale);
		hash.Add(Size);
		hash.Add(Frame);
		hash.Add(Alignment);
		hash.Add(Quantity);
		hash.Add(Extra);
		hash.Add(UniqueId);
		hash.Add(NoComponent);
		hash.Add(OnClick);
		hash.Add(OnHover);
		hash.Add(OnRightClick);
		return hash.ToHashCode();
	}

	public static bool operator ==(SpriteNode left, SpriteNode right) {
		return left.Equals(right);
	}

	public static bool operator !=(SpriteNode left, SpriteNode right) {
		return !(left == right);
	}
}

#endif
