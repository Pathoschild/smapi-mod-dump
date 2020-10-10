/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using StardewModdingAPI.Utilities;
using System;
using TwilightShards.Common;
using TwilightShards.Stardew.Common;

namespace DynamicNightTime.Patches
{
    class GetFullyDarkPatch
    {
        public static void Postfix(ref int __result)
        {
                SDVTime calcTime = DynamicNightTime.GetNavalTwilight();
                calcTime.ClampToTenMinutes();

                __result = calcTime.ReturnIntTime();
        }
    }
}

