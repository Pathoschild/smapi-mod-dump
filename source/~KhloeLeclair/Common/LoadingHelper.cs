/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StardewModdingAPI;

using Leclair.Stardew.Common.Types;

namespace Leclair.Stardew.Common
{
    public static class LoadingHelper
    {

		private static IModHelper Helper;

		public static void SetHelper(IModHelper helper) {
			Helper = helper;
		}


		public static void CheckIntegrations(Mod mod, IEnumerable<RecommendedIntegration> integrations, LogLevel level = LogLevel.Warn) {
			if (integrations == null)
				return;

			var registry = mod.Helper.ModRegistry;

			foreach (RecommendedIntegration itg in integrations) {
				if (registry.IsLoaded(itg.Id) || itg.Mods == null)
					continue;

				string[] mods = itg.Mods
					.Where(id => registry.IsLoaded(id))
					.Select(id => registry.Get(id))
					.Select(info => info.Manifest.Name)
					.ToArray();

				if (mods.Length == 0)
					continue;

				mod.Monitor.Log(
					$"Please install {itg.Name} ({itg.Url}) to improve support for: {string.Join(", ", mods)}",
					level
				);
			}
		}


		public static bool HasLocalizedAsset(this IContentPack pack, string key, string locale) {
			int idx = string.IsNullOrEmpty(locale) ? -1 : key.LastIndexOf('.');

			// If we have an index, let's try loading various language versions.
			if (idx != -1) {
				string prefix = key.Substring(0, idx);
				string postfix = key.Substring(idx + 1);

				string path = $"{prefix}.{locale}.{postfix}";

				if (pack.HasFile(path))
					return true;

				int i = locale.IndexOf('-');
				if (i != -1) {
					path = $"{prefix}.{locale.Substring(0, i)}.{postfix}";

					if (pack.HasFile(path))
						return true;
				}
			}

			// Still here? Return the bare resource.
			return pack.HasFile(key);
		}

		public static T LoadLocalizedAsset<T>(this IContentPack pack, string key, string locale) {
			int idx = string.IsNullOrEmpty(locale) ? -1 : key.LastIndexOf('.');

			// If we have an index, let's try loading various language versions.
			if (idx != -1) {
				string prefix = key.Substring(0, idx);
				string postfix = key.Substring(idx + 1);

				string path = $"{prefix}.{locale}.{postfix}";

				if (pack.HasFile(path))
					return pack.LoadAsset<T>(path);

				int i = locale.IndexOf('-');
				if (i != -1) {
					path = $"{prefix}.{locale.Substring(0, i)}.{postfix}";

					if (pack.HasFile(path))
						return pack.LoadAsset<T>(path);
				}
			}

			// Still here? Return the bare resource.
			return pack.LoadAsset<T>(key);
		}


		public static T LoadLocalized<T>(this IContentHelper helper, string key, string locale = null, ContentSource source = ContentSource.ModFolder) {
			locale ??= helper.CurrentLocale;
			int idx = string.IsNullOrEmpty(locale) ? -1 : key.LastIndexOf('.');

			// If we have an index, let's try loading various language versions.
			if (idx != -1) {
				string prefix = key.Substring(0, idx);
				string postfix = key.Substring(idx + 1);

				string path = $"{prefix}.{locale}.{postfix}";

				try {
					return helper.Load<T>(path, source);
				} catch (Exception e) {
					if (!e.Message.Contains("path doesn't exist"))
						throw;
				}

				int i = locale.IndexOf('-');
				if (i != -1) {
					path = $"{prefix}.{locale.Substring(0, i)}.{postfix}";

					try {
						return helper.Load<T>(path, source);
					} catch (Exception e) {
						if (!e.Message.Contains("path doesn't exist"))
							throw;
					}
				}
			}

			// Still here? Return the bare resource.
			return helper.Load<T>(key, source);
		}
	}
}
