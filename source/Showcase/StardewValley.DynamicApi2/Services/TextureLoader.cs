using System;
using System.IO;
using Igorious.StardewValley.DynamicApi2.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Igorious.StardewValley.DynamicApi2.Services
{
    public class TextureLoader
    {
        private static readonly Lazy<TextureLoader> Lazy = new Lazy<TextureLoader>(() => new TextureLoader());
        public static TextureLoader Instance => Lazy.Value;

        private static readonly BlendState BlendColor = new BlendState
        {
            ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue,
            AlphaDestinationBlend = Blend.Zero,
            ColorDestinationBlend = Blend.Zero,
            AlphaSourceBlend = Blend.SourceAlpha,
            ColorSourceBlend = Blend.SourceAlpha
        };

        private static readonly BlendState BlendAlpha = new BlendState
        {
            ColorWriteChannels = ColorWriteChannels.Alpha,
            AlphaDestinationBlend = Blend.Zero,
            ColorDestinationBlend = Blend.Zero,
            AlphaSourceBlend = Blend.One,
            ColorSourceBlend = Blend.One
        };

        public Texture2D Load(string path)
        {
            using (var imageStream = File.OpenRead(path))
            {
                return PreMultiply(Texture2D.FromStream(Game1.graphics.GraphicsDevice, imageStream));
            }
        }   

        // Prevent issues with alpha channel.
        // http://gamedev.stackexchange.com/questions/35460/load-texture-from-image-content-in-runtime
        private Texture2D PreMultiply(Texture2D texture)
        {
            try
            {
                var result = new RenderTarget2D(Game1.graphics.GraphicsDevice, texture.Width, texture.Height);
                Game1.graphics.GraphicsDevice.SetRenderTarget(result);
                Game1.graphics.GraphicsDevice.Clear(Color.Black);

                Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendColor);
                Game1.spriteBatch.Draw(texture, texture.Bounds, Color.White);
                Game1.spriteBatch.End();

                Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendAlpha);
                Game1.spriteBatch.Draw(texture, texture.Bounds, Color.White);
                Game1.spriteBatch.End();

                Game1.graphics.GraphicsDevice.SetRenderTarget(null);

                // Need copy to prevent lost texture after result.ContentLost event.
                return CreateCopy(result);
            }
            catch (Exception e)
            {
                Log.Warning("Failed to PreMultiply texture: " + e);
                return texture;
            }
        }

        private static Texture2D CreateCopy(Texture2D source)
        {
            // Entoarox says, that memory stream works faster than copying via Get/SetData.
            var stream = new MemoryStream();
            source.SaveAsPng(stream, source.Width, source.Height);
            return Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream);
        }
    }
}