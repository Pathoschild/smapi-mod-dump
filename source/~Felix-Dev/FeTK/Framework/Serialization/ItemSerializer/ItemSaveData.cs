using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Serialization
{
    /// <summary>
    /// Encapsulates serialized data about an instance of the <see cref="Item"/> class.
    /// </summary>
    internal class ItemSaveData
    {
        /// <summary>
        /// Create a new instance of the <see cref="ItemSaveData"/> class.
        /// </summary>
        /// <param name="typeOrigin">The origin of the type of the item.</param>
        /// <param name="itemData">A serializable representation of an instance of the <see cref="Item"/> class.</param>
        /// <exception cref="ArgumentNullException">The specified <paramref name="itemData"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="typeOrigin"/> is not valid.</exception>
        public ItemSaveData(ItemTypeOrigin typeOrigin, object itemData)
        {
            if (!Enum.IsDefined(typeof(ItemTypeOrigin), typeOrigin))
            {
                throw new ArgumentOutOfRangeException(nameof(typeOrigin));
            }

            ItemTypeOrigin = typeOrigin;
            ItemData = itemData ?? throw new ArgumentNullException(nameof(itemData));
        }

        /// <summary>
        /// Specifies the origin of the type of the item.
        /// </summary>
        public ItemTypeOrigin ItemTypeOrigin { get; }

        /// <summary>
        /// A serializable representation of an instance of the <see cref="Item"/> class.
        /// </summary>
        public object ItemData { get; }
    }
}
