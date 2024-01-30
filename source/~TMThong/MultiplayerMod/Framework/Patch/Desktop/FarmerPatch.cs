/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerMod.Framework.Patch.Desktop
{
    internal class FarmerPatch : IPatch
    {

        private static readonly Type TYPE_PATCH = typeof(Farmer);

        public void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.PropertyGetter(TYPE_PATCH, "ActiveObject"), prefix: new HarmonyMethod(this.GetType(), nameof(prefix_ActiveObject)));
            harmony.Patch(AccessTools.PropertyGetter(TYPE_PATCH, "CurrentItem"), prefix: new HarmonyMethod(this.GetType(), nameof(prefix_CurrentItem)));
        }


        
        public static bool prefix_CurrentItem(Farmer __instance, ref Item __result)
        {
            int CurrentToolIndex = __instance.CurrentToolIndex;

            var cloneList = __instance.Items.ToArray();

            if (__instance.TemporaryItem != null)
            {
                __result = __instance.TemporaryItem;
                return false;
            }

            if (__instance.netItemStowed)
            {
                __result = null;
                return false;
            }

            if (CurrentToolIndex >= cloneList.Length || CurrentToolIndex < 0)
            {
                __result = null;
                return false;
            }

            __result = cloneList[CurrentToolIndex]; 

            return false;
        }

        public static bool prefix_ActiveObject(Farmer __instance, ref StardewValley.Object __result)
        {
            int CurrentToolIndex = __instance.CurrentToolIndex;

            var cloneList = __instance.Items.ToArray();

            if (__instance.TemporaryItem != null)
            {
                if (__instance.TemporaryItem is StardewValley.Object)
                {
                    __result = (StardewValley.Object)__instance.TemporaryItem;
                    return false;
                }
                __result = null;
                return false;
            }

            if (__instance.netItemStowed)
            {
                __result = null;
                return false;
            }

            if (CurrentToolIndex < cloneList.Length && CurrentToolIndex >= 0)
            {
                if(cloneList[CurrentToolIndex] != null && cloneList[CurrentToolIndex] is StardewValley.Object)
                {
                    __result = (StardewValley.Object)cloneList[CurrentToolIndex];
                    return false;
                }
                __result = null;
            }
            else
            {
                __result = null;
            }
            return false;
        }
    }
}
