using System;
using Microsoft.Xna.Framework;
using StardewValley;

namespace TehPers.Core.Items {
    internal static class GuidExtensions {
        public static Guid ToGuid(this Rectangle sourceRectangle) {
            byte[] bytes = new byte[16];

            // x
            bytes[15] = (byte) ((sourceRectangle.X >> 24) % byte.MaxValue);
            bytes[14] = (byte) ((sourceRectangle.X >> 16) % byte.MaxValue);
            bytes[13] = (byte) ((sourceRectangle.X >> 8) % byte.MaxValue);
            bytes[12] = (byte) (sourceRectangle.X % byte.MaxValue);

            // y
            bytes[11] = (byte) ((sourceRectangle.Y >> 24) % byte.MaxValue);
            bytes[10] = (byte) ((sourceRectangle.Y >> 16) % byte.MaxValue);
            bytes[9] = (byte) ((sourceRectangle.Y >> 8) % byte.MaxValue);
            bytes[8] = (byte) (sourceRectangle.Y % byte.MaxValue);

            // width
            bytes[7] = (byte) ((sourceRectangle.Width >> 24) % byte.MaxValue);
            bytes[6] = (byte) ((sourceRectangle.Width >> 16) % byte.MaxValue);
            bytes[5] = (byte) ((sourceRectangle.Width >> 8) % byte.MaxValue);
            bytes[4] = (byte) (sourceRectangle.Width % byte.MaxValue);

            // height
            bytes[3] = (byte) ((sourceRectangle.Height >> 24) % byte.MaxValue);
            bytes[2] = (byte) ((sourceRectangle.Height >> 16) % byte.MaxValue);
            bytes[1] = (byte) ((sourceRectangle.Height >> 8) % byte.MaxValue);
            bytes[0] = (byte) (sourceRectangle.Height % byte.MaxValue);

            return new Guid(bytes);
        }

        public static Rectangle ToSourceRectangle(this Guid id) {
            byte[] bytes = id.ToByteArray();
            int x = bytes[15] << 24 + bytes[14] << 16 + bytes[13] << 8 + bytes[12];
            int y = bytes[11] << 24 + bytes[10] << 16 + bytes[9] << 8 + bytes[8];
            int width = bytes[7] << 24 + bytes[6] << 16 + bytes[5] << 8 + bytes[4];
            int height = bytes[3] << 24 + bytes[2] << 16 + bytes[1] << 8 + bytes[0];
            return new Rectangle(x, y, width, height);
        }

        public static bool IsValidForItem(this Guid id) {
            return id.ToSourceRectangle().X > Game1.objectSpriteSheet.Width;
        }

        public static string ToItemName(this Guid id) {
            return id.ToString();
        }

        public static bool TryToGuid(this string name, out Guid id) {
            return Guid.TryParse(name, out id);
        }
    }
}
