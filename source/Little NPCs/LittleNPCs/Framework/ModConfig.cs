/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/LittleNPCs
**
*************************************************/


namespace LittleNPCs.Framework {
    /// <summary>
    /// ModConfig Options:
    /// AgeWhenKidsAreModified: The age in days when a child is replaced by a LittleNPC. Default is 83 days.
    /// DoChildrenWander: If true, children wander around the house every hour unless they have a schedule.
    /// DoChildrenHaveCurfew: If true, children will head home at curfew time.
    /// CurfewTime: The time of curfew when DoChildrenHaveCurfew is true. Default is 1900 (7PM).
    /// DoChildrenVisitVolcanoIsland: Children visit Volcano Island by chance. Default is false.
    /// </summary>
    public class ModConfig {
        public int AgeWhenKidsAreModified { get; set; } = 83;
        public bool DoChildrenWander { get; set; } = true;
        public bool DoChildrenHaveCurfew { get; set; } = true;
        public int CurfewTime { get; set; } = 1900;
        public bool DoChildrenVisitVolcanoIsland { get; set; } = false;
    }
}
