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
using Omegasis.StardustCore.Animations;
using StardewModdingAPI;
using StardewValley;

namespace Omegasis.StardustCore.UIUtilities
{
    public class TextureManager
    {
        public static Dictionary<string, Dictionary<string, TextureManager>> TextureManagers = new Dictionary<string, Dictionary<string, TextureManager>>();


        public Dictionary<string, Texture2DExtended> extendedTextures;

        public string textureManagerId;

        public string directory;

        public TextureManager(string directory,string Name)
        {
            this.textureManagerId = Name;
            this.extendedTextures = new Dictionary<string, Texture2DExtended>();
            this.directory = directory;
        }


        public TextureManager(string BaseDirectory,string Name, IContentPack ContentPack)
        {
            this.textureManagerId = Name;
            this.extendedTextures = new Dictionary<string, Texture2DExtended>();
            this.directory = BaseDirectory;
        }

        public void addTexture(string name, Texture2DExtended texture)
        {
            if (this.extendedTextures.ContainsKey(name)) return;

            this.extendedTextures.Add(name, texture);
        }

        /// <summary>Returns a Texture2DExtended held by the manager.</summary>
        public Texture2DExtended getExtendedTexture(string textureId,bool ThrowError=true)
        {
            if (this.extendedTextures.ContainsKey(textureId))
            {
                return this.extendedTextures[textureId].Copy();
            }

            if (ThrowError)
            {
                throw new Exception(string.Format("Error, texture {0} not found!!!",textureId));
            }
            else
            {
                return null;
            }
        }

        /// <summary>Returns a <see cref="Texture2D"/> held by the manager.</summary>
        public Texture2D getTexture(string textureId, bool ThrowError = true)
        {
            Texture2DExtended tex = this.getExtendedTexture(textureId, ThrowError);
            if (tex == null)
            {
                return null;
            }
            return tex.texture;
        }

        /// <summary>
        /// Checks if the texture exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool containsTexture(string name)
        {
            return this.extendedTextures.ContainsKey(name);
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
                    this.processFoundTexture(file, relativePath,manifest);
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
            Texture2DExtended textureExtended = new Texture2DExtended(ModCore.Manifest, Path.Combine(relativePath, Path.GetFileName(file)),this.textureManagerId);

            //ModCore.log("Found texture: " + textureExtended.Name);
            
            textureExtended.texture.Name = ModCore.Manifest.UniqueID + "_" + this.textureManagerId + "_" + textureExtended.name;

            this.addTexture(textureExtended.name, textureExtended);
        }

        private void processFoundTexture(string file,string relativePath , IManifest Manifest)
        {
            Texture2DExtended textureExtended = new Texture2DExtended(Manifest, Path.Combine(relativePath, Path.GetFileName(file)),this.textureManagerId);

            //ModCore.log("Found texture: " + textureExtended.Name);

            textureExtended.texture.Name = Manifest.UniqueID + "_" + this.textureManagerId + "_" + textureExtended.name;

            this.addTexture(textureExtended.name, textureExtended);
        }

        private void processFoundTexture(string file, string relativePath, IContentPack ContentPack)
        {
            Texture2DExtended textureExtended = new Texture2DExtended(ContentPack, Path.Combine(relativePath, Path.GetFileName(file)),this.textureManagerId);

            textureExtended.texture.Name = ContentPack.Manifest.UniqueID + "_" + this.textureManagerId + "_" + textureExtended.name;
            //ModCore.log("Found texture: " + textureExtended.Name);

            //this.addTexture(textureExtended.Name, textureExtended);
        }

        /// <summary>
        /// Loads in a texture and attempts to save it to the existing texture2DExtended passed in. 
        /// </summary>
        /// <param name="RelativePath"></param>
        /// <param name="TextureName"></param>
        /// <param name="existingTexture"></param>
        /// <returns></returns>
        public virtual Texture2D loadTexture(string RelativePath, string TextureName, Texture2DExtended existingTexture=null)
        {
            string path = Path.Combine(this.directory, RelativePath);
            if (File.Exists(path))
            {
                Texture2D tex = Texture2D.FromFile(Game1.graphics.GraphicsDevice, path);

                if (existingTexture != null)
                {
                    existingTexture.setTexture(tex);
                    if (this.extendedTextures.ContainsKey(existingTexture.name) == false)
                    {
                        this.addTexture(existingTexture.name, existingTexture);
                    }
                }

                return tex;
            }
            return null;
        }

        /// <summary>
        /// Creates a new animation manager with the given parameters.
        /// </summary>
        /// <param name="TextureName"></param>
        /// <param name="DefaultAnimation"></param>
        /// <returns></returns>
        public AnimationManager createAnimationManager(string TextureName, Animation DefaultAnimation)
        {
            return new AnimationManager(this.getExtendedTexture(TextureName), DefaultAnimation);
        }

        /// <summary>
        /// Creates a new animation manager with the given parameters.
        /// </summary>
        /// <param name="TextureName"></param>
        /// <param name="Animations"></param>
        /// <param name="DefaultAnimationKey"></param>
        /// <param name="StartingAnimationKey"></param>
        /// <param name="startingAnimationFrame"></param>
        /// <param name="EnabledByDefault"></param>
        /// <returns></returns>
        public AnimationManager createAnimationManager(string TextureName, Dictionary<string, Animation> Animations, string DefaultAnimationKey, string StartingAnimationKey, int startingAnimationFrame = 0, bool EnabledByDefault = true)
        {
            return new AnimationManager(this.getExtendedTexture(TextureName), Animations, DefaultAnimationKey, StartingAnimationKey, startingAnimationFrame, EnabledByDefault);
        }


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        //                      Static Functions                 //
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

        public static void addTexture(IManifest ModManifest, string managerName, string textureName, Texture2DExtended Texture)
        {
            Texture.texture.Name = managerName + "." + textureName;
            TextureManagers[ModManifest.UniqueID][managerName].addTexture(textureName, Texture);
        }

        public static void AddTextureManager(string BasePath,IManifest ModManifest, string Name)
        {
            if (TextureManager.TextureManagers.ContainsKey(ModManifest.UniqueID))
            {
                TextureManagers[ModManifest.UniqueID].Add(Name, new TextureManager(BasePath,Name));
            }
            else
            {
                TextureManager.TextureManagers.Add(ModManifest.UniqueID, new Dictionary<string, TextureManager>());
                TextureManagers[ModManifest.UniqueID].Add(Name, new TextureManager(BasePath,Name));
            }

        }

        public static TextureManager GetTextureManager(IManifest Manifest, string Name)
        {
            return GetTextureManager(Manifest.UniqueID, Name);
        }

        /// <summary>
        /// Gets a texture 
        /// </summary>
        /// <param name="ModId"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static TextureManager GetTextureManager(string ModId, string Name)
        {
            return TextureManagers[ModId][Name];
        }

        public static Texture2D GetTexture(IManifest Manifest, string ManagerName, string TextureName)
        {
            return GetTextureManager(Manifest, ManagerName).getExtendedTexture(TextureName).texture;
        }


        public static Texture2DExtended GetExtendedTexture(IManifest Manifest, string ManagerName, string TextureName)
        {
            return GetTextureManager(Manifest, ManagerName).getExtendedTexture(TextureName);
        }


    }
}
