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
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley.Menus;

namespace Leclair.Stardew.Common.UI.FlowNode;

public class SelectableNode : IFlowNode {

	public Texture2D? SelectedTexture { get; set; }
	public Rectangle? SelectedSource { get; set; }
	public Color? SelectedColor { get; set; }

	public float SelectedScale { get; set; } = 4f;

	public Texture2D? HoverTexture { get; set; }
	public Rectangle? HoverSource { get; set; }
	public Color? HoverColor { get; set; }

	public float HoverScale { get; set; } = 4f;

	public bool Selected { get; set; }
	public bool Hovered { get; private set; }
	public IFlowNode[] Nodes { get; }
	public Alignment Alignment { get; }

	public string? UniqueId { get; }
	public object? Extra { get; }

	public Func<IFlowNodeSlice, int, int, bool>? OnClick => _onClick;
	public Func<IFlowNodeSlice, int, int, bool>? OnHover => _onHover;
	public Func<IFlowNodeSlice, int, int, bool>? OnRightClick { get; }

	public Func<IFlowNodeSlice, int, int, bool>? delClick { get; }
	public Func<IFlowNodeSlice, int, int, bool>? delHover { get; }

	private CachedFlow? Flow = null;

	public SelectableNode(
		IFlowNode[] nodes,
		Alignment? align = null,
		Func<IFlowNodeSlice, int, int, bool>? onClick = null,
		Func<IFlowNodeSlice, int, int, bool>? onHover = null,
		Func<IFlowNodeSlice, int, int, bool>? onRightClick = null,
		object? extra = null,
		string? id = null
	) {
		Nodes = nodes;
		Alignment = align ?? Alignment.None;
		delClick = onClick;
		delHover = onHover;
		OnRightClick = onRightClick;
		Extra = extra;
		UniqueId = id;
	}

	public ClickableComponent? UseComponent(IFlowNodeSlice slice) {
		return null;
	}

	public bool? WantComponent(IFlowNodeSlice slice) {
		return true;
	}

	public bool IsEmpty() {
		return Nodes == null || Nodes.Length == 0 || Nodes.All(val => val.IsEmpty());
	}

	#region Slicing

	public IFlowNodeSlice? Slice(IFlowNodeSlice? last, SpriteFont font, float maxWidth, float remaining, IFlowNodeSlice? nextSlice) {
		if (last != null)
			return null;

		if (Flow.HasValue)
			Flow = FlowHelper.CalculateFlow(
				Flow.Value,
				maxWidth: maxWidth - 24,
				defaultFont: font
			);
		else
			Flow = FlowHelper.CalculateFlow(
				Nodes,
				maxWidth: maxWidth - 24,
				defaultFont: font
			);

		return new UnslicedNode(
			this,
			maxWidth,
			Flow.Value.Height + 24,
			WrapMode.ForceAfter
		);
	}

	#endregion

	#region Events

	public bool _onClick(IFlowNodeSlice slice, int x, int y) {
		if (delClick?.Invoke(slice, x, y) ?? false) {
			Selected = true;
			return true;
		}

		return false;
	}

	public bool _onHover(IFlowNodeSlice slice, int x, int y) {
		if (delHover?.Invoke(slice, x, y) ?? true) {
			Hovered = true;
			return true;
		}

		return false;
	}

	#endregion

	#region Drawing

	public void Draw(IFlowNodeSlice slice, SpriteBatch batch, Vector2 position, float scale, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor, CachedFlowLine line, CachedFlow flow) {

		if (Selected) {
			if (SelectedTexture != null)
				RenderHelper.DrawBox(
					batch,
					SelectedTexture,
					SelectedSource ?? SelectedTexture.Bounds,
					(int) position.X + 2, (int) position.Y + 2,
					(int) slice.Width - 4, (int) slice.Height - 4,
					SelectedColor ?? Color.White,
					scale: SelectedScale,
					drawShadow: false
				);

		} else if (Hovered) {
			if (HoverTexture != null)
				RenderHelper.DrawBox(
					batch,
					HoverTexture,
					HoverSource ?? HoverTexture.Bounds,
					(int) position.X, (int) position.Y,
					(int) slice.Width, (int) slice.Height,
					HoverColor ?? Color.White,
					scale: HoverScale,
					drawShadow: false
				);
		}

		Hovered = false;

		if (Flow.HasValue)
			FlowHelper.RenderFlow(
				batch,
				Flow.Value,
				new Vector2(
					position.X + 12,
					position.Y + 12
				),
				defaultColor,
				defaultShadowColor: defaultShadowColor,
				lineOffset: 0
			);
	}

	#endregion

}

#endif
