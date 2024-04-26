/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/WeatherTotems
**
*************************************************/

using System;
using StardewValley;
using StardewModdingAPI;

namespace WeatherTotems
{
    internal static class i18n
    {
        private static ITranslationHelper translation;
        public static void gethelpers(ITranslationHelper translation)
        {
            i18n.translation = translation;
        }
        public static string string_SunTotemUse()
        {
            return i18n.GetTranslation("TheMightyAmondee.WeatherTotems/SunTotemUse");
        }
        public static string string_WindTotemUse()
        {
            return i18n.GetTranslation("TheMightyAmondee.WeatherTotems/WindTotemUse");
        }
        public static string string_SnowTotemUse()
        {
            return i18n.GetTranslation("TheMightyAmondee.WeatherTotems/SnowTotemUse");
        }
        public static string string_ThunderTotemUse()
        {
            return i18n.GetTranslation("TheMightyAmondee.WeatherTotems/ThunderTotemUse");
        }
        public static string string_GreenRainTotemUse()
        {
            return i18n.GetTranslation("TheMightyAmondee.WeatherTotems/GreenRainTotemUse");
        }
        public static string string_Error()
        {
            return i18n.GetTranslation("TheMightyAmondee.WeatherTotems/Error");
        }

        public static string string_ErrorFestival()
        {
            return i18n.GetTranslation("TheMightyAmondee.WeatherTotems/ErrorFestival");
        }

        /// <summary>
        /// Gets the correct translation
        /// </summary>
        /// <param name="key">The translation key</param>
        /// <param name="tokens">Tokens, if any</param>
        /// <returns>The translated string</returns>
        public static Translation GetTranslation(string key, object tokens = null)
        {
            if (i18n.translation == null)
            {
                throw new InvalidOperationException($"You must call {nameof(i18n)}.{nameof(i18n.gethelpers)} from the mod's entry method before reading translations.");
            }

            return i18n.translation.Get(key, tokens);
        }
    }
}
