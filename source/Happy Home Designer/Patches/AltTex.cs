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
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using SObject = StardewValley.Object;

namespace HappyHomeDesigner.Patches
{
	internal class AltTex
	{
		public static bool forceMenuDraw = false;
		public static bool forcePreviewDraw = false;

		internal static bool IsApplied = false;
		internal static Assembly asm;

		internal static void Apply(Harmony harmony)
		{
			if (!ModUtilities.TryFindAssembly("AlternativeTextures", out asm))
				return;

			var min_version = new Version(ModEntry.manifest.ExtraFields["AlternativeTexturesVersion"].ToString());
			var android_version = new Version(ModEntry.manifest.ExtraFields["AlternativeTexturesAndroid"].ToString());
			var current_version = asm.GetName().Version;

			bool use_android = false;

			if (OperatingSystem.IsAndroid())
			{
				if (current_version < min_version)
				{
					use_android = true;
					min_version = android_version;
				}
			}

			if (current_version < min_version)
			{
				ModEntry.monitor.Log(
					ModEntry.i18n.Get("logging.alternativeTextures.versionWarning",
					new { min = min_version, current = current_version }),
					LogLevel.Warn
				);
				return;
			}

			var furniturePatcher = asm.GetType("AlternativeTextures.Framework.Patches.StandardObjects.FurniturePatch");
			var objectPatcher = asm.GetType("AlternativeTextures.Framework.Patches.StandardObjects.ObjectPatch");
			var bedPatcher = asm.GetType("AlternativeTextures.Framework.Patches.StandardObjects.BedFurniturePatch");

			if (furniturePatcher is null || objectPatcher is null || bedPatcher is null)
			{
				ModEntry.monitor.Log("Failed to find one or more Alternative Textures patch targets", LogLevel.Warn);
				return;
			}

			var flag = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			harmony.Patch(objectPatcher.GetMethod("DrawPlacementBoundsPrefix", flag), prefix: new(typeof(AltTex), nameof(skipNameCaching)));
			harmony.Patch(furniturePatcher.GetMethod("DrawPrefix", flag), transpiler: new(typeof(AltTex), nameof(fixFurniturePreview)));
			harmony.Patch(objectPatcher.GetMethod("PlacementActionPostfix", flag), prefix: new(typeof(AltTex), nameof(preventRandomVariant)));
			harmony.Patch(bedPatcher.GetMethod("DrawPrefix", flag), transpiler: new(typeof(AltTex), nameof(fixBedPreview)));

			if (use_android)
				harmony.Patch(furniturePatcher.GetMethod("DrawInMenuPrefix", flag), transpiler: new(typeof(Android), nameof(Android.menuDraw)));
			else
				harmony.Patch(furniturePatcher.GetMethod("DrawInMenuPrefix", flag), transpiler: new(typeof(AltTex), nameof(menuDraw)));
			IsApplied = true;
		}

