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
    /// AgeWhenKidsAreModified: the age at which the CP mod will patch over the child.
    /// The Default is currently 83, which is 14 newborn/14 baby/28 crawler/28 toddler,
    /// on the assumption that CP mods will make them a child.
    /// DoChildrenWander: when true, children wander around the house every hour (unless scheduled).
    /// DoChildrenHaveCurfew: when true, children will head home at curfew time (unless already walking somewhere).
    /// CurfewTime: the time of curfew (when DoChildrenHaveCurfew is true). Default is 1900 (7PM).
    /// </summary>
    public class ModConfig {
        public int AgeWhenKidsAreModified { get; set; } = 83;
        public bool DoChildrenWander { get; set; } = true;
        public bool DoChildrenHaveCurfew { get; set; } = true;
        public int CurfewTime { get; set; } = 1900;
    }
}
