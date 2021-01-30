/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bpendragon/GreenhouseSprinklers
**
*************************************************/

using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using StardewModdingAPI;

namespace Bpendragon.GreenhouseSprinklers
{
    /// <summary>Get translations from the mod's <c>i18n</c> folder.</summary>
    /// <remarks>This is auto-generated from the <c>i18n/default.json</c> file when the T4 template is saved.</remarks>
    [GeneratedCode("TextTemplatingFileGenerator", "1.0.0")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Deliberately named for consistency and to match translation conventions.")]
    internal static class I18n
    {
        /*********
        ** Fields
        *********/
        /// <summary>The mod's translation helper.</summary>
        private static ITranslationHelper Translations;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="translations">The mod's translation helper.</param>
        public static void Init(ITranslationHelper translations)
        {
            I18n.Translations = translations;
        }

        /// <summary>Get a translation equivalent to "@,^^The Junimos are pleased with your contributions to The Valley.^They had an idea to improve your greenhouse, I have translated these ideas into something Robin can use.^Talk to her if you want this upgrade.^^   -M. Rasmodius, Wizard".</summary>
        public static string Mail_Wizard1()
        {
            return I18n.GetByKey("mail.Wizard1");
        }

        /// <summary>Get a translation equivalent to "@,^^Despite having removed them from their home in the Community Center the Junimos are pleased with your contributions to The Valley.^They had an idea to improve your greenhouse, I have translated these ideas into something Robin can use.^Talk to her if you want this upgrade.^^   -M. Rasmodius, Wizard".</summary>
        public static string Mail_Wizard1b()
        {
            return I18n.GetByKey("mail.Wizard1b");
        }

        /// <summary>Get a translation equivalent to "@,^^The Junimos continue to be impressed with your farm.^They had an idea to further improve your greenhouse, I have translated these ideas into something Robin can use.^Talk to her if you want this upgrade.^^   -M. Rasmodius, Wizard".</summary>
        public static string Mail_Wizard2()
        {
            return I18n.GetByKey("mail.Wizard2");
        }

        /// <summary>Get a translation equivalent to "@,^^The Junimos continue to be impressed with your farm.^They had one final idea to upgrade your farm's sprinkler system, I have translated these ideas into something Robin can use.^Talk to her if you want this upgrade.^^   -M. Rasmodius, Wizard".</summary>
        public static string Mail_Wizard3()
        {
            return I18n.GetByKey("mail.Wizard3");
        }

        /// <summary>Get a translation equivalent to "Sprinkler System Upgrade".</summary>
        public static string CarpenterShop_BluePrintName()
        {
            return I18n.GetByKey("CarpenterShop.BluePrintName");
        }

        /// <summary>Get a translation equivalent to "Automated Sprinklers on the ceiling of your greenhouse, runs every morning".</summary>
        public static string CarpenterShop_FirstUpgradeDescription()
        {
            return I18n.GetByKey("CarpenterShop.FirstUpgradeDescription");
        }

        /// <summary>Get a translation equivalent to "Automated Sprinklers on the ceiling of your greenhouse, runs every morning and night".</summary>
        public static string CarpenterShop_SecondUpgradeDescription()
        {
            return I18n.GetByKey("CarpenterShop.SecondUpgradeDescription");
        }

        /// <summary>Get a translation equivalent to "Hidden underground sprinklers all over the farm, runs morning and night".</summary>
        public static string CarpenterShop_FinalUpgradeDescription()
        {
            return I18n.GetByKey("CarpenterShop.FinalUpgradeDescription");
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get a translation by its key.</summary>
        /// <param name="key">The translation key.</param>
        /// <param name="tokens">An object containing token key/value pairs. This can be an anonymous object (like <c>new { value = 42, name = "Cranberries" }</c>), a dictionary, or a class instance.</param>
        private static Translation GetByKey(string key, object tokens = null)
        {
            if (I18n.Translations == null)
                throw new InvalidOperationException($"You must call {nameof(I18n)}.{nameof(I18n.Init)} from the mod's entry method before reading translations.");
            return I18n.Translations.Get(key, tokens);
        }
    }
}
