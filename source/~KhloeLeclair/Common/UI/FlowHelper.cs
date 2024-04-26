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
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using Leclair.Stardew.Common.UI.FlowNode;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.Common.UI;

public class FlowBuilder {
	private readonly FlowBuilder? Parent;
	private readonly NestedNode? Node;
	private List<IFlowNode>? Nodes;
	private IFlowNode[]? Built;

	public FlowBuilder(FlowBuilder? parent = null, NestedNode? node = null) {
		Parent = parent;
		Node = node;
	}

	public int Count {
		get {
			if (Built != null)
				return Built.Length;
			if (Nodes != null)
				return Nodes.Count;
			return 0;
		}
	}

	[MemberNotNull(nameof(Nodes))]
	private void AssertState() {
		if (Built != null) throw new ArgumentException("cannot modify built flow");
		if (Nodes == null) Nodes = new();
	}

	public FlowBuilder Add(IFlowNode node) {
		AssertState();
		Nodes.Add(node);
		return this;
	}

	public FlowBuilder AddRange(FlowBuilder builder) {
		return AddRange(builder.Build());
	}

	public FlowBuilder AddRange(IEnumerable<IFlowNode> nodes) {
		AssertState();
		Nodes.AddRange(nodes);
		return this;
	}

	public FlowBuilder Divider(
		Color? color = null,
		Color? shadowColor = null,
		float size = 4f,
		float padding = 14f,
		float shadowOffset = 2f,
		object? extra = null,
		string? id = null
	) {
		AssertState();
		Nodes.Add(new DividerNode(
			color: color,
			shadowColor: shadowColor,
			size: size,
			padding: padding,
			shadowOffset: shadowOffset,
			extra: extra,
			id: id
		));
		return this;
	}

	public FlowBuilder Texture(
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
		AssertState();
		Nodes.Add(new TextureNode(
			texture: texture,
			source: source,
			scale: scale,
			align: align,
			onClick: onClick,
			onHover: onHover,
			onRightClick: onRightClick,
			noComponent: noComponent,
			extra: extra,
			id: id
		));
		return this;
	}

	public FlowBuilder Sprite(
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
		string? id = null
	) {
		AssertState();
		Nodes.Add(new SpriteNode(
			sprite: sprite,
			scale: scale,
			align: align,
			onClick: onClick,
			onHover: onHover,
			onRightClick: onRightClick,
			noComponent: noComponent,
			size: size,
			frame: frame,
			extra: extra,
			id: id
		));
		return this;
	}

	public FlowBuilder FormatText(
		string text,
		TextStyle style,
		Alignment? align = null,
		Func<IFlowNodeSlice, int, int, bool>? onClick = null,
		Func<IFlowNodeSlice, int, int, bool>? onHover = null,
		Func<IFlowNodeSlice, int, int, bool>? onRightClick = null,
		bool noComponent = false,
		object? extra = null,
		string? id = null
	) {
		AssertState();
		Nodes.AddRange(FlowHelper.FormatText(
			text: text,
			style: style,
			align: align,
			onClick: onClick,
			onHover: onHover,
			onRightClick: onRightClick,
			noComponent: noComponent,
			extra: extra,
			id: id
		));
		return this;
	}

	public FlowBuilder Text(
		string text,
		TextStyle style,
		Alignment? align = null,
		Func<IFlowNodeSlice, int, int, bool>? onClick = null,
		Func<IFlowNodeSlice, int, int, bool>? onHover = null,
		Func<IFlowNodeSlice, int, int, bool>? onRightClick = null,
		bool noComponent = false,
		object? extra = null,
		string? id = null
	) {
		AssertState();
		Nodes.Add(new TextNode(
			text: text,
			style: style,
			align: align,
			onClick: onClick,
			onHover: onHover,
			onRightClick: onRightClick,
			noComponent: noComponent,
			extra: extra,
			id: id
		));
		return this;
	}

	public FlowBuilder Translate(
		Translation source,
		object values,
		TextStyle? style = null,
		Alignment? align = null,
		Func<IFlowNodeSlice, int, int, bool>? onClick = null,
		Func<IFlowNodeSlice, int, int, bool>? onHover = null,
		Func<IFlowNodeSlice, int, int, bool>? onRightClick = null,
		bool noComponent = false,
		object? extra = null,
		string? id = null
	) {
		AssertState();
		Nodes.AddRange(FlowHelper.Translate(
			source: source,
			values: values,
			style: style,
			align: align,
			onClick: onClick,
			onHover: onHover,
			onRightClick: onRightClick,
			noComponent: noComponent,
			extra: extra,
			id: id
		));
		return this;
	}

	public FlowBuilder FormatText(
		string text,
		Color? color = null,
		bool? prismatic = null,
		SpriteFont? font = null,
		bool? fancy = null,
		bool? bold = null,
		bool? shadow = null,
		Color? shadowColor = null,
		bool? strikethrough = null,
		bool? underline = null,
		float? scale = null,
		Alignment? align = null,
		Func<IFlowNodeSlice, int, int, bool>? onClick = null,
		Func<IFlowNodeSlice, int, int, bool>? onHover = null,
		Func<IFlowNodeSlice, int, int, bool>? onRightClick = null,
		bool noComponent = false,
		object? extra = null,
		string? id = null
	) {
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

		return FormatText(
			text: text,
			style: style,
			align: align,
			onClick: onClick,
			onHover: onHover,
			onRightClick: onRightClick,
			noComponent: noComponent,
			extra: extra,
			id: id
		);
	}

	public FlowBuilder Text(
		string text,
		Color? color = null,
		bool? prismatic = null,
		SpriteFont? font = null,
		bool? fancy = null,
		bool? bold = null,
		bool? shadow = null,
		Color? shadowColor = null,
		bool? strikethrough = null,
		bool? underline = null,
		float? scale = null,
		Alignment? align = null,
		Func<IFlowNodeSlice, int, int, bool>? onClick = null,
		Func<IFlowNodeSlice, int, int, bool>? onHover = null,
		Func<IFlowNodeSlice, int, int, bool>? onRightClick = null,
		bool noComponent = false,
		object? extra = null,
		string? id = null
	) {
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

		return Text(
			text: text,
			style: style,
			align: align,
			onClick: onClick,
			onHover: onHover,
			onRightClick: onRightClick,
			noComponent: noComponent,
			extra: extra,
			id: id
		);
	}

