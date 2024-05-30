/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using HappyHomeDesigner.Framework;
using HappyHomeDesigner.Integration;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Reflection;

namespace HappyHomeDesigner.Patches
{
	internal class CatalogFX
	{
		private static AccessTools.FieldRef<Furniture, NetVector2> drawPosition;
		private static Func<Furniture, float> getScaleSize;
		private static readonly Vector2 menuOffset = new(32f, 32f);
		private const float PIXEL_DEPTH = 1f / 10_000f;
		private const float DISCRIMINATOR = PIXEL_DEPTH / 10f;

		internal static void Apply(Harmony harmony)
		{
			drawPosition = AccessTools.FieldRefAccess<Furniture, NetVector2>("drawPosition");
			getScaleSize = typeof(Furniture)
				.GetMethod("getScaleSize", BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic)
				.CreateDelegate<Func<Furniture, float>>();

			harmony.TryPatch(
				typeof(Furniture).GetMethod(nameof(Furniture.draw), BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly),
				postfix: new(typeof(CatalogFX), nameof(DrawPatch))
			);
			harmony.TryPatch(
				typeof(Furniture).GetMethod(nameof(Furniture.drawAtNonTileSpot)),
				postfix: new(typeof(CatalogFX), nameof(DrawOnSpot))
			);
			harmony.TryPatch(
				typeof(Furniture).GetMethod(nameof(Furniture.drawInMenu), BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly),
				postfix: new(typeof(CatalogFX), nameof(DrawMenu))
			);
		}

		private static void DrawPatch(Furniture __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
		{
			switch(__instance.QualifiedItemId)
			{
				case "(F)" + AssetManager.COLLECTORS_ID:
					GetVars(__instance, x, y, out var pos, out var effect, out var depth);
					DrawCollectorFX(__instance, spriteBatch, pos, Color.White * alpha, depth, 4f, effect, Vector2.Zero);
					break;
				case "(F)" + AssetManager.DELUXE_ID:
					GetVars(__instance, x, y, out pos, out effect, out depth);
					DrawDeluxeFX(__instance, spriteBatch, pos, Color.White * alpha, depth, 4f, effect, Vector2.Zero);
					break;
			}

			if (__instance.heldObject.Value is Item item && item.QualifiedItemId is "(O)" + AssetManager.PORTABLE_ID)
			{
				HandCatalogue.DrawInWorld(
					item, spriteBatch,
					new(__instance.boundingBox.Center.X - 32, __instance.boundingBox.Center.Y - (__instance.drawHeldObjectLow.Value ? 32 : 85)),
					(__instance.boundingBox.Bottom + 1) * PIXEL_DEPTH
				);
			}
		}

		private static void DrawMenu(Furniture __instance, SpriteBatch spriteBatch, Vector2 location, 
			float scaleSize, float transparency, float layerDepth, Color color)
		{
			switch(__instance.QualifiedItemId)
			{
				case "(F)" + AssetManager.COLLECTORS_ID:
					DrawCollectorFX(
						__instance, spriteBatch, location + menuOffset, color * transparency, 
						layerDepth, scaleSize * getScaleSize(__instance), SpriteEffects.None, 
						new(__instance.sourceRect.Width / 2, __instance.sourceRect.Height / 2)
					); break;
				case "(F)" + AssetManager.DELUXE_ID:
					DrawDeluxeFX(
						__instance, spriteBatch, location + menuOffset, color * transparency, 
						layerDepth, scaleSize * getScaleSize(__instance), SpriteEffects.None, 
						new(__instance.sourceRect.Width / 2, __instance.sourceRect.Height / 2)
					); break;
			}
		}

		private static void DrawOnSpot(Furniture __instance, SpriteBatch spriteBatch, Vector2 location, float layerDepth, float alpha)
		{
			switch(__instance.QualifiedItemId)
			{
				case "(F)" + AssetManager.COLLECTORS_ID:
					var flipped = __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
					DrawCollectorFX(__instance, spriteBatch, location, Color.White * alpha, layerDepth, 4f, flipped, Vector2.Zero);
					break;
				case "(F)" + AssetManager.DELUXE_ID:
					flipped = __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
					DrawDeluxeFX(__instance, spriteBatch, location, Color.White * alpha, layerDepth, 4f, flipped, Vector2.Zero);
					break;
			}
		}

		private static void GetVars(Furniture furn, int x, int y, out Vector2 location, out SpriteEffects effect, out float depth)
		{
			location =
				Game1.GlobalToLocal(Game1.viewport, 
				Furniture.isDrawingLocationFurniture ?
				drawPosition(furn).Value :
				new(x * 64, y * 64 - (furn.sourceRect.Height * 4 - furn.boundingBox.Height))
			);
			effect = furn.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			depth =
				Furniture.isDrawingLocationFurniture ?
				(furn.boundingBox.Bottom - 8) * PIXEL_DEPTH :
				0f;
		}

		private static void DrawCollectorFX(Furniture furn, SpriteBatch batch, Vector2 pos, Color color, float depth, float scale, SpriteEffects effect, Vector2 origin)
		{
			var data = ItemRegistry.GetData(furn.QualifiedItemId);
			pos = new(pos.X, pos.Y + (float)Math.Sin(Game1.ticks * (Math.Tau / 100.0)) * 1.5f * scale);
			var source = furn.sourceRect.Value;
			var texture = data.GetTexture();

			if (AlternativeTextures.Installed)
				AlternativeTextures.GetTextureSource(furn.modData, furn.defaultSourceRect.Value, ref texture, ref source);

			source.X += source.Width;

			batch.Draw(texture, pos, source, color, 0f, origin, scale, effect, depth + DISCRIMINATOR);
		}

		private static void DrawDeluxeFX(Furniture furn, SpriteBatch batch, Vector2 pos, Color color, float depth, float scale, SpriteEffects effect, Vector2 origin)
		{
			var data = ItemRegistry.GetData(furn.QualifiedItemId);
			var texture = data.GetTexture();
			var source = furn.sourceRect.Value;

			if (AlternativeTextures.Installed)
				AlternativeTextures.GetTextureSource(furn.modData, furn.defaultSourceRect.Value, ref texture, ref source);

			var col = color.Mult(Utility.GetPrismaticColor(0));
			source.X += source.Width;
			batch.Draw(texture, pos, source, col, 0f, origin, scale, effect, depth + (1f * DISCRIMINATOR));

			col = color.Mult(Utility.GetPrismaticColor(2));
			source.Y += source.Height;
			batch.Draw(texture, pos, source, col, 0f, origin, scale, effect, depth + (2f * DISCRIMINATOR));

			col = color.Mult(Utility.GetPrismaticColor(4));
			source.X -= source.Width;
			batch.Draw(texture, pos, source, col, 0f, origin, scale, effect, depth + (3f * DISCRIMINATOR));

		}
	}
}
