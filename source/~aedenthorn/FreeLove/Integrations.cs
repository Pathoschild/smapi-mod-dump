/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using System;

namespace FreeLove
{
    public partial class ModEntry
    {
        public static IKissingAPI kissingAPI;
        public static IBedTweaksAPI bedTweaksAPI;
        public static IChildrenTweaksAPI childrenAPI;
        public static ICustomSpouseRoomsAPI customSpouseRoomsAPI;

        public static void LoadModApis()
        {
            kissingAPI = SHelper.ModRegistry.GetApi<IKissingAPI>("aedenthorn.HugsAndKisses");
            bedTweaksAPI = SHelper.ModRegistry.GetApi<IBedTweaksAPI>("aedenthorn.BedTweaks");
            childrenAPI = SHelper.ModRegistry.GetApi<IChildrenTweaksAPI>("aedenthorn.ChildrenTweaks");
            customSpouseRoomsAPI = SHelper.ModRegistry.GetApi<ICustomSpouseRoomsAPI>("aedenthorn.CustomSpouseRooms");

            if (kissingAPI != null)
            {
                SMonitor.Log("Kissing API loaded");
            }
            if (bedTweaksAPI != null)
            {
                SMonitor.Log("BedTweaks API loaded");
            }
            if (childrenAPI != null)
            {
                SMonitor.Log("ChildrenTweaks API loaded");
            }
            if (customSpouseRoomsAPI != null)
            {
                SMonitor.Log("CustomSpouseRooms API loaded");
            }
        }
    }
}