using System;
using System.IO;
using Netcode;
using Newtonsoft.Json;

namespace TehPers.CoreMod.Json {
    public class NetConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            NetExposer exposer;

            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter binaryWriter = new BinaryWriter(stream)) {
                // Write the object to a stream and store it in an exposer object
                ((AbstractNetSerializable) value).WriteFull(binaryWriter);
                exposer = new NetExposer(value.GetType(), stream.ToArray());
            }

            serializer.Serialize(writer, exposer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType) {
            return typeof(AbstractNetSerializable).IsAssignableFrom(objectType);
        }

        public override bool CanRead { get; } = false;
    }
}
