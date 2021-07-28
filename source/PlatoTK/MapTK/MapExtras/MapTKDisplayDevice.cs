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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using TMXTile;
using xTile.Dimensions;
using xTile.Display;
using xTile.Layers;
using xTile.Tiles;

namespace MapTK.MapExtras
{
    public class MapTKDisplayDevice : IDisplayDevice
    {
        protected ContentManager m_contentManager;
        protected GraphicsDevice m_graphicsDevice;
        protected Vector2 m_tilePosition;
        protected Microsoft.Xna.Framework.Rectangle m_sourceRectangle;
        protected SpriteBatch m_spriteBatchAlpha;
        protected Color m_modulationColour;
        protected DrawInstructions m_instructions;
        protected Dictionary<TileSheet, Texture2D> m_tileSheetTextures2;
        protected bool adjustOrigin = false;

            public MapTKDisplayDevice(ContentManager contentManager, GraphicsDevice graphicsDevice)
                : this(contentManager, graphicsDevice, false)
        {

        }

        public MapTKDisplayDevice(ContentManager contentManager, GraphicsDevice graphicsDevice, bool compatibility)
        {
            adjustOrigin = compatibility;
            this.m_contentManager = contentManager;
            this.m_graphicsDevice = graphicsDevice;
            this.m_spriteBatchAlpha = new SpriteBatch(graphicsDevice);
            this.m_tileSheetTextures2 = new Dictionary<TileSheet, Texture2D>();
            this.m_tilePosition = new Vector2();
            this.m_sourceRectangle = new Microsoft.Xna.Framework.Rectangle();
            this.m_modulationColour = Color.White;
        }

        public virtual void Clear()
        {
            m_tileSheetTextures2.Clear();
        }

        public virtual void BeginScene(SpriteBatch b)
        {
            m_spriteBatchAlpha = b;
        }

        public virtual Texture2D GetTexture(TileSheet tilesheet)
        {
            if (m_tileSheetTextures2.TryGetValue(tilesheet, out Texture2D texture))
                return texture;
            else
            {
                LoadTileSheetInternal(tilesheet);
                if (m_tileSheetTextures2.TryGetValue(tilesheet, out Texture2D texture2))
                    return texture2;
                else
                    return null;
            }
        }

        public virtual void DisposeTileSheet(TileSheet tileSheet)
        {
            m_tileSheetTextures2.Remove(tileSheet);
        }

        protected virtual void DrawBefore(Layer layer, Location location, float layerDepth)
        {
            if (MapExtrasHandler.DrawBeforeCache.ContainsKey(layer))
                foreach (var extraLayer in MapExtrasHandler.DrawBeforeCache[layer])
                {
                    DrawBefore(extraLayer, location, layerDepth);

                    if(extraLayer.PickTile(location, Game1.viewport.Size) is Tile extraTile)
                        DrawTile(extraTile, location, layerDepth);

                    DrawAfter(extraLayer, location, layerDepth);
                }
        }

        protected virtual void DrawAfter(Layer layer, Location location, float layerDepth)
        {
            if (MapExtrasHandler.DrawAfterCache.ContainsKey(layer))
                foreach (var extraLayer in MapExtrasHandler.DrawAfterCache[layer])
                {
                    DrawBefore(extraLayer, location, layerDepth);

                    if (extraLayer.PickTile(location, Game1.viewport.Size) is Tile extraTile)
                        DrawTile(extraTile, location, layerDepth);

                    DrawAfter(extraLayer, location, layerDepth);
                }
        }


