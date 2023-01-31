/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;

namespace CoreBoy.gpu
{
    public class IntQueue
    {
        private Queue<int> _inner;
        public IntQueue(int capacity) => _inner = new Queue<int>(capacity);
        public int Size() => _inner.Count;
        public void Enqueue(int value) => _inner.Enqueue(value);
        public int Dequeue() => _inner.Dequeue();
        public int Get(int index) => _inner.ToArray()[index];
        public void Clear() => _inner.Clear();

        public void Set(int index, int value)
        {
            lock (_inner)
            {
                var asArray = _inner.ToArray();
                asArray[index] = value;
                _inner = new Queue<int>(asArray);
            }
        }
    }
}