using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace FelixDev.StardewMods.FeTK.Framework.Serialization
{
    /// <summary>
    /// Provides an API to (de-)serialize instances of the <see cref="Item"/> class.
    /// </summary>
    /// <remarks>Currently only supports (de-)serializing vanilla items.</remarks>
    internal class ItemSerializer : IItemSerializer
    {
        /// <summary>The serializer to use for game-provided item types.</summary>
        private readonly XmlSerializer itemSerializer;

        /// <summary>
        /// Create a new instance of the <see cref="ItemSerializer"/> class.
        /// </summary>
        public ItemSerializer()
        {
            itemSerializer = new XmlSerializer(typeof(Item));
        }

        /// <summary>
        /// Construct a matching <see cref="Item"/> instance from the provided data.
        /// </summary>
        /// <param name="itemData">The data to reconstruct into a <see cref="Item"/> instance.</param>
        /// <returns>A <see cref="Item"/> instance matching the data given by <paramref name="itemData"/>.</returns>
        /// <exception cref="ArgumentNullException">The given <paramref name="itemData"/> is <c>null</c>.</exception>
        /// <exception cref="NotImplementedException">The given <paramref name="itemData"/> does not represent a supported <see cref="Item"/> instance.</exception>
        /// <exception cref="InvalidOperationException">An error occured during deserialization.</exception>
        public Item Construct(ItemSaveData itemData)
        {
            if (itemData == null)
            {
                throw new ArgumentNullException(nameof(itemData));
            }

            switch (itemData.ItemTypeOrigin)
            {
                case ItemTypeOrigin.Vanilla:
                    StringReader strReader = new StringReader((string)itemData.ItemData);
                    using (var reader = XmlReader.Create(strReader))
                    {
                        try
                        {
                            return (Item)itemSerializer.Deserialize(reader);
                        }
                        catch (InvalidOperationException ex)
                        {
                            string error = $"Cannot parse the given item data. The data doesn't appear to be valid XML!\nTechnical details: {ex.Message}";
                            throw new InvalidOperationException(error);
                        }                        
                    }

                default:
                    throw new NotImplementedException($"Unsupported item type \"{itemData.ItemTypeOrigin}\"");
            }
        }

        /// <summary>
        /// Deconstruct a <see cref="Item"/> instance into a format which can be serialized.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> instance to deconstruct.</param>
        /// <returns>A serializable representation of the <see cref="Item"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The specified <paramref name="item"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">An error occured during deserialization.</exception>
        public ItemSaveData Deconstruct(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            XmlWriterSettings settings = new XmlWriterSettings
            {
                ConformanceLevel = ConformanceLevel.Auto,
                CloseOutput = true
            };

            StringWriter strWriter = new StringWriter();
            using (var writer = XmlWriter.Create(strWriter, settings))
            {
                try
                {
                    itemSerializer.Serialize(writer, item);
                }
                catch (InvalidOperationException ex)
                {
                    string error = $"Cannot serialize the given item! \nTechnical details: {ex.Message}";
                    throw new InvalidOperationException(error);
                }
                
            }

            return new ItemSaveData(ItemTypeOrigin.Vanilla, strWriter.ToString());
        }
    }
}
