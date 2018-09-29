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
        
        /// <summary>
        /// Empty/null constructor.
        /// </summary>
        public Texture2DExtended()
        {
            this.Name = "";
            this.texture = null;
            this.path = "";
            this.helper = null;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">The relative path to file on disk. See StardustCore.Utilities.getRelativePath(modname,path);
        public Texture2DExtended(IModHelper helper,string path)
        {
            this.Name = Path.GetFileName(path);
            this.path = path;
            this.texture = helper.Content.Load<Texture2D>(path);
            this.helper = helper;
        }
        
        public Texture2DExtended Copy()
        {
            return new Texture2DExtended(this.helper,this.path);
        } 

        public IModHelper getHelper()
        {
            return this.helper;
        }
    }
}
