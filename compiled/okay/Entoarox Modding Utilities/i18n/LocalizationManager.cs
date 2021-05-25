using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

using StardewValley;
using LanguageCode = StardewValley.LocalizedContentManager.LanguageCode;

using StardewModdingAPI;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Entoarox.Utilities.I18n
{
    public class LocalizationManager
    {
        private const LanguageCode Default = LanguageCode.en;
        private static Func<string, string, string, Translation> GetTranslation;

        static LocalizationManager()
        {
            var ctor = typeof(Translation).GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string), typeof(string), typeof(string) }, null);
            if(ctor != null)
            {
                try
                {
                    var param0 = Expression.Parameter(typeof(string));
                    var param1 = Expression.Parameter(typeof(string));
                    var param2 = Expression.Parameter(typeof(string));
                    GetTranslation = Expression.Lambda<Func<string, string, string, Translation>>(Expression.New(ctor, param0, param1, param2), param0, param1, param2).Compile();
                    GetTranslation("", "", "");
                }
                catch(Exception e)
                {
                    Internals.EntoUtilsMod.Instance.Monitor.Log("Failed to hook the constructor of the Translation class: " + e, LogLevel.Error);
                }
            }
        }

        private readonly Random Rand = new Random();
        private readonly Dictionary<LanguageCode, Dictionary<string, JToken>> Cache = new Dictionary<LocalizedContentManager.LanguageCode, Dictionary<string, JToken>>();
        private readonly string RootPath;
        public LocalizationManager(string rootDir) : this(new DirectoryInfo(rootDir)) { }
        public LocalizationManager(DirectoryInfo rootDir)
        {
            if(GetTranslation == null)
            {
                throw new MissingMemberException("The internal format of SMAPI's Translation class has changed, a EMU update is required to make the LocalizationManager functional again.");
            }
            if (!rootDir.Exists)
            {
                throw new DirectoryNotFoundException("Root directory not found.");
            }
            if (!Directory.Exists(Path.Combine(rootDir.FullName, "en")))
            {
                throw new DirectoryNotFoundException("No default (english) localization directory found.");
            }
            this.RootPath = rootDir.FullName;
            this.LoadLanguage(Default);
            LocalizedContentManager.OnLanguageChange += this.LoadLanguage;

            var code = LocalizedContentManager.CurrentLanguageCode;

            if (code != Default)
            {
                this.LoadLanguage(code);
            }
        }

        private void LoadLanguage(LanguageCode language)
        {
            if (!this.Cache.ContainsKey(language))
            {
                var dir = new DirectoryInfo(Path.Combine(this.RootPath, language.ToString()));

                if (dir.Exists)
                {
                    var dict = new Dictionary<string, JToken>();
                    this.Cache.Add(language, dict);
                    foreach (var file in dir.EnumerateFiles("*.json"))
                    {
                        using (var stream = file.OpenRead())
                        using (var sreader = new StreamReader(stream))
                        using (var jreader = new JsonTextReader(sreader))
                        {
                            try
                            {
                                dict.Add(file.Name, JToken.ReadFrom(jreader));
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        private string InternalGet(string key, LanguageCode code)
        {
            string[] parts = key.Split('#');

            if (parts.Length == 2)
            {
                if (this.Cache.TryGetValue(code, out var data) && data.TryGetValue(parts[0], out var token))
                {
                    token = token.SelectToken(parts[1]);
                    if (token != null)
                    {
                        var type = token.Type;

                        if (type == JTokenType.String)
                        {
                            return token.ToObject<string>();
                        }
                        if (type == JTokenType.Array)
                        {
                            try
                            {
                                string[] options = token.ToObject<string[]>();
                                return options[this.Rand.Next(options.Length - 1)];
                            }
                            catch { }
                        }
                        return "(invalid key: " + key + ')';
                    }
                }
                return null;
            }
            return "(invalid key: " + key + ')';
        }

        private string[] InternalGetAll(string key, LanguageCode code)
        {
            string[] parts = key.Split('#');

            if (parts.Length == 2)
            {
                if (this.Cache.TryGetValue(code, out var data) && data.TryGetValue(parts[0], out var token))
                {
                    token = token.SelectToken(parts[1]);
                    if (token != null)
                    {
                        var type = token.Type;

                        if (type == JTokenType.String)
                        {
                            return new[]{ token.ToObject<string>() };
                        }
                        if (type == JTokenType.Array)
                        {
                            try
                            {
                                return token.ToObject<string[]>();
                            }
                            catch { }
                        }
                        return new[] { "(invalid key: " + key.Replace('#', '/') + ')' };
                    }
                }
                return null;
            }
            return new[] { "(invalid key: " + key.Replace('#', '/') + ')' };
        }

        public Translation Get(string key)
        {
            var code = LocalizedContentManager.CurrentLanguageCode;
            return GetTranslation(code.ToString(), key, this.InternalGet(key, code) ?? (code == Default ? null : this.InternalGet(key, Default)));
        }

        public Translation[] GetAll(string key)
        {
            var code = LocalizedContentManager.CurrentLanguageCode;
            return (this.InternalGetAll(key, code) ?? (code == Default ? new string[0] : this.InternalGetAll(key, Default)) ?? new string[0]).Select(_ => GetTranslation(code.ToString(), key, _)).ToArray();
        }
    }
}