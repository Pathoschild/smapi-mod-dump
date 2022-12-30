/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Leclair.Stardew.ThemeManager.VariableSets;

public class TypedVariableSet<TValue> : BaseVariableSet<TValue> {

	public delegate bool ParserDelegate(string input, IThemeManager? manager, IThemeManifest? manifest, [NotNullWhen(true)] out object? value);

	public static readonly Dictionary<Type, (Type, ParserDelegate)> ValueParsers = new();

	public static bool RegisterType<TVal>(IThemeManagerApi.TryParseVariableSetValue<TVal> parser) {
		lock((ValueParsers as ICollection).SyncRoot) {
			if (ValueParsers.ContainsKey(typeof(TVal)))
				return false;

			bool Invoke(string input, IThemeManager? manager, IThemeManifest? manifest, [NotNullWhen(true)] out object? value) {
				if (parser(input, manager, manifest, out var val)) {
					value = val;
					return true;
				}

				value = null;
				return false;
			}

			ValueParsers[typeof(TVal)] = (
				typeof(TypedVariableSet<TVal>),
				Invoke
			);

			return true;
		}
	}

	public static bool CanHandleType(Type? type) {
		if (type is null)
			return false;

		lock((ValueParsers as ICollection).SyncRoot) {
			return ValueParsers.ContainsKey(type);
		}
	}

	public static IVariableSet? CreateInstance(Type? type) {
		if (type is null)
			return null;

		lock((ValueParsers as ICollection).SyncRoot) {
			if (ValueParsers.TryGetValue(type, out var pair)) {
				return Activator.CreateInstance(pair.Item1) as IVariableSet;
			}
		}

		return null;
	}

	private readonly ParserDelegate? Parser;

	public TypedVariableSet() {
		lock((ValueParsers as ICollection).SyncRoot) {
			if (ValueParsers.TryGetValue(typeof(TValue), out var pair))
				Parser = pair.Item2;
		}
	}

	public override bool TryParseValue(string input, [NotNullWhen(true)] out TValue? result) {
		try {
			if (Parser is not null && Parser(input, Manager, Manifest, out object? value) && value is TValue tval) {
				result = tval;
				return true;
			}
		} catch (Exception ex) {
			ModEntry.Instance.Log($"Failed to load {typeof(TValue)} from \"{input}\": {ex}", StardewModdingAPI.LogLevel.Error);
		}

		result = default;
		return false;
	}

}
