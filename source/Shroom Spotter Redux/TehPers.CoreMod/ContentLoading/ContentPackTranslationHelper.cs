/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.ContentLoading;

namespace TehPers.CoreMod.ContentLoading {
    internal class ContentPackTranslationHelper : ICoreTranslationHelper {
        private readonly ICoreApi _api;
        private readonly IContentSource _contentSource;
        private readonly Dictionary<string, string> _defaultTranslations;
        private Dictionary<string, string> _translations;
        private LocalizedContentManager.LanguageCode? _lastLocale = null;

        public ContentPackTranslationHelper(ICoreApi api, IContentSource contentSource) {
            this._api = api;
            this._contentSource = contentSource;
            this._defaultTranslations = this._api.Json.ReadJson<Dictionary<string, string>>("i18n/default.json", this._contentSource, settings => { }) ?? new Dictionary<string, string>();
        }

        public ICoreTranslation Get(string key) {
            this.UpdateTranslations();

            // Try to get the current translation
            if (this._translations.TryGetValue(key, out string value)) {
                return new CoreTranslation(key, value);
            }

            // Try to get the default translation
            return this._defaultTranslations.TryGetValue(key, out value) ? new CoreTranslation(key, value) : new CoreTranslation(key);
        }

        public IEnumerable<ICoreTranslation> GetAll() {
            this.UpdateTranslations();
            return this._translations.Select(kv => new CoreTranslation(kv.Key, kv.Value));
        }

        private void UpdateTranslations() {
            if (this._lastLocale == this._api.Owner.Helper.Content.CurrentLocaleConstant) {
                return;
            }

            this._lastLocale = this._api.Owner.Helper.Content.CurrentLocaleConstant;
            this._translations = this.GetTranslationsForLocale(this._lastLocale.Value) ?? this.GetTranslationsForLocale(LocalizedContentManager.LanguageCode.en) ?? new Dictionary<string, string>();
        }

        private Dictionary<string, string> GetTranslationsForLocale(LocalizedContentManager.LanguageCode locale) {
            return locale == LocalizedContentManager.LanguageCode.en ? this._defaultTranslations : this._api.Json.ReadJson<Dictionary<string, string>>($"i18n/{locale.ToString()}.json", this._contentSource, settings => { });
        }
    }
}