using System.Collections.Generic;

namespace ChildToNPC
{
    /* ModConfig Options:
     * AgeWhenKidsAreModified: the age at which the CP mod will patch over the child.
     *      The Default is currently 83, which is 14 newborn/14 baby/28 crawler/28 toddler,
     *      on the assumption that CP mods will make them a child.
     * DoChildrenWander: when true, children wander around the house every hour (unless scheduled.)
     * DoChildrenHaveCurfew: when true, children will head home at curfew time (unless already walking somewhere.)
     * CurfewTime: the time of curfew (when DoChildrenHaveCurfew is true).
     * ChildParentPairs: A pair of names, the name of a child and the name of their NPC parent, to be used by CP mods.
     *      This allows you to have children with parents who aren't your spouse.
     * ModdingCommands: when true, adds commands which make it easier to generate new test children.
     */
    class ModConfig
    {
        public int AgeWhenKidsAreModified { get; set; }
        public bool DoChildrenWander { get; set; }
        public bool DoChildrenHaveCurfew { get; set; }
        public int CurfewTime { get; set; }
        public Dictionary<string, string> ChildParentPairs { get; set; }
        public bool ModdingCommands { get; set; }

        public ModConfig()
        {
            AgeWhenKidsAreModified = 83;
            DoChildrenWander = true;
            DoChildrenHaveCurfew = true;
            CurfewTime = 1900; //7 pm
            ChildParentPairs = new Dictionary<string, string>();
            ModdingCommands = false;
        }
    }
}