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
