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
using StardewValley;

namespace Omegasis.StardustCore.UIUtilities.MenuComponents.ComponentsV2.Buttons
{
    /// <summary>
    /// A simple menu component for displaying SDV Items as well as being able to click them.
    /// </summary>
    public class ItemDisplayButton
    {

        /// <summary>
        /// The position for the button.
        /// </summary>
        private Vector2 position;
        /// <summary>
        /// The item owned by the button.
        /// </summary>
        public StardewValley.Item item;

        private Rectangle defaultBounds;

        /// <summary>
        /// The hit box for detecting interaction.
        /// </summary>
        public Rectangle boundingBox
        {
            get
            {
                return new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)(this.defaultBounds.Width * this.scale), (int)(this.defaultBounds.Height * this.scale));
            }
        }
        /// <summary>
        /// The scale of the button.
        /// </summary>
        public float scale;
        /// <summary>
        /// Should the stack number be drawn?
        /// </summary>
        public bool drawStackNumber;
        /// <summary>
        /// The color for the item.
        /// </summary>
        public Color drawColor;
        /// <summary>
        /// The background sprite for the item.
        /// </summary>
        public Omegasis.StardustCore.Animations.AnimatedSprite background;

        /// <summary>
        /// The position of the button on screen.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
                this.defaultBounds.X =(int)this.position.X;
                this.defaultBounds.Y =(int)this.position.Y;
            }
        }

        public ItemDisplayButton()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="I">The itme to display.</param>
        /// <param name="Position">The position of the item.</param>
        /// <param name="BoundingBox">The bounding box for the item.</param>
        /// <param name="Scale"></param>
        /// <param name="DrawStackNumber"></param>
        /// <param name="DrawColor"></param>
        public ItemDisplayButton(Item I, Omegasis.StardustCore.Animations.AnimatedSprite Background,Vector2 Position, Rectangle BoundingBox, float Scale, bool DrawStackNumber, Color DrawColor)
        {
            this.item = I;
            this.defaultBounds = BoundingBox;
            this.Position = Position;
            this.scale = Scale;
            this.drawStackNumber = DrawStackNumber;
            this.drawColor = DrawColor;
            this.background = Background;
        }

        public void update(GameTime time)
        {

        }

        /// <summary>
        /// A simple draw function.
        /// </summary>
        /// <param name="b"></param>
        public void draw(SpriteBatch b,float Alpha=1f)
        {
            //this.background.draw(b);
            //if(this.item!=null)this.item.drawInMenu(b, this.position, this.scale);
            this.draw(b, 0f, Alpha, false);
        }


        /// <summary>
        /// The full draw function for drawing this component to the screen.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="Depth"></param>
        /// <param name="Alpha"></param>
        /// <param name="DrawShadow"></param>
        public void draw(SpriteBatch b,float Depth, float Alpha,bool DrawShadow)
        {
            if(this.background!=null)this.background.draw(b, this.scale, Depth,Alpha);
            if (this.item != null)
            {
                this.item.drawInMenu(b, this.position, 1f, Alpha, Depth, StackDrawType.Draw, this.drawColor, DrawShadow);
            }
        }

        /// <summary>
        /// Draws the component at the given position.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="Position"></param>
        /// <param name="Depth"></param>
        /// <param name="Alpha"></param>
        /// <param name="DrawShadow"></param>
        public void draw(SpriteBatch b,Vector2 Position ,float Depth, float Alpha, bool DrawShadow)
        {
            if (this.background != null) this.background.draw(b,Position,this.scale, Depth, Alpha);
            if (this.item != null)
            {
                this.item.drawInMenu(b, Position, 1f, Alpha, Depth, StackDrawType.Draw, this.drawColor, DrawShadow);
            }
        }

        public void draw(SpriteBatch b,float ItemScale ,float Depth, float Alpha, bool DrawShadow)
        {
            this.background.draw(b, this.scale, Depth, Alpha);
            if (this.item != null) this.item.drawInMenu(b, this.position, ItemScale, Alpha, Depth, StackDrawType.Draw, this.drawColor, DrawShadow);
        }

        /// <summary>
        /// Draws just the item to the screen.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="Scale">The scale for the item.</param>
        /// <param name="Depth">The spritebatch depth to draw the item.</param>
        /// <param name="Alpha">The alpha for the item.</param>
        /// <param name="DrawShadow">Should the shadow be drawn for the item?</param>
        public void drawJustItem(SpriteBatch b,float Scale,float Depth, float Alpha, bool DrawShadow)
        {
            if (this.item != null) this.item.drawInMenu(b, this.position, Scale, Alpha, Depth, StackDrawType.Draw, this.drawColor, DrawShadow);
        }

        public bool receiveLeftClick(int x, int y)
        {
            return this.boundingBox.Contains(new Point(x, y));
        }

        public bool receiveRightClick(int x, int y)
        {
            return this.boundingBox.Contains(new Point(x, y));
        }

        public bool ContainsPoint(int x, int y)
        {
            return this.boundingBox.Contains(new Point(x, y));
        }
    }
}
