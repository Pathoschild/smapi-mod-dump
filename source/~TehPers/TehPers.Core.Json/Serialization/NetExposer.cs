using System;
using System.IO;
using System.Reflection;
using Netcode;
using Newtonsoft.Json;

namespace TehPers.Core.Json.Serialization {
    public sealed class NetExposer {
        private readonly MethodInfo _unwrap = typeof(NetExposer).GetMethod(nameof(NetExposer.Unwrap), BindingFlags.Public | BindingFlags.Static);

        public Type Type { get; set; }
        public byte[] Data { get; set; }

        [JsonConstructor]
        public NetExposer(Type type, byte[] data) {
            this.Type = type;
            this.Data = data;
        }

        public object Unwrap() {
            return this._unwrap.MakeGenericMethod(this.Type).Invoke(null, new object[] { this.Data });
        }

        public static T Unwrap<T>(byte[] data) where T : AbstractNetSerializable {
            T netField = Activator.CreateInstance<T>();
            using (MemoryStream stream = new MemoryStream(data)) {
                using (BinaryReader reader = new BinaryReader(stream)) {
                    netField.ReadFull(reader, new NetVersion());
                }
            }

            return netField;
        }
    }
}