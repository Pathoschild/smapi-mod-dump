/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/WhoLivesHere
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhoLivesHereCore.i18n
{
    internal static class I18n
    {
        private static ITranslationHelper Translations;
        public static void Init(ITranslationHelper translations)
        {
            Translations = translations;
        }
        private static Translation GetByKey(string key, object tokens = null)
        {
            if (Translations == null)
                throw new InvalidOperationException($"You must call {nameof(I18n)}.{nameof(Init)} from the mod's entry method before reading translations.");
            return Translations.Get(key, tokens);
        }
        public static string ShowMissing()
        {
            return GetByKey("showmissinghay");
        }
        public static string ShowMissing_TT()
        {
            return GetByKey("showmissinghay.tt");
        }

        public static string HideEmpty()
        {
            return GetByKey("hideempty");
        }
        public static string HideEmpty_TT()
        {
            return GetByKey("hideempty.tt");
        }

        public static string PageDelay()
        {
            return GetByKey("pagedelay");
        }
        public static string PageDelay_TT()
        {
            return GetByKey("pagedelay.tt");
        }
        public static string AutoOff()
        {
            return GetByKey("autooff");
        }
        public static string AutoOff_TT()
        {
            return GetByKey("autooff.tt");
        }
        public static string AutoOn()
        {
            return GetByKey("autoon");
        }
        public static string AutoOn_TT()
        {
            return GetByKey("autoon.tt");
        }
        public static string ToggleKey()
        {
            return GetByKey("togglekey");
        }
        public static string ToggleKey_TT()
        {
            return GetByKey("togglekey.tt");
        }
    }
}
