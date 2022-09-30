/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Omegasis.StardustCore.Animations
{
    /// <summary>
    /// Deals with animated sprites.
    /// </summary>
    public class AnimatedSprite
    {
        /// <summary>
        /// The position of the sprite.
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// The animation manager for the sprite.
        /// </summary>
        public AnimationManager animation;
        /// <summary>
        /// The name of the sprite.
        /// </summary>
        public string name;
        /// <summary>
        /// The draw color for the sprite.
        /// </summary>
        public Color color;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AnimatedSprite()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Name">The name of the sprite.</param>
        /// <param name="Position">The position of the sprite.</param>
        /// <param name="Animation">The animation manager for the sprite.</param>
        /// <param name="DrawColor">The draw color for the sprite.</param>
        public AnimatedSprite(string Name, Vector2 Position, AnimationManager Animation, Color DrawColor)
        {
            this.position = Position;
            this.name = Name;
            this.animation = Animation;
            this.color = DrawColor;
        }

        /// <summary>
        /// Updates the sprite's logic.
        /// </summary>
        /// <param name="Time"></param>
        public virtual void Update(GameTime Time)
        {

        }

        /// <summary>
        /// Draws the sprite to the screen.
        /// </summary>
        /// <param name="b"></param>
        public virtual void draw(SpriteBatch b, float Alpha = 1f)
        {
            this.draw(b, 1f, 0f, Alpha);
        }

        /// <summary>
        /// Draws the sprite to the screen.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="Position"></param>
        /// <param name="Scale"></param>
        /// <param name="Alpha"></param>
        public virtual void draw(SpriteBatch b, Vector2 Position, float Scale, float Alpha = 1f)
        {
            this.draw(b, Position, Scale, 0f, Alpha);
        }

        public virtual void draw(SpriteBatch b, float Scale, float Alpha = 1f)
        {
            this.draw(b, Scale, 0f, Alpha);
        }

        /// <summary>
        /// Draws the sprite to the screen.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="scale"></param>
        /// <param name="depth"></param>
        /// <param name="alpha"></param>
        public virtual void draw(SpriteBatch b, float scale, float depth, float alpha = 1f)
        {
            this.animation.draw(b, this.position, new Color(this.color.R, this.color.G, this.color.B, alpha), scale, SpriteEffects.None, depth);
        }

        /// <summary>
        /// Draws the sprite to the screen.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        /// <param name="depth"></param>
        ///         /// <param name="alpha"></param>
        public virtual void draw(SpriteBatch b, Vector2 position, float scale, float depth, float alpha = 1f)
        {
            this.animation.draw(b, position, new Color(this.color.R, this.color.G, this.color.B, alpha), scale, SpriteEffects.None, depth);
        }

        /// <summary>
        /// Draws the sprite to the screen.
        /// </summary>
        /// <param name="b">The spritebatch which to draw the sprite.</param>
        /// <param name="position">The position on screen.</param>
        /// <param name="scale">The scale of the sprite.</param>
        /// <param name="rotation">The rotation of the sprite.</param>
        /// <param name="depth">The depth of the sprite.</param>
        /// <param name="alpha">The alpha for the sprite.</param>
        public virtual void draw(SpriteBatch b, Vector2 position, float scale, float rotation, float depth, float alpha = 1f)
        {
            this.animation.draw(b, position, new Color(this.color.R, this.color.G, this.color.B, alpha), scale, rotation, SpriteEffects.None, depth);
        }

        /// <summary>
        /// Draws the sprite to the screen.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        /// <param name="depth"></param>
        /// <param name="alpha"></param>
        public virtual void draw(SpriteBatch b, Vector2 position, Vector2 scale, float depth, float alpha = 1f)
        {
            this.animation.draw(b, position, new Color(this.color.R, this.color.G, this.color.B, alpha), scale, SpriteEffects.None, depth);
        }


    }
}
