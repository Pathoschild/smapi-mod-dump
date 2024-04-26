/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/SkinToneLoader
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Newtonsoft.Json.Linq;
using SkinToneLoader.Framework.ContentEditors;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using static StardewValley.Menus.LoadGameMenu;

namespace SkinToneLoader.Framework.Patches
{
    /// <summary>
    /// Class that patches the length of skin colors
    /// </summary>
    public class SkinTonePatch
    {

        // Instance of ModEntry
        private static ModEntry modEntryInstance;

        /// <summary>
        /// SkinColorPatch Constructor
        /// </summary>
        /// <param name="entry">The instance of ModEntry</param>
        public SkinTonePatch(ModEntry entry)
        {
            // Set the field
            modEntryInstance = entry;
        }

        internal void Apply(Harmony harmony)
        {
            modEntryInstance.Monitor.Log("Patching changeSkinColor()", LogLevel.Info);

            //harmony.Patch(
            //    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.changeSkinColor)),
            //    transpiler: new HarmonyMethod(GetType(), nameof(ChangeSkinColorTranspiler))
            //);

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.changeSkinColor)),
                prefix: new HarmonyMethod(GetType(), nameof(ChangeSkinColorPrefix))
            );
        }

        /// <summary>
        /// Transpiler patch that patches the skin color length
        /// </summary>
        /// <param name="orginal">The original method</param>
        /// <param name="instructions">The instructions for the original method</param>
        /// <returns>The new instructions</returns>
        public static IEnumerable<CodeInstruction> ChangeSkinColorTranspiler(MethodBase orginal, IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {

            try
            {
                // Create a new list of instructions
                List<CodeInstruction> newInstructions = new List<CodeInstruction>();

                // Loop through each instruction and check for h*rdcoded ints
                foreach (CodeInstruction codeInstruction in instructions)
                {
                    // If the opcode equals Ldc_I4_S, or laymens terms, an int
                    if (codeInstruction.opcode == OpCodes.Ldc_I4_S)
                    {
                        // Create the new instruction and add it to the new list
                        CodeInstruction newIntInstruction = codeInstruction.operand.Equals(0x18) ? new CodeInstruction(OpCodes.Call, typeof(SkinToneEditor).GetMethod(nameof(SkinToneEditor.GetNumberOfSkinTones))) : new CodeInstruction(OpCodes.Call, (typeof(SkinToneEditor).GetMethod(nameof(SkinToneEditor.GetNumberOfSkinToneMinusOne))));
                        newInstructions.Add(newIntInstruction);
                    }
                    else
                    {
                        // Just add the regular instruction to the list
                        newInstructions.Add(codeInstruction);
                    }
                }

                // Return the new instructions for the method
                return newInstructions;
            }
            catch (Exception e)
            {
                // Something went boom, so we abort and send the orginal method instructions
                modEntryInstance.Monitor.Log($"Failed in {nameof(ChangeSkinColorTranspiler)}:\n{e}", LogLevel.Error);
                return instructions;
            }

        }

        /// <summary>
        /// A Prefix patch that patches the skin color length
        /// </summary>
        /// <param name="which">The skin index</param>
        /// <param name="force">A boolean that forces the skin index to -1</param>
        /// <param name="___skin">The skin field in the Farmer class</param>
        /// <param name="__instance">The instance of the Farmer class</param>
        /// <returns>false</returns>
        /// 
        public static bool ChangeSkinColorPrefix(int which, bool force, NetInt ___skin, Farmer __instance)
        {
            if (which < 0)
            {
                which = SkinToneEditor.GetNumberOfSkinToneMinusOne();
            }
            else if (which >= SkinToneEditor.GetNumberOfSkinTones())
            {
                which = 0;
            }

            ___skin.Set(__instance.FarmerRenderer.recolorSkin(which, force));
            SkinToneConfigModelManager.SaveCharacterLayout(modEntryInstance, which);

            return false;
        }
    }
}
