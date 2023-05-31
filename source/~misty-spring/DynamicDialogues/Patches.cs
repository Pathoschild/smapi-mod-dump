/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using DynamicDialogues.Framework;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StardewModdingAPI;

namespace DynamicDialogues
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class ModPatches
    {
        //for custom greetings
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
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

        //for AddScene
        internal static bool PrefixTryGetCommandH(Event __instance, GameLocation location, GameTime time, string[] args) =>
            PrefixTryGetCommand(__instance, location, time, args);

        private static bool PrefixTryGetCommand(Event __instance, GameLocation location, GameTime time, string[] split)
        {
            if (split.Length <= 1) //scene has optional parameters, so its 2 OR more
            {
                return true;
            }
            else if (split[0].Equals(ModEntry.AddScene, StringComparison.Ordinal))
            {
                EventScene.Add(__instance, location, time, split);
                return false;
            }
            else if (split[0].Equals(ModEntry.RemoveScene, StringComparison.Ordinal))
            {
                EventScene.Remove(__instance, location, time, split);
                return false;
            }
            else if(split[0].Equals(ModEntry.PlayerFind, StringComparison.Ordinal))
            {
                Finder.ObjectHunt(__instance, location, time, split);
                return false;
            }
            return true;
        }
        
        //for custom gifting dialogue
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

        private static bool HasCustomDialogue(Character __instance, string itemId)
        {
            return ModEntry.HasCustomGifting.ContainsKey(__instance.Name) && ModEntry.HasCustomGifting[__instance.Name].Contains(itemId);
        }
    }
}