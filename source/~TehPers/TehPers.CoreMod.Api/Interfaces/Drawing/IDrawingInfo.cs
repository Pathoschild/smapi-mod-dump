using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Api.Drawing {
    public interface IDrawingInfo : IReadonlyDrawingInfo {
        /// <summary>True if the original drawing call has been cancelled.</summary>
        bool Cancelled { get; }

        /// <summary>True if this drawing call has been modified by the current overrider.</summary>
        bool Modified { get; }

        /// <summary>True if the drawing information should be propagated to the next overrider afterwards.</summary>
        bool Propagate { get; }

        /// <summary>Adds a tint by multiplying it with the current tint.</summary>
        /// <param name="tint">The tint to add.</param>
        void AddTint(in SColor tint);

        /// <summary>Prevents this drawing information from being drawn.</summary>
        void Cancel();

        /// <summary>Immediately draws the texture and prevents the texture from being automatically drawn after propagation. This also prevents the current draw call from propagating further.</summary>
        void DrawAndCancel();

        /// <summary>Sets the scaling of the source image.</summary>
        /// <param name="scale">The amount to scale the source by when drawing.</param>
        void SetScale(float scale);

        /// <summary>Sets the scaling of the source image.</summary>
        /// <param name="scale">The amount to scale the source by when drawing.</param>
        void SetScale(Vector2 scale);

        /// <summary>Sets the source texture and rectangle.</summary>
        /// <param name="texture">The new source texture.</param>
        /// <param name="sourceRectangle">The new source rectangle.</param>
        void SetSource(Texture2D texture, in SRectangle? sourceRectangle);

        /// <summary>Sets the destination rectangle for the texture.</summary>
        /// <param name="destination">The new destination rectangle.</param>
        void SetDestination(in SRectangle destination);

        /// <summary>Sets the rotational and scaling origin for the texture. Any scaling or rotating of the texture will be centered about the given vector.</summary>
        /// <param name="origin">The new rotational and scaling origin.</param>
        void SetOrigin(Vector2 origin);

        /// <summary>Sets the rotation of the texture on the destination.</summary>
        /// <param name="rotation">The new rotation.</param>
        void SetRotation(float rotation);

        /// <summary>Sets the tint color.</summary>
        /// <param name="tint">The new tint color.</param>
        void SetTint(in SColor tint);

        /// <summary>Sets the sprite effects for the texture, which include flipping it horizontally and vertically when drawn.</summary>
        /// <param name="effects">The new effects to apply to the texture.</param>
        void SetEffects(SpriteEffects effects);

        /// <summary>Sets the depth the texture will be drawn at, which determines draw order in some draw batches.</summary>
        /// <param name="depth">The new depth for the texture to be drawn at.</param>
        void SetDepth(float depth);

        /// <summary>Prevents any other drawing overriders from handling this draw call afterwards.</summary>
        void StopPropagating();
    }
}