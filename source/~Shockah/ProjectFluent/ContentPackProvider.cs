/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shockah.ProjectFluent
{
	internal interface IContentPackProvider
	{
		event Action<IContentPackProvider>? ContentPacksContentsChanged;

		IEnumerable<ContentPackContent> GetContentPackContents();
	}

	internal class SerialContentPackProvider: IContentPackProvider, IDisposable
	{
		public event Action<IContentPackProvider>? ContentPacksContentsChanged;

		private IContentPackProvider[] Providers { get; set; }

		public SerialContentPackProvider(params IContentPackProvider[] providers)
		{
			// making a copy on purpose
			this.Providers = providers.ToArray();

			foreach (var provider in providers)
				provider.ContentPacksContentsChanged += OnContentPacksContentsChanged;
		}

		public void Dispose()
		{
			foreach (var provider in Providers)
				provider.ContentPacksContentsChanged -= OnContentPacksContentsChanged;
		}

		public IEnumerable<ContentPackContent> GetContentPackContents()
		{
			foreach (var provider in Providers)
				foreach (var candidate in provider.GetContentPackContents())
					yield return candidate;
		}

		private void OnContentPacksContentsChanged(IContentPackProvider provider)
			=> ContentPacksContentsChanged?.Invoke(this);
	}

	internal class AssetContentPackProvider: IContentPackProvider, IDisposable
	{
		// asset of type `List<Dictionary<string, object>>`, the inner dictionary is actually the same model as `RawContentPackContent`
		private static readonly string AssetPath = "Shockah.ProjectFluent/ContentPacks";

		public event Action<IContentPackProvider>? ContentPacksContentsChanged;

		private IMonitor Monitor { get; set; }
		private IContentEvents ContentEvents { get; set; }
		private IContentPackParser ContentPackParser { get; set; }

		private JsonSerializer JsonSerializer { get; set; }
		private IList<ContentPackContent>? CachedContentPackContents { get; set; }

		private IList<ContentPackContent> CurrentContentPackContents
		{
			get
			{
				if (CachedContentPackContents is null)
					CachedContentPackContents = ParseAssetContentPackContents();
				return CachedContentPackContents;
			}
		}

		public AssetContentPackProvider(IMonitor monitor, IDataHelper dataHelper, IContentEvents contentEvents, IContentPackParser contentPackParser)
		{
			this.Monitor = monitor;
			this.ContentEvents = contentEvents;
			this.ContentPackParser = contentPackParser;

			Type dataHelperType = AccessTools.TypeByName("StardewModdingAPI.Framework.ModHelpers.DataHelper, StardewModdingAPI");
			Type jsonHelperType = AccessTools.TypeByName("StardewModdingAPI.Toolkit.Serialization.JsonHelper, SMAPI.Toolkit");

			FieldInfo jsonHelperField = AccessTools.Field(dataHelperType, "JsonHelper");
			MethodInfo jsonSettingsGetter = AccessTools.PropertyGetter(jsonHelperType, "JsonSettings");

			var jsonHelper = jsonHelperField.GetValue(dataHelper)!;
			var jsonSettings = (JsonSerializerSettings)jsonSettingsGetter.Invoke(jsonHelper, null)!;
			this.JsonSerializer = JsonSerializer.CreateDefault(jsonSettings);

			contentEvents.AssetRequested += OnAssetRequested;
			contentEvents.AssetsInvalidated += OnAssetsInvalidated;
		}

		public void Dispose()
		{
			ContentEvents.AssetRequested -= OnAssetRequested;
			ContentEvents.AssetsInvalidated -= OnAssetsInvalidated;
		}

		private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
		{
			if (e.Name.IsEquivalentTo(AssetPath))
				e.LoadFrom(() => new List<Dictionary<string, object>>(), AssetLoadPriority.Exclusive);
		}

		private void OnAssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e)
		{
			foreach (var name in e.Names)
			{
				if (name.IsEquivalentTo(AssetPath))
				{
					CachedContentPackContents = null;
					ContentPacksContentsChanged?.Invoke(this);
					I18nIntegration.ReloadTranslations();
					break;
				}
			}
		}

		private IList<ContentPackContent> ParseAssetContentPackContents()
		{
			var results = new List<ContentPackContent>();
			var asset = Game1.content.Load<List<Dictionary<string, object>>>(AssetPath);

			int ignoredContentCount = 0;
			foreach (var entry in asset)
			{
				JObject jobject = JObject.FromObject(entry);
				var rawContent = jobject.ToObject<RawContentPackContent>(JsonSerializer);
				if (rawContent is null)
				{
					ignoredContentCount++;
					continue;
				}

				var parseResult = ContentPackParser.Parse(null, rawContent);
				foreach (var error in parseResult.Errors)
					Monitor.Log($"Asset content pack: {error}", LogLevel.Error);
				foreach (var warning in parseResult.Warnings)
					Monitor.Log($"Asset content pack: {warning}", LogLevel.Warn);
				if (parseResult.Parsed is not null)
					results.Add(parseResult.Parsed);
			}

			if (ignoredContentCount != 0)
				Monitor.Log($"{ignoredContentCount} of the provided asset content pack(s) could not be parsed. This is most likely a problem with (a) mod(s) integrating with Project Fluent, not with Project Fluent itself.", LogLevel.Error);

			return results;
		}

		public IEnumerable<ContentPackContent> GetContentPackContents()
			=> CurrentContentPackContents;
	}
}