	public FlowBuilder Group(
		Alignment? align = null,
		Func<IFlowNodeSlice, int, int, bool>? onClick = null,
		Func<IFlowNodeSlice, int, int, bool>? onHover = null,
		Func<IFlowNodeSlice, int, int, bool>? onRightClick = null,
		Func<IFlowNodeSlice, bool?>? wantComponent = null,
		object? extra = null,
		string? id = null
	) {
		AssertState();
		NestedNode nested = new(
			null,
			align: align,
			onClick: onClick,
			onHover: onHover,
			onRightClick: onRightClick,
			wantComponent: wantComponent,
			extra: extra,
			id: id
		);
		Nodes.Add(nested);
		return new FlowBuilder(this, nested);
	}

	protected FlowBuilder ReplaceNode(IFlowNode source, IFlowNode replacement) {
		AssertState();
		int idx = Nodes.IndexOf(source);
		if (idx != -1)
			Nodes[idx] = replacement;
		return this;
	}

	public FlowBuilder EndGroup() {
		if (Parent != null && Node.HasValue)
			Parent.ReplaceNode(Node.Value, new NestedNode(BuildThis(), align: Node.Value.Alignment));

		return Parent ?? this;
	}

	public IFlowNode[] Build() {
		return Parent?.Build() ?? BuildThis();
	}

	[MemberNotNull(nameof(Built))]
	public IFlowNode[] BuildThis() {
		if (Built != null) return Built;
		Built = Nodes?.ToArray() ?? Array.Empty<IFlowNode>();
		Nodes = null;
		return Built;
	}
}

public static class FlowHelper {

#if DEBUG
	private static readonly Color[] DEBUG_COLORS = new Color[] {
		Color.Blue,
		Color.Purple,
		Color.Gray,
		Color.Gold,
		Color.Fuchsia,
		Color.Orange
	};
#endif

	public static readonly Regex I18N_REPLACER = new("{{([ \\w\\.\\-]+)}}");

	public static IFlowNode? GetNode(
		object? obj,
		Alignment? align = null,
		TextStyle? style = null,
		bool format = false
	) {
		if (obj == null)
			return null;
		if (obj is IFlowNode node)
			return node;
		if (obj is IFlowNode[] nodes)
			return new NestedNode(nodes, align: align);
		if (obj is IEnumerable<IFlowNode> nlist)
			return new NestedNode(nlist.ToArray(), align: align);
		if (obj is Tuple<int>) {
			return new DividerNode(null);
		}
		if (obj is Texture2D tex)
			return new SpriteNode(
				new SpriteInfo(tex, tex.Bounds),
				2f,
				align: align
			);
		if (obj is Tuple<Texture2D, float> twople)
			return new SpriteNode(
				new SpriteInfo(twople.Item1, twople.Item1.Bounds),
				twople.Item2,
				align: align
			);
		if (obj is Tuple<Texture2D, Rectangle> tuple)
			return new SpriteNode(
				new SpriteInfo(tuple.Item1, tuple.Item2),
				2f,
				align: align
			);
		if (obj is Tuple<Texture2D, Rectangle, float> triple)
			return new SpriteNode(
				new SpriteInfo(triple.Item1, triple.Item2),
				triple.Item3,
				align: align
			);
		if (obj is SpriteInfo sprite)
			return new SpriteNode(sprite, 2f, align: align);
		if (obj is Tuple<SpriteInfo, float> spriple)
			return new SpriteNode(spriple.Item1, spriple.Item2, align: align);
		if (obj is Item item)
			return new SpriteNode(
				SpriteHelper.GetSprite(item),
				2f,
				align: align
			);
		if (obj is Tuple<Item, float> ituple)
			return new SpriteNode(
				SpriteHelper.GetSprite(ituple.Item1),
				ituple.Item2,
				align: align
			);

		string? sobj = obj.ToString();
		if (sobj is null)
			return null;

		if (format) {
			IFlowNode[]? nods = FormatText(sobj, style: style, align: align)?.ToArray();
			if (nods == null || nods.Length == 0)
				return null;
			if (nods.Length == 1)
				return nods[0];
			return new NestedNode(nods, align: align);
		}

		return new TextNode(sobj, style: style, align: align);
	}

	public static List<IFlowNode> GetNodes(
		object[] objs,
		Alignment? align = null,
		TextStyle? style = null,
		bool format = false
	) {
		List<IFlowNode> result = new();

		foreach(object obj in objs) {
			IFlowNode? node = GetNode(obj, align, style, format);
			if (node is NestedNode nn && nn.Nodes != null)
				result.AddRange(nn.Nodes);
			else if (node != null)
				result.Add(node);
		}

		return result;
	}

	private static string? ReadSubString(string text, int i, out int end) {
		char chr = text[i];
		if (chr != '{') {
			end = i;
			return null;
		}

		i++;
		int start = i;

		while (i < text.Length && text[i] != '}')
			i++;

		end = i + 1;
		return text[start..i];
	}

	public static IEnumerable<IFlowNode> FormatText(
		string text,
		TextStyle? style = null,
		Alignment? align = null,
		Func<IFlowNodeSlice, int, int, bool>? onClick = null,
		Func<IFlowNodeSlice, int, int, bool>? onHover = null,
		Func<IFlowNodeSlice, int, int, bool>? onRightClick = null,
		bool noComponent = false,
		object? extra = null,
		string? id = null
	) {
		return FormatText(
			text: text,
			out TextStyle _,
			out Alignment __,
			style: style,
			align: align,
			onClick: onClick,
			onHover: onHover,
			onRightClick: onRightClick,
			noComponent: noComponent,
			extra: extra,
			id: id
		);
	}

	public static string EscapeFormatText(string text) {
		return text.Replace("@", "@@");
	}

