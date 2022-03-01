/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;

namespace Shockah.CommonModCode
{
	public static class I18nEnum
	{
		public static IEnumerable<string> GetTranslations<EnumType>(Func<EnumType, string> translationFunction) where EnumType: Enum
		{
			foreach (object value in Enum.GetValues(typeof(EnumType)))
				yield return translationFunction((EnumType)value);
		}

		public static EnumType? GetFromTranslation<EnumType>(string translation, Func<EnumType, string> translationFunction) where EnumType: Enum
		{
			foreach (object value in Enum.GetValues(typeof(EnumType)))
				if (translationFunction((EnumType)value) == translation)
					return (EnumType)value;
			return default;
		}
	}
}
