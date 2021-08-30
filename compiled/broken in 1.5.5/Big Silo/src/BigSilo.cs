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
using System.Reflection.Emit;
using Harmony.ILCopying;

namespace BigSiloMod
{
    
    //~ public class Utility {
        
        //~ static MethodInfo source;
        
        //~ /*public static void SwapMethodBodies(MethodInfo a, MethodInfo b, string harmonyName) {
            
            //~ var harmonyInstance = typeof(HarmonyInstance);
            //~ var harmonyCreate = harmonyInstance.GetMethod("Create");
            //~ var patch = harmonyInstance.GetMethod("Patch");
            //~ var harmony = harmonyCreate.Invoke(null, new object[]{harmonyName});
            //~ var swapCI = new HarmonyMethod(typeof(Utility).GetMethod("SwapCI"));
            //~ patch.Invoke(harmony, new object[]{a, null, null, swapCI}); // load a into CIL, if not first call of SwapMethodBodies, load junk into a
            //~ patch.Invoke(harmony, new object[]{b, null, null, swapCI}); // swap CIL and b, b holds original a, CIL holds original b
            //~ patch.Invoke(harmony, new object[]{a, null, null, swapCI}); // swap CIL with a, a holds original b, CIL holds junk value
        //~ }
        
        //~ public static IEnumerable<CodeInstruction> SwapCI(IEnumerable<CodeInstruction> instructions) {
            //~ if (CIL==null) {
                //~ CIL = instructions;
                //~ return instructions;
            //~ }
            
            //~ IEnumerable<CodeInstruction> result = CIL;
            //~ CIL = instructions;
            //~ return result;
            
        //~ }*/
        
        //~ public static void CopyMethod(MethodInfo from, MethodInfo to, string harmonyName) {
            //~ var harmonyInstance = typeof(HarmonyInstance);
            //~ var harmonyCreate = harmonyInstance.GetMethod("Create");
            //~ var patch = harmonyInstance.GetMethod("Patch");
            //~ var harmony = harmonyCreate.Invoke(null, new object[]{harmonyName});
            //~ var copyCI = new HarmonyMethod(typeof(Utility).GetMethod("CopyCI"));
            //~ source = from;
            
            //~ patch.Invoke(harmony, new object[]{to, null, null, copyCI});
            
        //~ }
        
        //~ public static IEnumerable<CodeInstruction> CopyCI(ILGenerator generator, MethodBase method, IEnumerable<CodeInstruction> instructions) {
            //~ var ils = MethodBodyReader.GetInstructions(generator, source);
            //~ foreach (var il in ils)
            //~ {
				//~ var instruction = il.GetCodeInstruction();
				//~ if (instruction.opcode != OpCodes.Ret)
					//~ yield return instruction;
			//~ }
            //~ yield return new CodeInstruction(OpCodes.Ret);
        //~ }
        
        
    //~ }
    
    
    
    public class ModEntry : Mod {
        
        public override void Entry(IModHelper helper)
        {
            var new_numSilos = new HarmonyMethod(typeof(ModEntry).GetMethod("numSilos"));
            
            var old_numSilos = typeof(StardewValley.Utility).GetMethod("numSilos");
            var harmonyInstance = typeof(HarmonyInstance);
            var harmonyCreate = harmonyInstance.GetMethod("Create");
            var harmony = harmonyCreate.Invoke(null, new object[]{"com.lp-programming.lperkins2.bigsilos"});
            var patch = harmonyInstance.GetMethod("Patch");
            
            patch.Invoke(harmony, new object[]{old_numSilos, null, new_numSilos, null});
            
        }
        
        public static void numSilos(ref int __result) {
            __result *= 1198;
        }
        
        
        
        
    }
}
