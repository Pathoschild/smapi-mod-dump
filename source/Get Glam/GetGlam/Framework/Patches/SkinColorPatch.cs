using Harmony;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace GetGlam.Framework.Patches
{
    /// <summary>Class that patches the length of skin colors</summary>
    public class SkinColorPatch
    {
        //Instance of ModEntry
        private static ModEntry Entry;

        /// <summary>SkinColorPatch Constructor</summary>
        /// <param name="entry">The instance of ModEntry</param>
        public SkinColorPatch(ModEntry entry)
        {
            //Set the field
            Entry = entry;
        }

        /// <summary>Transpiler patch that patches the skin color length</summary>
        /// <param name="orginal">The original method</param>
        /// <param name="instructions">The instructions for the original method</param>
        /// <returns>The new instructions</returns>
        public static IEnumerable<CodeInstruction> ChangeSkinColorTranspiler(MethodBase orginal, IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                //Create a new list of instructions
                List<CodeInstruction> newInstructions = new List<CodeInstruction>();

                //Loop through each instruction and check for h*rdcoded ints
                foreach (CodeInstruction codeInstruction in instructions)
                {
                    //If the opcode equals Ldc_I4_S, or laymens terms, an int
                    if (codeInstruction.opcode == OpCodes.Ldc_I4_S)
                    {
                        //Create the new instruction and add it to the new list
                        CodeInstruction newIntInstruction = codeInstruction.operand.Equals(0x18) ? new CodeInstruction(OpCodes.Call, typeof(ImageInjector).GetMethod(nameof(ImageInjector.GetNumberOfSkinColorMinusOne))) : new CodeInstruction(OpCodes.Call, (typeof(ImageInjector).GetMethod(nameof(ImageInjector.GetNumberOfSkinColorMinusOne))));
                        newInstructions.Add(newIntInstruction);
                    }
                    else
                    {
                        //Just add the regular instruction to the list
                        newInstructions.Add(codeInstruction);
                    }
                }

                //Return the new instructions for the method
                return newInstructions;
            }
            catch (Exception e)
            {
                //Something went boom, so we abort and send the orginal method instructions
                Entry.Monitor.Log($"Failed in {nameof(ChangeSkinColorTranspiler)}:\n{e}", LogLevel.Error);
                return instructions;
            }

        }
    }
}
