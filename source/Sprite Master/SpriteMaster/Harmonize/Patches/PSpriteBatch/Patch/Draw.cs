using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Harmonize.Patches.PSpriteBatch.Patch {
	[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
	internal static class Draw {
		/*
		 * All patches that have fewer arguments than the two primary .Draw methods are forwarded to the ones with more arguments, since we will override those arguments.
		 * This also means that they must be actually FIRST so we can effectively prevent other mods/overrides from altering their arguments, since when they call .Draw again,
		 * those mods would then alter the arguments _again_, causing issues.
		 * 
		 * Previously, the logic would be like this:
		 * Draw -> OTHERMOD.Draw -> SpriteMaster.Draw -> DrawMoreArguments -> OTHERMOD.DrawMoreArguments -> SpriteMaster.DrawMoreArguments
		 * 
		 * It is now:
		 * 
		 * Draw -> SpriteMaster.Draw -> DrawMoreArguments -> OTHERMOD.DrawMoreArguments -> SpriteMaster.DrawMoreArguments
		 * 
		 */

		[Harmonize("Draw", priority: Harmonize.PriorityLevel.First)]
		internal static bool OnDrawFirst (
			SpriteBatch __instance,
			ref Texture2D texture,
			ref Rectangle destinationRectangle,
			ref Rectangle? sourceRectangle,
			Color color,
			float rotation,
			ref Vector2 origin,
			SpriteEffects effects,
			float layerDepth
		) {
			if (!Config.Enabled)
				return true;

			return __instance.OnDrawFirst(
				texture: ref texture,
				destination: ref destinationRectangle,
				source: ref sourceRectangle,
				color: color,
				rotation: rotation,
				origin: ref origin,
				effects: effects,
				layerDepth: layerDepth
			);
		}

		[Harmonize("Draw", priority: Harmonize.PriorityLevel.Last)]
		internal static bool OnDrawLast (
			SpriteBatch __instance,
			ref Texture2D texture,
			ref Rectangle destinationRectangle,
			ref Rectangle? sourceRectangle,
			Color color,
			float rotation,
			ref Vector2 origin,
			SpriteEffects effects,
			ref float layerDepth
		) {
			if (!Config.Enabled)
				return true;

			return __instance.OnDraw(
				texture: ref texture,
				destination: ref destinationRectangle,
				source: ref sourceRectangle,
				color: color,
				rotation: rotation,
				origin: ref origin,
				effects: effects,
				layerDepth: ref layerDepth
			);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool ForwardDraw (
			SpriteBatch @this,
			Texture2D texture,
			Rectangle destinationRectangle,
			Color color,
			Rectangle? sourceRectangle = null,
			float rotation = 0f,
			Vector2? origin = null,
			SpriteEffects effects = SpriteEffects.None,
			float layerDepth = 0f
		) {
			if (!Config.Enabled)
				return true;

			@this.Draw(
				texture: texture,
				destinationRectangle: destinationRectangle,
				sourceRectangle: sourceRectangle,
				color: color,
				rotation: rotation,
				origin: origin ?? Vector2.Zero,
				effects: effects,
				layerDepth: layerDepth
			);

			return false;
		}

		[Harmonize("Draw", priority: Harmonize.PriorityLevel.First)]
		internal static bool OnDraw (SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color) {
			return ForwardDraw(
				@this: __instance,
				texture: texture,
				destinationRectangle: destinationRectangle,
				sourceRectangle: sourceRectangle,
				color: color
			);
		}

		[Harmonize("Draw", priority: Harmonize.PriorityLevel.First)]
		internal static bool OnDraw (SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Color color) {
			return ForwardDraw(
				@this: __instance,
				texture: texture,
				destinationRectangle: destinationRectangle,
				color: color
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool ForwardDraw (
			SpriteBatch @this,
			Texture2D texture,
			Vector2 position,
			Color color,
			Rectangle? sourceRectangle = null,
			float rotation = 0f,
			Vector2? origin = null,
			Vector2? scale = null,
			SpriteEffects effects = SpriteEffects.None,
			float layerDepth = 0f
		) {
			if (!Config.Enabled)
				return true;

			@this.Draw(
				texture: texture,
				position: position,
				sourceRectangle: sourceRectangle,
				color: color,
				rotation: rotation,
				origin: origin ?? Vector2.Zero,
				scale: scale ?? Vector2.One,
				effects: effects,
				layerDepth: layerDepth
			);

			return false;
		}


		[Harmonize("Draw", priority: Harmonize.PriorityLevel.Last)]
		internal static bool OnDraw (SpriteBatch __instance, ref Texture2D texture, ref Vector2 position, ref Rectangle? sourceRectangle, Color color, float rotation, ref Vector2 origin, ref Vector2 scale, SpriteEffects effects, float layerDepth) {
			if (!Config.Enabled)
				return true;

			return __instance.OnDraw(
				texture: ref texture,
				position: ref position,
				source: ref sourceRectangle,
				color: color,
				rotation: rotation,
				origin: ref origin,
				scale: ref scale,
				effects: effects,
				layerDepth: ref layerDepth
			);
		}

		[Harmonize("Draw", priority: Harmonize.PriorityLevel.First)]
		internal static bool OnDraw (SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth) {
			return ForwardDraw(
				@this: __instance,
				texture: texture,
				position: position,
				sourceRectangle: sourceRectangle,
				color: color,
				rotation: rotation,
				origin: origin,
				scale: new Vector2(scale),
				effects: effects,
				layerDepth: layerDepth
			);
		}

		[Harmonize("Draw", priority: Harmonize.PriorityLevel.First)]
		internal static bool OnDraw (SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color) {
			return ForwardDraw(
				@this: __instance,
				texture: texture,
				position: position,
				sourceRectangle: sourceRectangle,
				color: color
			);
		}

		[Harmonize("Draw", priority: Harmonize.PriorityLevel.First)]
		internal static bool OnDraw (SpriteBatch __instance, Texture2D texture, Vector2 position, Color color) {
			return ForwardDraw(
				@this: __instance,
				texture: texture,
				position: position,
				color: color
			);
		}
	}
}
