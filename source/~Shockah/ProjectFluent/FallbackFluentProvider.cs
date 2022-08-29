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

namespace Shockah.ProjectFluent
{
	internal interface IFallbackFluentProvider
	{
		IFluent<string> GetFallbackFluent(IManifest mod);
	}

	internal class FallbackFluentProvider : IFallbackFluentProvider
	{
		private IModTranslationsProvider ModTranslationsProvider { get; set; }

		public FallbackFluentProvider(IModTranslationsProvider modTranslationsProvider)
		{
			this.ModTranslationsProvider = modTranslationsProvider;
		}

		public IFluent<string> GetFallbackFluent(IManifest mod)
		{
			var translations = ModTranslationsProvider.GetModTranslations(mod);
			return translations is null ? new NoOpFluent() : new I18nFluent(translations);
		}
	}
}