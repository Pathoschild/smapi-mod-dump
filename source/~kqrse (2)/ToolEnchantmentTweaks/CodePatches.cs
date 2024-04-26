/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley;
using StardewValley.GameData;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Quests;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;
using Object = StardewValley.Object;

namespace ToolEnchantmentTweaks
{
    public partial class ModEntry
    {
        [HarmonyPatch(typeof(SwiftToolEnchantment), nameof(SwiftToolEnchantment.CanApplyTo))]
        public class SwiftToolEnchantment_CanApplyTo_Patch
        {
            public static bool Prefix(Item item, ref bool __result)
            {
                if (!Config.ModEnabled)
                    return true;

                if(item is WateringCan)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

    }
}