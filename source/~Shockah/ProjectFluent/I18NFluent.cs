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
	internal class I18nFluent : IFluent<string>
	{
		private readonly ITranslationHelper Translations;

		public I18nFluent(ITranslationHelper translations)
		{
			this.Translations = translations;
		}

		public bool ContainsKey(string key)
			=> Translations.Get(key).HasValue();

		public string Get(string key, object? tokens)
			=> Translations.Get(key, tokens);
	}
}