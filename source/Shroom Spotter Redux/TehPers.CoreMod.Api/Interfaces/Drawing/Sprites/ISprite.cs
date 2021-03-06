/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Api.Drawing.Sprites {
    public interface ISprite : ITextureEvents {
        /// <summary>This sprite's index in its parent sheet.</summary>
        int Index { get; }

        /// <summary>The sprite sheet this sprite comes from.</summary>
        ISpriteSheet ParentSheet { get; }

        /// <summary>The source rectangle for this sprite from the parent sheet.</summary>
        SRectangle? SourceRectangle { get; }

        /// <summary>The x-coordinate of the top-left corner of this sprite.</summary>
        int U { get; }

        /// <summary>The y-coordinate of the top-left corner of this sprite.</summary>
        int V { get; }

        /// <summary>The width of this sprite.</summary>
        int Width { get; }

        /// <summary>The height of this sprite.</summary>
        int Height { get; }

        void Draw(SpriteBatch batch, Vector2 position, Color color);
        void Draw(SpriteBatch batch, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
        void Draw(SpriteBatch batch, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);
        void Draw(SpriteBatch batch, Rectangle destinationRectangle, Color color);
        void Draw(SpriteBatch batch, Rectangle destinationRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth);
    }
}