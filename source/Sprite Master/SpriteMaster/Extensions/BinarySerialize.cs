/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Types;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions {
	internal static class BinarySerialize {
		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static void Write<T> (this BinaryWriter stream, T value) {
			switch (value) {
				case string v:
					stream.Write(v);
					break;
				case float v:
					stream.Write(v);
					break;
				case double v:
					stream.Write(v);
					break;
				case char v:
					stream.Write(v);
					break;
				case byte v:
					stream.Write(v);
					break;
				case sbyte v:
					stream.Write(v);
					break;
				case short v:
					stream.Write(v);
					break;
				case ushort v:
					stream.Write(v);
					break;
				case int v:
					stream.Write(v);
					break;
				case uint v:
					stream.Write(v);
					break;
				case long v:
					stream.Write(v);
					break;
				case ulong v:
					stream.Write(v);
					break;
				case VolatileULong v:
					stream.Write((ulong)v);
					break;
				case bool v:
					stream.Write(v ? (byte)1 : (byte)0); // Intentionally not writing a boolean, as booleans serialize very large.
					break;
				case decimal v:
					stream.Write(v);
					break;
				case Vector2I v:
					stream.Write(v.Packed);
					break;
				case Vector2B v:
					stream.Write(v.Packed);
					break;
				case Compression.Algorithm v:
					stream.Write((int)v);
					break;
				case Resample.TextureFormat v:
					stream.Write((int)(TeximpNet.Compression.CompressionFormat)v);
					break;
				default:
					throw new ArgumentException($"Type {typeof(T).FullName} cannot be serialized by BinaryWriter");
			}
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static Action<BinaryWriter, object> GetWriter (this Type type) {
			return type switch {
				var _ when type == typeof(string) => (stream, obj) => stream.Write((string)obj),
				var _ when type == typeof(float) => (stream, obj) => stream.Write((float)obj),
				var _ when type == typeof(double) => (stream, obj) => stream.Write((double)obj),
				var _ when type == typeof(char) => (stream, obj) => stream.Write((char)obj),
				var _ when type == typeof(byte) => (stream, obj) => stream.Write((byte)obj),
				var _ when type == typeof(sbyte) => (stream, obj) => stream.Write((sbyte)obj),
				var _ when type == typeof(short) => (stream, obj) => stream.Write((short)obj),
				var _ when type == typeof(ushort) => (stream, obj) => stream.Write((ushort)obj),
				var _ when type == typeof(int) => (stream, obj) => stream.Write((int)obj),
				var _ when type == typeof(uint) => (stream, obj) => stream.Write((uint)obj),
				var _ when type == typeof(long) => (stream, obj) => stream.Write((long)obj),
				var _ when type == typeof(ulong) => (stream, obj) => stream.Write((ulong)obj),
				var _ when type == typeof(VolatileULong) => (stream, obj) => stream.Write((ulong)(VolatileULong)obj),
				var _ when type == typeof(bool) => (stream, obj) => stream.Write((bool)obj ? (byte)1 : (byte)0), // not ReadBoolean, because bool is 4 bytes
				var _ when type == typeof(decimal) => (stream, obj) => stream.Write((decimal)obj),
				var _ when type == typeof(Vector2I) => (stream, obj) => stream.Write(((Vector2I)obj).Packed),
				var _ when type == typeof(Vector2B) => (stream, obj) => stream.Write(((Vector2B)obj).Packed),
				var _ when type == typeof(Compression.Algorithm) => (stream, obj) => stream.Write((int)(Compression.Algorithm)obj),
				var _ when type == typeof(Resample.TextureFormat) || type == typeof(Resample.TextureFormat?) =>
					(stream, obj) => stream.Write((int)(TeximpNet.Compression.CompressionFormat)(Resample.TextureFormat)obj),
				_ => throw new ArgumentException($"Type {type.FullName} cannot be serialized by BinaryReader")
			};
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static object Read (this BinaryReader stream, Type type) => GetReader(type)(stream);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static Func<BinaryReader, object> GetReader (this Type type) {
			return type switch
			{
				var _ when type == typeof(string) => (stream) => stream.ReadString(),
				var _ when type == typeof(float) => (stream) => stream.ReadSingle(),
				var _ when type == typeof(double) => (stream) => stream.ReadDouble(),
				var _ when type == typeof(char) => (stream) => stream.ReadChar(),
				var _ when type == typeof(byte) => (stream) => stream.ReadByte(),
				var _ when type == typeof(sbyte) => (stream) => stream.ReadSByte(),
				var _ when type == typeof(short) => (stream) => stream.ReadInt16(),
				var _ when type == typeof(ushort) => (stream) => stream.ReadUInt16(),
				var _ when type == typeof(int) => (stream) => stream.ReadInt32(),
				var _ when type == typeof(uint) => (stream) => stream.ReadUInt32(),
				var _ when type == typeof(long) => (stream) => stream.ReadInt64(),
				var _ when type == typeof(ulong) => (stream) => stream.ReadUInt64(),
				var _ when type == typeof(VolatileULong) => (stream) => stream.ReadUInt64(),
				var _ when type == typeof(bool) => (stream) => stream.ReadByte() != 0, // not ReadBoolean, because bool is 4 bytes
				var _ when type == typeof(decimal) => (stream) => stream.ReadDecimal(),
				var _ when type == typeof(Vector2I) => (stream) => new Vector2I(stream.ReadUInt64()),
				var _ when type == typeof(Vector2B) => (stream) => new Vector2B(stream.ReadByte()),
				var _ when type == typeof(Compression.Algorithm) => (stream) => (Compression.Algorithm)stream.ReadInt32(),
				var _ when type == typeof(Resample.TextureFormat) || type == typeof(Resample.TextureFormat?) =>
					(stream) => Resample.TextureFormat.Get((TeximpNet.Compression.CompressionFormat)stream.ReadInt32()),
				_ => throw new ArgumentException($"Type {type.FullName} cannot be serialized by BinaryReader"),
			};
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static object Read<T> (this BinaryReader stream, Type<T> type) => (T)Read(stream, type);

		internal static uint GetSerializedSize (this Type type) {
			return type switch {
				var _ when type == typeof(string) => throw new ArgumentException($"Type {type.FullName} does not have a constant serialization size"),
				var _ when type == typeof(float) => sizeof(float),
				var _ when type == typeof(double) => sizeof(double),
				var _ when type == typeof(char) => sizeof(char),
				var _ when type == typeof(byte) => sizeof(byte),
				var _ when type == typeof(sbyte) => sizeof(sbyte),
				var _ when type == typeof(short) => sizeof(short),
				var _ when type == typeof(ushort) => sizeof(ushort),
				var _ when type == typeof(int) => sizeof(int),
				var _ when type == typeof(uint) => sizeof(uint),
				var _ when type == typeof(long) => sizeof(long),
				var _ when type == typeof(ulong) => sizeof(ulong),
				var _ when type == typeof(VolatileULong) => sizeof(ulong),
				var _ when type == typeof(bool) => sizeof(byte), // not bool. Because 1 < 4.
				var _ when type == typeof(decimal) => sizeof(decimal),
				var _ when type == typeof(Vector2I) => sizeof(ulong),
				var _ when type == typeof(Vector2B) => sizeof(byte),
				var _ when type == typeof(Compression.Algorithm) => sizeof(int),
				var _ when type == typeof(Resample.TextureFormat) || type == typeof(Resample.TextureFormat?) => sizeof(int),
				_ => throw new ArgumentException($"Type {type.FullName} cannot be serialized by BinaryReader"),
			};
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static T Read<T> (this BinaryReader stream) {
			return (T)Read(stream, typeof(T));
		}
	}
}
