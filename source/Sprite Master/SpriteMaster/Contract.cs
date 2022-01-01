/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Types;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SpriteMaster;
static class Contract {
	[DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static private bool IsExceptionType(this Type type) => type.IsSubclassOf(typeof(Exception));

	internal delegate bool ClosedPredicate();

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void AssertNull<T>(this T value, in string message = "Variable is not null", Type exception = null) {
		Assert(value is null, message, exception ?? typeof(ArgumentOutOfRangeException));
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void AssertNotNull<T>(this T value, in string message = "Variable is null", Type exception = null) {
		Assert(value is not null, message, exception ?? typeof(NullReferenceException));
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void AssertTrue(this bool value, in string message = "Variable is not true", Type exception = null) {
		Assert(value == true, message, exception ?? typeof(ArgumentOutOfRangeException));
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void AssertFalse(this bool value, in string message = "Variable is not false", Type exception = null) {
		Assert(value == false, message, exception ?? typeof(ArgumentOutOfRangeException));
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void Assert(bool predicate, in string message = "Variable's value is invalid", Type exception = null) {
		if (exception is not null && !exception.IsExceptionType()) {
			throw new ArgumentOutOfRangeException("Provided assert exception type is not a subclass of Exception");
		}
		if (!predicate) {
			throw (ArgumentOutOfRangeException)Activator.CreateInstance(exception ?? typeof(ArgumentOutOfRangeException), Arrays.Singleton(message));
		}
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void Assert(in ClosedPredicate predicate, in string message = "Variable failed predicated assertion", in Type exception = null) {
		if (predicate is null) {
			throw new ArgumentNullException($"Argument '{nameof(predicate)}' is null");
		}
		Assert(predicate.Invoke(), message, exception ?? typeof(ArgumentOutOfRangeException));
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void Assert<T>(in T value, in Predicate<T> predicate, in string message = "Variable failed predicated assertion", in Type exception = null) {
		if (predicate is null) {
			throw new ArgumentNullException($"Argument '{nameof(predicate)}' is null");
		}
		Assert(predicate.Invoke(value), message, exception ?? typeof(ArgumentOutOfRangeException));
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void AssertEqual<T, U>(this T value, in U reference, in string message = null, Type exception = null)
		where T : IComparable, IComparable<U>, IEquatable<U>
		where U : IComparable, IComparable<T>, IEquatable<T> {
		static bool Predicate(in T value, in U reference) {
			if (typeof(T).IsSubclassOf(typeof(IEquatable<U>))) {
				return value.Equals(reference);
			}
			else {
				return value.CompareTo(reference) == 0;
			}
		}

		Assert(Predicate(value, reference), message ?? $"Variable '{value}' is not equal to '{reference}'", exception ?? typeof(ArgumentOutOfRangeException));
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void AssertNotEqual<T, U>(this T value, in U reference, in string message = null, Type exception = null)
		where T : IComparable, IComparable<U>, IEquatable<U>
		where U : IComparable, IComparable<T>, IEquatable<T> {
		static bool Predicate(in T value, in U reference) {
			if (typeof(T).IsSubclassOf(typeof(IEquatable<U>))) {
				return !value.Equals(reference);
			}
			else {
				return value.CompareTo(reference) != 0;
			}
		}

		Assert(Predicate(value, reference), message ?? $"Variable '{value}' is equal to '{reference}'", exception ?? typeof(ArgumentOutOfRangeException));
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void AssertGreater<T, U>(this T value, in U reference, in string message = null, Type exception = null)
		where T : IComparable, IComparable<U>
		where U : IComparable, IComparable<T> {
		static bool Predicate(in T value, in U reference) {
			return value.CompareTo(reference) > 0;
		}

		Assert(Predicate(value, reference), message ?? $"Variable '{value}' is less than or equal to '{reference}'", exception ?? typeof(ArgumentOutOfRangeException));
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void AssertGreaterEqual<T, U>(this T value, in U reference, in string message = null, Type exception = null)
		where T : IComparable, IComparable<U>
		where U : IComparable, IComparable<T> {
		static bool Predicate(in T value, in U reference) {
			return value.CompareTo(reference) >= 0;
		}

		Assert(Predicate(value, reference), message ?? $"Variable '{value}' is less than to '{reference}'", exception ?? typeof(ArgumentOutOfRangeException));
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void AssertLess<T, U>(this T value, in U reference, in string message = null, Type exception = null)
		where T : IComparable, IComparable<U>
		where U : IComparable, IComparable<T> {
		static bool Predicate(in T value, in U reference) {
			return value.CompareTo(reference) < 0;
		}

		Assert(Predicate(value, reference), message ?? $"Variable '{value}' is greater than or equal to '{reference}'", exception ?? typeof(ArgumentOutOfRangeException));
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void AssertLessEqual<T, U>(this T value, in U reference, in string message = null, Type exception = null)
		where T : IComparable, IComparable<U>
		where U : IComparable, IComparable<T> {
		static bool Predicate(in T value, in U reference) {
			return value.CompareTo(reference) <= 0;
		}

		Assert(Predicate(value, reference), message ?? $"Variable '{value}' is greater than '{reference}'", exception ?? typeof(ArgumentOutOfRangeException));
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void AssertZero<T>(this T value, in string message = null, Type exception = null) where T : IComparable, IComparable<T>, IEquatable<T>, IConvertible {
		AssertEqual(
			value,
			(T)Convert.ChangeType(0, typeof(T)),
			message ?? $"Variable '{value}' is not equal to zero",
			exception ?? typeof(ArgumentOutOfRangeException)
		);
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void AssertOne<T>(this T value, in string message = null, Type exception = null) where T : IComparable, IComparable<T>, IEquatable<T>, IConvertible {
		AssertEqual(
			value,
			(T)Convert.ChangeType(1, typeof(T)),
			message ?? $"Variable '{value}' is not equal to one",
			exception ?? typeof(ArgumentOutOfRangeException)
		);
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void AssertNotZero<T>(this T value, in string message = null, Type exception = null) where T : IComparable, IComparable<T>, IEquatable<T>, IConvertible {
		AssertNotEqual(
			value,
			(T)Convert.ChangeType(0, typeof(T)),
			message ?? $"Variable '{value}' is equal to zero",
			exception ?? typeof(ArgumentOutOfRangeException)
		);
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void AssertPositive<T>(this T value, in string message = null, Type exception = null) where T : IComparable, IComparable<T>, IEquatable<T>, IConvertible {
		AssertGreater(
			value,
			(T)Convert.ChangeType(0, typeof(T)),
			message ?? $"Variable '{value}' is not positive",
			exception ?? typeof(ArgumentOutOfRangeException)
		);
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void AssertPositiveOrZero<T>(this T value, in string message = null, Type exception = null) where T : IComparable, IComparable<T>, IEquatable<T>, IConvertible {
		AssertGreaterEqual(
			value,
			(T)Convert.ChangeType(0, typeof(T)),
			message ?? $"Variable '{value}' is not positive or zero",
			exception ?? typeof(ArgumentOutOfRangeException)
		);
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void AssertNotNegative<T>(this T value, in string message = null, Type exception = null) where T : IComparable, IComparable<T>, IEquatable<T>, IConvertible {
		AssertPositiveOrZero(value, message, exception);
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void AssertNegative<T>(this T value, in string message = null, Type exception = null) where T : IComparable, IComparable<T>, IEquatable<T>, IConvertible {
		AssertLess(
			value,
			(T)Convert.ChangeType(0, typeof(T)),
			message ?? $"Variable '{value}' is not negative",
			exception ?? typeof(ArgumentOutOfRangeException)
		);
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void AssertNegativeOrZero<T>(this T value, in string message = null, Type exception = null) where T : IComparable, IComparable<T>, IEquatable<T>, IConvertible {
		AssertLessEqual(
			value,
			(T)Convert.ChangeType(0, typeof(T)),
			message ?? $"Variable '{value}' is not negative or zero",
			exception ?? typeof(ArgumentOutOfRangeException)
		);
	}

	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(Runtime.MethodImpl.Hot)]
	static internal void AssertNotPositive<T>(this T value, in string message = null, Type exception = null) where T : IComparable, IComparable<T>, IEquatable<T>, IConvertible {
		AssertNegativeOrZero(value, message, exception);
	}

	// TODO : Integer overloads for most asserts
	// TODO : Implement a check for ==/!= operators?
}
