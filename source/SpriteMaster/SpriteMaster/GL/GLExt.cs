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

using LinqFasterer;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.OpenGL;
using SpriteMaster.Extensions;
using SpriteMaster.Extensions.Reflection;
using SpriteMaster.Mitigations.PyTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Console = System.Console;

// Defined with a 32-bit depth
using GLEnum = System.UInt32;
using IntPtr = System.IntPtr;

namespace SpriteMaster.GL;

internal static unsafe class GLExt {
	// ReSharper disable UnusedMember.Global

	internal enum ObjectId : uint {
		None = 0
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ObjectId AsObjectId(this int value) => (ObjectId)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int AsInt(this ObjectId value) => (int)value;

	internal class GLExtException : MonoGameGLException {
		protected GLExtException(string message) : base(message) { }

		[DoesNotReturn]
		[MethodImpl(Runtime.MethodImpl.ErrorPath)]
		internal static void Throw(string message) =>
			throw new GLExtException(message);
	}

	internal sealed class GLExtValueException : GLExtException {
		private GLExtValueException(string message) : base(message) { }

		[DoesNotReturn]
		[MethodImpl(Runtime.MethodImpl.ErrorPath)]
		internal static void Throw(string name, int value) =>
			throw new GLExtValueException($"Value for '{name}' was invalid: '{value}'");

		[DoesNotReturn]
		[MethodImpl(Runtime.MethodImpl.ErrorPath)]
		internal static void Throw<TEnum>(TEnum name, int value) where TEnum : unmanaged, Enum =>
			throw new GLExtValueException($"Value for '{name}' was invalid: '{value}'");
	}

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

	private static readonly Lazy<HashSet<string>> _extensionsLazy = new(() => MonoGame.OpenGL.GL.Extensions.ToHashSet(), isThreadSafe: false);
	internal static HashSet<string> Extensions => _extensionsLazy.Value;

	internal static void FlushErrors() {
		while (MonoGame.OpenGL.GL.GetError() != MonoGame.OpenGL.ErrorCode.NoError) {
			// Flush the error buffer
		}
	}

	[DebuggerHidden, DoesNotReturn]
	private static void HandleError(ErrorCode error, string expression) {
		using var errorList = ObjectPoolExt<List<string>>.Take(list => list.Clear());
		if (error is not ErrorCode.NoError) {
			errorList.Value.Add(error.ToString());
		}

		if (Interlocked.Exchange(ref LastCallbackException, null) is { } callbackException) {
			errorList.Value.Add(callbackException.Message);
		}

		while ((error = (ErrorCode)MonoGame.OpenGL.GL.GetError()) != ErrorCode.NoError) {
			errorList.Value.Add(error.ToString());
		}

		string errorMessage = $"GL.GetError() returned '{string.Join(", ", errorList.Value)}': {expression}";
		System.Diagnostics.Debug.WriteLine(errorMessage);
		Debug.Break();
		throw new MonoGameGLException(errorMessage);
	}

	[Conditional("GL_DEBUG"), Conditional("CONTRACTS_FULL"), Conditional("DEBUG")]
	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void CheckError(string? expression = null, [CallerMemberName] string member = "") {
		AlwaysCheckError(expression, member);
	}

