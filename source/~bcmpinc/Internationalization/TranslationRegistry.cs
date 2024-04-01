/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using Microsoft.CodeAnalysis;
using StardewModdingAPI;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Internationalization
{
    struct TranslationStatus {
        public bool modified;
        public int lines_translated;
    }

    static class TranslationRegistry {
        private struct Entry { 
            readonly public IModInfo Mod;
            readonly public ITranslationHelper Translations;
            readonly public string I18nPath;
            readonly private object Translator;
            readonly public IDictionary<string, IDictionary<string, string>> All;
            readonly public IDictionary<string, bool> Dirty;

            internal Entry(IModInfo mod) {
                Mod = mod;
                Translations = ReflectionHelper.Property<ITranslationHelper>(mod, "Translations");
                I18nPath     = Path.Combine(ReflectionHelper.Property<string>(mod, "DirectoryPath"), "i18n");
                Translator   = ReflectionHelper.Field<object>(Translations, "Translator");
                All          = ReflectionHelper.Field<IDictionary<string, IDictionary<string, string>>>(Translator, "All");
                Dirty        = new Dictionary<string, bool>();
                foreach (string key in All.Keys) {
                    Dirty[key] = false;
                }
            }

            private readonly IDictionary<string, Translation> ForLocale {get => ReflectionHelper.Field<IDictionary<string, Translation>>(Translator , "ForLocale"); }
            internal bool HasKey(string key) {
                return ForLocale.ContainsKey(key);
            }

            internal void UpdateKey(string key) {
                string text = ReflectionHelper.Method<string>(Translator, "GetRaw", key, Translations.Locale, true);
                if (text != null)
                    ForLocale[key] = ReflectionHelper.Constructor<Translation>(Translations.Locale, key, text);
            }

            internal IDictionary<string, string> Current() {
                return new Dictionary<string, string>(ForLocale.Select(item => new KeyValuePair<string, string>(item.Key, item.Value.ToString())));
            }
        }

        static private Translation NewTranslation(string locale, string key, string text) {
            return ReflectionHelper.Constructor<Translation>(locale, key, text); 
        }

        static private Dictionary<string,Entry> table;

        static public void Init(IModRegistry registry) {
            // Make sure we can make new translations to prevent issues later.
            NewTranslation("", "", ""); 

            // Create the internal table.
            table = new Dictionary<string, Entry>();
            foreach (var i in registry.GetAll()) {
                var ent = new Entry(i);
                if (ent.All.Count > 0) {
                    table[i.Manifest.UniqueID] = ent;
                }
            }
        }

        internal static IEnumerable<IModInfo> AllMods() {
            return table.Values.Select(e => e.Mod);
        }

        internal static IModInfo Mod(string uniqueId) {
            if (!table.TryGetValue(uniqueId, out var e)) return null;
            return e.Mod;
        }

        internal static string TranslationPath(string uniqueId, string locale) {
            locale = en_to_default(locale);
            if (!table.TryGetValue(uniqueId, out var e)) return null;
            return Path.Combine(e.I18nPath, locale + ".json");
        }
        
        internal static IDictionary<string, string> GetAll(string uniqueId, string locale) {
            locale = en_to_default(locale);
            if (!table.TryGetValue(uniqueId, out var e)) return null;
            if (!e.All.TryGetValue(locale, out var dict)) return null;
            return dict;
        }
        internal static IDictionary<string, string> GetCurrent(string uniqueId) {
            if (!table.TryGetValue(uniqueId, out var e)) throw new System.ArgumentException($"Mod '{uniqueId}' not found!");
            return e.Current();
        }

        internal static string Get(string uniqueId, string locale, string key) {
            locale = en_to_default(locale);
            if (!table.TryGetValue(uniqueId, out var e)) return null;
            if (!e.All.TryGetValue(locale, out var dict)) return null;
            if (!dict .TryGetValue(key, out var res)) return null;
            return res;
        }

        internal static bool Set(string uniqueId, string locale, string key, string value) {
            locale = en_to_default(locale);
            if (!table.TryGetValue(uniqueId, out var e)) return false;
            
            // Make sure this key exists 
            if (!e.HasKey(key)) return false;

            // Create locale if it does not yet exist.
            if (!e.All.TryGetValue(locale, out var dict)) {
                dict = new Dictionary<string, string>();
                e.All[locale] = dict;
            }
            
            // Set new value & update current localization
            dict[key] = value;
            e.Dirty[locale] = true;
            e.UpdateKey(key);
            return true;
        }

        internal static IEnumerable<string> Locales(string uniqueId) {
            if (!table.TryGetValue(uniqueId, out var e)) throw new System.ArgumentException($"Mod '{uniqueId}' not found!");
            return e.All.Keys.Select(default_to_en);
        }

        internal static void MarkSaved(string uniqueId, string locale) {
            locale = en_to_default(locale);
            if (!table.TryGetValue(uniqueId, out var e)) return;
            e.Dirty[locale] = false;
        }

        internal static TranslationStatus Status(string uniqueId, string locale) {
            locale = en_to_default(locale);
            if (!table.TryGetValue(uniqueId, out var e)) throw new System.ArgumentException($"Mod '{uniqueId}' not found!");
            var def = e.All["default"];
            var dict = e.All[locale];
            return new TranslationStatus() {
                modified = e.Dirty[locale],
                lines_translated = def.Where((pair) => dict.TryGetValue(pair.Key, out var v) && v.Length > 0).Count(),
            };
        }

        internal static IEnumerable<string> AllLanguages() {
            return table.Values.SelectMany((mod) => mod.All.Keys).Distinct().Select(default_to_en);
        }

        private static string default_to_en(string lang) {
            return lang == "default" ? "en" : lang;
        }
        private static string en_to_default(string lang) {
            return lang == "en" ? "default" : lang;
        }
    }
}
