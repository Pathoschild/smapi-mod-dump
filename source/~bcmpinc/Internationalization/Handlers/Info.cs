/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Internationalization.Handlers
{
    public class Info : RequestHandler
    {
        private readonly ITranslationHelper translation;

        struct InfoEntry {
            public Dictionary<string, ModEntry> mods;
            public Dictionary<string, LocaleEntry> locales;
            public string current_locale;
            public InfoEntry(string current_locale) {
                mods = new Dictionary<string, ModEntry>();
                locales = new Dictionary<string, LocaleEntry>();
                this.current_locale = current_locale;
            }
        }

        struct ModEntry {
            public string name;
            public Dictionary<string,TranslationStatus> locales;
            public int lines_total;
            public ModEntry(string name) { 
                this.name = name;
                locales = new Dictionary<string, TranslationStatus>();
                lines_total = 0;
            }
        }

        struct LocaleEntry {
            public string modname;
        }

        public Info(ITranslationHelper translation) {
            this.translation = translation;
        }

        public string current_locale() {
            return translation.Locale.Length > 0 ? translation.Locale.Split("-")[0] : "en";
        }

        public override bool Get(Request r) {
            if (r.path.Length == 0) {
                var options = new JsonSerializerOptions{
                    IncludeFields = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                };
                InfoEntry info = new InfoEntry(current_locale());
                foreach (var m in TranslationRegistry.AllMods()) {
                    var id = m.Manifest.UniqueID;
                    var mod = new ModEntry(m.Manifest.Name);
                    foreach (var locale in TranslationRegistry.Locales(id)) {
                        mod.locales[locale] = TranslationRegistry.Status(id,locale);
                    }
                    mod.lines_total = mod.locales["en"].lines_translated;
                    info.mods[m.Manifest.UniqueID] = mod;
                }
                DataLoader.AdditionalLanguages(Game1.content);
                foreach (var m in System.Enum.GetValues<LocalizedContentManager.LanguageCode>()) {
                    if (m != LocalizedContentManager.LanguageCode.mod) {
                        info.locales[m.ToString()] = new LocaleEntry();
                    }
                }
                foreach (var m in DataLoader.AdditionalLanguages(Game1.content)) {
                    info.locales[m.LanguageCode] = new LocaleEntry() {modname = m.Id};
                }
                foreach (var lang in TranslationRegistry.AllLanguages()) {
                    if (!info.locales.ContainsKey(lang)) {
                        info.locales[lang] = new LocaleEntry() { modname = "(unknown)" };
                    }
                }
                var data = JsonSerializer.Serialize(info, options);
                r.content_json();
                return r.write_text(HttpStatusCode.OK, data);
            } else if (r.path.Length == 1 && r.path[0] == "current") {
                return r.write_text(HttpStatusCode.OK, current_locale());
            } else {
                return r.status(HttpStatusCode.BadRequest);
            }
        }
    }
}
