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
using StardewModdingAPI;
using System;
using System.Reflection;

namespace Shockah.ProjectFluent
{
	internal interface IModDirectoryProvider
	{
		string GetModDirectoryPath(IManifest mod);
	}

	internal class ModDirectoryProvider : IModDirectoryProvider
	{
		private IModRegistry ModRegistry { get; set; }

		private bool IsReflectionSetup { get; set; } = false;
		private Func<IManifest, string> GetModDirectoryPathDelegate { get; set; } = null!;

		public ModDirectoryProvider(IModRegistry modRegistry)
		{
			this.ModRegistry = modRegistry;
		}

		private void SetupReflectionIfNeeded()
		{
			if (IsReflectionSetup)
				return;

			Type modMetadataType = AccessTools.TypeByName("StardewModdingAPI.Framework.IModMetadata, StardewModdingAPI")!;
			MethodInfo directoryPathGetter = AccessTools.PropertyGetter(modMetadataType, "DirectoryPath");
			GetModDirectoryPathDelegate = (manifest) =>
			{
				var modInfo = ModRegistry.Get(manifest.UniqueID);
				return (string)directoryPathGetter.Invoke(modInfo, null)!;
			};

			IsReflectionSetup = true;
		}

		public string GetModDirectoryPath(IManifest mod)
		{
			SetupReflectionIfNeeded();
			return GetModDirectoryPathDelegate(mod);
		}
	}
}