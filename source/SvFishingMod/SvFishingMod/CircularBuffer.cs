using System;

namespace SvFishingMod
{
    /*  Circular Buffer Generic Implementation
     *  By KDERazorback (http://twitter.com/kderazorback)
     *  
     *   Fast and Lightweight, Generic Circular Buffer Implementation, supports for bidirectional rotations and Resize operations
     *   Free to use or modify! Just keep me on the comments!
     */


    /// <summary>
    /// Used to store a Circular Buffer of objects with a particular size, that rotates when an item is added to the collection.
    /// </summary>
    public class CircularBuffer<T>
    {
        /// <summary>
        /// Creates a new Circular Buffer with the specified Capacity
        /// </summary>
        /// <param name="capacity">Total elements that can be stored inside the buffer before it starts discarding items</param>
        public CircularBuffer(int capacity)
        {
            _plainBuffer = new T[capacity];
            _startIndex = 0;
        }

        private T[] _plainBuffer;
        private int _startIndex; // Stores the start of the Circular Buffer

        /// <summary>
        /// Stores the current Capacity of this Buffer
        /// </summary>
        public int Capacity
        {
            get { return _plainBuffer.Length; }
        }

        /// <summary>
        /// Returns the item that is stored on the specified Index inside the Circular Buffer
        /// </summary>
        /// <param name="index">Index of the Item to be returned</param>
        /// <returns>The object value stored on the specified index</returns>
        public T ElementAt(int index)
        {
            if ((index >= _plainBuffer.Length) || (index < 0))
                throw new IndexOutOfRangeException();

            index += _startIndex;

            if (index >= _plainBuffer.Length)
                index -= _plainBuffer.Length;

            return _plainBuffer[index];
        }

        public void UpdateElementAt(int index, T value)
        {
            if ((index >= _plainBuffer.Length) || (index < 0))
                throw new IndexOutOfRangeException();

            index += _startIndex;

            if (index >= _plainBuffer.Length)
                index -= _plainBuffer.Length;

            _plainBuffer[index] = value;
        }

        public T this[int index]
        {
            get
            {
                return ElementAt(index);
            }
            set
            {
                UpdateElementAt(index, value);
            }
        }

        /// <summary>
        /// Returns an array with the full content of the actual Circular Buffer
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            int i;
            T[] output = new T[_plainBuffer.Length];

            for (i = 0; i < _plainBuffer.Length; i++)
            {
                output[i] = ElementAt(i);
            }

            return output;
        }

        /// <summary>
        /// Inserts a new item inside the Circular Buffer and rotates the entire structure by one step forwards
        /// </summary>
        /// <param name="newItem">New item to be inserted into the Circular Buffer</param>
        public void Insert(T newItem)
        {
            if (_startIndex == 0)
                _startIndex = _plainBuffer.Length - 1;
            else
                _startIndex--;

            _plainBuffer[_startIndex] = newItem;
        }

        /// <summary>
        /// Inserts a new item inside the Circular Buffer and rotates the entire structure by one step backwards
        /// </summary>
        /// <param name="newItem">New item to be inserted into the Circular Buffer</param>
        public void InsertBackwards(T newItem)
        {
            _plainBuffer[_startIndex] = newItem;

            if (_startIndex == _plainBuffer.Length - 1)
                _startIndex = 0;
            else
                _startIndex++;
        }

        /// <summary>
        /// Resizes this Circular Buffer to a new Capacity using Forward resizes, removing or inserting new items as needed
        /// </summary>
        /// <param name="newCapacity">New Capacity for this Circular Buffer</param>
        public void Resize(int newCapacity)
        {
            T[] bufferData = ToArray();

            _plainBuffer = new T[newCapacity];
            _startIndex = 0;

            int i;
            for (i = 0; i < Math.Min(_plainBuffer.Length, bufferData.Length); i++)
            {
                InsertBackwards(bufferData[i]); // Use Backward rotations for Forward Resizes
            }

            _startIndex = 0;
        }

        /// <summary>
        /// Resizes this Circular Buffer to a new Capacity using Backward resizes, removing or inserting new items as needed
        /// </summary>
        /// <param name="newCapacity">New Capacity for this Circular Buffer</param>
        public void ResizeBackwards(int newCapacity)
        {
            T[] bufferData = ToArray();

            _plainBuffer = new T[newCapacity];
            _startIndex = newCapacity - 1;

            int i;
            for (i = 0; i < Math.Min(_plainBuffer.Length, bufferData.Length); i++)
            {
                Insert(bufferData[i]); // Use Forward rotations for Backwards Resizes
            }
        }

        /// <summary>
        /// Rotates the entire Circular Buffer by a defined amount in any direction. A negative value indicates a backwards rotation.
        /// </summary>
        /// <param name="amount">Amount of position the Circular Buffer will be rotated</param>
        public void Rotate(int amount)
        {
            _startIndex += amount;
            while (_startIndex < 0)
                _startIndex += _plainBuffer.Length;

            while (_startIndex >= _plainBuffer.Length)
                _startIndex -= _plainBuffer.Length;
        }
    }
}
