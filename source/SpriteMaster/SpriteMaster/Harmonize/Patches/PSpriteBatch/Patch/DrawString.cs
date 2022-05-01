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
using System.Text;

namespace SpriteMaster.Harmonize.Patches.PSpriteBatch.Patch;

internal class DrawString {
	// public unsafe void DrawString (SpriteFont spriteFont, string text, Vector2 position, Color color)
	// public unsafe void DrawString (SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
	// public unsafe void DrawString (SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color)
	// public unsafe void DrawString (SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)

	[Harmonize("DrawString", priority: Harmonize.PriorityLevel.First)]
	public static bool OnDrawString(SpriteBatch __instance, SpriteFont spriteFont, string text, XNA.Vector2 position, XNA.Color color) {
		__instance.DrawString(
			spriteFont: spriteFont,
			text: text,
			position: position,
			color: color,
			rotation: 0.0f,
			origin: XNA.Vector2.Zero,
			scale: XNA.Vector2.One,
			effects: SpriteEffects.None,
			layerDepth: 0.0f
		);
		return false;
	}

	[Harmonize("DrawString", priority: Harmonize.PriorityLevel.First)]
	public static bool OnDrawString(SpriteBatch __instance, SpriteFont spriteFont, StringBuilder text, XNA.Vector2 position, XNA.Color color) {
		__instance.DrawString(
			spriteFont: spriteFont,
			text: text,
			position: position,
			color: color,
			rotation: 0.0f,
			origin: XNA.Vector2.Zero,
			scale: XNA.Vector2.One,
			effects: SpriteEffects.None,
			layerDepth: 0.0f
		);
		return false;
	}

	[Harmonize("DrawString", priority: Harmonize.PriorityLevel.Last)]
	public static bool OnDrawString(
		SpriteBatch __instance,
		SpriteFont spriteFont,
		string text,
		XNA.Vector2 position,
		XNA.Color color,
		float rotation,
		XNA.Vector2 origin,
		XNA.Vector2 scale,
		SpriteEffects effects,
		float layerDepth
	) {
		return Core.OnDrawStringImpl.DrawString(
			__instance,
			spriteFont,
			text,
			position,
			color,
			rotation,
			origin,
			scale,
			effects,
			layerDepth
		);
	}

	[Harmonize("DrawString", priority: Harmonize.PriorityLevel.Last)]
	public static bool OnDrawString(
		SpriteBatch __instance,
		SpriteFont spriteFont,
		StringBuilder text,
		XNA.Vector2 position,
		XNA.Color color,
		float rotation,
		XNA.Vector2 origin,
		XNA.Vector2 scale,
		SpriteEffects effects,
		float layerDepth
	) {
		return Core.OnDrawStringImpl.DrawString(
			__instance,
			spriteFont,
			text,
			position,
			color,
			rotation,
			origin,
			scale,
			effects,
			layerDepth
		);
	}
}
