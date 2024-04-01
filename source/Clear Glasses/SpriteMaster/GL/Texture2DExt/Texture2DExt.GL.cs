/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.OpenGL;
using SpriteMaster.Harmonize;
using SpriteMaster.Harmonize.Patches.PSpriteBatch.Patch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.CompilerServices;
using static SpriteMaster.GL.GLExt.Delegates;

namespace SpriteMaster.GL;

internal static partial class Texture2DExt {
	internal static volatile bool Working = true;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static GLExt.SizedInternalFormat GetSizedFormat(PixelInternalFormat format) => format switch {
		PixelInternalFormat.Rgba => GLExt.SizedInternalFormat.RGBA8,
		PixelInternalFormat.Rgb => GLExt.SizedInternalFormat.RGB8,
		PixelInternalFormat.Luminance => GLExt.SizedInternalFormat.R8,
		PixelInternalFormat.Srgb => GLExt.SizedInternalFormat.SRGB8,
		_ => (GLExt.SizedInternalFormat)format
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static bool IsPow2(this int value) {
		return value != 0 && (value & (value - 1)) == 0;
	}

	[Harmonize(
		typeof(Texture2D),
		"GenerateGLTextureIfRequired",
		Harmonize.Harmonize.Fixation.Prefix,
		Harmonize.Harmonize.PriorityLevel.Last,
		critical: false,
		instance: true
	)]
	public static bool GenerateGLTextureIfRequiredOverride(Texture2D __instance) {
		GenerateTexture(__instance, false);
		return false;
	}

	[HarmonizeTranspile(
		typeof(SamplerState),
		"Activate",
		argumentTypes: new[] { typeof(GraphicsDevice), typeof(TextureTarget), typeof(bool) }
	)]
	public static IEnumerable<CodeInstruction> ActivateTranspiler(IEnumerable<CodeInstruction> instructions) {
		var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();

		bool applied = false;

		var referencePropertyGetter = typeof(GraphicsCapabilities)
			.GetProperty(nameof(GraphicsCapabilities.SupportsTextureMaxLevel), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
			?.GetMethod;

		if (referencePropertyGetter is null) {
			Debug.Error("Could not apply SamplerState Activate patch: getter method was null");
		}

		IEnumerable<CodeInstruction> ApplyPatch() {
			var callOpCode = OpCodes.Callvirt.Value;

			foreach (var instruction in codeInstructions) {
				if (
					instruction.opcode.Value == callOpCode &&
					ReferenceEquals(instruction.operand, referencePropertyGetter)
				) {
					yield return new(instruction) {
						opcode = OpCodes.Pop,
						operand = null
					};
					yield return new(OpCodes.Ldc_I4_0);
					applied = true;
				}
				else  {
					yield return instruction;
				}
			}
		}

		var result = ApplyPatch().ToArray();

		if (!applied) {
			Debug.Error("Could not apply SamplerState Activate patch: could not find patch point");
		}

		return result;
	}

	[Conditional("CHECK_TEXTURE_MIP")]
	internal static void CheckTextureMip(this Texture2D texture) {
		GLExt.BindTexture(TextureTarget.Texture2D, texture.glTexture);
		unsafe {
			Span<int> levels = stackalloc int[2];
			fixed (int* levelsPtr = levels) {
				GLExt.GetTexParameteriv(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, (nint)levelsPtr);
				GLExt.GetTexParameteriv(
					TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, (nint)(levelsPtr + 1)
				);
			}

			if (levels[0] != 0) {
				Debug.Break();
			}

			if (levels[1] != 0) {
				Debug.Break();
			}
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int GenerateTexture(Texture2D @this, bool usingStorage) {
		int texture = @this.glTexture;

		if (texture >= 0) {
			@this.CheckTextureMip();
			return texture;
		}

		if (GLExt.CreateTextures.Enabled) {
			GLExt.ObjectId textureObject = 0;
			unsafe {
				GLExt.Checked(
					() => GLExt.CreateTextures.Function(
						TextureTarget.Texture2D,
						1,
						(GLExt.ObjectId*)Unsafe.AsPointer(ref textureObject)
					)
				);
			}

			texture = (int)textureObject;
		}
		else {
			GLExt.Checked(() => MonoGame.OpenGL.GL.GenTextures(1, out texture));
		}
		GLExt.BindTextureChecked(TextureTarget.Texture2D, texture);

		@this.glTexture = texture;

		GLExt.Checked(
			() => MonoGame.OpenGL.GL.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureMinFilter,
				(int)((@this.LevelCount > 1) ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear)
			)
		);
		GLExt.Checked(
			() => MonoGame.OpenGL.GL.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureMagFilter,
				(int)TextureMagFilter.Linear
			)
		);

		// XNA compat
		var wrap = (!@this.Width.IsPow2() || !@this.Height.IsPow2())
			? TextureWrapMode.ClampToEdge
			: TextureWrapMode.Repeat;

		GLExt.Checked(() => MonoGame.OpenGL.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrap));
		GLExt.Checked(() => MonoGame.OpenGL.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrap));

		GLExt.Checked(() => MonoGame.OpenGL.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0));

		if (@this.GraphicsDevice.GraphicsCapabilities.SupportsTextureMaxLevel) {
			GLExt.Checked(() => MonoGame.OpenGL.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, @this.LevelCount - 1));
		}

		@this.CheckTextureMip();

		return texture;
	}
}
