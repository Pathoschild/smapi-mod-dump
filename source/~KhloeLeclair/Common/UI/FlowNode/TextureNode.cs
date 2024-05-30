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

using StardewValley.Menus;

namespace Leclair.Stardew.Common.UI.FlowNode;

public struct TextureNode : IFlowNode {

	public Texture2D? Texture { get; }
	public Rectangle? Source { get; }

	public float Scale { get; }
	public Alignment Alignment { get; }
	public object? Extra { get; }
	public string? UniqueId { get; }

	public bool NoComponent { get; }
	public Func<IFlowNodeSlice, int, int, bool>? OnClick { get; }
	public Func<IFlowNodeSlice, int, int, bool>? OnHover { get; }
	public Func<IFlowNodeSlice, int, int, bool>? OnRightClick { get; }

	public TextureNode(
		Texture2D? texture,
		Rectangle? source,
		float scale,
		Alignment? align = null,
		Func<IFlowNodeSlice, int, int, bool>? onClick = null,
		Func<IFlowNodeSlice, int, int, bool>? onHover = null,
		Func<IFlowNodeSlice, int, int, bool>? onRightClick = null,
		bool noComponent = false,
		object? extra = null,
		string? id = null
	) {
		Texture = texture;
		Source = source;
		Scale = scale;
		Alignment = align ?? Alignment.None;
		OnClick = onClick;
		OnHover = onHover;
		OnRightClick = onRightClick;
		NoComponent = noComponent;
		Extra = extra;
		UniqueId = id;
	}

	public ClickableComponent? UseComponent(IFlowNodeSlice slice) {
		return null;
	}

	public bool? WantComponent(IFlowNodeSlice slice) {
		if (NoComponent)
			return false;
		return null;
	}

	[MemberNotNullWhen(false, nameof(Texture))]
	[MemberNotNullWhen(false, nameof(Source))]
	public bool IsEmpty() {
		return Texture is null || !Source.HasValue || Source.Value.Width <= 0 || Source.Value.Height <= 0 || Scale <= 0;
	}

	public IFlowNodeSlice? Slice(IFlowNodeSlice? last, SpriteFont font, float maxWidth, float remaining, IFlowNodeSlice? nextSlice) {
		if (last != null || IsEmpty())
			return null;

		return new UnslicedNode(this, Source.Value.Width * Scale, Source.Value.Height * Scale, WrapMode.None);
	}

	public void Draw(IFlowNodeSlice slice, SpriteBatch batch, Vector2 position, float scale, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor, CachedFlowLine line, CachedFlow flow) {
		if (IsEmpty())
			return;

		batch.Draw(
			texture: Texture,
			position: position,
			sourceRectangle: Source.Value,
			color: Color.White,
			rotation: 0f,
			origin: Vector2.Zero,
			scale: Scale * scale,
			effects: SpriteEffects.None,
			layerDepth: 1f
		);
	}

	public override bool Equals(object? obj) {
		return obj is TextureNode node &&
			   EqualityComparer<Texture2D>.Default.Equals(Texture, node.Texture) &&
			   EqualityComparer<Rectangle?>.Default.Equals(Source, node.Source) &&
			   Scale == node.Scale &&
			   Alignment == node.Alignment &&
			   EqualityComparer<object>.Default.Equals(Extra, node.Extra) &&
			   UniqueId == node.UniqueId &&
			   NoComponent == node.NoComponent &&
			   EqualityComparer<Func<IFlowNodeSlice, int, int, bool>>.Default.Equals(OnClick, node.OnClick) &&
			   EqualityComparer<Func<IFlowNodeSlice, int, int, bool>>.Default.Equals(OnHover, node.OnHover) &&
			   EqualityComparer<Func<IFlowNodeSlice, int, int, bool>>.Default.Equals(OnRightClick, node.OnRightClick);
	}

	public override int GetHashCode() {
		HashCode hash = new();
		hash.Add(Texture);
		hash.Add(Source);
		hash.Add(Scale);
		hash.Add(Alignment);
		hash.Add(Extra);
		hash.Add(UniqueId);
		hash.Add(NoComponent);
		hash.Add(OnClick);
		hash.Add(OnHover);
		hash.Add(OnRightClick);
		return hash.ToHashCode();
	}

	public static bool operator ==(TextureNode left, TextureNode right) {
		return left.Equals(right);
	}

	public static bool operator !=(TextureNode left, TextureNode right) {
		return !(left == right);
	}
}

#endif
