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
using System.Reflection;

using HarmonyLib;

using StardewModdingAPI;

using Newtonsoft.Json;
using Leclair.Stardew.ThemeManager.Serialization;
using System.Linq;

namespace Leclair.Stardew.ThemeManager.VariableSets;


[JsonConverter(typeof(RealVariableSetConverter))]
public abstract class BaseVariableSet<TValue> : IVariableSet<TValue> {

	#region Functions

	public delegate bool TryParseValueDelegate(string input, [NotNullWhen(true)] out object? result);

	public delegate bool TryFunctionDelegate(string[] args, TryParseValueDelegate tryParse, [NotNullWhen(true)] out object? result);

	internal static readonly Dictionary<Type, Dictionary<string, TryFunctionDelegate>> Functions = new();

	public static void RegisterFunction(Type type, string name, TryFunctionDelegate handler) {
		Dictionary<string, TryFunctionDelegate>? values;
		lock((Functions as ICollection).SyncRoot) {
			if (!Functions.TryGetValue(type, out values)) {
				values = new(StringComparer.OrdinalIgnoreCase);
				Functions[type] = values;
			}
		}

		lock((values as ICollection).SyncRoot) {
			if (!values.ContainsKey(name))
				values.Add(name, handler);
		}
	}

	private Dictionary<string, TryFunctionDelegate> GetFunctions() {
		Type type = GetType();
		Dictionary<string, TryFunctionDelegate>? result;
		lock ((Functions as ICollection).SyncRoot) {
			if (Functions.TryGetValue(type, out result))
				return result;

			result = new(StringComparer.OrdinalIgnoreCase);
			Functions[type] = result;
		}

		foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)) {
			if (!method.Name.StartsWith("Function_"))
				continue;

			try {
				TryFunctionDelegate @delegate = (TryFunctionDelegate) Delegate.CreateDelegate(typeof(TryFunctionDelegate), method);
				result.TryAdd(method.Name[9..], @delegate);

			} catch (Exception ex) {
				ModEntry.Instance.Log($"Unable to create function delegate for {ModEntry.Instance.ToTargetString(method)}: {ex}", LogLevel.Error);
				continue;
			}
		}

		return result;
	}

	#endregion

	#region Fields

	internal IThemeManifest? Manifest;
	internal IThemeManager? Manager;

	internal Tuple<PropertyInfo?>? _Property;

	private IReadOnlyDictionary<string, string>? _RawValues;

	private IReadOnlyDictionary<string, string>? _InheritedValues;

	private IReadOnlyDictionary<string, string>? _DefaultValues;

	private Dictionary<string, TValue>? _Values;

	#endregion

	#region Properties

	public bool ProcessingInheritance { get; private set; } = false;

	public IReadOnlyDictionary<string, string>? RawValues {
		get {
			return _RawValues;
		}
		set {
			if (_RawValues != value) {
				_RawValues = value;
				_InheritedValues = null;
				_Values = null;
			}
		}
	}

	public IReadOnlyDictionary<string, string>? DefaultValues {
		get {
			return _DefaultValues;
		}
		set {
			if (_DefaultValues != value) {
				_DefaultValues = value;
				_Values = null;
			}
		}
	}

	#endregion

	#region Life Cycle

	public BaseVariableSet() {

	}

	#endregion

	#region Inheritance Access

	public void SetReferences(IThemeManager? manager, IThemeManifest? manifest) {
		Manager = manager;
		Manifest = manifest;
	}

	private PropertyInfo? GetProperty() {
		if (_Property is not null)
			return _Property.Item1;

		if (Manager is IThemeManagerInternal ts && Manifest is not null && ts.TryGetThemeRaw(Manifest.UniqueID, out object? theme)) {
			foreach (var prop in AccessTools.GetDeclaredProperties(theme.GetType())) {
				if (prop.CanRead && prop.GetValue(theme) == this) {
					_Property = new(prop);
					return prop;
				}
			}
		}

		_Property = new(null);
		return null;
	}

	private BaseVariableSet<TValue>? GetParentSet() {
		if (Manager is not IThemeManagerInternal ts || string.IsNullOrEmpty(Manifest?.FallbackTheme) || !ts.TryGetThemeRaw(Manifest.FallbackTheme, out object? theme))
			return null;

		var prop = GetProperty();
		if (prop is null)
			return null;

		if (prop.GetValue(theme) is BaseVariableSet<TValue> vs)
			return vs;

		return null;
	}

	#endregion

	#region Calculation

	public IReadOnlyDictionary<string, string> InheritedValues {
		get {
			if (_InheritedValues is not null)
				return _InheritedValues!;

			Dictionary<string, string> result;
			ProcessingInheritance = true;

			var parent = GetParentSet();
			if (parent is not null && !parent.ProcessingInheritance)
				result = new(parent.InheritedValues, StringComparer.OrdinalIgnoreCase);
			else
				result = new(StringComparer.OrdinalIgnoreCase);

			if (RawValues is not null)
				foreach (var entry in RawValues)
					result[entry.Key.StartsWith('$') ? entry.Key[1..] : entry.Key] = entry.Value;

			_InheritedValues = result;
			ProcessingInheritance = false;
			return result;
		}
	}

	public IReadOnlyDictionary<string, TValue> CalculatedValues {
		get {
			if (_Values is not null)
				return _Values!;

			IReadOnlyDictionary<string, string> input;

			// Do we have default values? Those are less important than every
			// existing value, but we still need to include them.
			if (_DefaultValues is not null && _DefaultValues.Count > 0) {
				var inp = new Dictionary<string, string>(InheritedValues, StringComparer.OrdinalIgnoreCase);
				input = inp;
				foreach (var entry in _DefaultValues)
					inp.TryAdd(
						entry.Key.StartsWith('$') ? entry.Key[1..] : entry.Key,
						entry.Value
					);
			} else
				input = InheritedValues;

			// Now, resolve every color.
			//Dictionary<string, TValue> result = new(StringComparer.OrdinalIgnoreCase);
			Dictionary<string, (bool, TValue)> wrappedResults = new(StringComparer.OrdinalIgnoreCase);

			// Process every entry.

			foreach (var entry in input)
				TryProcessItem(entry.Key, input, wrappedResults, null, out _);

			_Values = wrappedResults.Where(x => x.Value.Item1).ToDictionary(x => x.Key, x => x.Value.Item2);
			return _Values;
		}
	}

	private bool TryProcessItem(string key, IReadOnlyDictionary<string, string> sources, Dictionary<string, (bool, TValue)> results, HashSet<string>? processing, [NotNullWhen(true)] out TValue? result) {
		if (results.TryGetValue(key, out var cached)) {
			result = cached.Item2;
			return cached.Item1;
		}

		processing ??= new();

		// If we've hit this item before, abort.
		if (!processing.Add(key)) {
			ModEntry.Instance.Log($"Infinite loop detected resolving {typeof(TValue).Name} for {Manifest?.UniqueID ?? "unknown theme"}: {string.Join(" -> ", processing)}", LogLevel.Warn);
			result = default;
			return false;
		}

		// Get the input string.
		if (!sources.TryGetValue(key, out string? input) || string.IsNullOrWhiteSpace(input)) {
			// Try the backup variable.
			if (TryBackupVariable(key, out result)) {
				results[key] = (true, result);
				return true;
			}

			result = default;
			return false;
		}

		if (TryResolveValue(input, sources, results, processing, out result)) {
			results[key] = (true, result);
			return true;
		}

		results[key] = (false, default!);
		return false;
	}

	private bool TryResolveValue(string input, IReadOnlyDictionary<string, string> sources, Dictionary<string, (bool, TValue)> results, HashSet<string>? processing, [NotNullWhen(true)] out TValue? result) {
		input = input.Trim();

		// Do we have function support? If not, we can do simple variable parsing.
		if (!SupportsFunctions) {
			if (input.StartsWith('$'))
				return TryProcessItem(input[1..], sources, results, processing, out result);

			else if (TryParseValue(input, out result))
				return true;

			else {
				ModEntry.Instance.Log($"Unable to parse {typeof(TValue).Name} for {Manifest?.UniqueID ?? "unknown theme"}: {input}", LogLevel.Warn);
				return false;
			}
		}

		result = default!;
		return false;
	}

	#endregion

	#region Implement Calculation

	public virtual bool SupportsFunctions => false;

	public virtual bool TryBackupVariable(string key, [NotNullWhen(true)] out TValue? result) {
		result = default;
		return false;
	}

	public abstract bool TryParseValue(string input, [NotNullWhen(true)] out TValue? result);

	#endregion

	#region IReadOnlyDictionary

	public TValue this[string key] => CalculatedValues[key];

	public IEnumerable<string> Keys => CalculatedValues.Keys;

	public int Count => CalculatedValues.Count;

	public IEnumerable<TValue> Values => CalculatedValues.Values;

	public bool ContainsKey(string key) {
		return CalculatedValues.ContainsKey(key);
	}

	public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator() {
		return CalculatedValues.GetEnumerator();
	}

	public bool TryGetValue(string key, [MaybeNullWhen(false)] out TValue value) {
		return CalculatedValues.TryGetValue(key, out value);
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return CalculatedValues.GetEnumerator();
	}

	#endregion
}
