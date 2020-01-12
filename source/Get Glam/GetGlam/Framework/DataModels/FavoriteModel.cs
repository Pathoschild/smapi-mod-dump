using Microsoft.Xna.Framework;

namespace GetGlam.Framework.DataModels
{
    public class FavoriteModel
    {
        //Wether the favorite is default one
        public bool IsDefault = true;

        //Whether the player is male
        public bool IsMale = true;

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
        public int ShoeIndex = 0;

        //The accessory index
        public int AccessoryIndex = -1;

        //Whether the player is bald
        public bool IsBald = false;

        //The players hair color
        public Color HairColor = new Color(193, 90, 50);

        //The players eye color
        public Color EyeColor = new Color(122, 68, 52);
    }
}
