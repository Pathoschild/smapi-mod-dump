using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.Serialization
{
    /// <summary>
    /// A class that handles saving/loading custom objects to and from the world.
    /// </summary>
   public class SerializerDataNode
    {
        /// <summary>
        /// A function that handles loading an object back into it's Item form.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public delegate Item ParsingFunction(string data);
        /// <summary>
        /// A function that handles saving an item.
        /// </summary>
        /// <param name="item"></param>
        public delegate void SerializingFunction(Item item);

        /// <summary>
        /// A function that handles saving an item from/to a container.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="s"></param>
        public delegate void SerializingToContainerFunction(Item item, string s);

        /// <summary>
        /// A function that handles saving.loading items into the world.
        /// </summary>
        /// <param name="obj"></param>
        public delegate void WorldParsingFunction(Item obj);

        /// <summary>
        /// Saves an object to an inventory.
        /// </summary>
        public SerializingFunction serialize;
        /// <summary>
        /// Saves an object to a container
        /// </summary>
        public SerializingToContainerFunction serializeToContainer;
        /// <summary>
        /// Loads in an object.
        /// </summary>
        public ParsingFunction parse;
        /// <summary>
        /// Loads in an object to the game world.
        /// </summary>
        public WorldParsingFunction worldObj;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serializeFunction">A function to be ran to save this object.</param>
        /// <param name="parsingFunction">A function to be ran to load this object.</param>
        /// <param name="worldObjectParsingFunction">A function to be ran to load this object to the world.</param>
        /// <param name="containerSerializationFunction">A function to be ran to save/load objects to storage containers such as chests.</param>
        public SerializerDataNode(SerializingFunction serializeFunction, ParsingFunction parsingFunction, WorldParsingFunction worldObjectParsingFunction, SerializingToContainerFunction containerSerializationFunction)
        {
            serialize = serializeFunction;
            parse = parsingFunction;
            worldObj = worldObjectParsingFunction;
            serializeToContainer = containerSerializationFunction;
        }
    }
}
