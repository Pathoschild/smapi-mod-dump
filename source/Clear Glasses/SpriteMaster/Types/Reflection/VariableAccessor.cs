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
using System.Runtime.InteropServices;

namespace SpriteMaster.Types.Reflection;

[StructLayout(LayoutKind.Auto)]
internal class VariableAccessor<TObject, TResult> {
	internal readonly VariableInfo Info;
	private readonly Func<TObject, TResult> Getter;
	private readonly Action<TObject, TResult> Setter;
	internal readonly bool HasGetter;
	internal readonly bool HasSetter;

	[DoesNotReturn]
	private static TResult InvalidGetter(VariableInfo info) =>
		ThrowHelper.ThrowInvalidOperationException<TResult>($"Variable '{info}' does not have a valid getter");

	[DoesNotReturn]
	private static void InvalidSetter(VariableInfo info) =>
		ThrowHelper.ThrowInvalidOperationException($"Variable '{info}' does not have a valid setter");

	internal TResult Get(TObject obj) =>
		Getter(obj);

	internal void Set(TObject obj, TResult value) =>
		Setter(obj, value);

	internal VariableStaticAccessor<TResult> Bind(TObject target) => new(
		Info,
		Getter.Method.CreateDelegate<Func<TResult>>(target),
		Setter.Method.CreateDelegate<Action<TResult>>(target)
	);

	internal VariableAccessor(VariableInfo info, Func<TObject, TResult>? getter, Action<TObject, TResult>? setter) {
		Info = info;
		HasGetter = getter is not null;
		Getter = getter ?? (_ => InvalidGetter(info));
		HasSetter = setter is not null;
		Setter = setter ?? ((_, _) => InvalidSetter(info));
	}
}
