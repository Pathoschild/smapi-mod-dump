/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace CropsSurviveSeasonChange
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool IncludeRegrowables { get; set; } = false;
        public bool IncludeWinter { get; set; } = false;
    }
}
