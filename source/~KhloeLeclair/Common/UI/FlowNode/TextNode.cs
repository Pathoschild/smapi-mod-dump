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
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.BellsAndWhistles;

namespace Leclair.Stardew.Common.UI.FlowNode {
	public struct TextNode : IFlowNode {

		public static char[] SEPARATORS = new char[] {
            // Whitespace
            ' ', '\t',

            // New Line
            '\n',

            // Other Stuff
            ',', '.'
		};

		public static char[] NOWRAP_SEPARATORS = new char[] {
			'\n'
		};

		public string Text { get; }
		public TextStyle Style { get; }

		public Alignment Alignment { get; }

		public bool NoComponent { get; }
		public Func<IFlowNodeSlice, bool> OnClick { get; }
		public Func<IFlowNodeSlice, bool> OnHover { get; }

		public TextNode(string text, TextStyle? style = null, Alignment? alignment = null, Func<IFlowNodeSlice, bool> onClick = null, Func<IFlowNodeSlice, bool> onHover = null, bool noComponent = false) {
			Text = text;
			Style = style ?? TextStyle.EMPTY;
			Alignment = alignment ?? Alignment.None;
			OnClick = onClick;
			OnHover = onHover;
			NoComponent = noComponent;
		}

		public bool IsEmpty() {
			return string.IsNullOrEmpty(Text);
		}

		public static Vector2 GetSpriteTextSize(string text) {
			return new Vector2(SpriteText.getWidthOfString(text), SpriteText.getHeightOfString(text));
		}

		public IFlowNodeSlice Slice(IFlowNodeSlice last, SpriteFont defaultFont, float maxWidth, float remaining) {
			int start = 0;
			if (last is TextSlice tslice)
				start = tslice.End;

			// If we're at the end, we're done.
			if (start >= Text.Length)
				return null;

			char[] seps = SEPARATORS;
			bool is_sprite = Style.IsFancy() || Style.IsJunimo();
			SpriteFont font = Style.Font ?? defaultFont;
			float scale = Style.Scale ?? 1f;
			Vector2 spaceSize = is_sprite ? GetSpriteTextSize(" ") : (font.MeasureString(" ") * scale);

			string pending = null;
			int pendingEnd = -1;
			bool pendingSpace = false;
			Vector2 pendingSize = Vector2.Zero;

			bool had_new = false;

			for (int idx = Text.IndexOfAny(seps, start); idx != -1; idx = Text.IndexOfAny(seps, idx + 1)) {
				// Check the character we found.
				char found = Text[idx];
				bool newLine = found == '\n';
				bool space = found == ' ' || found == '\t';
				bool include = !(newLine || space);

				// Alright, let's see what we've got.
				int end = idx + 1;
				int length = (idx + (include ? 1 : 0)) - start;

				string snippet = Text.Substring(start, length);
				Vector2 size = is_sprite ? GetSpriteTextSize(snippet) : (font.MeasureString(snippet) * scale);

				// Does this fit on the line?
				if (!had_new && (size.X + (space ? spaceSize.X : 0)) < remaining) {
					pending = snippet;
					pendingSpace = space;
					pendingSize = size;
					pendingEnd = end;
					had_new = newLine;
					continue;
				}

				// Do we have a pending string that fits?
				if (pending != null)
					return new TextSlice(this, pending, start, pendingEnd, pendingSize.X + (pendingSpace ? spaceSize.X : 0), Math.Max(spaceSize.Y, pendingSize.Y), had_new ? WrapMode.ForceAfter : WrapMode.None);

				// We didn't have pending. That means we can't fit anything on the
				// existing line. What about fitting content on a new line?
				if (remaining < maxWidth) {
					remaining = maxWidth;

					// Does it fit now?
					if (!had_new && size.X < remaining) {
						pending = snippet;
						pendingSize = size;
						pendingEnd = end;
						had_new = newLine;
						continue;
					}
				}

				// Still doesn't fit. Just give up and return this bit.
				if (snippet.Length > 1)
					return new TextSlice(this, snippet, start, end, size.X, Math.Max(spaceSize.Y, size.Y), had_new ? WrapMode.ForceAfter : WrapMode.None);
			}

			// We ran out of separators.
			// Can the rest of the string fit?

			string final = Text.Substring(start);
			int offset = 0;
			if (had_new && final.EndsWith("\n")) {
				final = final.Substring(0, final.Length - 1);
				offset = 1;
			}

			Vector2 finalSize = is_sprite ? GetSpriteTextSize(final) : (font.MeasureString(final) * scale);

			if (finalSize.X > remaining && pending != null)
				return new TextSlice(this, pending, start, pendingEnd, pendingSize.X + (pendingSpace ? spaceSize.X : 0), Math.Max(spaceSize.Y, pendingSize.Y), had_new ? WrapMode.ForceAfter : WrapMode.None);

			// Just give up and send it.
			return new TextSlice(this, final, start, start + final.Length + offset, finalSize.X, Math.Max(finalSize.Y, spaceSize.Y), had_new ? WrapMode.ForceAfter : WrapMode.None);
		}

