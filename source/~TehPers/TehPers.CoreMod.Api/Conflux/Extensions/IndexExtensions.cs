using System;
using System.Collections.Generic;
using TehPers.CoreMod.Api.Conflux.Collections;

namespace TehPers.CoreMod.Api.Conflux.Extensions
{
    public static class IndexExtensions
    {
        public static int Resolve<T>(this Index index, IReadonlyArrayLike<T> source) => index.Resolve(source.Length);
        public static int Resolve<T>(this Index index, IList<T> source) => index.Resolve(source.Count);
        public static int Resolve(this Index index, int length)
        {
            if (index.Value >= length)
            {
                throw new IndexOutOfRangeException("Index must be within the bounds of the source.");
            }

            return index.IsFromEnd ? length - index.Value : index.Value;
        }
    }
}
