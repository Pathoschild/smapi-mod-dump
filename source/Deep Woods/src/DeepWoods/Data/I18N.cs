/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace DeepWoodsMod
{
    public class I18N
    {
        private static ITranslationHelper I18n;

        private static string Get(string key, bool isGendered)
        {
            string text = I18n.Get(key);

            if (isGendered && text.Contains("^"))
            {
                string[] array = text.Split('^');
                var player = Game1.player;
                if (player != null)
                {
                    return player.IsMale ? array[0] : array[1];
                }
                else
                {
                    return array[1];
                }
            }

            return text;
        }

        public static string ExcaliburDisplayName => Get("excalibur.name", false);
        public static string ExcaliburDescription => Get("excalibur.description", false);
        public static string WoodsObeliskDisplayName => Get("woods-obelisk.name", false);
        public static string WoodsObeliskDescription => Get("woods-obelisk.description", false);
        public static string EasterEggDisplayName => Get("easter-egg.name", false);
        public static string EasterEggHatchedMessage => Get("easter-egg.hatched-message", false);
        public static string LostMessage => Get("lost-message", false);
        public static string WoodsObeliskWizardMailMessage => Get("woods-obelisk.wizard-mail", false);
        public static string HealingFountainDrinkMessage => Get("healing-fountain.drink-message", false);
        public static string ExcaliburNopeMessage => Get("excalibur.nope-message", false);
        public static string MessageBoxClose => Get("messagebox.close", false);
        public static string MaxHousePuzzleNopeMessage => Get("maxhouse.puzzle.nope", false);

        public static string OrbStoneTouchQuestion => Get("orb-stone.question", false);
        public static string OrbStoneTouchYes => Get("orb-stone.yes", false);
        public static string OrbStoneTouchNope => Get("orb-stone.no", false);
        public static string OrbStoneTouchMessage => Get("orb-stone.touch-message", false);
        public static string OrbStoneTouchMessageNoOrb => Get("orb-stone.touch-message-no-orb", false);

        public static string CloseBook => Get("maxhouse.books.closebook", false);

        public static string StuffMessage => Get("maxhouse.stuff.question", false);
        public static string StuffAnswerSearch => Get("maxhouse.stuff.answer.search", false);
        public static string StuffAnswerNevermind => Get("maxhouse.stuff.answer.nevermind", false);
        public static string StuffNothing => Get("maxhouse.stuff.nothing", false);

        public static string QuestsEmptyMessage => Get("maxhouse.quests.empty", false);
        public static string ShopEmptyMessage => Get("maxhouse.shop.empty", false);

        public static string BigWoodenSignMessage => Get("bigsign.message", false);
        public static string EntrySignMessage => Get("entrysign.message", false);

        public static string DeepWoodsMineCartText => Get("minecart.destination.deepwoods", false);


        public static void Init(ITranslationHelper i18n)
        {
            I18n = i18n;
        }

        public class BookTexts
        {
            public readonly static string[] textIDs = new string[]
            {
                "maxhouse.books.random.1",
                "maxhouse.books.random.2",
                "maxhouse.books.random.3",
                "maxhouse.books.random.4",
                "maxhouse.books.random.5",
            };

            public static string Get(int index)
            {
                return I18N.Get(textIDs[index], false);
            }
        }
    }
}
