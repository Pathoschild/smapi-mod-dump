using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Drawing {
    internal class DrawingInfo : IDrawingInfo, IDrawingInfoInternal {
        public Texture2D Texture { get; private set; }
        public SRectangle? SourceRectangle { get; private set; }
        public SRectangle Destination { get; private set; }
        public SColor Tint { get; private set; }
        public SpriteBatch Batch { get; }
        public Vector2 Origin { get; private set; }
        public float Rotation { get; private set; }
        public SpriteEffects Effects { get; private set; }
        public float Depth { get; private set; }

        public bool Cancelled { get; private set; }
        public bool Modified { get; private set; }
        public bool Propagate { get; private set; }

        private DrawingInfo() {
            this.Cancelled = false;
            this.Modified = false;
            this.Propagate = true;
        }

        public DrawingInfo(SpriteBatch batch, Texture2D texture, SRectangle? sourceRectangle, SRectangle destination, SColor tint, Vector2 origin, float rotation, SpriteEffects effects, float depth) : this() {
            this.Texture = texture;
            this.SourceRectangle = sourceRectangle;
            this.Destination = destination;
            this.Tint = tint;
            this.Batch = batch;
            this.Origin = origin;
            this.Rotation = rotation;
            this.Effects = effects;
            this.Depth = depth;
        }

        public DrawingInfo(SpriteBatch batch, Texture2D texture, SRectangle? sourceRectangle, Vector2 destination, SColor tint, Vector2 origin, float rotation, SpriteEffects effects, float depth) : this() {
            this.Texture = texture;
            this.SourceRectangle = sourceRectangle;
            this.Tint = tint;
            this.Batch = batch;
            this.Origin = origin;
            this.Rotation = rotation;
            this.Effects = effects;
            this.Depth = depth;

            SRectangle sourceBounds = this.SourceRectangle ?? this.Texture.Bounds;
            this.Destination = new SRectangle((int) destination.X, (int) destination.Y, sourceBounds.Width, sourceBounds.Height);
        }

        public void SetTint(in SColor tint) {
            this.Tint = tint;
            this.Modified = true;
        }

        public void SetEffects(SpriteEffects effects) {
            this.Effects = effects;
        }

        public void SetDepth(float depth) {
            this.Depth = depth;
        }

        public void AddTint(in SColor tint) {
            this.SetTint(this.Tint * tint);
        }

        public void Cancel() {
            this.StopPropagating();
            this.Cancelled = true;
        }

        public void DrawAndCancel() {
            this.Cancel();
            this.Draw();
        }

        public void SetScale(float scale) => this.SetScale(new Vector2(scale, scale));
        public void SetScale(Vector2 scale) {
            SRectangle source = this.SourceRectangle ?? this.Texture.Bounds;
            Vector2 prevScale = this.GetScale();

            // Calculate new size for the rectangle
            float newWidth = scale.X * source.Width;
            float newHeight = scale.Y * source.Height;

            // Calculate new location for the rectangle
            float xOffsetScale = this.Origin.X / source.Width;
            float yOffsetScale = this.Origin.Y / source.Height;
            float destOriginX = this.Destination.X + this.Destination.Width * xOffsetScale;
            float destOriginY = this.Destination.Y + this.Destination.Height * yOffsetScale;
            float newX = destOriginX - this.Destination.Width * xOffsetScale;
            float newY = destOriginY - this.Destination.Height * yOffsetScale;

            // Set the destination rectangle
            this.Destination = new SRectangle((int) newX, (int) newY, (int) newWidth, (int) newHeight);
        }

        public Vector2 GetScale() {
            SRectangle source = this.SourceRectangle ?? this.Texture.Bounds;
            return new Vector2((float) this.Destination.Width / source.Width, (float) this.Destination.Height / source.Height);
        }

        public void SetSource(Texture2D texture, in SRectangle? sourceRectangle) {
            SRectangle prevSource = this.SourceRectangle ?? this.Texture.Bounds;
            SRectangle newSource = sourceRectangle ?? texture.Bounds;

            this.Texture = texture;
            this.SourceRectangle = sourceRectangle;
            this.Origin = new Vector2(this.Origin.X * newSource.Width / prevSource.Width, this.Origin.Y * newSource.Height / prevSource.Height);
            this.Modified = true;
        }

        public void SetDestination(in SRectangle destination) {
            this.Destination = destination;
            this.Modified = true;
        }

        public void SetOrigin(Vector2 origin) {
            this.Origin = origin;
            this.Modified = true;
        }

        public void SetRotation(float rotation) {
            this.Rotation = rotation;
            this.Modified = true;
        }

        public void StopPropagating() {
            this.Propagate = false;
            this.Modified = true;
        }

        public void Reset() {
            this.Modified = false;
        }

        public void Draw() {
            this.Batch.Draw(this.Texture, this.Destination, this.SourceRectangle, this.Tint, this.Rotation, this.Origin, this.Effects, this.Depth);
        }
    }
}