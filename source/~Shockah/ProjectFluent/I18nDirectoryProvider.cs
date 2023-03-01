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
	internal interface II18nDirectoryProvider
	{
		event Action<II18nDirectoryProvider>? DirectoriesChanged;

		IEnumerable<string> GetI18nDirectories(IManifest mod);
	}

	internal class SerialI18nDirectoryProvider : II18nDirectoryProvider
	{
		public event Action<II18nDirectoryProvider>? DirectoriesChanged;

		private II18nDirectoryProvider[] Providers { get; set; }

		public SerialI18nDirectoryProvider(params II18nDirectoryProvider[] providers)
		{
			// making a copy on purpose
			this.Providers = providers.ToArray();

			foreach (var provider in providers)
				provider.DirectoriesChanged += OnDirectoriesChanged;
		}

		public IEnumerable<string> GetI18nDirectories(IManifest mod)
		{
			foreach (var provider in Providers)
				foreach (var path in provider.GetI18nDirectories(mod))
					yield return path;
		}

		private void OnDirectoriesChanged(II18nDirectoryProvider provider)
			=> DirectoriesChanged?.Invoke(this);
	}

	internal class ContentPackI18nDirectoryProvider : II18nDirectoryProvider, IDisposable
	{
		public event Action<II18nDirectoryProvider>? DirectoriesChanged;

		private IModRegistry ModRegistry { get; set; }
		private IContentPackProvider ContentPackProvider { get; set; }
		private IModDirectoryProvider ModDirectoryProvider { get; set; }

		public ContentPackI18nDirectoryProvider(IModRegistry modRegistry, IContentPackProvider contentPackProvider, IModDirectoryProvider modDirectoryProvider)
		{
			this.ModRegistry = modRegistry;
			this.ContentPackProvider = contentPackProvider;
			this.ModDirectoryProvider = modDirectoryProvider;

			contentPackProvider.ContentPacksContentsChanged += OnContentPacksContentsChanges;
		}

		public void Dispose()
		{
			ContentPackProvider.ContentPacksContentsChanged -= OnContentPacksContentsChanges;
		}

		private void OnContentPacksContentsChanges(IContentPackProvider contentPackProvider)
			=> DirectoriesChanged?.Invoke(this);

		public IEnumerable<string> GetI18nDirectories(IManifest mod)
		{
			foreach (var content in ContentPackProvider.GetContentPackContents())
			{
				if (content.AdditionalI18nPaths is null)
					continue;
				foreach (var entry in content.AdditionalI18nPaths)
				{
					if (!entry.LocalizedMod.Equals(mod.UniqueID, StringComparison.InvariantCultureIgnoreCase))
						continue;

					var localizingMod = ModRegistry.Get(entry.LocalizingMod);
					if (localizingMod is null)
						continue;
					string entryPath = ModDirectoryProvider.GetModDirectoryPath(localizingMod.Manifest);

					string localizationsPath = Path.Combine(entryPath, "i18n");
					if (entry.LocalizingSubdirectory is not null)
						localizationsPath = Path.Combine(localizationsPath, entry.LocalizingSubdirectory);
					yield return localizationsPath;
				}
			}
		}
	}
}