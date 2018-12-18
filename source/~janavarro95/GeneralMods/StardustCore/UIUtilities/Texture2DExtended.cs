using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.UIUtilities
{
    public class Texture2DExtended
    {
        public string Name;
        public Texture2D texture;
        public string path;
        IModHelper helper;
        public string modID;
        public ContentSource source;
        
        /// <summary>
        /// Empty/null constructor.
        /// </summary>
        public Texture2DExtended()
        {
            this.Name = "";
            this.texture = null;
            this.path = "";
            this.helper = null;
            this.modID = "";
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">The relative path to file on disk. See StardustCore.Utilities.getRelativePath(modname,path);
        public Texture2DExtended(IModHelper helper,IManifest manifest,string path,ContentSource contentSource=ContentSource.ModFolder)
        {
            this.Name = Path.GetFileNameWithoutExtension(path);
            this.path = path;
            this.texture = helper.Content.Load<Texture2D>(path,contentSource);
            this.helper = helper;
            this.modID = manifest.UniqueID;
            this.source = contentSource;
        }

        public Texture2DExtended(IModHelper helper, string modID, string path, ContentSource contentSource = ContentSource.ModFolder)
        {
            this.Name = Path.GetFileNameWithoutExtension(path);
            this.path = path;
            this.texture = helper.Content.Load<Texture2D>(path,contentSource);
            this.helper = helper;
            this.modID = modID;
            this.source = contentSource;
        }

        public Texture2DExtended Copy()
        {
            return new Texture2DExtended(this.helper,this.modID,this.path);
        } 

        public IModHelper getHelper()
        {
            return this.helper;
        }

        /// <summary>
        /// Returns the actual 2D texture held by this wrapper class.
        /// </summary>
        /// <returns></returns>
        public Texture2D getTexture()
        {
            return this.texture;
        }

        public void setTexure(Texture2D text)
        {
            this.texture = text;
        }
    }
}
