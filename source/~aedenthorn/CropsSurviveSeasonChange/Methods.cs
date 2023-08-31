/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace CropsSurviveSeasonChange
{
    public partial class ModEntry
    {
        private static bool CheckKill(bool outdoors, Crop crop, GameLocation environment)
        {
            if (!Config.ModEnabled || crop.forageCrop.Value || crop.dead.Value || (!Config.IncludeRegrowables && crop.regrowAfterHarvest.Value != -1) || (environment.GetSeasonForLocation() == "winter" && !Config.IncludeWinter))
            {
                return outdoors;
            }
            return false;
        }
    }
}