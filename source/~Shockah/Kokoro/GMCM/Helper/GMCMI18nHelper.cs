/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Shockah.Kokoro.GMCM;
using Shockah.Kokoro.SMAPI;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Shockah.CommonModCode.GMCM
{
	public sealed class GMCMI18nHelper
	{
		public IGenericModConfigMenuApi Api { get; private set; }
		public IManifest Mod { get; private set; }
		public ITranslationSet<string> Translations { get; private set; }

		private string NamePattern { get; set; }
		private string TooltipPattern { get; set; }
		private string ValuePattern { get; set; }

		public GMCMI18nHelper(
			IGenericModConfigMenuApi api,
			IManifest mod,
			ITranslationSet<string> translations,
			string namePattern = "{Key}.name",
			string tooltipPattern = "{Key}.tooltip",
			string valuePattern = "{Key}.value.{Value}"
		)
		{
			this.Api = api;
			this.Mod = mod;
			this.Translations = translations;
			this.NamePattern = namePattern;
			this.TooltipPattern = tooltipPattern;
			this.ValuePattern = valuePattern;
		}

		public GMCMI18nHelper(
			IGenericModConfigMenuApi api,
			IManifest mod,
			ITranslationHelper translations,
			string namePattern = "{Key}.name",
			string tooltipPattern = "{Key}.tooltip",
			string valuePattern = "{Key}.value.{Value}"
		) : this(api, mod, new SMAPITranslationSetWrapper(translations), namePattern, tooltipPattern, valuePattern) { }

		public void AddSectionTitle(string keyPrefix, object? tokens = null)
		{
			Api.AddSectionTitle(
				mod: Mod,
				text: () => Translations.Get(NamePattern.Replace("{Key}", keyPrefix), tokens),
				tooltip: GetOptionalTranslatedStringDelegate(TooltipPattern.Replace("{Key}", keyPrefix), tokens)
			);
		}

		public void AddParagraph(string key, object? tokens = null)
		{
			Api.AddParagraph(
				mod: Mod,
				text: () => Translations.Get(key, tokens)
			);
		}

		public void AddBoolOption(string keyPrefix, Func<bool> getValue, Action<bool> setValue, string? fieldId = null, object? tokens = null)
		{
			Api.AddBoolOption(
				mod: Mod,
				name: () => Translations.Get(NamePattern.Replace("{Key}", keyPrefix), tokens),
				tooltip: GetOptionalTranslatedStringDelegate(TooltipPattern.Replace("{Key}", keyPrefix), tokens),
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
				name: () => Translations.Get(NamePattern.Replace("{Key}", keyPrefix), tokens),
				tooltip: GetOptionalTranslatedStringDelegate(TooltipPattern.Replace("{Key}", keyPrefix), tokens),
				getValue: getValue,
				setValue: setValue,
				fieldId: fieldId
			);
		}

		public void AddNumberOption(string keyPrefix, Func<int> getValue, Action<int> setValue, int? min = null, int? max = null, int? interval = null, Func<int, string>? formatValue = null, string? fieldId = null, object? tokens = null)
		{
			Api.AddNumberOption(
				mod: Mod,
				name: () => Translations.Get(NamePattern.Replace("{Key}", keyPrefix), tokens),
				tooltip: GetOptionalTranslatedStringDelegate(TooltipPattern.Replace("{Key}", keyPrefix), tokens),
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
				name: () => Translations.Get(NamePattern.Replace("{Key}", keyPrefix), tokens),
				tooltip: GetOptionalTranslatedStringDelegate(TooltipPattern.Replace("{Key}", keyPrefix), tokens),
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
				name: () => Translations.Get(NamePattern.Replace("{Key}", keyPrefix), tokens),
				tooltip: GetOptionalTranslatedStringDelegate(TooltipPattern.Replace("{Key}", keyPrefix), tokens),
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
				name: () => Translations.Get(NamePattern.Replace("{Key}", keyPrefix), tokens),
				tooltip: GetOptionalTranslatedStringDelegate(TooltipPattern.Replace("{Key}", keyPrefix), tokens),
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
				name: () => Translations.Get(NamePattern.Replace("{Key}", keyPrefix), tokens),
				tooltip: GetOptionalTranslatedStringDelegate(TooltipPattern.Replace("{Key}", keyPrefix), tokens),
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
				name: () => Translations.Get(NamePattern.Replace("{Key}", keyPrefix), tokens),
				tooltip: GetOptionalTranslatedStringDelegate(TooltipPattern.Replace("{Key}", keyPrefix), tokens),
				getValue: getValue,
				setValue: setValue,
				allowedValues: allowedValues,
				formatAllowedValue: formatAllowedValue,
				fieldId: fieldId
			);
		}

		public void AddEnumOption<EnumType>(string keyPrefix, Expression<Func<EnumType>> property, string? valuePrefix = null, Func<EnumType, bool>? isAllowed = null, string? fieldId = null, object? tokens = null) where EnumType : struct, Enum
		{
			var getValue = property.Compile()!;
			var setValue = CreateSetter(property).Compile()!;
			Api.AddTextOption(
				mod: Mod,
				name: () => Translations.Get(NamePattern.Replace("{Key}", keyPrefix), tokens),
				tooltip: GetOptionalTranslatedStringDelegate(TooltipPattern.Replace("{Key}", keyPrefix), tokens),
				getValue: () => Enum.GetName(getValue())!,
				setValue: value => setValue(Enum.Parse<EnumType>(value)),
				allowedValues: Enum.GetNames<EnumType>().Where(name => isAllowed is null || isAllowed(Enum.Parse<EnumType>(name))).ToArray(),
				formatAllowedValue: value => Translations.Get(ValuePattern.Replace("{Key}", valuePrefix ?? keyPrefix).Replace("{Value}", value), tokens),
				fieldId: fieldId
			);
		}

		public void AddEnumOption<EnumType>(string keyPrefix, Func<EnumType> getValue, Action<EnumType> setValue, string? valuePrefix = null, Func<EnumType, bool>? isAllowed = null, string? fieldId = null, object? tokens = null) where EnumType : struct, Enum
		{
			Api.AddTextOption(
				mod: Mod,
				name: () => Translations.Get(NamePattern.Replace("{Key}", keyPrefix), tokens),
				tooltip: GetOptionalTranslatedStringDelegate(TooltipPattern.Replace("{Key}", keyPrefix), tokens),
				getValue: () => Enum.GetName(getValue())!,
				setValue: value => setValue(Enum.Parse<EnumType>(value)),
				allowedValues: Enum.GetNames<EnumType>().Where(name => isAllowed is null || isAllowed(Enum.Parse<EnumType>(name))).ToArray(),
				formatAllowedValue: value => Translations.Get(ValuePattern.Replace("{Key}", valuePrefix ?? keyPrefix).Replace("{Value}", value), tokens),
				fieldId: fieldId
			);
		}

		public void AddKeybind(string keyPrefix, Func<SButton> getValue, Action<SButton> setValue, string? fieldId = null, object? tokens = null)
		{
			Api.AddKeybind(
				mod: Mod,
				name: () => Translations.Get(NamePattern.Replace("{Key}", keyPrefix), tokens),
				tooltip: GetOptionalTranslatedStringDelegate(TooltipPattern.Replace("{Key}", keyPrefix), tokens),
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
				name: () => Translations.Get(NamePattern.Replace("{Key}", keyPrefix), tokens),
				tooltip: GetOptionalTranslatedStringDelegate(TooltipPattern.Replace("{Key}", keyPrefix), tokens),
				getValue: getValue,
				setValue: setValue,
				fieldId: fieldId
			);
		}

		public void AddKeybindList(string keyPrefix, Func<KeybindList> getValue, Action<KeybindList> setValue, string? fieldId = null, object? tokens = null)
		{
			Api.AddKeybindList(
				mod: Mod,
				name: () => Translations.Get(NamePattern.Replace("{Key}", keyPrefix), tokens),
				tooltip: GetOptionalTranslatedStringDelegate(TooltipPattern.Replace("{Key}", keyPrefix), tokens),
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
				name: () => Translations.Get(NamePattern.Replace("{Key}", keyPrefix), tokens),
				tooltip: GetOptionalTranslatedStringDelegate(TooltipPattern.Replace("{Key}", keyPrefix), tokens),
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
				pageTitle: () => Translations.Get(NamePattern.Replace("{Key}", keyPrefix), tokens)
			);
		}

		public void AddPageLink(string pageId, string keyPrefix, object? tokens = null)
		{
			Api.AddPageLink(
				mod: Mod,
				pageId: pageId,
				text: () => Translations.Get(NamePattern.Replace("{Key}", keyPrefix), tokens),
				tooltip: GetOptionalTranslatedStringDelegate(TooltipPattern.Replace("{Key}", keyPrefix), tokens)
			);
		}

		public Func<string>? GetOptionalTranslatedStringDelegate(string key, object? tokens = null)
		{
			return Translations.ContainsKey(key) ? () => Translations.Get(key, tokens) : null;
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
