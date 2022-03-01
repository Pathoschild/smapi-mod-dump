/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using Leclair.Stardew.Common.UI.FlowNode;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.Common.UI {

	public class FlowBuilder {
		private FlowBuilder Parent;
		private NestedNode? Node;
		private List<IFlowNode> Nodes;
		private IFlowNode[] Built;

		public FlowBuilder(FlowBuilder parent = null, NestedNode? node = null) {
			Parent = parent;
			Node = node;
		}

		private void AssertState() {
			if (Built != null) throw new ArgumentException("cannot modify built flow");
			if (Nodes == null) Nodes = new();
		}


		public FlowBuilder Add(IFlowNode node) {
			AssertState();
			Nodes.Add(node);
			return this;
		}

		public FlowBuilder AddRange(IEnumerable<IFlowNode> nodes) {
			AssertState();
			Nodes.AddRange(nodes);
			return this;
		}

		public FlowBuilder Sprite(SpriteInfo sprite, float scale, Alignment? alignment = null, Func<IFlowNodeSlice, bool> onClick = null, Func<IFlowNodeSlice, bool> onHover = null, bool noComponent = false, float size = 16, int frame = -1) {
			AssertState();
			Nodes.Add(new SpriteNode(sprite, scale, alignment, onClick, onHover, noComponent, size, frame));
			return this;
		}

		public FlowBuilder FormatText(string text, TextStyle style, Alignment? alignment = null, Func<IFlowNodeSlice, bool> onClick = null, Func<IFlowNodeSlice, bool> onHover = null, bool noComponent = false) {
			AssertState();
			Nodes.AddRange(FlowHelper.FormatText(text, style, alignment, onClick, onHover, noComponent));
			return this;
		}

		public FlowBuilder Text(string text, TextStyle style, Alignment? alignment = null, Func<IFlowNodeSlice, bool> onClick = null, Func<IFlowNodeSlice, bool> onHover = null, bool noComponent = false) {
			AssertState();
			Nodes.Add(new TextNode(text, style, alignment, onClick, onHover, noComponent));
			return this;
		}

		public FlowBuilder Translate(Translation source, object values, TextStyle? style = null, Alignment? alignment = null) {
			AssertState();
			Nodes.AddRange(FlowHelper.Translate(source, values, style, alignment));
			return this;
		}

		public FlowBuilder FormatText(string text, Color? color = null, bool? prismatic = null, SpriteFont font = null, bool? fancy = null, bool? bold = null, bool? shadow = null, Color? shadowColor = null, bool? strikethrough = null, bool? underline = null, float? scale = null, Alignment? align = null, Func<IFlowNodeSlice, bool> onClick = null, Func<IFlowNodeSlice, bool> onHover = null, bool noComponent = false) {
			TextStyle style = new TextStyle(
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

			return FormatText(text, style, align, onClick, onHover, noComponent);
		}

		public FlowBuilder Text(string text, Color? color = null, bool? prismatic = null, SpriteFont font = null, bool? fancy = null, bool? bold = null, bool? shadow = null, Color? shadowColor = null, bool? strikethrough = null, bool? underline = null, float? scale = null, Alignment? align = null, Func<IFlowNodeSlice, bool> onClick = null, Func<IFlowNodeSlice, bool> onHover = null, bool noComponent = false) {
			TextStyle style = new TextStyle(
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

			return Text(text, style, align, onClick, onHover, noComponent);
		}

		public FlowBuilder Group(Alignment? align = null, Func<IFlowNodeSlice, bool> onClick = null, Func<IFlowNodeSlice, bool> onHover = null, bool noComponent = false) {
			AssertState();
			NestedNode nested = new NestedNode(null, align, onClick, onHover, noComponent);
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
				Parent.ReplaceNode(Node.Value, new NestedNode(BuildThis(), alignment: Node.Value.Alignment));

			return Parent ?? this;
		}

		public IFlowNode[] Build() {
			return Parent?.Build() ?? BuildThis();
		}

		public IFlowNode[] BuildThis() {
			if (Built != null) return Built;
			Built = Nodes?.ToArray() ?? new IFlowNode[0];
			Nodes = null;
			return Built;
		}
	}

	public static class FlowHelper {

		public static Regex I18N_REPLACER = new("{{([ \\w\\.\\-]+)}}");

		public static IFlowNode GetNode(object obj) {
			if (obj == null)
				return null;
			if (obj is IFlowNode node)
				return node;
			if (obj is IFlowNode[] nodes)
				return new NestedNode(nodes);
			if (obj is SpriteInfo sprite)
				return new SpriteNode(sprite, 2f);

			return new TextNode(obj.ToString());
		}

		private static string ReadSubString(string text, int i, out int end) {
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

		private static Color? ParseColor(string input) {
			if (string.IsNullOrEmpty(input))
				return null;

			System.Drawing.Color color;
			try {
				color = System.Drawing.ColorTranslator.FromHtml(input);
			} catch(Exception) {
				return null;
			}

			return new Color(color.R, color.G, color.B, color.A);
		}

		public static IEnumerable<IFlowNode> FormatText(string text, TextStyle? style = null, Alignment? alignment = null, Func<IFlowNodeSlice, bool> onClick = null, Func<IFlowNodeSlice, bool> onHover = null, bool noComponent = false) {
			return FormatText(
				text: text,
				out TextStyle _,
				out Alignment __,
				style: style,
				alignment: alignment,
				onClick: onClick,
				onHover: onHover,
				noComponent: noComponent
			);
		}

		public static IEnumerable<IFlowNode> FormatText(string text, out TextStyle endStyle, out Alignment endAlignment, TextStyle? style = null, Alignment? alignment = null, Func<IFlowNodeSlice, bool> onClick = null, Func<IFlowNodeSlice, bool> onHover = null, bool noComponent = false) {
			if (string.IsNullOrEmpty(text) || !text.Contains('@')) {
				endStyle = style ?? TextStyle.EMPTY;
				endAlignment = alignment ?? Alignment.None;

				return new IFlowNode[]{
					new TextNode(text, style, alignment, onClick, onHover, noComponent)
				};
			}

			List<IFlowNode> nodes = new();

			TextStyle s = style ?? TextStyle.EMPTY;
			Alignment a = alignment ?? Alignment.None;

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
					case '^':
						if (na == Alignment.Top)
							continue;
						na = Alignment.Top;
						break;

					case 'v':
						if (na == Alignment.Bottom)
							continue;
						na = Alignment.Bottom;
						break;

					case '-':
						if (na == Alignment.Middle)
							continue;
						na = Alignment.Middle;
						break;

					// Style
					case '_':
						string size = ReadSubString(text, i, out ni);
						i = ni;
						float? scale = null;
						if (float.TryParse(size, out float sp))
							scale = sp;
						if (ns.Scale == scale)
							continue;
						ns = new(ns, font: ns.Font, color: ns.Color, shadowColor: ns.ShadowColor, scale: scale);
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
						color = ParseColor(ReadSubString(text, i, out ni));
						i = ni;
						if (ns.ShadowColor == color)
							continue;
						ns = new(ns, font: ns.Font, color: ns.Color, shadowColor: color, scale: ns.Scale);
						break;

					case 'C':
						color = ParseColor(ReadSubString(text, i, out ni));
						i = ni;
						if (ns.Color == color)
							continue;
						ns = new(ns, font: ns.Font, color: color, shadowColor: ns.ShadowColor, scale: ns.Scale);
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
						if (ns.Font == null)
							continue;
						ns = new(ns, font: null, color: ns.Color, shadowColor: ns.ShadowColor, scale: ns.Scale);
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
							alignment: a,
							onClick: onClick,
							onHover: onHover,
							noComponent: noComponent
						)
					);

					builder.Clear();
				}

				s = ns;
				a = na;
			}

			if (builder.Length > 0)
				nodes.Add(
					new TextNode(
						builder.ToString(),
						style: s,
						alignment: a,
						onClick: onClick,
						onHover: onHover,
						noComponent: noComponent
					)
				);

			endStyle = s;
			endAlignment = a;

			return nodes;
		}


		public static IEnumerable<IFlowNode> Translate(Translation source, object values, TextStyle? style = null, Alignment? alignment = null, Func<IFlowNodeSlice, bool> onClick = null, Func<IFlowNodeSlice, bool> onHover = null, bool noComponent = false) {
			IDictionary<string, IFlowNode> vals = new Dictionary<string, IFlowNode>(StringComparer.OrdinalIgnoreCase);

			if (values is IDictionary dictionary) {
				foreach (DictionaryEntry entry in dictionary) {
					string key = entry.Key?.ToString().Trim();
					if (key != null)
						vals[key] = GetNode(entry.Value);
				}
			} else {
				Type type = values.GetType();
				foreach (PropertyInfo prop in type.GetProperties())
					vals[prop.Name] = GetNode(prop.GetValue(values));
				foreach (FieldInfo field in type.GetFields())
					vals[field.Name] = GetNode(field.GetValue(values));
			}

			return Translate(source, vals, style, alignment, onClick, onHover, noComponent);
		}

		public static IEnumerable<IFlowNode> Translate(Translation source, IDictionary<string, IFlowNode> values, TextStyle? style = null, Alignment? alignment = null, Func<IFlowNodeSlice, bool> onClick = null, Func<IFlowNodeSlice, bool> onHover = null, bool noComponent = false) {
			string val = source.ToString();
			if (!source.HasValue())
				return new IFlowNode[] {
					new TextNode(val, style, alignment, onClick, onHover, noComponent)
				};

			string[] bits = I18N_REPLACER.Split(val);
			List<IFlowNode> nodes = new();

			bool replacement = false;

			TextStyle s = style ?? TextStyle.EMPTY;
			Alignment a = alignment ?? Alignment.None;

			foreach (string bit in bits) {
				if (replacement && !string.IsNullOrWhiteSpace(bit)) {
					if (values.TryGetValue(bit, out IFlowNode node))
						nodes.Add(node);

				} else if (!string.IsNullOrEmpty(bit))
					nodes.AddRange(FormatText(
						text: bit,
						out s,
						out a,
						style: s,
						alignment: a,
						onClick: onClick,
						onHover: onHover,
						noComponent: noComponent
					));

				replacement = !replacement;
			}

			return nodes;
		}


		public static FlowBuilder Builder() {
			return new FlowBuilder();
		}

		private static float GetYOffset(Alignment alignment, float height, float containerHeight) {
			return alignment switch {
				Alignment.Bottom => containerHeight - height,
				Alignment.Middle => (containerHeight - height) / 2,
				_ => 0,
			};
		}

		private static SpriteFont GetDefaultFont() {
			return Game1.smallFont;
		}

		public static CachedFlow CalculateFlow(IEnumerable<IFlowNode> nodes, float maxWidth = -1, SpriteFont defaultFont = null) {
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

			foreach (IFlowNode node in nodes) {
				// Make sure the segment has content, or skip it.
				if (node == null || node.IsEmpty())
					continue;

				IFlowNodeSlice last = null;

				while (true) {
					IFlowNodeSlice slice = node.Slice(last, font, bound, bound - lineWidth);
					if (slice == null)
						break;

					last = slice;
					WrapMode mode = slice.ForceWrap;

					// Do we need to do a line wrap?
					if (mode == WrapMode.ForceBefore || forceNew || (cached.Count > 0 && lineWidth + slice.Width >= bound)) {
						lines.Add(new CachedFlowLine(cached.ToArray(), width: lineWidth, height: lineHeight));
						width = Math.Max(lineWidth, width);
						height += lineHeight;
						lineWidth = lineHeight = 0;
						cached.Clear();
					}

					forceNew = mode == WrapMode.ForceAfter;
					cached.Add(slice);
					lineWidth += slice.Width;
					lineHeight = Math.Max(slice.Height, lineHeight);
				}
			}

			// Add the remaining line to the output.
			if (cached.Count > 0) {
				lines.Add(new CachedFlowLine(cached.ToArray(), width: lineWidth, height: lineHeight));
				width = Math.Max(lineWidth, width);
				height += lineHeight;
			}

			return new CachedFlow(
				nodes: nodes.ToArray(),
				lines: lines.ToArray(),
				height: height,
				width: width,
				font: font,
				maxWidth: maxWidth
			);
		}

		public static CachedFlow CalculateFlow(CachedFlow flow, float maxWidth = -1, SpriteFont defaultFont = null) {
			defaultFont ??= GetDefaultFont();
			if (flow.IsCached(defaultFont, maxWidth))
				return flow;

			return CalculateFlow(flow.Nodes, maxWidth, defaultFont);
		}


		public static IFlowNodeSlice GetSliceAtPoint(CachedFlow flow, int x, int y, float scale = 1, int lineOffset = 0, float maxHeight = -1) {
			float startX = 0;
			float startY = 0;

			foreach (CachedFlowLine line in flow.Lines) {
				if (lineOffset > 0) {
					lineOffset--;
					continue;
				}

				foreach (IFlowNodeSlice slice in line.Slices) {
					if (slice?.IsEmpty() ?? true)
						continue;

					IFlowNode node = slice.Node;

					float offsetY = GetYOffset(node.Alignment, slice.Height * scale, line.Height * scale);

					float endX = startX + slice.Width * scale;
					float sY = startY + offsetY;
					float endY = startY + slice.Height * scale;

					if (x >= startX && x <= endX && y >= sY && y <= endY)
						return slice;

					startX = endX;
					if (x < startX)
						break;
				}

				startX = 0;
				startY += line.Height * scale;

				if (y < startY || (maxHeight >= 0 && startY >= maxHeight))
					break;
			}

			return null;
		}


		public static bool ClickFlow(CachedFlow flow, int x, int y, float scale = 1, int lineOffset = 0, float maxHeight = -1) {
			IFlowNodeSlice slice = GetSliceAtPoint(flow, x, y, scale, lineOffset, maxHeight);
			return slice?.Node.OnClick?.Invoke(slice) ?? false;
		}

		public static bool HoverFlow(CachedFlow flow, int x, int y, float scale = 1, int lineOffset = 0, float maxHeight = -1) {
			IFlowNodeSlice slice = GetSliceAtPoint(flow, x, y, scale, lineOffset, maxHeight);
			return slice?.Node.OnHover?.Invoke(slice) ?? false;
		}

		public static void UpdateComponentsForFlow(CachedFlow flow, List<ClickableComponent> components, int offsetX, int offsetY, float scale = 1, int lineOffset = 0, float maxHeight = -1, Action<ClickableComponent> onCreate = null, Action<ClickableComponent> onDestroy = null) {
			float x = 0;
			float y = 0;

			int i = -1;

			foreach (CachedFlowLine line in flow.Lines) {
				if (lineOffset > 0) {
					lineOffset--;
					continue;
				}

				foreach (IFlowNodeSlice slice in line.Slices) {
					if (slice == null || slice.IsEmpty())
						continue;

					IFlowNode node = slice.Node;
					float width = slice.Width * scale;

					if ((node.OnHover != null || node.OnClick != null) && !node.NoComponent) {
						float offY = GetYOffset(node.Alignment, slice.Height * scale, line.Height * scale);

						// Get a component.
						ClickableComponent cmp;
						i++;
						if (i >= components.Count) {
							cmp = new(Rectangle.Empty, (string) null) {
								myID = 99000 + i,
								upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
								downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
								leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
								rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
							};

							components.Add(cmp);
							onCreate?.Invoke(cmp);
						} else
							cmp = components[i];

						cmp.bounds = new Rectangle(
							(int) (offsetX + x),
							(int) (offsetY + y + offY),
							(int) width,
							(int) (slice.Height * scale)
						);
					}

					x += width;
				}

				x = 0;
				y += line.Height * scale;
				if (maxHeight >= 0 && y >= maxHeight)
					break;
			}

			// Remove excess components.
			while (components.Count > i + 1) {
				ClickableComponent last = components[components.Count - 1];
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

				foreach (IFlowNodeSlice slice in line.Slices) {
					if (slice == null || slice.IsEmpty())
						continue;

					IFlowNode node = slice.Node;
					float offsetY = GetYOffset(node.Alignment, slice.Height * scale, line.Height * scale);

					node.Draw(slice, batch, new Vector2(position.X + x, position.Y + y + offsetY), scale, flow.Font, defaultColor, defaultShadowColor, line, flow);

					x += slice.Width * scale;
				}

				x = 0;
				y += line.Height * scale;

				if (maxHeight >= 0 && y >= maxHeight)
					break;
			}
		}

		public static CachedFlow RenderFlow(SpriteBatch batch, CachedFlow flow, Vector2 position, float maxWidth = -1, SpriteFont defaultFont = null, Color? defaultColor = null, Color? defaultShadowColor = null, float scale = 1, int lineOffset = 0, float maxHeight = -1) {
			CachedFlow result = CalculateFlow(flow, maxWidth, defaultFont);
			RenderFlow(batch, result, position, defaultColor, defaultShadowColor, scale, lineOffset, maxHeight);
			return result;
		}

		public static CachedFlow RenderFlow(SpriteBatch batch, IEnumerable<IFlowNode> nodes, Vector2 position, float maxWidth = -1, SpriteFont defaultFont = null, Color? defaultColor = null, Color? defaultShadowColor = null, float scale = 1, int lineOffset = 0, float maxHeight = -1) {
			CachedFlow result = CalculateFlow(nodes, maxWidth, defaultFont);
			RenderFlow(batch, result, position, defaultColor, defaultShadowColor, scale, lineOffset, maxHeight);
			return result;
		}
	}
}
