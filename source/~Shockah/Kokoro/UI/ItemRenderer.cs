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
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using SObject = StardewValley.Object;

namespace Shockah.Kokoro.UI;

public class ItemRenderer
{
	private static RenderTarget2D? DrawInMenuRenderTarget { get; set; }

	private static void DrawItemViaGameImplementation(SpriteBatch b, SObject @object, Vector2 rectLocation, Vector2 rectSize, Color color, StackDrawType drawStackNumber, UIAnchorSide rectAnchorSide, float layerDepth)
	{
		Vector2 realRectSize = rectSize;
		Vector2 realRectLocation = rectLocation;

		if (realRectSize.X != realRectSize.Y)
		{
			var minLength = Math.Min(rectSize.X, rectSize.Y);
			realRectSize = new(minLength);
			realRectLocation -= (rectSize - realRectSize) / 2;
		}

		float scale = Math.Min(realRectSize.X, realRectSize.Y) / 64f;
		realRectLocation = rectAnchorSide.GetAnchorPoint(realRectLocation, realRectSize);
		@object.drawInMenu(b, realRectLocation, scale, 1f, layerDepth, drawStackNumber, color, drawShadow: true);
	}

	private static void DrawItemViaRenderTargetGameImplementation(SpriteBatch b, SObject @object, Vector2 rectLocation, Vector2 rectSize, Color color, StackDrawType drawStackNumber, UIAnchorSide rectAnchorSide, float layerDepth)
	{
		if (DrawInMenuRenderTarget is null || DrawInMenuRenderTarget.IsDisposed)
		{
			DrawInMenuRenderTarget?.Dispose();
			DrawInMenuRenderTarget = new RenderTarget2D(b.GraphicsDevice, 80, 80); // a bit bigger to fit the text
		}

		static bool TryEnd(SpriteBatch b)
		{
			try
			{
				b.End();
				return true;
			}
			catch
			{
				return false;
			}
		}

		bool wasInProgress = TryEnd(b);
		var oldRenderTarget = b.GraphicsDevice.GetRenderTargets().FirstOrNull()?.RenderTarget as RenderTarget2D;

		b.GraphicsDevice.SetRenderTarget(DrawInMenuRenderTarget);
		b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
		b.GraphicsDevice.Clear(Color.Transparent);

		DrawItemViaGameImplementation(b, @object, Vector2.Zero, new Vector2(64), Color.White, drawStackNumber, UIAnchorSide.TopLeft, 0f);

		b.End();
		b.GraphicsDevice.SetRenderTarget(oldRenderTarget);

		if (wasInProgress)
			b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

		var actualScale = Math.Min(rectSize.X, rectSize.Y) / 64f;
		b.Draw(DrawInMenuRenderTarget, rectLocation - rectAnchorSide.GetAnchorPoint(Vector2.Zero, new Vector2(Math.Min(rectSize.X, rectSize.Y))), null, color, 0f, Vector2.Zero, actualScale, SpriteEffects.None, layerDepth);
	}

	public void DrawItem(SpriteBatch b, SObject @object, Vector2 rectLocation, Vector2 rectSize, Color color, StackDrawType drawStackNumber, UIAnchorSide rectAnchorSide = UIAnchorSide.TopLeft, float layerDepth = 0f)
	{
		DrawItemViaRenderTargetGameImplementation(b, @object, rectLocation, rectSize, color, drawStackNumber, rectAnchorSide, layerDepth);
	}
}