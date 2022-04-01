/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.Linq.Expressions;

namespace Shockah.CommonModCode.GMCM
{
	public class GMCMI18nHelper
	{
		public readonly IGenericModConfigMenuApi Api;
		public readonly IManifest Mod;
		public readonly ITranslationHelper Translations;

		public GMCMI18nHelper(IGenericModConfigMenuApi api, IManifest mod, ITranslationHelper translations)
		{
			this.Api = api;
			this.Mod = mod;
			this.Translations = translations;
		}

		public void AddSectionTitle(string keyPrefix)
		{
			Api.AddSectionTitle(
				mod: Mod,
				text: () => Translations.Get($"{keyPrefix}.name"),
				tooltip: GetOptionalTranslatedStringDelegate($"{keyPrefix}.tooltip")
			);
		}

		public void AddParagraph(string key)
		{
			Api.AddParagraph(
				mod: Mod,
				text: () => Translations.Get(key)
			);
		}

		public void AddBoolOption(string keyPrefix, Func<bool> getValue, Action<bool> setValue, string? fieldId = null, object? tokens = null)
		{
			Api.AddBoolOption(
				mod: Mod,
				name: () => Translations.Get($"{keyPrefix}.name", tokens),
				tooltip: GetOptionalTranslatedStringDelegate($"{keyPrefix}.tooltip", tokens),
				getValue: getValue,
				setValue: setValue,
				fieldId: fieldId
			);
		}

		public void AddBoolOption(string keyPrefix, Expression<Func<bool>> property, string? fieldId = null, object? tokens = null)
		{
			var getValue = property.Compile()!;
			var setValue = CreateSetter(property).Compile()!;
			Api.AddBoolOption(
				mod: Mod,
				name: () => Translations.Get($"{keyPrefix}.name", tokens),
				tooltip: GetOptionalTranslatedStringDelegate($"{keyPrefix}.tooltip", tokens),
				getValue: getValue,
				setValue: setValue,
				fieldId: fieldId
			);
		}

		public void AddNumberOption(string keyPrefix, Func<int> getValue, Action<int> setValue, int? min = null, int? max = null, int? interval = null, Func<int, string>? formatValue = null, string? fieldId = null, object? tokens = null)
		{
			Api.AddNumberOption(
				mod: Mod,
				name: () => Translations.Get($"{keyPrefix}.name", tokens),
				tooltip: GetOptionalTranslatedStringDelegate($"{keyPrefix}.tooltip", tokens),
				getValue: getValue,
				setValue: setValue,
				min: min,
				max: max,
				interval: interval,
				formatValue: formatValue,
				fieldId: fieldId
			);
		}

		public void AddNumberOption(string keyPrefix, Expression<Func<int>> property, int? min = null, int? max = null, int? interval = null, Func<int, string>? formatValue = null, string? fieldId = null, object? tokens = null)
		{
			var getValue = property.Compile()!;
			var setValue = CreateSetter(property).Compile()!;
			Api.AddNumberOption(
				mod: Mod,
				name: () => Translations.Get($"{keyPrefix}.name", tokens),
				tooltip: GetOptionalTranslatedStringDelegate($"{keyPrefix}.tooltip", tokens),
				getValue: getValue,
				setValue: setValue,
				min: min,
				max: max,
				interval: interval,
				formatValue: formatValue,
				fieldId: fieldId
			);
		}

		public void AddNumberOption(string keyPrefix, Func<float> getValue, Action<float> setValue, float? min = null, float? max = null, float? interval = null, Func<float, string>? formatValue = null, string? fieldId = null, object? tokens = null)
		{
			Api.AddNumberOption(
				mod: Mod,
				name: () => Translations.Get($"{keyPrefix}.name", tokens),
				tooltip: GetOptionalTranslatedStringDelegate($"{keyPrefix}.tooltip", tokens),
				getValue: getValue,
				setValue: setValue,
				min: min,
				max: max,
				interval: interval,
				formatValue: formatValue,
				fieldId: fieldId
			);
		}

		public void AddNumberOption(string keyPrefix, Expression<Func<float>> property, float? min = null, float? max = null, float? interval = null, Func<float, string>? formatValue = null, string? fieldId = null, object? tokens = null)
		{
			var getValue = property.Compile()!;
			var setValue = CreateSetter(property).Compile()!;
			Api.AddNumberOption(
				mod: Mod,
				name: () => Translations.Get($"{keyPrefix}.name", tokens),
				tooltip: GetOptionalTranslatedStringDelegate($"{keyPrefix}.tooltip", tokens),
				getValue: getValue,
				setValue: setValue,
				min: min,
				max: max,
				interval: interval,
				formatValue: formatValue,
				fieldId: fieldId
			);
		}

		public void AddTextOption(string keyPrefix, Func<string> getValue, Action<string> setValue, string[]? allowedValues = null, Func<string, string>? formatAllowedValue = null, string? fieldId = null, object? tokens = null)
		{
			Api.AddTextOption(
				mod: Mod,
				name: () => Translations.Get($"{keyPrefix}.name", tokens),
				tooltip: GetOptionalTranslatedStringDelegate($"{keyPrefix}.tooltip", tokens),
				getValue: getValue,
				setValue: setValue,
				allowedValues: allowedValues,
				formatAllowedValue: formatAllowedValue,
				fieldId: fieldId
			);
		}