		public void Draw(IFlowNodeSlice slice, SpriteBatch batch, Vector2 position, float scale, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor, CachedFlowLine line, CachedFlow flow) {
			if (slice is not TextSlice tslice)
				return;

			float s = scale * (Style.Scale ?? 1f);
			SpriteFont font = Style.Font ?? defaultFont;
			Color color = Style.Color ?? defaultColor ?? Game1.textColor;
			Color? shadowColor = Style.ShadowColor ?? defaultShadowColor;
			if (Style.IsPrismatic())
				color = Utility.GetPrismaticColor();

			string text = tslice.Text;

			if (Style.IsJunimo())
				SpriteText.drawString(batch, text, (int) position.X, (int) position.Y, junimoText: true);
			else if (Style.IsFancy())
				SpriteText.drawString(batch, text, (int) position.X, (int) position.Y);
			else if (Style.IsBold())
				Utility.drawBoldText(batch, text, font, position, color, s);
			else if (Style.HasShadow()) {
				if (shadowColor.HasValue)
					Utility.drawTextWithColoredShadow(batch, text, font, position, color, shadowColor.Value, s);
				else
					Utility.drawTextWithShadow(batch, text, font, position, color, s);
			} else
				batch.DrawString(font, text, position, color, 0f, Vector2.Zero, s, SpriteEffects.None, GUIHelper.GetLayerDepth(position.Y));

			if (Style.IsStrikethrough())
				Utility.drawLineWithScreenCoordinates(
					(int) position.X,
					(int) (position.Y + tslice.Height / 2),
					(int) (position.X + tslice.Width),
					(int) (position.Y + tslice.Height / 2),
					batch,
					color
				);

			if (Style.IsUnderline())
				Utility.drawLineWithScreenCoordinates(
					(int) position.X,
					(int) (position.Y + tslice.Height * 3/4),
					(int) (position.X + tslice.Width),
					(int) (position.Y + tslice.Height * 3/4),
					batch,
					color
				);

			// TODO: Strike and Underline
		}

		public override bool Equals(object obj) {
			return obj is TextNode node &&
				   Text == node.Text &&
				   EqualityComparer<TextStyle>.Default.Equals(Style, node.Style) &&
				   Alignment == node.Alignment &&
				   NoComponent == node.NoComponent &&
				   EqualityComparer<Func<IFlowNodeSlice, bool>>.Default.Equals(OnClick, node.OnClick) &&
				   EqualityComparer<Func<IFlowNodeSlice, bool>>.Default.Equals(OnHover, node.OnHover);
		}

		public override int GetHashCode() {
			int hashCode = 735414917;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Text);
			hashCode = hashCode * -1521134295 + Style.GetHashCode();
			hashCode = hashCode * -1521134295 + Alignment.GetHashCode();
			hashCode = hashCode * -1521134295 + NoComponent.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<Func<IFlowNodeSlice, bool>>.Default.GetHashCode(OnClick);
			hashCode = hashCode * -1521134295 + EqualityComparer<Func<IFlowNodeSlice, bool>>.Default.GetHashCode(OnHover);
			return hashCode;
		}
	}

	public struct TextSlice : IFlowNodeSlice {
		public IFlowNode Node { get; }

		public string Text { get; }
		public int Start { get; }
		public int End { get; }

		public float Width { get; }
		public float Height { get; }
		public WrapMode ForceWrap { get; }

		public TextSlice(IFlowNode node, string text, int start, int end, float width, float height, WrapMode forceWrap) {
			Node = node;
			Text = text;
			Start = start;
			End = end;
			Width = width;
			Height = height;
			ForceWrap = forceWrap;
		}

		public bool IsEmpty() {
			return string.IsNullOrEmpty(Text);
		}

		public override bool Equals(object obj) {
			return obj is TextSlice slice &&
				   EqualityComparer<IFlowNode>.Default.Equals(Node, slice.Node) &&
				   Text == slice.Text &&
				   Start == slice.Start &&
				   End == slice.End &&
				   Width == slice.Width &&
				   Height == slice.Height &&
				   ForceWrap == slice.ForceWrap;
		}

		public override int GetHashCode() {
			int hashCode = 837092661;
			hashCode = hashCode * -1521134295 + EqualityComparer<IFlowNode>.Default.GetHashCode(Node);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Text);
			hashCode = hashCode * -1521134295 + Start.GetHashCode();
			hashCode = hashCode * -1521134295 + End.GetHashCode();
			hashCode = hashCode * -1521134295 + Width.GetHashCode();
			hashCode = hashCode * -1521134295 + Height.GetHashCode();
			hashCode = hashCode * -1521134295 + ForceWrap.GetHashCode();
			return hashCode;
		}
	}
}
