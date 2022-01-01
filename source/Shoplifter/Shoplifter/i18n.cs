/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/Shoplifter
**
*************************************************/

using System;
using StardewValley;
using StardewModdingAPI;


namespace Shoplifter
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

        public static string string_Shoplift()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/Shoplift");
        }

        public static string string_Banned()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/Banned");
        }

        public static string string_Caught(string shopkeeper)
        {
            var fineamount = Math.Min(Game1.player.Money, (int)config.MaxFine);
            return i18n.GetTranslation($"TheMightyAmondee.Shoplifter/Caught{shopkeeper}", new { fineamount = fineamount });
        }

        public static string string_Caught_NoMoney(string shopkeeper)
        {
            return i18n.GetTranslation($"TheMightyAmondee.Shoplifter/Caught{shopkeeper}_NoMoney");
        }

        public static string string_BanFromShop()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/BanFromShop", new { daysbanned = config.DaysBannedFor });
        }

        public static string string_BanFromShop_Single()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/BanFromShop_Single");
        }

        public static string string_AlreadyShoplifted()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/AlreadyShoplifted", new { shopliftingamount = config.MaxShopliftsPerDay });
        }

        public static string string_AlreadyShoplifted_Single()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/AlreadyShoplifted_Single");
        }

        public static string string_AlreadyShopliftedSameShop()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/AlreadyShopliftedSameShop");
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
