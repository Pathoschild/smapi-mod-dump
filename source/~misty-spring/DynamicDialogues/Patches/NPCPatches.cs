/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;
// ReSharper disable UnusedParameter.Local

namespace DynamicDialogues.Patches;

// ReSharper disable once InconsistentNaming
internal class NPCPatches
{
    private static void Log(string msg, LogLevel lv = LogLevel.Trace) => ModEntry.Mon.Log(msg, lv);
    
    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(NPCPatches)}\": prefixing SDV method \"NPC.getHi(string)\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(NPC), nameof(NPC.getHi)),
            postfix: new HarmonyMethod(typeof(NPCPatches), nameof(Post_getHi))
        );

        Log($"Applying Harmony patch \"{nameof(NPCPatches)}\": prefixing SDV method \"NPC.showTextAboveHead(string, Color?, int, int, int)\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(NPC), nameof(NPC.showTextAboveHead)),
            prefix: new HarmonyMethod(typeof(NPCPatches), nameof(Pre_showTextAboveHead))
        );

        Log($"Applying Harmony patch \"{nameof(NPCPatches)}\": postfixing SDV method \"NPC.getGiftTasteForThisItem(Item)\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(NPC), nameof(NPC.getGiftTasteForThisItem)),
            postfix: new HarmonyMethod(typeof(NPCPatches), nameof(Post_getGiftTasteForThisItem))
            );
    }

	//if context tag has override_default, will make category smth nonexistatn
	internal static void Post_getGiftTasteForThisItem(NPC __instance, Item item, ref int __result)
	{
        if (item is not Object obj)
			return;

		if (!item.HasContextTag("override_arch_taste") || obj.Type != "Arch")
			return;

        var tasteForItem = 0;

        var categoryNumber = obj.Category;
        var categoryNumberString = categoryNumber.ToString();
        var universalLoves = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Love"]);
        var universalHates = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Hate"]);
        var universalLikes = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Like"]);
        var universalDislikes = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Dislike"]);
        var universalNeutrals = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Neutral"]);
        if (universalLoves.Contains(categoryNumberString))
        {
            tasteForItem = 0;
        }
        else if (universalHates.Contains(categoryNumberString))
        {
            tasteForItem = 6;
        }
        else if (universalLikes.Contains(categoryNumberString))
        {
            tasteForItem = 2;
        }
        else if (universalDislikes.Contains(categoryNumberString))
        {
            tasteForItem = 4;
        }
        if (__instance.CheckTasteContextTags(obj, universalLoves))
        {
            tasteForItem = 0;
        }
        else if (__instance.CheckTasteContextTags(obj, universalHates))
        {
            tasteForItem = 6;
        }
        else if (__instance.CheckTasteContextTags(obj, universalLikes))
        {
            tasteForItem = 2;
        }
        else if (__instance.CheckTasteContextTags(obj, universalDislikes))
        {
            tasteForItem = 4;
        }
        var wasIndividualUniversal = false;
        var skipDefaultValueRules = false;
        if (__instance.CheckTaste(universalLoves, obj))
        {
            tasteForItem = 0;
            wasIndividualUniversal = true;
        }
        else if (__instance.CheckTaste(universalHates, obj))
        {
            tasteForItem = 6;
            wasIndividualUniversal = true;
        }
        else if (__instance.CheckTaste(universalLikes, obj))
        {
            tasteForItem = 2;
            wasIndividualUniversal = true;
        }
        else if (__instance.CheckTaste(universalDislikes, obj))
        {
            tasteForItem = 4;
            wasIndividualUniversal = true;
        }
        else if (__instance.CheckTaste(universalNeutrals, obj))
        {
            tasteForItem = 8;
            wasIndividualUniversal = true;
            skipDefaultValueRules = true;
        }
        if (tasteForItem == 8 && !skipDefaultValueRules)
        {
            if (obj.Edibility != -300 && obj.Edibility < 0)
            {
                tasteForItem = 6;
            }
            else if (obj.Price < 20)
            {
                tasteForItem = 4;
            }
        }
        if (Game1.NPCGiftTastes.TryGetValue(__instance.Name, out var dispositionData))
        {
            var split = dispositionData.Split('/');
            var items = new List<string[]>();
            for (var i = 0; i < 10; i += 2)
            {
                var splitItems = ArgUtility.SplitBySpace(split[i + 1]);
                var thisItems = new string[splitItems.Length];
                for (var j = 0; j < splitItems.Length; j++)
                {
                    if (splitItems[j].Length > 0)
                    {
                        thisItems[j] = splitItems[j];
                    }
                }
                items.Add(thisItems);
            }
            if (__instance.CheckTaste(items[0], obj))
            {
                __result = 0;
                return;
            }
            if (__instance.CheckTaste(items[3], obj))
            {
                __result = 6;
                return;
            }
            if (__instance.CheckTaste(items[1], obj))
            {
                __result = 2;
                return;
            }
            if (__instance.CheckTaste(items[2], obj))
            {
                __result = 4;
                return;
            }
            if (__instance.CheckTaste(items[4], obj))
            {
                __result = 8;
                return;
            }
            if (__instance.CheckTasteContextTags(obj, items[0]))
            {
                __result = 0;
                return;
            }
            if (__instance.CheckTasteContextTags(obj, items[3]))
            {
                __result = 6;
                return;
            }
            if (__instance.CheckTasteContextTags(obj, items[1]))
            {
                __result = 2;
                return;
            }
            if (__instance.CheckTasteContextTags(obj, items[2]))
            {
                __result = 4;
                return;
            }
            if (__instance.CheckTasteContextTags(obj, items[4]))
            {
                __result = 8;
                return;
            }
            if (!wasIndividualUniversal)
            {
                if (categoryNumber != 0 && items[0].Contains(categoryNumberString))
                {
                    __result = 0;
                    return;
                }
                if (categoryNumber != 0 && items[3].Contains(categoryNumberString))
                {
                    __result = 6;
                    return;
                }
                if (categoryNumber != 0 && items[1].Contains(categoryNumberString))
                {
                    __result = 2;
                    return;
                }
                if (categoryNumber != 0 && items[2].Contains(categoryNumberString))
                {
                    __result = 4;
                    return;
                }
                if (categoryNumber != 0 && items[4].Contains(categoryNumberString))
                {
                    __result = 8;
                    return;
                }
            }
        }

        __result = tasteForItem;
    }

    private static bool Pre_showTextAboveHead(ref string text, Color? spriteTextColor = null, int style = 2, int duration = 3000, int preTimer = 0)
    {
        if (!ModEntry.Config.ChangeAt)
            return true;

        Log($"Changing message: {text}", ModEntry.Config.Verbose ? LogLevel.Debug : LogLevel.Trace);

        text = text.Replace("@", Game1.player.displayName);
        return false;
    }

    internal static void Post_getHi(ref NPC __instance, string nameToGreet, ref string __result)
    {
        var greeter = __instance.Name;
        var greeted = nameToGreet;
        var characters = (greeter, greeted);

        try
        {
            //if a (thisnpc, othernpc) key exists
            if (!ModEntry.Greetings.TryGetValue(characters, out var greeting))
            {
                return;
            }

            //log, then use previous key to find value
            Log($"Found greeting patch for {__instance.Name}");
            __result = greeting;
        }
        catch (Exception ex)
        {
            Log($"Error while applying patch: {ex}", LogLevel.Error);
        }
    }
}
