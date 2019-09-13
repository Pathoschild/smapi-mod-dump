using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Serialization
{
    /// <summary>
    /// Provides an API to (de-)serialize instances of the <see cref="Item"/> class.
    /// </summary>
    internal interface IItemSerializer
    {
        /// <summary>
        /// Construct a matching <see cref="Item"/> instance from the provided data.
        /// </summary>
        /// <param name="itemData">The data to reconstruct into a <see cref="Item"/> instance.</param>
        /// <returns>A <see cref="Item"/> instance matching the data given by <paramref name="itemData"/>.</returns>
        /// <exception cref="ArgumentNullException">The given <paramref name="itemData"/> is <c>null</c>.</exception>
        /// <exception cref="NotImplementedException">The given <paramref name="itemData"/> does not represent a supported <see cref="Item"/> instance.</exception>
        /// <exception cref="InvalidOperationException">An error occured during deserialization.</exception>
        Item Construct(ItemSaveData itemData);

        /// <summary>
        /// Deconstruct a <see cref="Item"/> instance into a format which can be serialized.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> instance to deconstruct.</param>
        /// <returns>A serializable representation of the <see cref="Item"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The specified <paramref name="item"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">An error occured during deserialization.</exception>
        ItemSaveData Deconstruct(Item item);
    }
}
