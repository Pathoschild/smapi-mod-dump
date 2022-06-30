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
using MonoGame.OpenGL;
using SpriteMaster.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

// Defined with a 32-bit depth
using GLEnum = System.UInt32;

namespace SpriteMaster.GL;

internal static class GLExt {
	// ReSharper disable UnusedMember.Global

	internal enum ErrorCode : GLEnum {
		NoError = 0x0000,
		InvalidEnum = 0x0500,
		InvalidValue = 0x0501,
		InvalidOperation = 0x0502,
		StackOverflow = 0x0503,
		StackUnderflow = 0x0504,
		OutOfMemory = 0x0505,
		InvalidFramebufferOperation = 0x0506,
		ContextLost = 0x0507,
		TableTooLarge = 0x8031
	}

	internal enum SizedInternalFormat : GLEnum {
		R8 = 0x8229,
		R8SNorm = 0x8F94,
		RG8 = 0x822B,
		RG8SNorm = 0x8F95,
		RGB8 = 0x8051,
		RGB8SNorm = 0x8F96,
		RGBA8 = 0x8058,
		RGBA8SNorm = 0x8F97,
		SRGB8 = 0x8C41,
		SRGB8A8 = 0x8C43
	}
	// ReSharper restore UnusedMember.Global




	[Conditional("DEBUG")]
	[DebuggerHidden]
	internal static void CheckError() {
		var error = (ErrorCode)MonoGame.OpenGL.GL.GetError();
		if (error == ErrorCode.NoError) {
			return;
		}

		var errorList = new List<ErrorCode> { error };

		while ((error = (ErrorCode)MonoGame.OpenGL.GL.GetError()) != ErrorCode.NoError) {
			errorList.Add(error);
		}

		throw new MonoGameGLException($"GL.GetError() returned '{string.Join(", ", errorList)}'");
	}

	// ReSharper disable MemberHidesStaticFromOuterClass
	internal static class Delegates {
		internal static class Generic<T> where T : class? {
			internal delegate T? LoadFunctionDelegate(string function, bool throwIfNotFound = false);

			internal static readonly LoadFunctionDelegate LoadFunction =
				typeof(MonoGame.OpenGL.GL).GetStaticMethod("LoadFunction")?.MakeGenericMethod(typeof(T))
					.CreateDelegate<LoadFunctionDelegate>() ??
					((_, _) => null);
		}

		[SuppressUnmanagedCodeSecurity]
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		[MonoNativeFunctionWrapper]
		internal delegate void TexStorage2D(
			TextureTarget target,
			int levels,
			SizedInternalFormat internalFormat,
			int width,
			int height
		);

		[SuppressUnmanagedCodeSecurity]
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		[MonoNativeFunctionWrapper]
		internal delegate void CopyImageSubData(
			uint srcName,
			TextureTarget srcTarget,
			int srcLevel,
			int srcX,
			int srcY,
			int srcZ,
			uint dstName,
			TextureTarget dstTarget,
			int dstLevel,
			int dstX,
			int dstY,
			int dstZ,
			uint srcWidth,
			uint srcHeight,
			uint srcDepth
		);

		[SuppressUnmanagedCodeSecurity]
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		[MonoNativeFunctionWrapper]
		public unsafe delegate void GetInteger64Delegate(
			int param,
			[Out] long* data
		);
	}
	// ReSharper restore MemberHidesStaticFromOuterClass

	private static bool DebuggingEnabled = false;

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private delegate void DebugMessageCallbackProc(int source, int type, int id, int severity, int length, IntPtr message, IntPtr userParam);

	private static readonly DebugMessageCallbackProc DebugProc = DebugMessageCallbackHandler;
	[System.Security.SuppressUnmanagedCodeSecurity]
	[MonoNativeFunctionWrapper]
	delegate void DebugMessageCallbackDelegate(DebugMessageCallbackProc callback, IntPtr userParam);
	static readonly DebugMessageCallbackDelegate DebugMessageCallback =
		Delegates.Generic<DebugMessageCallbackDelegate>.LoadFunction("glDebugMessageCallback")!;

	private static void DebugMessageCallbackHandler(int source, int type, int id, int severity, int length, IntPtr message, IntPtr userParam) {
#if DEBUG
		switch (id) {
			case 131218: // "Program/shader state performance warning: Vertex shader in program 1 is being recompiled based on GL state."
				return;
		}

		var errorMessage = Marshal.PtrToStringAnsi(message);
		System.Diagnostics.Debug.WriteLine(errorMessage);
#endif
	}

	[Conditional("DEBUG")]
	internal static void EnableDebugging() {
		if (DebuggingEnabled) {
			return;
		}

		DebuggingEnabled = true;

		DebugMessageCallback(DebugProc, IntPtr.Zero);
		MonoGame.OpenGL.GL.Enable(EnableCap.DebugOutput);
		MonoGame.OpenGL.GL.Enable(EnableCap.DebugOutputSynchronous);
	}

	internal static readonly Delegates.TexStorage2D? TexStorage2D =
		Delegates.Generic<Delegates.TexStorage2D>.LoadFunction("glTexStorage2D");

	internal static readonly Delegates.CopyImageSubData? CopyImageSubData =
		Delegates.Generic<Delegates.CopyImageSubData>.LoadFunction("glCopyImageSubData");

	internal static readonly Delegates.GetInteger64Delegate? GetInteger64v =
		Delegates.Generic<Delegates.GetInteger64Delegate>.LoadFunction("glGetInteger64v");
}