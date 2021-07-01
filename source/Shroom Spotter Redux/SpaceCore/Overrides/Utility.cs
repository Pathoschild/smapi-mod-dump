/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using SpaceCore.Events;
using StardewValley.Events;

namespace SpaceCore.Overrides
{
    public class NightlyFarmEventHook
    {
        public static void Postfix( ref FarmEvent __result )
        {
            __result = SpaceEvents.InvokeChooseNightlyFarmEvent( __result );
        }
    }
}
