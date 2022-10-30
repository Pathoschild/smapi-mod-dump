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

namespace DeepWoodsMod
{
    public class I18N
    {
        private static ITranslationHelper I18n;

        public static string ExcaliburDisplayName => I18n.Get("excalibur.name");
        public static string ExcaliburDescription => I18n.Get("excalibur.description");
        public static string WoodsObeliskDisplayName => I18n.Get("woods-obelisk.name");
        public static string WoodsObeliskDescription => I18n.Get("woods-obelisk.description");
        public static string EasterEggDisplayName => I18n.Get("easter-egg.name");
        public static string EasterEggHatchedMessage => I18n.Get("easter-egg.hatched-message");
        public static string LostMessage => I18n.Get("lost-message");
        public static string WoodsObeliskWizardMailMessage => I18n.Get("woods-obelisk.wizard-mail");
        public static string HealingFountainDrinkMessage => I18n.Get("healing-fountain.drink-message");
        public static string ExcaliburNopeMessage => I18n.Get("excalibur.nope-message");
        public static string MessageBoxOK => I18n.Get("messagebox.ok");
        public static string MaxHousePuzzleNopeMessage => I18n.Get("maxhouse.puzzle.nope");

        public static string OrbStoneTouchQuestion => I18n.Get("orb-stone.question");
        public static string OrbStoneTouchYes => I18n.Get("orb-stone.yes");
        public static string OrbStoneTouchNope => I18n.Get("orb-stone.no");
        public static string OrbStoneTouchMessage => I18n.Get("orb-stone.touch-message");
        public static string OrbStoneTouchMessageNoOrb => I18n.Get("orb-stone.touch-message-no-orb");

        public static string BooksMessage => I18n.Get("maxhouse.books.question");
        public static string BooksAnswerRead => I18n.Get("maxhouse.books.answer.read");
        public static string BooksAnswerNevermind => I18n.Get("maxhouse.books.answer.nevermind");
        public static string BooksInteresting => I18n.Get("maxhouse.books.interesting");


        public static string StuffMessage => I18n.Get("maxhouse.stuff.question");
        public static string StuffAnswerSearch => I18n.Get("maxhouse.stuff.answer.search");
        public static string StuffAnswerNevermind => I18n.Get("maxhouse.stuff.answer.nevermind");
        public static string StuffNothing => I18n.Get("maxhouse.stuff.nothing");

        public static string QuestsEmptyMessage => I18n.Get("maxhouse.quests.empty");
        public static string ShopEmptyMessage => I18n.Get("maxhouse.shop.empty");

        public static string BigWoodenSignMessage => I18n.Get("bigsign.message");


        public static void Init(ITranslationHelper i18n)
        {
            I18n = i18n;
        }

        public class SignTexts
        {
            public readonly static string[] textIDs = new string[]
            {
                "sign.text.welcome",
                "sign.text.random.1",
                "sign.text.random.2",
                "sign.text.random.3",
                "sign.text.random.4",
                "sign.text.random.5",
                "sign.text.random.6",
                "sign.text.random.7",
                "sign.text.random.8",
            };

            public static string Get(int index)
            {
                return I18n.Get(textIDs[index]);
            }
        }
    }
}
