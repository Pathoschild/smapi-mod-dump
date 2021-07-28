/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatoTK.Patching
{
    internal class TextureDrawHandler<TData> : TextureDrawHandler, ITextureDrawHandler<TData>
    {
        public TData Data => Texture is IPlatoTexture<TData> pTex ? pTex.Data : default;
        public TextureDrawHandler(string id,
            SpriteBatch __instance,
            Texture2D texture,
            Rectangle destinationRectangle,
            Rectangle? sourceRectangle,
            Color color, Vector2 origin,
            float rotation,
            SpriteEffects effects,
            float layerDepth)
            : base(id,__instance, texture, destinationRectangle,sourceRectangle,color,origin,rotation,effects,layerDepth)
        {

        }

       
    }

    internal class TextureDrawHandler : ITextureDrawHandler
    {
        public SpriteBatch SpriteBatch { get; }
        public string Id { get; }
        public Texture2D Texture { get; set; }
        public Rectangle DestinationRectangle { get; set; }

        public Rectangle? SourceRectangle { get; set; }

        public Color Color { get; set; }

        public Vector2 Origin { get; set; }

        public float Rotation { get; set; }

        public SpriteEffects Effects { get; set; }

        public float LayerDepth { get; set; }

        public TextureDrawHandler(string id,
            SpriteBatch __instance,
            Texture2D texture,
            Rectangle destinationRectangle,
            Rectangle? sourceRectangle,
            Color color, Vector2 origin,
            float rotation,
            SpriteEffects effects,
            float layerDepth)
        {
            Texture = texture;
            Id = id;
            SpriteBatch = __instance;
            DestinationRectangle = destinationRectangle;
            SourceRectangle = sourceRectangle;
            Color = color;
            Rotation = rotation;
            Effects = effects;
            LayerDepth = layerDepth;
        }

        public void Draw()
        {
            SpriteBatch.Draw(Texture, DestinationRectangle, SourceRectangle, Color, Rotation, Origin, Effects, LayerDepth);
        }


    }
}
