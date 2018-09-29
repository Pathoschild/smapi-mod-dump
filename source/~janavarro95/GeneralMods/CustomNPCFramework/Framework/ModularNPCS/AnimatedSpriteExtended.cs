using CustomNPCFramework.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardustCore.UIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPCFramework.Framework.ModularNPCS
{
    /// <summary>
    /// Used as a wrapper for the AnimatedSprite class.
    /// </summary>
    public class AnimatedSpriteExtended
    {
        /// <summary>
        /// The actual sprite of the object.
        /// </summary>
        public AnimatedSprite sprite;
        /// <summary>
        /// The path to the texture to load the sprite from.
        /// </summary>
        public string path;


        public AnimatedSpriteExtended(Texture2DExtended texture,AssetSheet assetSheet)
        {
            //Set the sprite texture
            this.sprite = new AnimatedSprite();
            Texture2D load = texture.Copy().texture;
            var thing = Class1.ModHelper.Reflection.GetField<Texture2D>(this.sprite, "Texture", true);
            thing.SetValue(load);

            this.path = assetSheet.path.Clone().ToString();
            this.sprite.currentFrame = assetSheet.index;

            this.sprite.SpriteWidth = (int)assetSheet.assetInfo.assetSize.X;
            this.sprite.SpriteHeight = (int)assetSheet.assetInfo.assetSize.Y;


        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="currentFrame"></param>
        /// <param name="spriteWidth"></param>
        /// <param name="spriteHeight"></param>
        public AnimatedSpriteExtended(string path ,int currentFrame, int spriteWidth, int spriteHeight)
        {
            this.path = Class1.getRelativeDirectory(path);

            //Set the sprite texture
            this.sprite = new AnimatedSprite();
            Texture2D load = Class1.ModHelper.Content.Load<Texture2D>(this.path);
            var thing=Class1.ModHelper.Reflection.GetField<Texture2D>(this.sprite, "Texture", true);
            thing.SetValue(load);

            //Set the fields.
            this.sprite.currentFrame = currentFrame;
            this.sprite.SpriteWidth = spriteWidth;
            this.sprite.SpriteHeight = spriteHeight;

            //this.sprite = new AnimatedSprite(texture, currentFrame, spriteWidth, spriteHeight);
        }

        /// <summary>
        /// Reloads the asset from disk.
        /// </summary>
        public void reload()
        {
            //Set the sprite texture
            Texture2D load = Class1.ModHelper.Content.Load<Texture2D>(this.path);
            var thing = Class1.ModHelper.Reflection.GetField<Texture2D>(this.sprite, "Texture", true);
            thing.SetValue(load);
        }
    }
}
