/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Types.Exceptions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SpriteMaster;

[Pure]
internal static class ThrowHelper {
	internal static class Strings {
		internal const string AbstractNonCls = "This non-CLS method is not implemented.";
		internal const string ByDesign = "By Design";
	}

	#region Exception

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowException<T>(string message) =>
		throw new Exception(message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowException(string message) =>
		throw new Exception(message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowException<T>(string message, Exception innerException) =>
		throw new Exception(message, innerException);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowException(string message, Exception innerException) =>
		throw new Exception(message, innerException);

	#endregion

	#region NullReferenceException

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowNullReferenceException<T>(string message) =>
		throw new NullReferenceException(message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowNullReferenceException(string message) =>
		throw new NullReferenceException(message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowNullReferenceException<T>(string message, Exception innerException) =>
		throw new NullReferenceException(message, innerException);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowNullReferenceException(string message, Exception innerException) =>
		throw new NullReferenceException(message, innerException);

	#endregion

	#region ArgumentNullException

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowArgumentNullException<T>(string paramName) =>
		throw new ArgumentNullException(paramName);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowArgumentNullException(string paramName) =>
		throw new ArgumentNullException(paramName);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowArgumentNullException<T>(string paramName, string message) =>
		throw new ArgumentNullException(paramName, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowArgumentNullException(string paramName, string message) =>
		throw new ArgumentNullException(paramName, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowArgumentNullException<T>(string message, Exception innerException) =>
		throw new ArgumentNullException(message, innerException);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowArgumentNullException(string message, Exception innerException) =>
		throw new ArgumentNullException(message, innerException);

	#endregion

	#region InvalidOperationException

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowInvalidOperationException<T>(string message) =>
		throw new InvalidOperationException(message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowInvalidOperationException(string message) =>
		throw new InvalidOperationException(message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowInvalidOperationException<T>(string message, Exception innerException) =>
		throw new InvalidOperationException(message, innerException);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowInvalidOperationException(string message, Exception innerException) =>
		throw new InvalidOperationException(message, innerException);

	#endregion

	#region ObjectDisposedException

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowObjectDisposedException<T>(string objectName) =>
		throw new ObjectDisposedException(objectName);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowObjectDisposedException(string objectName) =>
		throw new ObjectDisposedException(objectName);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowObjectDisposedException<T>(string objectName, string message) =>
		throw new ObjectDisposedException(objectName, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowObjectDisposedException(string objectName, string message) =>
		throw new ObjectDisposedException(objectName, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowObjectDisposedException<T>(string message, Exception innerException) =>
		throw new ObjectDisposedException(message, innerException);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowObjectDisposedException(string message, Exception innerException) =>
		throw new ObjectDisposedException(message, innerException);

	#endregion

	#region ArgumentException

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowArgumentException<T>(string message) =>
		throw new ArgumentException(message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowArgumentException(string message) =>
		throw new ArgumentException(message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowArgumentException<T>(string message, Exception innerException) =>
		throw new ArgumentException(message, innerException);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowArgumentException(string message, Exception innerException) =>
		throw new ArgumentException(message, innerException);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowArgumentException<T>(string message, string paramName) =>
		throw new ArgumentException(message, paramName);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowArgumentException(string message, string paramName) =>
		throw new ArgumentException(message, paramName);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowArgumentException<T>(string message, string paramName, Exception innerException) =>
		throw new ArgumentException(message, paramName, innerException);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowArgumentException(string message, string paramName, Exception innerException) =>
		throw new ArgumentException(message, paramName, innerException);

	#endregion

	#region ArgumentOutOfRangeException

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowArgumentOutOfRangeException<T>(string paramName) =>
		throw new ArgumentOutOfRangeException(paramName);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowArgumentOutOfRangeException(string paramName) =>
		throw new ArgumentOutOfRangeException(paramName);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowArgumentOutOfRangeException<T>(string paramName, string message) =>
		throw new ArgumentOutOfRangeException(paramName, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowArgumentOutOfRangeException(string paramName, string message) =>
		throw new ArgumentOutOfRangeException(paramName, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowArgumentOutOfRangeException(string paramName, object? actualValue, string? message) =>
		throw new ArgumentOutOfRangeException(paramName, actualValue?.ToString(), message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowArgumentOutOfRangeException<T>(string paramName, object? actualValue, string? message) =>
		throw new ArgumentOutOfRangeException(paramName, actualValue?.ToString(), message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowArgumentOutOfRangeException<T>(string message, Exception innerException) =>
		throw new ArgumentOutOfRangeException(message, innerException);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowArgumentOutOfRangeException(string message, Exception innerException) =>
		throw new ArgumentOutOfRangeException(message, innerException);

	#endregion

	#region NotImplementedException

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowNotImplementedException<T>(string message) =>
		throw new NotImplementedException(message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowNotImplementedException(string message) =>
		throw new NotImplementedException(message);

	#endregion

	#region NotSupportedException

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowNotSupportedException<T>(string message) =>
		throw new NotSupportedException(message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowNotSupportedException(string message) =>
		throw new NotSupportedException(message);

	#endregion

	#region ReversePatchException

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowReversePatchException<T>([CallerMemberName] string member = null!) =>
		throw new ReversePatchException(member);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowReversePatchException([CallerMemberName] string member = null!) =>
		throw new ReversePatchException(member);

	#endregion

	#region UnknownArgumentException

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowUnknownArgumentException<TArgument, T>(string paramName, TArgument? value) =>
		throw new UnknownArgumentException<TArgument>(paramName, value);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowUnknownArgumentException<TArgument>(string paramName, TArgument? value) =>
		throw new UnknownArgumentException<TArgument>(paramName, value);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowUnknownArgumentException<TArgument, T>(string paramName, TArgument? value, string message) =>
		throw new UnknownArgumentException<TArgument>(paramName, value, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowUnknownArgumentException<TArgument>(string paramName, TArgument? value, string message) =>
		throw new UnknownArgumentException<TArgument>(paramName, value, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowUnknownArgumentException<T>(string paramName, object? value) =>
		throw new UnknownArgumentException(paramName, value);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowUnknownArgumentException(string paramName, object? value) =>
		throw new UnknownArgumentException(paramName, value);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowUnknownArgumentException<T>(string paramName, object? value, string message) =>
		throw new UnknownArgumentException(paramName, value, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowUnknownArgumentException(string paramName, object? value, string message) =>
		throw new UnknownArgumentException(paramName, value, message);

	#endregion

	#region UnknownArgumentEnumException

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowUnknownArgumentEnumException<TArgument, T>(string paramName, TArgument value) where TArgument : unmanaged, Enum =>
		throw new UnknownArgumentEnumException<TArgument>(paramName, value);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowUnknownArgumentEnumException<TArgument>(string paramName, TArgument value) where TArgument : unmanaged, Enum =>
		throw new UnknownArgumentEnumException<TArgument>(paramName, value);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowUnknownArgumentEnumException<TArgument, T>(string paramName, TArgument value, string message) where TArgument : unmanaged, Enum =>
		throw new UnknownArgumentEnumException<TArgument>(paramName, value, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowUnknownArgumentEnumException<TArgument>(string paramName, TArgument value, string message) where TArgument : unmanaged, Enum =>
		throw new UnknownArgumentEnumException<TArgument>(paramName, value, message);

	#endregion

	#region UnknownEnumException

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowUnknownEnumException<TArgument, T>(string varName, TArgument value) where TArgument : unmanaged, Enum =>
		throw new UnknownEnumException<TArgument>(varName, value);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowUnknownEnumException<TArgument>(string varName, TArgument value) where TArgument : unmanaged, Enum =>
		throw new UnknownEnumException<TArgument>(varName, value);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowUnknownEnumException<TArgument, T>(string varName, TArgument value, string message) where TArgument : unmanaged, Enum =>
		throw new UnknownEnumException<TArgument>(varName, value, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowUnknownEnumException<TArgument>(string varName, TArgument value, string message) where TArgument : unmanaged, Enum =>
		throw new UnknownEnumException<TArgument>(varName, value, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowUnknownEnumException<TArgument, T>(TArgument value) where TArgument : unmanaged, Enum =>
		throw new UnknownEnumException<TArgument>(value);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowUnknownEnumException<TArgument>(TArgument value) where TArgument : unmanaged, Enum =>
		throw new UnknownEnumException<TArgument>(value);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowUnknownEnumException<TArgument, T>(TArgument value, string message) where TArgument : unmanaged, Enum =>
		throw new UnknownEnumException<TArgument>(value, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowUnknownEnumException<TArgument>(TArgument value, string message) where TArgument : unmanaged, Enum =>
		throw new UnknownEnumException<TArgument>(value, message);

	#endregion

	#region UnknownValueException

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowUnknownValueException<TArgument, T>(string paramName, TArgument? value) =>
		throw new UnknownValueException<TArgument>(paramName, value);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowUnknownValueException<TArgument>(string paramName, TArgument? value) =>
		throw new UnknownValueException<TArgument>(paramName, value);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowUnknownValueException<TArgument, T>(string paramName, TArgument? value, string message) =>
		throw new UnknownValueException<TArgument>(paramName, value, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowUnknownValueException<TArgument>(string paramName, TArgument? value, string message) =>
		throw new UnknownValueException<TArgument>(paramName, value, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowUnknownValueException<T>(string paramName, object? value) =>
		throw new UnknownValueException(paramName, value);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowUnknownValueException(string paramName, object? value) =>
		throw new UnknownValueException(paramName, value);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowUnknownValueException<T>(string paramName, object? value, string message) =>
		throw new UnknownValueException(paramName, value, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowUnknownValueException(string paramName, object? value, string message) =>
		throw new UnknownValueException(paramName, value, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowUnknownValueException<TArgument, T>(TArgument? value) =>
		throw new UnknownValueException<TArgument>(value);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowUnknownValueException<TArgument>(TArgument? value) =>
		throw new UnknownValueException<TArgument>(value);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowUnknownValueException<TArgument, T>(TArgument? value, string message) =>
		throw new UnknownValueException<TArgument>(value, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowUnknownValueException<TArgument>(TArgument? value, string message) =>
		throw new UnknownValueException<TArgument>(value, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowUnknownValueException<T>(object? value) =>
		throw new UnknownValueException(value);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowUnknownValueException(object? value) =>
		throw new UnknownValueException(value);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowUnknownValueException<T>(object? value, string message) =>
		throw new UnknownValueException(value, message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowUnknownValueException(object? value, string message) =>
		throw new UnknownValueException(value, message);

	#endregion

	#region InvalidTypeParameterException

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowInvalidTypeParameterException<T>() =>
		throw new InvalidTypeParameterException();

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowInvalidTypeParameterException() =>
		throw new InvalidTypeParameterException();

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowInvalidTypeParameterException<T>(Type type) =>
		throw new InvalidTypeParameterException(type);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowInvalidTypeParameterException(Type type) =>
		throw new InvalidTypeParameterException(type);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowInvalidTypeParameterException<T>(string message) =>
		throw new InvalidTypeParameterException(message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowInvalidTypeParameterException(string message) =>
		throw new InvalidTypeParameterException(message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowInvalidTypeParameterException<T>(string message, Type type) =>
		throw new InvalidTypeParameterException(message, type);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowInvalidTypeParameterException(string message, Type type) =>
		throw new InvalidTypeParameterException(message, type);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowInvalidTypeParameterException<T>(string message, Exception innerException) =>
		throw new InvalidTypeParameterException(message, innerException);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowInvalidTypeParameterException(string message, Exception innerException) =>
		throw new InvalidTypeParameterException(message, innerException);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowInvalidTypeParameterException<T>(string message, Type type, Exception innerException) =>
		throw new InvalidTypeParameterException(message, type, innerException);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowInvalidTypeParameterException(string message, Type type, Exception innerException) =>
		throw new InvalidTypeParameterException(message, type, innerException);

	#endregion

	#region FormatException

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowFormatException<T>(string message) =>
		throw new FormatException(message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowFormatException(string message) =>
		throw new FormatException(message);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static T ThrowFormatException<T>(string message, Exception innerException) =>
		throw new FormatException(message, innerException);

	[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowFormatException(string message, Exception innerException) =>
		throw new FormatException(message, innerException);

	#endregion

}
