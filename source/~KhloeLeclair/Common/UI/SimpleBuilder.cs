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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Leclair.Stardew.Common.UI.SimpleLayout;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.Common.UI;



public interface ISimpleBuilder {

	/// <summary>
	/// Whether or not this builder contains any <see cref="ISimpleNode"/>s.
	/// </summary>
	bool IsEmpty();

	/// <summary>
	/// Add a new <see cref="ISimpleNode"/> instance to the builder.
	/// </summary>
	/// <param name="node">The node to add.</param>
	ISimpleBuilder Add(ISimpleNode node);

	/// <summary>
	/// Add an enumeration of new <see cref="ISimpleNode"/> instances
	/// to the builder.
	/// </summary>
	/// <param name="nodes">The enumeration of nodes to add.</param>
	ISimpleBuilder AddRange(IEnumerable<ISimpleNode> nodes);

	/// <summary>
	/// Add an enumeration of new <see cref="ISimpleNode"/> instances
	/// to the builder, with spacing between each added node.
	/// </summary>
	/// <param name="space">The amount of space to insert.</param>
	/// <param name="nodes">The enumeration of nodes to add.</param>
	ISimpleBuilder AddSpacedRange(float space, IEnumerable<ISimpleNode> nodes);

	/// <summary>
	/// Add a node representing a <see cref="ClickableComponent"/>
	/// to the builder. When this node is rendered, it will update
	/// the <see cref="ClickableComponent.bounds"/>, and if the
	/// component is a <see cref="ClickableAnimatedComponent"/> or
	/// <see cref="ClickableTextureComponent"/>, it will draw it
	/// automatically. Alternatively, you can pass in a
	/// drawing delegate.
	/// </summary>
	/// <param name="component">The component to position / draw.</param>
	/// <param name="onDraw">A delegate for drawing the component.</param>
	/// <param name="align">How this node should be aligned
	/// within its parent node.</param>
	ISimpleBuilder Component(
		ClickableComponent component,
		ComponentSNode.DrawDelegate? onDraw = null,
		Alignment align = Alignment.None
	);

	/// <summary>
	/// Add a node representing a visual divider to the builder.
	/// This node may be horizontal or vertical, depending on the
	/// layout direction of its parent node.
	/// </summary>
	/// <param name="thick">Whether to draw this as a thick divider or a thin one.</param>
	/// <param name="thinColor">The color to draw the divider when thin.</param>
	/// <param name="source">An optional texture to draw it from, when thick.</param>
	/// <param name="sourceRectVert">If this is set, and the divider
	/// is vertical, this source will be used to draw the divider.</param>
	/// <param name="sourceRectHoriz">If this is set, and the divider
	/// is horizontal, this source will be used to draw the divider.</param>
	ISimpleBuilder Divider(bool thick = true, Color? thinColor = null, Texture2D? source = null, Rectangle? sourceRectVert = null, Rectangle? sourceRectHoriz = null);

	/// <summary>
	/// Add a spacing node to the builder.
	/// </summary>
	/// <param name="expand">Whether or not this node should expand
	/// to fill the available space.</param>
	/// <param name="size">The minimum size of this space.</param>
	ISimpleBuilder Space(bool expand = true, float size = 16);

#if COMMON_FLOW
	/// <summary>
	/// Add a Flow layout to the builder. Flow is the name of the
	/// rich text layout system, which incorporated word wrapping
	/// and in-line elements.
	/// </summary>
	/// <param name="nodes">An enumeration of <see cref="FlowNode.IFlowNode"/>s
	/// to add.</param>
	/// <param name="wrapText">Whether or not to render with
	/// word wrapping enabled.</param>
	/// <param name="minWidth">The minimum width to display this
	/// Flow with.</param>
	/// <param name="align">How this node should be aligned
	/// within its parent node.</param>
	ISimpleBuilder Flow(IEnumerable<FlowNode.IFlowNode> nodes, bool wrapText = true, float minWidth = -1, Alignment align = Alignment.None);
#endif

	ISimpleBuilder Texture(Texture2D texture, Rectangle? source = null, float scale = 1f, bool drawShadow = false, Alignment align = Alignment.None);

	ISimpleBuilder Sprite(SpriteInfo? sprite, float scale = 4f, string? label = null, int quantity = 0, Alignment align = Alignment.None, float? overrideHeight = null);

	ISimpleBuilder Dynamic(DynamicDrawingNode.GetSizeDelegate getSize, DynamicDrawingNode.DrawNodeDelegate draw, Alignment align = Alignment.None);

	ISimpleBuilder Attachments(Item item, Alignment align = Alignment.None);

#if COMMON_FLOW
	ISimpleBuilder FormatText(string text, Color? color = null, bool? prismatic = null, SpriteFont? font = null, bool? fancy = null, bool? bold = null, bool? shadow = null, Color? shadowColor = null, bool? strikethrough = null, bool? underline = null, float? scale = null, bool wrapText = false, float minWidth = -1, Alignment align = Alignment.None);
#endif

	ISimpleBuilder Text(string? text, Color? color = null, bool? prismatic = null, SpriteFont? font = null, bool? fancy = null, bool? bold = null, bool? shadow = null, Color? shadowColor = null, bool? strikethrough = null, bool? underline = null, float? scale = null, Alignment align = Alignment.None);

	ISimpleBuilder Group(int margin = 0, Vector2? minSize = null, Alignment align = Alignment.None);

	ISimpleBuilder EndGroup();

	ISimpleNode[] Build();

	ISimpleNode[] BuildThis();

