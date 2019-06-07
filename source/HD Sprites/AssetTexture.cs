using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HDSprites
{
    // Modified from PyTK.Types.ScaledTexture2D
    // Origial Source: https://github.com/Platonymous/Stardew-Valley-Mods/blob/master/PyTK/Types/ScaledTexture2D.cs
    // Original Licence: GNU General Public License v3.0
    // Original Author: Platonymous
    public class AssetTexture : Texture2D
    {
        public string AssetName { get; set; }
        public float Scale { get; set; }
        public virtual Texture2D STexture { get; set; }

        private int UniqueID { get; set; }

        public AssetTexture(string assetName, Texture2D originalTexture, Texture2D newTexture, float scale = 1, bool shouldEncode = false)
            : base(originalTexture.GraphicsDevice, originalTexture.Width, originalTexture.Height)
        {
            AssetName = assetName;
            Scale = scale;
            STexture = newTexture;
            UniqueID = assetName.GetHashCode() & 0xffffff;
            Color[] data = new Color[originalTexture.Width * originalTexture.Height];
            originalTexture.GetData(data);
            if (shouldEncode) data[0] = encode(UniqueID);
            SetData(data);
        }

        public bool checkUniqueID(Color[] data)
        {
            if (data.Length < 1) return false;
            return decode(data[0]).Equals(UniqueID);
        }

        private static Color encode(int uniqueId)
        {
            return new Color(
                (uniqueId >> 16) & 0xff,
                (uniqueId >> 8) & 0xff,
                (uniqueId >> 0) & 0xff,
                0);
        }

        private static int decode(Color color)
        {
            return ((color.R << 16) | (color.G << 8) | (color.B << 0));
        }
    }
}
