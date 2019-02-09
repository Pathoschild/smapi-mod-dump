using System;
using System.Collections.Generic;
using System.Linq;
using TehPers.Core.Api.Weighted;

namespace TehPers.Core.Helpers.Static {
    public static class WeightedHelpers {
        public static T Choose<T>(this IDictionary<T, double> source) => source.Choose(new Random());
        public static T Choose<T>(this IDictionary<T, double> source, Random rand) => source.ToWeighted().Choose(rand);

        public static T Choose<T>(this IEnumerable<T> source) where T : IWeighted => source.Choose(new Random());
        public static T Choose<T>(this IEnumerable<T> source, Random rand) where T : IWeighted => source.ToList().Choose(rand);

        public static T Choose<T>(this IEnumerable<IWeightedElement<T>> source) => source.Choose(new Random());
        public static T Choose<T>(this IEnumerable<IWeightedElement<T>> source, Random rand) => ((IWeightedElement<T>) ((IEnumerable<IWeighted>) source).Choose(rand)).Value;

        public static T Choose<T>(this IList<T> source) where T : IWeighted => source.Choose(new Random());
        public static T Choose<T>(this IList<T> source, Random rand) where T : IWeighted {
            if (!source.Any())
                throw new ArgumentException("Source must contain entries", nameof(source));

            double totalWeight = source.SumWeights();
            if (Math.Abs(totalWeight) < double.Epsilon * 10)
                throw new ArgumentException("Source must have a non-zero total weight", nameof(source));

            double n = rand.NextDouble();
            foreach (T entry in source) {
                double chance = entry.GetWeight() / totalWeight;
                if (n < chance)
                    return entry;
                n -= chance;
            }

            throw new ArgumentException("Source should contain positively weighted entries", nameof(source));
        }

        public static IEnumerable<IWeightedElement<T>> ToWeighted<T>(this IDictionary<T, double> source) => source.ToWeighted(kv => kv.Value, kv => kv.Key);
        public static IEnumerable<IWeightedElement<T>> ToWeighted<T>(this IEnumerable<T> source, Func<T, double> weightSelector) => source.ToWeighted(weightSelector, e => e);
        public static IEnumerable<IWeightedElement<TEntry>> ToWeighted<TSource, TEntry>(this IEnumerable<TSource> source, Func<TSource, double> weightSelector, Func<TSource, TEntry> elementSelector) {
            return source.Select(e => new WeightedElement<TEntry>(elementSelector(e), weightSelector(e))).ToArray();
        }

        public static IEnumerable<IWeightedElement<T>> Normalize<T>(this IEnumerable<T> source) where T : IWeighted => source.NormalizeTo(1D);
        public static IEnumerable<IWeightedElement<T>> NormalizeTo<T>(this IEnumerable<T> source, double weight) where T : IWeighted => source.ToList().NormalizeTo(weight);

        public static IEnumerable<IWeightedElement<T>> Normalize<T>(this IList<T> source) where T : IWeighted => source.NormalizeTo(1D);
        public static IEnumerable<IWeightedElement<T>> NormalizeTo<T>(this IList<T> source, double weight) where T : IWeighted {
            double totalWeight = source.SumWeights();
            if (totalWeight == 0)
                totalWeight = 1;
            return source.Select(e => new WeightedElement<T>(e, weight * e.GetWeight() / totalWeight)).ToArray();
        }

        public static IEnumerable<IWeightedElement<T>> Normalize<T>(this IEnumerable<IWeightedElement<T>> source) => source.NormalizeTo(1D);
        public static IEnumerable<IWeightedElement<T>> NormalizeTo<T>(this IEnumerable<IWeightedElement<T>> source, double weight) => source.ToList().NormalizeTo(weight);

        public static IEnumerable<IWeightedElement<T>> Normalize<T>(this IList<IWeightedElement<T>> source) => source.NormalizeTo(1D);
        public static IEnumerable<IWeightedElement<T>> NormalizeTo<T>(this IList<IWeightedElement<T>> source, double weight) {
            double totalWeight = source.SumWeights();
            if (totalWeight == 0)
                totalWeight = 1;
            return source.Select(e => new WeightedElement<T>(e.Value, weight * e.GetWeight() / totalWeight)).ToArray();
        }

        public static double SumWeights<T>(this IEnumerable<T> source) where T : IWeighted => source.Sum(e => e.GetWeight());
    }
}
