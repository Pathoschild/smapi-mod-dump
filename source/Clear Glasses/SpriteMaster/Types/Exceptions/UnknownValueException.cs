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
using System.Collections.Generic;

namespace SpriteMaster.Types.Exceptions;

internal interface IUnknownValueException {
	internal string? Name { get; }
	internal object? Value { get; }

	IEnumerable<object>? LegalValues { get; }
}

internal interface IUnknownValueException<out TArgument> : IUnknownValueException {
	internal new TArgument? Value { get; }

	new IEnumerable<TArgument>? LegalValues { get; }
}

internal class UnknownValueException : InvalidOperationException, IUnknownValueException {
	internal readonly string? Name;
	internal readonly object? Value;

	string? IUnknownValueException.Name => Name;
	object? IUnknownValueException.Value => Value;

	public virtual IEnumerable<object>? LegalValues => null;

	internal UnknownValueException(string varName, object? value) : base($"Unknown '{varName}' Value: '{value}'") {
		Name = varName;
		Value = value;
	}

	internal UnknownValueException(string varName, object? value, string message) : base($"{message} '{varName}': '{value}'") {
		Name = varName;
		Value = value;
	}

	internal UnknownValueException(object? value) : base($"Unknown Value: '{value}'") {
		Value = value;
	}

	internal UnknownValueException(object? value, string message) : base($"{message}: '{value}'") {
		Value = value;
	}
}

internal class UnknownValueException<TArgument> : UnknownValueException, IUnknownValueException<TArgument> {
	internal new TArgument? Value => (TArgument?)((UnknownValueException)this).Value;
	TArgument? IUnknownValueException<TArgument>.Value => Value;

	public new virtual IEnumerable<TArgument>? LegalValues => null;

	internal UnknownValueException(string varName, in TArgument? value) : base(varName, value) {
	}

	internal UnknownValueException(string varName, in TArgument? value, string message) : base(varName, value, message) {
	}

	internal UnknownValueException(in TArgument? value) : base(value) {
	}

	internal UnknownValueException(in TArgument? value, string message) : base(value, message) {
	}
}