		private static IEnumerable<CodeInstruction> menuDraw(IEnumerable<CodeInstruction> source, ILGenerator gen)
		{
			var skipRotation = gen.DefineLabel();
			var skipOffset = gen.DefineLabel();
			var skipCheck = gen.DefineLabel();

			var batchdraw = typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new[] {
				typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color),
				typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) });

			var il = new CodeMatcher(source)
				.MatchStartForward(
					new(OpCodes.Isinst),
					new(OpCodes.Brtrue_S)
				)
				.Advance(-1)
				.InsertAndAdvance(
					new(OpCodes.Ldarg_0),
					new(OpCodes.Call, typeof(AltTex).GetMethod(nameof(shouldForceDrawObject))),
					new(OpCodes.Brtrue, skipCheck)
				)
				.MatchEndForward(
					new(OpCodes.Ldc_I4_0),
					new(OpCodes.Ceq),
					new(OpCodes.Br_S)
				)
				.MatchEndForward(
					new(OpCodes.Brfalse),
					new(OpCodes.Nop)
				)
				.AddLabels(new[] { skipCheck })
				.MatchStartForward(
					new(OpCodes.Ldarg_0),
					new(OpCodes.Ldfld, typeof(Furniture).GetField(nameof(Furniture.rotations)))
				)
				.InsertAndAdvance(
					new(OpCodes.Ldsfld, typeof(AltTex).GetField(nameof(forceMenuDraw))),
					new(OpCodes.Brtrue, skipRotation)
				)
				.MatchStartForward(
					new(OpCodes.Ldarg_0),
					new(OpCodes.Ldfld, typeof(Furniture).GetField(nameof(Furniture.defaultSourceRect)))
				)
				.AddLabels(new[] { skipRotation })
				.MatchStartForward(
					new(OpCodes.Ldc_I4_0),
					new(OpCodes.Ldarg_0),
					new(OpCodes.Ldfld, typeof(Furniture).GetField(nameof(Furniture.sourceRect)))
				)
				.InsertAndAdvance(
					new(OpCodes.Ldc_I4_0),
					new(OpCodes.Ldsfld, typeof(AltTex).GetField(nameof(forceMenuDraw))),
					new(OpCodes.Brtrue, skipOffset),
					new(OpCodes.Pop)
				)
				.MatchStartForward(
					new CodeMatch(OpCodes.Stfld, typeof(Rectangle).GetField(nameof(Rectangle.X)))
				)
				.AddLabels(new[] { skipOffset })
				.MatchEndForward(
					new CodeMatch(OpCodes.Callvirt, batchdraw)
				)
				.RemoveInstruction()
				.InsertAndAdvance(
					new(OpCodes.Ldarg_0),
					new(OpCodes.Call, typeof(AltTex).GetMethod(nameof(DrawReplace)))
				);

			return il.InstructionEnumeration();
		}

		public static bool shouldForceDrawObject(Furniture f)
			=> forceMenuDraw && f.modData.ContainsKey("AlternativeTextureName") && 
			f.modData.TryGetValue("AlternativeTextureOwner", out var owner) && owner is not "Stardew.Default";

		public static void DrawReplace(SpriteBatch b, Texture2D tex, Vector2 pos, Rectangle? src, Color tint, 
			float ang, Vector2 origin, float scale, SpriteEffects fx, float depth, Furniture f)
		{
			b.Draw(tex, pos, src, tint, ang, origin, scale, fx, depth);

			if (f is not BedFurniture || !src.HasValue)
				return;

			var source = src.Value;
			source.X += source.Width;

			b.Draw(tex, pos, source, tint, ang, origin, scale, fx, depth);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void drawFront(Furniture f, Texture2D texture, Rectangle source, Vector2 location, SpriteBatch batch,
			float scale, float alpha, float depth)
		{
			//if (f is not BedFurniture)
			//	return;

			source.X += source.Width;

			batch.Draw(
				texture,
				new Vector2(location.X + 32f, location.Y + 32f),
				source,
				Color.White * alpha,
				0f, 
				new Vector2(source.Width / 2, source.Height / 2),
				scale * GetScaledSize(source),
				f.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				depth
			);
		}

		private static float GetScaledSize(Rectangle source)
		{
			// based on Furniture.getScaleSize from vanilla
			// and GetScaledSize from AT

			int tilesWide = source.Width / 16;
			int tilesHigh = source.Height / 16;

			return
				tilesWide >= 7 ? .05f :
				tilesWide is 6 ? .66f :
				tilesWide is 5 ? .75f :
				tilesHigh >= 5 ? .80f :
				tilesHigh >= 3 ? 1.0f :
				tilesWide <= 2 ? 2.0f :
				tilesWide <= 4 ? 1.0f :
				.1f;
		}

		private static bool skipNameCaching(ref bool __result, StardewValley.Object __0)
		{
			if (!forcePreviewDraw)
				return true;

			// don't ditch at name, so we can see it in previews
			__result = true;
			if (__0.modData.TryGetValue("AlternativeTextureName", out var name))
				__0.modData["AlternativeTextureNameCached"] = name;
			return false;
		}

		private static IEnumerable<CodeInstruction> fixFurniturePreview(IEnumerable<CodeInstruction> source)
		{
			// when not drawing in-world, still use AT sourcerect instead of default sourcerect
			var il = new CodeMatcher(source)
				.MatchStartForward(
					new CodeMatch(OpCodes.Ldsfld, typeof(Furniture).GetField(nameof(Furniture.isDrawingLocationFurniture)))
				).MatchStartBackwards(
					new CodeMatch(OpCodes.Ldloca_S)
				);
			var sourceRect = il.Instruction.operand;
			il.MatchStartForward(
					new(OpCodes.Ldarg_0),
					new(OpCodes.Ldfld, typeof(Furniture).GetField(nameof(Furniture.sourceRect))),
					new(OpCodes.Call, typeof(NetFieldBase<Rectangle, NetRectangle>).GetMethod("op_Implicit"))
				).RemoveInstructions(3)
				.InsertAndAdvance(
					new CodeInstruction(OpCodes.Ldloc_S, sourceRect)
				);
			return il.InstructionEnumeration();
		}
		private static IEnumerable<CodeInstruction> fixBedPreview(IEnumerable<CodeInstruction> source, ILGenerator gen)
		{
			LocalBuilder bounds = gen.DeclareLocal(typeof(Vector2));
			// just remove location furniture check completely and fix the offset
			var il = new CodeMatcher(source)
				.MatchStartForward(
					new CodeMatch(OpCodes.Ldsfld, typeof(Furniture).GetField(nameof(Furniture.isDrawingLocationFurniture)))
				).MatchStartForward(
					new CodeMatch(OpCodes.Brfalse)
				)
				.RemoveInstruction()
				.InsertAndAdvance(
					new(OpCodes.Ldarg_1),
					new(OpCodes.Call, typeof(NetFieldBase<Vector2, NetVector2>).GetProperty(nameof(NetVector2.Value)).GetMethod),
					new(OpCodes.Ldarg_3),
					new(OpCodes.Ldarg_S, 4),
					new(OpCodes.Ldarg_0),
					new(OpCodes.Call, typeof(AltTex).GetMethod(nameof(AdjustBedPosition))),
					new(OpCodes.Stloc, bounds)
				);

			while (true)
			{
				il.MatchStartForward(
					new(OpCodes.Ldarg_1),
					new(OpCodes.Call, typeof(NetFieldBase<Vector2, NetVector2>).GetMethod("op_Implicit"))
				);
				if (il.IsInvalid)
					break;
				il.RemoveInstructions(2)
					.InsertAndAdvance(
						new CodeInstruction(OpCodes.Ldloc, bounds)
					);
			}

			return il.InstructionEnumeration();
		}
		public static Vector2 AdjustBedPosition(bool placed, Vector2 DrawPosition, int x, int y, BedFurniture __instance)
		{
			if (placed)
				return DrawPosition;
			return new(x * 64, y * 64 - (__instance.sourceRect.Height * 4 - __instance.boundingBox.Height));
		}
		private static bool preventRandomVariant(StardewValley.Object __0)
		{
			return __0 is not Furniture || !forcePreviewDraw;
		}

		private static class Android
		{
			public static IEnumerable<CodeInstruction> menuDraw(IEnumerable<CodeInstruction> source, ILGenerator gen)
			{
				var skipRotation = gen.DefineLabel();
				var skipOffset = gen.DefineLabel();

				var il = new CodeMatcher(source, gen)
					.MatchEndForward(
						new(OpCodes.Call, typeof(Game1).GetProperty(nameof(Game1.activeClickableMenu)).GetMethod),
						new(OpCodes.Isinst),
						new(OpCodes.Brfalse_S)
					)
					.Advance(1)
					.CreateLabel(out var skipCheck)
					.Advance(-3)
					.InsertAndAdvance(
						new(OpCodes.Ldsfld, typeof(AltTex).GetField(nameof(forceMenuDraw))),
						new(OpCodes.Brtrue, skipCheck)
					).MatchStartForward(
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Furniture).GetField(nameof(Furniture.rotations)))
					).InsertAndAdvance(
						new(OpCodes.Ldsfld, typeof(AltTex).GetField(nameof(forceMenuDraw))),
						new(OpCodes.Brtrue, skipRotation)
					).MatchStartForward(
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Furniture).GetField(nameof(Furniture.defaultSourceRect)))
					);
				il.Instruction.labels.Add(skipRotation);

				il.MatchStartForward(
						new(OpCodes.Ldc_I4_0),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Furniture).GetField(nameof(Furniture.sourceRect)))
					)
					.InsertAndAdvance(
						new(OpCodes.Ldc_I4_0),
						new(OpCodes.Ldsfld, typeof(AltTex).GetField(nameof(forceMenuDraw))),
						new(OpCodes.Brtrue, skipOffset),
						new(OpCodes.Pop)
					)
					.MatchStartForward(
						new CodeMatch(OpCodes.Stfld, typeof(Rectangle).GetField(nameof(Rectangle.X)))
					);
				il.Instruction.labels.Add(skipOffset);

				return il.InstructionEnumeration();
			}
		}
	}
}
