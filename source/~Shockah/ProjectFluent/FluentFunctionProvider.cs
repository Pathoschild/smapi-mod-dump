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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.ProjectFluent
{
	internal interface IFluentFunctionProvider
	{
		IEnumerable<(IManifest mod, string name, FluentFunction function)> GetFluentFunctions();
	}

	internal class SerialFluentFunctionProvider : IFluentFunctionProvider
	{
		private IFluentFunctionProvider[] Providers { get; set; }

		public SerialFluentFunctionProvider(params IFluentFunctionProvider[] providers)
		{
			// making a copy on purpose
			this.Providers = providers.ToArray();
		}

		public IEnumerable<(IManifest mod, string name, FluentFunction function)> GetFluentFunctions()
		{
			foreach (var provider in Providers)
				foreach (var function in provider.GetFluentFunctions())
					yield return function;
		}
	}

	internal class BuiltInFluentFunctionProvider : IFluentFunctionProvider
	{
		private IManifest ProjectFluentMod { get; set; }
		private IModRegistry ModRegistry { get; set; }
		private IFluentValueFactory FluentValueFactory { get; set; }
		private IModTranslationsProvider ModTranslationsProvider { get; set; }
		internal IFluentProvider FluentProvider { get; set; } = null!;

		private IDictionary<(IGameLocale locale, string mod, string? name), IFluent<string>> FluentCache { get; set; } = new Dictionary<(IGameLocale locale, string mod, string? name), IFluent<string>>();

		public BuiltInFluentFunctionProvider(IManifest projectFluentMod, IModRegistry modRegistry, IFluentValueFactory fluentValueFactory, IModTranslationsProvider modTranslationsProvider)
		{
			this.ProjectFluentMod = projectFluentMod;
			this.ModRegistry = modRegistry;
			this.FluentValueFactory = fluentValueFactory;
			this.ModTranslationsProvider = modTranslationsProvider;
		}

		public IEnumerable<(IManifest mod, string name, FluentFunction function)> GetFluentFunctions()
		{
			yield return (ProjectFluentMod, "MOD_NAME", ModNameFunction);
			yield return (ProjectFluentMod, "FLUENT", FluentFunction);
			yield return (ProjectFluentMod, "I18N", I18nFunction);
			yield return (ProjectFluentMod, "TOUPPER", ToUpperFunction);
			yield return (ProjectFluentMod, "TOLOWER", ToLowerFunction);
			yield return (ProjectFluentMod, "CAPITALIZE_WORDS", CapitalizeWordsFunction);
		}

		private IFluentFunctionValue ModNameFunction(
			IGameLocale locale,
			IManifest mod,
			IReadOnlyList<IFluentFunctionValue> positionalArguments,
			IReadOnlyDictionary<string, IFluentFunctionValue> namedArguments)
		{
			var modID = mod.UniqueID;
			if (positionalArguments.Count >= 1)
				modID = positionalArguments[0].AsString();

			var otherMod = ModRegistry.Get(modID);
			return FluentValueFactory.CreateStringValue(otherMod?.Manifest.Name ?? modID);
		}

		private IFluentFunctionValue FluentFunction(
			IGameLocale locale,
			IManifest mod,
			IReadOnlyList<IFluentFunctionValue> positionalArguments,
			IReadOnlyDictionary<string, IFluentFunctionValue> namedArguments)
		{
			if (positionalArguments.Count == 0)
				throw new ArgumentException("Missing `Key` positional argument.");
			string targetKey = positionalArguments[0].AsString();

			string targetFluentMod = mod.UniqueID;
			string? targetFluentFile = null;

			var remainingNamedArguments = new Dictionary<string, IFluentFunctionValue>(namedArguments);
			if (remainingNamedArguments.TryGetValue("mod", out var modArg))
			{
				targetFluentMod = modArg.AsString();
				remainingNamedArguments.Remove("mod");
			}
			if (remainingNamedArguments.TryGetValue("file", out var fileArg))
			{
				targetFluentFile = fileArg.AsString();
				remainingNamedArguments.Remove("file");
			}

			if (!FluentCache.TryGetValue((locale, targetFluentMod, targetFluentFile), out var fluent))
			{
				var otherMod = ModRegistry.Get(targetFluentMod);
				if (otherMod is null)
					throw new ArgumentException($"Mod `{targetFluentMod}` is not installed.");
				fluent = FluentProvider.GetFluent(locale, otherMod.Manifest, targetFluentFile);
				FluentCache[(locale, targetFluentMod, targetFluentFile)] = fluent;
			}

			var remainingStringNamedArguments = new Dictionary<string, string>();
			foreach (var (key, arg) in remainingNamedArguments)
				remainingStringNamedArguments[key] = arg.AsString();

			return FluentValueFactory.CreateStringValue(fluent.Get(targetKey, remainingStringNamedArguments));
		}

		private IFluentFunctionValue I18nFunction(
			IGameLocale locale,
			IManifest mod,
			IReadOnlyList<IFluentFunctionValue> positionalArguments,
			IReadOnlyDictionary<string, IFluentFunctionValue> namedArguments)
		{
			if (positionalArguments.Count == 0)
				throw new ArgumentException("Missing `Key` positional argument.");
			string targetKey = positionalArguments[0].AsString();

			string targetI18nMod = mod.UniqueID;
			var remainingNamedArguments = new Dictionary<string, IFluentFunctionValue>(namedArguments);
			if (remainingNamedArguments.TryGetValue("mod", out var modArg))
			{
				targetI18nMod = modArg.AsString();
				remainingNamedArguments.Remove("mod");
			}

			var otherMod = ModRegistry.Get(targetI18nMod);
			if (otherMod is null)
				throw new ArgumentException($"Mod `{targetI18nMod}` is not installed.");
			var translations = ModTranslationsProvider.GetModTranslations(otherMod.Manifest);

			if (translations is null)
				return FluentValueFactory.CreateStringValue(targetKey);

			var remainingStringNamedArguments = new Dictionary<string, string>();
			foreach (var (key, arg) in remainingNamedArguments)
				remainingStringNamedArguments[key] = arg.AsString();

			return FluentValueFactory.CreateStringValue(translations.Get(targetKey, remainingStringNamedArguments));
		}

		private IFluentFunctionValue ToUpperFunction(
			IGameLocale locale,
			IManifest mod,
			IReadOnlyList<IFluentFunctionValue> positionalArguments,
			IReadOnlyDictionary<string, IFluentFunctionValue> namedArguments)
		{
			if (positionalArguments.Count == 0)
				throw new ArgumentException("Missing `Value` positional argument.");
			string value = positionalArguments[0].AsString();
			return FluentValueFactory.CreateStringValue(value.ToUpper(locale.CultureInfo));
		}

		private IFluentFunctionValue ToLowerFunction(
			IGameLocale locale,
			IManifest mod,
			IReadOnlyList<IFluentFunctionValue> positionalArguments,
			IReadOnlyDictionary<string, IFluentFunctionValue> namedArguments)
		{
			if (positionalArguments.Count == 0)
				throw new ArgumentException("Missing `Value` positional argument.");
			string value = positionalArguments[0].AsString();
			return FluentValueFactory.CreateStringValue(value.ToLower(locale.CultureInfo));
		}

		private IFluentFunctionValue CapitalizeWordsFunction(
			IGameLocale locale,
			IManifest mod,
			IReadOnlyList<IFluentFunctionValue> positionalArguments,
			IReadOnlyDictionary<string, IFluentFunctionValue> namedArguments)
		{
			if (positionalArguments.Count == 0)
				throw new ArgumentException("Missing `Value` positional argument.");
			string value = positionalArguments[0].AsString();
			return FluentValueFactory.CreateStringValue(locale.CultureInfo.TextInfo.ToTitleCase(value));
		}
	}
}