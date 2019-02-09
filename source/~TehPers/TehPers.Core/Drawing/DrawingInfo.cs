using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.Core.Helpers.Static;

namespace TehPers.Core.Drawing {
    public class DrawingInfo {
        private readonly DrawingDelegator.NativeDraw _nativeDraw;
        public Texture2D Texture { get; private set; }
        public Rectangle? SourceRectangle { get; private set; }
        public Color Tint { get; private set; }
        public bool Cancelled { get; private set; }
        public Vector2 Scale { get; private set; }
        public bool Modified { get; private set; }
        public bool Propagate { get; private set; }

        internal DrawingInfo(Texture2D texture, in Rectangle? sourceRectangle, in Color tint, in Vector2 scale, DrawingDelegator.NativeDraw nativeDraw, Action<Action> resetSignal) {
            this._nativeDraw = nativeDraw;
            this.Texture = texture;
            this.SourceRectangle = sourceRectangle;
            this.Tint = tint;
            this.Scale = scale;

            this.Cancelled = false;
            this.Modified = false;
            this.Propagate = true;

            resetSignal(() => this.Modified = false);
        }

        /// <summary>Sets the source texture and rectangle.</summary>
        /// <param name="texture">The new source texture.</param>
        /// <param name="sourceRectangle">The new source rectangle.</param>
        public void SetSource(Texture2D texture, Rectangle? sourceRectangle) {
            Rectangle originalBounds = this.SourceRectangle ?? this.Texture.Bounds;
            Rectangle newBounds = sourceRectangle ?? texture.Bounds;

            this.Texture = texture;
            this.SourceRectangle = sourceRectangle;
            this.Scale = new Vector2(this.Scale.X * originalBounds.Width / newBounds.Width, this.Scale.Y * originalBounds.Height / newBounds.Height);
            this.Modified = true;
        }

        /// <summary>Sets the tint color.</summary>
        /// <param name="tint">The new tint color.</param>
        public void SetTint(Color tint) {
            this.Tint = tint;
            this.Modified = true;
        }

        /// <summary>Adds a tint by multiplying it with the current tint.</summary>
        /// <param name="tint">The tint to add.</param>
        public void AddTint(Color tint) {
            this.SetTint(this.Tint.Multiply(tint));
        }

        /// <summary>Sets the scaling of the source image.</summary>
        /// <param name="scale">The amount to scale the source by when drawing.</param>
        public void SetScale(float scale) => this.SetScale(new Vector2(scale, scale));

        /// <summary>Sets the scaling of the source image.</summary>
        /// <param name="scale">The amount to scale the source by when drawing.</param>
        public void SetScale(Vector2 scale) {
            this.Scale = scale;
            this.Modified = true;
        }

        public void StopPropagating() {
            this.Propagate = false;
            this.Modified = true;
        }

        /// <summary>Prevents this drawing information from being drawn.</summary>
        public void Cancel() {
            this.StopPropagating();
            this.Cancelled = true;
        }

        /// <summary>Immediately draws the texture and prevents the <see cref="DrawingDelegator"/> from drawing it.</summary>
        public void DrawAndCancel() {
            this.Cancel();
            this._nativeDraw(this);
        }
    }
}