	public static IEnumerable<IFlowNode> FormatText(
		string text,
		out TextStyle endStyle,
		out Alignment endAlign,
		TextStyle? style = null,
		Alignment? align = null,
		Func<IFlowNodeSlice, int, int, bool>? onClick = null,
		Func<IFlowNodeSlice, int, int, bool>? onHover = null,
		Func<IFlowNodeSlice, int, int, bool>? onRightClick = null,
		bool noComponent = false,
		object? extra = null,
		string? id = null
	) {
		if (string.IsNullOrEmpty(text) || !text.Contains('@')) {
			endStyle = style ?? TextStyle.EMPTY;
			endAlign = align ?? Alignment.None;

			return new IFlowNode[]{
				new TextNode(
					text: text,
					style: style,
					align: align,
					onClick: onClick,
					onHover: onHover,
					onRightClick: onRightClick,
					noComponent: noComponent,
					extra: extra,
					id: id
				)
			};
		}

		List<IFlowNode> nodes = new();

		TextStyle s = style ?? TextStyle.EMPTY;
		Alignment a = align ?? Alignment.None;

		Item? item = null;
		bool itemLabel = false;

		StringBuilder builder = new();

		int i = 0;

		while (i < text.Length) {
			char chr = text[i++];
			if (chr != '@' || i == text.Length) {
				builder.Append(chr);
				continue;
			}

			char next = text[i++];
			TextStyle ns = s;
			Alignment na = a;

			Color? color;
			int ni;

			switch (next) {
				// Alignment
				case '<':
					if (na.HasFlag(Alignment.Left))
						continue;
					na = na.With(Alignment.Left);
					break;

				case '|':
					if (na.HasFlag(Alignment.HCenter))
						continue;
					na = na.With(Alignment.HCenter);
					break;

				case '>':
					if (na.HasFlag(Alignment.Right))
						continue;
					na = na.With(Alignment.Right);
					break;

				case '^':
					if (na.HasFlag(Alignment.Top))
						continue;
					na = na.With(Alignment.Top);
					break;

				case '-':
					if (na.HasFlag(Alignment.VCenter))
						continue;
					na = na.With(Alignment.VCenter);
					break;

				case 'v':
					if (na.HasFlag(Alignment.Bottom))
						continue;
					na = na.With(Alignment.Bottom);
					break;

				// Style
				case '_':
					string? size = ReadSubString(text, i, out ni);
					i = ni;
					float? scale = null;
					if (float.TryParse(size, out float sp))
						scale = sp;
					if (ns.Scale == scale)
						continue;
					ns = new(ns, font: ns.Font, color: ns.Color, backgroundColor: ns.BackgroundColor, shadowColor: ns.ShadowColor, scale: scale);
					break;

				case 'b':
					if (!ns.IsBold())
						continue;
					ns = new(ns, bold: false);
					break;

				case 'B':
					if (ns.IsBold())
						continue;
					ns = new(ns, bold: true);
					break;

				case 'c':
					color = CommonHelper.ParseColor(ReadSubString(text, i, out ni));
					i = ni;
					if (ns.ShadowColor == color)
						continue;
					ns = new(ns, font: ns.Font, color: ns.Color, backgroundColor: ns.BackgroundColor, shadowColor: color, scale: ns.Scale);
					break;

				case 'C':
					color = CommonHelper.ParseColor(ReadSubString(text, i, out ni));
					i = ni;
					if (ns.Color == color)
						continue;
					ns = new(ns, font: ns.Font, color: color, backgroundColor: ns.BackgroundColor, shadowColor: ns.ShadowColor, scale: ns.Scale);
					break;

				case 'f':
					if (!ns.IsFancy())
						continue;
					ns = new(ns, fancy: false);
					break;

				case 'F':
					if (ns.IsFancy())
						continue;
					ns = new(ns, fancy: true);
					break;

				case 'h':
					if (!ns.HasShadow())
						continue;
					ns = new(ns, shadow: false);
					break;

				case 'H':
					if (ns.HasShadow())
						continue;
					ns = new(ns, shadow: true);
					break;

				case 'i':
					if (!ns.IsInverted())
						continue;
					ns = new(ns, invert: false);
					break;

				case 'I':
					if (ns.IsInverted())
						continue;
					ns = new(ns, invert: true);
					break;

				case 'j':
					if (!ns.IsJunimo())
						continue;
					ns = new(ns, junimo: false);
					break;

				case 'J':
					if (ns.IsJunimo())
						continue;
					ns = new(ns, junimo: true);
					break;

				case 'm':
				case 'M':
					string? iname = ReadSubString(text, i, out ni);
					itemLabel = next == 'M';
					i = ni;
					try {
						item = string.IsNullOrEmpty(iname) ? null : ItemRegistry.Create(iname, 1);
						break;
					} catch (Exception) {
						// What do?
					}
					continue;

				case 'p':
					if (!ns.IsPrismatic())
						continue;
					ns = new(ns, prismatic: false);
					break;

				case 'P':
					if (ns.IsPrismatic())
						continue;
					ns = new(ns, prismatic: true);
					break;

				case 'r':
				case 'R':
					color = CommonHelper.ParseColor(ReadSubString(text, i, out ni));
					i = ni;
					if (ns.BackgroundColor == color)
						continue;
					ns = new(ns, font: ns.Font, color: ns.Color, backgroundColor: color, shadowColor: ns.ShadowColor, scale: ns.Scale);
					break;

				case 's':
					if (!ns.IsStrikethrough())
						continue;
					ns = new(ns, strikethrough: false);
					break;

				case 'S':
					if (ns.IsStrikethrough())
						continue;
					ns = new(ns, strikethrough: true);
					break;

				case 't':
				case 'T':
					string? name = ReadSubString(text, i, out ni);
					i = ni;
					SpriteFont? font = null;
					if (!string.IsNullOrEmpty(name)) {
						if (name.StartsWith("dialog", StringComparison.InvariantCultureIgnoreCase))
							font = Game1.dialogueFont;
						else if (name.Equals("small", StringComparison.InvariantCultureIgnoreCase))
							font = Game1.smallFont;
						else if (name.Equals("tiny", StringComparison.InvariantCultureIgnoreCase))
							font = Game1.tinyFont;
					}
					if (ns.Font == font)
						continue;
					ns = new(ns, font: font, color: ns.Color, backgroundColor: ns.BackgroundColor, shadowColor: ns.ShadowColor, scale: ns.Scale);
					break;

				case 'u':
					if (!ns.IsUnderline())
						continue;
					ns = new(ns, underline: false);
					break;

				case 'U':
					if (ns.IsUnderline())
						continue;
					ns = new(ns, underline: true);
					break;

				default:
					builder.Append(chr);
					continue;
			}

			if (builder.Length > 0) {
				nodes.Add(
					new TextNode(
						builder.ToString(),
						style: s,
						align: a,
						onClick: onClick,
						onHover: onHover,
						onRightClick: onRightClick,
						noComponent: noComponent,
						extra: extra,
						id: id
					)
				);

				id = null;

				builder.Clear();
			}

			if (item != null) {
				nodes.Add(
					new SpriteNode(
						SpriteHelper.GetSprite(item),
						2f * (s.Scale ?? 1f),
						align: na,
						onClick: onClick,
						onHover: onHover,
						onRightClick: onRightClick,
						noComponent: noComponent,
						extra: item,
						id: id
					)
				);

				id = null;

				if (itemLabel)
					nodes.Add(new TextNode(
						$" {item.DisplayName}",
						style: ns,
						align: na,
						onClick: onClick,
						onHover: onHover,
						onRightClick: onRightClick,
						noComponent: noComponent,
						extra: item,
						id: id
					));

				item = null;
			}

			s = ns;
			a = na;
		}

		if (builder.Length > 0)
			nodes.Add(
				new TextNode(
					builder.ToString(),
					style: s,
					align: a,
					onClick: onClick,
					onHover: onHover,
					onRightClick: onRightClick,
					noComponent: noComponent,
					extra: extra,
					id: id
				)
			);

		endStyle = s;
		endAlign = a;

		return nodes;
	}


