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
using StardewValley;
using System;

namespace PlatoTK.Patching
{
    internal class PlatoTexture : Texture2D, IPlatoTexture
    {
        public string Id { get; }

        public bool SkipHandler { get; protected set; }
        protected Func<ITextureDrawHandler, bool> Handler { get; }
        
        public PlatoTexture(string id, Func<ITextureDrawHandler, bool> handler, Texture2D placeHolder, GraphicsDevice graphicsDevice = null)
            : this(id, handler, placeHolder.Width,placeHolder.Height,graphicsDevice)
        {
            Color[] color = new Color[placeHolder.Width * placeHolder.Height];
            placeHolder?.GetData(color);
            SetData(color);
        }

        public PlatoTexture(string id, Func<ITextureDrawHandler, bool> handler, int width, int height, GraphicsDevice graphicsDevice = null)
            : base(graphicsDevice == null ? Game1.graphics.GraphicsDevice : graphicsDevice, width, height)
        {
            Handler = handler;
            Id = id;
            SkipHandler = false;
        }

        public virtual bool CallTextureHandler(SpriteBatch spriteBatch, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, Vector2 origin, float rotation, SpriteEffects effects, float layerDepth)
        {
            SkipHandler = true;
            bool result = false;

            if (Handler?.Invoke(new TextureDrawHandler(
                    Id,
                    spriteBatch,
                    texture,
                    destinationRectangle,
                    sourceRectangle,
                    color, origin,
                    rotation, effects,
                    layerDepth)
                    ) is bool handled)
                result = handled;
            
            SkipHandler = false;
            return result;
        }
    }


    internal class PlatoTexture<TData> : PlatoTexture, IPlatoTexture<TData>
    {
        public TData Data { get; set; }

        protected Func<ITextureDrawHandler<TData>, bool> DataHandler {get;}

        public override bool CallTextureHandler(SpriteBatch spriteBatch, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, Vector2 origin, float rotation, SpriteEffects effects, float layerDepth)
        {
            SkipHandler = true;
            bool result = false;

            if (DataHandler.Invoke(new TextureDrawHandler<TData>(
                    Id,
                    spriteBatch,
                    texture,
                    destinationRectangle,
                    sourceRectangle,
                    color, origin,
                    rotation, effects,
                    layerDepth)
                    ) is bool handled)
                result = handled;

            SkipHandler = false;
            return result;
        }

        public PlatoTexture(string id, TData data, Func<ITextureDrawHandler<TData>, bool> handler, Texture2D placeHolder, GraphicsDevice graphicsDevice = null)
            : base(id, null, placeHolder, graphicsDevice)
        {
            DataHandler = handler;
            Data = data;
        }

        public PlatoTexture(string id, TData data, Func<ITextureDrawHandler, bool> handler, int width, int height, GraphicsDevice graphicsDevice = null)
            : base(id,handler,width,height,graphicsDevice)
        {
            Data = data;
        }
    }
}
