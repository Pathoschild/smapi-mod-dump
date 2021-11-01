/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CustomCommunityCentre
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomCommunityCentre
{
	public class AssetManager : IAssetLoader, IAssetEditor
	{
		// Assets
		public static string AssetPrefix => CustomCommunityCentre.ModEntry.Instance.ModManifest.UniqueID;
		public static readonly char[] ForbiddenAssetNameCharacters = new char[]
		{
			System.IO.Path.DirectorySeparatorChar,
			Bundles.ModDataKeyDelim,
			Bundles.ModDataValueDelim
		};
		public const string RequiredAssetNamePrefix = "Custom";
		public const char RequiredAssetNameDivider = '_';
		public static readonly string RootGameContentPath = PathUtilities.NormalizeAssetName(
			$@"Mods/{CustomCommunityCentre.ModEntry.Instance.ModManifest.UniqueID}.Assets");

		// Content pack assets
		public static string ContentPackDataFileName { get; private set; } = "content";
		public static string TemporaryManifestFileName { get; private set; } = "content-pack";

		// Internal sneaky asset business
		internal static readonly string BundleCacheAssetKey = CustomCommunityCentre.AssetManager.PrefixAsset(
			asset: "BundleCacheAssetkey", prefix: CustomCommunityCentre.AssetManager.RootGameContentPath, separator: "/");

		// Asset dictionary keys
		public const string BundleMetadataKey = "Metadata";
		public const string BundleDefinitionsKey = "Definitions";
		public const string BundleSubstitutesKey = "Substitutes";

		// Asset lists
		public static readonly List<string> GameAssetKeys = new()
        {
			@"Data/Events/Town",
			@"Strings/BundleNames",
			@"Strings/Locations",
			@"Strings/UI",
		};


		public bool CanLoad<T>(IAssetInfo asset)
		{
			return asset.AssetName.StartsWith(CustomCommunityCentre.AssetManager.RootGameContentPath);
		}

		public T Load<T>(IAssetInfo asset)
		{
			// Internal sneaky asset business
			if (asset.AssetNameEquals(CustomCommunityCentre.AssetManager.BundleCacheAssetKey))
            {
				return (T)(object)BundleManager.Parse();
			}

			// Load content pack spritesheets when referenced by the game
			Texture2D spritesheet = CustomCommunityCentre.ModEntry.ContentPacks
				.Find(cp => cp.Spritesheets.Keys.Any(s => asset.AssetNameEquals(s)))
				.Spritesheets
				.First(pair => asset.AssetNameEquals(pair.Key)).Value;
			if (spritesheet != null)
			{
				return (T)(object)spritesheet;
			}

			return (T)(object)null;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return CustomCommunityCentre.AssetManager.GameAssetKeys
				.Any(assetName => asset.AssetNameEquals(assetName));
		}

		public void Edit<T>(IAssetData asset)
		{
			this.Edit(asset: ref asset); // eat that, ENC0036
		}

		public void Edit(ref IAssetData asset)
		{
			if (ModEntry.ContentPacks == null || !ModEntry.ContentPacks.Any())
				return;

            IEnumerable<Data.BundleMetadata> bundleMetadata = Bundles.GetAllCustomBundleMetadataEntries();

			if (asset.AssetNameEquals(@"Data/Events/Town"))
			{
				var data = asset.AsDictionary<string, string>().Data;

				// Append completed mail received for all custom areas as required flags for CC completion event

				const char delimiter = '/';
				const string mailFlag = "Hn";

				string eventId = ((int)Bundles.EventIds.CommunityCentreComplete).ToString();
				string eventKey = data.Keys.FirstOrDefault(key => key.Split(delimiter).First() == eventId);
				string eventScript = data[eventKey];
				string[] mailFlags = new List<string> { eventKey }
					.Concat(Bundles.CustomAreaNamesAndNumbers.Keys
						.Select(areaName => $"{mailFlag} {string.Format(Bundles.MailAreaCompleted, Bundles.GetAreaNameAsAssetKey(areaName))}"))
					.ToArray();

				data.Remove(eventKey);
				eventKey = string.Join(delimiter.ToString(), mailFlags);
				data[eventKey] = eventScript;

				asset.ReplaceWith(data);
				return;
			}

			if (asset.AssetNameEquals(@"Strings/BundleNames"))
			{
				var data = asset.AsDictionary<string, string>().Data;

				// Add bundle display names to localised bundle names dictionary
				foreach (CustomCommunityCentre.Data.BundleMetadata bmd in bundleMetadata)
				{
					foreach (string bundleName in bmd.BundleDisplayNames.Keys)
					{
						data[bundleName] = CustomCommunityCentre.Data.BundleMetadata.GetLocalisedString(
							dict: bmd.BundleDisplayNames[bundleName],
							defaultValue: bundleName);
					}
				}

				asset.ReplaceWith(data);
				return;
			}

			if (asset.AssetNameEquals(@"Strings/Locations"))
			{
				var data = asset.AsDictionary<string, string>().Data;

				// Add area display names and completion strings
				foreach (CustomCommunityCentre.Data.BundleMetadata bmd in bundleMetadata)
				{
					string areaNameAsAssetKey = Bundles.GetAreaNameAsAssetKey(bmd.AreaName);
					data[$"CommunityCenter_AreaName_{areaNameAsAssetKey}"] = bmd.AreaDisplayName
						.TryGetValue(LocalizedContentManager.CurrentLanguageCode.ToString(), out string str)
						? str
						: bmd.AreaName;

					str = CustomCommunityCentre.Data.BundleMetadata.GetLocalisedString(dict: bmd.AreaCompleteDialogue, defaultValue: string.Empty);
					data[$"CommunityCenter_AreaCompletion_{areaNameAsAssetKey}"] = str;
				}

				asset.ReplaceWith(data);
				return;
			}

			if (asset.AssetNameEquals(@"Strings/UI"))
			{
				var data = asset.AsDictionary<string, string>().Data;

				// Add reward text
				foreach (CustomCommunityCentre.Data.BundleMetadata bmd in bundleMetadata)
				{
					int areaNumber = Bundles.GetCustomAreaNumberFromName(bmd.AreaName);
					data[$"JunimoNote_Reward{areaNumber}"] = bmd.AreaCompleteDialogue
						.TryGetValue(LocalizedContentManager.CurrentLanguageCode.ToString(), out string str)
						? str
						: string.Empty;
				}

				asset.ReplaceWith(data);
				return;
			}
		}

		public static void ReloadAssets(IModHelper helper)
		{
			// Invalidate game assets
			helper.Content.InvalidateCache(@"Strings/UI");
		}

		public static string PrefixAsset(string asset, string prefix = null, string separator = ".")
		{
			return PathUtilities.NormalizeAssetName(string.Join(separator,
				prefix ?? CustomCommunityCentre.AssetManager.AssetPrefix, asset));
		}

		public static string PrefixPath(string asset, string prefix = null, string separator = "/")
		{
			return PathUtilities.NormalizePath(string.Join(separator,
				prefix ?? CustomCommunityCentre.AssetManager.RootGameContentPath, asset));
		}
	}
}
