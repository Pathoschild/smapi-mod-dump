/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Configuration;
using SpriteMaster.Core;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SpriteMaster.Harmonize.Patches.PSpriteBatch.Patch;

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

	// This is here because the M1 seems to have issues with reverse patches.
	internal static readonly ThreadLocal<bool> IsReverse = new(false);

	[Harmonize("Draw", priority: Harmonize.PriorityLevel.First)]
	public static bool OnDrawFirst(
		XSpriteBatch __instance,
		ref XTexture2D? texture,
		ref XRectangle destinationRectangle,
		ref XRectangle? sourceRectangle,
		XColor color,
		float rotation,
		ref XVector2 origin,
		ref SpriteEffects effects,
		float layerDepth,
		ref ManagedTexture2D? __state
	) {
		if (!Config.IsEnabled) {
			return true;
		}

		if (IsReverse.Value) {
			return true;
		}

		return __instance.OnDrawFirst(
			texture: ref texture,
			destination: ref destinationRectangle,
			source: ref sourceRectangle,
			color: color,
			rotation: rotation,
			origin: ref origin,
			effects: ref effects,
			layerDepth: layerDepth,
			__state: ref __state
		);
	}

	[Harmonize("Draw", priority: Harmonize.PriorityLevel.Last)]
	public static bool OnDrawLast(
		XSpriteBatch __instance,
		ref XTexture2D? texture,
		ref XRectangle destinationRectangle,
		ref XRectangle? sourceRectangle,
		ref XColor color,
		float rotation,
		ref XVector2 origin,
		ref SpriteEffects effects,
		ref float layerDepth,
		ref ManagedTexture2D? __state
	) {
		if (!Config.IsEnabled) {
			return true;
		}

		if (IsReverse.Value) {
			return true;
		}

		return __instance.OnDraw(
			texture: ref texture,
			destination: ref destinationRectangle,
			source: ref sourceRectangle,
			color: ref color,
			rotation: rotation,
			origin: ref origin,
			effects: ref effects,
			layerDepth: ref layerDepth,
			__state: ref __state
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static bool ForwardDraw(
		XSpriteBatch @this,
		XTexture2D? texture,
		XRectangle destinationRectangle,
		XColor color,
		XRectangle? sourceRectangle = null,
		float rotation = 0f,
		XVector2? origin = null,
		SpriteEffects effects = SpriteEffects.None,
		float layerDepth = 0f
	) {
		if (!Config.IsEnabled) {
			return true;
		}

		if (IsReverse.Value) {
			return true;
		}

		@this.Draw(
			texture: texture,
			destinationRectangle: destinationRectangle,
			sourceRectangle: sourceRectangle,
			color: color,
			rotation: rotation,
			origin: origin ?? XVector2.Zero,
			effects: effects,
			layerDepth: layerDepth
		);

		return false;
	}

	[Harmonize("Draw", priority: Harmonize.PriorityLevel.First)]
	public static bool OnDraw(XSpriteBatch __instance, XTexture2D? texture, XRectangle destinationRectangle, XRectangle? sourceRectangle, XColor color) {
		return ForwardDraw(
			@this: __instance,
			texture: texture,
			destinationRectangle: destinationRectangle,
			sourceRectangle: sourceRectangle,
			color: color
		);
	}

	[Harmonize("Draw", priority: Harmonize.PriorityLevel.First)]
	public static bool OnDraw(XSpriteBatch __instance, XTexture2D? texture, XRectangle destinationRectangle, XColor color) {
		return ForwardDraw(
			@this: __instance,
			texture: texture,
			destinationRectangle: destinationRectangle,
			color: color
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static bool ForwardDraw(
		XSpriteBatch @this,
		XTexture2D? texture,
		XVector2 position,
		XColor color,
		XRectangle? sourceRectangle = null,
		float rotation = 0f,
		XVector2? origin = null,
		XVector2? scale = null,
		SpriteEffects effects = SpriteEffects.None,
		float layerDepth = 0f
	) {
		if (!Config.IsEnabled) {
			return true;
		}

		if (IsReverse.Value) {
			return true;
		}

		@this.Draw(
			texture: texture,
			position: position,
			sourceRectangle: sourceRectangle,
			color: color,
			rotation: rotation,
			origin: origin ?? XVector2.Zero,
			scale: scale ?? XVector2.One,
			effects: effects,
			layerDepth: layerDepth
		);

		return false;
	}

	[Harmonize("Draw", priority: Harmonize.PriorityLevel.Last)]
	public static bool OnDraw(XSpriteBatch __instance, ref XTexture2D? texture, ref XVector2 position, ref XRectangle? sourceRectangle, ref XColor color, float rotation, ref XVector2 origin, ref XVector2 scale, SpriteEffects effects, float layerDepth) {
		if (!Config.IsEnabled) {
			return true;
		}

		if (IsReverse.Value) {
			return true;
		}

		return __instance.OnDraw(
			texture: ref texture,
			position: ref position,
			source: ref sourceRectangle,
			color: ref color,
			rotation: rotation,
			origin: ref origin,
			scale: ref scale,
			effects: effects,
			layerDepth: ref layerDepth
		);
	}

	[Harmonize("Draw", priority: Harmonize.PriorityLevel.First)]
	public static bool OnDraw(XSpriteBatch __instance, XTexture2D? texture, XVector2 position, XRectangle? sourceRectangle, XColor color, float rotation, XVector2 origin, float scale, SpriteEffects effects, float layerDepth) {
		return ForwardDraw(
			@this: __instance,
			texture: texture,
			position: position,
			sourceRectangle: sourceRectangle,
			color: color,
			rotation: rotation,
			origin: origin,
			scale: new XVector2(scale),
			effects: effects,
			layerDepth: layerDepth
		);
	}

	[Harmonize("Draw", priority: Harmonize.PriorityLevel.First)]
	public static bool OnDraw(XSpriteBatch __instance, XTexture2D? texture, XVector2 position, XRectangle? sourceRectangle, XColor color) {
		return ForwardDraw(
			@this: __instance,
			texture: texture,
			position: position,
			sourceRectangle: sourceRectangle,
			color: color
		);
	}

	[Harmonize("Draw", priority: Harmonize.PriorityLevel.First)]
	public static bool OnDraw(XSpriteBatch __instance, XTexture2D? texture, XVector2 position, XColor color) {
		return ForwardDraw(
			@this: __instance,
			texture: texture,
			position: position,
			color: color
		);
	}
}
