/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using MusicMaster.Extensions;
using MusicMaster.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MusicMaster;

internal static class Contracts {
	[DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsExceptionType(this Type type) => type.IsSubclassOf(typeof(Exception));

	internal delegate bool ClosedPredicate();

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void CheckExceptionType(Type? type, [CallerArgumentExpression("type")] string expression = "") {
		if (type is not null && !type.IsExceptionType()) {
			ThrowHelper.ThrowArgumentOutOfRangeException(
				expression,
				type,
				"Provided assert exception type is not a subclass of Exception"
			);
		}
	}

	[DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string WrapExpression(string expression) =>
		expression.AllF(char.IsLetterOrDigit) ? $"({expression})" : expression;

	[DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string AppendMessage(string message, string expression) {
		return $"{message} ({expression})";
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DoesNotReturn, DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.NoInlining)]
	private static void ThrowException(Type? exceptionType, string message) {
		throw Activator.CreateInstance(
			exceptionType ?? typeof(ArgumentOutOfRangeException),
			Arrays.Singleton<object>(
				message
			)
		) as Exception ??
		new ArgumentOutOfRangeException(
			message
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AssertDefault<T>(
		this T value,
		string message = "Variable is not default",
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		Assert(
			EqualityComparer<T>.Default.Equals(
				value,
				default
			),
			message,
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} is default"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AssertNotDefault<T>(
		this T value,
		string message = "Variable is default",
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		Assert(
			!EqualityComparer<T>.Default.Equals(
				value,
				default
			),
			message,
			exception ?? typeof(NullReferenceException),
			$"{WrapExpression(valueExpression)} is not default"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AssertNull<T>(
		this T value,
		string message = "Variable is not null",
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		Assert(
			value is null,
			message,
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} is null"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AssertNotNull<T>(
		this T value,
		string message = "Variable is null",
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		Assert(
			value is not null,
			message,
			exception ?? typeof(NullReferenceException),
			$"{WrapExpression(valueExpression)} is not null"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AssertTrue(
		this bool value,
		string message = "Variable is not true",
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		Assert(
			value is true,
			message,
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} is true"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AssertTrue<T>(
		this T value,
		string message = "Variable is not true",
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	)
		where T : IConvertible {
		Assert(
			Convert.ToBoolean(
				value
			),
			message,
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} is true"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AssertFalse(
		this bool value,
		string message = "Variable is not false",
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		Assert(
			value is false,
			message,
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} is false"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AssertFalse<T>(
		this T value,
		string message = "Variable is not false",
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	)
		where T : IConvertible {
		Assert(
			Convert.ToBoolean(
				value
			) ==
			false,
			message,
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} is false"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Assert(
		bool predicate,
		string message = "Variable's value is invalid",
		Type? exception = null,
		[CallerArgumentExpression(
			"predicate"
		)]
		string predicateExpression = ""
	) {
		CheckExceptionType(exception);
		if (!predicate) {
			Debugger.Break();
			ThrowException(exception ?? typeof(ArgumentException), AppendMessage(message, predicateExpression));
		}
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Assert(
		ClosedPredicate predicate,
		string message = "Variable failed predicated assertion",
		Type? exception = null,
		[CallerArgumentExpression(
			"predicate"
		)]
		string predicateExpression = ""
	) {
		Assert(predicate.Invoke(), message, exception ?? typeof(ArgumentOutOfRangeException), predicateExpression);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Assert<T>(
		in T value,
		Predicate<T> predicate,
		string message = "Variable failed predicated assertion",
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		Assert(predicate.Invoke(value), message, exception ?? typeof(ArgumentOutOfRangeException), valueExpression);
	}

	[DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool EqualityPredicate<TFirst, TSecond>(this TFirst value, TSecond reference)
		where TFirst : IComparable, IComparable<TSecond>, IEquatable<TSecond>
		where TSecond : IComparable, IComparable<TFirst>, IEquatable<TFirst> {
		return true switch {
			_ when typeof(TFirst) == typeof(string) && typeof(TSecond) == typeof(string) =>
				(string)(object)value == (string)(object)reference,
			_ when typeof(TFirst) == typeof(TSecond) =>
				EqualityComparer<TFirst>.Default.Equals(value, (TFirst)(object)reference),
			_ when typeof(TFirst).IsAssignableFrom(typeof(TFirst)) =>
				EqualityComparer<TFirst>.Default.Equals(value, (TFirst)(object)reference),
			_ when typeof(TFirst).IsAssignableTo(typeof(TFirst)) =>
				EqualityComparer<TSecond>.Default.Equals((TSecond)(object)value, reference),
			_ when typeof(TFirst).IsSubclassOf(typeof(IEquatable<TSecond>)) =>
				value.Equals(reference),
			_ when typeof(TSecond).IsSubclassOf(typeof(IEquatable<TFirst>)) =>
				reference.Equals(value),
			_ =>
				value.CompareTo(reference) == 0
		};
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertEqual<TFirst, TSecond>(
		this TFirst value,
		TSecond reference,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = "",
		[CallerArgumentExpression(
			"reference"
		)]
		string referenceExpression = ""
	)
		where TFirst : IComparable, IComparable<TSecond>, IEquatable<TSecond>
		where TSecond : IComparable, IComparable<TFirst>, IEquatable<TFirst> {
		Assert(
			EqualityPredicate(
				value,
				reference
			),
			message ?? $"Variable '{value}' is not equal to '{reference}'",
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} == {WrapExpression(referenceExpression)}"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertNotEqual<TFirst, TSecond>(
		this TFirst value,
		TSecond reference,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = "",
		[CallerArgumentExpression(
			"reference"
		)]
		string referenceExpression = ""
	)
		where TFirst : IComparable, IComparable<TSecond>, IEquatable<TSecond>
		where TSecond : IComparable, IComparable<TFirst>, IEquatable<TFirst> {
		Assert(
			!EqualityPredicate(
				value,
				reference
			),
			message ?? $"Variable '{value}' is equal to '{reference}'",
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} != {WrapExpression(referenceExpression)}"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertGreater<TFirst, TSecond>(
		this TFirst value,
		TSecond reference,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = "",
		[CallerArgumentExpression(
			"reference"
		)]
		string referenceExpression = ""
	)
		where TFirst : IComparable, IComparable<TSecond>
		where TSecond : IComparable, IComparable<TFirst> {
		static bool Predicate(TFirst value, TSecond reference) {
			return value.CompareTo(reference) > 0;
		}

		Assert(
			Predicate(
				value,
				reference
			),
			message ?? $"Variable '{value}' is less than or equal to '{reference}'",
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} > {WrapExpression(referenceExpression)}"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertGreaterEqual<TFirst, TSecond>(
		this TFirst value,
		TSecond reference,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = "",
		[CallerArgumentExpression(
			"reference"
		)]
		string referenceExpression = ""
	)
		where TFirst : IComparable, IComparable<TSecond>
		where TSecond : IComparable, IComparable<TFirst> {
		static bool Predicate(TFirst value, TSecond reference) {
			return value.CompareTo(reference) >= 0;
		}

		Assert(
			Predicate(
				value,
				reference
			),
			message ?? $"Variable '{value}' is less than to '{reference}'",
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} >= {WrapExpression(referenceExpression)}"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertLess<TFirst, TSecond>(
		this TFirst value,
		TSecond reference,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = "",
		[CallerArgumentExpression(
			"reference"
		)]
		string referenceExpression = ""
	)
		where TFirst : IComparable, IComparable<TSecond>
		where TSecond : IComparable, IComparable<TFirst> {
		static bool Predicate(TFirst value, TSecond reference) {
			return value.CompareTo(reference) < 0;
		}

		Assert(
			Predicate(
				value,
				reference
			),
			message ?? $"Variable '{value}' is greater than or equal to '{reference}'",
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} < {WrapExpression(referenceExpression)}"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertLessEqual<TFirst, TSecond>(
		this TFirst value,
		TSecond reference,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = "",
		[CallerArgumentExpression(
			"reference"
		)]
		string referenceExpression = ""
	)
		where TFirst : IComparable, IComparable<TSecond>
		where TSecond : IComparable, IComparable<TFirst> {
		static bool Predicate(TFirst value, TSecond reference) {
			return value.CompareTo(reference) <= 0;
		}

		Assert(
			Predicate(
				value,
				reference
			),
			message ?? $"Variable '{value}' is greater than '{reference}'",
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} <= {WrapExpression(referenceExpression)}"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertAligned(
		this nuint value,
		nuint alignment,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = "",
		[CallerArgumentExpression("alignment")]
		string alignmentExpression = ""
	) {
		AssertZero(
			value % alignment,
			message ?? $"Variable '{value}' is not aligned to '{alignment}'",
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} %% {WrapExpression(alignmentExpression)}"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertAligned(
		this nint value,
		nint alignment,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = "",
		[CallerArgumentExpression("alignment")]
		string alignmentExpression = ""
	) {
		AssertZero(
			value % alignment,
			message ?? $"Variable '{value}' is not aligned to '{alignment}'",
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} %% {WrapExpression(alignmentExpression)}"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertAligned(
		this byte value,
		byte alignment,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = "",
		[CallerArgumentExpression("alignment")]
		string alignmentExpression = ""
	) {
		AssertZero(
			value % alignment,
			message ?? $"Variable '{value}' is not aligned to '{alignment}'",
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} %% {WrapExpression(alignmentExpression)}"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertAligned(
		this sbyte value,
		sbyte alignment,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = "",
		[CallerArgumentExpression("alignment")]
		string alignmentExpression = ""
	) {
		AssertZero(
			value % alignment,
			message ?? $"Variable '{value}' is not aligned to '{alignment}'",
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} %% {WrapExpression(alignmentExpression)}"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertAligned(
		this short value,
		short alignment,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = "",
		[CallerArgumentExpression("alignment")]
		string alignmentExpression = ""
	) {
		AssertZero(
			value % alignment,
			message ?? $"Variable '{value}' is not aligned to '{alignment}'",
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} %% {WrapExpression(alignmentExpression)}"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertAligned(
		this ushort value,
		ushort alignment,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = "",
		[CallerArgumentExpression("alignment")]
		string alignmentExpression = ""
	) {
		AssertZero(
			value % alignment,
			message ?? $"Variable '{value}' is not aligned to '{alignment}'",
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} %% {WrapExpression(alignmentExpression)}"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertAligned(
		this int value,
		int alignment,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = "",
		[CallerArgumentExpression("alignment")]
		string alignmentExpression = ""
	) {
		AssertZero(
			value % alignment,
			message ?? $"Variable '{value}' is not aligned to '{alignment}'",
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} %% {WrapExpression(alignmentExpression)}"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertAligned(
		this uint value,
		uint alignment,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = "",
		[CallerArgumentExpression("alignment")]
		string alignmentExpression = ""
	) {
		AssertZero(
			value % alignment,
			message ?? $"Variable '{value}' is not aligned to '{alignment}'",
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} %% {WrapExpression(alignmentExpression)}"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertAligned(
		this long value,
		long alignment,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = "",
		[CallerArgumentExpression("alignment")]
		string alignmentExpression = ""
	) {
		AssertZero(
			value % alignment,
			message ?? $"Variable '{value}' is not aligned to '{alignment}'",
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} %% {WrapExpression(alignmentExpression)}"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertAligned(
		this ulong value,
		ulong alignment,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = "",
		[CallerArgumentExpression("alignment")]
		string alignmentExpression = ""
	) {
		AssertZero(
			value % alignment,
			message ?? $"Variable '{value}' is not aligned to '{alignment}'",
			exception ?? typeof(ArgumentOutOfRangeException),
			$"{WrapExpression(valueExpression)} %% {WrapExpression(alignmentExpression)}"
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertZero(
		this nuint value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		AssertEqual(
			value,
			(nuint)0,
			message ?? $"Variable '{value}' is not equal to zero",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertZero(
		this nint value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		AssertEqual(
			value,
			(nint)0,
			message ?? $"Variable '{value}' is not equal to zero",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertZero<T>(
		this T value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	)
		where T : IComparable, IComparable<T>, IEquatable<T>, IConvertible {
		AssertEqual(
			value,
			(T)Convert.ChangeType(0, typeof(T)),
			message ?? $"Variable '{value}' is not equal to zero",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertNotZero(
		this nuint value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		AssertNotEqual(
			value,
			(nuint)0,
			message ?? $"Variable '{value}' is equal to zero",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertNotZero(
		this nint value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		AssertNotEqual(
			value,
			(nint)0,
			message ?? $"Variable '{value}' is equal to zero",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertNotZero<T>(
		this T value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	)
		where T : IComparable, IComparable<T>, IEquatable<T>, IConvertible {
		AssertNotEqual(
			value,
			(T)Convert.ChangeType(0, typeof(T)),
			message ?? $"Variable '{value}' is equal to zero",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertPositive(
		this nint value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		AssertGreater(
			value,
			(nint)0,
			message ?? $"Variable '{value}' is not positive",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertPositive<T>(
		this T value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	)
		where T : IComparable, IComparable<T>, IEquatable<T>, IConvertible {
		AssertGreater(
			value,
			(T)Convert.ChangeType(0, typeof(T)),
			message ?? $"Variable '{value}' is not positive",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertPositiveOrZero(
		this nuint value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		AssertGreaterEqual(
			value,
			(nuint)0,
			message ?? $"Variable '{value}' is not positive or zero",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertPositiveOrZero(
		this nint value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		AssertGreaterEqual(
			value,
			(nint)0,
			message ?? $"Variable '{value}' is not positive or zero",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertPositiveOrZero<T>(
		this T value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	)
		where T : IComparable, IComparable<T>, IEquatable<T>, IConvertible {
		AssertGreaterEqual(
			value,
			(T)Convert.ChangeType(0, typeof(T)),
			message ?? $"Variable '{value}' is not positive or zero",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertNotNegative(
		this nint value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		AssertGreaterEqual(
			value,
			(nint)0,
			message ?? $"Variable '{value}' is negative",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertNotNegative<T>(
		this T value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	)
		where T : IComparable, IComparable<T>, IEquatable<T>, IConvertible {
		AssertGreaterEqual(
			value,
			(T)Convert.ChangeType(0, typeof(T)),
			message ?? $"Variable '{value}' is negative",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertNegative(
		this nint value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		AssertLess(
			value,
			(nint)0,
			message ?? $"Variable '{value}' is not negative",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertNegative<T>(
		this T value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	)
		where T : IComparable, IComparable<T>, IEquatable<T>, IConvertible {
		AssertLess(
			value,
			(T)Convert.ChangeType(0, typeof(T)),
			message ?? $"Variable '{value}' is not negative",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertNegativeOrZero(
		this nuint value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		AssertLessEqual(
			value,
			(nuint)0,
			message ?? $"Variable '{value}' is not negative or zero",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertNegativeOrZero(
		this nint value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		AssertLessEqual(
			value,
			(nint)0,
			message ?? $"Variable '{value}' is not negative or zero",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertNegativeOrZero<T>(
		this T value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	)
		where T : IComparable, IComparable<T>, IEquatable<T>, IConvertible {
		AssertLessEqual(
			value,
			(T)Convert.ChangeType(0, typeof(T)),
			message ?? $"Variable '{value}' is not negative or zero",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertNotPositive(
		this nuint value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		AssertLessEqual(
			value,
			(nuint)0,
			message ?? $"Variable '{value}' is positive",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertNotPositive(
		this nint value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	) {
		AssertLessEqual(
			value,
			(nint)0,
			message ?? $"Variable '{value}' is not negative or zero",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	[Conditional("CONTRACTS_FULL"), Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	internal static void AssertNotPositive<T>(
		this T value,
		string? message = null,
		Type? exception = null,
		[CallerArgumentExpression("value")]
		string valueExpression = ""
	)
		where T : IComparable, IComparable<T>, IEquatable<T>, IConvertible {
		AssertLessEqual(
			value,
			(T)Convert.ChangeType(0, typeof(T)),
			message ?? $"Variable '{value}' is positive",
			exception ?? typeof(ArgumentOutOfRangeException),
			valueExpression
		);
	}

	// TODO : Integer overloads for most asserts
	// TODO : Implement a check for ==/!= operators?
}
