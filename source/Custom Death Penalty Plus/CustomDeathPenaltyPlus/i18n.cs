/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/CustomDeathPenaltyPlus
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;

namespace CustomDeathPenaltyPlus
{
    /// <summary>
    /// Class for determining translations
    /// </summary>
    internal static class i18n
    {
        private static ITranslationHelper translation;
        private static ModConfig config;
        public static void gethelpers(ITranslationHelper translation, ModConfig config)
        {
            i18n.translation = translation;
            i18n.config = config;
        }

        private static string TranslationFixedForGender(string translationtofix)
        {
            if (translationtofix.Contains("\\"))
            {
                translationtofix = (Game1.player.IsMale ? translationtofix.Substring(0, translationtofix.IndexOf("\\")) : translationtofix.Substring(translationtofix.IndexOf("\\") + 1));
            }
            return translationtofix;
        }

        // Translations for string fragments
        public static string string_responseperson()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/responseperson"));
        }
        public static string string_responseappendix1()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/responseappendix1"));
        }
        public static string string_responseappendix2()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/responseappendix2"));
        }
        public static string string_responseappendix3()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/responseappendix3"));
        }
        public static string string_finallyawake()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/finallyawake"));
        }

        // Translations for event fragments
        public static string string_nomoneylost()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/nomoneylost"));
        }
        public static string string_nomoneylostmine()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/nomoneylostmine"));
        }
        public static string string_nocharge()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/nocharge"));
        }
        public static string string_nomoney()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/nomoney"));
        }
        public static string string_becareful()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/becareful"));
        }
        public static string string_bereallycareful()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/bereallycareful"));
        }
        public static string string_nicetoseeyou()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/nicetoseeyou"));
        }
        public static string string_wakeplayer()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/wakeplayer"));
        }
        public static string string_easynow1()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/easynow1"));
        }
        public static string string_easynow2()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/easynow2"));
        }
        public static string string_qi()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/qi"));
        }
        public static string string_whathappened()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/whathappened"));
        }
        public static string string_somethingbad()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/somethingbad"));
        }
        public static string string_moneylost()
        {
            var lost = (int)Math.Round(PlayerStateRestorer.statedeathps.Value.moneylost);
            return i18n.GetTranslation("TheMightyAmondee.CDPP/moneylost", new { moneylost = lost });
        }

        // Translations for replacements
        public static string string_replacementdialogue()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/replacementdialogue"));
        }
        public static string string_replacementmail1()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/replacementmail1"));
        }
        public static string string_replacementmail2()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/replacementmail2"));
        }

        // Translations for mail fragments
        public static string string_mailnocharge()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/mailnocharge"));
        }

        public static string string_mailnochargeharvey()
        {
            return TranslationFixedForGender(i18n.GetTranslation("TheMightyAmondee.CDPP/mailnochargeharvey"));
        }

        /// <summary>
        /// Gets the correct translation
        /// </summary>
        /// <param name="key">The translation key</param>
        /// <param name="tokens">Tokens, if any</param>
        /// <returns></returns>
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