	public static IEnumerable<IFlowNode> Translate(
		Translation source,
		object values,
		TextStyle? style = null,
		Alignment? align = null,
		Func<IFlowNodeSlice, int, int, bool>? onClick = null,
		Func<IFlowNodeSlice, int, int, bool>? onHover = null,
		Func<IFlowNodeSlice, int, int, bool>? onRightClick = null,
		bool noComponent = false,
		object? extra = null,
		string? id = null
	) {
		IDictionary<string, object?> vals = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

		if (values is IDictionary dictionary) {
			foreach (DictionaryEntry entry in dictionary) {
				string? key = entry.Key?.ToString()?.Trim();
				if (key != null)
					vals[key] = entry.Value;
			}
		} else {
			Type type = values.GetType();
			foreach (PropertyInfo prop in type.GetProperties())
				vals[prop.Name] = prop.GetValue(values);
			foreach (FieldInfo field in type.GetFields())
				vals[field.Name] = field.GetValue(values);
		}

		return Translate(
			source: source,
			values: vals,
			style: style,
			align: align,
			onClick: onClick,
			onHover: onHover,
			onRightClick: onRightClick,
			noComponent: noComponent,
			extra: extra,
			id: id
		);
	}

	public static IEnumerable<IFlowNode> Translate(
		Translation source,
		IDictionary<string, object?> values,
		TextStyle? style = null,
		Alignment? align = null,
		Func<IFlowNodeSlice, int, int, bool>? onClick = null,
		Func<IFlowNodeSlice, int, int, bool>? onHover = null,
		Func<IFlowNodeSlice, int, int, bool>? onRightClick = null,
		bool noComponent = false,
		object? extra = null,
		string? id = null
	) {
		string val = source.ToString();
		if (!source.HasValue())
			return new IFlowNode[] {
				new TextNode(val, style, align, onClick, onHover, onRightClick, noComponent)
			};

		string[] bits = I18N_REPLACER.Split(val);
		List<IFlowNode> nodes = new();

		bool replacement = false;

		TextStyle s = style ?? TextStyle.EMPTY;
		Alignment a = align ?? Alignment.None;

		foreach (string bit in bits) {
			if (replacement && !string.IsNullOrWhiteSpace(bit)) {
				if (values.TryGetValue(bit, out object? node)) {
					if (node is NestedNode nested && nested.Nodes != null)
						nodes.AddRange(nested.Nodes);
					else if (node is IFlowNode n)
						nodes.Add(n);
					else if (node is IEnumerable<IFlowNode> nlist)
						nodes.AddRange(nlist);
					else {
						var nd = GetNode(node, a, s);
						if (nd != null)
							nodes.Add(nd);
					}
				}

			} else if (!string.IsNullOrEmpty(bit))
				nodes.AddRange(FormatText(
					text: bit,
					out s,
					out a,
					style: s,
					align: a,
					onClick: onClick,
					onHover: onHover,
					onRightClick: onRightClick,
					noComponent: noComponent,
					extra: extra,
					id: id
				));

			replacement = !replacement;
		}

		return nodes;
	}


	public static FlowBuilder Builder() {
		return new FlowBuilder();
	}

	private static float GetYOffset(Alignment alignment, float height, float containerHeight) {
		if (alignment.HasFlag(Alignment.Bottom))
			return (float) Math.Floor(containerHeight - height);
		if (alignment.HasFlag(Alignment.VCenter))
			return (float) Math.Floor((containerHeight - height) / 2f);

		return 0;
	}

