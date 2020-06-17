using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubterranianOverhaul
{
    class IndexManager
    {
        public const int MAX_INDEX = 2000;
        public const int MAX_CROP_INDEX = 100;
        public static IMonitor monitor;
        public static int lastIndex = -1;
        public static int lastCropIndex = -1;

        public static int getUnusedObjectIndex(int startingIndex = 984)
        {   
            if(lastIndex == -1)
            {
                lastIndex = startingIndex;
            }

            int start = lastIndex;
            int currentIndex = start;

            if(currentIndex < MAX_INDEX)
            {
                IndexManager.log("Issuing Object ID " + currentIndex);
                lastIndex = currentIndex + 1;
                return currentIndex;
            } else
            {
                IndexManager.log("Could not find unused object index between " + startingIndex + " and " + MAX_INDEX);
                return -1;
            }
        }

        internal static int getUnusedCropIndex(int startingIndex = 90)
        {
            IndexManager.log("Crop Index Requested.");
            if (lastCropIndex == -1)
            {
                lastCropIndex = startingIndex;
            }

            int start = lastCropIndex;
            int currentIndex = start;

            if (currentIndex < MAX_CROP_INDEX)
            {
                IndexManager.log("Issuing Crop ID: " + currentIndex);
                lastCropIndex = currentIndex + 1;
                return currentIndex;
            }
            else
            {
                IndexManager.log("Could not find unused crop index between " + startingIndex + " and " + MAX_CROP_INDEX);
                return -1;
            }
        }

        private static void log(string message)
        {
            if(monitor != null)
            {
                monitor.Log(message,LogLevel.Debug);
            }
        }

        
    }
}
