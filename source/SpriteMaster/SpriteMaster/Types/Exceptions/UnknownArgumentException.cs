/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Collections.Generic;

namespace SpriteMaster.Types.Exceptions;

internal class UnknownArgumentException : ArgumentOutOfRangeException, IUnknownValueException {
	private static string GetDefaultMessage(object? value) => $"Unknown Argument Value: '{value}'";
	internal readonly object? Value;
	public virtual IEnumerable<object>? LegalValues => null;

	string? IUnknownValueException.Name => ParamName;
	object? IUnknownValueException.Value => Value;
	
	internal UnknownArgumentException(string paramName, object? value) : this(paramName, value, GetDefaultMessage(value)) {
	}

	internal UnknownArgumentException(string paramName, object? value, string message) : base(paramName, value, message) {
		Value = value;
	}
}

internal class UnknownArgumentException<TArgument> : UnknownArgumentException, IUnknownValueException<TArgument> {
	private static string GetDefaultMessage(in TArgument? value) => $"Unknown Argument Value: '{value}'";
	internal new readonly TArgument? Value;

	TArgument? IUnknownValueException<TArgument>.Value => Value;
	public new virtual IEnumerable<TArgument>? LegalValues => null;

	internal UnknownArgumentException(string paramName, in TArgument? value) : this(paramName, value, GetDefaultMessage(value)) {
	}

	internal UnknownArgumentException(string paramName, in TArgument? value, string message) : base(paramName, value, message) {
		Value = value;
	}
}