	private static float GetXOffset(Alignment alignment, IFlowNodeSlice slice, CachedFlowLine line, float scale, float pos, float maxWidth) {
		bool found = false;
		float remaining = 0;

		bool can_center = true;
		bool can_right = true;

		foreach(var s in line.Slices) {
			if (slice == s)
				found = true;

			if (!found) {
				if (s.Node.Alignment.HasFlag(Alignment.HCenter))
					can_center = false;
				if (s.Node.Alignment.HasFlag(Alignment.Right)) {
					can_right = false;
					can_center = false;
				}

			} else
				remaining += s.Width * scale;
		}

		float before = maxWidth - remaining - pos;

		if (alignment.HasFlag(Alignment.Right))
			return can_right ? (float) Math.Floor(before) : 0;

		if (alignment.HasFlag(Alignment.HCenter))
			return can_center ? (float) Math.Floor(before / 2f) : 0;

		return 0;
	}

	private static SpriteFont GetDefaultFont() {
		return Game1.smallFont;
	}

	public static CachedFlow CalculateFlow(IEnumerable<IFlowNode> nodes, float maxWidth = -1, SpriteFont? defaultFont = null) {
		SpriteFont font = defaultFont ?? GetDefaultFont();
		List<CachedFlowLine> lines = new();

		// Space Dimensions
		// float spaceWidth = font.MeasureString("A B").X - font.MeasureString("AB").X;

		// Boundary
		float bound = maxWidth < 0 ? float.PositiveInfinity : maxWidth;

		// Overall Dimensions
		float width = 0;
		float height = 0;

		// Current Line
		float lineWidth = 0;
		float lineHeight = 0;
		List<IFlowNodeSlice> cached = new();

		bool forceNew = false;

		if (nodes is not IFlowNode[] nodesArray)
			nodesArray = nodes.ToArray();

		for(int i = 0; i < nodesArray.Length; i++) {
			// Make sure the segment has content, or skip it.
			IFlowNode node = nodesArray[i];
			if (node == null || node.IsEmpty())
				continue;

			IFlowNodeSlice? last = null;

			IFlowNodeSlice? nextSlice;
			if (i + 1 < nodesArray.Length)
				nextSlice = nodesArray[i + 1].Slice(null, font, 0f, 0f, null);
			else
				nextSlice = null;

			while (true) {
				IFlowNodeSlice? slice = node.Slice(last, font, bound, bound - lineWidth, nextSlice);
				if (slice == null)
					break;

				last = slice;
				WrapMode mode = slice.ForceWrap;

				// Do we need to do a line wrap?
				if (mode.HasFlag(WrapMode.ForceBefore) || forceNew || (cached.Count > 0 && lineWidth + slice.Width >= bound)) {
					lines.Add(new CachedFlowLine(cached.ToArray(), width: lineWidth, height: lineHeight));
					width = Math.Max(lineWidth, width);
					height += lineHeight;
					lineWidth = lineHeight = 0;
					cached.Clear();
				}

				forceNew = mode.HasFlag(WrapMode.ForceAfter);
				cached.Add(slice);
				lineWidth += (float) Math.Ceiling(slice.Width);
				lineHeight = Math.Max((float) Math.Ceiling(slice.Height), lineHeight);
			}
		}

		// Add the remaining line to the output.
		if (cached.Count > 0) {
			lines.Add(new CachedFlowLine(cached.ToArray(), width: lineWidth, height: lineHeight));
			width = Math.Max(lineWidth, width);
			height += lineHeight;
		}

		return new CachedFlow(
			nodes: nodesArray,
			lines: lines.ToArray(),
			height: height,
			width: width,
			font: font,
			maxWidth: maxWidth
		);
	}

	public static CachedFlow CalculateFlow(CachedFlow flow, float maxWidth = -1, SpriteFont? defaultFont = null) {
		defaultFont ??= GetDefaultFont();
		if (flow.IsCached(defaultFont, maxWidth))
			return flow;

		return CalculateFlow(flow.Nodes, maxWidth, defaultFont);
	}

	public static IFlowNodeSlice? GetSliceAtPoint(CachedFlow flow, int x, int y, float scale = 1, int lineOffset = 0, float maxHeight = -1) {
		return GetSliceAtPoint(
			flow: flow,
			x: x,
			y: y,
			relativeX: out _,
			relativeY: out _,
			scale: scale,
			lineOffset: lineOffset,
			maxHeight: maxHeight
		);
	}

	public static IFlowNodeSlice? GetSliceAtPoint(CachedFlow flow, int x, int y, out int relativeX, out int relativeY, float scale = 1, int lineOffset = 0, float maxHeight = -1) {
		float startX = 0;
		float startY = 0;

		foreach (CachedFlowLine line in flow.Lines) {
			if (lineOffset > 0) {
				lineOffset--;
				continue;
			}

			float lHeight = line.Height * scale;
			if (maxHeight >= 0 && startY + lHeight > maxHeight)
				break;

			foreach (IFlowNodeSlice slice in line.Slices) {
				if (slice?.IsEmpty() ?? true)
					continue;

				IFlowNode node = slice.Node;

				float offsetY = GetYOffset(node.Alignment, slice.Height * scale, line.Height * scale);
				float offsetX = GetXOffset(node.Alignment, slice, line, scale, startX, Math.Max(flow.Width * scale, flow.MaxWidth));

				float sX = startX + offsetX;
				float endX = sX + slice.Width * scale;
				float sY = startY + offsetY;
				float endY = sY + slice.Height * scale;

				if (x >= sX && x <= endX && y >= sY && y <= endY) {
					relativeX = x - (int) sX;
					relativeY = y - (int) sY;

					return slice;
				}

				startX = (float) Math.Ceiling(endX);
				if (x < startX)
					break;
			}

			startX = 0;
			startY += (float) Math.Ceiling(lHeight);

			if (y < startY)
				break;
		}

		relativeX = -1;
		relativeY = -1;

		return null;
	}

	public static IFlowNodeSlice? GetSliceAtPoint(CachedFlow flow, int x, int y, float scale = 1, float scrollOffset = 0, float maxHeight = -1) {
		return GetSliceAtPoint(
			flow: flow,
			x: x,
			y: y,
			relativeX: out _,
			relativeY: out _,
			scale: scale,
			scrollOffset: scrollOffset,
			maxHeight: maxHeight
		);
	}

