/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;

namespace OrnithologistsGuild
{
    public class Utilities
    {
        public static T WeightedRandom<T>(IEnumerable<T> values, Func<T, float> getWeight)
        {
            List<float> weights = values.Select(v => getWeight(v)).ToList();

            float random = Utility.RandomFloat(0, weights.Sum());
            float sum = 0;

            for (int i = 0; i < values.Count(); i++)
            {
                var weight = weights.ElementAt(i);
                if (weight == 0) continue;

                if (random < (sum + weight))
                {
                    return values.ElementAt(i);
                }
                else
                {
                    sum += weight;
                }
            }

            return default;
        }

        public static IEnumerable<T> Randomize<T>(IEnumerable<T> source)
        {
            return source.OrderBy<T, double>((item) => Game1.random.NextDouble());
        }

        public static float EaseOutSine(float x)
        {
            return MathF.Sin((x * MathF.PI) / 2);
        }

        public static string GetLocaleSeparator()
        {
            try
            {
                var locale = ModEntry.Instance.Helper.Translation.Locale;
                return $"{System.Globalization.CultureInfo.GetCultureInfo(locale).TextInfo.ListSeparator} ";
            } catch
            {
                return ", ";
            }
        }

        public static string LocaleToUpper(string value)
        {
            var locale = ModEntry.Instance.Helper.Translation.Locale;
            return System.Globalization.CultureInfo.GetCultureInfo(locale).TextInfo.ToUpper(value);
        }

        public static Vector2 XY(Vector3 value)
        {
            return new Vector2(value.X, value.Y);
        }
    }
}

