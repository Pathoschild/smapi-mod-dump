/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MartyrPher/GetGlam
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace GetGlam.Framework.DataModels
{
    /// <summary>
    /// Class used to gather Dresser information from the content packs
    /// </summary>
    public class DresserModel
    {
        // The dressers Texture
        public Texture2D Texture;

        // The textures height
        public int TextureHeight;

        // The mod name that the dresser came from
        public string ModName;
    }
}
