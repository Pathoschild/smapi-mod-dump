/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using NUnit.Framework;

namespace StardewTests.Harness; 

public class TestTranslationHelper : ITranslationHelper {

    private readonly string _modFolder;

    private LocalizedContentManager.LanguageCode? _readLanguage;
    private Dictionary<string, string>? _readI18N;
    
    public TestTranslationHelper(string modFolder, string? modId = null) {
        _modFolder = modFolder;
        ModID = modId ?? Guid.NewGuid().ToString();
    }

    public string ModID { get; }
    public string Locale => LocaleEnum.ToString();
    public LocalizedContentManager.LanguageCode LocaleEnum { get; set; } = LocalizedContentManager.LanguageCode.en;
    public LocalizedContentManager.LanguageCode[] SupportedLocales { get; set; } =  Enum.GetValues(typeof(LocalizedContentManager.LanguageCode))
        .Cast<LocalizedContentManager.LanguageCode>().ToArray();
    public bool ValidateTranslations { get; set; } = true;
    
    public IEnumerable<Translation> GetTranslations() { 
        return GetReadFile().Select(keyValue => CreateTranslation(keyValue.Key, keyValue.Value));
    }

    public Translation Get(string key) {
        var value = GetReadFile().GetValueOrDefault(key);
        if (value == null && ValidateTranslations) {
            Assert.Fail($"Could not find key {key} in locale {Locale}.");
        }
        return CreateTranslation(key, value);
    }

    private Dictionary<string, string> GetReadFile() {
        if (_readI18N == null || LocaleEnum != _readLanguage) {
            _readI18N = ReadFile(LocaleEnum);
            _readLanguage = LocaleEnum;
        }
        return _readI18N!;
    }

    private Dictionary<string, string> ReadFile(LocalizedContentManager.LanguageCode localeEnum) {
        var locale = localeEnum == LocalizedContentManager.LanguageCode.en ? "default" : localeEnum.ToString();
        using var reader = new StreamReader($"{_modFolder}/i18n/{locale}.json");
        var json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
    }

    private Translation CreateTranslation(string key, string? text) {
        var constructorInfo = typeof(Translation).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
            new[] {typeof(string), typeof(string), typeof(string)}, null);
        return (Translation) constructorInfo!.Invoke(new object?[] {Locale, key, text});
    }

    public Translation Get(string key, object? tokens) {
        return Get(key).Tokens(tokens);
    }

    public IDictionary<string, Translation> GetInAllLocales(string key, bool withFallback = false) {
        var result = new Dictionary<string, Translation>();
        foreach (var supportedLocale in SupportedLocales) {
            if (!withFallback && supportedLocale == LocalizedContentManager.LanguageCode.en) {
                continue;
            }
            try {
                var translation = ReadFile(supportedLocale).GetValueOrDefault(key);
                if (translation != null) {
                    result[supportedLocale.ToString()] = CreateTranslation(key, translation);
                }
            } catch (FileNotFoundException) {
                // we can ignore this for now
            }
        }
        return result;
    }
}