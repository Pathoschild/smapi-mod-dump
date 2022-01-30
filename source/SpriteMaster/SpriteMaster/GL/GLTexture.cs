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
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SpriteMaster.GL;

static class GLTexture {
	private static readonly Type TextureType = typeof(Texture);
	private static readonly Type Texture2DType = typeof(Texture2D);
	private static readonly Type? SurfaceTypeType = Texture2DType.GetNestedType("SurfaceType", BindingFlags.NonPublic);
	private static readonly Enum? SwapChainRenderTarget = SurfaceTypeType is null ? null : System.Enum.Parse(SurfaceTypeType, "SwapChainRenderTarget") as Enum;

	private static readonly Func<Texture, int>? GetGLTexture = TextureType.GetFieldGetter<Texture, int>("glTexture");
	private static readonly Action<Texture, int>? SetGLTexture = TextureType.GetFieldSetter<Texture, int>("glTexture");

	private static readonly Func<Texture, Enum>? GetGLInternalFormat = TextureType.GetFieldGetter<Texture, Enum>("glInternalFormat");
	private static readonly Action<Texture, Enum>? SetGLInternalFormat = TextureType.GetFieldSetter<Texture, Enum>("glInternalFormat");

	private static readonly Func<Texture, Enum>? GetGLFormat = TextureType.GetFieldGetter<Texture, Enum>("glFormat");
	private static readonly Action<Texture, Enum>? SetGLFormat = TextureType.GetFieldSetter<Texture, Enum>("glFormat");

	private static readonly Func<Texture, Enum>? GetGLType = TextureType.GetFieldGetter<Texture, Enum>("glType");
	private static readonly Action<Texture, Enum>? SetGLType = TextureType.GetFieldSetter<Texture, Enum>("glType");

	private static readonly ConstructorInfo? Texture2DConstructor = 
		Texture2DType.GetConstructor(
			BindingFlags.NonPublic | BindingFlags.Instance,
			null,
			new Type[] { typeof(GraphicsDevice), typeof(int), typeof(int), typeof(bool), typeof(SurfaceFormat), SurfaceTypeType ?? typeof(Enum), typeof(bool), typeof(int) },
			null
		);

	private static Func<GraphicsDevice, int, int, SurfaceFormat, Enum, Texture2D>? CreateInstance = null;

	static GLTexture() {
		if (Texture2DConstructor is null || SurfaceTypeType is null) {
			return;
		}
		try {
			var dynMethod = new DynamicMethod(
				string.Empty,
				Texture2DType,
				new Type[] { typeof(GraphicsDevice), typeof(int), typeof(int), typeof(SurfaceFormat), typeof(Enum) },
				Texture2DType
			);
			var ilGen = dynMethod.GetILGenerator();
			ilGen.Emit(OpCodes.Ldarg_0); // graphicsDevice
			ilGen.Emit(OpCodes.Ldarg_1); // width
			ilGen.Emit(OpCodes.Ldarg_2); // height
			ilGen.Emit(OpCodes.Ldc_I4_0); // mipmap
			ilGen.Emit(OpCodes.Ldarg_3); // format
			ilGen.Emit(OpCodes.Ldarg_S, 4); // type
			ilGen.Emit(OpCodes.Ldc_I4_0); // shared
			ilGen.Emit(OpCodes.Ldc_I4_0); // arraySize
			ilGen.Emit(OpCodes.Newobj, Texture2DConstructor);
			ilGen.Emit(OpCodes.Ret);
			CreateInstance = dynMethod.CreateDelegate<Func<GraphicsDevice, int, int, SurfaceFormat, Enum, Texture2D>>();
		}
		catch (Exception) {
			throw;
		}

		try {
			var p0 = System.Linq.Expressions.Expression.Parameter(typeof(GraphicsDevice));
			var p1 = System.Linq.Expressions.Expression.Parameter(typeof(int));
			var p2 = System.Linq.Expressions.Expression.Parameter(typeof(int));
			var p3 = System.Linq.Expressions.Expression.Parameter(typeof(SurfaceFormat));
			var p4 = System.Linq.Expressions.Expression.Parameter(typeof(Enum));
			CreateInstance = System.Linq.Expressions.Expression.Lambda<Func<GraphicsDevice, int, int, SurfaceFormat, Enum, Texture2D>>(
				System.Linq.Expressions.Expression.New(
					Texture2DConstructor,
					p0, // graphicsDevice
					p1, // width
					p2, // height
					System.Linq.Expressions.Expression.Constant(false), // mipmap
					p3, // format
					System.Linq.Expressions.Expression.Convert(p4, SurfaceTypeType), // type
					System.Linq.Expressions.Expression.Constant(false), // shared
					System.Linq.Expressions.Expression.Constant(0)  // arraySize
				),
				p0, p1, p2, p3, p4
			).Compile();
		}
		catch (Exception) {
			throw;
		}

		// L_0000: ldarg.0
		// L_0001: ldarg.1
		// L_0002: ldarg.2
		// L_0003: ldc.i4.0
		// L_0004: ldarg.3
		// L_0005: ldarg.s f
		// L_0007: ldc.i4.0
		// L_0008: ldc.i4.0
		// L_0009: newobj instance void C/Foo::.ctor(object, int32, int32, bool, class [System.Private.CoreLib]System.Enum, class [System.Private.CoreLib]System.Enum, bool, int32)
		// L_000e: ret
	}

	internal static Texture2D? CreateTexture2D(Vector2I size, bool mipmap, SurfaceFormat format) {
		if (CreateInstance is null || SwapChainRenderTarget is null) {
			return null;
		}

		var newNewObj = Activator.CreateInstance(typeof(Texture2D), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { DrawState.Device, size.Width, size.Height, false, SurfaceFormat.Dxt5, SwapChainRenderTarget, false, 0 }, null) as Texture2D;

		var newObj = CreateInstance(DrawState.Device, size.Width, size.Height, SurfaceFormat.Dxt5, SwapChainRenderTarget);
		return newObj;
	}
}
