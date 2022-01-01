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
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace StardustCore.UIUtilities
{
    public class TextureManager
    {
        public static Dictionary<string, Dictionary<string, TextureManager>> TextureManagers = new Dictionary<string, Dictionary<string, TextureManager>>();


        public Dictionary<string, Texture2DExtended> textures;

        public string name;

        public TextureManager(string Name)
        {
            this.name = Name;
            this.textures = new Dictionary<string, Texture2DExtended>();
        }


        public TextureManager(string Name, IContentPack ContentPack)
        {
            this.name = Name;
            this.textures = new Dictionary<string, Texture2DExtended>();
        }

        public void addTexture(string name, Texture2DExtended texture)
        {
            this.textures.Add(name, texture);
        }

        /// <summary>Returns a Texture2DExtended held by the manager.</summary>
        public Texture2DExtended getTexture(string name,bool ThrowError=true)
        {
            if (this.textures.ContainsKey(name))
            {
                return this.textures[name].Copy();
            }

            if (ThrowError)
            {
                throw new Exception(string.Format("Error, texture {0} not found!!!",name));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Checks if the texture exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool containsTexture(string name)
        {
            return this.textures.ContainsKey(name);
        }


        /// <summary>
        /// Content pack search.
        /// </summary>
        public void searchForTextures(IContentPack content, string RelativePath)
        {
            if (string.IsNullOrEmpty(RelativePath))
            {
                string path = content.DirectoryPath;
                this.searchDirectories(path, "", content);
            }
            else
            {
                string path = Path.Combine(content.DirectoryPath, RelativePath);
                this.searchDirectories(path, RelativePath, content);
            }
        }

        /// <summary>
        /// Non-Content pack search.
        /// </summary>
        public void searchForTextures(string RelativePath = "")
        {
            if (string.IsNullOrEmpty(RelativePath))
            {
                string path = ModCore.ModHelper.DirectoryPath;
                this.searchDirectories(path, "", ModCore.Manifest);
            }
            else
            {
                string path = Path.Combine(ModCore.ModHelper.DirectoryPath, RelativePath);
                this.searchDirectories(path, RelativePath, ModCore.Manifest);
            }
        }

        /// <summary>
        /// Searches for textures in the given path. Used to search for textures in extended mods.
        /// </summary>
        /// <param name="ABSPath">The absolute starting path to start looking</param>
        /// <param name="RelativePath"></param>
        public void searchForTextures(string ABSPath,IModHelper helper,IManifest manifest,string RelativePath = "")
        {
            if (string.IsNullOrEmpty(RelativePath))
            {
                string path = ABSPath;
                this.searchDirectories(path, "", manifest);
            }
            else
            {
                string path = Path.Combine(ABSPath, RelativePath);
                this.searchDirectories(path, RelativePath, manifest);
            }
        }
        /// <summary>
        /// Used to search through a mod's directories for textures.
        /// </summary>
        /// <param name="helper">The mod's SMAPI helper object.</param>
        /// <param name="manifest">The mod's SMAPI manifest object.</param>
        /// <param name="RelativePath">The relative starting path.</param>
        public void searchForTextures(IModHelper helper, IManifest manifest, string RelativePath = "")
        {
            if (string.IsNullOrEmpty(RelativePath))
            {
                string path = helper.DirectoryPath;
                this.searchDirectories(path, "",helper,manifest);
            }
            else
            {
                string path = Path.Combine(helper.DirectoryPath, RelativePath);
                this.searchDirectories(path, RelativePath,helper ,manifest);
            }
        }
        /// <summary>
        /// Used to search the directories of a mod that has been extended from Stardust Core.
        /// </summary>
        /// <param name="path">The path to the manifest's directory path</param>
        /// <param name="relativePath"></param>
        /// <param name="helper"></param>
        /// <param name="manifest"></param>
        private void searchDirectories(string path, string relativePath, IModHelper helper,IManifest manifest)
        {
            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);

            string[] extensions = new string[2]
            {
                ".png",
                ".jpg"
            };

            foreach (string directory in dirs)
            {
                string combo = string.IsNullOrEmpty(relativePath) ? Path.GetFileName(directory) : Path.Combine(relativePath, Path.GetFileName(directory));
                this.searchDirectories(directory, combo, helper,manifest);
            }

            foreach (string file in files)
            {
                if (extensions.Contains(Path.GetExtension(file)))
                {
                    this.processFoundTexture(file, relativePath,helper,manifest);
                    //StardustCore.ModCore.ModMonitor.Log("Found a texture!: "+Path.GetFileNameWithoutExtension(file));
                }
            }

        }

        private void searchDirectories(string path, string relativePath, IManifest manifest)
        {
            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);

            string[] extensions = new string[2]
            {
                ".png",
                ".jpg"
            };

            foreach (string directory in dirs)
            {
                string combo = string.IsNullOrEmpty(relativePath) ? Path.GetFileName(directory) : Path.Combine(relativePath, Path.GetFileName(directory));
                this.searchDirectories(directory, combo, manifest);
            }

            foreach (string file in files)
            {
                if (extensions.Contains(Path.GetExtension(file)))
                {
                    this.processFoundTexture(file, relativePath);
                    //StardustCore.ModCore.ModMonitor.Log("Found a texture!: "+Path.GetFileNameWithoutExtension(file));
                }
            }

        }
        private void searchDirectories(string path, string relativePath, IContentPack ContentPack)
        {
            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);

            string[] extensions = new string[2]
            {
                ".png",
                ".jpg"
            };

            foreach (string directory in dirs)
            {
                string combo = string.IsNullOrEmpty(relativePath) ? Path.GetFileName(directory) : Path.Combine(relativePath, Path.GetFileName(directory));
                this.searchDirectories(directory, combo, ContentPack);
            }

            foreach (string file in files)
            {
                if (extensions.Contains(Path.GetExtension(file)))
                {
                    this.processFoundTexture(file, relativePath, ContentPack);
                    //StardustCore.ModCore.ModMonitor.Log("Found a texture!: " + Path.GetFileNameWithoutExtension(file));
                }
            }

        }

        private void processFoundTexture(string file, string relativePath)
        {
            Texture2DExtended textureExtended = new Texture2DExtended(ModCore.ModHelper, ModCore.Manifest, Path.Combine(relativePath, Path.GetFileName(file)));

            //ModCore.log("Found texture: " + textureExtended.Name);
            
            textureExtended.texture.Name = ModCore.Manifest.UniqueID + "_" + this.name + "_" + textureExtended.Name;

            this.addTexture(textureExtended.Name, textureExtended);
        }

        private void processFoundTexture(string file,string relativePath ,IModHelper Helper, IManifest Manifest)
        {
            Texture2DExtended textureExtended = new Texture2DExtended(Helper, Manifest, Path.Combine(relativePath, Path.GetFileName(file)));

            //ModCore.log("Found texture: " + textureExtended.Name);

            textureExtended.texture.Name = Manifest.UniqueID + "_" + this.name + "_" + textureExtended.Name;

            this.addTexture(textureExtended.Name, textureExtended);
        }

        private void processFoundTexture(string file, string relativePath, IContentPack ContentPack)
        {
            Texture2DExtended textureExtended = new Texture2DExtended(ContentPack, Path.Combine(relativePath, Path.GetFileName(file)));

            textureExtended.texture.Name = ContentPack.Manifest.UniqueID + "_" + this.name + "_" + textureExtended.Name;
            //ModCore.log("Found texture: " + textureExtended.Name);

            //this.addTexture(textureExtended.Name, textureExtended);
        }


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        //                      Static Functions                 //
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

        public static void addTexture(IManifest ModManifest, string managerName, string textureName, Texture2DExtended Texture)
        {
            Texture.texture.Name = managerName + "." + textureName;
            TextureManagers[ModManifest.UniqueID][managerName].addTexture(textureName, Texture);
        }

        public static void AddTextureManager(IManifest ModManifest, string Name)
        {
            if (TextureManager.TextureManagers.ContainsKey(ModManifest.UniqueID))
            {
                TextureManagers[ModManifest.UniqueID].Add(Name, new TextureManager(Name));
            }
            else
            {
                TextureManager.TextureManagers.Add(ModManifest.UniqueID, new Dictionary<string, TextureManager>());
                TextureManagers[ModManifest.UniqueID].Add(Name, new TextureManager(Name));
            }

        }

        public static TextureManager GetTextureManager(IManifest Manifest, string Name)
        {
            return TextureManagers[Manifest.UniqueID][Name];
        }

        public static Texture2D GetTexture(IManifest Manifest, string ManagerName, string TextureName)
        {
            return GetTextureManager(Manifest, ManagerName).getTexture(TextureName).texture;
        }
        public static Texture2DExtended GetExtendedTexture(IManifest Manifest, string ManagerName, string TextureName)
        {
            return GetTextureManager(Manifest, ManagerName).getTexture(TextureName);
        }
    }
}
