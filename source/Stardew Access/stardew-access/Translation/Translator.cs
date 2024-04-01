/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using Shockah.ProjectFluent;
using StardewModdingAPI;
using System.Text.RegularExpressions;

namespace stardew_access.Translation
{
    public enum TranslationCategory
    {
        Default,
        Menu,
        MiniGames,
        CharacterCreationMenu,
        StaticTiles,
        CustomCommands,
    }

    internal class Translator
    {
        private static Translator? instance;
        private IFluent<string>? DefaultEntries { get; set; }
        private IFluent<string>? MenuEntries { get; set; }
        private IFluent<string>? MiniGamesEntries { get; set; }
        private IFluent<string>? CharacterCreationMenuEntries { get; set; }
        private IFluent<string>? StaticTilesEntries { get; set; }
        private IFluent<string>? CustomCommandsEntries { get; set; }
        private static readonly object InstanceLock = new();

        private Translator()
        {
        }

        internal CustomFluentFunctions? CustomFunctions;

        public static Translator Instance
        {
            get
            {
                lock (InstanceLock)
                {
                    instance ??= new Translator();
                    return instance;
                }
            }
        }

        public void Initialize(IManifest modManifest)
        {
                #if DEBUG
            Log.Debug("Initializing FluentApi");
            #endif
            IFluentApi? fluentApi = MainClass.ModHelper?.ModRegistry.GetApi<IFluentApi>("Shockah.ProjectFluent");
            if (fluentApi != null)
            {
                DefaultEntries = fluentApi.GetLocalizations(fluentApi.CurrentLocale, modManifest);
                MenuEntries = fluentApi.GetLocalizations(fluentApi.CurrentLocale, modManifest, "menu");
                MiniGamesEntries = fluentApi.GetLocalizations(fluentApi.CurrentLocale, modManifest, "mini_games");
                CharacterCreationMenuEntries = fluentApi.GetLocalizations(fluentApi.CurrentLocale, modManifest, "character_creation_menu");
                StaticTilesEntries = fluentApi.GetLocalizations(fluentApi.CurrentLocale, modManifest, "static_tiles");
                CustomCommandsEntries = fluentApi.GetLocalizations(fluentApi.CurrentLocale, modManifest, "commands");
                
                #if DEBUG
                Log.Verbose("Registering custom fluent functions");
                #endif
                CustomFunctions = new CustomFluentFunctions(modManifest, fluentApi);
                foreach (var (mod, name, function) in CustomFunctions.GetAll())
                {
                    fluentApi.RegisterFunction(mod, name, function);
                    #if DEBUG
                    Log.Verbose($"Registered function \"{name}\"");
                #endif
                }
            }
            else
            {
                Log.Error("Unable to initialize fluent api", true);
            }
        }

        public string Translate(string translationKey,
            TranslationCategory translationCategory = TranslationCategory.Default, bool disableWarning = false)
        {
            IFluent<string>? requiredEntries = GetEntriesFromCategory(translationCategory);

            if (requiredEntries == null)
            {
                Log.Error("Fluent not initialized!", true);
                return translationKey;
            }

            var match = Regex.Match(translationKey, @"(.*?)\[(.*?)\]$");
            string key = translationKey;
            string? extra = null;
            if (match.Success)
            {
                key = match.Groups[1].Value;
                extra = match.Groups[2].Value;
            }
            if (requiredEntries.ContainsKey(key))
            {
                #if DEBUG
                Log.Verbose($"Translate: found translation key \"{key}\"", true);
                #endif
                return $"{requiredEntries.Get(key)}{(extra != null ? $" {extra}" : "")}";
            }

            if (!disableWarning)
            {
                Log.Debug($"No translation available for key in {translationCategory} category: {key}", true);
            }

            // Unmodified; will still have brackets suffix 
            return translationKey;
        }

        public string Translate(string translationKey, object? tokens,
            TranslationCategory translationCategory = TranslationCategory.Default, bool disableWarning = false)
        {
            IFluent<string>? requiredEntries = GetEntriesFromCategory(translationCategory);

            if (requiredEntries == null)
            {
                Log.Error("Fluent not initialized!", true);
                return translationKey;
            }

            var match = Regex.Match(translationKey, @"(.*?)\[(.*?)\]$");
            string key = translationKey;
            string? extra = null;
            if (match.Success)
            {
                key = match.Groups[1].Value;
                extra = match.Groups[2].Value;
            }
            if (requiredEntries.ContainsKey(key))
            {
                #if DEBUG
                if (tokens is Dictionary<string, object> dictTokens)
                {
                    Log.Verbose(
                        $"Translate with tokens: found translation key \"{key}\" with tokens: {string.Join(", ", dictTokens.Select(kv => $"{kv.Key}: {kv.Value}"))}",
                        true);
                }
                else
                {
                    var tokenStr = tokens is not null
                        ? string.Join(", ",
                            tokens.GetType().GetProperties().Select(prop => $"{prop.Name}: {prop.GetValue(tokens)}"))
                        : "null";
                    Log.Verbose(
                        $"Translate with tokens: found translation key \"{key}\" with tokens: {tokenStr}",
                        true);
                }
                #endif
                var result = tokens is null ? requiredEntries.Get(key) : requiredEntries.Get(key, tokens);
                #if DEBUG
                Log.Verbose($"Translated to: {result}", true);
                #endif
                return $"{result}{(extra != null ? $" {extra}" : "")}";
            }

            if (!disableWarning)
            {
                Log.Debug($"No translation available for key in {translationCategory} category: {key}", true);
            }

            // Unmodified; will still have brackets suffix 
            return translationKey;
        }

        /// <summary>
        /// Checks whether the translation is available for the given translation key.
        /// </summary>
        /// <param name="translationKey">The key to check for availability.</param>
        /// <param name="translationCategory">The key's category or the translation sub-file to check for.</param>
        /// <returns>true if the key is available otherwise false.</returns>
        public bool IsAvailable(string translationKey, TranslationCategory translationCategory = TranslationCategory.Default)
        {
            IFluent<string>? requiredEntries = GetEntriesFromCategory(translationCategory);

            if (requiredEntries == null)
            {
                Log.Error("Fluent not initialized!", true);
                return false;
            }

            return requiredEntries.ContainsKey(translationKey);
        }

        private IFluent<string>? GetEntriesFromCategory(TranslationCategory translationCategory) =>
            translationCategory switch
            {
                TranslationCategory.Default => DefaultEntries,
                TranslationCategory.Menu => MenuEntries,
                TranslationCategory.MiniGames => MiniGamesEntries,
                TranslationCategory.CharacterCreationMenu => CharacterCreationMenuEntries,
                TranslationCategory.StaticTiles => StaticTilesEntries,
                TranslationCategory.CustomCommands => CustomCommandsEntries,
                _ => null
            };
    }
}
