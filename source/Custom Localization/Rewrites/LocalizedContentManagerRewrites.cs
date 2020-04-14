using System;
using System.IO;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using static StardewValley.LocalizedContentManager;

namespace StardewModdingAPI.Mods.CustomLocalization.Rewrites
{
    public class LocalizedContentManagerRewrites
    {
        public class LanguageCodeStringRewrite
        {
            public static bool Prefix(LanguageCode code, ref string __result)
            {
                switch (code)
                {
                    case LanguageCode.ja:
                    case LanguageCode.ru:
                    case LanguageCode.pt:
                    case LanguageCode.es:
                    case LanguageCode.de:
                    case LanguageCode.th:
                    case LanguageCode.fr:
                    case LanguageCode.ko:
                    case LanguageCode.it:
                    case LanguageCode.tr:
                    case LanguageCode.hu:
                        return true;
                    default:
                        foreach (ModConfig.Locale locale in ModEntry.ModConfig.locales)
                        {
                            if (locale.CodeEnum == (int)code)
                            {
                                __result = locale.LocaleCode;
                                return false;
                            }
                        }
                        return true;
                }
            }
        }
        public class GetCurrentLanguageLatinRewrite
        {
            public static bool Prefix(ref bool __result)
            {
                ModConfig.Locale locale = ModEntry.ModConfig.GetByCode((int)LocalizedContentManager.CurrentLanguageCode);
                if(locale != null)
                {
                    __result = locale.IsLatin;
                    return false;
                }
                return true;
            }
        }
    }
}
