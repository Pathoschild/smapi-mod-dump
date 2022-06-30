/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types.Exceptions;

/// <summary>
/// Indicates that the generic type parameter is invalid or does not match the required constraints.
/// </summary>
internal class InvalidTypeParameterException : InvalidOperationException {
	private const string DefaultMessage = "Type Parameter is invalid";

	/// <summary>Initializes a new instance of the <see cref="InvalidTypeParameterException" /> <see langword="class"/>.</summary>
	internal InvalidTypeParameterException()
		: this(DefaultMessage) {
	}

	/// <summary>Initializes a new instance of the <see cref="InvalidTypeParameterException" /> <see langword="class"/> with a specified <paramref name="message">error message</paramref>.</summary>
	/// <param name="message">The message that describes the error.</param>
	internal InvalidTypeParameterException(string? message)
		: base(message ?? DefaultMessage) {
		this.HResult = -2146233079;
	}

	/// <summary>Initializes a new instance of the <see cref="InvalidTypeParameterException" /> <see langword="class"/> with a specified <paramref name="message">error message</paramref> and a reference to the <paramref name="innerException">inner exception</paramref> that is the cause of this <see cref="Exception" />.</summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a <see langword="null" /> reference (<see langword="Nothing" /> in Visual Basic), the current exception is raised in a <see langword="catch" /> block that handles the inner exception.</param>
	internal InvalidTypeParameterException(string? message, Exception? innerException)
		: base(message ?? DefaultMessage, innerException) {
		this.HResult = -2146233079;
	}

	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	internal void Throw() =>
		throw new InvalidTypeParameterException();
}

/// <summary>
/// Indicates that the generic type parameter (<seealso cref="TParameter"/>) is invalid or does not match the required constraints.
/// </summary>
internal class InvalidTypeParameterException<TParameter> : InvalidTypeParameterException {
	private static readonly string DefaultMessage = string.Intern($"Type Parameter '{typeof(TParameter).GetTypeName()}' is invalid");

	/// <summary>Initializes a new instance of the <see cref="InvalidTypeParameterException{TParameter}" /> <see langword="class"/>.</summary>
	internal InvalidTypeParameterException()
		: this(DefaultMessage) {
	}

	/// <summary>Initializes a new instance of the <see cref="InvalidTypeParameterException{TParameter}" /> <see langword="class"/> with a specified <paramref name="message">error message</paramref>.</summary>
	/// <param name="message">The message that describes the error.</param>
	internal InvalidTypeParameterException(string? message)
		: base(message ?? DefaultMessage) {
		this.HResult = -2146233079;
	}

	/// <summary>Initializes a new instance of the <see cref="InvalidTypeParameterException{TParameter}" /> <see langword="class"/> with a specified <paramref name="message">error message</paramref> and a reference to the <paramref name="innerException">inner exception</paramref> that is the cause of this <see cref="Exception" />.</summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a <see langword="null" /> reference (<see langword="Nothing" /> in Visual Basic), the current exception is raised in a <see langword="catch" /> block that handles the inner exception.</param>
	internal InvalidTypeParameterException(string? message, Exception? innerException)
		: base(message ?? DefaultMessage, innerException) {
		this.HResult = -2146233079;
	}

	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	internal new void Throw() =>
		throw new InvalidTypeParameterException();
}
