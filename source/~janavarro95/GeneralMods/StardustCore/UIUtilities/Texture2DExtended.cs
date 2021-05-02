/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace StardustCore.UIUtilities
{
    public class Texture2DExtended
    {
        public string Name;
        public Texture2D texture;
        public string path;
        public string modID;
        public ContentSource source;
        private readonly IModHelper helper;
        private readonly IContentPack content;
        public int Width
        {
            get
            {
                return this.texture.Width;
            }
        }
        public int Height
        {
            get
            {
                return this.texture.Height;
            }
        }

        /// <summary>Empty/null constructor.</summary>
        public Texture2DExtended()
        {
            this.Name = "";
            this.texture = null;
            this.path = "";
            this.helper = null;
            this.modID = "";
        }


        /// <summary>Construct an instance.</summary>
        /// <param name="path">The relative path to file on disk. See StardustCore.Utilities.getRelativePath(modname,path);
        public Texture2DExtended(IModHelper helper, IManifest manifest, string path, ContentSource contentSource = ContentSource.ModFolder)
        {
            this.Name = Path.GetFileNameWithoutExtension(path);
            this.path = path;
            this.texture = helper.Content.Load<Texture2D>(path, contentSource);
            this.helper = helper;
            this.modID = manifest.UniqueID;
            this.source = contentSource;
        }

        public Texture2DExtended(IModHelper helper, string modID, string path, ContentSource contentSource = ContentSource.ModFolder)
        {
            this.Name = Path.GetFileNameWithoutExtension(path);
            this.path = path;
            this.texture = helper.Content.Load<Texture2D>(path, contentSource);
            this.helper = helper;
            this.modID = modID;
            this.source = contentSource;
        }

        public Texture2DExtended(IContentPack ContentPack, IManifest manifest, string path)
        {
            this.Name = Path.GetFileNameWithoutExtension(path);
            this.path = path;
            this.texture = ContentPack.LoadAsset<Texture2D>(path);
            this.helper = null;
            this.modID = manifest.UniqueID;
            this.source = ContentSource.ModFolder;
        }
        public Texture2DExtended(IContentPack ContentPack, string modID, string path)
        {
            this.Name = Path.GetFileNameWithoutExtension(path);
            this.path = path;
            this.texture = ContentPack.LoadAsset<Texture2D>(path);
            this.helper = null;
            this.modID = modID;
            this.source = ContentSource.ModFolder;
        }

        public Texture2DExtended(IContentPack content, string path)
        {
            this.Name = Path.GetFileNameWithoutExtension(path);
            this.path = path;
            this.content = content;
            this.texture = content.LoadAsset<Texture2D>(path);
            this.helper = null;
            this.modID = content.Manifest.UniqueID;
        }

        public Texture2DExtended Copy()
        {
            if (this.helper != null)
            {
                return new Texture2DExtended(this.helper, this.modID, this.path);
            }
            else if (this.content != null)
            {
                return new Texture2DExtended(this.content, this.path);
            }
            else
            {
                throw new System.Exception("Trying to copy a texture that isn't from a mod or a content pack!!!");
            }
        }

        public IModHelper getHelper()
        {
            return this.helper;
        }

        /// <summary>Returns the actual 2D texture held by this wrapper class.</summary>
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
