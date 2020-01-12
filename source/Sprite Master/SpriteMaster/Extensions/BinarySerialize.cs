using SpriteMaster.Types;
using System;
using System.IO;

namespace SpriteMaster.Extensions {
	internal static class BinarySerialize {
		internal static void Write<T> (this BinaryWriter stream, T value) {
			switch (value) {
				case string v:
					stream.Write(v);
					return;
				case float v:
					stream.Write(v);
					return;
				case double v:
					stream.Write(v);
					return;
				case char v:
					stream.Write(v);
					return;
				case byte v:
					stream.Write(v);
					return;
				case sbyte v:
					stream.Write(v);
					return;
				case short v:
					stream.Write(v);
					return;
				case ushort v:
					stream.Write(v);
					return;
				case int v:
					stream.Write(v);
					return;
				case uint v:
					stream.Write(v);
					return;
				case long v:
					stream.Write(v);
					return;
				case ulong v:
					stream.Write(v);
					return;
				case bool v:
					stream.Write(v);
					return;
				case decimal v:
					stream.Write(v);
					return;
				case Vector2I v:
					stream.Write(v.X);
					stream.Write(v.Y);
					return;
				case Vector2B v:
					stream.Write(v.X);
					stream.Write(v.Y);
					return;
				case Resample.TextureFormat v:
					stream.Write((int)(TeximpNet.Compression.CompressionFormat)v);
					return;
			}

			throw new ArgumentException($"Type {typeof(T).FullName} cannot be serialized by BinaryWriter");
		}

		internal static object Read (this BinaryReader stream, Type type) {
			switch (type) {
				case var _ when type.Equals(typeof(string)):
					return stream.ReadString();
				case var _ when type.Equals(typeof(float)):
					return stream.ReadSingle();
				case var _ when type.Equals(typeof(double)):
					return stream.ReadDouble();
				case var _ when type.Equals(typeof(char)):
					return stream.ReadChar();
				case var _ when type.Equals(typeof(byte)):
					return stream.ReadByte();
				case var _ when type.Equals(typeof(sbyte)):
					return stream.ReadSByte();
				case var _ when type.Equals(typeof(short)):
					return stream.ReadInt16();
				case var _ when type.Equals(typeof(ushort)):
					return stream.ReadUInt16();
				case var _ when type.Equals(typeof(int)):
					return stream.ReadInt32();
				case var _ when type.Equals(typeof(uint)):
					return stream.ReadUInt32();
				case var _ when type.Equals(typeof(long)):
					return stream.ReadInt64();
				case var _ when type.Equals(typeof(ulong)):
					return stream.ReadUInt64();
				case var _ when type.Equals(typeof(bool)):
					return stream.ReadBoolean();
				case var _ when type.Equals(typeof(decimal)):
					return stream.ReadDecimal();
				case var _ when type.Equals(typeof(Vector2I)):
					return new Vector2I(stream.ReadInt32(), stream.ReadInt32());
				case var _ when type.Equals(typeof(Vector2B)):
					return new Vector2B(stream.ReadBoolean(), stream.ReadBoolean());
				case var _ when type.Equals(typeof(Resample.TextureFormat)):
				case var _ when type.Equals(typeof(Resample.TextureFormat?)):
					return Resample.TextureFormat.Get((TeximpNet.Compression.CompressionFormat)stream.ReadInt32());
			}

			throw new ArgumentException($"Type {type.FullName} cannot be serialized by BinaryReader");
		}

		internal static T Read<T> (this BinaryReader stream) {
			return (T)Read(stream, typeof(T));
		}
	}
}
