using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace TehPers.Stardew.Framework {

    internal static class Helpers {
        public static T CopyFields<T>(T original, T target) {
            Type typ = original.GetType();
            while (typ != null) {
                FieldInfo[] fields = typ.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                    field.SetValue(target, field.GetValue(original));
                typ = typ.BaseType;
            }

            return target;
        }

        public static void Shuffle<T>(this IList<T> list) => list.Shuffle(new Random());

        public static void Shuffle<T>(this IList<T> list, Random rand) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) => new HashSet<T>(source);

        public static string LocalizePath(string baseDir, string otherDir) {
            Uri baseUri = new Uri(baseDir);
            Uri otherUri = new Uri(otherDir);
            return WebUtility.UrlDecode(baseUri.MakeRelativeUri(otherUri).ToString());
        }

        public static Season? ToSeason(string s) {
            switch (s.ToLower()) {
                case "spring":
                    return Season.SPRING;
                case "summer":
                    return Season.SUMMER;
                case "fall":
                    return Season.FALL;
                case "winter":
                    return Season.WINTER;
                default:
                    return null;
            }
        }

        public static Weather ToWeather(bool raining) => raining ? Weather.RAINY : Weather.SUNNY;

        public static WaterType? ConvertWaterType(int type) {
            switch (type) {
                case -1:
                    return WaterType.BOTH;
                case 0:
                    return WaterType.RIVER;
                case 1:
                    return WaterType.LAKE;
                default:
                    return null;
            }
        }

        public static int ConvertWaterType(WaterType type) => type == WaterType.BOTH ? -1 : (type == WaterType.RIVER ? 0 : 1);

        public static T As<T>(this object o, T fallback = default(T)) => o is T t ? t : fallback;

        public static TVal GetDefault<TKey, TVal>(this Dictionary<TKey, TVal> dict, TKey key, TVal fallback = default(TVal)) => dict.ContainsKey(key) ? dict[key] : fallback;

        public static T Choose<T>(this IEnumerable<KeyValuePair<T, double>> elements) => elements.Choose(new Random());
        public static T Choose<T>(this IEnumerable<KeyValuePair<T, double>> elements, Random rand) {
            WeightedAuto<T> choice = elements.Select(kv => new WeightedAuto<T>(kv.Key, kv.Value)).Choose(rand);
            return choice == null ? default(T) : choice.Element;
        }

        public static T Choose<T>(this IEnumerable<T> entries) where T : IWeighted => entries.Choose(new Random());
        public static T Choose<T>(this IEnumerable<T> entries, Random rand) where T : IWeighted {
            entries = entries.ToList();
            double totalWeight = entries.Sum(entry => entry.GetWeight());
            double n = rand.NextDouble();
            foreach (T entry in entries) {
                double chance = entry.GetWeight() / totalWeight;
                if (n < chance) return entry;
                else n -= chance;
            }
            throw new ArgumentException("Enumerable must contain entries", nameof(entries));
        }

        /// <summary>Defines a weighted chance for an object, allowing easy weighted choosing of a random element from a list of the object.</summary>
        public interface IWeighted {
            /// <summary>Returns the weighted chance of the object, in comparison to the other objects in the list.</summary>
            double GetWeight();
        }

        private class WeightedAuto<T> : IWeighted {
            public readonly T Element;
            private readonly double _weight;

            public WeightedAuto(T elem, double weight) {
                this.Element = elem;
                this._weight = weight;
            }

            public double GetWeight() {
                return this._weight;
            }
        }

        public static string GetLanguageCode() {
            if (Game1.content != null && Game1.content.LanguageCodeOverride != null)
                return Game1.content.LanguageCodeOverride;
            switch (LocalizedContentManager.CurrentLanguageCode) {
                case LocalizedContentManager.LanguageCode.ja:
                    return "ja-JP";
                case LocalizedContentManager.LanguageCode.ru:
                    return "ru-RU";
                case LocalizedContentManager.LanguageCode.zh:
                    return "zh-CN";
                case LocalizedContentManager.LanguageCode.pt:
                    return "pt-BR";
                case LocalizedContentManager.LanguageCode.es:
                    return "es-ES";
                case LocalizedContentManager.LanguageCode.de:
                    return "de-DE";
                case LocalizedContentManager.LanguageCode.th:
                    return "th-TH";
            }
            return "";
        }
    }
}
