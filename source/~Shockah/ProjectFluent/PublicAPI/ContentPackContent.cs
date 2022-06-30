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
using System.Collections.Generic;

namespace Shockah.ProjectFluent
{
	public class RawContentPackContent
	{
		public string? ID { get; init; }
		public ISemanticVersion? Format { get; init; }
		public IList<AdditionalFluentPath>? AdditionalFluentPaths { get; init; }
		public IList<AdditionalI18nPath>? AdditionalI18nPaths { get; init; }

		public class AdditionalFluentPath
		{
			public string? LocalizedMod { get; init; }
			public string? LocalizingMod { get; init; }
			public string? LocalizedFile { get; init; }
			public string? LocalizingFile { get; init; }
			public string? LocalizingSubdirectory { get; init; }
			public bool? IgnoreMissingLocalizedMod { get; init; }
		}

		public class AdditionalI18nPath
		{
			public string? LocalizedMod { get; init; }
			public string? LocalizingMod { get; init; }
			public string? LocalizingSubdirectory { get; init; }
			public bool? IgnoreMissingLocalizedMod { get; init; }
		}
	}

	public record ContentPackContent
	{
		public string? ID { get; init; }
		public ISemanticVersion Format { get; init; }
		public IList<AdditionalFluentPath> AdditionalFluentPaths { get; init; }
		public IList<AdditionalI18nPath> AdditionalI18nPaths { get; init; }

		public ContentPackContent(string? id, ISemanticVersion format, IList<AdditionalFluentPath> additionalFluentPaths, IList<AdditionalI18nPath> additionalI18nPaths)
		{
			this.ID = id;
			this.Format = format;
			this.AdditionalFluentPaths = additionalFluentPaths;
			this.AdditionalI18nPaths = additionalI18nPaths;
		}

		public record AdditionalFluentPath
		{
			public string LocalizedMod { get; init; }
			public string LocalizingMod { get; init; }
			public string? LocalizedFile { get; init; }
			public string? LocalizingFile { get; init; }
			public string? LocalizingSubdirectory { get; init; }
			public bool IgnoreMissingLocalizedMod { get; init; }

			public AdditionalFluentPath(
				string localizedMod,
				string localizingMod,
				string? localizedFile,
				string? localizingFile,
				string? localizingSubdirectory,
				bool ignoreMissingLocalizedMod
			)
			{
				this.LocalizedMod = localizedMod;
				this.LocalizingMod = localizingMod;
				this.LocalizedFile = localizedFile;
				this.LocalizingFile = localizingFile;
				this.LocalizingSubdirectory = localizingSubdirectory;
				this.IgnoreMissingLocalizedMod = ignoreMissingLocalizedMod;
			}
		}

		public record AdditionalI18nPath
		{
			public string LocalizedMod { get; init; }
			public string LocalizingMod { get; init; }
			public string? LocalizingSubdirectory { get; init; }
			public bool IgnoreMissingLocalizedMod { get; init; }

			public AdditionalI18nPath(
				string localizedMod,
				string localizingMod,
				string? localizingSubdirectory,
				bool ignoreMissingLocalizedMod
			)
			{
				this.LocalizedMod = localizedMod;
				this.LocalizingMod = localizingMod;
				this.LocalizingSubdirectory = localizingSubdirectory;
				this.IgnoreMissingLocalizedMod = ignoreMissingLocalizedMod;
			}
		}
	}
}