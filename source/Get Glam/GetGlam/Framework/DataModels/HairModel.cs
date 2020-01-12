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
