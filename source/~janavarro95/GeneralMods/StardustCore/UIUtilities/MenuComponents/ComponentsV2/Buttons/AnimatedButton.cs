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
using Omegasis.StardustCore.Animations;

namespace Omegasis.StardustCore.UIUtilities.MenuComponents.ComponentsV2.Buttons
{
    public class AnimatedButton
    {
        /// <summary>
        /// The sprite that handles all of the visuals for the button.
        /// </summary>
        public AnimatedSprite sprite;
        /// <summary>
        /// The default bounds for the button.
        /// </summary>
        public Rectangle defaultBounds;
        /// <summary>
        /// The actual bounds for the button which takes scale into acount.
        /// </summary>
        public Rectangle bounds
        {
            get
            {
                return new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)(this.defaultBounds.Width * this.scale), (int)(this.defaultBounds.Height * this.scale));
            }
        }
        /// <summary>
        /// The scale for the button.
        /// </summary>
        public float scale;

        /// <summary>
        /// The label for the button.
        /// </summary>
        public string label;
        /// <summary>
        /// The name of the button.
        /// </summary>
        public string name;
        /// <summary>
        /// The hovertext for the button.
        /// </summary>
        public string hoverText;

        /// <summary>
        /// The position of the bounding box.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return this.sprite.position;
            }
            set
            {
                this.sprite.position = value;
                this.defaultBounds.X = (int)this.sprite.position.X;
                this.defaultBounds.Y = (int)this.sprite.position.Y;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Sprite">The sprite for the button.</param>
        /// <param name="DefaultBounds">The default hitbox for the button.</param>
        /// <param name="Scale">The scale for the button's sprite and it's hitbox</param>
        public AnimatedButton(AnimatedSprite Sprite, Rectangle DefaultBounds, float Scale)
        {

            this.sprite = Sprite;
            this.scale = Scale;
            this.defaultBounds = DefaultBounds;
            this.label = "";
            this.name = "";
            this.hoverText = "";
        }

        /// <summary>
        /// Update the button's logic.
        /// </summary>
        /// <param name="time"></param>
        public void update(GameTime time)
        {

        }

        /// <summary>
        /// Draw the button to the screen.
        /// </summary>
        /// <param name="b"></param>
        public void draw(SpriteBatch b,float Alpha=1f)
        {
            this.sprite.draw(b,this.scale,Alpha);
        }

        /// <summary>
        /// Draw the button to the screen.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="Depth"></param>
        public void draw(SpriteBatch b, float Depth,float Alpha=1f)
        {
            this.sprite.draw(b, this.scale, Depth,Alpha);
        }

        /// <summary>
        /// Checks to see if the bounding box for this button contains the given x,y cordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool containsPoint(int x, int y)
        {
            return this.bounds.Contains(x, y);
        }

        /// <summary>
        /// Checks to see if this button has been left clicked.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool receiveLeftClick(int x, int y)
        {
            return this.containsPoint(x, y);
        }
        /// <summary>
        /// Checks to see if this button has been right clicked.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool receiveRightClick(int x, int y)
        {
            return this.containsPoint(x, y);
        }
        /// <summary>
        /// Checks to see if this button has been hover overed.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool receiveHover(int x, int y)
        {
            return this.containsPoint(x, y);
        }
    }
}
