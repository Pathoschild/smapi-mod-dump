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
    /// <summary>Class for the accessory model</summary>
    public class AccessoryModel
    {
        [JsonIgnore]
        //The accessories texture
        public Texture2D Texture;

        [JsonIgnore]
        //The textures height
        public int TextureHeight;

        [JsonIgnore]
        //Which mod the accessories came from
        public string ModName;

        //The number of hairstyles added by the content pack
        public int NumberOfAccessories;
    }
}
