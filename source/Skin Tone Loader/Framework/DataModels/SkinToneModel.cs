/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/SkinToneLoader
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinToneLoader.Framework.DataModels
{
    /// <summary>
    /// Class used to add Skin color packs
    /// </summary>
    public class SkinToneModel
    {
        // The hairs texture
        public Texture2D Texture;

        // The texture height
        //public int TextureHeight;

        // The mod name where the hairstyles came from
        public string ModName;
    }
}
