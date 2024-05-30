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
using System.Diagnostics.CodeAnalysis;

using StardewModdingAPI;

namespace Leclair.Stardew.Common;

public static class LoadingHelper {

	private static IModHelper? Helper;

	[MemberNotNull(nameof(Helper))]
	public static void SetHelper(IModHelper helper) {
		Helper = helper;
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
				return pack.ModContent.Load<T>(path);

			int i = locale.IndexOf('-');
			if (i != -1) {
				path = $"{prefix}.{locale[..i]}.{postfix}";

				if (pack.HasFile(path))
					return pack.ModContent.Load<T>(path);
			}
		}

		// Still here? Return the bare resource.
		return pack.ModContent.Load<T>(key);
	}

	internal static T LoadLocalized<T>(this IModContentHelper helper, string key, string? locale = null) where T : notnull {
		int idx = string.IsNullOrEmpty(locale) ? -1 : key.LastIndexOf('.');

		// If we have an index, let's try loading various language versions.
		if (idx != -1) {
			string prefix = key[..idx];
			string postfix = key[(idx + 1)..];
			string path = $"{prefix}.{locale}.{postfix}";

			try {
				return helper.Load<T>(path);
			} catch (Exception e) {
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
}
