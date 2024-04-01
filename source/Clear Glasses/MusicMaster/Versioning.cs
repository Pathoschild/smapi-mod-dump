/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using LinqFasterer;
using MusicMaster.Configuration;
using MusicMaster.Extensions;
using System;
using System.Reflection;

namespace MusicMaster;

// TODO : make a proper Version class

internal static class Versioning {
	private static T? GetAssemblyAttribute<T>() where T : Attribute => typeof(MusicMaster).Assembly.GetCustomAttribute<T>();

	[Attributes.Ignore]
	internal static readonly string CurrentVersion =
		GetAssemblyAttribute<FullVersionAttribute>()?.Value.Split('-', 2).ElementAtOrDefaultF(0) ??
		throw new BadImageFormatException($"Could not extract version from assembly {typeof(MMConfig).Assembly.FullName ?? typeof(MMConfig).Assembly.ToString()}");

	[Attributes.Ignore]
	internal static readonly Version AssemblyVersion =
		MusicMaster.Assembly.GetName().Version ??
		throw new BadImageFormatException($"Could not extract version from assembly {typeof(MMConfig).Assembly.FullName ?? typeof(MMConfig).Assembly.ToString()}");

	internal static readonly string ChangeList = GetAssemblyAttribute<ChangeListAttribute>()?.Value ?? "local";
	internal static readonly string BuildComputerName = GetAssemblyAttribute<BuildComputerNameAttribute>()?.Value ?? "unknown";
	internal static readonly string FullVersion = GetAssemblyAttribute<FullVersionAttribute>()?.Value ?? CurrentVersion;

	internal static bool IsOutdated(string configVersion) {
		string referenceVersion = Config.ClearConfigBefore;

		var configStrArray = configVersion.Split('.');
		var referenceStrArray = referenceVersion.Split('.');

		try {
			int maxLen = Math.Max(configStrArray.Length, referenceStrArray.Length);
			for (int i = 0; i < maxLen; ++i) {
				if (configStrArray.Length <= i || configStrArray[i].IsEmpty()) {
					return true;
				}
				if (referenceStrArray.Length <= i || referenceStrArray[i].IsEmpty()) {
					return false;
				}

				var configElement = int.Parse(configStrArray[i]);
				var referenceElement = int.Parse(referenceStrArray[i]);

				if (configElement > referenceElement) {
					return false;
				}

				if (configElement < referenceElement) {
					return true;
				}
			}
		}
		catch {
			return true;
		}
		return false;
	}

	internal static string StringHeader =>
		$"MusicMaster {FullVersion} build {AssemblyVersion.Revision} ({Config.BuildConfiguration}, {ChangeList}, {BuildComputerName})";
}
