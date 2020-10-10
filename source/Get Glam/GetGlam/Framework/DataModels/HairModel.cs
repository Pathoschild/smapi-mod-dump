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
using Newtonsoft.Json;

namespace GetGlam.Framework.DataModels
{
    /// <summary>Class the allows hairstyles to be loaded by content packs</summary>
    public class HairModel
    {
        [JsonIgnore]
        //The hairs texture
        public Texture2D Texture;

        [JsonIgnore]
        //The texture height
        public int TextureHeight;

        [JsonIgnore]
        //The mod name where the hairstyles came from
        public string ModName;

        //The number of hairstyles added by the content pack
        public int NumberOfHairstyles;
    }
}