        public virtual void DrawTile(Tile tile, Location location, float layerDepth)
        {
            if (tile == null)
                return;

            if (tile.Layer.Id == "Front")
                DrawBefore(tile.Layer, location, layerDepth);

           

            xTile.Dimensions.Rectangle tileImageBounds = tile.TileSheet.GetTileImageBounds(tile.TileIndex);
            Texture2D tileSheetTexture = GetTexture(tile.TileSheet);
            if (tileSheetTexture == null || tileSheetTexture.IsDisposed)
                return;

            this.m_instructions = tile.GetDrawInstructions();

            this.m_tilePosition.X = (float)location.X + this.m_instructions.Offset.X;
            this.m_tilePosition.Y = (float)location.Y + this.m_instructions.Offset.Y;
            this.m_sourceRectangle.X = tileImageBounds.X;
            this.m_sourceRectangle.Y = tileImageBounds.Y;
            this.m_sourceRectangle.Width = tileImageBounds.Width;
            this.m_sourceRectangle.Height = tileImageBounds.Height;

            if (this.m_instructions.Color is TMXColor color)
                this.m_modulationColour = new Color(color.R, color.G, color.B, color.A);
            else
                this.m_modulationColour = Color.White;
            var origin = Vector2.Zero;
            if (this.m_instructions.Rotation != 0f)
            {
                if (!adjustOrigin)
                {
                    origin = new Vector2(tileImageBounds.Width / 2, tileImageBounds.Height / 2);
                    m_tilePosition += new Vector2(tileImageBounds.Width * Game1.pixelZoom / 2, tileImageBounds.Height * Game1.pixelZoom / 2);
                }
                else
                    m_tilePosition += GetOffsetForFlippedTile(tile.GetRotationValue());
            }
            var destinationRectangle = new Microsoft.Xna.Framework.Rectangle((int)this.m_tilePosition.X, (int)this.m_tilePosition.Y, Game1.tileSize, Game1.tileSize);
            var sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(this.m_sourceRectangle);
            var tileColor = this.m_modulationColour * this.m_instructions.Opacity;
            var rotation = this.m_instructions.Rotation;

            foreach (var tileDraw in Api.MapTKAPI.TileDrawHandlers)
                if (!tileDraw(tileSheetTexture, destinationRectangle, sourceRectangle, tileColor, rotation, origin, (SpriteEffects)m_instructions.Effect, layerDepth))
                    return;

            this.m_spriteBatchAlpha.Draw(
            texture: tileSheetTexture,
            destinationRectangle: destinationRectangle,
            sourceRectangle: sourceRectangle,
            color: tileColor,
            rotation: rotation,
            origin: origin,
            effects: (SpriteEffects)m_instructions.Effect,
            layerDepth);

            if (tile.Layer.Id == "Front")
                DrawAfter(tile.Layer, location, layerDepth);
        }

        protected virtual Vector2 GetOffsetForFlippedTile(int rotation)
        {
            Vector2 offset = Vector2.Zero;

            if (rotation == 90)
                offset.X = Game1.tileSize;
            else if (rotation == -90)
                offset.Y = Game1.tileSize;
            else if (rotation == 180)
            {
                offset.X = Game1.tileSize;
                offset.Y = Game1.tileSize;
            }
            return offset;
        }

        public virtual void EndScene()
        {

        }

        protected virtual void LoadTileSheetInternal(TileSheet tileSheet)
        {
            try
            {

                if (m_contentManager.Load<Texture2D>(tileSheet.ImageSource) is Texture2D texture)
                {
                    if (m_tileSheetTextures2.ContainsKey(tileSheet))
                        m_tileSheetTextures2[tileSheet] = texture;
                    else
                        m_tileSheetTextures2.Add(tileSheet, texture);
                }
                else if (string.IsNullOrWhiteSpace(Path.GetDirectoryName(tileSheet.ImageSource)))
                {
                    tileSheet.ImageSource = Path.Combine("Maps", Path.GetFileName(tileSheet.ImageSource));
                    try
                    {
                        if (m_contentManager.Load<Texture2D>(tileSheet.ImageSource) is Texture2D texture2)
                            if (m_tileSheetTextures2.ContainsKey(tileSheet))
                                m_tileSheetTextures2[tileSheet] = texture2;
                            else
                                m_tileSheetTextures2.Add(tileSheet, texture2);
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {
                if (string.IsNullOrWhiteSpace(Path.GetDirectoryName(tileSheet.ImageSource)))
                {
                    tileSheet.ImageSource = Path.Combine("Maps", Path.GetFileName(tileSheet.ImageSource));

                    if (m_contentManager.Load<Texture2D>(tileSheet.ImageSource) is Texture2D texture)
                        if (m_tileSheetTextures2.ContainsKey(tileSheet))
                            m_tileSheetTextures2[tileSheet] = texture;
                        else
                            m_tileSheetTextures2.Add(tileSheet, texture);
                }
            }
        }

        public virtual void LoadTileSheet(TileSheet tileSheet)
        {
           
        }

        public virtual void SetClippingRegion(xTile.Dimensions.Rectangle clippingRegion)
        {
            int backBufferWidth = this.m_graphicsDevice.PresentationParameters.BackBufferWidth;
            int backBufferHeight = this.m_graphicsDevice.PresentationParameters.BackBufferHeight;
            int x = this.Clamp(clippingRegion.X, 0, backBufferWidth);
            int y = this.Clamp(clippingRegion.Y, 0, backBufferHeight);
            int num1 = this.Clamp(clippingRegion.X + clippingRegion.Width, 0, backBufferWidth);
            int num2 = this.Clamp(clippingRegion.Y + clippingRegion.Height, 0, backBufferHeight);
            int width = num1 - x;
            int height = num2 - y;
            this.m_graphicsDevice.Viewport = new Viewport(x, y, width, height);
        }
        protected virtual int Clamp(int nValue, int nMin, int nMax)
        {
            return Math.Min(Math.Max(nValue, nMin), nMax);
        }
    }
}
