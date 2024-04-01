/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using Object = StardewValley.Object;

namespace GiftRejection
{
    public partial class ModEntry
    {
        [HarmonyPatch(typeof(NPC), nameof(NPC.receiveGift))]
        public class NPC_receiveGift_Patch
        {
            public static void Prefix(NPC __instance, Object o)
            {
                if (!Config.ModEnabled)
                    return;
                int tasteForItem = __instance.getGiftTasteForThisItem(o);
                if(tasteForItem == 6 && Config.RejectHated)
                {
                    ThrowObject(o, __instance, Config.HatedThrowDistance);
                }
                else if(tasteForItem == 4 && Config.RejectDisliked)
                {
                    ThrowObject(o, __instance, Config.DislikedThrowDistance);
                }
            }
        }
    }
}