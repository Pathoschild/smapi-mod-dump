/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Circuit.Events;
using Circuit.VirtualProperties;
using HarmonyLib;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(NPC), nameof(NPC.dayUpdate))]
    internal class NPCGetSchedulePatch
    {
        private HashSet<string> MappedNpcs { get; } = new();

        private static void ResetPortrait(NPC who)
        {
            bool isWearingIslandAttire = (bool)who.GetType().GetField("isWearingIslandAttire", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(who)!;

            who.Portrait = null;
            try
            {
                string portraitPath = ((!string.IsNullOrEmpty(who.syncedPortraitPath.Value)) ? (who.syncedPortraitPath.Value) : ("Portraits\\" + who.getTextureName()));
                if (isWearingIslandAttire)
                {
                    try
                    {
                        who.Portrait = Game1.content.Load<Texture2D>(portraitPath + "_Beach");
                    }
                    catch (ContentLoadException)
                    {
                        who.Portrait = null;
                    }
                }

                who.Portrait ??= Game1.content.Load<Texture2D>(portraitPath);
            }
            catch (ContentLoadException)
            {
                who.Portrait = null;
            }
        }

        public static bool Prefix(NPC __instance)
        {
            if (!ModEntry.ShouldPatch(EventType.ChaoticScheduling) || !__instance.isVillager())
                return true;

            ChaoticScheduling evt = (ChaoticScheduling)EventManager.GetCurrentEvent()!;
            if (evt.SecondsRemaining <= 0)
            {
                evt.UnswapNpc(__instance);
                return true;
            }

            evt.SwapNpc(__instance);
            return true;
        }

        public static void Postfix(NPC __instance, string __state)
        {
            if (__instance.get_NPCIsSwapped())
            {
                __instance.reloadSprite();
                ResetPortrait(__instance);
            }
        }
    }
}