	public static IFlowNodeSlice? GetSliceAtPoint(CachedFlow flow, int x, int y, out int relativeX, out int relativeY, float scale = 1, float scrollOffset = 0, float maxHeight = -1) {
		float startX = 0;
		float startY = 0;

		foreach (CachedFlowLine line in flow.Lines) {
			float lHeight = line.Height * scale;

			if (lHeight < scrollOffset) {
				scrollOffset -= lHeight;
				continue;
			} else if (scrollOffset != 0) {
				startY -= scrollOffset;
				scrollOffset = 0;
			}

			if (maxHeight >= 0 && startY >= maxHeight)
				break;

			foreach (IFlowNodeSlice slice in line.Slices) {
				if (slice?.IsEmpty() ?? true)
					continue;

				IFlowNode node = slice.Node;

				float offsetY = GetYOffset(node.Alignment, slice.Height * scale, line.Height * scale);
				float offsetX = GetXOffset(node.Alignment, slice, line, scale, startX, Math.Max(flow.Width * scale, flow.MaxWidth));

				float sX = startX + offsetX;
				float endX = sX + slice.Width * scale;
				float sY = startY + offsetY;
				float endY = sY + slice.Height * scale;

				if (x >= sX && x <= endX && y >= sY && y <= endY) {
					relativeX = x - (int) sX;
					relativeY = y - (int) sY;

					return slice;
				}

				startX = (float) Math.Ceiling(endX);
				if (x < startX)
					break;
			}

			startX = 0;
			startY += (float) Math.Ceiling(lHeight);

			if (y < startY)
				break;
		}

		relativeX = -1;
		relativeY = -1;

		return null;
	}


	public static int GetMaximumScrollOffset(CachedFlow flow, int height, int step = 1) {
		float remaining = height;

		for (int i = flow.Lines.Length - 1; i >= 0; i--) {
			remaining -= flow.Lines[i].Height;
			if (remaining < 0) {
				i++;
				return i + (i % step);
			}
		}

		return 0;
	}

	public static float GetScrollOffsetForUniqueNode(CachedFlow flow, string? id, float scale = 1f) {
		float y = 0;
		if (string.IsNullOrEmpty(id))
			return -1;

		foreach(CachedFlowLine line in flow.Lines) {
			float lHeight = line.Height * scale;

			foreach (IFlowNodeSlice slice in line.Slices) {
				if (slice.Node.UniqueId == id)
					return y;
			}

			y += (float) Math.Ceiling(lHeight);
		}

		return -1;
	}

	public static (IFlowNode, float)? GetClosestUniqueNode(CachedFlow flow, float scale = 1, float scrollOffset = 0f) {
		float y = -scrollOffset;

		IFlowNode? node = null;
		float offset = 0;
		float distance = float.MaxValue;

		foreach (CachedFlowLine line in flow.Lines) {
			float lHeight = line.Height * scale;

			float dist = Math.Abs(y);
			if (dist < distance) {
				foreach (IFlowNodeSlice slice in line.Slices) {
					if (!string.IsNullOrEmpty(slice.Node.UniqueId)) {
						node = slice.Node;
						distance = dist;
						offset = y;
						break;
					}
				}
			} else
				break;

			y += (float) Math.Ceiling(lHeight);
		}

		if (node == null)
			return null;

		return (node, offset);
	}

	// Smooth Scroll

	public static object? GetExtra(CachedFlow flow, int x, int y, float scale = 1, float scrollOffset = 0f, float maxHeight = -1) {
		IFlowNodeSlice? slice = GetSliceAtPoint(flow, x, y, scale, scrollOffset, maxHeight);
		return slice?.Node.Extra;
	}
	public static bool ClickFlow(CachedFlow flow, int x, int y, float scale = 1, float scrollOffset = 0f, float maxHeight = -1) {
		IFlowNodeSlice? slice = GetSliceAtPoint(flow, x, y, scale, scrollOffset, maxHeight);
		return slice?.Node.OnClick?.Invoke(slice, x, y) ?? false;
	}

	public static bool HoverFlow(CachedFlow flow, int x, int y, float scale = 1, float scrollOffset = 0f, float maxHeight = -1) {
		IFlowNodeSlice? slice = GetSliceAtPoint(flow, x, y, scale, scrollOffset, maxHeight);
		return slice?.Node.OnHover?.Invoke(slice, x, y) ?? false;
	}

	public static bool RightClickFlow(CachedFlow flow, int x, int y, float scale = 1, float scrollOffset = 0f, float maxHeight = -1) {
		IFlowNodeSlice? slice = GetSliceAtPoint(flow, x, y, scale, scrollOffset, maxHeight);
		return slice?.Node.OnRightClick?.Invoke(slice, x, y) ?? false;
	}

