using StardewValley;

namespace StardewModdingAPI.Mods.CustomLocalization
{
    public class ModConfig
    {
        public sbyte OriginLocaleCount = 11;
        public int CurrentLanguageCode = (int)LocalizedContentManager.CurrentLanguageCode;
        public Locale[] locales { get; set;} = new Locale[] {
            new Locale("Chinese", "简体中文", 3, "zh-CN", false, "Fonts\\Chinese", 1.5f)
        };
        public class Locale
        {
            public string Name;
            public string DisplayName;
            public int CodeEnum;
            public string LocaleCode;
            public bool IsLatin;
            public string FontFileName;
            public float FontPixelZoom;

            public Locale(string name, string displayName, int codeEnum, string localeCode, bool isLatin, string fontFileName, float fontPixelZoom)
            {
                this.Name = name;
                this.DisplayName = displayName;
                this.CodeEnum = codeEnum;
                this.LocaleCode = localeCode;
                this.IsLatin = isLatin;
                this.FontFileName = fontFileName;
                this.FontPixelZoom = fontPixelZoom;
            }

        }
        public Locale GetByName(string name)
        {
            foreach (ModConfig.Locale locale in ModEntry.ModConfig.locales)
            {
                if (locale.Name == name)
                {
                    return locale;
                }
            }
            return null;
        }
        public Locale GetByCode(int codeEnum)
        {
            foreach (ModConfig.Locale locale in ModEntry.ModConfig.locales)
            {
                if (locale.CodeEnum == codeEnum)
                {
                    return locale;
                }
            }
            return null;
        }
    }
}
