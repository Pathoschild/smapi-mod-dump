using CustomNPCFramework.Framework.Enums;
using CustomNPCFramework.Framework.Graphics.TextureGroups;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    /// Used to handle loading different textures and handling opperations on those textures.
    /// </summary>
    public class AssetSheet
    {
        /// <summary>
        /// Used to hold the textures for the AssetSheet.
        /// </summary>
        public TextureGroups.TextureGroup textures;

        /// <summary>
        /// Used to hold the info for the paths to these textures.
        /// </summary>
        public AssetInfo assetInfo;
        
        /// <summary>
        /// The path to this assetinfo.json file
        /// </summary>
        public string path;

        /// <summary>
        /// The soruce rectangle for the current texture to draw.
        /// </summary>
        public Rectangle currentAsset;

        public int index;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="info">The asset info file to be read in or created. Holds path information.</param>
        /// <param name="path">The path to the assetinfo file.</param>
        /// <param name="direction">The direction to set the animation.</param>
        public AssetSheet(AssetInfo info,string path,Direction direction=Direction.down)
        {
            this.assetInfo = info;
            this.textures = new TextureGroup(info,path,direction);
            try
            {
                this.path = Class1.getShortenedDirectory(path);
            }
            catch(Exception err)
            {
                this.path = path;
            }

            this.index = 0;
        }
        /// <summary>
        /// Get the path to the current texture.
        /// </summary>
        /// <returns></returns>
        public virtual KeyValuePair<string, Texture2DExtended> getPathTexturePair()
        {
            return new KeyValuePair<string, Texture2DExtended>(this.path, this.textures.currentTexture.currentTexture);
        }


        /// <summary>
        /// Used just to get a copy of this asset sheet.
        /// </summary>
        public virtual AssetSheet clone()
        {
            var asset = new AssetSheet(this.assetInfo,(string)this.path.Clone());
            return asset;
        }

        /// <summary>
        /// Sets the textures for this sheet to face left.
        /// </summary>
        public virtual void setLeft()
        {
            this.textures.setLeft();
        }

        /// <summary>
        /// Sets the textures for this sheet to face up.
        /// </summary>
        public virtual void setUp()
        {
            this.textures.setUp();
        }

        /// <summary>
        /// Sets the textures for this sheet to face down.
        /// </summary>
        public virtual void setDown()
        {
            this.textures.setDown();
        }

        /// <summary>
        /// Sets the textures for this sheet to face left.
        /// </summary>
        public virtual void setRight()
        {
            this.textures.setRight();
        }

        /// <summary>
        /// Get the current animation texture.
        /// </summary>
        /// <returns></returns>
        public virtual Texture2DExtended getCurrentSpriteTexture()
        {
            return this.textures.currentTexture.currentTexture;
        }

        /// <summary>
        /// Get the specific texture depending on the direction and animation type.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual Texture2DExtended getTexture(Direction direction,AnimationType type)
        {
            return this.textures.getTextureFromAnimation(type).getTextureFromDirection(direction);
        }
    }
}
