/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/FlipBuildings
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using FlipBuildings.Utilities;

namespace FlipBuildings.Patches
{
	internal class FishPondPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(FishPond), nameof(FishPond.draw), new Type[] { typeof(SpriteBatch) }),
				transpiler: new HarmonyMethod(typeof(FishPondPatch), nameof(DrawTranspiler))
			);
		}

		private static IEnumerable<CodeInstruction> DrawTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					// Flip inner shadow texture
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 13,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					// Flip water texture
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 13,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					// Flip water texture
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 13,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					// Flip water coloring filter
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 13,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					// Flip pond edge texture
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 11,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					// Flip net texture
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 13,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					// Offset sign position (Counter)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 48,
					targetInstruction: new(OpCodes.Add),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4, 240),
						new(OpCodes.Add)
					},
					goNext: false
				),
				new(
					// Flip sign texture (Counter)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 13,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					// Offset item shadow position (Counter)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 46,
					targetInstruction: new(OpCodes.Sub),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Sub),
						new(OpCodes.Ldc_I4, 240),
						new(OpCodes.Add)
					}
				),
				new(
					// Offset item position (Counter)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 45,
					targetInstruction: new(OpCodes.Sub),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Sub),
						new(OpCodes.Ldc_I4, 240),
						new(OpCodes.Add)
					}
				),
				new(
					// Offset occupant count (Counter)
					referenceInstruction: new(OpCodes.Call, typeof(Utility).GetMethod(nameof(Utility.drawTinyDigits))),
					offset: 44,
					targetInstruction: new(OpCodes.Add),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4, 240),
						new(OpCodes.Add)
					}
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					skip: true
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					skip: true
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					skip: true
				),
				new(
					// Offset bowl position (Chest)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 43,
					targetInstruction: new(OpCodes.Mul),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Mul),
						new(OpCodes.Ldc_I4, 260),
						new(OpCodes.Sub)
					},
					goNext: false
				),
				new(
					// Flip bowl texture (Chest)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 13,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					// Offset bubble position (Chest)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 15,
					targetInstruction: new(OpCodes.Call, typeof(Game1).GetMethod(nameof(Game1.GlobalToLocal), new Type[] { typeof(xTile.Dimensions.Rectangle), typeof(Vector2) })),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_R4, 272f),
						new(OpCodes.Ldc_R4, 0f),
						new(OpCodes.Newobj, typeof(Vector2).GetConstructor(new Type[] { typeof(float), typeof(float) })),
						new(OpCodes.Call, typeof(Vector2).GetMethod(nameof(Vector2.Subtract), new Type[] { typeof(Vector2), typeof(Vector2) })),
						new(OpCodes.Call, typeof(Game1).GetMethod(nameof(Game1.GlobalToLocal), new Type[] { typeof(xTile.Dimensions.Rectangle), typeof(Vector2) }))
					},
					goNext: false
				),
				new(
					// Flip bubble texture (Chest)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 2,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					// Offset item position (Chest)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 18,
					targetInstruction: new(OpCodes.Call, typeof(Game1).GetMethod(nameof(Game1.GlobalToLocal), new Type[] { typeof(xTile.Dimensions.Rectangle), typeof(Vector2) })),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_R4, 272f),
						new(OpCodes.Ldc_R4, 0f),
						new(OpCodes.Newobj, typeof(Vector2).GetConstructor(new Type[] { typeof(float), typeof(float) })),
						new(OpCodes.Call, typeof(Vector2).GetMethod(nameof(Vector2.Subtract), new Type[] { typeof(Vector2), typeof(Vector2) })),
						new(OpCodes.Call, typeof(Game1).GetMethod(nameof(Game1.GlobalToLocal), new Type[] { typeof(xTile.Dimensions.Rectangle), typeof(Vector2) }))
					}
				),
				new(
					// Offset item color position (Chest)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 17,
					targetInstruction: new(OpCodes.Call, typeof(Game1).GetMethod(nameof(Game1.GlobalToLocal), new Type[] { typeof(xTile.Dimensions.Rectangle), typeof(Vector2) })),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_R4, 272f),
						new(OpCodes.Ldc_R4, 0f),
						new(OpCodes.Newobj, typeof(Vector2).GetConstructor(new Type[] { typeof(float), typeof(float) })),
						new(OpCodes.Call, typeof(Vector2).GetMethod(nameof(Vector2.Subtract), new Type[] { typeof(Vector2), typeof(Vector2) })),
						new(OpCodes.Call, typeof(Game1).GetMethod(nameof(Game1.GlobalToLocal), new Type[] { typeof(xTile.Dimensions.Rectangle), typeof(Vector2) }))
					}
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, typeof(FishPond), nameof(FishPond.draw));
		}
	}
}
