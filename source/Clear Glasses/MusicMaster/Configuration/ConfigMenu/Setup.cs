/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using GenericModConfigMenu;
using LinqFasterer;
using MusicMaster.Extensions;
using MusicMaster.Extensions.Reflection;
using MusicMaster.Types;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace MusicMaster.Configuration.ConfigMenu;

internal static class Setup {
	private static volatile bool Initialized = false;

	private static IGenericModConfigMenuApi? ConfigApi = null;

	internal static void Initialize() {
		if (Initialized) {
			ThrowHelper.ThrowInvalidOperationException("GMCM already initialized");
		}
		Initialized = true;

		// https://github.com/spacechase0/StardewValleyMods/tree/develop/GenericModConfigMenu#for-c-mod-authors

		if (MusicMaster.Self.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu") is not { } configApi) {
			Debug.Trace("Could not acquire GenericModConfigMenu interface");
			return;
		}
		ConfigApi = configApi;

		configApi.Register(
			mod: MusicMaster.Self.ModManifest,
			reset: Reset,
			save: Save
		);

		configApi.OnFieldChanged(
			MusicMaster.Self.ModManifest,
			OnValueChange
		);

		ProcessCategory(
			parent: null,
			category: Serialize.Root,
			advanced: false,
			manifest: MusicMaster.Self.ModManifest,
			config: configApi
		);

		ProcessCategory(
			parent: null,
			category: Serialize.Root,
			advanced: true,
			manifest: MusicMaster.Self.ModManifest,
			config: configApi
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void ForceOpen() {
		ConfigApi?.OpenModMenu(MusicMaster.Self.ModManifest);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Reset() {
		if (Config.DefaultConfig is null) {
			return;
		}

		Serialize.Load(Config.DefaultConfig, retain: true);
		Config.DefaultConfig.Position = 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Save() {
		Serialize.Save(Config.Path);
		Serialize.RefreshHash();
	}

	private static bool IsAdvanced(Type type) {
		while (true) {
			bool isAdvancedField = type.HasAttribute<Attributes.AdvancedAttribute>();
			if (isAdvancedField) {
				return true;
			}

			if (type.DeclaringType is null) {
				return false;
			}

			type = type.DeclaringType;
		}
	}

	private static bool IsAdvanced(FieldInfo field) {
		bool isAdvancedField = field.HasAttribute<Attributes.AdvancedAttribute>();
		if (isAdvancedField) {
			return true;
		}

		if (field.DeclaringType is null) {
			return false;
		}

		return IsAdvanced(field.DeclaringType);
	}

	private static bool Hidden(FieldInfo field, bool advanced) {
		if (field.HasAttribute<Attributes.GMCMHiddenAttribute>()) {
			return true;
		}

		bool isAdvancedField = IsAdvanced(field);
		if (advanced != isAdvancedField) {
			return true;
		}


		if (!IsFieldRepresentable(field)) {
			return true;
		}

		return Hidden(field.DeclaringType, advanced);
	}

	private static bool Hidden(Type? type, bool advanced) {
		while (true) {
			if (type is null) {
				return false;
			}

			if (type.HasAttribute<Attributes.GMCMHiddenAttribute>()) {
				return true;
			}

			if (!advanced && type.HasAttribute<Attributes.AdvancedAttribute>()) {
				return true;
			}

			type = type.DeclaringType;
		}
	}

	private static readonly string[][] Prefixes = {
		new[]{ "B" },
		new[]{ "KiB", "KB", "K" },
		new[]{ "MiB", "MB", "M" },
		new[]{ "GiB", "GB", "G" },
		new[]{ "TiB", "TB", "T" },
		new[]{ "PiB", "PB", "P" }
	};

	/// <summary>
	/// Returns the value to the given order of magnitude (power of 10)
	/// </summary>
	/// <param name="value">Value to get to the power of 10</param>
	/// <param name="order">What order of magnitude to return</param>
	/// <returns>Order-of-magnitude-adjusted value</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	private static long SizeOrder(long value, int order) {
		if (order < 0) {
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(order), order, "parameter must not be negative");
		}

		// 1024^1 = (1024 << 0)
		// 1024^2 = (1024 << 10)
		// 1024^3 = (1024 << 20)
		return checked(value << 10 * order);
	}

	private static string FormatLong(long value) {
		string? Magnitude(int order, bool force = false) {
			if (!force && value >= SizeOrder(1024, order + 1)) {
				return null;
			}

			// This uses doubles because we want to get a fraction
			var divisor = SizeOrder(1024, order);
			var dValue = (double)value / divisor;
			var prefix = Prefixes[Math.Min(order, Prefixes.Length - 1)][0];
			return $"{dValue:0.##} {prefix}";
		}
		return
			Magnitude(0) ??
			Magnitude(1) ??
			Magnitude(2) ??
			Magnitude(3) ??
			Magnitude(4) ??
			Magnitude(5, true)!;
	}

	private static long ParseLong(string value) {
		(int Order, string Prefix)? prefixResult = null;
		for (int i = 0; (prefixResult?.Order ?? 0) == 0 && i < Prefixes.Length; ++i) {
			foreach (var p in Prefixes[i]) {
				if (!value.EndsWith(p, StringComparison.InvariantCultureIgnoreCase)) {
					continue;
				}

				prefixResult = (i, p);
				break;
			}
		}

		if (prefixResult is null) {
			return long.Parse(value);
		}

		value = value[..^prefixResult.Value.Prefix.Length];

		long resultValue;

		// Try it as a pure integral-value
		if (long.TryParse(value, out long intValue)) {
			intValue *= SizeOrder(1024, prefixResult.Value.Order);
			resultValue = intValue;
		}
		// Otherwise, try it as a decimal value
		else if (double.TryParse(value, out double realValue)) {
			realValue *= SizeOrder(1024, prefixResult.Value.Order);
			resultValue = (long)Math.Round(realValue);
		}
		else {
			return ThrowHelper.ThrowFormatException<long>($"Could not parse '{value}' as a numeric value");
		}

		return resultValue;
	}

	private static bool IsFieldRepresentable(FieldInfo field) {
		return
			field.FieldType == typeof(bool) ||
			field.FieldType == typeof(byte) ||
			field.FieldType == typeof(sbyte) ||
			field.FieldType == typeof(ushort) ||
			field.FieldType == typeof(short) ||
			field.FieldType == typeof(int) ||
			field.FieldType == typeof(float) ||
			field.FieldType == typeof(double) ||
			field.FieldType == typeof(string) ||
			field.FieldType == typeof(SButton) ||
			field.FieldType.IsEnum ||
			field.FieldType == typeof(long);
	}

	private static string FormatName(string name) {
		var result = new StringBuilder();

		char prevC = ' ';
		for (int i = 0; i < name.Length; ++i) {
			char c = name[i];
			if (i != 0 && char.IsUpper(c) && !char.IsUpper(prevC)) {
				result.Append(' ');
			}
			result.Append(c);
			prevC = c;
		}

		return result.ToString();
	}

	private static T ExtractCombinedEnum<T>(Type enumType, string combinedName) where T : unmanaged {
		int splitOffset = combinedName.IndexOf('/');
		if (splitOffset != -1) {
			combinedName = combinedName[..splitOffset];
		}
		return (T)Enum.Parse(enumType, combinedName);
	}

	private static T ExtractCombinedEnum<T>(string combinedName) where T : unmanaged, Enum =>
		ExtractCombinedEnum<T>(typeof(T), combinedName);

	private static string GetFieldName(FieldInfo field) {
		var attribute = field.GetCustomAttribute<Attributes.MenuNameAttribute>();
		return attribute?.Name ?? FormatName(field.Name);
	}

	private static void ProcessField(FieldInfo field, bool advanced, IManifest manifest, IGenericModConfigMenuApi config) {
		if (Hidden(field, advanced)) {
			return;
		}

		var comments = field.GetCustomAttributes<Attributes.CommentAttribute>();
		string? comment = null;
		var commentAttributes = comments.AsArray();
		if (commentAttributes.Length != 0) {
			comment = string.Join(
				Environment.NewLine,
				commentAttributes.SelectF(attr => attr.Message)
			);
		}

		var fieldType = field.FieldType;
		var fieldId = $"{field.ReflectedType?.FullName ?? "unknown"}.{field.Name}";
		string FieldName() => GetFieldName(field);
		Func<string>? tooltip = null;
		if (comment is not null) {
			tooltip = () => comment;
		}

		if (fieldType == typeof(bool)) {
			config.AddBoolOption(
				mod: manifest,
				getValue: () => Command.GetValue<bool>(field),
				setValue: value => Command.SetValue(field, value),
				name: FieldName,
				tooltip: tooltip,
				fieldId: fieldId
			);
		}
		else if (fieldType == typeof(byte) || fieldType == typeof(sbyte) || fieldType == typeof(ushort) || fieldType == typeof(short) || fieldType == typeof(int)) {
			var limitAttribute = field.GetCustomAttribute<Attributes.LimitsIntAttribute>();

			config.AddNumberOption(
				mod: manifest,
				getValue: () => Command.GetValue<int>(field),
				setValue: value => Command.SetValue(field, value),
				name: FieldName,
				tooltip: tooltip,
				min: limitAttribute?.GetMin<int>(fieldType),
				max: limitAttribute?.GetMax<int>(fieldType),
				interval: null,
				formatValue: null,
				fieldId: fieldId
			);
		}
		else if (fieldType == typeof(float)) {
			var limitAttribute = field.GetCustomAttribute<Attributes.LimitsRealAttribute>();

			config.AddNumberOption(
				mod: manifest,
				getValue: () => Command.GetValue<float>(field),
				setValue: value => Command.SetValue(field, value),
				name: FieldName,
				tooltip: tooltip,
				min: limitAttribute?.GetMin<float>() ?? float.MinValue,
				max: limitAttribute?.GetMax<float>() ?? float.MaxValue,
				interval: null,
				formatValue: null,
				fieldId: fieldId
			);
		}
		else if (fieldType == typeof(double)) {
			var limitAttribute = field.GetCustomAttribute<Attributes.LimitsRealAttribute>();

			config.AddNumberOption(
				mod: manifest,
				getValue: () => (float)Command.GetValue<double>(field),
				setValue: value => Command.SetValue<double>(field, value),
				name: FieldName,
				tooltip: tooltip,
				min: limitAttribute?.GetMin<float>(typeof(double)) ?? float.MinValue,
				max: limitAttribute?.GetMax<float>(typeof(double)) ?? float.MaxValue,
				interval: null,
				formatValue: null,
				fieldId: fieldId
			);
		}
		else if (fieldType == typeof(string)) {
			config.AddTextOption(
				mod: manifest,
				getValue: () => Command.GetValue<string>(field),
				setValue: value => Command.SetValue<string>(field, value),
				name: FieldName,
				tooltip: tooltip,
				allowedValues: null,
				formatAllowedValue: null,
				fieldId: fieldId
			);
		}
		else if (fieldType == typeof(SButton)) {
			config.AddKeybind(
				mod: manifest,
				getValue: () => Command.GetValue<SButton>(field),
				setValue: value => Command.SetValue(field, value),
				name: FieldName,
				tooltip: tooltip,
				fieldId: fieldId
			);
		}
		else if (fieldType.IsEnum) {
			Dictionary<int, string> enumMap = new();
			foreach (var enumPairs in EnumExt.Get(fieldType)) {
				if (!enumMap.TryAdd(enumPairs.Value, enumPairs.Key)) {
					enumMap[enumPairs.Value] += $"/{enumPairs.Key}";
				}
			}

			config.AddTextOption(
				mod: manifest,
				getValue: () => enumMap[Command.GetValue<int>(field)],
				setValue: value => Command.SetValue(field, ExtractCombinedEnum<int>(fieldType, value)),
				name: FieldName,
				tooltip: tooltip,
				allowedValues: enumMap.Values.AsArray(),
				formatAllowedValue: null,
				fieldId: fieldId
			);
		}
		else if (fieldType == typeof(long)) {
			var limitAttribute = field.GetCustomAttribute<Attributes.LimitsIntAttribute>();

			config.AddTextOption(
				mod: manifest,
				getValue: () => FormatLong(Command.GetValue<long>(field)),
				setValue: value => Command.SetValue(
					field,
					Math.Clamp(
						ParseLong(value),
						limitAttribute?.MinValue ?? long.MinValue,
						limitAttribute?.MaxValue ?? long.MaxValue
					)
				),
				name: FieldName,
				tooltip: tooltip,
				allowedValues: null,
				formatAllowedValue: null,
				fieldId: fieldId
			);
		}
		else {
			Debug.Error($"Cannot apply type '{fieldType.Name}' to GMCM");
		}
	}

	private static void OnValueChange(string fieldId, object value) {
	}

	private static string? GetCategoryName(Serialize.Category category) {
		if (category.Name.IsBlank()) {
			return null;
		}

		var names = new List<string>();
		names.Add(category.Name);
		foreach (var currentCategory in category.ParentTraverser) {
			if (!currentCategory.Name.IsBlank()) {
				names.Add(currentCategory.Name);
			}
		}

		names.Reverse();

		return string.Join('.', names);
	}

	private static bool IsCategoryValid(Serialize.Category category, bool advanced) {
		if (Hidden(category.Type, advanced)) {
			return false;
		}

		foreach (var field in category.Fields.Values) {
			if (!Hidden(field, advanced)) {
				return true;
			}
		}

		foreach (var child in category.Children.Values) {
			if (IsCategoryValid(child, advanced)) {
				return true;
			}
		}

		return false;
	}

	private static void ProcessCategory(Serialize.Category? parent, Serialize.Category category, bool advanced, IManifest manifest, IGenericModConfigMenuApi config, bool first = false) {
		bool isRoot = parent is null;

		if (isRoot && advanced) {
			config.AddPage(
				mod: manifest,
				pageId: "advanced",
				pageTitle: () => "Advanced Settings"
			);
		}

		if (category.Name.Length != 0) {
			var categoryName = GetCategoryName(category);
			if (categoryName.IsBlank()) {
				if (!first && (!isRoot || advanced)) {
					config.AddSectionTitle(manifest, () => "");
				}
			}
			else {
				if (category.Type.GetAttribute<Attributes.CommentAttribute>(out var commentAttribute)) {
					config.AddSectionTitle(manifest, () => categoryName, () => commentAttribute.Message);
				}
				else {
					config.AddSectionTitle(manifest, () => categoryName);
				}
			}
		}

		if (isRoot) {
			if (!advanced) {
				config.AddPageLink(
					mod: manifest,
					pageId: "advanced",
					text: () => "[ Advanced Settings ]",
					tooltip: () => "Display Advanced Settings"
				);
			}

			config.AddSectionTitle(manifest, () => "");
		}

		foreach (var field in category.Fields.Values) {
			ProcessField(field, advanced, manifest, config);
		}

		first = true;
		foreach (var child in category.Children.Values) {
			if (Hidden(child.Type, advanced)) {
				continue;
			}

			if (!IsCategoryValid(child, advanced)) {
				continue;
			}

			ProcessCategory(
				category,
				child,
				advanced,
				manifest,
				config,
				first
			);

			first = false;
		}
	}
}
