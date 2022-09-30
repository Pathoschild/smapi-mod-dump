/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Rafseazz/Ridgeside-Village-Mod
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;

namespace RidgesideVillage
{

    internal static class SODialogue
    {
        private static IMonitor Monitor { get; set; }
        private static IModHelper Helper { get; set; }

        public static void ApplyPatch(Harmony harmony, IModHelper helper)
        {
            Helper = helper;
            
            harmony.Patch(
                original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.OnFail)),
                prefix: new HarmonyMethod(typeof(SODialogue), nameof(SODialogue.SpecialOrder_OnFail_prefix)));
        }

        // Don't put the delivered items in the lost and found
        private static bool SpecialOrder_OnFail_prefix(ref SpecialOrder __instance)
        {
            try
            {
                if (__instance.questKey.Value != RSVConstants.SO_PIKAQUEST)
                {
                    return true;
                }
                foreach (OrderObjective objective in __instance.objectives)
                {
                    objective.OnFail();
                }
                if (Game1.IsMasterGame)
                {
                    __instance.HostHandleQuestEnd();
                }
                __instance.questState.Value = SpecialOrder.QuestState.Failed;
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"RSV: Error prefixing SpecialOrder.OnFail:\n\n{ex}");
            }
            return true;

        }

    }
}
