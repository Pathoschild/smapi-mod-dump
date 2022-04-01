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
using System.Collections.Generic;
using System.Reflection;

using StardewModdingAPI;

namespace Leclair.Stardew.Common
{
    public static class TranslationHelper {

		public static bool ContainsKey(this ITranslationHelper helper, string key) {
			// This is so ugly.
			FieldInfo field = helper.GetType()
				.GetField("Translator", BindingFlags.Instance | BindingFlags.NonPublic);

			if (field == null)
				return false;

			object Translator = field.GetValue(helper);
			if (Translator == null)
				return false;

			FieldInfo flfield = Translator.GetType()
				.GetField("ForLocale", BindingFlags.Instance | BindingFlags.NonPublic);

			if (flfield == null)
				return false;

			return flfield.GetValue(Translator) is
				IDictionary<string, Translation> ForLocale &&
				ForLocale.ContainsKey(key);
		}

	}
}
