using CustomNPCFramework.Framework.NPCS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPCFramework.Framework.ModularNPCS
{
    /// <summary>
    /// Used as a base class for character animations.
    /// </summary>
    public class CharacterAnimationBase
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public CharacterAnimationBase()
        {
        }

        /// <summary>
        /// Set the character sprites to left.
        /// </summary>
        public virtual void setLeft()
        {
        }

        /// <summary>
        /// Set the character sprites to right.
        /// </summary>
        public virtual void setRight()
        {

        }

        /// <summary>
        /// Set the character sprites to up.
        /// </summary>
        public virtual void setUp()
        {

        }
        /// <summary>
        /// Set the character sprites to down.
        /// </summary>
        public virtual void setDown()
        {

        }

        /// <summary>
        /// Used to reload the sprite textures.
        /// </summary>
        public virtual void reload()
        {
        }

        /// <summary>
        /// Animate the sprites.
        /// </summary>
        /// <param name="animationInterval">How long between animation frames in milliseconds.</param>
        public virtual void Animate(float animationInterval)
        {

        }

        /// <summary>
        /// Used to animate sprites.
        /// </summary>
        /// <param name="animationInterval">How long between animation frames in milliseconds.</param>
        /// <param name="loop">Loop the animation.</param>
        public virtual void Animate(float animationInterval, bool loop=true)
        {

        }

        /// <summary>
        /// Used to draw the sprite to the screen.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="screenPosition"></param>
        /// <param name="layerDepth"></param>
        public virtual void draw(SpriteBatch b, Vector2 screenPosition, float layerDepth)
        {
          
        }



        /// <summary>
        /// Used to draw the sprite to the screen.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="screenPosition"></param>
        /// <param name="layerDepth"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="c"></param>
        /// <param name="flip"></param>
        /// <param name="scale"></param>
        /// <param name="rotation"></param>
        /// <param name="characterSourceRectOffset"></param>
        public virtual void draw(SpriteBatch b, Vector2 screenPosition, float layerDepth, int xOffset, int yOffset, Color c, bool flip = false, float scale = 1f, float rotation = 0.0f, bool characterSourceRectOffset = false)
        {
           
        }

        /// <summary>
        /// A very verbose asset drawer.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="npc"></param>
        /// <param name="position"></param>
        /// <param name="sourceRectangle"></param>
        /// <param name="color"></param>
        /// <param name="alpha"></param>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="effects"></param>
        /// <param name="layerDepth"></param>
        public virtual void draw(SpriteBatch b, ExtendedNPC npc, Vector2 position, Rectangle sourceRectangle, Color color, float alpha, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
           
        }

    }
}
