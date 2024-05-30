/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using DeluxeJournal.Task;

namespace DeluxeJournal.Util
{
    /// <inheritdoc cref="LocalizedGameDataMap{T}"/>
    public abstract class LocalizedGameDataMap : LocalizedGameDataMap<string>
    {
        public LocalizedGameDataMap(string i18nAliasPrefix, ITranslationHelper translation, TaskParserSettings settings, IMonitor? monitor = null)
            : base(i18nAliasPrefix, translation, settings, monitor)
        {
        }

        /// <returns>An object identifier or <c>null</c> if none was found.</returns>
        /// <inheritdoc cref="LocalizedGameDataMap{T}.GetValue(string)"/>
        public override string? GetValue(string localizedName)
        {
            return base.GetValue(localizedName);
        }

        public override bool TryGetValue(string localizedName, out string value)
        {
            return !string.IsNullOrEmpty(value = base.GetValue(localizedName) ?? string.Empty);
        }
    }

    /// <summary>Stores a mapping of localized display names to their corresponding game data identifiers.</summary>
    public abstract class LocalizedGameDataMap<T> where T : class
    {
        private readonly ITranslationHelper _translation;
        private readonly TaskParserSettings _settings;
        private readonly IDictionary<string, HashSet<T>> _data;

        /// <summary>Parser settings.</summary>
        protected TaskParserSettings Settings => _settings;

        /// <summary>Translation helper.</summary>
        protected ITranslationHelper Translation => _translation;

        /// <summary>Monitor for logging messages.</summary>
        protected IMonitor? Monitor { get; }

        public LocalizedGameDataMap(string i18nAliasPrefix, ITranslationHelper translation, TaskParserSettings settings, IMonitor? monitor = null)
        {
            _data = new Dictionary<string, HashSet<T>>();
            _translation = translation;
            _settings = settings;
            Monitor = monitor;

            PopulateDataMap();

            if (!string.IsNullOrEmpty(i18nAliasPrefix))
            {
                ApplyTranslationAliases(i18nAliasPrefix);
            }
        }

        /// <summary>Get an enumerable list of all game object identifiers for a given display name.</summary>
        /// <param name="localizedName">Localized display name (the one that appears in-game).</param>
        /// <returns>An enumerable list of game object identifiers that had the same localized display name.</returns>
        public IEnumerable<T> GetValues(string localizedName)
        {
            return TryGetValues(localizedName, out var values) ? values : Enumerable.Empty<T>();
        }

        /// <summary>Try to get an enumerable list of all game object identifiers for a given display name.</summary>
        /// <param name="localizedName">Localized display name (the one that appears in-game).</param>
        /// <param name="values">An enumerable list of all game object identifiers.</param>
        /// <returns><c>true</c> if an enumerable list of game object identifiers was found and <c>false</c> otherwise.</returns>
        public bool TryGetValues(string localizedName, out IEnumerable<T> values)
        {
            string? key = localizedName.Trim().ToLower();
            key = Settings.EnableFuzzySearch ? Utility.fuzzySearch(key, _data.Keys.ToList()) : key;

            if (key != null && _data.TryGetValue(key, out var matches))
            {
#if DEBUG
                if (Monitor is IMonitor monitor)
                {
                    monitor.Log($"{nameof(LocalizedGameDataMap)}.{nameof(TryGetValues)}: '{key}' => '{string.Join(',', matches)}'");
                }
#endif
                values = matches;
                return true;
            }

            values = Enumerable.Empty<T>();
            return false;
        }

        /// <summary>Get a game object identifier for a given display name.</summary>
        /// <param name="localizedName">Localized display name (the one that appears in-game).</param>
        /// <returns>A game object identifier or <c>default(T)</c> if none was found.</returns>
        public virtual T? GetValue(string localizedName)
        {
            return GetValues(localizedName).FirstOrDefault();
        }

        /// <summary>Try to get a game object identifier for a given display name.</summary>
        /// <param name="localizedName">Localized display name (the one that appears in-game).</param>
        /// <param name="value">The game object identifier.</param>
        /// <returns><c>true</c> if a localized name was matched to a game object identifier and <c>false</c> if none was found.</returns>
        public virtual bool TryGetValue(string localizedName, out T? value)
        {
            return EqualityComparer<T>.Default.Equals(value = GetValue(localizedName), default);
        }

