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
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace Leclair.Stardew.Common.UI.FlowNode;

public class TextNode : IFlowNode {

	public static readonly char[] SEPARATORS = new char[] {
            // Whitespace
            ' ', '\t',

            // New Line
            '\n',

            // Other Stuff
            ',', '.', '。', '，'
	};

	public static readonly char[] NOWRAP_SEPARATORS = new char[] {
		'\n'
	};

	public string Text { get; }

	private TextStyle _Style;
	public TextStyle Style {
		get => _Style;
		set {
			if (_Style.IsFancy() != value.IsFancy() || _Style.Scale != value.Scale || _Style.Font != value.Font || _Style.IsJunimo() != value.IsJunimo())
				throw new ArgumentException("unable to assign style that would change size of output");
			_Style = value;
		}
	}

	public Alignment Alignment { get; }
	public object? Extra { get; }
	public string? UniqueId { get; }

	public bool NoComponent { get; }
	public Func<IFlowNodeSlice, int, int, bool>? OnClick { get; }
	public Func<IFlowNodeSlice, int, int, bool>? OnHover { get; }
	public Func<IFlowNodeSlice, int, int, bool>? OnRightClick { get; }

	public TextNode(
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
		Text = text;
		_Style = style ?? TextStyle.EMPTY;
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

	public bool IsEmpty() {
		return string.IsNullOrEmpty(Text);
	}

	public static Vector2 GetSpriteTextSize(string text) {
		return new Vector2(SpriteText.getWidthOfString(text), SpriteText.getHeightOfString(text));
	}

	public IFlowNodeSlice? Slice(IFlowNodeSlice? last, SpriteFont defaultFont, float maxWidth, float remaining, IFlowNodeSlice? nextSlice) {
		// TODO: Rewrite all of this.
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

		string? pending = null;
		int pendingEnd = -1;
		bool pendingSpace = false;
		Vector2 pendingSize = Vector2.Zero;

		//bool starting_line = remaining == maxWidth;
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

			// This should not be the case?
			if (snippet.Length <= 1)
				continue;

			// This word doesn't fit at all. We need to break it up.
			var slice = TryPartial(font, length, start, remaining, spaceSize);
			if (slice is not null)
				return slice;

			// Still doesn't fit. Just give up and return this bit.
			if (snippet.Length > 1)
				return new TextSlice(this, snippet, start, end, size.X, Math.Max(spaceSize.Y, size.Y), had_new ? WrapMode.ForceAfter : WrapMode.None);
		}

		// We ran out of separators.
		// Can the rest of the string fit?

		string final = Text[start..];
		int offset = 0;
		if (had_new && final.EndsWith("\n")) {
			final = final[0..^1];
			offset = 1;
		}

		Vector2 finalSize = is_sprite ? GetSpriteTextSize(final) : (font.MeasureString(final) * scale);

		if (finalSize.X > remaining && pending != null)
			return new TextSlice(this, pending, start, pendingEnd, pendingSize.X + (pendingSpace ? spaceSize.X : 0), Math.Max(spaceSize.Y, pendingSize.Y), had_new ? WrapMode.ForceAfter : WrapMode.None);

		// If we're dealing with a no-separators-at-all situation, attempt to slice the word.
		var sliced = !had_new && start == 0 ? TryPartial(font, final.Length, start, remaining, spaceSize) : null;
		if (sliced is not null)
			return sliced;

		// Just give up and send it.
		return new TextSlice(this, final, start, start + final.Length + offset, finalSize.X, Math.Max(finalSize.Y, spaceSize.Y), had_new ? WrapMode.ForceAfter : WrapMode.None);
	}

	private TextSlice? TryPartial(SpriteFont font, int length, int start, float remaining, Vector2 spaceSize, WrapMode mode = WrapMode.None) {
		// This word doesn't fit at all. We need to break it up.
		int snipLength = 0;
		Vector2 snipSize = Vector2.Zero;

		while (snipLength <= length) {
			snipLength++;
			if (Text.Length < (start + snipLength)) {
				snipLength--;
				break;
			}

			var newSize = font.MeasureString(Text.Substring(start, snipLength));
			if (newSize.X <= remaining)
				snipSize = newSize;
			else {
				snipLength--;
				break;
			}
		}

		// Did we get some?
		if (snipLength > 0)
			return new TextSlice(this, Text.Substring(start, snipLength), start, start + snipLength, snipSize.X, Math.Max(spaceSize.Y, snipSize.Y), mode);

		return null;
	}

	public void Draw(IFlowNodeSlice slice, SpriteBatch batch, Vector2 position, float scale, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor, CachedFlowLine line, CachedFlow flow) {
		if (slice is not TextSlice tslice)
			return;

		float s = scale * (Style.Scale ?? 1f);
		SpriteFont font = Style.Font ?? defaultFont;
		Color? stColor = Style.Color ?? defaultColor;
		Color color = Style.Color ?? defaultColor ?? Game1.textColor;
		Color background = Style.BackgroundColor ?? Color.Transparent;
		Color? shadowColor = Style.ShadowColor ?? defaultShadowColor;
		if (Style.IsPrismatic())
			color = Utility.GetPrismaticColor();

		if (Style.Opacity.HasValue)
			color *= Style.Opacity.Value;

		string text = tslice.Text;

		if (Style.IsInverted())
			(color, background) = (background, color);

		if (background.A > 0) {
			float alpha = (float) background.A / 255f;

			batch.Draw(
				Game1.fadeToBlackRect,
				new Rectangle(
					(int) position.X, (int) position.Y,
					(int) slice.Width, (int) slice.Height
				),
				background * alpha
			);
		}

		if (Style.IsJunimo() || Style.IsFancy())
			SpriteText.drawString(
				batch,
				text,
				(int) position.X, (int) position.Y,
				junimoText: Style.IsJunimo(),
				color: stColor,
				alpha: Style.Opacity ?? 1
			);
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
				(int) (position.Y + tslice.Height * 3 / 4),
				(int) (position.X + tslice.Width),
				(int) (position.Y + tslice.Height * 3 / 4),
				batch,
				color
			);
	}

	public override bool Equals(object? obj) {
		return obj is TextNode node &&
			   Text == node.Text &&
			   EqualityComparer<TextStyle>.Default.Equals(Style, node.Style) &&
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
		hash.Add(Text);
		hash.Add(Style);
		hash.Add(Alignment);
		hash.Add(Extra);
		hash.Add(UniqueId);
		hash.Add(NoComponent);
		hash.Add(OnClick);
		hash.Add(OnHover);
		hash.Add(OnRightClick);
		return hash.ToHashCode();
	}

	public static bool operator ==(TextNode left, TextNode right) {
		return left.Equals(right);
	}

	public static bool operator !=(TextNode left, TextNode right) {
		return !(left == right);
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

	public override bool Equals(object? obj) {
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
		return HashCode.Combine(Node, Text, Start, End, Width, Height, ForceWrap);
	}

	public static bool operator ==(TextSlice left, TextSlice right) {
		return left.Equals(right);
	}

	public static bool operator !=(TextSlice left, TextSlice right) {
		return !(left == right);
	}
}

#endif
