/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types.Reflection;

[StructLayout(LayoutKind.Auto)]
internal class VariableStaticAccessor<TResult> {
	internal readonly VariableInfo Info;
	private readonly Func<TResult> Getter;
	private readonly Action<TResult> Setter;
	internal readonly bool HasGetter;
	internal readonly bool HasSetter;

	[DoesNotReturn]
	private static TResult InvalidGetter(VariableInfo info) =>
		ThrowHelper.ThrowInvalidOperationException<TResult>($"Variable '{info}' does not have a valid getter");

	[DoesNotReturn]
	private static void InvalidSetter(VariableInfo info) =>
		ThrowHelper.ThrowInvalidOperationException<TResult>($"Variable '{info}' does not have a valid setter");

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal TResult Get() =>
		Getter();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void Set(TResult value) =>
		Setter(value);

	internal TResult Value {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Getter();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Setter(value);
	}

	internal VariableStaticAccessor(VariableInfo info, Func<TResult>? getter, Action<TResult>? setter) {
		Info = info;
		HasGetter = getter is not null;
		Getter = getter ?? (() => InvalidGetter(info));
		HasSetter = setter is not null;
		Setter = setter ?? (_ => InvalidSetter(info));
	}
}
