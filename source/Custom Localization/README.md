**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ZaneYork/SDV_CustomLocalization**

----

# Custom Localization #

Custom Localization is a Localization Mod for Stardew Valley Android only.

## Description ##
- For user

Extract mod into Mods folder, put localization xnb resources into Content folder.
The mod will load Content folder and add or replace xnb files into game's own content dynamically without needs of modify game's apk.

- For developer

Typically mod's config file looks like this:

```json
{
  "OriginLocaleCount": 11,
  "CurrentLanguageCode": 3,
  "locales": [
    {
      "Name": "Chinese",
      "DisplayName": "简体中文",
      "CodeEnum": 3,
      "LocaleCode": "zh-CN",
      "IsLatin": false,
      "FontFileName": "Fonts\\Chinese",
      "FontPixelZoom": 1.5
    }
  ]
}
```
| Config Name | Description   |
| ------------ | ------------ |
| OriginLocaleCount | Game default support language count, do not modify |
| CurrentLanguageCode | User current selected language code enum value |
| locales | Mod manually added locales list |
| Name | Locale name, make sure it's unique |
| DisplayName | Locale display name |
| CodeEnum | Locale enum value, game has taken 0 to 11|
| LocaleCode | Locale country code, also it is xnb file's suffix name |
| IsLatin | If the locale is latin based locale, which didn't have to provide font |
| FontFileName | Font's asset name |
| FontPixelZoom | Font's zoom scale |
