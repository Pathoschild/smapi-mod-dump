using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.FarmHousePatches
{
    /// <summary>
    /// REASON FOR PATCHING: Custom furniture layouts.
    /// 
    /// Patches the constructor of FarmHouse to allow map makers and content
    /// creators to adjust the starting furniture set at the beginning of a new
    /// game.
    /// </summary>
    public class ConstructorFarmHousePatch
    {
        private static ICustomManager customManager;
        private static int SwappedID;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="customManager">The class controlling information pertaining to the customs (and the loaded customs).</param>
        public ConstructorFarmHousePatch(ICustomManager customManager) {
            ConstructorFarmHousePatch.customManager = customManager;
            SwappedID = -1;
        }

        /// <summary>
        /// Prefix Method. Occurs before the original method is executed.
        /// 
        /// Checks to see if a custom farm is loaded and wants to use a canon layout. 
        /// Swaps the whichFarm Id out temporary to invoke adding furniture layouts from canon farms.
        /// </summary>
        public static void Prefix() {
            if (!customManager.Canon && customManager.FurnitureLayout < 5 && customManager.FurnitureLayout >= 0) {
                SwappedID = Game1.whichFarm;
                Game1.whichFarm = customManager.FurnitureLayout;
            }
        }

        /// <summary>
        /// Postfix Method. Occurs after the original method has executed.
        /// 
        /// Checks to if a custom farm is loaded. Swaps back the whichFarm Id, and loads any additional
        /// furniture into the Farm house object if provided.
        /// </summary>
        /// <param name="__instance">The instance of FarmHouse that was constructed.</param>
        public static void Postfix(FarmHouse __instance) {
            if (customManager.Canon) return;

            if (SwappedID != -1) {
                Game1.whichFarm = SwappedID;
                SwappedID = -1;
            }

            if (customManager.LoadedFarm.FurnitureList != null) {
                foreach (Furniture f in customManager.LoadedFarm.FurnitureList) {
                    __instance.furniture.Add(f);
                }
            }

            if (customManager.LoadedFarm.ObjectList != null) {
                foreach (StardewValley.Object o in customManager.LoadedFarm.ObjectList) {
                    //__instance.objects.Add(o);
                }
            }
        }
    }
}
