/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.IO;
using Netcode;
using Newtonsoft.Json;

namespace TehPers.Core.Json
{
    internal class NetConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is not AbstractNetSerializable serializable)
            {
                throw new ArgumentException(
                    "Value must be a serializable net object.",
                    nameof(serializable)
                );
            }

            // Write the object to a stream and store it in an exposer object
            using var stream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(stream);
            serializable.WriteFull(binaryWriter);
            var exposer = new NetExposer(serializable.GetType(), stream.ToArray());

            serializer.Serialize(writer, exposer);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer
        )
        {
            throw new NotSupportedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(AbstractNetSerializable).IsAssignableFrom(objectType);
        }

        public override bool CanRead => false;
    }
}