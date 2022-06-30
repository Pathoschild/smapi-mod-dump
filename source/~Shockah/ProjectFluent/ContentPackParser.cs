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
using System.Collections.Immutable;
using System.Linq;

namespace Shockah.ProjectFluent
{
	internal interface IContentPackParser
	{
		ParseResult<ContentPackContent> Parse(IManifest? context, RawContentPackContent raw);
	}

	internal record ParseResult<Result> where Result : notnull
	{
		public Result? Parsed { get; init; }
		public ImmutableList<string> Errors { get; init; }
		public ImmutableList<string> Warnings { get; init; }

		public ParseResult(Result? parsed, ImmutableList<string> errors, ImmutableList<string> warnings)
		{
			this.Parsed = parsed;
			this.Errors = errors;
			this.Warnings = warnings;
		}
	}

	internal class ContentPackParser: IContentPackParser
	{
		private ISemanticVersion ProjectFluentVersion { get; set; }
		private IModRegistry ModRegistry { get; set; }

		private ISemanticVersion OldestVersion { get; set; }
		private ISemanticVersion IgnoreMissingLocalizedModVersion { get; set; }

		public ContentPackParser(ISemanticVersion projectFluentVersion, IModRegistry modRegistry)
		{
			this.ProjectFluentVersion = projectFluentVersion;
			this.ModRegistry = modRegistry;

			this.OldestVersion = new SemanticVersion(1, 0, 0);
			this.IgnoreMissingLocalizedModVersion = new SemanticVersion(1, 1, 0);
		}

		public ParseResult<ContentPackContent> Parse(IManifest? context, RawContentPackContent raw)
		{
			List<string> errors = new();
			List<string> warnings = new();

			if (raw.ID is null && context is null)
				warnings.Add($"Missing `{nameof(raw.ID)}` field. The field is not required, but it is recommended when asset editing to allow patching by other mods.");
			else if (raw.ID is not null && context is not null)
				warnings.Add($"Unnecessary `{nameof(raw.ID)}` field. The field is only requred when asset editing to allow patching by other mods.");

			if (raw.Format is null)
				errors.Add($"Missing `{nameof(raw.Format)}` field.");
			else if (raw.Format.IsNewerThan(ProjectFluentVersion))
				errors.Add($"`{nameof(raw.Format)}` is newer than {ProjectFluentVersion} and cannot be parsed.");
			else if (raw.Format.IsOlderThan(OldestVersion))
				errors.Add($"`{nameof(raw.Format)}` is older than {OldestVersion} and cannot be parsed.");

			if (errors.Count != 0)
				return new(null, errors: errors.ToImmutableList(), warnings: warnings.ToImmutableList());

			List<ContentPackContent.AdditionalFluentPath> additionalFluentPaths = new();
			if (raw.AdditionalFluentPaths is not null)
			{
				foreach (var (entry, entryIndex) in raw.AdditionalFluentPaths.Select((e, i) => (e, i)))
				{
					var entryParseResult = ParseAdditionalFluentPath(context, entry, raw.Format!);
					foreach (var error in entryParseResult.Errors)
						errors.Add($"`{nameof(raw.AdditionalFluentPaths)}`: #{entryIndex}: {error}");
					foreach (var warning in entryParseResult.Warnings)
						warnings.Add($"`{nameof(raw.AdditionalFluentPaths)}`: #{entryIndex}: {warning}");
					if (entryParseResult.Parsed is not null)
						additionalFluentPaths.Add(entryParseResult.Parsed);
				}
			}

			List<ContentPackContent.AdditionalI18nPath> additionalI18nPaths = new();
			if (raw.AdditionalI18nPaths is not null)
			{
				foreach (var (entry, entryIndex) in raw.AdditionalI18nPaths.Select((e, i) => (e, i)))
				{
					var entryParseResult = ParseAdditionalI18nPath(context, entry, raw.Format!);
					foreach (var error in entryParseResult.Errors)
						errors.Add($"`{nameof(raw.AdditionalI18nPaths)}`: #{entryIndex}: {error}");
					foreach (var warning in entryParseResult.Warnings)
						warnings.Add($"`{nameof(raw.AdditionalI18nPaths)}`: #{entryIndex}: {warning}");
					if (entryParseResult.Parsed is not null)
						additionalI18nPaths.Add(entryParseResult.Parsed);
				}
			}

			ContentPackContent? parsed = errors.Count == 0 ? new(
				raw.ID,
				raw.Format!,
				additionalFluentPaths,
				additionalI18nPaths
			) : null;
			return new(parsed, errors: errors.ToImmutableList(), warnings: warnings.ToImmutableList());
		}

