using CustomNPCFramework.Framework.ModularNPCS.ModularRenderers;
using CustomNPCFramework.Framework.NPCS;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPCFramework.Framework.ModularNPCS
{
    /// <summary>
    /// Used as a wrapper for the npcs to hold sprite information.
    /// </summary>
    public class Sprite
    {
        /// <summary>
        /// The actual sprite to draw for the npc.
        /// </summary>
        public AnimatedSprite sprite;
        /// <summary>
        /// The path to the texture to use for the animated sprite.
        /// </summary>
        public string relativePath;

        /// <summary>
        /// A class for handling character sprites.
        /// </summary>
        /// <param name="path">The full path to the file.</param>
        public Sprite(string path)
        {
            try
            {
                this.relativePath = Class1.getShortenedDirectory(path);
            }
            catch(Exception err)
            {
                this.relativePath = path;
            }
            try
            {
                this.sprite = new AnimatedSprite();
                Texture2D text = Class1.ModHelper.Content.Load<Texture2D>(this.relativePath);
                var reflect=Class1.ModHelper.Reflection.GetField<Texture2D>(this.sprite, "Texture", true);
                reflect.SetValue(text);
                
            }
            catch(Exception err)
            {
                this.sprite = new AnimatedSprite();
                Texture2D text = Class1.ModHelper.Content.Load<Texture2D>(this.relativePath);
                var reflect = Class1.ModHelper.Reflection.GetField<Texture2D>(this.sprite, "Texture", true);
                reflect.SetValue(text);
            }
            this.sprite.SpriteWidth = this.sprite.Texture.Width;
            this.sprite.SpriteHeight = this.sprite.Texture.Height;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">Used to hold the path to the asset.</param>
        /// <param name="texture">Used to assign the texture to the sprite from a pre-loaded asset.</param>
        public Sprite(string path, string texture)
        {
            this.relativePath = path;
            this.sprite = new AnimatedSprite(texture);
            this.sprite.SpriteWidth = this.sprite.Texture.Width;
            this.sprite.SpriteHeight = this.sprite.Texture.Height;
        }

        /// <summary>
        /// Sets the npc's portrait to be this portrait texture.
        /// </summary>
        /// <param name="npc"></param>
        public void setCharacterSpriteFromThis(ExtendedNPC npc)
        {
            npc.Sprite = this.sprite;
        }

        /// <summary>
        /// Reloads the texture for the NPC portrait.
        /// </summary>
        public void reload()
        {
            var text=CustomNPCFramework.Class1.ModHelper.Reflection.GetField<Texture2D>(this.sprite.Texture, "Texture", true);
            Texture2D loaded= Class1.ModHelper.Content.Load<Texture2D>(this.relativePath);
            text.SetValue(loaded);
        }

        /// <summary>
        /// Set's the npc's sprites to face left IF and only if there is a non-null modular Renderer attached to the npc.
        /// </summary>
        /// <param name="npc"></param>
        public void setLeft(ExtendedNPC npc)
        {
            if (npc.characterRenderer == null)
            {
                return;
            }
            else npc.characterRenderer.setLeft();
        }

        /// <summary>
        /// Set's the npc's sprites to face left IF and only if there is a non-null modular Renderer attached to the npc.
        /// </summary>
        /// <param name="npc"></param>
        public void setRight(ExtendedNPC npc)
        {
            if (npc.characterRenderer == null)
            {
                return;
            }
            else npc.characterRenderer.setRight();
        }

        /// <summary>
        /// Set's the npc's sprites to face left IF and only if there is a non-null modular Renderer attached to the npc.
        /// </summary>
        /// <param name="npc"></param>
        public void setDown(ExtendedNPC npc)
        {
            if (npc.characterRenderer == null)
            {
                return;
            }
            else npc.characterRenderer.setDown();
        }

        /// <summary>
        /// Set's the npc's sprites to face left IF and only if there is a non-null modular Renderer attached to the npc.
        /// </summary>
        /// <param name="npc"></param>
        public void setUp(ExtendedNPC npc)
        {
            if (npc.characterRenderer == null)
            {
                return;
            }
            else npc.characterRenderer.setUp();
        }
    }
}
