using CustomNPCFramework.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardustCore.UIUtilities;

namespace CustomNPCFramework.Framework.ModularNpcs
{
    /// <summary>Used as a wrapper for the AnimatedSprite class.</summary>
    public class AnimatedSpriteExtended
    {
        /// <summary>The actual sprite of the object.</summary>
        public AnimatedSprite sprite;

        /// <summary>The path to the texture to load the sprite from.</summary>
        public string path;

        /// <summary>Construct an instance.</summary>
        public AnimatedSpriteExtended(Texture2DExtended texture, AssetSheet assetSheet)
        {
            //Set the sprite texture
            this.sprite = new AnimatedSprite();
            Texture2D load = texture.Copy().Texture;
            var thing = Class1.ModHelper.Reflection.GetField<Texture2D>(this.sprite, "Texture", true);
            thing.SetValue(load);

            this.path = assetSheet.path.Clone().ToString();
            this.sprite.currentFrame = assetSheet.index;

            this.sprite.SpriteWidth = (int)assetSheet.assetInfo.assetSize.X;
            this.sprite.SpriteHeight = (int)assetSheet.assetInfo.assetSize.Y;
        }

        /// <summary>Construct an instance.</summary>
        public AnimatedSpriteExtended(string path, int currentFrame, int spriteWidth, int spriteHeight)
        {
            this.path = Class1.getRelativeDirectory(path);

            //Set the sprite texture
            this.sprite = new AnimatedSprite();
            Texture2D load = Class1.ModHelper.Content.Load<Texture2D>(this.path);
            var thing = Class1.ModHelper.Reflection.GetField<Texture2D>(this.sprite, "Texture", true);
            thing.SetValue(load);

            //Set the fields.
            this.sprite.currentFrame = currentFrame;
            this.sprite.SpriteWidth = spriteWidth;
            this.sprite.SpriteHeight = spriteHeight;

            //this.sprite = new AnimatedSprite(texture, currentFrame, spriteWidth, spriteHeight);
        }

        /// <summary>Reloads the asset from disk.</summary>
        public void reload()
        {
            //Set the sprite texture
            Texture2D load = Class1.ModHelper.Content.Load<Texture2D>(this.path);
            var thing = Class1.ModHelper.Reflection.GetField<Texture2D>(this.sprite, "Texture", true);
            thing.SetValue(load);
        }
    }
}
