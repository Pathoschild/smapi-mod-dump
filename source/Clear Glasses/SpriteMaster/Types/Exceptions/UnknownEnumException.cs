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
using System.Linq;

namespace SpriteMaster.Types.Exceptions;

internal interface IUnknownEnumException {
	protected IEnumerable<Enum> LegalEnumValues { get; }
	Enum Value { get; }
	IEnumerable<Enum> LegalValues => LegalEnumValues;
}

internal interface IUnknownEnumException<out TArgument> : IUnknownEnumException, IUnknownValueException<TArgument> where TArgument : unmanaged, Enum {
	protected static readonly Lazy<IEnumerable<TArgument>> StaticLegalValues = new(Enum.GetValues<TArgument>);

	TArgument IUnknownValueException<TArgument>.Value => Value;
	internal new TArgument Value { get; }

	IEnumerable<Enum> IUnknownEnumException.LegalEnumValues => StaticLegalValues.Value.Cast<Enum>();
	IEnumerable<TArgument> IUnknownValueException<TArgument>.LegalValues => StaticLegalValues.Value;

	Enum IUnknownEnumException.Value => Value;

	new IEnumerable<TArgument> LegalValues => StaticLegalValues.Value;
}

internal class UnknownEnumException<TArgument> : UnknownValueException<TArgument>, IUnknownEnumException<TArgument>
	where TArgument : unmanaged, Enum {
	TArgument IUnknownEnumException<TArgument>.Value => Value;
	internal new TArgument Value => (TArgument)((UnknownValueException)this).Value!;

	IEnumerable<TArgument> IUnknownEnumException<TArgument>.LegalValues => IUnknownEnumException<TArgument>.StaticLegalValues.Value;

	internal UnknownEnumException(string varName, in TArgument value) : base(varName, value, $"Unknown '{varName}' Enum Value") {
	}

	internal UnknownEnumException(string varName, in TArgument value, string message) : base(varName, value, message) {
	}

	internal UnknownEnumException(in TArgument value) : base(value, $"Unknown Enum Value") {
	}

	internal UnknownEnumException(in TArgument value, string message) : base(value, message) {
	}
}