using System;
using System.Collections.Generic;
using TehPers.CoreMod.Api.Conflux.Extensions;

namespace TehPers.CoreMod.Api.Conflux.Collections {
    public static class CollectionExtensions {
        public static Slice<T> Slice<T>(this IList<T> source, Index start, Index end) => source.Slice(new Range(start, end));
        public static Slice<T> Slice<T>(this IList<T> source, Range range) {
            return ListX<T>.FromLinked(source)[range];
        }

        public static ReadonlySlice<char> Slice(this string source, Index start, Index end) => source.Slice(new Range(start, end));
        public static ReadonlySlice<char> Slice(this string source, Range range) {
            return new ListX<char>(source).Slice(range);
        }

        public static string Substring(this string source, Range range) {
            int start = range.Start.Resolve(source.Length);
            return source.Substring(start, range.End.Resolve(source.Length) - start);
        }
    }
}
