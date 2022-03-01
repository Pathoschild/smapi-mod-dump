/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Object = StardewValley.Object;

namespace AdvancedDrumBlocks
{
    public partial class ModEntry
    {
        public static bool Game1_pressSwitchToolButton_Prefix()
        {
            return !Config.EnableMod || (!SHelper.Input.IsDown(Config.IndexModKey) || !Game1.currentLocation.objects.TryGetValue(Game1.currentCursorTile, out Object obj) || !obj.Name.Equals("Drum Block"));
        }

    }
}