using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.Interfaces
{
    /// <summary>
    /// Used to extend custom serialization to a variety of items.
    /// </summary>
    public interface IItemSerializeable
    {
        /// <summary>
        /// Gets the type of object I am trying to parse.
        /// </summary>
        /// <returns></returns>
        Type getCustomType();

        /// <summary>
        /// Returns the serialization name of the object I am serializing.
        /// </summary>
        /// <returns></returns>
        string GetSerializationName();
    }
}
