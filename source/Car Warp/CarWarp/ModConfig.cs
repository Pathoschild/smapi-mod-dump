/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

namespace CarWarp;

class ModConfig
{
    public string Configuration { get; set; }
    public bool SeasonalOverlay { get; set; }

    public ModConfig()
    {
        // options:
        // Right - steering wheel on right side
        // Left - steering wheel on left side
        // None - no steering wheel, only dashboard
        // Empty - no dashboard

        Configuration = "Right";
        SeasonalOverlay = true;
    }
}
