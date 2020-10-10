/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GilarF/SVM
**
*************************************************/

using StardewValley;
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace ModSettingsTab.Framework.Integration
{
    public class I18N
    {
        public string en { get; set; } = null;
        public string zh { get; set; } = null;
        public string fr { get; set; } = null;
        public string de { get; set; } = null;
        public string hu { get; set; } = null;
        public string it { get; set; } = null;
        public string ja { get; set; } = null;
        public string th { get; set; } = null;
        public string ko { get; set; } = null;
        public string pt { get; set; } = null;
        public string ru { get; set; } = null;
        public string es { get; set; } = null;
        public string tr { get; set; } = null;

        public string this[LocalizedContentManager.LanguageCode code]
        {
            get
            {
                switch (code)
                {
                    case LocalizedContentManager.LanguageCode.en:
                        return en;
                    case LocalizedContentManager.LanguageCode.ja:
                        return ja ?? en;
                    case LocalizedContentManager.LanguageCode.ru:
                        return ru ?? en;
                    case LocalizedContentManager.LanguageCode.zh:
                        return zh ?? en;
                    case LocalizedContentManager.LanguageCode.pt:
                        return pt ?? en;
                    case LocalizedContentManager.LanguageCode.es:
                        return es ?? en;
                    case LocalizedContentManager.LanguageCode.de:
                        return de ?? en;
                    case LocalizedContentManager.LanguageCode.th:
                        return th ?? en;
                    case LocalizedContentManager.LanguageCode.fr:
                        return fr ?? en;
                    case LocalizedContentManager.LanguageCode.ko:
                        return ko ?? en;
                    case LocalizedContentManager.LanguageCode.it:
                        return it ?? en;
                    case LocalizedContentManager.LanguageCode.tr:
                        return tr ?? en;
                    case LocalizedContentManager.LanguageCode.hu:
                        return hu ?? en;
                    default:
                        return null;
                }
            }
        }
    }
}