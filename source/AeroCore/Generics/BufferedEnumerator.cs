/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AeroCore.Generics
{
    public class BufferedEnumerator<T> : IEnumerator<T>
    {
        // give it a big initial capacity to improve performance
        // at the cost of some memory
        private readonly Stack<T> buffer = new(32);
        private readonly IEnumerator<T> basis;
        private T current = default;
        object IEnumerator.Current => current;
        public T Current => current;
        public int Count => buffer.Count;

        public BufferedEnumerator(IEnumerator<T> Base)
        {
            basis = Base;
        }
        public void Dispose() => basis.Dispose();
        public bool MoveNext()
        {
            if (buffer.Count > 0)
            {
                current = buffer.Pop();
                return true;
            }
            bool b = basis.MoveNext();
            current = b ? basis.Current : default;
            return b;
        }
        public void Reset()
        {
            buffer.Clear(); //no way to maintain position
            basis.Reset();
            current = basis.Current;
        }
        public IList<T> GetBuffer() => buffer.Reverse().ToArray();

        /// <summary>Add an item to the buffer. Must be added in reverse order.</summary>
        /// <param name="item"></param>
        public void Push(T item) => buffer.Push(item);
        public void Clear() => buffer.Clear();
    }
}
