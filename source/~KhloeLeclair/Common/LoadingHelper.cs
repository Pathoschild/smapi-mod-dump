/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

#define PRE_314

using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

using StardewModdingAPI;

using Leclair.Stardew.Common.Types;

namespace Leclair.Stardew.Common;

public static class LoadingHelper {

	private static IModHelper? Helper;

	[MemberNotNull(nameof(Helper))]
	public static void SetHelper(IModHelper helper) {
		Helper = helper;
	}


	public static void CheckIntegrations(Mod mod, IEnumerable<RecommendedIntegration>? integrations, LogLevel level = LogLevel.Warn) {
		if (integrations == null)
			return;

		var registry = mod.Helper.ModRegistry;

		foreach (RecommendedIntegration itg in integrations) {
			if (registry.IsLoaded(itg.Id) || itg.Mods == null)
				continue;

			string[] mods = itg.Mods
				.Where(id => registry.IsLoaded(id))
				.Select(id => registry.Get(id))
				.Where(info => info is not null)
				.Select(info => info!.Manifest.Name)
				.ToArray();

			if (mods.Length == 0)
				continue;

			mod.Monitor.Log(
				$"Please install {itg.Name} ({itg.Url}) to improve support for: {string.Join(", ", mods)}",
				level
			);
		}
	}


	internal static bool HasLocalizedAsset(this IContentPack pack, string key, string locale) {
		int idx = string.IsNullOrEmpty(locale) ? -1 : key.LastIndexOf('.');

		// If we have an index, let's try loading various language versions.
		if (idx != -1) {
			string prefix = key[..idx];
			string postfix = key[(idx + 1)..];

			string path = $"{prefix}.{locale}.{postfix}";

			if (pack.HasFile(path))
				return true;

			int i = locale.IndexOf('-');
			if (i != -1) {
				path = $"{prefix}.{locale[..i]}.{postfix}";

				if (pack.HasFile(path))
					return true;
			}
		}

		// Still here? Return the bare resource.
		return pack.HasFile(key);
	}

	internal static T LoadLocalizedAsset<T>(this IContentPack pack, string key, string locale) where T : notnull {
		int idx = string.IsNullOrEmpty(locale) ? -1 : key.LastIndexOf('.');

		// If we have an index, let's try loading various language versions.
		if (idx != -1) {
			string prefix = key[..idx];
			string postfix = key[(idx + 1)..];

			string path = $"{prefix}.{locale}.{postfix}";

			if (pack.HasFile(path))
#if PRE_314
				return pack.LoadAsset<T>(path);
#else
				return pack.ModContent.Load<T>(path);
#endif


			int i = locale.IndexOf('-');
			if (i != -1) {
				path = $"{prefix}.{locale[..i]}.{postfix}";

				if (pack.HasFile(path))
#if PRE_314
					return pack.LoadAsset<T>(path);
#else
					return pack.ModContent.Load<T>(path);
#endif
			}
		}

		// Still here? Return the bare resource.
#if PRE_314
		return pack.LoadAsset<T>(key);
#else
		return pack.ModContent.Load<T>(key);
#endif
	}

#if PRE_314

#else
	internal static T LoadLocalized<T>(this IModContentHelper helper, string key, string? locale = null) where T : notnull {
		int idx = string.IsNullOrEmpty(locale) ? -1 : key.LastIndexOf('.');

		// If we have an index, let's try loading various language versions.
		if (idx != -1) {
			string prefix = key[..idx];
			string postfix = key[(idx + 1)..];
			string path = $"{prefix}.{locale}.{postfix}";

			try {
				return helper.Load<T>(path);
			} catch(Exception e) {
				if (!e.Message.Contains("path doesn't exist"))
					throw;
			}

			int i = locale!.IndexOf('-');
			if (i != -1) {
				path = $"{prefix}.{locale[..i]}.{postfix}";

				try {
					return helper.Load<T>(path);
				} catch (Exception e) {
					if (!e.Message.Contains("path doesn't exist"))
						throw;
				}
			}
		}

		// Still here? Return the bare resource.
		return helper.Load<T>(key);
	}
#endif
}
