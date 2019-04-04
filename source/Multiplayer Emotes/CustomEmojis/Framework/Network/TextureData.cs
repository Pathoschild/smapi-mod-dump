
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.IO;
using System;
using CustomEmojis.Framework.Utilities;

namespace CustomEmojis.Framework.Network {

	[Serializable]
	public class TextureData : IEquatable<TextureData> {

		public int Width { get; set; }
		public int Height { get; set; }
		public string Hash { get; private set; }
		public Color[] Data { get; set; }

		public TextureData() {
		}

		public TextureData(Texture2D texture) {
			InitData(texture);
		}

		public TextureData(Stream stream) {
			InitData(stream);
		}

		public void InitData(Stream stream) {
			InitData(Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream));
		}

		public void InitData(Texture2D texture) {

			Width = texture.Width;
			Height = texture.Height;

			Data = new Color[Width * Height];
			texture.GetData(Data);

			using(MemoryStream stream = new MemoryStream()) {
				texture.SaveAsPng(stream, Width, Height);
				stream.Seek(0, SeekOrigin.Begin);
				Hash = ModUtilities.GetHash(stream);
			}

		}

		//public void InitData(BinaryReader reader) {
		//	InitData(reader, reader.ReadInt32(), reader.ReadInt32());
		//}

		//public void InitData(BinaryReader reader, int width, int height) {

		//	Width = width;
		//	Height = height;

		//	Data = new Color[Width * Height];

		//	for(int i = 0; i < Data.Length; i++) {
		//		int r = reader.ReadByte();
		//		int g = reader.ReadByte();
		//		int b = reader.ReadByte();
		//		int a = reader.ReadByte();
		//		Data[i] = new Color(r, g, b, a);
		//	}

		//	//Texture2D texture = new Texture2D(Game1.graphics.GraphicsDevice, Width, Height);
		//	//texture.SetData(Data);

		//}

		public Texture2D GetTexture() {
			Texture2D texture = new Texture2D(Game1.graphics.GraphicsDevice, Width, Height);
			texture.SetData(Data);
			return texture;
		}

		public static bool operator ==(TextureData a, TextureData b) {

			// If both are null, or both are same instance, return true.
			if(ReferenceEquals(a, b)) {
				return true;
			}

			// If one is null, but not both, return false.
			if(((object)a == null) || ((object)b == null)) {
				return false;
			}

			// Return true if the fields match
			return (a.Width == b.Width) && (a.Height == b.Height) && (a.Hash == b.Hash);
		}

		public static bool operator !=(TextureData a, TextureData b) {
			return !(a == b);
		}

		public override bool Equals(object obj) {
			return Equals(obj as TextureData);
		}

		public bool Equals(TextureData other) {

			// If parameter is null, return false.
			if(other is null) {
				return false;
			}

			// Optimization for a common success case.
			if(ReferenceEquals(this, other)) {
				return true;
			}

			// If run-time types are not exactly the same, return false.
			if(GetType() != other.GetType()) {
				return false;
			}

			// Return true if the fields match.
			return (Width == other.Width) && (Height == other.Height) && (Hash == other.Hash);
		}

		public override int GetHashCode() {

			unchecked {

				int hash = 17;

				hash = hash * 23 + GetHashCode(Width);
				hash = hash * 23 + GetHashCode(Height);
				hash = hash * 23 + GetHashCode(Hash);

				return hash;
			}

		}

		private int GetHashCode<T>(T t) {
			return t == null ? 0 : t.GetHashCode();
		}

		public override string ToString() {
			return String.Format("{0}({1})[Width: {2}, Height: {3}, Data: {{{4}}}]", nameof(TextureData), Hash, Width, Height, string.Join(", ", Data));
		}

	}

}
