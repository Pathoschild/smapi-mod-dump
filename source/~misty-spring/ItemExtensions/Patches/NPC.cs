/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace ItemExtensions.Patches;

internal static class NpcPatches
{
    private static void Log(string msg, LogLevel lv = LogLevel.Trace) => ModEntry.Mon.Log(msg, lv);
    
    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(NpcPatches)}\": postfixing SDV method \"NPC.getGiftTasteForThisItem(Item)\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(NPC), nameof(NPC.getGiftTasteForThisItem)),
            postfix: new HarmonyMethod(typeof(NpcPatches), nameof(Post_getGiftTasteForThisItem))
            );
    }

	//if context tag has override_default, will make category smth nonexistatn
	internal static void Post_getGiftTasteForThisItem(NPC __instance, Item item, ref int __result)
	{
        try
        {   
            /*if (item is Furniture f)
            {
                __result = GetTaste(__instance, f);
                return;
            }*/

            if (item is not Object obj)
                return;

            if (!item.HasContextTag("override_arch_taste") || obj.Type != "Arch")
                return;

            __result = GetTaste(__instance, obj);
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }

    private static int GetTaste(NPC who, Object obj)
    {
        try
        {
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

            if (who.CheckTasteContextTags(obj, universalLoves))
            {
                tasteForItem = 0;
            }
            else if (who.CheckTasteContextTags(obj, universalHates))
            {
                tasteForItem = 6;
            }
            else if (who.CheckTasteContextTags(obj, universalLikes))
            {
                tasteForItem = 2;
            }
            else if (who.CheckTasteContextTags(obj, universalDislikes))
            {
                tasteForItem = 4;
            }

            var wasIndividualUniversal = false;
            var skipDefaultValueRules = false;
            if (who.CheckTaste(universalLoves, obj))
            {
                tasteForItem = 0;
                wasIndividualUniversal = true;
            }
            else if (who.CheckTaste(universalHates, obj))
            {
                tasteForItem = 6;
                wasIndividualUniversal = true;
            }
            else if (who.CheckTaste(universalLikes, obj))
            {
                tasteForItem = 2;
                wasIndividualUniversal = true;
            }
            else if (who.CheckTaste(universalDislikes, obj))
            {
                tasteForItem = 4;
                wasIndividualUniversal = true;
            }
            else if (who.CheckTaste(universalNeutrals, obj))
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

            if (Game1.NPCGiftTastes.TryGetValue(who.Name, out var dispositionData))
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

                if (who.CheckTaste(items[0], obj))
                {
                    return 0;
                }

                if (who.CheckTaste(items[3], obj))
                {
                    return 6;
                }

                if (who.CheckTaste(items[1], obj))
                {
                    return 2;
                }

                if (who.CheckTaste(items[2], obj))
                {
                    return 4;
                }

                if (who.CheckTaste(items[4], obj))
                {
                    return 8;
                }

                if (who.CheckTasteContextTags(obj, items[0]))
                {
                    return 0;
                }

                if (who.CheckTasteContextTags(obj, items[3]))
                {
                    return 6;
                }

                if (who.CheckTasteContextTags(obj, items[1]))
                {
                    return 2;
                }

                if (who.CheckTasteContextTags(obj, items[2]))
                {
                    return 4;
                }

                if (who.CheckTasteContextTags(obj, items[4]))
                {
                    return 8;
                }

                if (!wasIndividualUniversal)
                {
                    if (categoryNumber != 0 && items[0].Contains(categoryNumberString))
                    {
                        return 0;
                    }

                    if (categoryNumber != 0 && items[3].Contains(categoryNumberString))
                    {
                        return 6;
                    }

                    if (categoryNumber != 0 && items[1].Contains(categoryNumberString))
                    {
                        return 2;
                    }

                    if (categoryNumber != 0 && items[2].Contains(categoryNumberString))
                    {
                        return 4;
                    }

                    if (categoryNumber != 0 && items[4].Contains(categoryNumberString))
                    {
                        return 8;
                    }
                }
            }
            return tasteForItem;
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
            return 0;
        }
    }
}