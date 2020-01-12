using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GetGlam.Framework.DataModels
{
    /// <summary>Class the allows the player to save their customization layout</summary>
    public class ConfigModel
    {
        //The save folders name
        public string SaveFolderName;

        //Whether it's the deafault config
        public bool IsDefault = true;

        //Whether the player is male
        public bool IsMale;

        //The base index
        public int BaseIndex = 0;

        //The skin index
        public int SkinIndex = 0;

        //The hair index
        public int HairIndex = 0;

        //The face index
        public int FaceIndex = 0;

        //The nose index
        public int NoseIndex = 0;

        //The shoe index
        public int ShoesIndex = 0;

        //The accessory index
        public int AccessoryIndex = -1;

        //The dresser index
        public int DresserIndex = 1;

        //Whether the player is bald
        public bool IsBald = false;

        //The players hair color
        public Color HairColor = new Color(193, 90, 50);

        //The players eye color
        public Color EyeColor = new Color(122, 68, 52);

        //List of the saved favorites
        public List<FavoriteModel> Favorites;
    }
}
