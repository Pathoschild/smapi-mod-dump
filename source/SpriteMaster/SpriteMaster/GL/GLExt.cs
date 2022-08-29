/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

//#define GL_DEBUG

using Microsoft.Xna.Framework.Graphics;
using MonoGame.OpenGL;
using SpriteMaster.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Defined with a 32-bit depth
using GLEnum = System.UInt32;

namespace SpriteMaster.GL;

internal static unsafe class GLExt {
	// ReSharper disable UnusedMember.Global

	internal enum ObjectId : uint {
		None = 0
	};

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

	[DebuggerHidden, DoesNotReturn]
	private static void HandleError(ErrorCode error, string expression) {
		var errorList = new List<ErrorCode> { error };

		while ((error = (ErrorCode)MonoGame.OpenGL.GL.GetError()) != ErrorCode.NoError) {
			errorList.Add(error);
		}

		string errorMessage = $"GL.GetError() returned '{string.Join(", ", errorList)}': {expression}";
		System.Diagnostics.Debug.WriteLine(errorMessage);
		Debugger.Break();
		throw new MonoGameGLException(errorMessage);
	}

	[Conditional("GL_DEBUG"), Conditional("CONTRACTS_FULL"), Conditional("DEBUG")]
	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void CheckError(string? expression = null, [CallerMemberName] string member = "") {
		AlwaysCheckError(expression, member);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AlwaysCheckError(string? expression = null, [CallerMemberName] string member = "") {
		var error = (ErrorCode)MonoGame.OpenGL.GL.GetError();
		if (error == ErrorCode.NoError) {
			return;
		}

		HandleError(error, expression ?? member);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void SwallowOrReportErrors() {
#if GL_DEBUG || CONTRACTS_FULL || DEBUG
		CheckError();
#else
		AlwaysSwallowErrors();
#endif
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AlwaysSwallowErrors() {
		while ((ErrorCode)MonoGame.OpenGL.GL.GetError() != ErrorCode.NoError) {
			// Do Nothing
		}
	}

	[Conditional("GL_DEBUG"), Conditional("CONTRACTS_FULL"), Conditional("DEBUG")]
	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void SwallowErrors() {
		while ((ErrorCode)MonoGame.OpenGL.GL.GetError() != ErrorCode.NoError) {
			// Do Nothing
		}
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Checked(Action action, [CallerArgumentExpression("action")] string expression = "") {
		action();
		CheckError(expression);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Checked<T0>(Action<T0> action, T0 param0, [CallerArgumentExpression("action")] string expression = "") {
		action(param0);
		CheckError(expression);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Checked<T0, T1>(Action<T0, T1> action, T0 param0, T1 param1, [CallerArgumentExpression("action")] string expression = "") {
		action(param0, param1);
		CheckError(expression);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Checked<T0, T1, T2>(Action<T0, T1, T2> action, T0 param0, T1 param1, T2 param2, [CallerArgumentExpression("action")] string expression = "") {
		action(param0, param1, param2);
		CheckError(expression);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Checked<T0, T1, T2, T3>(Action<T0, T1, T2, T3> action, T0 param0, T1 param1, T2 param2, T3 param3, [CallerArgumentExpression("action")] string expression = "") {
		action(param0, param1, param2, param3);
		CheckError(expression);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Checked<T0, T1, T2, T3, T4>(Action<T0, T1, T2, T3, T4> action, T0 param0, T1 param1, T2 param2, T3 param3, T4 param4, [CallerArgumentExpression("action")] string expression = "") {
		action(param0, param1, param2, param3, param4);
		CheckError(expression);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AlwaysChecked(Action action, [CallerArgumentExpression("action")] string expression = "") {
		action();
		AlwaysCheckError(expression);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AlwaysChecked<T0>(Action<T0> action, T0 param0, [CallerArgumentExpression("action")] string expression = "") {
		action(param0);
		AlwaysCheckError(expression);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AlwaysChecked<T0, T1>(Action<T0, T1> action, T0 param0, T1 param1, [CallerArgumentExpression("action")] string expression = "") {
		action(param0, param1);
		AlwaysCheckError(expression);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AlwaysChecked<T0, T1, T2>(Action<T0, T1, T2> action, T0 param0, T1 param1, T2 param2, [CallerArgumentExpression("action")] string expression = "") {
		action(param0, param1, param2);
		AlwaysCheckError(expression);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AlwaysChecked<T0, T1, T2, T3>(Action<T0, T1, T2, T3> action, T0 param0, T1 param1, T2 param2, T3 param3, [CallerArgumentExpression("action")] string expression = "") {
		action(param0, param1, param2, param3);
		AlwaysCheckError(expression);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AlwaysChecked<T0, T1, T2, T3, T4>(Action<T0, T1, T2, T3, T4> action, T0 param0, T1 param1, T2 param2, T3 param3, T4 param4, [CallerArgumentExpression("action")] string expression = "") {
		action(param0, param1, param2, param3, param4);
		AlwaysCheckError(expression);
	}

	// ReSharper disable MemberHidesStaticFromOuterClass
	internal static class Delegates {
		internal static class Generic<T> where T : class? {
			internal delegate T? LoadFunctionDelegate(string function, bool throwIfNotFound = false);

			internal static readonly LoadFunctionDelegate LoadFunctionDelegator =
				typeof(MonoGame.OpenGL.GL).GetStaticMethod("LoadFunction")?.MakeGenericMethod(typeof(T))
					.CreateDelegate<LoadFunctionDelegate>() ??
					((_, _) => null);

			internal static T? LoadFunction(string function, bool throwIfNotFound = false) {
				return LoadFunctionDelegator(function, throwIfNotFound);
			}
		}

#if false
		internal static class Sdl {
			internal static readonly Type? SdlType = ReflectionExt.GetTypeExt("Sdl");
			internal static readonly Type? SdlGlType = SdlType?.GetNestedType("GL");

			internal static class Generic<T> {
				internal delegate T? LoadFunctionDelegate(IntPtr library, string function, bool throwIfNotFound = false);

				internal static readonly LoadFunctionDelegate? LoadFunction =
					ReflectionExt.GetTypeExt("MonoGame.Framework.Utilities.FuncLoader")?.GetStaticMethod("LoadFunction")?.MakeGenericMethod(typeof(T))
						.CreateDelegate<LoadFunctionDelegate>();
			}

			internal static readonly nint NativeLibrary = (IntPtr?)SdlType?.GetStaticField("NativeLibrary")?.GetValue(null) ?? (nint)0;

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate nint GetProcAddressDelegate(string proc);

			internal static readonly GetProcAddressDelegate? GetProcAddress = Generic<GetProcAddressDelegate>.LoadFunction?.Invoke(NativeLibrary, "SDL_GL_GetProcAddress");
		}

		internal static nint LoadFunctionPtr(string function, bool throwIfNotFound = false) {
			var result = Sdl.GetProcAddress?.Invoke(function) ?? 0;
			if (result is 0) {
				return 0;
			}
			return result;
		}
#endif

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		internal delegate void TexStorage2D(
			TextureTarget target,
			int levels,
			SizedInternalFormat internalFormat,
			int width,
			int height
		);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		internal delegate void TextureStorage2D(
			ObjectId target,
			int levels,
			SizedInternalFormat internalFormat,
			int width,
			int height
		);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		internal delegate void TexStorage2DExt(
			TextureTarget target,
			int levels,
			SizedInternalFormat internalFormat,
			int width,
			int height
		);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		internal delegate void TextureStorage2DExt(
			ObjectId target,
			int levels,
			SizedInternalFormat internalFormat,
			int width,
			int height
		);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		internal delegate void CopyImageSubData(
			ObjectId srcName,
			TextureTarget srcTarget,
			int srcLevel,
			int srcX,
			int srcY,
			int srcZ,
			ObjectId dstName,
			TextureTarget dstTarget,
			int dstLevel,
			int dstX,
			int dstY,
			int dstZ,
			uint srcWidth,
			uint srcHeight,
			uint srcDepth
		);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		internal unsafe delegate void GetInteger64Delegate(
			int param,
			[Out] long* data
		);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		internal delegate void GetTexImageDelegate(TextureTarget target, int level, PixelFormat format, PixelType type, [Out] nint pixels);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		internal delegate void GetTextureSubImageDelegate(
			ObjectId target,
			int level,
			int xOffset,
			int yOffset,
			int zOffset,
			uint width,
			uint height,
			uint depth,
			PixelFormat format,
			PixelType type,
			uint bufferSize,
			[Out] nint pixels
		);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		internal delegate void GetCompressedTexImageDelegate(TextureTarget target, int level, [Out] nint pixels);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		internal delegate void GetCompressedTextureSubImageDelegate(
			ObjectId target,
			int level,
			int xOffset,
			int yOffset,
			int zOffset,
			uint width,
			uint height,
			uint depth,
			uint bufferSize,
			[Out] nint pixels
		);
	}
	// ReSharper restore MemberHidesStaticFromOuterClass

	private static bool DebuggingEnabled = false;

	[UnmanagedFunctionPointer(CallingConvention.Winapi)]
	private delegate void DebugMessageCallbackProc(int source, int type, int id, int severity, int length, nint message, nint userParam);

	private static readonly DebugMessageCallbackProc DebugProc = DebugMessageCallbackHandler;
	delegate void DebugMessageCallbackDelegate(DebugMessageCallbackProc callback, nint userParam);
	static readonly DebugMessageCallbackDelegate DebugMessageCallback =
		Delegates.Generic<DebugMessageCallbackDelegate>.LoadFunction("glDebugMessageCallback")!;

	private enum CallbackSeverity : int {
		High = 0x9146,
		Medium = 0x9147,
		Low = 0x9148,
		Notification = 0x826B
	}

	[DebuggerHidden]
	private static void DebugMessageCallbackHandler(int source, int type, int id, int severityValue, int length, nint message, nint userParam) {
#if GL_DEBUG || DEBUG || CONTRACTS_FULL
		var severity = (CallbackSeverity)severityValue;

		if (severity == CallbackSeverity.Notification) {
			return;
		}

		switch (id) {
			case 131218: // "Program/shader state performance warning: Vertex shader in program 1 is being recompiled based on GL state."
			//case 131185: // "Buffer detailed info: Buffer object 1 (bound to GL_ELEMENT_ARRAY_BUFFER_ARB, usage hint is GL_STATIC_DRAW) will use VIDEO memory as the source for buffer object operations."
			return;
		}

		var errorMessage = Marshal.PtrToStringAnsi(message) ?? "unknown";
		var stackTrace = Environment.StackTrace;
		System.Diagnostics.Debug.WriteLine(errorMessage);
		System.Diagnostics.Debug.WriteLine(stackTrace);

		try {
			throw new MonoGameGLException(errorMessage);
		}
		catch (MonoGameGLException ex) {
			Debug.Error(errorMessage, ex);
		}

		Debugger.Break();
#endif
	}

	[Conditional("GL_DEBUG"), Conditional("CONTRACTS_FULL"), Conditional("DEBUG")]
	internal static void EnableDebugging() {
		if (DebuggingEnabled) {
			return;
		}

		DebuggingEnabled = true;

		DebugMessageCallback(DebugProc, 0);
		MonoGame.OpenGL.GL.Enable(EnableCap.DebugOutput);
		MonoGame.OpenGL.GL.Enable(EnableCap.DebugOutputSynchronous);
	}

	internal interface IToggledDelegate {
		bool Enabled { get; }
		void Disable();
	}

	internal interface IToggledDelegate<TDelegate> : IToggledDelegate where TDelegate : Delegate {
		[MemberNotNullWhen(true, "Function")]
		new bool Enabled { get; }

		TDelegate? Function { get; }
	}

	[StructLayout(LayoutKind.Auto)]
	internal readonly struct ToggledDelegate<TDelegate> : IToggledDelegate<TDelegate> where TDelegate : Delegate {
		private readonly bool _enabled;
		internal readonly TDelegate? Function;

		[MemberNotNullWhen(true, "Function")]
		internal readonly bool Enabled => _enabled;

		readonly TDelegate? IToggledDelegate<TDelegate>.Function => Function;
		[MemberNotNullWhen(true, "Function")]
		readonly bool IToggledDelegate.Enabled => Enabled;
		[MemberNotNullWhen(true, "Function")]
		readonly bool IToggledDelegate<TDelegate>.Enabled => Enabled;

		private ToggledDelegate(TDelegate? function) {
			Function = function;
			_enabled = function is not null;
		}

		internal ToggledDelegate(string name) : this(Delegates.Generic<TDelegate>.LoadFunction(name)) {
		}

		public readonly void Disable() => Unsafe.AsRef(in _enabled) = false;

//[MemberNotNullWhen(true, "Function")]
//public static implicit operator bool(ToggledDelegate<TDelegate> toggledDelegate) => toggledDelegate.Enabled;
	}

#if false
	internal static readonly delegate* unmanaged<TextureTarget, int, PixelInternalFormat, int, int, void> TexStorage2DPtr =
		(delegate* unmanaged<TextureTarget, int, PixelInternalFormat, int, int, void>)(void*)Delegates.LoadFunctionPtr("glTexStorage2D");
#endif
	internal static readonly ToggledDelegate<Delegates.TexStorage2D> TexStorage2D = new("glTexStorage2D");
	internal static readonly ToggledDelegate<Delegates.TextureStorage2D> TextureStorage2D = new("glTextureStorage2D");

	internal static readonly ToggledDelegate<Delegates.TexStorage2DExt> TexStorage2DExt = new("glTexStorage2DEXT");
	internal static readonly ToggledDelegate<Delegates.TextureStorage2DExt> TextureStorage2DExt = new("glTextureStorage2DEXT");

	internal static readonly ToggledDelegate<Delegates.CopyImageSubData> CopyImageSubData = new("glCopyImageSubData");

	internal static readonly ToggledDelegate<Delegates.GetInteger64Delegate> GetInteger64v = new("glGetInteger64v");

	internal static readonly ToggledDelegate<Delegates.GetTexImageDelegate> GetTexImage = new("glGetTexImage");

	internal static readonly ToggledDelegate<Delegates.GetTextureSubImageDelegate> GetTextureSubImage = new("glGetTextureSubImage");
	internal static readonly ToggledDelegate<Delegates.GetCompressedTexImageDelegate> GetCompressedTexImage = new("glGetCompressedTexImage");
	internal static readonly ToggledDelegate<Delegates.GetCompressedTextureSubImageDelegate> GetCompressedTextureSubImage = new("glGetCompressedTextureSubImage");
}