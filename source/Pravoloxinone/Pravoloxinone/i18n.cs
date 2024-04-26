/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/Pravoloxinone
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pravoloxinone
{
    internal static class i18n
    {
        private static ITranslationHelper translation;
        public static void gethelpers(ITranslationHelper translation)
        {
            i18n.translation = translation;
        }
        public static string string_Pravoloxinone()
        {
            return i18n.GetTranslation("TheMightyAmondee.Pravoloxinone/Pravoloxinone");
        }
        public static string string_Pravoloxinone_Description()
        {
            return i18n.GetTranslation("TheMightyAmondee.Pravoloxinone/Pravoloxinone_Description");
        }
        public static string string_Pravoloxinone_Buff()
        {
            return i18n.GetTranslation("TheMightyAmondee.Pravoloxinone/Pravoloxinone_Buff");
        }
        public static string string_HarveySpeak1()
        {
            return i18n.GetTranslation("TheMightyAmondee.Pravoloxinone/HarveySpeak1", new { PlayerName = Game1.player.Name });
        }
        public static string string_HarveySpeak2()
        {
            return i18n.GetTranslation("TheMightyAmondee.Pravoloxinone/HarveySpeak2");
        }
        public static string string_HarveySpeak3()
        {
            return i18n.GetTranslation("TheMightyAmondee.Pravoloxinone/HarveySpeak3");
        }
        public static string string_HarveyMail()
        {
            return i18n.GetTranslation("TheMightyAmondee.Pravoloxinone/HarveyMail");
        }
        public static string string_GMCM_BuffChance()
        {
            return i18n.GetTranslation("TheMightyAmondee.Pravoloxinone/GMCM_BuffChance");
        }
        public static string string_GMCM_DebuffChance()
        {
            return i18n.GetTranslation("TheMightyAmondee.Pravoloxinone/GMCM_DebuffChance");
        }
        public static string string_GMCM_DamageChance()
        {
            return i18n.GetTranslation("TheMightyAmondee.Pravoloxinone/GMCM_DamageChance");
        }
        public static string string_GMCM_DeathChance()
        {
            return i18n.GetTranslation("TheMightyAmondee.Pravoloxinone/GMCM_DeathChance");
        }
        public static string string_GMCM_BuffChanceTooltip()
        {
            return i18n.GetTranslation("TheMightyAmondee.Pravoloxinone/GMCM_BuffChanceTooltip");
        }
        public static string string_GMCM_DebuffChanceTooltip()
        {
            return i18n.GetTranslation("TheMightyAmondee.Pravoloxinone/GMCM_DebuffChanceTooltip");
        }
        public static string string_GMCM_DamageChanceTooltip()
        {
            return i18n.GetTranslation("TheMightyAmondee.Pravoloxinone/GMCM_DamageChanceTooltip");
        }
        public static string string_GMCM_DeathChanceTooltip()
        {
            return i18n.GetTranslation("TheMightyAmondee.Pravoloxinone/GMCM_DeathChanceTooltip");
        }
        public static string string_GMCM_ConfigReminder()
        {
            return i18n.GetTranslation("TheMightyAmondee.Pravoloxinone/GMCM_ConfigReminder");
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
