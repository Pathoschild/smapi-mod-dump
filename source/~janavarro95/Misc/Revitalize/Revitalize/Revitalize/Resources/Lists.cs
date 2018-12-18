using Revitalize.Aesthetics;
using Revitalize.Aesthetics.WeatherDebris;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revitalize.Resources
{
    class Lists
    {
      public static  List<Revitalize.Resources.DataNodes.TrackedTerrainDataNode> trackedTerrainFeatures;
        public static List<Revitalize.Resources.DataNodes.TrackedTerrainDummyDataNode> trackedTerrainFeaturesDummyList;

        public static List<Revitalize.CoreObject> trackedObjectList;
        //public static List<Revitalize.Objects.ModularDecoration> DecorationsToDraw;

        public static List<Type> serializerTypes;
        public static List<string> saplingNames;

        public static void initializeAllLists()
        {
            trackedTerrainFeatures = new List<DataNodes.TrackedTerrainDataNode>();
            trackedTerrainFeaturesDummyList = new List<DataNodes.TrackedTerrainDummyDataNode>();
            trackedObjectList = new List<CoreObject>();
           
            WeatherDebrisSystem.thisWeatherDebris = new List<WeatherDebrisPlus>();
            //DecorationsToDraw = new List<Objects.ModularDecoration>();
        }

        public static void loadAllListsAfterMovement()
        {
            Serialize.parseTrackedTerrainDataNodeList(Path.Combine( Serialize.PlayerDataPath ,"TrackedTerrainFeaturesList.json"));
              
            Class1.hasLoadedTerrainList = true;
            //  Log.AsyncC(Path.Combine(Serialize.PlayerDataPath, "TrackedTerrainFeaturesList.json"));
           
        }
        public static void loadAllListsAtEntry()
        {
            serializerTypes = new List<Type>();
            saplingNames = new List<string>();
            loadSerializerTypesList();
            addSaplingNames();
        }

        public static void loadSerializerTypesList()
        {
            serializerTypes.Add(typeof(StardewValley.Item));
            serializerTypes.Add(typeof(StardewValley.Tool));
            serializerTypes.Add(typeof(Revitalize.Objects.Light));
            serializerTypes.Add(typeof(Revitalize.CoreObject));
        }

        public static void addSaplingNames()
        {
            saplingNames.Add("Pine Cone");
            saplingNames.Add("Maple Seed");
            saplingNames.Add("Acorn");
            saplingNames.Add("Apricot Sapling");
            saplingNames.Add("Cherry Sapling");
            saplingNames.Add("Orange Sapling");
            saplingNames.Add("Peach Sapling");
            saplingNames.Add("Apple Sapling");
            saplingNames.Add("Pomegranate Sapling");
        }

    }
}