	ILayoutNode GetLayout();

}



public class SimpleBuilder : ISimpleBuilder {
	private readonly SimpleBuilder? Parent;
	private readonly LayoutNode Layout;
	private List<ISimpleNode>? Nodes;
	private ISimpleNode[]? Built;

	public SimpleBuilder(SimpleBuilder? parent = null, LayoutNode? layout = null) {
		Parent = parent;
		Layout = layout ?? new LayoutNode(LayoutDirection.Vertical, null);
	}

	public bool IsEmpty() {
		if (Built != null) return Built.Length == 0;
		if (Nodes != null) return Nodes.Count == 0;
		return true;
	}

	[MemberNotNull(nameof(Nodes))]
	private void AssertState() {
		if (Built != null) throw new ArgumentException("cannot modify built layout");
		Nodes ??= new();
	}

	public ISimpleBuilder Add(ISimpleNode node) {
		AssertState();
		Nodes.Add(node);
		return this;
	}

	public ISimpleBuilder AddRange(IEnumerable<ISimpleNode> nodes) {
		AssertState();
		Nodes.AddRange(nodes);
		return this;
	}

	public ISimpleBuilder AddSpacedRange(float space, IEnumerable<ISimpleNode> nodes) {
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

	public ISimpleBuilder Component(ClickableComponent component, ComponentSNode.DrawDelegate? onDraw = null, Alignment align = Alignment.None) {
		AssertState();
		Nodes.Add(new ComponentSNode(component, onDraw: onDraw, align: align));
		return this;
	}

	public ISimpleBuilder Divider(bool thick = true, Color? thinColor = null, Texture2D? source = null, Rectangle? sourceRectVert = null, Rectangle? sourceRectHoriz = null) {
		AssertState();
		Nodes.Add(new DividerNode(Layout, thick, thinColor, source, sourceRectVert, sourceRectHoriz));
		return this;
	}

	public ISimpleBuilder Space(bool expand = true, float size = 16) {
		AssertState();
		Nodes.Add(new SpaceNode(Layout, expand, size));
		return this;
	}

#if COMMON_FLOW
	public ISimpleBuilder Flow(IEnumerable<FlowNode.IFlowNode> nodes, bool wrapText = true, float minWidth = -1, Alignment align = Alignment.None) {
		AssertState();
		Nodes.Add(new SimpleLayout.FlowNode(nodes, wrapText, minWidth, align));
		return this;
	}
#endif

	public ISimpleBuilder Texture(Texture2D texture, Rectangle? source = null, float scale = 1f, bool drawShadow = false, Alignment align = Alignment.None) {
		AssertState();
		Nodes.Add(new TextureNode(texture, source, scale, drawShadow, align));
		return this;
	}

	public ISimpleBuilder Sprite(SpriteInfo? sprite, float scale = 4f, string? label = null, int quantity = 0, Alignment align = Alignment.None, float? overrideHeight = null) {
		AssertState();
		Nodes.Add(new SpriteNode(sprite, scale, label, quantity, align, overrideHeight));
		return this;
	}

	public ISimpleBuilder Dynamic(DynamicDrawingNode.GetSizeDelegate getSize, DynamicDrawingNode.DrawNodeDelegate draw, Alignment align = Alignment.None) {
		AssertState();
		Nodes.Add(new DynamicDrawingNode(getSize, draw, align));
		return this;
	}

	public ISimpleBuilder Attachments(Item item, Alignment align = Alignment.None) {
		AssertState();
		Nodes.Add(new AttachmentSlotsNode(item, align));
		return this;
	}

#if COMMON_FLOW
	public ISimpleBuilder FormatText(string text, TextStyle style, bool wrapText = false, float minWidth = -1, Alignment align = Alignment.None) {
		AssertState();
		Nodes.Add(new SimpleLayout.FlowNode(
			FlowHelper.FormatText(text, style),
			wrapText: wrapText,
			minWidth: minWidth,
			alignment: align
		));
		return this;
	}

	public ISimpleBuilder FormatText(string text, Color? color = null, bool? prismatic = null, SpriteFont? font = null, bool? fancy = null, bool? bold = null, bool? shadow = null, Color? shadowColor = null, bool? strikethrough = null, bool? underline = null, float? scale = null, bool wrapText = false, float minWidth = -1, Alignment align = Alignment.None) {
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
#endif

	public ISimpleBuilder Text(string? text, TextStyle style, Alignment align = Alignment.None) {
		AssertState();
		Nodes.Add(new TextNode(text, style, align));
		return this;
	}

	public ISimpleBuilder Text(string? text, Color? color = null, bool? prismatic = null, SpriteFont? font = null, bool? fancy = null, bool? bold = null, bool? shadow = null, Color? shadowColor = null, bool? strikethrough = null, bool? underline = null, float? scale = null, Alignment align = Alignment.None) {
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

	public ISimpleBuilder Group(int margin = 0, Vector2? minSize = null, Alignment align = Alignment.None) {
		AssertState();
		LayoutDirection dir = Layout.Direction == LayoutDirection.Vertical ? LayoutDirection.Horizontal : LayoutDirection.Vertical;

		LayoutNode child = new(dir, null, margin, minSize, alignment: align);
		Nodes.Add(child);
		return new SimpleBuilder(this, child);
	}

	public ISimpleBuilder EndGroup() {
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
		Built = Nodes?.ToArray() ?? [];
		Layout.Children = Built;
		Nodes = null;
		return Built;
	}

	public ILayoutNode GetLayout() {
		BuildThis();
		return Layout;
	}
}

#endif
