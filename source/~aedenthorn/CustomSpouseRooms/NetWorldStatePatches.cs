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
using StardewValley;

namespace CustomSpouseRooms
{
    public partial class ModEntry
    {
        public static bool hasWorldStateID_Prefix(ref string id, ref bool __result)
        {
            if (!Config.EnableMod)
                return true;
            if(id == "sebastianFrogReal")
            {
                SMonitor.Log($"Allowing frogs");
                id = "sebastianFrog";
                return true;
            }
            if(id == "sebastianFrog")
            {
                SMonitor.Log($"Preventing frogs");
                __result = false;
                return false;
            }
            return true;
        }
    }
}
