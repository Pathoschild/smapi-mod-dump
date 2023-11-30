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
        public IntQueue(int capacity) => this._inner = new Queue<int>(capacity);
        public int Size() => this._inner.Count;
        public void Enqueue(int value) => this._inner.Enqueue(value);
        public int Dequeue() => this._inner.Dequeue();
        public int Get(int index) => this._inner.ToArray()[index];
        public void Clear() => this._inner.Clear();

        public void Set(int index, int value)
        {
            lock (this._inner)
            {
                var asArray = this._inner.ToArray();
                asArray[index] = value;
                this._inner = new Queue<int>(asArray);
            }
        }
    }
}