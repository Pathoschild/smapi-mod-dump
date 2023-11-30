/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BinaryLip/ScheduleViewer
**
*************************************************/

using StardewModdingAPI;

namespace ScheduleViewer
{
    internal class ModConfig
    {
        public SButton ShowSchedulesKey { get; set; } = SButton.V;
        public string SortOrder { get; set; } = ModEntry.SortOrderOptions[0];
    }
}
