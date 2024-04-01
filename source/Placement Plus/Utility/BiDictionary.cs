/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/2Retr0/PlacementPlus
**
*************************************************/

using System.Collections;
using System.Collections.Generic;

namespace PlacementPlus.Utility
{
    /// <summary> Helper data structure representing a bi-directional mapping between two of the same type. </summary>
    /// <remarks>Adapted from Dave Zych: https://stackoverflow.com/a/32658403 </remarks>
    public class BiDictionary<T> : IEnumerable
    {
        private readonly Dictionary<T, T> leftDict = new(), rightDict = new();

        /// <summary> Adds the specified key and value to the dictionary. </summary>
        public void Add(T leftItem, T rightItem)
        {
            leftDict.Add(leftItem, rightItem); 
            rightDict.Add(rightItem, leftItem);
        }

        
        
        /// <summary> Determines whether the BiDictionary contains the specified key. </summary>
        public bool ContainsKey(T item) => leftDict.ContainsKey(item) || rightDict.ContainsKey(item);
        
        
        
        /// <summary> Gets the value associated with the specified key. </summary>
        public T this[T value]
        {
            get
            {
                if (leftDict.ContainsKey(value)) return leftDict[value];
                if (rightDict.ContainsKey(value)) return rightDict[value];
                    
                throw new KeyNotFoundException(null, null);
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator() => leftDict.GetEnumerator();
    }
}