	public static void UpdateComponentsForFlow(CachedFlow flow, List<ClickableComponent> components, int offsetX, int offsetY, float scale = 1, float scrollOffset = 0f, float maxHeight = -1, Action<ClickableComponent>? onCreate = null, Action<ClickableComponent>? onDestroy = null, int startID = 99000) {
		float x = 0;
		float y = 0;

		int i = -1;

		bool done = false;

		foreach (CachedFlowLine line in flow.Lines) {
			float lHeight = line.Height * scale;

			if (lHeight < scrollOffset) { 
				foreach (IFlowNodeSlice slice in line.Slices) {
					if (slice == null || slice.IsEmpty())
						continue;

					IFlowNode node = slice.Node;
					ClickableComponent? cmp = node.UseComponent(slice);
					if (cmp != null)
						cmp.visible = false;
				}

				scrollOffset -= (float) Math.Ceiling(lHeight);
				continue;

			} else if (scrollOffset != 0) {
				y -= scrollOffset;
				scrollOffset = 0;
			}

			if (maxHeight >= 0 && y >= maxHeight) {
				done = true;
				break;
			}

			foreach (IFlowNodeSlice slice in line.Slices) {
				if (slice == null || slice.IsEmpty())
					continue;

				IFlowNode node = slice.Node;
				if (done) {
					ClickableComponent? cmp = node.UseComponent(slice);
					if (cmp != null)
						cmp.visible = false;
					continue;
				}

				float sHeight = slice.Height * scale;
				float sWidth = slice.Width * scale;

				float offX = GetXOffset(node.Alignment, slice, line, scale, x, Math.Max(flow.Width * scale, flow.MaxWidth));

				if (node.WantComponent(slice) ?? (node.OnHover != null || node.OnClick != null || node.OnRightClick != null)) { 
					float offY = GetYOffset(node.Alignment, sHeight, lHeight);

					// Get a component.
					ClickableComponent? cmp = node.UseComponent(slice);
					bool used = cmp != null;
					if (cmp != null) {
						cmp.visible = true;
					} else {
						i++;
						if (i >= components.Count) {
							cmp = new(Rectangle.Empty, (string?) null) {
								myID = startID + i,
								upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
								downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
								leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
								rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
							};

							components.Add(cmp);
							onCreate?.Invoke(cmp);
						} else
							cmp = components[i];
					}

					int cHeight = (int) sHeight;
					if (maxHeight >= 0 && y + sHeight > maxHeight)
						cHeight -= (int) ((y + sHeight) - maxHeight);

					bool clip_top = !used && y < 0;

					if (clip_top)
						cHeight += (int) y;

					cmp.bounds = new Rectangle(
						(int) (offsetX + x + offX),
						(int) (offsetY + (clip_top ? 0 : y) + offY),
						(int) sWidth,
						cHeight
					);
				}

				x += offX + (float) Math.Ceiling(sWidth);
			}

			x = 0;
			y += (float) Math.Ceiling(lHeight);
		}

		// Remove excess components.
		while (components.Count > i + 1) {
			ClickableComponent last = components[^1];
			onDestroy?.Invoke(last);
			components.Remove(last);
		}
	}

	public static void RenderFlow(SpriteBatch batch, CachedFlow flow, Vector2 position, Color? defaultColor = null, Color? defaultShadowColor = null, float scale = 1, float scrollOffset = 0f, float maxHeight = -1) {
		float x = 0;
		float y = 0;

#if DEBUG
		bool debugDraw = Game1.oldKBState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F3);
		int cidx = 0;
		IFlowNode? lastNode = null;
		int nidx = 0;
#endif

