
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MapPings.Framework.Network {

	public static class DataSerialization {

		public static byte[] Serialize<T>(T serializableData) where T : new() {
			if(!typeof(T).IsSerializable && !(typeof(ISerializable).IsAssignableFrom(typeof(T)))) {
				throw new InvalidOperationException("A serializable Type is required");
			} else {
				using(MemoryStream memoryStream = new MemoryStream()) {
					using(GZipStream compressionStream = new GZipStream(memoryStream, CompressionMode.Compress)) {
						BinaryFormatter formatter = new BinaryFormatter();
						formatter.Serialize(compressionStream, serializableData);
					}
					return memoryStream.ToArray();
				}
			}
		}

		public static T Deserialize<T>(Stream stream) where T : new() {
			if(!typeof(T).IsSerializable && !(typeof(ISerializable).IsAssignableFrom(typeof(T)))) {
				throw new InvalidOperationException("A serializable Type is required");
			} else {
				using(GZipStream decompressionStream = new GZipStream(stream, CompressionMode.Decompress)) {
					BinaryFormatter formatter = new BinaryFormatter();
					return (T)formatter.Deserialize(decompressionStream);
				}
			}

		}

	}

}
