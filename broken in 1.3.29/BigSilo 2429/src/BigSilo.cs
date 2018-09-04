using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Harmony;
using System.Collections.Generic;


namespace BigSiloMod
{
    
    public class Utility {
        
        static IEnumerable<CodeInstruction> CIL = null;
        
        public static void SwapMethodBodies(MethodInfo a, MethodInfo b, string harmonyName) {
            
            var harmonyInstance = typeof(HarmonyInstance);
            var harmonyCreate = harmonyInstance.GetMethod("Create");
            var patch = harmonyInstance.GetMethod("Patch");
            var harmony = harmonyCreate.Invoke(null, new object[]{harmonyName});
            var swapCI = new HarmonyMethod(typeof(Utility).GetMethod("SwapCI"));
            patch.Invoke(harmony, new object[]{a, null, null, swapCI}); // load a into CIL, if not first call of SwapMethodBodies, load junk into a
            patch.Invoke(harmony, new object[]{b, null, null, swapCI}); // swap CIL and b, b holds original a, CIL holds original b
            patch.Invoke(harmony, new object[]{a, null, null, swapCI}); // swap CIL with a, a holds original b, CIL holds junk value
        }
        
        public static IEnumerable<CodeInstruction> SwapCI(IEnumerable<CodeInstruction> instructions) {
            if (CIL==null) {
                CIL = instructions;
                return instructions;
            }
            
            IEnumerable<CodeInstruction> result = CIL;
            CIL = instructions;
            return result;
            
        }
        
        
        
    }
    
    
    
    public class ModEntry : Mod {
        
        public override void Entry(IModHelper helper)
        {
            var new_numSilos = typeof(ModEntry).GetMethod("numSilos");
            
            var old_numSilos = typeof(StardewValley.Utility).GetMethod("numSilos");
            
            Utility.SwapMethodBodies(new_numSilos, old_numSilos, "com.lp-programming.lperkins2.bigsilos");
        }
        
        public static int numSilos() {
            int num = 0;
            foreach (Building current in (Game1.getLocationFromName ("Farm") as Farm).buildings) {
                if (current.buildingType.Equals ("Silo") && current.daysOfConstructionLeft <= 0) {
                    num++;
                }
            }
            return num * 1198;
        }
        
        
        
        
    }
}
