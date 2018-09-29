using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValleyMods.LoveBubbles
{
    static class SpriteBatchExtensions
    {
        public static void Draw(this SpriteBatch batch, TextureRegion textureRegion, Vector2 position, Color? color = null)
        {
            batch.Draw(textureRegion.Texture, new Rectangle((int) position.X, (int) position.Y, textureRegion.Width, textureRegion.Height), textureRegion.Region, color ?? Color.White);
        }
    }
}
