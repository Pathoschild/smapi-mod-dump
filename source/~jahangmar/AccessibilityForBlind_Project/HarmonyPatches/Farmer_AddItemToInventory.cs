/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jahangmar/StardewValleyMods
**
*************************************************/

// Copyright (c) 2020 Jahangmar
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
using System;
using StardewValley;
using Harmony;
using System.Threading;
using System.Runtime.CompilerServices;

namespace AccessibilityForBlind.HarmonyPatches
{
    public class Farmer_AddItemToInventory
    {
        public static void Patch(HarmonyInstance harmony)
        {
            //AccessTools.Method(typeof(StardewValley.Farmer), "addItemToInventory", new[] { typeof(Item), typeof(int) });
            harmony.Patch(AccessTools.Method(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.addItemToInventory), new Type[] { typeof(Item), typeof(int) }),
                new HarmonyMethod(AccessTools.Method(typeof(Farmer_AddItemToInventory), nameof(addItemToInventory_prefix))));
            harmony.Patch(AccessTools.Method(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.addItemToInventory), new Type[] { typeof(Item) }),
                new HarmonyMethod(AccessTools.Method(typeof(Farmer_AddItemToInventory), nameof(addItemToInventory_prefix))));

            harmony.Patch(AccessTools.Method(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.addItemToInventoryBool)),
                new HarmonyMethod(AccessTools.Method(typeof(Farmer_AddItemToInventory), nameof(addItemToInventoryBool_prefix))));
        }

        public static void Speak(Item item)
        {
            if (item != null)
                TextToSpeech.Speak(Game1.player.Name + " receives " + item.Stack + " " + item.DisplayName);
        }

        public static bool addItemToInventory_prefix(Item item, int position)
        {
            Speak(item);
            return true;
        }

        public static bool addItemToInventory_prefix(Item item)
        {
            Speak(item);
            return true;
        }

        public static bool addItemToInventoryBool_prefix(Item item, bool makeActiveObject = false)
        {
            Speak(item);
            return true;
        }
    }
}
