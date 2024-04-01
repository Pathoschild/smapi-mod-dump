/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MoonMisadventures.VirtualProperties;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Network;

namespace MoonMisadventures.Patches
{

    [HarmonyPatch( typeof( Farmer ), "farmerInit" )]
    public static class FarmerInjectNetFieldsPatch
    {
        public static void Postfix( Farmer __instance )
        {
            __instance.NetFields.AddField( __instance.get_necklaceItem(), "necklaceItem" );
        }
    }

    [HarmonyPatch( typeof( FarmerTeam ), MethodType.Constructor )]
    public static class FarmerTeamInjectNetFieldsPatch
    {
        public static void Postfix( FarmerTeam __instance )
        {
            __instance.NetFields.AddField( __instance.get_hasLunarKey(), "hasLunarKey" );
        }
    }

    [HarmonyPatch( typeof( Monster ), "initNetFields" )]
    public static class MonsterShockedFieldPatch
    {
        public static void Postfix( Monster __instance )
        {
            __instance.NetFields.AddField( __instance.get_shocked(), "shocked" );
        }
    }
}
