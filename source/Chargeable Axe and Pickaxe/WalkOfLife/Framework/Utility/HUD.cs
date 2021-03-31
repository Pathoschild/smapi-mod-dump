/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using SUtility = StardewValley.Utility;

namespace TheLion.AwesomeProfessions
{
	/// <summary>Holds common methods and properties related to rendering elements to the game HUD.</summary>
	public static partial class Utility
	{
		public static ArrowPointer ArrowPointer { get; } = new();

		/// <summary>Draw a tracking arrow pointer on the edge of the screen pointing to a target off-screen.</summary>
		/// <param name="target">The target to point to.</param>
		/// <param name="color">The color of the pointer.</param>
		/// <remarks>Note that the game will add a yellow tinge to the color supplied here.</remarks>
		public static void DrawTrackingArrowPointer(Vector2 target, Color color)
		{
			if (SUtility.isOnScreen(target * 64f + new Vector2(32f, 32f), 64)) return;

			Rectangle vpbounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;
			Vector2 onScreenPosition = default;
			float rotation = 0f;
			if (target.X * 64f > Game1.viewport.MaxCorner.X - 64)
			{
				onScreenPosition.X = vpbounds.Right - 8;
				rotation = (float)Math.PI / 2f;
			}
			else if (target.X * 64f < Game1.viewport.X)
			{
				onScreenPosition.X = 8f;
				rotation = -(float)Math.PI / 2f;
			}
			else
				onScreenPosition.X = target.X * 64f - Game1.viewport.X;

			if (target.Y * 64f > Game1.viewport.MaxCorner.Y - 64)
			{
				onScreenPosition.Y = vpbounds.Bottom - 8;
				rotation = (float)Math.PI;
			}
			else if (target.Y * 64f < Game1.viewport.Y)
				onScreenPosition.Y = 8f;
			else
				onScreenPosition.Y = target.Y * 64f - Game1.viewport.Y;

			if (onScreenPosition.X == 8f && onScreenPosition.Y == 8f)
				rotation += (float)Math.PI / 4f;

			if (onScreenPosition.X == 8f && onScreenPosition.Y == vpbounds.Bottom - 8)
				rotation += (float)Math.PI / 4f;

			if (onScreenPosition.X == vpbounds.Right - 8 && onScreenPosition.Y == 8f)
				rotation -= (float)Math.PI / 4f;

			if (onScreenPosition.X == vpbounds.Right - 8 && onScreenPosition.Y == vpbounds.Bottom - 8)
				rotation -= (float)Math.PI / 4f;

			Rectangle srcRect = new Rectangle(0, 0, 5, 4);
			float renderScale = 4f;
			Vector2 safePos = SUtility.makeSafe(renderSize: new Vector2(srcRect.Width * renderScale, srcRect.Height * renderScale), renderPos: onScreenPosition);
			Game1.spriteBatch.Draw(ArrowPointer.Texture, safePos, srcRect, color, rotation, new Vector2(2f, 2f), renderScale, SpriteEffects.None, 1f);
		}

		/// <summary>Draw a tracking arrow pointer over a target on-screen.</summary>
		/// <param name="target">A target on the game location.</param>
		/// <param name="color">The color of the pointer.</param>
		/// <remarks>Note that the game will add a yellow tinge to the color supplied here. Credit to Bpendragon for this logic.</remarks>
		public static void DrawArrowPointerOverTarget(Vector2 target, Color color)
		{
			if (!SUtility.isOnScreen(target * 64f + new Vector2(32f, 32f), 64)) return;

			Rectangle srcRect = new Rectangle(0, 0, 5, 4);
			float renderScale = 4f;
			Vector2 targetPixel = new Vector2((target.X * 64f) + 32f, (target.Y * 64f) + 32f) + ArrowPointer.GetOffset();
			Vector2 adjustedPixel = Game1.GlobalToLocal(Game1.viewport, targetPixel);
			adjustedPixel = SUtility.ModifyCoordinatesForUIScale(adjustedPixel);
			Game1.spriteBatch.Draw(ArrowPointer.Texture, adjustedPixel, srcRect, color, (float)Math.PI, new Vector2(2f, 2f), renderScale, SpriteEffects.None, 1f);
		}
	}

	/// <summary>Vertical arrow indicator to reveal on-screen objects of interest for tracker professions.</summary>
	public class ArrowPointer
	{
		public Texture2D Texture { get; set; }
		private float _height = -42f, _step = 0f, _maxStep = 3f, _minStep = -3f, _jerk = 1f;

		/// <summary>Advance the pointer's vertical offset motion by one step, in a bobbing fashion.</summary>
		public void Bob()
		{
			if (_step == _maxStep || _step == _minStep) _jerk = -_jerk;

			_step += _jerk;
			_height += _step;
		}

		/// <summary>Get the pointer's current vertical offset.</summary>
		public Vector2 GetOffset()
		{
			return new Vector2(0f, _height);
		}
	}
}
