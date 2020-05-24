using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static void Init(ITranslationHelper i18n)
        {
            I18n = i18n;
        }
    }
}
