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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardustCore.UIUtilities;
namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame
{
    /// <summary>
    /// Deals with loading/storing tetxures for Seaside Scramble minigame.
    /// </summary>
    public class SSCTextureUtilities
    {
        /// <summary>
        /// A list of all the texture managers.
        /// </summary>
        public Dictionary<string, TextureManager> textureManagers;
        /// <summary>
        /// Constructor.
        /// </summary>
        public SSCTextureUtilities()
        {
            this.textureManagers = new Dictionary<string, TextureManager>();
        }

        /// <summary>
        /// Adds a texture manager to the list of texture managers.
        /// </summary>
        /// <param name="manager"></param>
        public void addTextureManager(TextureManager manager)
        {
            this.textureManagers.Add(manager.name, manager);
        }
        /// <summary>
        /// Gets the texture manager from the dictionary of them.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public TextureManager getTextureManager(string Name)
        {
            if (this.textureManagers.ContainsKey(Name))
            {
                return this.textureManagers[Name];
            }
            else
            {
                throw new Exception("Sea Side Scramble: Texture Manager:"+Name+"does not exist!");
            }
        }

        /// <summary>
        /// Gets a texture2dExtended from the given texture manager.
        /// </summary>
        /// <param name="ManagerName"></param>
        /// <param name="TextureName"></param>
        /// <returns></returns>
        public Texture2DExtended getExtendedTexture (string ManagerName, string TextureName)
        {
            TextureManager manager = this.getTextureManager(ManagerName);
            if (manager == null)
            {
                return null;
            }
            else
            {
                if (manager.textures.ContainsKey(TextureName))
                {
                    return manager.getTexture(TextureName);
                }
                else
                {
                    throw new Exception("Sea Side Scramble: Texture " + TextureName + " does not exist in texture manager: " + ManagerName);
                }
            }
        }

        /// <summary>
        /// Gets a texture2d from the given texture manager.
        /// </summary>
        /// <param name="ManagerName"></param>
        /// <param name="TextureName"></param>
        /// <returns></returns>
        public Microsoft.Xna.Framework.Graphics.Texture2D getTexture(string ManagerName, string TextureName)
        {
            TextureManager manager = this.getTextureManager(ManagerName);
            if (manager == null)
            {
                return null;
            }
            else
            {
                if (manager.textures.ContainsKey(TextureName))
                {
                    return manager.getTexture(TextureName).texture;
                }
                else
                {
                    throw new Exception("Sea Side Scramble: Texture " + TextureName + " does not exist in texture manager: " + ManagerName);
                }
            }
        }

        /// <summary>
        /// Adds a texture to the given texture manager.
        /// </summary>
        /// <param name="ManagerName"></param>
        /// <param name="TextureName"></param>
        /// <param name="Texture"></param>
        public void addTexture(string ManagerName,string TextureName ,Texture2DExtended Texture)
        {
            TextureManager manager = this.getTextureManager(ManagerName);
            if (manager == null)
            {
                return;
            }
            else
            {
                manager.addTexture(TextureName, Texture);
            }
        }


    }
}
