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
	internal interface IModTranslationsProvider
	{
		ITranslationHelper? GetModTranslations(IManifest mod);
	}

	internal class ModTranslationsProvider : IModTranslationsProvider
	{
		private IModRegistry ModRegistry { get; set; }

		private bool IsReflectionSetup { get; set; } = false;
		private Func<IManifest, ITranslationHelper?> GetModTranslationsDelegate { get; set; } = null!;

		public ModTranslationsProvider(IModRegistry modRegistry)
		{
			this.ModRegistry = modRegistry;
		}

		private void SetupReflectionIfNeeded()
		{
			if (IsReflectionSetup)
				return;

			Type modMetadataType = AccessTools.TypeByName("StardewModdingAPI.Framework.IModMetadata, StardewModdingAPI")!;
			MethodInfo translationsGetter = AccessTools.PropertyGetter(modMetadataType, "Translations");
			GetModTranslationsDelegate = (manifest) =>
			{
				var modInfo = ModRegistry.Get(manifest.UniqueID);
				if (modInfo is null)
					return null;
				return translationsGetter.Invoke(modInfo, null) as ITranslationHelper;
			};

			IsReflectionSetup = true;
		}

		public ITranslationHelper? GetModTranslations(IManifest mod)
		{
			SetupReflectionIfNeeded();
			return GetModTranslationsDelegate(mod);
		}
	}
}