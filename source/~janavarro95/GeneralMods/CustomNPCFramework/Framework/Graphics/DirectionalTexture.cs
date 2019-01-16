using System.IO;
using CustomNPCFramework.Framework.Enums;
using StardewModdingAPI;
using StardustCore.UIUtilities;

namespace CustomNPCFramework.Framework.Graphics
{
    /// <summary>A class that's used to hold textures for different directions.</summary>
    public class DirectionalTexture
    {
        /// <summary>The left texture for this group.</summary>
        public Texture2DExtended leftTexture;

        /// <summary>The right texture for this group.</summary>
        public Texture2DExtended rightTexture;

        /// <summary>The down textiure for this group.</summary>
        public Texture2DExtended downTexture;

        /// <summary>The up texture for this group.</summary>
        public Texture2DExtended upTexture;

        /// <summary>The current texture for this group.</summary>
        public Texture2DExtended currentTexture;

        /// <summary>Construct an instance.</summary>
        /// <param name="left">The left texture to use.</param>
        /// <param name="right">The right texture to use.</param>
        /// <param name="up">The up texture to use.</param>
        /// <param name="down">The down texture to use.</param>
        /// <param name="direction">The direction texture for the sprite to face.</param>
        public DirectionalTexture(Texture2DExtended left, Texture2DExtended right, Texture2DExtended up, Texture2DExtended down, Direction direction = Direction.down)
        {
            this.leftTexture = left;
            this.rightTexture = right;
            this.upTexture = up;
            this.downTexture = down;

            switch (direction)
            {
                case Direction.left:
                    this.currentTexture = this.leftTexture;
                    break;

                case Direction.right:
                    this.currentTexture = this.rightTexture;
                    break;

                case Direction.up:
                    this.currentTexture = this.upTexture;
                    break;

                case Direction.down:
                    this.currentTexture = this.downTexture;
                    break;
            }
        }

        public DirectionalTexture(IModHelper helper, NamePairings info, string relativePath, Direction direction = Direction.down)
        {
            this.leftTexture = new Texture2DExtended(helper, Path.Combine(relativePath, $"{info.leftString}.png"));
            this.rightTexture = new Texture2DExtended(helper, Path.Combine(relativePath, $"{info.rightString}.png"));
            this.upTexture = new Texture2DExtended(helper, Path.Combine(relativePath, $"{info.upString}.png"));
            this.downTexture = new Texture2DExtended(helper, Path.Combine(relativePath, $"{info.downString}.png"));

            switch (direction)
            {
                case Direction.left:
                    this.currentTexture = this.leftTexture;
                    break;

                case Direction.right:
                    this.currentTexture = this.rightTexture;
                    break;

                case Direction.up:
                    this.currentTexture = this.upTexture;
                    break;

                case Direction.down:
                    this.currentTexture = this.downTexture;
                    break;
            }
        }

        /// <summary>Sets the direction of this current texture to left.</summary>
        public void setLeft()
        {
            this.currentTexture = this.leftTexture;
        }

        /// <summary>Sets the direction of this current texture to up.</summary>
        public void setUp()
        {
            this.currentTexture = this.upTexture;
        }

        /// <summary>Sets the direction of this current texture to down.</summary>
        public void setDown()
        {
            this.currentTexture = this.downTexture;
        }

        /// <summary>Sets the direction of this current texture to right.</summary>
        public void setRight()
        {
            this.currentTexture = this.rightTexture;
        }

        /// <summary>Gets the texture from this texture group depending on the direction.</summary>
        /// <param name="direction">The facing direction.</param>
        public virtual Texture2DExtended getTextureFromDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.left:
                    return this.leftTexture;

                case Direction.right:
                    return this.rightTexture;

                case Direction.up:
                    return this.upTexture;

                case Direction.down:
                    return this.downTexture;

                default:
                    return null;
            }
        }
    }
}
