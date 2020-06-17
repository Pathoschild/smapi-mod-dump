using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SubterranianOverhaul.Crops
{
    public class CaveCarrotFlower : Crop
    {
        private static int seedIndex = -1;
        private static int cropTextureIndex = -1;

        public const String NAME = "Plantable Cave Carrot";
        public const String DISPLAY_NAME = "Cave Carrot";
        public const int QUALITY = 0;
        public const int PRICE = 0;
        public const int CATEGORY = 74;
        public const int EDIBILITY = -300;
        public const string DESCRIPTION = "A placeholder that will never appear anywhere, it's just so we can plant these things.";

        private const string CROP_DATA_STRING = "2 7 9 6 4/spring summer fall winter/{1}/{0}/3/0/true 4 8 2 .2/false/false";
        private const string OBJECT_DATA_STRING = "{0}/{1}/{2}/Seeds -{3}/{4}/{5}";

        public static void setIndex()
        {
            if (CaveCarrotFlower.seedIndex == -1)
            {
                CaveCarrotFlower.seedIndex = IndexManager.getUnusedObjectIndex();
            }
        }

        public static int getIndex()
        {
            if (CaveCarrotFlower.seedIndex == -1)
            {
                CaveCarrotFlower.setIndex();
            }

            return CaveCarrotFlower.seedIndex;
        }

        public static void setCropIndex()
        {
            if (CaveCarrotFlower.cropTextureIndex == -1)
            {
                CaveCarrotFlower.cropTextureIndex = IndexManager.getUnusedCropIndex();
            }
        }

        public static int getCropIndex()
        {
            if (CaveCarrotFlower.cropTextureIndex == -1)
            {
                CaveCarrotFlower.setCropIndex();
            }

            return CaveCarrotFlower.cropTextureIndex;
        }

        public CaveCarrotFlower() : this(Vector2.Zero)
        {   
        }

        public CaveCarrotFlower(Vector2 tileLocation) : base(seedIndex,(int) tileLocation.X, (int) tileLocation.Y)
        {   
        }

        public static string getCropData()
        {
            return String.Format(CROP_DATA_STRING,CaveCarrotSeed.getIndex(), CaveCarrotFlower.getCropIndex());
        }

        internal static string getObjectData()
        {
            return String.Format(OBJECT_DATA_STRING, CaveCarrotFlower.NAME, CaveCarrotFlower.PRICE, CaveCarrotFlower.EDIBILITY, CaveCarrotFlower.CATEGORY, CaveCarrotFlower.NAME, CaveCarrotFlower.DESCRIPTION);
        }
    }
}