	[Conditional("GL_DEBUG"), Conditional("CONTRACTS_FULL"), Conditional("DEBUG")]
	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void CheckErrorExpression((string Expression, object? Value)[] arguments, string callerMember, [CallerMemberName] string member = "") {
		member = member.RemoveFromEnd("Checked");

		var expandedArguments = arguments.Select(pair => $"{pair.Expression} [[{pair.Value}]]");

		AlwaysCheckError($"{member}({string.Join(", ", expandedArguments)})", callerMember);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static (string Expression, object? Value)[] MethodArgs(params (string Expression, object? Value)[] arguments) => arguments;

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AlwaysCheckError(string? expression = null, [CallerMemberName] string member = "") {
		if (LastCallbackException is not null) {
			HandleError(ErrorCode.NoError, expression ?? member);
		}

		if ((ErrorCode)MonoGame.OpenGL.GL.GetError() is var error and not ErrorCode.NoError) {
			HandleError(error, expression ?? member);
		}
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
		LastCallbackException = null;

		while ((ErrorCode)MonoGame.OpenGL.GL.GetError() != ErrorCode.NoError) {
			// Do Nothing
		}
	}

	[Conditional("GL_DEBUG"), Conditional("CONTRACTS_FULL"), Conditional("DEBUG")]
	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void SwallowErrors() {
		LastCallbackException = null;

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
	internal static void Checked<TAction, T0>(TAction action, T0 param0, [CallerArgumentExpression("action")] string expression = "") where TAction : IMethodStruct<T0> {
		action.Invoke(param0);
		CheckError(expression);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Checked<T0, T1>(Action<T0, T1> action, T0 param0, T1 param1, [CallerArgumentExpression("action")] string expression = "") {
		action(param0, param1);
		CheckError(expression);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Checked<TAction, T0, T1>(TAction action, T0 param0, T1 param1, [CallerArgumentExpression("action")] string expression = "") where TAction : IMethodStruct<T0, T1> {
		action.Invoke(param0, param1);
		CheckError(expression);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Checked<T0, T1, T2>(Action<T0, T1, T2> action, T0 param0, T1 param1, T2 param2, [CallerArgumentExpression("action")] string expression = "") {
		action(param0, param1, param2);
		CheckError(expression);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Checked<TAction, T0, T1, T2>(TAction action, T0 param0, T1 param1, T2 param2, [CallerArgumentExpression("action")] string expression = "") where TAction : IMethodStruct<T0, T1, T2> {
		action.Invoke(param0, param1, param2);
		CheckError(expression);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Checked<T0, T1, T2, T3>(Action<T0, T1, T2, T3> action, T0 param0, T1 param1, T2 param2, T3 param3, [CallerArgumentExpression("action")] string expression = "") {
		action(param0, param1, param2, param3);
		CheckError(expression);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Checked<TAction, T0, T1, T2, T3>(TAction action, T0 param0, T1 param1, T2 param2, T3 param3, [CallerArgumentExpression("action")] string expression = "") where TAction : IMethodStruct<T0, T1, T2, T3> {
		action.Invoke(param0, param1, param2, param3);
		CheckError(expression);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Checked<T0, T1, T2, T3, T4>(Action<T0, T1, T2, T3, T4> action, T0 param0, T1 param1, T2 param2, T3 param3, T4 param4, [CallerArgumentExpression("action")] string expression = "") {
		action(param0, param1, param2, param3, param4);
		CheckError(expression);
	}

	[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Checked<TAction, T0, T1, T2, T3, T4>(TAction action, T0 param0, T1 param1, T2 param2, T3 param3, T4 param4, [CallerArgumentExpression("action")] string expression = "") where TAction : IMethodStruct<T0, T1, T2, T3, T4> {
		action.Invoke(param0, param1, param2, param3, param4);
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

	internal static readonly Lazy<(int Major, int Minor)> ContextVersion = new(
		() => {
			try {
				MonoGame.OpenGL.GL.GetInteger(0x821B, out var majorVersion);
				MonoGame.OpenGL.GL.GetInteger(0x821C, out var minorVersion);

				return (majorVersion, minorVersion);
			}
			catch {
				return (2, 0);
			}
		}, isThreadSafe: false
	);

	internal readonly struct FunctionFeature {
		internal readonly (int Major, int Minor)? Version = null;
		internal readonly string? Feature = null;
		internal readonly string Name;

		// ReSharper disable once InconsistentNaming
		internal FunctionFeature(string name) {
			Name = name;
		}

		internal FunctionFeature(string? feature, string name) {
			Feature = feature;
			Name = name;
		}

		internal FunctionFeature((int Major, int minor)? version, string name) {
			Version = version;
			Name = name;
		}

		internal bool Validate() {
			if (Feature is { } feature && !Extensions.Contains(feature)) {
				return false;
			}

			if (Version is { } version) {
				var contextVersion = ContextVersion.Value;
				if (version.Major < contextVersion.Major) {
					return false;
				}

				if (version.Major == contextVersion.Major && version.Minor < contextVersion.Minor) {
					return false;
				}
			}

			return true;
		}
	}

	// ReSharper disable MemberHidesStaticFromOuterClass
	internal static class Delegates {
		private static readonly MethodInfo? LoadFunctionGeneric =
			typeof(MonoGame.OpenGL.GL).GetStaticMethod("LoadFunction");

		internal static class Generic<T> where T : class? {

			private delegate T? LoadFunctionDelegate(string function, bool throwIfNotFound = false);

			private static readonly LoadFunctionDelegate LoadFunctionDelegator =
				LoadFunctionGeneric?.MakeGenericMethod(typeof(T))
					.CreateDelegate<LoadFunctionDelegate>() ??
					((_, _) => null);

			internal static T? LoadFunction(string function) {
				var f = LoadFunctionDelegator(function, false);
				return LoadFunctionDelegator(function, false);
			}

			internal static T LoadFunctionRequired(string function) {
				try {
					return LoadFunctionDelegator(function, true)!;
				}
				catch (Exception ex) {
					throw new DelegateException(function, ex);
				}
			}
		}

		internal static nint? LoadActionPtr(string function) {
			// Doesn't work on Linux due to their use of a dispatch table
			if (Sdl.GL.GetProcAddress(function) is var address && address == IntPtr.Zero) {
				return null;
			}

			return address;
		}

		internal static nint? LoadActionPtr(in FunctionFeature function) {
			if (!function.Validate()) {
				return null;
			}

			// Doesn't work on Linux due to their use of a dispatch table
			if (Sdl.GL.GetProcAddress(function.Name) is var address && address == IntPtr.Zero) {
				return null;
			}

			return address;
		}

		internal static nint LoadActionPtrRequired(string function) {
			if (LoadActionPtr(function) is not {} address) {
				throw new DelegateException(function);
			}

			return address;
		}

		internal static nint LoadActionPtrRequired(in FunctionFeature function) {
			if (LoadActionPtr(in function) is not { } address) {
				throw new DelegateException(function.Name);
			}

			return address;
		}

		internal static nint LoadActionPtrRequired(params FunctionFeature[] functions) {
			foreach (var function in functions) {
				if (LoadActionPtr(function) is { } address) {
					return address;
				}
			}

			throw new DelegateException(functions.SelectF(function => function.Name).FirstOrDefaultF() ?? "<Unknown>");
		}

		internal static nint LoadActionPtrRequired(string feature, params string[] functions) {
			foreach (var function in functions) {
				if (LoadActionPtr(function) is { } address) {
					return address;
				}
			}

			throw new DelegateException(functions.FirstOrDefaultF() ?? "<Unknown>");
		}

		/*
		internal readonly struct ActionPtr<TArg0> {
			private readonly delegate* unmanaged[Stdcall]<TArg0, void> Pointer;

			internal ActionPtr(nint pointer) {
				Pointer = (delegate* unmanaged[Stdcall]<TArg0, void>)pointer;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal readonly void Invoke(TArg0 arg0) =>
				Pointer(arg0);
		}
		*/

		internal static delegate* unmanaged[Stdcall]<TArg0, void> LoadActionPtrRequired<TArg0>(string function) =>
			(delegate* unmanaged[Stdcall]<TArg0, void>)LoadActionPtrRequired(function);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, void> LoadActionPtrRequired<TArg0, TArg1>(string function) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, void>)LoadActionPtrRequired(function);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, void> LoadActionPtrRequired<TArg0, TArg1, TArg2>(string function) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, void>)LoadActionPtrRequired(function);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, void> LoadActionPtrRequired<TArg0, TArg1, TArg2, TArg3>(string function) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, void>)LoadActionPtrRequired(function);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, TArg4, void> LoadActionPtrRequired<TArg0, TArg1, TArg2, TArg3, TArg4>(string function) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, TArg4, void>)LoadActionPtrRequired(function);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, void> LoadActionPtrRequired<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5>(string function) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, void>)LoadActionPtrRequired(function);

		internal static delegate* unmanaged[Stdcall]<TArg0, void> LoadActionPtrRequired<TArg0>(string feature, string function) =>
			(delegate* unmanaged[Stdcall]<TArg0, void>)LoadActionPtrRequired(feature, function);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, void> LoadActionPtrRequired<TArg0, TArg1>(string feature, string function) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, void>)LoadActionPtrRequired(feature, function);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, void> LoadActionPtrRequired<TArg0, TArg1, TArg2>(string feature, string function) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, void>)LoadActionPtrRequired(feature, function);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, void> LoadActionPtrRequired<TArg0, TArg1, TArg2, TArg3>(string feature, string function) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, void>)LoadActionPtrRequired(feature, function);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, TArg4, void> LoadActionPtrRequired<TArg0, TArg1, TArg2, TArg3, TArg4>(string feature, string function) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, TArg4, void>)LoadActionPtrRequired(feature, function);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, void> LoadActionPtrRequired<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5>(string feature, string function) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, void>)LoadActionPtrRequired(feature, function);

		internal static delegate* unmanaged[Stdcall]<TArg0, void> LoadActionPtrRequired<TArg0>(params FunctionFeature[] functions) =>
			(delegate* unmanaged[Stdcall]<TArg0, void>)LoadActionPtrRequired(functions);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, void> LoadActionPtrRequired<TArg0, TArg1>(params FunctionFeature[] functions) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, void>)LoadActionPtrRequired(functions);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, void> LoadActionPtrRequired<TArg0, TArg1, TArg2>(params FunctionFeature[] functions) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, void>)LoadActionPtrRequired(functions);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, void> LoadActionPtrRequired<TArg0, TArg1, TArg2, TArg3>(params FunctionFeature[] functions) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, void>)LoadActionPtrRequired(functions);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, TArg4, void> LoadActionPtrRequired<TArg0, TArg1, TArg2, TArg3, TArg4>(params FunctionFeature[] functions) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, TArg4, void>)LoadActionPtrRequired(functions);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, void> LoadActionPtrRequired<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5>(params FunctionFeature[] functions) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, void>)LoadActionPtrRequired(functions);

		internal static delegate* unmanaged[Stdcall]<TArg0, void> LoadActionPtrRequired<TArg0>(string feature, params string[] functions) =>
			(delegate* unmanaged[Stdcall]<TArg0, void>)LoadActionPtrRequired(feature, functions);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, void> LoadActionPtrRequired<TArg0, TArg1>(string feature, params string[] functions) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, void>)LoadActionPtrRequired(feature, functions);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, void> LoadActionPtrRequired<TArg0, TArg1, TArg2>(string feature, params string[] functions) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, void>)LoadActionPtrRequired(feature, functions);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, void> LoadActionPtrRequired<TArg0, TArg1, TArg2, TArg3>(string feature, params string[] functions) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, void>)LoadActionPtrRequired(feature, functions);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, TArg4, void> LoadActionPtrRequired<TArg0, TArg1, TArg2, TArg3, TArg4>(string feature, params string[] functions) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, TArg4, void>)LoadActionPtrRequired(feature, functions);

		internal static delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, void> LoadActionPtrRequired<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5>(string feature, params string[] functions) =>
			(delegate* unmanaged[Stdcall]<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, void>)LoadActionPtrRequired(feature, functions);

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
		internal delegate void CreateTextures(
			TextureTarget target,
			int n,
			[Out] ObjectId* textures
		);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		internal delegate void CreateTexturesButt(
			TextureTarget target,
			int n,
			[Out] ObjectId* textures
		);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		internal delegate void CreateTexture(
			TextureTarget target,
			int n,
			ref ObjectId texture
		);

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
		internal delegate void TextureStorage2DExt(
			ObjectId texture,
			TextureTarget target,
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
		internal delegate void GetInteger64Delegate(
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

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		internal delegate void DrawElements(
			GLPrimitiveType mode,
			uint count,
			ValueType type,
			nint indices
		);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		internal delegate void DrawRangeElements(
			GLPrimitiveType mode,
			uint start,
			uint end,
			uint count,
			ValueType type,
			nint indices
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

	private static MonoGameGLException? LastCallbackException = null;
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
		System.Diagnostics.Debug.WriteLine($"(severity: {severity}, type: {type}, id: {id}, source: {source}) : {errorMessage}");
		System.Diagnostics.Debug.WriteLine(stackTrace);

		try {
			throw new MonoGameGLException(errorMessage);
		}
		catch (MonoGameGLException ex) {
			Debug.Error(errorMessage, ex);
			LastCallbackException = ex;
		}

		Debug.Break();
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

		internal ToggledDelegate(string feature, string name) : this(
			Extensions.Contains(feature) ?
				Delegates.Generic<TDelegate>.LoadFunction(name) :
				null
			) {
		}

		private static (TDelegate? Delegate, bool Enabled) Load(bool toggle, FunctionFeature[] functions) {
			if (!toggle) {
				return default;
			}

			foreach (var function in functions) {
				if (!function.Validate()) {
					continue;
				}

				if (Delegates.Generic<TDelegate>.LoadFunction(function.Name) is not {} functionDelegate) {
					continue;
				}

				return (functionDelegate, true);
			}

			return default;
		}

		internal ToggledDelegate(params FunctionFeature[] functions) : this(toggle: true, functions) {
		}

		internal ToggledDelegate(bool toggle, params FunctionFeature[] functions) {
			(Function, _enabled) = Load(toggle, functions);
		}

		private static (TDelegate? Delegate, bool Enabled) Load(bool toggle, string feature, string[] functions) {
			if (!toggle) {
				return default;
			}

			if (!Extensions.Contains(feature)) {
				return default;
			}

			foreach (var function in functions) {
				var functionDelegate = Delegates.Generic<TDelegate>.LoadFunction(function);
				if (functionDelegate is null) {
					continue;
				}

				return (functionDelegate, true);
			}

			return default;
		}

		internal ToggledDelegate(string feature, params string[] functions) : this(toggle: true, feature, functions) {
		}

		internal ToggledDelegate(bool toggle, string feature, params string[] functions) {
			(Function, _enabled) = Load(toggle, feature, functions);
		}

		public readonly void Disable() => Unsafe.AsRef(in _enabled) = false;

//[MemberNotNullWhen(true, "Function")]
//public static implicit operator bool(ToggledDelegate<TDelegate> toggledDelegate) => toggledDelegate.Enabled;
	}

	internal abstract class ExtException : Exception {
		internal ExtException(string message) : base(message) {
		}

		internal ExtException(string message, Exception innerException) : base(message, innerException) {
		}
	}

	internal sealed class DelegateException : ExtException {
		private static string MakeMessage(string name) => $"Could not find or load function '{name}'";

		internal DelegateException(string name) : base(MakeMessage(name)) {
		}

		internal DelegateException(string name, Exception innerException) : base(MakeMessage(name), innerException) {
		}
	}

	public enum ValueType : int {
		Byte = 0x1400,
		UnsignedByte = 0x1401,
		Short = 0x1402,
		UnsignedShort = 0x1403,
		Int = 0x1404,
		UnsignedInt = 0x1405,

		Float = 0x1406,
		HalfFloat = 0x140B,
		Double = 0x140A,
		Fixed = 0x140C,
		IntRev_2_10_10_10 = 0x8D9F,
		UIntRev_2_10_10_10 = 0x8368,
		UIntRev_10f_11f_11f = 0x8C3B,
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsLegalIndexType(this ValueType type) =>
		type is ValueType.UnsignedByte or ValueType.UnsignedShort or ValueType.UnsignedInt;

	internal interface IMethodStruct { }

	internal interface IMethodStruct<T0> : IMethodStruct {
		[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
		void Invoke(T0 _);
	}

	internal interface IMethodStruct<T0, T1> : IMethodStruct {
		[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
		void Invoke(T0 _0, T1 _1);
	}

	internal interface IMethodStruct<T0, T1, T2> : IMethodStruct {
		[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
		void Invoke(T0 _0, T1 _1, T2 _2);
	}

	internal interface IMethodStruct<T0, T1, T2, T3> : IMethodStruct {
		[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
		void Invoke(T0 _0, T1 _1, T2 _2, T3 _3);
	}

	internal interface IMethodStruct<T0, T1, T2, T3, T4> : IMethodStruct {
		[DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
		void Invoke(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4);
	}

#if false
	internal static readonly delegate* unmanaged<TextureTarget, int, PixelInternalFormat, int, int, void> TexStorage2DPtr =
		(delegate* unmanaged<TextureTarget, int, PixelInternalFormat, int, int, void>)(void*)Delegates.LoadFunctionPtr("glTexStorage2D");
#endif
	internal static readonly ToggledDelegate<Delegates.CreateTextures> CreateTextures = new(
		new FunctionFeature("GL_ARB_direct_state_access", "glCreateTextures")
	);
	internal static readonly ToggledDelegate<Delegates.TexStorage2D> TexStorage2D = new(
		new FunctionFeature("GL_EXT_texture_storage", "glTexStorage2DEXT"),
		new FunctionFeature("GL_ARB_texture_storage", "glTexStorage2D")
	);
	internal static readonly ToggledDelegate<Delegates.TextureStorage2D> TextureStorage2D = new(
		"GL_ARB_texture_storage",
		"glTextureStorage2D"
	);
	internal static readonly ToggledDelegate<Delegates.TextureStorage2DExt> TextureStorage2DExt = new(
		"GL_EXT_texture_storage",
		"glTextureStorage2DEXT"
	);

	internal static readonly ToggledDelegate<Delegates.CopyImageSubData> CopyImageSubData = new(
		new FunctionFeature("GL_EXT_copy_image", "glCopyImageSubDataEXT"),
		new FunctionFeature("GL_ARB_copy_image", "glCopyImageSubData")
	);

	internal static readonly ToggledDelegate<Delegates.GetInteger64Delegate> GetInteger64v =
		new(
			new FunctionFeature((3, 2), "glGetInteger64v")
		);

	internal static readonly ToggledDelegate<Delegates.GetTexImageDelegate> GetTexImage =
		new("glGetTexImage");

	internal static readonly ToggledDelegate<Delegates.GetTextureSubImageDelegate> GetTextureSubImage =
		new("GL_ARB_get_texture_sub_image", "glGetTextureSubImage");
	internal static readonly ToggledDelegate<Delegates.GetCompressedTexImageDelegate> GetCompressedTexImage =
		new("glGetCompressedTexImage");
	internal static readonly ToggledDelegate<Delegates.GetCompressedTextureSubImageDelegate> GetCompressedTextureSubImage =
		new("GL_ARB_get_texture_sub_image", "glGetCompressedTextureSubImage");

	internal static readonly delegate* unmanaged[Stdcall]<GLPrimitiveType, uint, ValueType, nint, void> DrawElements =
		Delegates.LoadActionPtrRequired<GLPrimitiveType, uint, ValueType, nint>("glDrawElements");

	internal static readonly delegate* unmanaged[Stdcall]<GLPrimitiveType, uint, uint, uint, ValueType, nint, void> DrawRangeElements =
		Delegates.LoadActionPtrRequired<GLPrimitiveType, uint, uint, uint, ValueType, nint>(
			new FunctionFeature("GL_EXT_draw_range_elements", "glDrawRangeElementsEXT"),
			new FunctionFeature("glDrawRangeElements")
		);

	internal static readonly delegate* unmanaged[Stdcall]<uint, int, ValueType, bool, uint, nint, void> VertexAttribPointer =
		Delegates.LoadActionPtrRequired<uint, int, ValueType, bool, uint, nint>("glVertexAttribPointer");

	internal static readonly delegate* unmanaged[Stdcall]<BufferTarget, GLExt.ObjectId, void> BindBuffer =
		Delegates.LoadActionPtrRequired<BufferTarget, GLExt.ObjectId>("glBindBuffer");

	internal static readonly delegate* unmanaged[Stdcall]<BufferTarget, nint, nint, BufferUsageHint, void> BufferData =
		Delegates.LoadActionPtrRequired<BufferTarget, nint, nint, BufferUsageHint>("glBufferData");

	internal static readonly delegate* unmanaged[Stdcall]<uint, uint, void> VertexAttribDivisor =
		Delegates.LoadActionPtrRequired<uint, uint>(
			"GL_ARB_instanced_arrays",
			"glVertexAttribDivisorEXT",
			"glVertexAttribDivisorARB",
			"glVertexAttribDivisor"
		);

	internal static readonly delegate* unmanaged[Stdcall]<TextureTarget, TextureParameterName, nint, void> GetTexParameteriv =
		Delegates.LoadActionPtrRequired<TextureTarget, TextureParameterName, nint>("glGetTexParameteriv");

	#region BindTexture

	private static readonly delegate* unmanaged[Stdcall]<TextureTarget, GLExt.ObjectId, void> _bindTexture = 
		Delegates.LoadActionPtrRequired<TextureTarget, GLExt.ObjectId>("glBindTexture");

	private static GLExt.ObjectId _lastBoundTexture = GLExt.ObjectId.None;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void BindTexture(TextureTarget target, GLExt.ObjectId texture) {
		if (target is TextureTarget.Texture2D) {
			if (texture != _lastBoundTexture) {
				_bindTexture(target, texture);
				_lastBoundTexture = texture;
			}
		}
		else {
			_bindTexture(target, texture);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void BindTexture(TextureTarget target, int texture) =>
		BindTexture(target, (GLExt.ObjectId)texture);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void BindTextureChecked(
		TextureTarget target,
		GLExt.ObjectId texture,
		[CallerArgumentExpression("target")] string targetExpression = "",
		[CallerArgumentExpression("texture")] string textureExpression = "",
		[CallerMemberName] string member = ""
	) {
		BindTexture(target, texture);
		CheckErrorExpression(MethodArgs((targetExpression, target), (textureExpression, texture)), member);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void BindTextureChecked(
		TextureTarget target,
		int texture,
		[CallerArgumentExpression("target")] string targetExpression = "",
		[CallerArgumentExpression("texture")] string textureExpression = "",
		[CallerMemberName] string member = ""
	) {
		BindTextureChecked(target, (ObjectId)texture, targetExpression, textureExpression, member);
	}

	#endregion

	#region PixelStore

	private static readonly delegate* unmanaged[Stdcall]<PixelStoreName, int, void> _pixelStorei =
		Delegates.LoadActionPtrRequired<PixelStoreName, int>("glPixelStorei"); // glPixelStoref is basically useless for us

	private static class PixelStoreValues {
		private static int _packAlignment;
		private static int _unpackAlignment;

		[Conditional("GL_DEBUG"), Conditional("CONTRACTS_FULL"), Conditional("DEBUG")]
		[MethodImpl(Runtime.MethodImpl.Inline)]
		private static void CheckAlignment(PixelStoreName name, int value) {
			if (value is not (1 or 2 or 4 or 8)) {
				GLExtValueException.Throw(name, value);
			}
		}

		// These are the ones commonly-used
		internal static int PackAlignment {
			[MethodImpl(Runtime.MethodImpl.Inline)]
			get => _packAlignment;
			[MethodImpl(Runtime.MethodImpl.Inline)]
			set {
				CheckAlignment(PixelStoreName.PackAlignment, value);
				_packAlignment = value;
			}
		}
		internal static int UnpackAlignment {
			[MethodImpl(Runtime.MethodImpl.Inline)]
			get => _unpackAlignment;
			[MethodImpl(Runtime.MethodImpl.Inline)]
			set {
				CheckAlignment(PixelStoreName.UnpackAlignment, value);
				_unpackAlignment = value;
			}
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void PixelStore(PixelStoreName name, int parameter) {
		if (name is (PixelStoreName.PackAlignment or PixelStoreName.UnpackAlignment)) {
			if (name is PixelStoreName.PackAlignment) {
				if (PixelStoreValues.PackAlignment != parameter) {
					PixelStoreValues.PackAlignment = parameter;
					_pixelStorei(PixelStoreName.PackAlignment, parameter);
				}
			}
			else {
				if (PixelStoreValues.UnpackAlignment != parameter) {
					PixelStoreValues.UnpackAlignment = parameter;
					_pixelStorei(PixelStoreName.UnpackAlignment, parameter);
				}
			}
		}
		else {
			_pixelStorei(name, parameter);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void PixelStore(PixelStoreParameter name, int parameter) =>
		PixelStore((PixelStoreName)name, parameter);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void PixelStoreChecked(
		PixelStoreName name,
		int parameter,
		[CallerArgumentExpression("name")] string nameExpression = "",
		[CallerArgumentExpression("parameter")] string parameterExpression = "",
		[CallerMemberName] string member = ""
	) {
		PixelStore(name, parameter);
		CheckErrorExpression(MethodArgs((nameExpression, name), (parameterExpression, parameter)), member);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void PixelStoreChecked(
		PixelStoreParameter name,
		int parameter,
		[CallerArgumentExpression("name")] string nameExpression = "",
		[CallerArgumentExpression("parameter")]
		string parameterExpression = "",
		[CallerMemberName] string member = ""
	) =>
		PixelStoreChecked((PixelStoreName)name, parameter, nameExpression, parameterExpression, member);

	#endregion

	internal static void Dump() {
		var dumpBuilder = new StringBuilder();

		dumpBuilder.AppendLine("GLExt Dump:");
		var contextVersion = ContextVersion.Value;
		dumpBuilder.AppendLine($"  Context Version: {contextVersion.Major}.{contextVersion.Minor}");

		var fields = typeof(GLExt)
			.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

		List<(string Name, bool Enabled)> fieldValues = new();

		foreach (var field in fields) {
			var fieldValue = field.GetValue(null);
			bool enabled;

			switch (fieldValue)
			{
				case IToggledDelegate toggledDelegate:
					enabled = toggledDelegate.Enabled;
					break;
				case nint pointer:
					enabled = pointer != 0;
					break;
				default:
					continue;
			}

			fieldValues.Add((field.Name, enabled));
		}

		int maxName = fieldValues.MaxF(pair => pair.Name.Length);

		foreach (var (name, enabled) in fieldValues) {
			dumpBuilder.AppendLine($"  {name.PadRight(maxName)}: {enabled}");
		}

		Debug.Message(dumpBuilder.ToString());
	}

	static GLExt() {
		MonoGame.OpenGL.GL.BindTexture = BindTexture;
		MonoGame.OpenGL.GL.GetInteger(GetPName.TextureBinding2D, out int currentBinding);
		_lastBoundTexture = (ObjectId)currentBinding;

		MonoGame.OpenGL.GL.PixelStore = PixelStore;
		MonoGame.OpenGL.GL.GetInteger((GetPName)PixelStoreName.PackAlignment, out int packAlignment);
		PixelStoreValues.PackAlignment = packAlignment;
		MonoGame.OpenGL.GL.GetInteger((GetPName)PixelStoreName.UnpackAlignment, out int unpackAlignment);
		PixelStoreValues.UnpackAlignment = unpackAlignment;
	}
}

// glGetInteger64vEXT 