		public void AddTextOption(string keyPrefix, Expression<Func<string>> property, string[]? allowedValues = null, Func<string, string>? formatAllowedValue = null, string? fieldId = null, object? tokens = null)
		{
			var getValue = property.Compile()!;
			var setValue = CreateSetter(property).Compile()!;
			Api.AddTextOption(
				mod: Mod,
				name: () => Translations.Get($"{keyPrefix}.name", tokens),
				tooltip: GetOptionalTranslatedStringDelegate($"{keyPrefix}.tooltip", tokens),
				getValue: getValue,
				setValue: setValue,
				allowedValues: allowedValues,
				formatAllowedValue: formatAllowedValue,
				fieldId: fieldId
			);
		}

		public void AddEnumOption<EnumType>(string keyPrefix, Expression<Func<EnumType>> property, string? valuePrefix = null, string? fieldId = null, object? tokens = null) where EnumType: struct, Enum
		{
			var getValue = property.Compile()!;
			var setValue = CreateSetter(property).Compile()!;
			Api.AddTextOption(
				mod: Mod,
				name: () => Translations.Get($"{keyPrefix}.name", tokens),
				tooltip: GetOptionalTranslatedStringDelegate($"{keyPrefix}.tooltip", tokens),
				getValue: () => Enum.GetName(getValue())!,
				setValue: value => setValue(Enum.Parse<EnumType>(value)),
				allowedValues: Enum.GetNames<EnumType>(),
				formatAllowedValue: value => Translations.Get($"{valuePrefix ?? keyPrefix}.value.{value}", tokens),
				fieldId: fieldId
			);
		}

		public void AddEnumOption<EnumType>(string keyPrefix, Func<EnumType> getValue, Action<EnumType> setValue, string? valuePrefix = null, string? fieldId = null, object? tokens = null) where EnumType: struct, Enum
		{
			Api.AddTextOption(
				mod: Mod,
				name: () => Translations.Get($"{keyPrefix}.name", tokens),
				tooltip: GetOptionalTranslatedStringDelegate($"{keyPrefix}.tooltip", tokens),
				getValue: () => Enum.GetName(getValue())!,
				setValue: value => setValue(Enum.Parse<EnumType>(value)),
				allowedValues: Enum.GetNames<EnumType>(),
				formatAllowedValue: value => Translations.Get($"{valuePrefix ?? keyPrefix}.value.{value}", tokens),
				fieldId: fieldId
			);
		}

		public void AddKeybind(string keyPrefix, Func<SButton> getValue, Action<SButton> setValue, string? fieldId = null, object? tokens = null)
		{
			Api.AddKeybind(
				mod: Mod,
				name: () => Translations.Get($"{keyPrefix}.name", tokens),
				tooltip: GetOptionalTranslatedStringDelegate($"{keyPrefix}.tooltip", tokens),
				getValue: getValue,
				setValue: setValue,
				fieldId: fieldId
			);
		}

		public void AddKeybind(string keyPrefix, Expression<Func<SButton>> property, string? fieldId = null, object? tokens = null)
		{
			var getValue = property.Compile()!;
			var setValue = CreateSetter(property).Compile()!;
			Api.AddKeybind(
				mod: Mod,
				name: () => Translations.Get($"{keyPrefix}.name", tokens),
				tooltip: GetOptionalTranslatedStringDelegate($"{keyPrefix}.tooltip", tokens),
				getValue: getValue,
				setValue: setValue,
				fieldId: fieldId
			);
		}

		public void AddKeybindList(string keyPrefix, Func<KeybindList> getValue, Action<KeybindList> setValue, string? fieldId = null, object? tokens = null)
		{
			Api.AddKeybindList(
				mod: Mod,
				name: () => Translations.Get($"{keyPrefix}.name", tokens),
				tooltip: GetOptionalTranslatedStringDelegate($"{keyPrefix}.tooltip", tokens),
				getValue: getValue,
				setValue: setValue,
				fieldId: fieldId
			);
		}

		public void AddKeybindList(string keyPrefix, Expression<Func<KeybindList>> property, string? fieldId = null, object? tokens = null)
		{
			var getValue = property.Compile()!;
			var setValue = CreateSetter(property).Compile()!;
			Api.AddKeybindList(
				mod: Mod,
				name: () => Translations.Get($"{keyPrefix}.name", tokens),
				tooltip: GetOptionalTranslatedStringDelegate($"{keyPrefix}.tooltip", tokens),
				getValue: getValue,
				setValue: setValue,
				fieldId: fieldId
			);
		}

		public void AddPage(string keyPrefix, string pageId, object? tokens = null)
		{
			Api.AddPage(
				mod: Mod,
				pageId: pageId,
				pageTitle: () => Translations.Get($"{keyPrefix}.name", tokens)
			);
		}

		public void AddPageLink(string pageId, string keyPrefix, object? tokens = null)
		{
			Api.AddPageLink(
				mod: Mod,
				pageId: pageId,
				text: () => Translations.Get($"{keyPrefix}.name", tokens),
				tooltip: GetOptionalTranslatedStringDelegate($"{keyPrefix}.tooltip", tokens)
			);
		}

		public Func<string>? GetOptionalTranslatedStringDelegate(string key, object? tokens = null)
		{
			var translation = Translations.Get(key, tokens);
			return translation.HasValue() ? () => translation : null;
		}

		public static Expression<Action<T>> CreateSetter<T>(Expression<Func<T>> getter)
		{
			var parameter = Expression.Parameter(typeof(T), "value");
			var body = Expression.Assign(getter.Body, parameter);
			var setter = Expression.Lambda<Action<T>>(body, parameter);
			return setter;
		}
	}
}
