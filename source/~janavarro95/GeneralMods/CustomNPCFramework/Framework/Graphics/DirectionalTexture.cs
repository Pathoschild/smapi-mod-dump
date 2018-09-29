using CustomNPCFramework.Framework.Enums;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardustCore.UIUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPCFramework.Framework.Graphics
{
    /// <summary>
    /// A class that's used to hold textures for different directions.
    /// </summary>
    public class DirectionalTexture
    {
        /// <summary>
        /// The left texture for this group.
        /// </summary>
        public Texture2DExtended leftTexture;
        /// <summary>
        /// The right texture for this group.
        /// </summary>
        public Texture2DExtended rightTexture;
        
        /// <summary>
        /// The down textiure for this group.
        /// </summary>
        public Texture2DExtended downTexture;
        /// <summary>
        /// The up texture for this group.
        /// </summary>
        public Texture2DExtended upTexture;

        /// <summary>
        /// The current texture for this group.
        /// </summary>
        public Texture2DExtended currentTexture;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="left">The left texture to use.</param>
        /// <param name="right">The right texture to use.</param>
        /// <param name="up">The up texture to use.</param>
        /// <param name="down">The down texture to use.</param>
        /// <param name="direction">The direction texture for the sprite to face.</param>
        public DirectionalTexture(Texture2DExtended left, Texture2DExtended right, Texture2DExtended up, Texture2DExtended down, Direction direction=Direction.down)
        {
            this.leftTexture = left;
            this.rightTexture = right;
            this.upTexture = up;
            this.downTexture = down;

            if (direction == Direction.left) this.currentTexture = leftTexture;
            if (direction == Direction.right) this.currentTexture = rightTexture;
            if (direction == Direction.up) this.currentTexture = upTexture;
            if (direction == Direction.down) this.currentTexture = downTexture;


        }

        
        public DirectionalTexture(IModHelper helper ,NamePairings info, string path, Direction direction = Direction.down)
        {

            new Texture2DExtended(helper, path);

            string leftString= Class1.getShortenedDirectory(Path.Combine(path, info.leftString + ".png")).Remove(0, 1);
            string rightString = Class1.getShortenedDirectory(Path.Combine(path, info.rightString + ".png")).Remove(0, 1);
            string upString = Class1.getShortenedDirectory(Path.Combine(path, info.upString + ".png")).Remove(0, 1);
            string downString = Class1.getShortenedDirectory(Path.Combine(path, info.downString + ".png")).Remove(0, 1);


            this.leftTexture = new Texture2DExtended(helper, leftString);
            this.rightTexture = new Texture2DExtended(helper, rightString);
            this.upTexture = new Texture2DExtended(helper, upString);
            this.downTexture = new Texture2DExtended(helper, downString);

            if (direction == Direction.left) this.currentTexture = leftTexture;
            if (direction == Direction.right) this.currentTexture = rightTexture;
            if (direction == Direction.up) this.currentTexture = upTexture;
            if (direction == Direction.down) this.currentTexture = downTexture;
        }

        /// <summary>
        /// Sets the direction of this current texture to left.
        /// </summary>
        public void setLeft()
        {
            this.currentTexture = leftTexture;
        }

        /// <summary>
        /// Sets the direction of this current texture to up.
        /// </summary>
        public void setUp()
        {
            this.currentTexture = upTexture;
        }

        /// <summary>
        /// Sets the direction of this current texture to down.
        /// </summary>
        public void setDown()
        {
            this.currentTexture = downTexture;
        }

        /// <summary>
        /// Sets the direction of this current texture to right.
        /// </summary>
        public void setRight()
        {
            this.currentTexture = rightTexture;
        }

        /// <summary>
        /// Gets the texture from this texture group depending on the direction.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public virtual Texture2DExtended getTextureFromDirection(Direction direction)
        {
            if (direction == Direction.left) return this.leftTexture;
            if (direction == Direction.right) return this.rightTexture;
            if (direction == Direction.up) return this.upTexture;
            if (direction == Direction.down) return this.downTexture;
            return null;
        }
    }
}
