/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/


using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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
