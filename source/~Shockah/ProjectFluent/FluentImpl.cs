/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Linguini.Bundle;
using Linguini.Bundle.Builder;
using Linguini.Shared.Types.Bundle;
using Linguini.Syntax.Ast;
using StardewModdingAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shockah.ProjectFluent
{
	internal class FluentImpl: IFluent<string>
	{
		private IFluent<string> Fallback { get; set; }
		private FluentBundle Bundle { get; set; }

		public FluentImpl(IEnumerable<(string name, ContextfulFluentFunction function)> functions, IMonitor monitor, IGameLocale locale, string content, IFluent<string>? fallback = null)
		{
			this.Fallback = fallback ?? new NoOpFluent();

			try
			{
				var (bundle, errors) = LinguiniBuilder.Builder()
					.CultureInfo(locale.CultureInfo)
					.AddResources(content)
					.SetUseIsolating(false)
					.Build();
				Bundle = bundle;

				foreach (var (functionName, function) in functions)
				{
					var identifier = new Identifier(new ReadOnlyMemory<char>(functionName.ToCharArray()));

					if (bundle.TryGetFunction(identifier, out _))
						continue;
					bundle.AddFunction(functionName, (fluentPositionalArguments, fluentNamedArguments) =>
					{
						var positionalArguments = fluentPositionalArguments.Select(a => new FluentFunctionValue(a)).ToList();
						var namedArguments = new Dictionary<string, IFluentFunctionValue>();
						foreach (var (key, a) in fluentNamedArguments)
							namedArguments[key] = new FluentFunctionValue(a);

						var result = function(locale, positionalArguments, namedArguments);
						var fluentResult = result.AsFluentValue();

						if (fluentResult is IFluentType fluentResultValue)
							return fluentResultValue;
						else
							throw new ArgumentException($"Function `{functionName}` returned a value that is not a `{nameof(IFluentType)}`.");
					}, out _);
				}

				if (errors is not null && errors.Count != 0)
				{
					var errorMessages = errors.Select(e =>
					{
						var span = e.GetSpan();
						if (span is null)
							return $"<unknown location>: {e}";
						return $"Key #{span.Row}: {e}";
					});
					throw new Exception(string.Join("\n", errorMessages));
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"Errors parsing Fluent:\n{ex}");
			}
		}

		private static IDictionary<string, IFluentType> ExtractTokens(object? tokens)
		{
			// source: https://github.com/Pathoschild/SMAPI/blob/develop/src/SMAPI/Translation.cs

			Dictionary<string, IFluentType> results = new();
			if (tokens == null)
				return results;

			void AddResult(string key, object? value)
			{
				if (value is double or float or int or long)
					results[key] = FluentNumber.FromString($"{value}");
				else if (value is not null)
					results[key] = new FluentString($"{value}");
			}

			if (tokens is IDictionary dictionary)
			{
				foreach (DictionaryEntry entry in dictionary)
				{
					string? key = entry.Key?.ToString()?.Trim();
					if (key is not null)
						AddResult(key, entry.Value);
				}
			}
			else
			{
				Type type = tokens.GetType();
				foreach (FieldInfo field in type.GetFields())
					AddResult(field.Name, field.GetValue(tokens));
				foreach (PropertyInfo prop in type.GetProperties())
					AddResult(prop.Name, prop.GetValue(tokens));
			}

			return results;
		}

		public bool ContainsKey(string key)
		{
			return Bundle.TryGetAttrMsg(key, null, out _, out _);
		}

		public string Get(string key, object? tokens)
		{
			var extractedTokens = ExtractTokens(tokens);
			return Bundle.GetAttrMessage(key, extractedTokens) ?? Fallback.Get(key, tokens);
		}
	}

	internal record FluentFunctionValue: IFluentFunctionValue
	{
		internal IFluentType Value { get; set; }

		public FluentFunctionValue(IFluentType value)
		{
			this.Value = value;
		}

		public object AsFluentValue()
			=> Value;

		public string AsString()
			=> Value.AsString();

		public int? AsIntOrNull()
			=> int.TryParse(AsString(), out var @int) ? @int : null;

		public long? AsLongOrNull()
			=> int.TryParse(AsString(), out var @int) ? @int : null;

		public float? AsFloatOrNull()
			=> float.TryParse(AsString(), out var @int) ? @int : null;

		public double? AsDoubleOrNull()
			=> double.TryParse(AsString(), out var @int) ? @int : null;
	}

	internal class FluentValueFactory: IFluentValueFactory
	{
		public IFluentFunctionValue CreateStringValue(string value)
			=> new FluentFunctionValue(new FluentString(value));

		public IFluentFunctionValue CreateIntValue(int value)
			=> new FluentFunctionValue(FluentNumber.FromString($"{value}"));

		public IFluentFunctionValue CreateLongValue(long value)
			=> new FluentFunctionValue(FluentNumber.FromString($"{value}"));

		public IFluentFunctionValue CreateFloatValue(float value)
			=> new FluentFunctionValue(FluentNumber.FromString($"{value}"));

		public IFluentFunctionValue CreateDoubleValue(double value)
			=> new FluentFunctionValue(FluentNumber.FromString($"{value}"));
	}
}