		foreach (CachedFlowLine line in flow.Lines) {
			float height = line.Height * scale;
			if (height < scrollOffset) {
				scrollOffset -= (float) Math.Ceiling(height);

#if DEBUG
				if (debugDraw) {
					foreach(IFlowNodeSlice slice in line.Slices) {
						if (slice == null || slice.IsEmpty())
							continue;

						if (slice.Node != lastNode) {
							lastNode = slice.Node;
							nidx++;
						}

						cidx = (cidx + 1) % DEBUG_COLORS.Length;
					}
				}
#endif

				continue;
			} else if (scrollOffset != 0) {
				y -= scrollOffset;
				scrollOffset = 0;
			}

			if (maxHeight >= 0 && y >= maxHeight)
				break;

			foreach (IFlowNodeSlice slice in line.Slices) {
				if (slice == null || slice.IsEmpty())
					continue;

				IFlowNode node = slice.Node;
				float offsetY = GetYOffset(node.Alignment, slice.Height * scale, line.Height * scale);
				float offsetX = GetXOffset(node.Alignment, slice, line, scale, x, Math.Max(flow.Width * scale, flow.MaxWidth));

				node.Draw(
					slice,
					batch,
					new Vector2(
						position.X + x + offsetX,
						position.Y + y + offsetY
					),
					scale,
					flow.Font,
					defaultColor,
					defaultShadowColor,
					line,
					flow
				);

#if DEBUG
				if (debugDraw) {
					if (node != lastNode) {
						lastNode = node;
						nidx++;
					}

					RenderHelper.DrawBox(
						batch,
						texture: Game1.mouseCursors,
						sourceRect: new Rectangle(379, 357, 3, 3),
						x: (int) (position.X + x + offsetX),
						y: (int) (position.Y + y + offsetY),
						width: (int) (slice.Width * scale),
						height: (int) (slice.Height * scale),
						color: DEBUG_COLORS[cidx],
						scale: 2f,
						drawShadow: false
					);

					Utility.drawTinyDigits(nidx, batch, new Vector2(
						position.X + x + offsetX,
						position.Y + y + offsetY
					), 2f, 1f, DEBUG_COLORS[cidx]);

					cidx = (cidx + 1) % DEBUG_COLORS.Length;
				}
#endif

				x += offsetX + (float) Math.Ceiling(slice.Width * scale);
			}

			x = 0;
			y += (float) Math.Ceiling(height);
		}
	}

	public static CachedFlow RenderFlow(SpriteBatch batch, CachedFlow flow, Vector2 position, float maxWidth = -1, SpriteFont? defaultFont = null, Color? defaultColor = null, Color? defaultShadowColor = null, float scale = 1, float scrollOffset = 0, float maxHeight = -1) {
		CachedFlow result = CalculateFlow(flow, maxWidth, defaultFont);
		RenderFlow(batch, result, position, defaultColor, defaultShadowColor, scale, scrollOffset, maxHeight);
		return result;
	}

	public static CachedFlow RenderFlow(SpriteBatch batch, IEnumerable<IFlowNode> nodes, Vector2 position, float maxWidth = -1, SpriteFont? defaultFont = null, Color? defaultColor = null, Color? defaultShadowColor = null, float scale = 1, float scrollOffset = 0f, float maxHeight = -1) {
		CachedFlow result = CalculateFlow(nodes, maxWidth, defaultFont);
		RenderFlow(batch, result, position, defaultColor, defaultShadowColor, scale, scrollOffset, maxHeight);
		return result;
	}

	// Line Scroll

	public static object? GetExtra(CachedFlow flow, int x, int y, float scale = 1, int lineOffset = 0, float maxHeight = -1) {
		IFlowNodeSlice? slice = GetSliceAtPoint(flow, x, y, scale, lineOffset, maxHeight);
		return slice?.Node.Extra;
	}

	public static bool ClickFlow(CachedFlow flow, int x, int y, float scale = 1, int lineOffset = 0, float maxHeight = -1) {
		IFlowNodeSlice? slice = GetSliceAtPoint(flow, x, y, scale, lineOffset, maxHeight);
		return slice?.Node.OnClick?.Invoke(slice, x, y) ?? false;
	}

	public static bool HoverFlow(CachedFlow flow, int x, int y, float scale = 1, int lineOffset = 0, float maxHeight = -1) {
		IFlowNodeSlice? slice = GetSliceAtPoint(flow, x, y, scale, lineOffset, maxHeight);
		return slice?.Node.OnHover?.Invoke(slice, x, y) ?? false;
	}

	public static bool RightClickFlow(CachedFlow flow, int x, int y, float scale = 1, int lineOffset = 0, float maxHeight = -1) {
		IFlowNodeSlice? slice = GetSliceAtPoint(flow, x, y, scale, lineOffset, maxHeight);
		return slice?.Node.OnRightClick?.Invoke(slice, x, y) ?? false;
	}

	public static void UpdateComponentsForFlow(CachedFlow flow, List<ClickableComponent> components, int offsetX, int offsetY, float scale = 1, int lineOffset = 0, float maxHeight = -1, Action<ClickableComponent>? onCreate = null, Action<ClickableComponent>? onDestroy = null, int startID = 99000) {
		float x = 0;
		float y = 0;

		int i = -1;

		bool done = false;

		foreach (CachedFlowLine line in flow.Lines) {
			if (lineOffset > 0) {
				foreach(IFlowNodeSlice slice in line.Slices) {
					if (slice == null || slice.IsEmpty())
						continue;

					IFlowNode node = slice.Node;
					ClickableComponent? cmp = node.UseComponent(slice);
					if (cmp != null)
						cmp.visible = false;
				}

				lineOffset--;
				continue;
			}

			float lHeight = line.Height * scale;
			if (maxHeight >= 0 && y + lHeight > maxHeight) {
				done = true;
				break;
			}

			foreach (IFlowNodeSlice slice in line.Slices) {
				if (slice == null || slice.IsEmpty())
					continue;

				IFlowNode node = slice.Node;
				if (done) {
					ClickableComponent? cmp = node.UseComponent(slice);
					if (cmp != null)
						cmp.visible = false;
					continue;
				}

				float sHeight = slice.Height * scale;
				float sWidth = slice.Width * scale;

				float offX = GetXOffset(node.Alignment, slice, line, scale, x, Math.Max(flow.Width * scale, flow.MaxWidth));

				if (node.WantComponent(slice) ?? (node.OnHover != null || node.OnClick != null || node.OnRightClick != null)) { 
					float offY = GetYOffset(node.Alignment, sHeight, lHeight);

					// Get a component.
					ClickableComponent? cmp = node.UseComponent(slice);
					if (cmp != null) { 
						cmp.visible = true;
					} else {
						i++;
						if (i >= components.Count) {
							cmp = new(Rectangle.Empty, (string?) null) {
								myID = startID + i,
								upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
								downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
								leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
								rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
							};

							components.Add(cmp);
							onCreate?.Invoke(cmp);
						} else
							cmp = components[i];
					}

					cmp.bounds = new Rectangle(
						(int) (offsetX + x + offX),
						(int) (offsetY + y + offY),
						(int) sWidth,
						(int) sHeight
					);
				}

				x += offX + (float) Math.Ceiling(sWidth);
			}

			x = 0;
			y += (float) Math.Ceiling(lHeight);
		}

		// Remove excess components.
		while (components.Count > i + 1) {
			ClickableComponent last = components[^1];
			onDestroy?.Invoke(last);
			components.Remove(last);
		}
	}

	public static void RenderFlow(SpriteBatch batch, CachedFlow flow, Vector2 position, Color? defaultColor = null, Color? defaultShadowColor = null, float scale = 1, int lineOffset = 0, float maxHeight = -1) {
		float x = 0;
		float y = 0;

		foreach (CachedFlowLine line in flow.Lines) {
			if (lineOffset > 0) {
				lineOffset--;
				continue;
			}

			float height = line.Height * scale;
			if (maxHeight >= 0 && y + height > maxHeight)
				break;

			foreach (IFlowNodeSlice slice in line.Slices) {
				if (slice == null || slice.IsEmpty())
					continue;

				IFlowNode node = slice.Node;
				float offsetY = GetYOffset(node.Alignment, slice.Height * scale, line.Height * scale);
				float offsetX = GetXOffset(node.Alignment, slice, line, scale, x, Math.Max(flow.Width * scale, flow.MaxWidth));

				node.Draw(
					slice,
					batch,
					new Vector2(
						position.X + x + offsetX,
						position.Y + y + offsetY
					),
					scale,
					flow.Font,
					defaultColor,
					defaultShadowColor,
					line,
					flow
				);

				x += offsetX + (float) Math.Ceiling(slice.Width * scale);
			}

			x = 0;
			y += (float) Math.Ceiling(height);
		}
	}

	public static CachedFlow RenderFlow(SpriteBatch batch, CachedFlow flow, Vector2 position, float maxWidth = -1, SpriteFont? defaultFont = null, Color? defaultColor = null, Color? defaultShadowColor = null, float scale = 1, int lineOffset = 0, float maxHeight = -1) {
		CachedFlow result = CalculateFlow(flow, maxWidth, defaultFont);
		RenderFlow(batch, result, position, defaultColor, defaultShadowColor, scale, lineOffset, maxHeight);
		return result;
	}

	public static CachedFlow RenderFlow(SpriteBatch batch, IEnumerable<IFlowNode> nodes, Vector2 position, float maxWidth = -1, SpriteFont? defaultFont = null, Color? defaultColor = null, Color? defaultShadowColor = null, float scale = 1, int lineOffset = 0, float maxHeight = -1) {
		CachedFlow result = CalculateFlow(nodes, maxWidth, defaultFont);
		RenderFlow(batch, result, position, defaultColor, defaultShadowColor, scale, lineOffset, maxHeight);
		return result;
	}
}