		private ParseResult<ContentPackContent.AdditionalFluentPath> ParseAdditionalFluentPath(IManifest? context, RawContentPackContent.AdditionalFluentPath raw, ISemanticVersion format)
		{
			List<string> errors = new();
			List<string> warnings = new();

			if (raw.LocalizedMod is null)
				errors.Add($"Missing `{nameof(raw.LocalizedMod)}` field.");

			string? localizingMod = null;
			if (raw.LocalizingMod is not null)
				localizingMod = raw.LocalizingMod;
			else if (context is not null)
				localizingMod = context.UniqueID;
			else
				errors.Add($"Missing `{nameof(raw.LocalizingMod)}` field.");

			bool ignoreMissingLocalizedMod = false;
			if (!format.IsOlderThan(IgnoreMissingLocalizedModVersion))
				ignoreMissingLocalizedMod = raw.IgnoreMissingLocalizedMod ?? false;

			if (raw.LocalizedMod is not null)
			{
				var localizedModInstance = ModRegistry.Get(raw.LocalizedMod);
				if (localizedModInstance is null)
				{
					if (!ignoreMissingLocalizedMod)
						warnings.Add($"`{nameof(raw.LocalizedMod)}` specifies mod `{raw.LocalizedMod}` that is not currently loaded.");
					else if (ProjectFluent.Instance.Config.DeveloperMode)
						warnings.Add($"[Developer Mode] `{nameof(raw.LocalizedMod)}` specifies mod `{raw.LocalizedMod}` that is not currently loaded. The developer opted out of this warning. This may or may not be an actual problem.");
				}
			}

			if (localizingMod is not null)
			{
				if (localizingMod.Equals("this", StringComparison.InvariantCultureIgnoreCase))
				{
					if (context is null)
					{
						errors.Add($"`{nameof(raw.LocalizingMod)}` is `this`, but we do not know the context of `this` (are you trying to use `this` while asset editing?).");
						localizingMod = null;
					}
					else
					{
						localizingMod = context.UniqueID;
					}
				}
				else
				{
					var localizingModInstance = ModRegistry.Get(localizingMod);
					if (localizingModInstance is null)
						warnings.Add($"`{nameof(raw.LocalizingMod)}` specifies mod `{localizingMod}` that is not currently loaded.");
				}
			}

			ContentPackContent.AdditionalFluentPath? parsed = errors.Count == 0 ? new(
				raw.LocalizedMod!,
				localizingMod!,
				raw.LocalizingFile,
				raw.LocalizedFile,
				raw.LocalizingSubdirectory,
				ignoreMissingLocalizedMod
			) : null;
			return new(parsed, errors: errors.ToImmutableList(), warnings: warnings.ToImmutableList());
		}

		private ParseResult<ContentPackContent.AdditionalI18nPath> ParseAdditionalI18nPath(IManifest? context, RawContentPackContent.AdditionalI18nPath raw, ISemanticVersion format)
		{
			List<string> errors = new();
			List<string> warnings = new();

			if (raw.LocalizedMod is null)
				errors.Add($"Missing `{nameof(raw.LocalizedMod)}` field.");

			string? localizingMod = null;
			if (raw.LocalizingMod is not null)
				localizingMod = raw.LocalizingMod;
			else if (context is not null)
				localizingMod = context.UniqueID;
			else
				errors.Add($"Missing `{nameof(raw.LocalizingMod)}` field.");

			bool ignoreMissingLocalizedMod = false;
			if (!format.IsOlderThan(IgnoreMissingLocalizedModVersion))
				ignoreMissingLocalizedMod = raw.IgnoreMissingLocalizedMod ?? false;

			if (raw.LocalizedMod is not null)
			{
				var localizedModInstance = ModRegistry.Get(raw.LocalizedMod);
				if (localizedModInstance is null)
				{
					if (!ignoreMissingLocalizedMod)
						warnings.Add($"`{nameof(raw.LocalizedMod)}` specifies mod `{raw.LocalizedMod}` that is not currently loaded.");
					else if (ProjectFluent.Instance.Config.DeveloperMode)
						warnings.Add($"[Developer Mode] `{nameof(raw.LocalizedMod)}` specifies mod `{raw.LocalizedMod}` that is not currently loaded. The developer opted out of this warning. This may or may not be an actual problem.");
				}
			}

			if (localizingMod is not null)
			{
				if (localizingMod.Equals("this", StringComparison.InvariantCultureIgnoreCase))
				{
					if (context is null)
					{
						errors.Add($"`{nameof(raw.LocalizingMod)}` is `this`, but we do not know the context of `this` (are you trying to use `this` while asset editing?).");
						localizingMod = null;
					}
					else
					{
						localizingMod = context.UniqueID;
					}
				}
				else
				{
					var localizingModInstance = ModRegistry.Get(localizingMod);
					if (localizingModInstance is null)
						warnings.Add($"`{nameof(raw.LocalizingMod)}` specifies mod `{localizingMod}` that is not currently loaded.");
				}
			}

			ContentPackContent.AdditionalI18nPath? parsed = errors.Count == 0 ? new(
				raw.LocalizedMod!,
				localizingMod!,
				raw.LocalizingSubdirectory,
				ignoreMissingLocalizedMod
			) : null;
			return new(parsed, errors: errors.ToImmutableList(), warnings: warnings.ToImmutableList());
		}
	}
}