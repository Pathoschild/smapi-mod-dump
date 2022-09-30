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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Leclair.Stardew.Common.UI.SimpleLayout;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley.Menus;

namespace Leclair.Stardew.Common.UI;

public class SimpleBuilder {
	private readonly SimpleBuilder? Parent;
	private readonly LayoutNode Layout;
	private List<ISimpleNode>? Nodes;
	private ISimpleNode[]? Built;

	public SimpleBuilder(SimpleBuilder? parent = null, LayoutNode? layout = null) {
		Parent = parent;
		Layout = layout ?? new LayoutNode(LayoutDirection.Vertical, null);
	}

	[MemberNotNull(nameof(Nodes))]
	private void AssertState() {
		if (Built != null) throw new ArgumentException("cannot modify built layout");
		if (Nodes == null) Nodes = new();
	}

	public SimpleBuilder Add(ISimpleNode node) {
		AssertState();
		Nodes.Add(node);
		return this;
	}

	public SimpleBuilder AddRange(IEnumerable<ISimpleNode> nodes) {
		AssertState();
		Nodes.AddRange(nodes);
		return this;
	}

	public SimpleBuilder AddSpacedRange(float space, IEnumerable<ISimpleNode> nodes) {
		AssertState();
		SpaceNode? n = null;
		foreach (ISimpleNode node in nodes) {
			if (n == null)
				n = new SpaceNode(Layout, false, space);
			else
				Nodes.Add(n);
			Nodes.Add(node);
		}
		return this;
	}

	public SimpleBuilder Component(ClickableComponent component, ComponentSNode.DrawDelegate? onDraw = null, Alignment align = Alignment.None) {
		AssertState();
		Nodes.Add(new ComponentSNode(component, onDraw: onDraw, align: align));
		return this;
	}

	public SimpleBuilder Divider(Texture2D? source = null, Rectangle? sourceRectVert = null, Rectangle? sourceRectHoriz = null) {
		AssertState();
		Nodes.Add(new DividerNode(Layout, source, sourceRectVert, sourceRectHoriz));
		return this;
	}

	public SimpleBuilder Space(bool expand = true, float size = 16) {
		AssertState();
		Nodes.Add(new SpaceNode(Layout, expand, size));
		return this;
	}

	public SimpleBuilder Flow(IEnumerable<FlowNode.IFlowNode> nodes, bool wrapText = true, float minWidth = -1, Alignment align = Alignment.None) {
		AssertState();
		Nodes.Add(new SimpleLayout.FlowNode(nodes, wrapText, minWidth, align));
		return this;
	}

	public SimpleBuilder Texture(Texture2D texture, Rectangle? source = null, float scale = 1f, bool drawShadow = false, Alignment align = Alignment.None) {
		AssertState();
		Nodes.Add(new TextureNode(texture, source, scale, drawShadow, align));
		return this;
	}

	public SimpleBuilder Sprite(SpriteInfo? sprite, float scale = 4f, string? label = null, int quantity = 0, Alignment align = Alignment.None) {
		AssertState();
		Nodes.Add(new SpriteNode(sprite, scale, label, quantity, align));
		return this;
	}

	public SimpleBuilder FormatText(string text, TextStyle style, bool wrapText = false, float minWidth = -1, Alignment align = Alignment.None) {
		AssertState();
		Nodes.Add(new SimpleLayout.FlowNode(
			FlowHelper.FormatText(text, style),
			wrapText: wrapText,
			minWidth: minWidth,
			alignment: align
		));
		return this;
	}

	public SimpleBuilder FormatText(string text, Color? color = null, bool? prismatic = null, SpriteFont? font = null, bool? fancy = null, bool? bold = null, bool? shadow = null, Color? shadowColor = null, bool? strikethrough = null, bool? underline = null, float? scale = null, bool wrapText = false, float minWidth = -1, Alignment align = Alignment.None) {
		TextStyle style = new(
			color: color,
			prismatic: prismatic,
			font: font,
			fancy: fancy,
			shadow: shadow,
			shadowColor: shadowColor,
			bold: bold,
			strikethrough: strikethrough,
			underline: underline,
			scale: scale
		);

		return FormatText(text, style, wrapText, minWidth, align);
	}

	public SimpleBuilder Text(string? text, TextStyle style, Alignment align = Alignment.None) {
		AssertState();
		Nodes.Add(new TextNode(text, style, align));
		return this;
	}

	public SimpleBuilder Text(string? text, Color? color = null, bool? prismatic = null, SpriteFont? font = null, bool? fancy = null, bool? bold = null, bool? shadow = null, Color? shadowColor = null, bool? strikethrough = null, bool? underline = null, float? scale = null, Alignment align = Alignment.None) {
		TextStyle style = new(
			color: color,
			prismatic: prismatic,
			font: font,
			fancy: fancy,
			shadow: shadow,
			shadowColor: shadowColor,
			bold: bold,
			strikethrough: strikethrough,
			underline: underline,
			scale: scale
		);

		return Text(text, style, align);
	}

	public SimpleBuilder Group(int margin = 0, Vector2? minSize = null, Alignment align = Alignment.None) {
		AssertState();
		LayoutDirection dir = Layout.Direction == LayoutDirection.Vertical ? LayoutDirection.Horizontal : LayoutDirection.Vertical;

		LayoutNode child = new(dir, null, margin, minSize, alignment: align);
		Nodes.Add(child);
		return new SimpleBuilder(this, child);
	}

	public SimpleBuilder EndGroup() {
		if (Parent != null)
			Layout.Children = BuildThis();
		return Parent ?? this;
	}

	public ISimpleNode[] Build() {
		return Parent?.Build() ?? BuildThis();
	}

	[MemberNotNull(nameof(Built))]
	public ISimpleNode[] BuildThis() {
		if (Built != null) return Built;
		Built = Nodes?.ToArray() ?? Array.Empty<ISimpleNode>();
		Layout.Children = Built;
		Nodes = null;
		return Built;
	}

	public LayoutNode GetLayout() {
		BuildThis();
		return Layout;
	}
}
