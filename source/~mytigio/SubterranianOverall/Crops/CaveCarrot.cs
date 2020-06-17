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
    public class CaveCarrot : Crop
    {
        private static int seedIndex = -1;
        private static int cropTextureIndex = -1;

        private const string CROP_DATA_STRING = "1 1 1/spring summer fall winter/{1}/{0}/-1/0/false/false/false";
        public const int HARVEST_INDEX = 78;

        public static void setIndex()
        {
            if (CaveCarrot.seedIndex == -1)
            {
                CaveCarrot.seedIndex = CaveCarrotSeed.getIndex();
            }
        }

        public static int getIndex()
        {
            if (CaveCarrot.seedIndex == -1)
            {
                CaveCarrot.setIndex();
            }

            return CaveCarrot.seedIndex;
        }

        public static void setCropIndex()
        {
            if (CaveCarrot.cropTextureIndex == -1)
            {
                CaveCarrot.cropTextureIndex = IndexManager.getUnusedCropIndex();
            }
        }

        public static int getCropIndex()
        {
            if (CaveCarrot.cropTextureIndex == -1)
            {
                CaveCarrot.setCropIndex();
            }

            return CaveCarrot.cropTextureIndex;
        }

        public CaveCarrot() : this(Vector2.Zero)
        {   
        }

        public CaveCarrot(Vector2 tileLocation) : base(seedIndex,(int) tileLocation.X, (int) tileLocation.Y)
        {   
        }

        public static string getCropData()
        {
            return String.Format(CROP_DATA_STRING,HARVEST_INDEX,CaveCarrot.getCropIndex());
        }
    }
}