        /// <summary>Add a localized display name and game object identifier pair to the data map.</summary>
        /// <param name="key">Localized display name (the one that appears in-game).</param>
        /// <param name="value">A game object identifier.</param>
        /// <param name="parent">A game object identifier to be added first if the alias group does not exist.</param>
        /// <returns>
        /// <c>true</c> if the <paramref name="value"/> was added for the given <paramref name="key"/>;
        /// <c>false</c> if the <paramref name="value"/> already exists, or <paramref name="key"/> is <c>null</c> or empty.
        /// </returns>
        protected bool Add(string key, T value, T? parent = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            key = key.ToLower();

            if (!_data.ContainsKey(key))
            {
                _data.Add(key, new HashSet<T>());

                if (parent != null)
                {
                    _data[key].Add(parent);
                }
            }
#if DEBUG
            else if (Monitor is IMonitor monitor)
            {
                monitor.LogOnce($"{nameof(LocalizedGameDataMap)}.{nameof(Add)}: Added duplicate key='{key}' with value='{value}'");
            }
#endif
            return _data[key].Add(value);
        }

        /// <summary>Add a localized display name along with its plural form to the data map.</summary>
        /// <remarks>
        /// The plural name is added as an alias, unless it is marked as <paramref name="strict"/>.
        /// See <see cref="AddAlias(string, string)"/>.
        /// </remarks>
        /// <param name="strict">Strictly add the plural and not the singular.</param>
        /// <inheritdoc cref="Add(string, T, T)"/>
        protected bool AddPlural(string key, T value, T? parent = null, bool strict = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            bool added = !strict && Add(key, value, parent);

            if (LocalizedContentManager.CurrentLanguageCode == 0)
            {
                string pluralKey = Lexicon.makePlural(key);

                if (!strict && !_data.ContainsKey(pluralKey))
                {
                    AddAlias(key, pluralKey);
                }

                added |= Add(pluralKey, value, parent);
            }

            return added;
        }

        /// <summary>Add an alias for a localized key in the data map.</summary>
        /// <param name="key">The primary localized display name (the one that appears in-game).</param>
        /// <param name="alias">The localized display name to be used as an alias for <paramref name="key"/>.</param>
        /// <returns>
        /// <c>true</c> if the <paramref name="alias"/> was added for a given <paramref name="key"/>;
        /// <c>false</c> if the <paramref name="alias"/> already exists.
        /// </returns>
        protected bool AddAlias(string key, string alias)
        {
            key = key.ToLower();

            if (!_data.ContainsKey(key))
            {
                _data.Add(key, new HashSet<T>());
            }

            return _data.TryAdd(alias.ToLower(), _data[key]);
        }

        /// <param name="aliases">
        /// A translation string for the localized display name, or comma separated names, to be used
        /// as an alias for <paramref name="key"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the any of the <paramref name="aliases"/> were added for a given <paramref name="key"/>;
        /// <c>false</c> if all of the <paramref name="aliases"/> already exist, or the <see cref="StardewModdingAPI.Translation"/>
        /// value was missing.
        /// </returns>
        /// <inheritdoc cref="AddAlias(string, string)"/>
        protected bool AddAlias(string key, Translation aliases)
        {
            if ((aliases = aliases.UsePlaceholder(false)).HasValue())
            {
                return aliases.ToString().Split(',')
                    .Select(alias => AddAlias(key, alias.Replace('_', ' ')))
                    .Aggregate(false, (anyAdded, nextAdded) => anyAdded || nextAdded);
            }

            return false;
        }

        /// <summary>Populate the data map using the various protected <c>AddX(...)</c> methods.</summary>
        protected abstract void PopulateDataMap();

        /// <summary>Apply aliases from the i18n file.</summary>
        /// <param name="i18nAliasPrefix">The prefix to test for matching alias keys.</param>
        private void ApplyTranslationAliases(string i18nAliasPrefix)
        {
            foreach (var alias in Translation.GetTranslations())
            {
                if (alias.Key.StartsWith(i18nAliasPrefix))
                {
                    AddAlias(alias.Key.Remove(0, i18nAliasPrefix.Length).Replace('_', ' '), alias);
                }
            }
        }
    }
}
