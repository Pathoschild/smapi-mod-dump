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
using System.IO;
using System.Linq;

namespace Shockah.ProjectFluent
{
	internal interface IModFluentPathProvider
	{
		event Action<IModFluentPathProvider>? CandidatesChanged;

		IEnumerable<string> GetFilePathCandidates(IGameLocale locale, IManifest mod, string? file);
	}

	internal class SerialModDirectoryFluentPathProvider : IModFluentPathProvider, IDisposable
	{
		public event Action<IModFluentPathProvider>? CandidatesChanged;

		private IModFluentPathProvider[] Providers { get; set; }

		public SerialModDirectoryFluentPathProvider(params IModFluentPathProvider[] providers)
		{
			// making a copy on purpose
			this.Providers = providers.ToArray();

			foreach (var provider in providers)
				provider.CandidatesChanged += OnCandidatesChanged;
		}

		public void Dispose()
		{
			foreach (var provider in Providers)
				provider.CandidatesChanged -= OnCandidatesChanged;
		}

		public IEnumerable<string> GetFilePathCandidates(IGameLocale locale, IManifest mod, string? file)
		{
			foreach (var provider in Providers)
				foreach (var candidate in provider.GetFilePathCandidates(locale, mod, file))
					yield return candidate;
		}

		private void OnCandidatesChanged(IModFluentPathProvider provider)
			=> CandidatesChanged?.Invoke(this);
	}

	internal class ModFluentPathProvider : IModFluentPathProvider
	{
		// never invoked, this provider does not change the candidates
		public event Action<IModFluentPathProvider>? CandidatesChanged;

		private IModDirectoryProvider ModDirectoryProvider { get; set; }
		private IFluentPathProvider FluentPathProvider { get; set; }
		private IGameLocale? LocaleOverride { get; set; }

		public ModFluentPathProvider(IModDirectoryProvider modDirectoryProvider, IFluentPathProvider fluentPathProvider, IGameLocale? localeOverride = null)
		{
			this.ModDirectoryProvider = modDirectoryProvider;
			this.FluentPathProvider = fluentPathProvider;
			this.LocaleOverride = localeOverride;
		}

		public IEnumerable<string> GetFilePathCandidates(IGameLocale locale, IManifest mod, string? file)
		{
			var baseModPath = ModDirectoryProvider.GetModDirectoryPath(mod);
			if (baseModPath is null)
				yield break;
			foreach (var candidate in FluentPathProvider.GetFilePathCandidates(LocaleOverride ?? locale, Path.Combine(baseModPath, "i18n"), file))
				yield return candidate;
		}
	}

	internal class ContentPackAdditionalModFluentPathProvider : IModFluentPathProvider, IDisposable
	{
		public event Action<IModFluentPathProvider>? CandidatesChanged;

		private IModRegistry ModRegistry { get; set; }
		private IContentPackProvider ContentPackProvider { get; set; }
		private IFluentPathProvider FluentPathProvider { get; set; }
		private IModDirectoryProvider ModDirectoryProvider { get; set; }

		public ContentPackAdditionalModFluentPathProvider(
			IModRegistry modRegistry,
			IContentPackProvider contentPackProvider,
			IFluentPathProvider fluentPathProvider,
			IModDirectoryProvider modDirectoryProvider
		)
		{
			this.ModRegistry = modRegistry;
			this.ContentPackProvider = contentPackProvider;
			this.FluentPathProvider = fluentPathProvider;
			this.ModDirectoryProvider = modDirectoryProvider;

			contentPackProvider.ContentPacksContentsChanged += OnContentPacksContentsChanges;
		}

		public void Dispose()
		{
			ContentPackProvider.ContentPacksContentsChanged -= OnContentPacksContentsChanges;
		}

		private void OnContentPacksContentsChanges(IContentPackProvider contentPackProvider)
			=> CandidatesChanged?.Invoke(this);

		public IEnumerable<string> GetFilePathCandidates(IGameLocale locale, IManifest mod, string? file)
		{
			foreach (var content in ContentPackProvider.GetContentPackContents())
			{
				if (content.AdditionalFluentPaths is null)
					continue;
				foreach (var entry in content.AdditionalFluentPaths)
				{
					if (!entry.LocalizedMod.Equals(mod.UniqueID, StringComparison.InvariantCultureIgnoreCase))
						continue;
					if ((entry.LocalizedFile is null) != (file is null))
						continue;
					if (entry.LocalizedFile is not null && !entry.LocalizedFile.Equals(file, StringComparison.InvariantCultureIgnoreCase))
						continue;

					var localizingMod = ModRegistry.Get(entry.LocalizingMod);
					if (localizingMod is null)
						continue;
					string entryPath = ModDirectoryProvider.GetModDirectoryPath(localizingMod.Manifest);

					string localizationsPath = Path.Combine(entryPath, "i18n");
					if (entry.LocalizingSubdirectory is not null)
						localizationsPath = Path.Combine(localizationsPath, entry.LocalizingSubdirectory);
					foreach (var candidate in FluentPathProvider.GetFilePathCandidates(locale, localizationsPath, entry.LocalizingFile))
						yield return candidate;
				}
			}
		}
	}
}