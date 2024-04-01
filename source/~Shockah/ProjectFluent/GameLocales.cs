/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewValley;
using StardewValley.GameData;
using System;
using System.Collections.Generic;

namespace Shockah.ProjectFluent
{
	public sealed class BuiltInGameLocale : IGameLocale
	{
		internal LocalizedContentManager.LanguageCode BuiltInLanguageCode { get; private set; }

		public string LocaleCode
			=> BuiltInLanguageCode == LocalizedContentManager.LanguageCode.en ? "en-US" : LocalizedContentManager.LanguageCodeString(BuiltInLanguageCode);

		public BuiltInGameLocale(LocalizedContentManager.LanguageCode code)
		{
			if (code == LocalizedContentManager.LanguageCode.mod)
				throw new ArgumentException("`mod` is not a valid built-in locale.");
			this.BuiltInLanguageCode = code;
		}

		public override string ToString()
			=> LocaleCode;
	}

	public sealed class ModGameLocale : IGameLocale
	{
		internal ModLanguage Language { get; private set; }
		public string LocaleCode => Language.LanguageCode;

		public ModGameLocale(ModLanguage language)
		{
			this.Language = language;
		}

		public override string ToString()
			=> LocaleCode;
	}

	internal static class IGameLocaleExtensions
	{
		internal static IEnumerable<string> GetRelevantLocaleCodes(this IGameLocale self)
		{
			// source: https://github.com/Pathoschild/SMAPI/blob/develop/src/SMAPI/Framework/Translator.cs

			// given locale
			yield return self.LocaleCode;

			// broader locales (like pt-BR => pt)
			var current = self.LocaleCode;
			while (true)
			{
				int dashIndex = current.LastIndexOf('-');
				if (dashIndex <= 0)
					break;

				current = current[..dashIndex];
				yield return current;
			}
		}
	}
}