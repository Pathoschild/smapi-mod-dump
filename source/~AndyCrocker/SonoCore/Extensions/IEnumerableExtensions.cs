/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace SonoCore.Extensions
{
    /// <summary>Extension methods for the <see cref="IEnumerable{T}"/> interface.</summary>
    public static class IEnumerableExtensions
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Returns a new collection in which all occurrances of a specified collection in the current instance are replaced with another specified collection.</summary>
        /// <param name="collection">The collection of which the replace operation will be performed on.</param>
        /// <param name="oldValue">The sub-collection to be replaced.</param>
        /// <param name="newValue">The sub-collection to replace all occurrances of <paramref name="oldValue"/>.</param>
        /// <param name="numberOfReplacements">The number of occurrances of <paramref name="oldValue"/> that were replaced.</param>
        /// <returns>A collection that is equivalent to the current collection except that all instances of <paramref name="oldValue"/> are replaced with <paramref name="newValue"/>. If <paramref name="oldValue"/> is not found in the current instance, the method returns the current instance unchanged.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="oldValue"/> or <paramref name="newValue"/> is <see langword="null"/>.</exception>
        public static IEnumerable<CodeInstruction> Replace(this IEnumerable<CodeInstruction> collection, IEnumerable<CodeInstruction> oldValue, IEnumerable<CodeInstruction> newValue, out int numberOfReplacements)
        {
            var collectionList = collection.ToList();
            var oldValueList = oldValue?.ToList() ?? throw new ArgumentNullException(nameof(oldValue));
            var newValueList = newValue?.ToList() ?? throw new ArgumentNullException(nameof(newValue));

            var oldValueIndexes = new List<int>();

            // record all occurrances of 'oldValue' in 'collection'
            for (int i = 0; i <= collectionList.Count - oldValueList.Count; i++)
                for (int j = 0; j < oldValueList.Count; j++)
                {
                    // ensure this portion (starting index of 'i') is an 'oldValue' section
                    if (collectionList[i + j].opcode != oldValueList[j].opcode)
                        break;

                    var operandValue = collectionList[i + j].operand;
                    if (operandValue is LocalBuilder localBuilder)
                        operandValue = localBuilder.LocalIndex;

                    var areOperandsEqual = operandValue == oldValueList[j].operand;
                    if (operandValue != null && oldValueList[j].operand != null)
                        areOperandsEqual = operandValue.Equals(oldValueList[j].operand);
                    if (operandValue is float c && oldValueList[j].operand is float d)
                        areOperandsEqual = FloatingPointEquals(c, d);
                    if (!areOperandsEqual)
                        break;

                    // ensure this ('j') is the end of the 'oldValue' section
                    if (j != oldValueList.Count - 1)
                        continue;

                    // mark 'i' as an 'oldValue' section (to be replaced with 'newValue')
                    oldValueIndexes.Add(i);
                }

            numberOfReplacements = oldValueIndexes.Count;

            // replace all occurrances of 'oldValue' with 'newValue'
            for (int i = oldValueIndexes.Count - 1; i >= 0; i--)
            {
                var newCollectionList = new List<CodeInstruction>();
                newCollectionList.AddRange(collectionList.Take(oldValueIndexes[i]));
                newCollectionList.AddRange(newValueList);
                newCollectionList.AddRange(collectionList.Skip(oldValueIndexes[i] + oldValueList.Count).Take(collectionList.Count - oldValueIndexes[i] - oldValueList.Count));
                collectionList = newCollectionList;
            }

            return collectionList;

            // Determines whether two floating-point numbers are equal.
            bool FloatingPointEquals(float a, float b) => Math.Abs(a - b) < .00001f;
        }
    }
}
