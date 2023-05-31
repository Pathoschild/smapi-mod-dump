/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using StardewModdingAPI;

namespace DynamicDialogues.Patches
{
    // ReSharper disable once InconsistentNaming
    internal class NPCPatches
    {
        internal static void Apply(Harmony harmony)
        {
            ModEntry.Mon.Log($"Applying Harmony patch \"{nameof(NPCPatches)}\": prefixing SDV method \"NPC.sayHiTo(Character)\".");
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.sayHiTo)),
                prefix: new HarmonyMethod(typeof(NPCPatches), nameof(SayHiTo_Prefix))
            );
            
            ModEntry.Mon.Log($"Applying Harmony patch \"{nameof(NPCPatches)}\": prefixing SDV method \"NPC.tryToReceiveActiveObject(Farmer who)\".");
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.tryToReceiveActiveObject)),
                prefix: new HarmonyMethod(typeof(NPCPatches), nameof(TryToReceiveItem))
            );
        }
        
        /// <summary>
        /// Use a custom greeting for NPCs.
        /// </summary>
        /// <param name="__instance">NPC saying hello.</param>
        /// <param name="c">Character being met.</param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        [HarmonyPrefix]
        [HarmonyPatch(nameof(NPC.sayHiTo))]
        public static bool SayHiTo_Prefix(ref NPC __instance, Character c)
        {
            var instancename = __instance.Name;
            var cname = (c as NPC).Name;
            var mainAndRef = (instancename, cname);
            var refAndMain = (cname, instancename);

            try
            {
                //if a (thisnpc, othernpc) key exists
                if (ModEntry.Greetings.TryGetValue(mainAndRef, out var greeting))
                {
                    //log, then use previous key to find value
                    ModEntry.Mon.Log($"Found greeting patch for {__instance.Name}");
                    __instance.showTextAboveHead(greeting);

                    //if that other npc has a key for thisnpc
                    if (!ModEntry.Greetings.TryGetValue(refAndMain, out var greeting1)) return false;
                    //same as before
                    ModEntry.Mon.Log($"Found greeting patch for {(c as NPC).Name}");
                    (c as NPC).showTextAboveHead(greeting1, Color.Black, 2, 3000, 1000 + Game1.random.Next(500));

                    return false;
                }
            }
            catch (Exception ex)
            {
                ModEntry.Mon.Log($"Error while applying patch: {ex}", LogLevel.Error);
            }

            return true;
        }

        /// <summary>
        /// Allows for custom gifting dialogue.
        /// </summary>
        /// <param name="__instance">NPC to give item to.</param>
        /// <param name="who">The player</param>
        /// <returns>A custom dialogue response (if found).</returns>
        [HarmonyPrefix]
        [HarmonyPatch(nameof(NPC.tryToReceiveActiveObject))]
        internal static bool TryToReceiveItem(ref NPC __instance, Farmer who)
        {
            var item = who.ActiveObject;

            var hasCustomDialogue = HasCustomDialogue(__instance,item.QualifiedItemId);

            if (!hasCustomDialogue)
            {
                return true;
            }
            else
            {
                __instance.CurrentDialogue.Push(new Dialogue(__instance, $"Characters\\Dialogue\\{__instance.Name}:Gift.{item.QualifiedItemId}"));
                
                return false;
            }
        }

        /// <summary>
        /// Checks if a Gifting dialogue exists for the specified item.
        /// </summary>
        /// <param name="__instance">Character whose dialogues to check</param>
        /// <param name="itemId">The item.</param>
        /// <returns>True if a custom response was found.</returns>
        private static bool HasCustomDialogue(Character __instance, string itemId)
        {
            return ModEntry.HasCustomGifting.ContainsKey(__instance.Name) && ModEntry.HasCustomGifting[__instance.Name].Contains(itemId);
        }
    }
}