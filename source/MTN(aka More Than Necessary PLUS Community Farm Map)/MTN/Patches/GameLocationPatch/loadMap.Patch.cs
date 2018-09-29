//using Harmony;
//using MTN.FarmInfo;
//using StardewModdingAPI;
//using StardewValley;
//using StardewValley.BellsAndWhistles;
//using StardewValley.Locations;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using xTile;
//using xTile.ObjectModel;
//using xTile.Tiles;

//namespace MTN.Patches.GameLocationPatch
//{
//    //[HarmonyPatch(typeof(GameLocation))]
//    //[HarmonyPatch("loadMap")]
//    public class loadMapPatch
//    {
//        /// <summary>
//        /// Check to see if we are loading a raw map file (.tbin) or .xnb.
//        /// </summary>
//        /// <param name="mapPath">Directory to the file (without extention)</param>
//        /// <returns></returns>
//        public static bool Prefix(string mapPath)
//        {
//            //If it has been signaled that we're loading a .tbin instead of .xnb, skip original method. 
//            if (Memory.mapLoadSignal == 1)
//            {
//                return false;
//            }
//            return true;
//        }

//        /// <summary>
//        /// If Memory was signaled that we're loading a .xnb, this will be skipped. Otherwise, the program
//        /// will proceed with the method.
//        /// </summary>
//        /// <param name="__instance"></param>
//        /// <param name="mapPath"></param>
//        public static void Postfix(GameLocation __instance, string mapPath)
//        {
//            //Of course, if we didn't signal an incoming .tbin, we skip the Postfix. 
//            if (Memory.mapLoadSignal != 1) return;

//            //Flip signal if we did.
//            Memory.mapLoadSignal = 0;

//            //Split off the last piece to get the name (May need to rework for FakeBSD aka Mac OSX & Linux)
//            string[] splitPath = mapPath.Split('\\');
//            __instance.map = Memory.instance.Helper.Content.Load<Map>(Path.Combine("Maps", Memory.loadedFarm.Folder, splitPath[splitPath.Length-1] + ".tbin"), ContentSource.ModFolder);
//            //Get Actual Asset
//            __instance.mapPath.Set(Memory.instance.Helper.Content.GetActualAssetKey(Path.Combine("Maps", Memory.loadedFarm.Folder, Memory.loadedFarm.farmMapFile + ".tbin"), ContentSource.ModFolder));
            
//            //Mimic the rest of the original method, or this may not work as intended.
//            __instance.map.LoadTileSheets(Game1.mapDisplayDevice);

//            PropertyValue isOutdoorsValue;
//            __instance.map.Properties.TryGetValue("Outdoors", out isOutdoorsValue);
//            if (isOutdoorsValue != null)
//            {
//                __instance.isOutdoors.Value = true;
//            }
//            PropertyValue pathLayerLightsValue;
//            __instance.map.Properties.TryGetValue("forceLoadPathLayerLights", out pathLayerLightsValue);
//            if (pathLayerLightsValue != null)
//            {
//                __instance.forceLoadPathLayerLights = true;
//            }
//            PropertyValue treatAsOutdoorsValue;
//            __instance.map.Properties.TryGetValue("TreatAsOutdoors", out treatAsOutdoorsValue);
//            if (treatAsOutdoorsValue != null)
//            {
//                __instance.treatAsOutdoors.Value = true;
//            }
//            if ((__instance.isOutdoors || __instance is Sewer || __instance is Submarine) && !(__instance is Desert))
//            {
//                __instance.waterTiles = new bool[__instance.map.Layers[0].LayerWidth, __instance.map.Layers[0].LayerHeight];
//                bool foundAnyWater = false;
//                for (int x = 0; x < __instance.map.Layers[0].LayerWidth; x++)
//                {
//                    for (int y = 0; y < __instance.map.Layers[0].LayerHeight; y++)
//                    {
//                        if (__instance.doesTileHaveProperty(x, y, "Water", "Back") != null)
//                        {
//                            foundAnyWater = true;
//                            __instance.waterTiles[x, y] = true;
//                        }
//                    }
//                }
//                if (!foundAnyWater)
//                {
//                    __instance.waterTiles = null;
//                }
//            }
//            if (__instance.isOutdoors)
//            {
//                Traverse.Create(__instance).Field("critters").SetValue(new List<Critter>());
//            }
//            __instance.loadLights();
//        }
//    }
//}
