using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.Patches.FarmPatch
{
    //[HarmonyPatch(typeof(Farm))]
    //[HarmonyPatch("getMapNameFromTypeInt")]
    class getMapNameFromTypeIntPatch
    {
        public static void Postfix(int type, ref string __result)
        {
            if (type > 4)
            {
                if (Memory.loadedFarm == null)
                {
                    Memory.loadCustomFarmType(Game1.whichFarm);
                }
                //Memory.mapLoadSignal = (Memory.loadedFarm.farmMapType == fileType.raw) ? 1 : 0;
                //__result = Path.Combine("..", "..", Memory.loadedFarm.contentpack.DirectoryPath, Memory.loadedFarm.farmMapFile);
                //__result = Path.Combine(Path.Combine("..", ".."), "Mods", Memory.loadedFarm.Folder, Memory.loadedFarm.farmMapFile); 
                Memory.instance.Monitor.Log("Loading: " + Memory.loadedFarm.contentpack.DirectoryPath + " File: " + Memory.loadedFarm.farmMapFile);
                __result = Path.Combine(Memory.loadedFarm.contentpack.DirectoryPath, Memory.loadedFarm.farmMapFile);
            }
            return;
        }
    }
}
