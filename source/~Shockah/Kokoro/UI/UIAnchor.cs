/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using System;

namespace Shockah.Kokoro.UI
{
	public enum UIAnchorSide
	{
		TopLeft,
		Top,
		TopRight,
		Left,
		Center,
		Right,
		BottomLeft,
		Bottom,
		BottomRight
	}

	public readonly struct UIAnchor
	{
		public readonly UIAnchorSide From;
		public readonly Vector2 Offset;
		public readonly UIAnchorSide To;

		public UIAnchor(UIAnchorSide from, Vector2 offset, UIAnchorSide to)
		{
			this.From = from;
			this.Offset = offset;
			this.To = to;
		}

		public UIAnchor(UIAnchorSide from, float inset, Vector2 offset, UIAnchorSide to)
			: this(from, GetRealOffset(from, inset, offset), to) { }

		private static Vector2 GetRealOffset(UIAnchorSide side, float inset, Vector2 baseOffset)
		{
			var result = baseOffset;
			switch (side)
			{
				case UIAnchorSide.TopLeft:
					result.X += inset;
					result.Y += inset;
					break;
				case UIAnchorSide.TopRight:
					result.X -= inset;
					result.Y += inset;
					break;
				case UIAnchorSide.BottomLeft:
					result.X += inset;
					result.Y -= inset;
					break;
				case UIAnchorSide.BottomRight:
					result.X -= inset;
					result.Y -= inset;
					break;
				case UIAnchorSide.Top:
					result.Y += inset;
					break;
				case UIAnchorSide.Bottom:
					result.Y -= inset;
					break;
				case UIAnchorSide.Left:
					result.X += inset;
					break;
				case UIAnchorSide.Right:
					result.X -= inset;
					break;
				case UIAnchorSide.Center:
					break;
				default:
					throw new ArgumentException($"{nameof(UIAnchorSide)} has an invalid value.");
			}
			return result;
		}

		public Vector2 GetAnchoredPoint(Vector2 fromLocation, Vector2 fromSize, Vector2 toSize)
			=> From.GetAnchorPoint(fromLocation, fromSize) - To.GetAnchorPoint(Vector2.Zero, toSize) + Offset;
	}

	public static class UIAnchorSideExtensions
	{
		public static UIAnchorSide Opposite(this UIAnchorSide self)
		{
			return self switch
			{
				UIAnchorSide.TopLeft => UIAnchorSide.BottomRight,
				UIAnchorSide.TopRight => UIAnchorSide.BottomLeft,
				UIAnchorSide.BottomLeft => UIAnchorSide.TopRight,
				UIAnchorSide.BottomRight => UIAnchorSide.TopLeft,
				UIAnchorSide.Center => UIAnchorSide.Center,
				UIAnchorSide.Top => UIAnchorSide.Bottom,
				UIAnchorSide.Bottom => UIAnchorSide.Top,
				UIAnchorSide.Left => UIAnchorSide.Right,
				UIAnchorSide.Right => UIAnchorSide.Left,
				_ => throw new ArgumentException($"{nameof(UIAnchorSide)} has an invalid value."),
			};
		}

		public static Vector2 GetAnchorOffset(this UIAnchorSide self, Vector2 size)
			=> self.GetAnchorPoint(Vector2.Zero, size);

		public static Vector2 GetAnchorPoint(this UIAnchorSide self, Vector2 location, Vector2 size)
		{
			return self switch
			{
				UIAnchorSide.TopLeft => location,
				UIAnchorSide.TopRight => new(location.X + size.X, location.Y),
				UIAnchorSide.BottomLeft => new(location.X, location.Y + size.Y),
				UIAnchorSide.BottomRight => new(location.X + size.X, location.Y + size.Y),
				UIAnchorSide.Center => new(location.X + size.X * 0.5f, location.Y + size.Y * 0.5f),
				UIAnchorSide.Top => new(location.X + size.X * 0.5f, location.Y),
				UIAnchorSide.Bottom => new(location.X + size.X * 0.5f, location.Y + size.Y),
				UIAnchorSide.Left => new(location.X, location.Y + size.Y * 0.5f),
				UIAnchorSide.Right => new(location.X + size.X, location.Y + size.Y * 0.5f),
				_ => throw new ArgumentException($"{nameof(UIAnchorSide)} has an invalid value."),
			};
		}
	}
}
