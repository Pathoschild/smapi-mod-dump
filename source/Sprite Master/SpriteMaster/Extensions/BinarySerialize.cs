using SpriteMaster.Types;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions {
	internal static class BinarySerialize {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
				case bool v:
					stream.Write(v);
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
				case Resample.TextureFormat v:
					stream.Write((int)(TeximpNet.Compression.CompressionFormat)v);
					break;
				default:
					throw new ArgumentException($"Type {typeof(T).FullName} cannot be serialized by BinaryWriter");
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static object Read (this BinaryReader stream, Type type) {
			return type switch
			{
				var _ when type == typeof(string) => stream.ReadString(),
				var _ when type == typeof(float) => stream.ReadSingle(),
				var _ when type == typeof(double) => stream.ReadDouble(),
				var _ when type == typeof(char) => stream.ReadChar(),
				var _ when type == typeof(byte) => stream.ReadByte(),
				var _ when type == typeof(sbyte) => stream.ReadSByte(),
				var _ when type == typeof(short) => stream.ReadInt16(),
				var _ when type == typeof(ushort) => stream.ReadUInt16(),
				var _ when type == typeof(int) => stream.ReadInt32(),
				var _ when type == typeof(uint) => stream.ReadUInt32(),
				var _ when type == typeof(long) => stream.ReadInt64(),
				var _ when type == typeof(ulong) => stream.ReadUInt64(),
				var _ when type == typeof(bool) => stream.ReadBoolean(),
				var _ when type == typeof(decimal) => stream.ReadDecimal(),
				var _ when type == typeof(Vector2I) => new Vector2I(stream.ReadInt64()),
				var _ when type == typeof(Vector2B) => new Vector2B(stream.ReadByte()),
				var _ when type == typeof(Resample.TextureFormat) || type == typeof(Resample.TextureFormat?) =>
					Resample.TextureFormat.Get((TeximpNet.Compression.CompressionFormat)stream.ReadInt32()),
				_ => throw new ArgumentException($"Type {type.FullName} cannot be serialized by BinaryReader"),
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static T Read<T> (this BinaryReader stream) {
			return (T)Read(stream, typeof(T));
		}
	}
}
